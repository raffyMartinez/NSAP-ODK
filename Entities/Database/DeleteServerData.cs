using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public static class DeleteServerData
    {
        public static string ServerID { get; set; }
        public static bool IsMultiVessel { get; set; }


        public static Task<bool> DeleteAsync()
        {
            return Task.Run(() => Delete());
        }
        public static bool Delete()
        {
            bool success = false;

            if (IsMultiVessel)
            {
                success = DeleteFromMultivesselTables();
            }
            else
            {
                success = DeleteFromNonMultiVesselTables();
            }

            //if (success)
            //{
            //    success = DeleteFromCommonTables();
            //}
            return success;
        }

        private static bool DeleteFromCommonTables()

        {
            int r = 0;
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@id", ServerID);
                    try
                    {
                        ////delete soak time
                        //cmd.CommandText = @"DELETE dbo_gear_soak.*
                        //                        FROM (dbo_gear_unload INNER JOIN 
                        //                        (dbo_vessel_unload INNER JOIN 
                        //                        dbo_gear_soak ON 
                        //                        dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id) ON 
                        //                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                        //                        dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                        //                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier= @id";
                        //r = cmd.ExecuteNonQuery();
                        //success = r >= 0;
                        ////success = cmd.ExecuteNonQuery() >= 0;

                        ////delete grids
                        //if (success)
                        //{
                        //    cmd.CommandText = @"DELETE  dbo_fg_grid.*
                        //                        FROM ((dbo_gear_unload INNER JOIN 
                        //                            dbo_vessel_unload ON 
                        //                            dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                        //                            dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                        //                            dbo_fg_grid ON dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id
                        //                        WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                        //    r = cmd.ExecuteNonQuery();
                        //    success = r >= 0;
                        //    //success = cmd.ExecuteNonQuery() >= 0;
                        //}

                        //delete vessel unload 1
                        //if (success)
                        //{
                        cmd.CommandText = @"DELETE dbo_vessel_unload_1.*
                                        FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                            dbo_gear_unload ON dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                            dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                            dbo_vessel_unload_1 ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                        WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                        r = cmd.ExecuteNonQuery();
                        success = r >= 0;
                        //success = cmd.ExecuteNonQuery() > 0;
                        //}

                        //delete vessel unload 
                        if (success)
                        {
                            cmd.CommandText = @"DELETE dbo_vessel_unload.*
                                            FROM (dbo_LC_FG_sample_day_1 INNER JOIN 
                                                dbo_gear_unload ON dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                            r = cmd.ExecuteNonQuery();
                            success = r >= 0;
                            //success = cmd.ExecuteNonQuery() > 0;
                        }
                        //delete gear unload
                        if (success)
                        {
                            cmd.CommandText = @"DELETE  dbo_gear_unload.*
                                            FROM dbo_gear_unload INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                            r = cmd.ExecuteNonQuery();
                            success = r >= 0;
                            //success = cmd.ExecuteNonQuery() > 0;
                        }

                        //delete landing site sampling
                        if (success)
                        {
                            //cmd.CommandText = "DELETE  * FROM dbo_LC_FG_sample_day_1 WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                            cmd.CommandText = @"DELETE dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                            r = cmd.ExecuteNonQuery();
                            success = r >= 0;
                            //success = cmd.ExecuteNonQuery() > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        private static bool DeleteFromMultivesselTables()
        {
            bool success = false;
            int r = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@id", ServerID);
                        con.Open();

                        try
                        {
                            //delete length data
                            cmd.CommandText = cmd.CommandText = @"DELETE dbo_catch_len.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    (dbo_vessel_unload INNER JOIN 
                                                    (dbo_vesselunload_fishinggear INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_len ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON 
                                                        dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                            dbo_LC_FG_sample_day_1 ON 
                                                            dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                            r = cmd.ExecuteNonQuery();
                            success = r >= 0;
                            //success = cmd.ExecuteNonQuery() >= 0;

                            //delete lenght weight data
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_catch_len_wt.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    (dbo_vessel_unload INNER JOIN 
                                                    (dbo_vesselunload_fishinggear INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_len_wt ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) ON 
                                                        dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                            dbo_LC_FG_sample_day_1 ON 
                                                            dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete len-freq
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_catch_len_freq.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    (dbo_vessel_unload INNER JOIN 
                                                    (dbo_vesselunload_fishinggear INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_len_freq ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON 
                                                        dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                            dbo_LC_FG_sample_day_1 ON 
                                                            dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete maturity
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_catch_maturity.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    (dbo_vessel_unload INNER JOIN 
                                                    (dbo_vesselunload_fishinggear INNER JOIN 
                                                    (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_maturity ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON 
                                                        dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                            dbo_LC_FG_sample_day_1 ON 
                                                            dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete catch composition
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_catch.*
                                                FROM (((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_catch ON dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete effort
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_effort.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    (dbo_vesselunload_fishinggear INNER JOIN 
                                                    dbo_vessel_effort ON dbo_vesselunload_fishinggear.row_id = dbo_vessel_effort.vessel_unload_fishing_gear_id) ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }
                            //delete weight validation
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload_weight_validation.*
                                                FROM dbo_vessel_unload_weight_validation INNER JOIN 
                                                    (((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                    dbo_gear_unload ON dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON 
                                                    dbo_vessel_unload_weight_validation.unload_gear = dbo_vesselunload_fishinggear.row_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }


                            //delete unload stats
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload_stats.*
                                                FROM dbo_vessel_unload_stats INNER JOIN 
                                                    (((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                    dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) ON 
                                                    dbo_vessel_unload_stats.unload_gear = dbo_vesselunload_fishinggear.row_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;

                            }

                            //delete vessel unload gear
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vesselunload_fishinggear.*
                                            FROM ((dbo_gear_unload INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON 
                                                dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                dbo_vesselunload_fishinggear ON dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }


                            //delete landingsite sampling submission IDs for multivessel JSON files
                            if (success)
                            {
                                cmd.CommandText = "DELETE * FROM dbo_lss_submissionIDs WHERE xFormIdentifier = @id ";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }

                            //delete gear soak
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_gear_soak.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                (dbo_vessel_unload INNER JOIN 
                                                dbo_gear_soak ON 
                                                dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id) ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier= @id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;

                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete grids
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_fg_grid.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_fg_grid ON dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete vessel unload 1
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload_1.*
                                        FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                            dbo_gear_unload ON dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                            dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                            dbo_vessel_unload_1 ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                        WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;

                                //success = cmd.ExecuteNonQuery() > 0;
                            }

                            //delete vessel unload 
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload.*
                                            FROM (dbo_LC_FG_sample_day_1 INNER JOIN 
                                                dbo_gear_unload ON dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }
                            //delete gear unload
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_gear_unload.*
                                            FROM dbo_gear_unload INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }

                            //delete landing site sampling
                            if (success)
                            {
                                //cmd.CommandText = "DELETE  * FROM dbo_LC_FG_sample_day_1 WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                cmd.CommandText = @"DELETE dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }

                    }
                }
            }
            return success;
        }
        private static bool DeleteFromNonMultiVesselTables()
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@id", ServerID);
                        con.Open();
                        int r = 0;
                        try
                        {
                            //update blank xformID field in LSS1 table from xformID in unload1 table

                            cmd.CommandText = @"UPDATE ((dbo_gear_unload INNER JOIN dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_unload_1 ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id SET dbo_LC_FG_sample_day_1.XFormIdentifier = @id
                                                    WHERE dbo_vessel_unload_1.XFormIdentifier = @id";
                            r = cmd.ExecuteNonQuery();
                            success = r >= 0;

                            //delete length
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_catch_len.*
                                                    FROM (dbo_gear_unload INNER JOIN 
                                                        (dbo_vessel_unload INNER JOIN 
                                                        (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_len ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id;";
                                success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete len-wt
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_catch_len_wt.*
                                                    FROM (dbo_gear_unload INNER JOIN 
                                                        (dbo_vessel_unload INNER JOIN 
                                                        (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_len_wt ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id;";

                                success = cmd.ExecuteNonQuery() >= 0;

                            }

                            //delete len-freq
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_catch_len_freq.*
                                                    FROM (dbo_gear_unload INNER JOIN 
                                                        (dbo_vessel_unload INNER JOIN 
                                                        (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_len_freq ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id;";

                                success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete maturity
                            if (success)

                            {
                                cmd.CommandText = @"DELETE dbo_catch_maturity.*
                                                    FROM (dbo_gear_unload INNER JOIN 
                                                        (dbo_vessel_unload INNER JOIN 
                                                        (dbo_vessel_catch INNER JOIN 
                                                        dbo_catch_maturity ON 
                                                        dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id;";
                                success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete catch
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_catch.*
                                                FROM (dbo_gear_unload INNER JOIN 
                                                    (dbo_vessel_unload INNER JOIN 
                                                    dbo_vessel_catch ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id) ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() >= 0;
                            }

                            //delete weight validation
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload_weight_validation.*
                                                    FROM ((dbo_gear_unload INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_unload_weight_validation ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_unload_weight_validation.v_unload_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=id";
                                success = cmd.ExecuteNonQuery() >= 0;
                            }


                            //delete effort
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_vessel_effort.*
                                                    FROM ((dbo_gear_unload INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_effort ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_effort.v_unload_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                success = cmd.ExecuteNonQuery() >= 0;
                            }

                            // delete unload stats
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_vessel_unload_stats.*
                                                    FROM ((dbo_gear_unload INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_unload_stats ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_unload_stats.v_unload_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                success = cmd.ExecuteNonQuery() >= 0;
                            }



                            //delete gear soak
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_gear_soak.*
                                                    FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                        dbo_gear_unload ON 
                                                        dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_gear_soak ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";

                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                            }

                            //delete fishing grid
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_fg_grid.*
                                                    FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                        dbo_gear_unload ON 
                                                        dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_fg_grid ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_fg_grid.v_unload_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";

                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                            }



                            //delete vessel unload 1
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload_1.*
                                                    FROM ((dbo_LC_FG_sample_day_1 INNER JOIN 
                                                        dbo_gear_unload ON 
                                                        dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_unload_1 ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;

                                //success = cmd.ExecuteNonQuery() > 0;
                            }

                            //delete vessel unload 
                            if (success)
                            {
                                cmd.CommandText = @"DELETE dbo_vessel_unload.*
                                            FROM (dbo_LC_FG_sample_day_1 INNER JOIN 
                                                dbo_gear_unload ON 
                                                dbo_LC_FG_sample_day_1.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id
                                            WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";

                                //cmd.CommandText = @"DELETE dbo_vessel_unload.*
                                //                    FROM (dbo_vessel_unload INNER JOIN 
                                //                        dbo_gear_unload ON 
                                //                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                //                        dbo_LC_FG_sample_day_1 ON 
                                //                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                //                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }
                            //delete gear unload
                            if (success)
                            {
                                cmd.CommandText = @"DELETE  dbo_gear_unload.*
                                                    FROM dbo_gear_unload INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }

                            //delete landing site sampling
                            if (success)
                            {
                                //cmd.CommandText = "DELETE  * FROM dbo_LC_FG_sample_day_1 WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                cmd.CommandText = @"DELETE dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                    FROM dbo_LC_FG_sample_day INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                                r = cmd.ExecuteNonQuery();
                                success = r >= 0;
                                //success = cmd.ExecuteNonQuery() > 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }


                    }
                }
            }
            return success;
        }
    }
}
