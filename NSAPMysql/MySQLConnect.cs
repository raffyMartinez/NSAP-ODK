﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using NSAP_ODK.Utilities;
using NSAP_ODK.Entities.Database;
using System.IO;

namespace NSAP_ODK.NSAPMysql
{
    public static class MySQLConnect
    {
        public static event EventHandler<CreateTablesInAccessEventArgs> AccessTableEvent;
        private static string _mySQLSecureFolder;
        private static int _importCSVCount;
        public static string MYSQLPrivFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_mySQLSecureFolder))
                {
                    _mySQLSecureFolder = GetMySQLSecureFileFolder();
                }
                return _mySQLSecureFolder;
            }
        }


        public static string UserName { get; set; }
        public static string Password { get; set; }

        public static string UserHostName { get; set; }
        public static string LastError { get; set; }
        public static bool DatabaseExists { get; set; }

        public static bool CreateDatabase { get; set; }

        public static bool ValidUser { get; set; }

        public static bool UserCanCreateDatabase { get; set; }

        public static bool IsEmptyDatabase_ { get; set; }

        public static string ConnectionString()
        {
            return $"server=localhost;userid={UserName};password={Password};database=nsap_odk";
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

        public static Task<bool> BulkUpdateMySQLTablesWithLandingSurveyDataAsync()
        {
            return Task.Run(() => BulkUpdateMySQLTablesWithLandingSurveyData());
        }

        private static string GetMySQLSecureFileFolder()
        {
            using (MySqlConnection con = new MySqlConnection(ConnectionString()))
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = "Select @@global.secure_file_priv";
                    try
                    {
                        return cmd.ExecuteScalar() as string;
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }

        }
        public static bool BulkUpdateMySQLTablesWithLandingSurveyData()
        {
            Logger.Log($"In MySQlConnect.BulkUpdateMySQLTablesWithLandingSurveyData");
            string table_name = "";
            bool success = false;
            //string base_dir = AppDomain.CurrentDomain.BaseDirectory;

            string csv_file = $@"{MYSQLPrivFolder}temp.csv".Replace('\\', '/');
            using (MySqlConnection conn = new MySqlConnection(ConnectionString() + ";AllowLoadLocalInfile = true;"))
            //using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (MySqlTransaction tran = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.Transaction = tran;
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "start importing csv", TotalTableCount = 14 });

                            File.WriteAllText(csv_file, LandingSiteSamplingViewModel.CSV);
                            table_name = "dbo_lc_fg_sample_day";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_LC_FG_sample_day", CurrentTableCount = ++_importCSVCount });


                            File.WriteAllText(csv_file, LandingSiteSamplingViewModel.CSV_1);
                            table_name = "dbo_lc_fg_sample_day_1";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_LC_FG_sample_day_1", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, GearUnloadViewModel.CSV);
                            table_name = "dbo_gear_unload";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_gear_unload", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, VesselUnloadViewModel.CSV);
                            table_name = "dbo_vessel_unload";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_vessel_unload", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, VesselUnloadViewModel.CSV_1);
                            table_name = "dbo_vessel_unload_1";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_vessel_unload_1", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, VesselUnloadViewModel.UnloadStatsCSV);
                            table_name = "dbo_vessel_unload_stats";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_vessel_unload_stats", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, GearSoakViewModel.CSV);
                            table_name = "dbo_gear_soak";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_gear_soak", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, VesselEffortViewModel.CSV);
                            table_name = "dbo_vessel_effort";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_vessel_effort", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, FishingGroundGridViewModel.CSV);
                            table_name = "dbo_fg_grid";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_fg_grid", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, VesselCatchViewModel.CSV);
                            table_name = "dbo_vessel_catch";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_vessel_catch", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, CatchLenFreqViewModel.CSV);
                            table_name = "dbo_catch_len_freq";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_catch_len_freq", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, CatchLengthViewModel.CSV);
                            table_name = "dbo_catch_length";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_catch_length", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, CatchLengthWeightViewModel.CSV);
                            table_name = "dbo_catch_len_wt";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_catch_len_wt", CurrentTableCount = ++_importCSVCount });

                            File.WriteAllText(csv_file, CatchMaturityViewModel.CSV);
                            table_name = "dbo_catch_maturity";
                            cmd.CommandText = MakeCommandText(csv_file, table_name);
                            cmd.ExecuteNonQuery();
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_catch_maturity", CurrentTableCount = ++_importCSVCount });

                            tran.Commit();
                            success = true;
                            _importCSVCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"{ex.Message}\r\nWill attempt to roll back transaction for table {table_name}");
                        try
                        {
                            tran.Rollback();
                            Logger.Log("Database upload failed. Transaction was rolled back");
                        }
                        catch
                        {
                            // Do nothing here; transaction is not active.
                        }
                    }
                }
            }
            return success;
        }

        private static string MakeCommandText(string csv_file, string table_name)
        {
            return $@"SET FOREIGN_KEY_CHECKS=0;
                     LOAD DATA INFILE '{csv_file}' INTO TABLE {table_name} 
                     FIELDS TERMINATED BY ',' 
                     OPTIONALLY ENCLOSED BY '""' 
                     LINES TERMINATED BY '\r\n'  
                     IGNORE 1 ROWS;
                     SET FOREIGN_KEY_CHECKS=1";
        }

        public static string GetColumnNamesCSV(string tableName)
        {
            string cols = "";
            using (MySqlConnection con = new MySqlConnection(ConnectionString()))
            {
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Select * from nsap_odk.{tableName}";
                    con.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    var dt = dr.GetSchemaTable();

                    foreach (DataRow r in dt.Rows)
                    {
                        cols += $"{r[dt.Columns["ColumnName"]]},";
                    }
                }
            }
            return cols.Trim(new char[] { ',', ' ' });
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
                                success = cmd.ExecuteNonQuery() > 0;
                                DatabaseExists = success;
                                if (success)
                                {
                                    //CreateMySQLTables.UserName = UserName;
                                    //CreateMySQLTables.Password = Password;
                                    TableCount = CreateMySQLTables.CreateTables();
                                }
                            }
                            catch (MySqlException msex)
                            {
                                switch (msex.ErrorCode)
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
            return success && LastError.Length == 0;
        }

        private static bool DeleteTableDataUsingScript()
        {
            bool success = false;
            using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                var sc = new MySqlScript();
                sc.Query = @"SET FOREIGN_KEY_CHECKS=0;  -- turn off foreign key checks
                            TRUNCATE TABLE nsap_odk.dbo_catch_len_freq;  
                            TRUNCATE TABLE nsap_odk.dbo_catch_len_wt;
                            TRUNCATE TABLE nsap_odk.dbo_catch_length;
                            TRUNCATE TABLE nsap_odk.dbo_catch_maturity;
                            TRUNCATE TABLE nsap_odk.dbo_vessel_unload_stats;
                            TRUNCATE TABLE nsap_odk.dbo_vessel_catch;
                            TRUNCATE TABLE nsap_odk.dbo_fg_grid;
                            TRUNCATE TABLE nsap_odk.dbo_gear_soak;
                            TRUNCATE TABLE nsap_odk.dbo_vessel_effort;
                            TRUNCATE TABLE nsap_odk.dbo_vessel_unload_1;
                            TRUNCATE TABLE nsap_odk.dbo_vessel_unload;
                            TRUNCATE TABLE nsap_odk.dbo_gear_unload;
                            TRUNCATE TABLE nsap_odk.dbo_lc_fg_sample_day_1;
                            TRUNCATE TABLE nsap_odk.dbo_lc_fg_sample_day;
                            TRUNCATE TABLE nsap_odk.kobo_servers;
                            SET FOREIGN_KEY_CHECKS=1;  -- turn on foreign key checks";

                try
                {
                    con.Open();
                    sc.Connection = con;
                    int count = sc.Execute();
                    success = count > 0;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return success;
        }
        public static bool DeleteDataFromTables(bool useScript = false)
        {
            if (useScript)
            {
                return DeleteTableDataUsingScript();
            }
            else
            {
                int counter = 0;
                if (DeleteTableData("dbo_catch_len_freq")) counter++;
                if (DeleteTableData("dbo_catch_len_wt")) counter++;
                if (DeleteTableData("dbo_catch_length")) counter++;
                if (DeleteTableData("dbo_catch_maturity")) counter++;
                if (DeleteTableData("dbo_vessel_catch")) counter++;
                if (DeleteTableData("dbo_fg_grid")) counter++;
                if (DeleteTableData("dbo_gear_soak")) counter++;
                if (DeleteTableData("dbo_vessel_effort")) counter++;

                if (DeleteTableData("dbo_vessel_unload_stats")) counter++;
                if (DeleteTableData("dbo_vessel_unload_1")) counter++;
                if (DeleteTableData("dbo_vessel_unload")) counter++;
                if (DeleteTableData("dbo_gear_unload")) counter++;
                if (DeleteTableData("dbo_lc_fg_sample_day_1")) counter++;
                if (DeleteTableData("dbo_lc_fg_sample_day")) counter++;

                return counter == 14;
            }
        }

        private static bool DeleteTableData(string tableName)
        {
            bool success = false;
            using (var con = new MySqlConnection(ConnectionString()))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {

                    cmd.CommandText = $"DELETE FROM {tableName}";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public static int SchemaTableCount(string schemaName = "nsap_odk")
        {
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@schema", schemaName);
                    cmd.CommandText = "SELECT count(*) FROM information_schema.TABLES where table_schema=@schema";
                    try
                    {
                        conn.Open();
                        if (int.TryParse(cmd.ExecuteScalar().ToString(), out int r))
                        {
                            count = r;
                            TableCount = r;
                        }
                    }
                    catch (MySqlException mx)
                    {
                        Logger.Log(mx);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return count;
        }
        private static bool dbExists(string conn, string dbName, bool isEmpty = false)
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
                        UserCanCreateDatabase = LoggedInUserCanCreateDatabase();
                    }
                    catch (MySqlException msex)
                    {
                        LastError = msex.Message;
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message.Length > 0)
                        {
                            LastError = ex.InnerException.Message;
                        }
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
                                catch (MySqlException msex)
                                {
                                    switch (msex.ErrorCode)
                                    {
                                        case -2147467259:
                                            //unknown database which means that the database does not exist
                                            break;
                                        default:
                                            Logger.Log(msex);
                                            break;
                                    }
                                }
                                catch (Exception ex)
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
            catch (MySqlException msex)
            {
                LastError = msex.Message;
            }
            catch (ArgumentException aex)
            {
                LastError = aex.Message;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return exists;
        }



    }
}
