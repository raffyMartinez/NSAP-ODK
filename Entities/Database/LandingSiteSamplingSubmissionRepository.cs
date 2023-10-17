using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSamplingSubmissionRepository
    {
        public LandingSiteSamplingSubmissionRepository()
        {
            LandingSiteSamplingSubmissions = getSubmissions();
        }
        public List<LandingSiteSamplingSubmission> LandingSiteSamplingSubmissions { get; set; }

        public static bool CheckForLSS_SubmissionIDTable()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString() == "dbo_lss_submissionIDs"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    tableExists = CreateTable();
                }
            }
            return tableExists;

        }

        public static bool CreateTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"CREATE TABLE dbo_lss_submissionIDs (
                                            submission_id VARCHAR NOT NULL,
                                            date_added DATETIME NOT NULL,
                                            json_File VARCHAR NOT NULL,
                                            xFormIdentifier VARCHAR NOT NULL,
                                            landing_site_sampling_id INT NOT NULL,
                                            CONSTRAINT PrimaryKey PRIMARY KEY (submission_id),
                                            CONSTRAINT FK_submissionIDs
                                                FOREIGN KEY (landing_site_sampling_id) REFERENCES
                                                dbo_LC_FG_sample_day (unload_day_id)
                                            )";


                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        success = true;

                    }
                    catch (OleDbException odx)
                    {
                        Logger.Log(odx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return success;
        }

        public static bool ClearTable(string otherConnectionString = "")
        {
            bool success = false;
            string con_string = Global.ConnectionString;
            if (otherConnectionString.Length > 0)
            {
                con_string = otherConnectionString;
            }

            using (OleDbConnection conn = new OleDbConnection(con_string))
            {
                conn.Open();
                var sql = "Delete * from dbo_lss_submissionIDs";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = true;
                    }
                    catch (OleDbException)
                    {
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        success = false;
                    }
                }
            }
            return success;
        }



        private List<LandingSiteSamplingSubmission> getSubmissions()
        {
            List<LandingSiteSamplingSubmission> this_list = new List<LandingSiteSamplingSubmission>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_lss_submissionIDs";
                        //if (Global.Filter1 == null)
                        //{
                        //    cmd.CommandText = "Select * from dbo_lss_submissionIDs";
                        //}
                        //else
                        //{
                        //    if (Global.Filter1 != null)
                        //    {
                        //        cmd.Parameters.AddWithValue("@d1", Global.Filter1DateString());
                        //        if (Global.Filter2 != null)
                        //        {
                        //            cmd.Parameters.AddWithValue("@d2", Global.Filter2DateString());
                        //            cmd.CommandText = $@"SELECT dbo_lss_submissionIDs.*
                        //                                FROM dbo_LC_FG_sample_day INNER JOIN 
                        //                                    dbo_lss_submissionIDs ON 
                        //                                    dbo_LC_FG_sample_day.unload_day_id = dbo_lss_submissionIDs.landing_site_sampling_id
                        //                                WHERE [sDate]>=@d1 AND [sDate]<@d2";
                        //        }
                        //        else
                        //        {
                        //            cmd.CommandText = $@"SELECT dbo_lss_submissionIDs.*
                        //                                FROM dbo_LC_FG_sample_day INNER JOIN 
                        //                                    dbo_lss_submissionIDs ON 
                        //                                    dbo_LC_FG_sample_day.unload_day_id = dbo_lss_submissionIDs.landing_site_sampling_id
                        //                                WHERE [sDate]>=@d1";
                        //        }
                        //    }
                        //}
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            LandingSiteSamplingSubmission landingSiteSamplingSubmission = new LandingSiteSamplingSubmission
                            {
                                SubmissionID = dr["submission_id"].ToString(),
                                DateAdded = (DateTime)dr["date_added"],
                                JSONFile = dr["json_File"].ToString(),
                                LandingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling((int)dr["landing_site_sampling_id"]),
                                XFormIdentifier = dr["xFormIdentifier"].ToString()
                            };

                            this_list.Add(landingSiteSamplingSubmission);
                        }
                    }
                }
            }
            return this_list;
        }

        public bool Add(LandingSiteSamplingSubmission lsss)
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
                        cmd.Parameters.AddWithValue("@id", lsss.SubmissionID);
                        cmd.Parameters.AddWithValue("@date_added", lsss.DateAdded);
                        cmd.Parameters.AddWithValue("@json", lsss.JSONFile);
                        cmd.Parameters.AddWithValue("@ls_id", lsss.LandingSiteSampling.PK);
                        cmd.Parameters.AddWithValue("@xForm", lsss.XFormIdentifier);

                        cmd.CommandText = @"INSERT INTO dbo_lss_submissionIDs 
                                            (submission_id,date_added,json_File,landing_site_sampling_id)
                                            Values (?,?,?,?,?) ";

                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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

        public bool Update(LandingSiteSamplingSubmission lsss)
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
                        cmd.Parameters.AddWithValue("@id", lsss.SubmissionID);
                        cmd.Parameters.AddWithValue("@date_added", lsss.DateAdded);
                        cmd.Parameters.AddWithValue("@json", lsss.JSONFile);
                        cmd.Parameters.AddWithValue("@xForm", lsss.XFormIdentifier);
                        cmd.Parameters.AddWithValue("@ls_id", lsss.LandingSiteSampling.PK);

                        cmd.CommandText = @"UPDATE dbo_lss_submissionIDs SET
                                                date_added = @date_added,
                                                json_File = @json,
                                                xFormIdentifier = @xForm,
                                                landing_site_sampling_id = @ls_id
                                            WHERE submission_id = @id";


                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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

        public bool Delete(string lsss_id)
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
                        cmd.Parameters.AddWithValue("@id", lsss_id);
                        cmd.CommandText = @"DELETE * from dbo_lss_submissionIDs WHERE submission_id=id  ";


                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
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
