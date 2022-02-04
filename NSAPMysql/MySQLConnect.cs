using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.NSAPMysql
{
    public static class MySQLConnect
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }

        public static string UserHostName { get; set; }
        public static string LastError { get; set; }
        public static bool DatabaseExists { get; set; }

        public static bool CreateDatabase { get; set; }

        public static bool ValidUser { get; set; }

        public static bool UserCanCreateDatabase { get; set; }

        public static bool IsEmptyDatabase_ { get; set; }

        public static string ConnectionString ()
        {
            return  $"server=localhost;userid={UserName};password={Password};database=nsap_odk";
        }

        public static string GetUserHostName()
        {
            string cs = $"server=localhost;userid={UserName};password={Password};";
            using (var conn = new MySqlConnection(cs))
            {

                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.Parameters.Add("@user", MySqlDbType.VarChar).Value = UserName;
                    cmd.CommandText = "Select HOST from information_schema.user_attributes where USER=@user";
                    UserHostName = cmd.ExecuteScalar().ToString();
                }
            }
            return UserHostName;
        }
        private static bool LoggedInUserCanCreateDatabase()
        {
            bool canCreate = false;
            string cs = $"server=localhost;userid={UserName};password={Password};";
            using (var conn = new MySqlConnection(cs))
            {

                using (var cmd = conn.CreateCommand())
                {
                    UserHostName = GetUserHostName();
                    conn.Open();
                    cmd.Parameters.Add("@priv", MySqlDbType.VarChar).Value = "CREATE";
                    cmd.Parameters.Add("@user", MySqlDbType.VarChar).Value = $"'{UserName}'@'{UserHostName}'";
                    cmd.CommandText = "Select PRIVILEGE_TYPE from information_schema.user_privileges where PRIVILEGE_TYPE=@priv and GRANTEE=@user";
                    try
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            canCreate = result.ToString() == "CREATE";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return canCreate;
        }

        public static int TableCount { get; internal set; }
        public static bool SetUP()
        {
            LastError = "";
            bool success = false;
            string cs = $"server=localhost;userid={UserName};password={Password};";

            if (dbExists(cs, "nsap_odk"))
            {
                DatabaseExists = true;
            }
            else
            {
                if (LastError == "" && CreateDatabase)
                {
                    using (var conn = new MySqlConnection(cs))
                    {

                        using (var cmd = conn.CreateCommand())
                        {
                            conn.Open();
                            cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `nsap_odk`;";
                            try
                            {
                                success = cmd.ExecuteNonQuery()>0;
                                DatabaseExists = success;
                                if(success)
                                {
                                    //CreateMySQLTables.UserName = UserName;
                                    //CreateMySQLTables.Password = Password;
                                    TableCount = CreateMySQLTables.CreateTables();
                                }
                            }
                            catch(MySqlException msex)
                            {
                                switch(msex.ErrorCode)
                                {
                                    case -2147467259:
                                        //not enough privilege to execute database action
                                        UserCanCreateDatabase = true;
                                        break;
                                }
                                LastError = msex.Message;

                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
            }
            return success && LastError.Length==0;
        }
        

        private static bool dbExists(string conn, string dbName, bool isEmpty=false)
        {
            
            bool exists = false;
            ValidUser = false;
            try
            {
                using (MySqlConnection dbconn = new MySqlConnection(conn))
                {

                    try
                    {
                        dbconn.Open();
                        ValidUser = true;
                        UserCanCreateDatabase=LoggedInUserCanCreateDatabase();
                    }
                    catch (MySqlException msex)
                    {
                        LastError = msex.Message;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                    if (ValidUser)
                    {
                        if (!UserCanCreateDatabase)
                        {
                            using (MySqlCommand c = dbconn.CreateCommand())
                            {
                                c.CommandText = "Select * from nsap_odk.nsap_region LIMIT 1";
                                try
                                {
                                    var rows = c.ExecuteReader();
                                    exists = true;
                                }
                                catch(MySqlException msex)
                                {
                                    switch(msex.ErrorCode)
                                    {
                                        case -2147467259:
                                            //unknown database which means that the database does not exist
                                            break;
                                        default:
                                            Logger.Log(msex);
                                            break;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                        }
                        else
                        {
                            using (MySqlCommand c = dbconn.CreateCommand())
                            {

                                c.Parameters.Add("@dbname", MySqlDbType.VarChar).Value = dbName;
                                c.CommandText = "SELECT Count(*) as n FROM information_schema.TABLES where TABLE_SCHEMA=@dbName";
                                //c.CommandText = "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME=@dbName";
                                try
                                {
                                    if (int.TryParse(c.ExecuteScalar().ToString(), out int r))
                                    {
                                        TableCount = r;
                                        exists = TableCount >= 1;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                                dbconn.Close();
                            }
                        }
                    }
                }
            }
            catch(MySqlException msex)
            {
                LastError = msex.Message;
            }
            catch(ArgumentException aex)
            {
                LastError = aex.Message;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return exists;
        }



    }
}
