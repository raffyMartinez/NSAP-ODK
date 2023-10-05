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
        public static string ServerID { get; set; }
        public static bool IsMultiVessel { get; set; }




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

                            //v unload
                            cmd.CommandText = @"DELETE  dbo_vessel_unload.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel = @is_mv";
                            r = cmd.ExecuteNonQuery();

                            //g unload
                            cmd.CommandText = @"DELETE dbo_gear_unload.*
                                                FROM dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();

                            //landing site sampling json submission IDs
                            if (isMultivessel)
                            {
                                cmd.CommandText = "Delete * from dbo_lss_submissionIDs";
                                r = cmd.ExecuteNonQuery();
                            }

                            //lss 1 and lss
                            cmd.CommandText = @"DELETE  dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.is_multivessel=@is_mv";
                            r = cmd.ExecuteNonQuery();


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

                            //v unload
                            cmd.CommandText = @"DELETE  dbo_vessel_unload.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();

                            //g unload
                            cmd.CommandText = @"DELETE dbo_gear_unload.*
                                                FROM dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();

                            //landing site sampling json submission IDs
                            cmd.CommandText = "Delete * from dbo_lss_submissionIDs WHERE xFormIdentifier=@id";
                            r = cmd.ExecuteNonQuery();


                            //lss 1 and lss
                            cmd.CommandText = @"DELETE  dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();


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

    }
}
