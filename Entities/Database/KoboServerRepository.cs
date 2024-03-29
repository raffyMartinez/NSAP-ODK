﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities.Database
{
    public class KoboServerRepository
    {
        public List<Koboserver> Koboservers { get; set; }

        public bool ClearTable()
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Delete * from kobo_servers";
                        con.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        catch (OleDbException olx)
                        {
                            Logger.Log(olx);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Delete * from kobo_servers";
                        con.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        catch (OleDbException olx)
                        {
                            Logger.Log(olx);
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
        public KoboServerRepository()
        {
            Koboservers = getKoboservers();
        }

        public bool Add(Koboserver ks)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@nid", MySqlDbType.Int32).Value = ks.ServerNumericID;
                        cmd.Parameters.Add("@fname", MySqlDbType.VarChar).Value = ks.FormName;
                        cmd.Parameters.Add("@sid", MySqlDbType.VarChar).Value = ks.ServerID;
                        cmd.Parameters.Add("@owner", MySqlDbType.VarChar).Value = ks.Owner;
                        cmd.Parameters.Add("@f_version", MySqlDbType.VarChar).Value = ks.FormVersion;
                        if (ks.eFormVersion == null)
                        {
                            cmd.Parameters.Add("@ef_version", MySqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@ef_version", MySqlDbType.VarChar).Value = ks.eFormVersion;
                        }
                        cmd.Parameters.Add("@d_created", MySqlDbType.DateTime).Value = ks.DateCreated;
                        cmd.Parameters.Add("@d_modified", MySqlDbType.DateTime).Value = ks.DateModified;
                        if (ks.DateLastSubmission == null)
                        {
                            cmd.Parameters.Add("@d_last_submission", MySqlDbType.DateTime).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@d_last_submission", MySqlDbType.DateTime).Value = ks.DateLastSubmission;
                        }
                        cmd.Parameters.Add("@s_count", MySqlDbType.Int32).Value = ks.SubmissionCount;
                        cmd.Parameters.Add("@u_count", MySqlDbType.Int32).Value = ks.UserCount;
                        cmd.Parameters.Add("@d_access", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@saved_count", MySqlDbType.Int32).Value = ks.SavedInDBCount;

                        if (ks.LastUploadedJSON == null)
                        {
                            cmd.Parameters.Add("@last_uploaded", MySqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_uploaded", MySqlDbType.VarChar).Value = ks.LastUploadedJSON;
                        }
                        if (ks.LastCreatedJSON == null)
                        {
                            cmd.Parameters.Add("@last_created", MySqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_created", MySqlDbType.VarChar).Value = ks.LastCreatedJSON;
                        }

                        cmd.CommandText = @"INSERT INTO kobo_servers
                                                ( server_numeric_id,
                                                form_name,
                                                server_id,
                                                owner,
                                                form_version,
                                                e_form_version,
                                                date_created,
                                                date_modified,
                                                date_last_submission,
                                                submission_count,
                                                user_count,
                                                date_last_accessed,
                                                saved_in_db_count,
                                                last_uploaded_json,
                                                last_created_json
                                                )
                                                VALUES
                                                ( @nid,
                                                @fname,
                                                @sid,
                                                @owner,
                                                @f_version,
                                                @ef_version,
                                                @d_created,
                                                @d_modified,
                                                @d_last_submission,
                                                @s_count,
                                                @u_count,
                                                @d_access,
                                                @saved_count,
                                                @last_uploaded,
                                                @last_created
                                                )";


                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@nid", OleDbType.Integer).Value = ks.ServerNumericID;
                        cmd.Parameters.Add("@fname", OleDbType.VarChar).Value = ks.FormName;
                        cmd.Parameters.Add("@sid", OleDbType.VarChar).Value = ks.ServerID;
                        cmd.Parameters.Add("@owner", OleDbType.VarChar).Value = ks.Owner;
                        if (ks.FormVersion == null)
                        {
                             cmd.Parameters.Add("@f_version", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@f_version", OleDbType.VarChar).Value = ks.FormVersion;
                        }
                        if (ks.eFormVersion == null)
                        {
                            cmd.Parameters.Add("@ef_version", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@ef_version", OleDbType.VarChar).Value = ks.eFormVersion;
                        }
                        cmd.Parameters.Add("@d_created", OleDbType.Date).Value = ks.DateCreated;
                        cmd.Parameters.Add("@d_modified", OleDbType.Date).Value = ks.DateModified;
                        if (ks.DateLastSubmission == null)
                        {
                            cmd.Parameters.Add("@d_last_submission", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@d_last_submission", OleDbType.Date).Value = ks.DateLastSubmission;
                        }
                        cmd.Parameters.Add("@s_count", OleDbType.Integer).Value = ks.SubmissionCount;
                        cmd.Parameters.Add("@u_count", OleDbType.Integer).Value = ks.UserCount;
                        cmd.Parameters.Add("@d_access", OleDbType.Date).Value = DateTime.Now;
                        cmd.Parameters.Add("@saved_count", OleDbType.Integer).Value = ks.SavedInDBCount;

                        if (ks.LastUploadedJSON == null)
                        {
                            cmd.Parameters.Add("@last_uploaded", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_uploaded", OleDbType.VarChar).Value = ks.LastUploadedJSON;
                        }
                        if (ks.LastCreatedJSON == null)
                        {
                            cmd.Parameters.Add("@last_created", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_created", OleDbType.VarChar).Value = ks.LastCreatedJSON;
                        }

                        cmd.CommandText = @"INSERT INTO kobo_servers
                                                ( server_numeric_id,
                                                form_name,
                                                server_id,
                                                owner,
                                                form_version,
                                                e_form_version,
                                                date_created,
                                                date_modified,
                                                date_last_submission,
                                                submission_count,
                                                user_count,
                                                date_last_accessed,
                                                saved_in_db_count,
                                                last_uploaded_json,
                                                last_created_json
                                                )
                                                VALUES
                                                ( @nid,
                                                @fname,
                                                @sid,
                                                @owner,
                                                @f_version,
                                                @ef_version,
                                                @d_created,
                                                @d_modified,
                                                @d_last_submission,
                                                @s_count,
                                                @u_count,
                                                @d_access,
                                                @saved_count,
                                                @last_uploaded,
                                                @last_created
                                                )";


                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

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

        public bool UpdateServerCount(Koboserver ks)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@saved_count", MySqlDbType.Int32).Value = ks.SavedInDBCount;
                        cmd.Parameters.Add("@nid", MySqlDbType.Int32).Value = ks.ServerNumericID;
                        cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                saved_in_db_count=@saved_count
                                                WHERE
                                                server_numeric_id=@nid ";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@saved_count", OleDbType.Integer).Value = ks.SavedInDBCount;
                        cmd.Parameters.Add("@nid", OleDbType.Integer).Value = ks.ServerNumericID;
                        cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                saved_in_db_count=@saved_count
                                                WHERE
                                                server_numeric_id=@nid ";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

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

        public bool UpdateUploadedJSONs(Koboserver ks, bool updateUploadedJson = false)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        if (updateUploadedJson)
                        {
                            cmd.Parameters.Add("@last_uploaded", MySqlDbType.VarChar).Value = ks.LastUploadedJSON;
                            cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                last_uploaded_json=@last_uploaded
                                                WHERE
                                                server_numeric_id=@nid ";
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_created", MySqlDbType.VarChar).Value = ks.LastCreatedJSON;
                            cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                last_created_json=@last_created
                                                WHERE
                                                server_numeric_id=@nid ";
                        }
                        cmd.Parameters.Add("@nid", MySqlDbType.Int32).Value = ks.ServerNumericID;




                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        if (updateUploadedJson)
                        {
                            cmd.Parameters.Add("@last_uploaded", OleDbType.VarChar).Value = ks.LastUploadedJSON;
                            cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                last_uploaded_json=@last_uploaded
                                                WHERE
                                                server_numeric_id=@nid ";
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_created", OleDbType.VarChar).Value = ks.LastCreatedJSON;
                            cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                last_created_json=@last_created
                                                WHERE
                                                server_numeric_id=@nid ";
                        }
                        cmd.Parameters.Add("@nid", OleDbType.Integer).Value = ks.ServerNumericID;




                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

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
        public bool Update(Koboserver ks)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@fname", MySqlDbType.VarChar).Value = ks.FormName;
                        cmd.Parameters.Add("@sid", MySqlDbType.VarChar).Value = ks.ServerID;
                        cmd.Parameters.Add("@owner", MySqlDbType.VarChar).Value = ks.Owner;
                        cmd.Parameters.Add("@f_version", MySqlDbType.VarChar).Value = ks.FormVersion;
                        if (ks.eFormVersion == null)
                        {
                            cmd.Parameters.Add("@ef_version", MySqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@ef_version", MySqlDbType.VarChar).Value = ks.eFormVersion;
                        }
                        cmd.Parameters.Add("@d_created", MySqlDbType.DateTime).Value = ks.DateCreated;
                        cmd.Parameters.Add("@d_modified", MySqlDbType.DateTime).Value = ks.DateModified;
                        if (ks.DateLastSubmission == null)
                        {
                            cmd.Parameters.Add("@d_last_submission", MySqlDbType.DateTime).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@d_last_submission", MySqlDbType.DateTime).Value = ks.DateLastSubmission;
                        }
                        cmd.Parameters.Add("@s_count", MySqlDbType.Int32).Value = ks.SubmissionCount;
                        cmd.Parameters.Add("@u_count", MySqlDbType.Int32).Value = ks.UserCount;
                        cmd.Parameters.Add("@d_access", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@saved_count", MySqlDbType.Int32).Value = ks.SavedInDBCount;

                        if (ks.LastUploadedJSON == null)
                        {
                            cmd.Parameters.Add("@last_uploaded", MySqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_uploaded", MySqlDbType.VarChar).Value = ks.LastUploadedJSON;
                        }
                        if (ks.LastCreatedJSON == null)
                        {
                            cmd.Parameters.Add("@last_created", MySqlDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_created", MySqlDbType.VarChar).Value = ks.LastCreatedJSON;
                        }

                        cmd.Parameters.Add("@nid", MySqlDbType.Int32).Value = ks.ServerNumericID;

                        cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                form_name=@fname,
                                                server_id=@sid,
                                                owner=@owner,
                                                form_version=@f_version,
                                                e_form_version=@ef_version,
                                                date_created=@d_created,
                                                date_modified=@d_modified,
                                                date_last_submission=@d_last_submission,
                                                submission_count=@s_count,
                                                user_count=@u_count,
                                                date_last_accessed= @d_access,
                                                saved_in_db_count=@saved_count,
                                                last_uploaded_json=@last_uploaded,
                                                last_created_json=@last_created
                                                WHERE
                                                server_numeric_id=@nid ";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@fname", OleDbType.VarChar).Value = ks.FormName;
                        cmd.Parameters.Add("@sid", OleDbType.VarChar).Value = ks.ServerID;
                        cmd.Parameters.Add("@owner", OleDbType.VarChar).Value = ks.Owner;
                        
                        if(ks.FormVersion==null)
                        {
                            cmd.Parameters.Add("@f_version", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@f_version", OleDbType.VarChar).Value = ks.FormVersion;
                        }
                        if (ks.eFormVersion == null)
                        {
                            cmd.Parameters.Add("@ef_version", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@ef_version", OleDbType.VarChar).Value = ks.eFormVersion;
                        }
                        cmd.Parameters.Add("@d_created", OleDbType.Date).Value = ks.DateCreated;
                        cmd.Parameters.Add("@d_modified", OleDbType.Date).Value = ks.DateModified;
                        if (ks.DateLastSubmission == null)
                        {
                            cmd.Parameters.Add("@d_last_submission", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@d_last_submission", OleDbType.Date).Value = ks.DateLastSubmission;
                        }
                        cmd.Parameters.Add("@s_count", OleDbType.Integer).Value = ks.SubmissionCount;
                        cmd.Parameters.Add("@u_count", OleDbType.Integer).Value = ks.UserCount;
                        cmd.Parameters.Add("@d_access", OleDbType.Date).Value = DateTime.Now;
                        if (ks.IsFishLandingMultiVesselSurveyForm)
                        {
                            cmd.Parameters.Add("@saved_count", OleDbType.Integer).Value = NSAPEntities.LandingSiteSamplingSubmissionViewModel.CountRecordsByFormID(ks.ServerID);
                        }
                        else
                        {
                            cmd.Parameters.Add("@saved_count", OleDbType.Integer).Value = ks.SavedInDBCount;
                        }

                        if (ks.LastUploadedJSON == null)
                        {
                            cmd.Parameters.Add("@last_uploaded", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_uploaded", OleDbType.VarChar).Value = ks.LastUploadedJSON;
                        }
                        if (ks.LastCreatedJSON == null)
                        {
                            cmd.Parameters.Add("@last_created", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@last_created", OleDbType.VarChar).Value = ks.LastCreatedJSON;
                        }

                        cmd.Parameters.Add("@nid", OleDbType.Integer).Value = ks.ServerNumericID;

                        cmd.CommandText = @"UPDATE kobo_servers
                                                SET
                                                form_name=@fname,
                                                server_id=@sid,
                                                owner=@owner,
                                                form_version=@f_version,
                                                e_form_version=@ef_version,
                                                date_created=@d_created,
                                                date_modified=@d_modified,
                                                date_last_submission=@d_last_submission,
                                                submission_count=@s_count,
                                                user_count=@u_count,
                                                date_last_accessed= @d_access,
                                                saved_in_db_count=@saved_count,
                                                last_uploaded_json=@last_uploaded,
                                                last_created_json=@last_created
                                                WHERE
                                                server_numeric_id=@nid ";


                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;

                        }
                        catch (OleDbException oex)
                        {

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

        public bool Delete(int numericID)
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
                        cmd.Parameters.AddWithValue("@id", numericID);
                        cmd.CommandText = "DELETE * FROM kobo_servers WHERE server_numeric_id=@id ";
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
        private List<Koboserver> getKoboservers()
        {
            List<Koboserver> this_list = new List<Koboserver>();

            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from kobo_servers";
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Koboserver ks = new Koboserver
                                {
                                    ServerNumericID = (int)dr["server_numeric_id"],
                                    ServerID = dr["server_id"].ToString(),
                                    FormName = dr["form_name"].ToString(),
                                    Owner = dr["owner"].ToString(),
                                    FormVersion = dr["form_version"].ToString(),
                                    eFormVersion = dr["e_form_version"].ToString(),
                                    DateCreated = DateTime.Parse(dr["date_created"].ToString()),
                                    DateModified = DateTime.Parse(dr["date_modified"].ToString()),
                                    DateLastSubmission = DateTime.Parse(dr["date_last_submission"].ToString()),
                                    SubmissionCount = (int)dr["submission_count"],
                                    UserCount = (int)dr["user_count"],
                                    DateLastAccessed = DateTime.Parse(dr["date_last_accessed"].ToString()),
                                    SavedInDBCount = (int)dr["saved_in_db_count"],
                                    LastUploadedJSON = dr["last_uploaded_json"].ToString(),
                                    LastCreatedJSON = dr["last_created_json"].ToString()
                                };
                                this_list.Add(ks);
                            }
                        }
                        catch (MySqlException mex)
                        {

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from kobo_servers";
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                DateTime? lastSubmission = null;
                                if (dr["date_modified"] != DBNull.Value)
                                {
                                    lastSubmission = (DateTime)dr["date_modified"];
                                }
                                Koboserver ks = new Koboserver
                                {
                                    ServerNumericID = (int)dr["server_numeric_id"],
                                    ServerID = dr["server_id"].ToString(),
                                    FormName = dr["form_name"].ToString(),
                                    Owner = dr["owner"].ToString(),
                                    FormVersion = dr["form_version"].ToString(),
                                    eFormVersion = dr["e_form_version"].ToString(),
                                    DateCreated = DateTime.Parse(dr["date_created"].ToString()),
                                    DateModified = DateTime.Parse(dr["date_modified"].ToString()),
                                    DateLastSubmission = lastSubmission,
                                    SubmissionCount = (int)dr["submission_count"],
                                    UserCount = (int)dr["user_count"],
                                    DateLastAccessed = DateTime.Parse(dr["date_last_accessed"].ToString()),
                                    SavedInDBCount = (int)dr["saved_in_db_count"],
                                    LastUploadedJSON = dr["last_uploaded_json"].ToString(),
                                    LastCreatedJSON = dr["last_created_json"].ToString()
                                };
                                this_list.Add(ks);
                            }
                        }
                        catch (OleDbException oex)
                        {
                            switch (oex.ErrorCode)
                            {
                                case -2147217865:
                                    if (oex.Message.Contains("The Microsoft Jet database engine cannot find the input table or query"))
                                    {
                                        var arr = oex.Message.Split('\'');
                                        if (CreateTable(arr[1]))
                                        {
                                            return getKoboservers();
                                        };
                                    }
                                    break;
                                default:
                                    Logger.Log(oex);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return this_list;
        }

        private bool CreateTable(string tableName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    switch (tableName)
                    {
                        case "kobo_servers":
                            cmd.CommandText = @"CREATE TABLE kobo_servers (
                                                server_numeric_id Int NOT NULL PRIMARY KEY,
                                                form_name VarChar,
                                                server_id VarChar,
                                                owner VarChar,
                                                form_version VarChar,
                                                e_form_version VarChar,
                                                date_created DateTime,
                                                date_modified DateTime,
                                                date_last_submission DateTime,
                                                submission_count Int,
                                                user_count Int,
                                                date_last_accessed DateTime,
                                                saved_in_db_count Int,
                                                last_uploaded_json VarChar,
                                                last_created_json VarChar
                                                )";

                            break;
                    }

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
    }
}
