using DocumentFormat.OpenXml.Bibliography;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public enum CatchAndEffortFormType
    {
        FormTypeNone,
        FormTypeMultiVessel,
        FormTypeNotMultiVessel
    }
    public static class DeleteServerData
    {
        public static event EventHandler<DeleteFromServerEventArg> DeletingServerDataEvent;
        public static string ServerID { get; set; }
        public static bool IsMultiVessel { get; set; }



        public static Task<bool> ClearNSAPDatabaseTablesAsync()
        {
            return Task.Run(() => ClearNSAPDatabaseTables());
        }

        /// <summary>
        /// CLears database mdb catch and effort entities table
        /// </summary>
        /// <param name="otherConnectionString"></param>
        /// <returns></returns>
        private static bool ClearNSAPDatabaseTables(string otherConnectionString = "")
        {
            int processedCount = 1;
            bool success = false;
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "start deleting", CountToProcess = 17 });
            
            UnmatchedFieldsFromJSONFileRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "unmatched fields from json" });
            
            JSONFileRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "json file repository" });
            
            CatchMaturityRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "maturity" });
            
            CatchLengthRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length" });
            
            CatchLenWeightRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length-weight" });
            
            CatchLenFreqRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length freq" });
            
            VesselCatchRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "catch" });
            
            FishingGroundGridRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "fishing ground grid" });
            
            GearSoakRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear soak" });
            
            VesselEffortRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "effort" });
            
            VesselUnload_Gear_Spec_Repository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear gear spec" });
            
            VesselUnload_FishingGearRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "unload fishihg gear" });
            
            VesselUnloadRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload" });
            
            TotalWtSpRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "total wt" });
            
            GearUnloadRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear unload" });
            
            LandingSiteSamplingSubmissionRepository.ClearTable(otherConnectionString);
            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "lss submission" });
            
            if (LandingSiteSamplingRepository.ClearTable(otherConnectionString))
            {
                success = true;
                DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "sampling day" });
            }

            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "finished deleting" });
            return success;
        }
        public static Task<bool> DeleteServerDataByServerIDAsync()
        {
            return Task.Run(() => DeleteServerDataByServerID());
        }

        public static Task<bool> DeleteServerDataByTypeAsync(bool isMultivessel)
        {
            return Task.Run(() => DeleteServerDataByType(isMultivessel));
        }
        private static bool DeleteServerDataByType(bool isMultivessel)
        {
            int processedCount = 1;
            bool success = false;
            try
            {
                if (Global.Settings.UsemySQL)
                {

                }
                else
                {
                    //OleDbTransaction transaction = null;
                    using (var con = new OleDbConnection(Global.ConnectionString))
                    {
                        DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "start deleting", CountToProcess = 25 });


                        using (var cmd = con.CreateCommand())
                        {
                            int r = 0;
                            cmd.Parameters.AddWithValue("@is_mv", isMultivessel);
                            con.Open();
                            //transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            //cmd.Transaction = transaction;

                            //Length not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_len ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length" });

                            //Length multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_len ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length" });

                            //Length Freq not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_freq.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_len_freq ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length freq" });

                            //Length Freq multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_freq.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_len_freq ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length freq" });

                            //Length Wt not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_wt.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_len_wt ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length weight" });

                            //Length Wt multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_wt.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_len_wt ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length weight" });

                            //Maturity not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_maturity.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_maturity ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "maturity" });

                            //Maturity multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_maturity.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_maturity ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "maturity" });

                            //Catch not multi-gear
                            cmd.CommandText = @"DELETE  dbo_vessel_catch.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "catch composition" });

                            //Catch multi-gear
                            cmd.CommandText = @"DELETE  dbo_vessel_catch.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "catch composition" });

                            //effort multi-gear
                            cmd.CommandText = @"DELETE  dbo_vessel_effort.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_effort ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_effort.vessel_unload_fishing_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "effort indicators" });

                            //weight validation multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_unload_weight_validation.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_unload_weight_validation ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_unload_weight_validation.unload_gear
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "weight validation" });

                            //unload stats multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_unload_stats.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                    dbo_vessel_unload_stats ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_unload_stats.unload_gear
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "unload stats" });

                            //vesselunload gear multigear
                            cmd.CommandText = @"DELETE  dbo_vesselunload_fishinggear.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload gears" });

                            //effort not multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_effort.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_effort ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_effort.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "effort" });

                            //weight validation not multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_unload_weight_validation.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_unload_weight_validation ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_weight_validation.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "weight validation" });

                            //unload stats not multigear
                            cmd.CommandText = @"DELETE dbo_vessel_unload_stats.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_unload_stats ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_stats.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "unload stats" });

                            //fishing grid
                            cmd.CommandText = @"DELETE dbo_fg_grid.*
                                                FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                    dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_fg_grid ON dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "fishing grid" });

                            //gear soak
                            cmd.CommandText = @"DELETE dbo_gear_soak.*
                                                FROM((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                    dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_gear_soak ON dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear soak" });

                            //v unload 1
                            cmd.CommandText = @"DELETE dbo_vessel_unload_1.*
                                                FROM ((dbo_gear_unload INNER JOIN       
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_unload_1 ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload 1" });

                            //v unload
                            cmd.CommandText = @"DELETE  dbo_vessel_unload.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload" });

                            //g unload
                            cmd.CommandText = @"DELETE dbo_gear_unload.*
                                                FROM dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear unload" });

                            //landing site sampling json submission IDs
                            if (isMultivessel)
                            {
                                cmd.CommandText = "Delete * from dbo_lss_submissionIDs";
                                r = cmd.ExecuteNonQuery();
                            }
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "submission ids" });

                            //lss 1 and lss
                            cmd.CommandText = @"DELETE  dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "sampling day" });


                            //delete sampling day with orphaned landing site
                            success = LandingSiteSamplingRepository.DeleteSamplingWithOrphanedLandingSite();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "sampling day with orphaned landing site" });


                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "finished deleting" });

                            //transaction.Commit();
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return success;
        }

        private static bool DeleteServerDataByServerID()
        {
            int processedCount = 1;
            bool success = false;
            try
            {
                if (Global.Settings.UsemySQL)
                {

                }
                else
                {
                    //OleDbTransaction transaction = null;
                    using (var con = new OleDbConnection(Global.ConnectionString))
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            int r = 0;

                            cmd.Parameters.AddWithValue("@id", ServerID);
                            con.Open();
                            //transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            //cmd.Transaction = transaction;

                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "start deleting", CountToProcess = 26 });

                            //update xform ID for data saved in earlier versions of eForm
                            cmd.CommandText = @"UPDATE ((dbo_gear_unload INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_unload_1 ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id 
                                                        SET dbo_LC_FG_sample_day_1.XFormIdentifier = @id
                                                    WHERE dbo_vessel_unload_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            success = r >= 0;
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload 1 xform identifier" });

                            //Length not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_len ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length" });

                            //Length multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_len ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length" });

                            //Length Freq not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_freq.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_len_freq ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length freq" });

                            //Length Freq multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_freq.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_len_freq ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length freq" });

                            //Length Wt not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_wt.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_len_wt ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length weight" });

                            //Length Wt multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_len_wt.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_len_wt ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "length weight" });

                            //Maturity not multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_maturity.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) INNER JOIN 
                                                    dbo_catch_maturity ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "maturity" });

                            //Maturity multi-gear landing
                            cmd.CommandText = @"DELETE dbo_catch_maturity.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                    dbo_catch_maturity ON 
                                                    dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "maturity" });

                            //Catch not multi-gear
                            cmd.CommandText = @"DELETE  dbo_vessel_catch.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "catch composition" });

                            //Catch multi-gear
                            cmd.CommandText = @"DELETE  dbo_vessel_catch.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "catch composition" });

                            //effort multi-gear
                            cmd.CommandText = @"DELETE  dbo_vessel_effort.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_effort ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_effort.vessel_unload_fishing_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "effort indicators" });
                            //weight validation multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_unload_weight_validation.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_unload_weight_validation ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_unload_weight_validation.unload_gear
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "weight validation" });

                            //unload stats multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_unload_stats.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                    dbo_vessel_unload_stats ON 
                                                    dbo_vesselunload_fishinggear.row_id = dbo_vessel_unload_stats.unload_gear
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "unload stats" });

                            //vesselunload gear multigear
                            cmd.CommandText = @"DELETE  dbo_vesselunload_fishinggear.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload gears" });

                            //effort not multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_effort.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_effort ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_effort.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "effort indicators" });

                            //weight validation not multigear
                            cmd.CommandText = @"DELETE  dbo_vessel_unload_weight_validation.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_unload_weight_validation ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_weight_validation.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "weight validation" });

                            //unload stats not multigear
                            cmd.CommandText = @"DELETE dbo_vessel_unload_stats.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_unload_stats ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_stats.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "unload stats" });

                            //fishing grid
                            cmd.CommandText = @"DELETE dbo_fg_grid.*
                                                FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                    dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_fg_grid ON dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "fishing grid" });

                            //gear soak
                            cmd.CommandText = @"DELETE dbo_gear_soak.*
                                                FROM((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                    dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_gear_soak ON dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear soak" });

                            //v unload 1
                            cmd.CommandText = @"DELETE dbo_vessel_unload_1.*
                                                FROM ((dbo_gear_unload INNER JOIN       
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_unload_1 ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload 1" });

                            //v unload
                            cmd.CommandText = @"DELETE  dbo_vessel_unload.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "vessel unload" });

                            //g unload
                            cmd.CommandText = @"DELETE dbo_gear_unload.*
                                                FROM dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "gear unload" });

                            //landing site sampling json submission IDs
                            cmd.CommandText = "Delete * from dbo_lss_submissionIDs WHERE xFormIdentifier=@id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "submission ids" });


                            //lss 1 and lss
                            cmd.CommandText = @"DELETE  dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "sampling day" });

                            //delete sampling day with orphaned landing site
                            success = LandingSiteSamplingRepository.DeleteSamplingWithOrphanedLandingSite();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "deleted from table", CountProcessed = processedCount++, TableName = "sampling day with orphaned landing site" });

                            //transaction.Commit();
                            DeletingServerDataEvent?.Invoke(null, new DeleteFromServerEventArg { Intent = "finished deleting" });
                            success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return success;
        }

    }
}
