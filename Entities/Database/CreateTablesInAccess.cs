using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using System.IO;
using System.Data;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities.Database
{
    public static class CreateTablesInAccess
    {
        private static int _importCSVCount;
        private static string _mdbFile;
        private static string _connectionString;


        public static event EventHandler<CreateTablesInAccessEventArgs> AccessTableEvent;
        public static List<SQLDumpParsed> ListSQLDumpParsed { get; set; }
        public static string MDBFile
        {
            get { return _mdbFile; }
            set
            {
                _mdbFile = value;
                _connectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + _mdbFile;
            }
        }
        public static List<MDBColumnInfo> MDBColumnInfos { get; set; } = new List<MDBColumnInfo>();
        public static Task<bool> UploadImportJsonResultAsync()
        {
            return Task.Run(() => UploadImportJsonResult());
        }
        public static string TableWithUploadError { get; set; }
        public static string UploadErrorMessage { get; set; }

        public static string GetAccessRelationshipName(string column_many, string table_many, string column_one, string table_one)
        {
            string result = "";
            using (OleDbConnection connection = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@many_col", column_many);
                    cmd.Parameters.AddWithValue("@many_tbl", table_many);
                    cmd.Parameters.AddWithValue("@one_col", column_one);
                    cmd.Parameters.AddWithValue("@one_tbl", table_one);
                    cmd.CommandText = "Select szRelationship from msysrelationships where szColumn=? and szObject=? and szReferencedColumn=? and szReferencedObject=?";


                    result = cmd.ExecuteScalar().ToString();
                }
            }
            return result;
        }
        /// <summary>
        /// inserts data into the database in batch mode
        /// </summary>
        /// <returns></returns>
        public static bool UploadImportJsonResult()
        {
            UploadErrorMessage = "";
            string currentTableName = "";
            //Logger.Log($"In CreateTablesInAccess.UploadImportJsonResult");
            bool success = false;
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string csv_file = $@"{base_dir}\temp.csv";
            MDBFile = Global.MDBPath;
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    OleDbTransaction transaction = null;
                    try
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = transaction;
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "start importing csv", TotalTableCount = 17 });
                        _importCSVCount = 0;

                        currentTableName = "dbo_LC_FG_sample_day";
                        File.WriteAllText(csv_file, LandingSiteSamplingViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_LC_FG_sample_day SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_LC_FG_sample_day_1";
                        File.WriteAllText(csv_file, LandingSiteSamplingViewModel.CSV_1);
                        cmd.CommandText = $@"INSERT INTO dbo_LC_FG_sample_day_1 SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        //add twsp count to csv
                        currentTableName = "dbo_gear_unload";
                        File.WriteAllText(csv_file, GearUnloadViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_gear_unload SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_vessel_unload";
                        File.WriteAllText(csv_file, VesselUnloadViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_unload SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });


                        //do not write enumerator text if enumerator id is not null
                        currentTableName = "dbo_vessel_unload_1";
                        File.WriteAllText(csv_file, VesselUnloadViewModel.CSV_1);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_unload_1 SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_vessel_unload_stats";
                        File.WriteAllText(csv_file, VesselUnloadViewModel.UnloadStatsCSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_unload_stats SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_vessel_unload_weight_validation";
                        File.WriteAllText(csv_file, VesselUnloadViewModel.WeightValidationCSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_unload_weight_validation SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        //MULTI GEAR DATA 

                        currentTableName = "dbo_vesselunload_fishinggear";
                        File.WriteAllText(csv_file, VesselUnload_FishingGearViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vesselunload_fishinggear SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_vessel_effort";
                        File.WriteAllText(csv_file, VesselUnload_Gear_Spec_ViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_effort SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });


                        currentTableName = "dbo_vessel_effort";
                        File.WriteAllText(csv_file, VesselEffortViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_effort SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });



                        currentTableName = "dbo_fg_grid";
                        File.WriteAllText(csv_file, FishingGroundGridViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_fg_grid SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_gear_soak";
                        File.WriteAllText(csv_file, GearSoakViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_gear_soak SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = "dbo_gear_soak", CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_vessel_catch";
                        File.WriteAllText(csv_file, VesselCatchViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_vessel_catch SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_catch_len";
                        File.WriteAllText(csv_file, CatchLengthViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_catch_len SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_catch_len_freq";
                        File.WriteAllText(csv_file, CatchLenFreqViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_catch_len_freq SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_catch_len_wt";
                        File.WriteAllText(csv_file, CatchLengthWeightViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_catch_len_wt SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });

                        currentTableName = "dbo_catch_maturity";
                        File.WriteAllText(csv_file, CatchMaturityViewModel.CSV);
                        cmd.CommandText = $@"INSERT INTO dbo_catch_maturity SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                        cmd.ExecuteNonQuery();
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = currentTableName, CurrentTableCount = ++_importCSVCount });


                        transaction.Commit();
                        success = true;

                    }
                    catch (Exception ex)
                    {
                        TableWithUploadError = currentTableName;
                        UploadErrorMessage = ex.Message;
                        Logger.Log($"{UploadErrorMessage}\r\nTable uploaded is {currentTableName}\r\nWill attempt to roll back transaction");
                        try
                        {
                            transaction.Rollback();
                            Logger.Log($"Database upload failed when processing {currentTableName}. Transaction was rolled back");
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

        public static List<string> GetColumnNames(string tableName, bool makeLowerCase = false)
        {
            List<string> cols = new List<string>();
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Select * from {tableName}";
                    con.Open();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        var table = reader.GetSchemaTable();
                        var nameCol = table.Columns["ColumnName"];
                        foreach (DataRow row in table.Rows)
                        {
                            if (makeLowerCase)
                            {
                                cols.Add($"{row[nameCol].ToString().ToLower()}");
                            }
                            else
                            {
                                cols.Add($"{row[nameCol]}");
                            }
                        }
                    }
                }
            }
            return cols;
        }

        public static DataTable GetDataTableFromCsv(string csv)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string csv_file = $@"{base_dir}\temp.csv";
            File.WriteAllText(csv_file, csv);

            string pathOnly = Path.GetDirectoryName(csv_file);
            string fileName = Path.GetFileName(csv_file);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=Yes\""))
            {
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Locale = System.Globalization.CultureInfo.CurrentCulture;
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public static List<string> GetColumnNamesList(string tableName)
        {
            List<string> this_list = new List<string>();
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Select * from {tableName}";
                    con.Open();
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly))
                    {
                        var table = reader.GetSchemaTable();
                        var nameCol = table.Columns["ColumnName"];
                        foreach (DataRow row in table.Rows)
                        {
                            this_list.Add(row[nameCol].ToString());
                        }
                    }
                }
            }
            return this_list;
        }
        public static string GetColumnNamesCSV(string tableName, List<string> excludeColumns = null)
        {
            string csv = "";
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $"Select * from {tableName}";
                    con.Open();
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly))
                    {
                        var table = reader.GetSchemaTable();
                        var nameCol = table.Columns["ColumnName"];
                        foreach (DataRow row in table.Rows)
                        {
                            string colName = row[nameCol].ToString();
                            if (excludeColumns == null || !excludeColumns.Contains(colName))
                            {
                                csv += $"{row[nameCol]},";
                            }

                        }
                    }
                }
            }
            return csv.Trim(',');
        }
        public static string ConnectionString
        {
            get
            {
                if (_mdbFile.Length == 0)
                {
                    throw new Exception("Connection string cannot be emtpy");
                }

                return _connectionString;
            }
        }
        /// <summary>
        /// Reads all the entire mdb file, scans all tables for columns and saves column name, type and sequence.
        /// </summary>
        /// <param name="connectionString"></param>
        public static void GetMDBColumnInfo(string connectionString)
        {
            MDBColumnInfos.Clear();
            OleDbConnection con = new OleDbConnection(connectionString);
            DataSet tablesFromDB = new DataSet();

            DataTable schemaTbl;
            try
            {
                // Open the connection
                con.Open();
                object[] objArrRestrict = new object[] { null, null, null, "TABLE" };
                //object[] objArrRestrict = new object[] { null, null, "fishingVesel",null,null, "dbo_Vessel_unload" };

                // Get the table names from the database we're connected to
                schemaTbl = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, objArrRestrict);

                // Not sure if this is correct syntax...fix it if it isn't :)
                //String commandText = @"SELECT * FROM {0}";

                // Get each table name that we just found and get the schema for that table.
                int seq = 0;
                foreach (DataRow row in schemaTbl.Rows)
                {

                    DataTable dt = new DataTable();
                    //OleDbCommand command = new OleDbCommand(String.Format(commandText, row["TABLE_NAME"] as String), con);
                    if (!row["TABLE_NAME"].ToString().Contains("~"))
                    {
                        OleDbCommand command = new OleDbCommand($"Select * from {row["TABLE_NAME"]}", con);
                        dt.Load(command.ExecuteReader(CommandBehavior.SchemaOnly));
                        tablesFromDB.Tables.Add(dt);

                        foreach (DataColumn item in dt.Columns)
                        {
                            try
                            {
                                MDBColumnInfos.Add(new MDBColumnInfo { TableName = row.ItemArray[2].ToString(), ColumnName = item.ColumnName, TypeName = item.DataType.Name, Sequence = ++seq });
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
        public static string CSVFromObjectDataDictionary(Dictionary<string, string> values, string tableName)
        {
            StringBuilder csv = new StringBuilder();
            string columns = "";
            foreach (var item in MDBColumnInfos.Where(t => t.TableName == tableName).OrderBy(t => t.Sequence))
            {
                try
                {
                    string s;
                    if (values.TryGetValue(item.ColumnName, out s))
                    {
                        switch (item.TypeName)
                        {
                            case "Int32":
                            case "Double":
                                csv.Append($"{s},");
                                break;
                            case "String":
                            case "Guid":
                                csv.Append($"\"{s}\",");
                                break;
                            case "DateTime":
                                csv.Append($"{s},");
                                break;
                            case "Boolean":
                                if (s == "True")
                                {
                                    csv.Append("1,");
                                }
                                else
                                {
                                    csv.Append("0,");
                                }
                                break;
                        }
                        //columns += $"{item.ColumnName},";
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            return csv.ToString().Trim(',');
        }
        public static Task<bool> DropTablesAsync()
        {
            return Task.Run(() => DropTables());
        }
        public static bool DropTables()
        {
            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "dropping tables" });
            int countDropped = 0;
            using (var con = new OleDbConnection(_connectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();

                        cmd.CommandText = "DELETE * from dbo_catch_maturity";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_catch_len";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_catch_len_freq";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_catch_len_wt";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_vessel_catch";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_fg_grid";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_gear_soak";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_vessel_effort";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_vessel_unload_stats";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_vessel_unload_1";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_vessel_unload";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_gear_unload";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_LC_FG_sample_day_1";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM dbo_LC_FG_sample_day";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM NSAPRegionLandingSite";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM NSAPRegionFMAFishingGrounds";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        //cmd.CommandText = "DELETE * FROM NSAPRegionFMA";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        cmd.CommandText = "DELETE * FROM NSAPRegionEnumerator";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM NSAPRegionGear";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM NSAPRegionVessel";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        //cmd.CommandText = "DELETE * FROM nsapRegion";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        //cmd.CommandText = "DELETE * FROM fma";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        cmd.CommandText = "DELETE * FROM fishingGround";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM landingSite";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        //cmd.CommandText = "DELETE * FROM Municipalities";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        //cmd.CommandText = "DELETE * FROM Provinces";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        cmd.CommandText = "DELETE * FROM NSAPEnumerator";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM FishingVesselEngine";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM fishingVesselGear";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM FishingVesselGPS";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM engine";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM fishingVessel";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM GearEffortSpecification";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM gps";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM gear";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM effortSpecification";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        cmd.CommandText = "DELETE * FROM kobo_servers";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        //cmd.CommandText = "DELETE * FROM sizeTypes";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        //cmd.CommandText = "DELETE * FROM taxa";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        //cmd.CommandText = "DELETE * FROM phFish";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        cmd.CommandText = "DELETE * FROM notFishSpecies";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                        //cmd.CommandText = "DELETE * FROM FBSpecies";
                        //cmd.ExecuteNonQuery();
                        //countDropped++;

                        cmd.CommandText = "DELETE * FROM JSONFile";
                        cmd.ExecuteNonQuery();
                        countDropped++;

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }


                }
            }
            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "tables dropped" });
            return countDropped == 34;
        }

        public static Task<bool> ImportAsync()
        {
            return Task.Run(() => Import());
        }
        public static bool Import()
        {
            var tablesForImporting = ListSQLDumpParsed
                .Where(t => t.ForParsing)
                .OrderBy(t => t.Sequence).ToList();

            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { TotalTableCount = tablesForImporting.Count, Intent = "begin import" }); ;

            int currentTableCount = 0;
            foreach (var item in tablesForImporting)
            {

                //string sql = MakeInsertSQL(item.AccessTableName, item.Columns);
                currentTableCount++;
                AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "parsing table", CurrentTableName = item.AccessTableName, CurrentRowCount = 0, CurrentTableCount = currentTableCount });
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Join(",", item.Columns));
                Console.WriteLine(sb.ToString());
                foreach (var line in item.DataLines)
                {
                    foreach (string lineItem in line.Split(new[] { "),(" }, StringSplitOptions.None))
                    {
                        var list = SplitQualified(lineItem.Trim(new char[] { ')', '(' }), ',', '\'', true);
                        var new_line = "";
                        //foreach (string list_member in list)
                        //{
                        //    //new_line = "";
                        //    //if(int.TryParse(list_member,out int i) || DateTime.TryParse(list_member,out DateTime d) || list_member=="NULL")
                        //    if (int.TryParse(list_member, out int i) || DateTime.TryParse(list_member, out DateTime d))
                        //    {
                        //        new_line += $"{list_member},";
                        //    }
                        //    else if (list_member == "NULL")
                        //    {
                        //        new_line += ",";
                        //    }
                        //    else
                        //    {
                        //        new_line += $"\"{list_member}\",";
                        //    }
                        //}
                        int loopCount = 0;
                        foreach (string list_member in list)
                        {
                            //new_line = "";
                            if (list_member == "NULL")
                            {
                                new_line += ",";
                            }
                            else
                            {
                                switch (item.Columns[loopCount].MySQLType.Split('(')[0])
                                {
                                    case "int":
                                    case "tinyint":
                                    case "double":
                                    case "datetime":
                                    case "date":
                                        new_line += $"{list_member},";
                                        break;
                                    case "varchar":
                                        new_line += $"\"{list_member}\",";
                                        break;

                                }
                            }
                            loopCount++;
                        }

                        sb.AppendLine(new_line.Trim(','));
                    }
                }

                System.IO.File.WriteAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\temp.csv", sb.ToString());
                Console.WriteLine(sb.ToString());
                if (DoInsertQuery(item.AccessTableName))
                {

                }

                //foreach(var parsed_item in parsed_items)
                //{
                //if(DoInsertQuery(sql,parsed_item,item.Columns))
                //{
                //    count++;
                //    AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { CurrentTableName = item.AccessTableName, Intent = "row imported", CurrentRowCount = count }); 
                //}
                //}
            }
            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "import done" });
            return currentTableCount == tablesForImporting.Count;
        }

        private static List<string> _tablesWithErrorButResumed = new List<string>();
        public static List<string> TablesWithErrorButResumed
        {
            get { return _tablesWithErrorButResumed; }
        }

        private static bool DoInsertQuery(string tableName, string csv)
        {
            bool success = false;

            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string csv_file = $@"{base_dir}\temp.csv";
            File.WriteAllText(csv_file, csv);

            using (var con = new OleDbConnection(ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"INSERT INTO {tableName} SELECT * FROM [Text;FMT=Delimited;DATABASE={base_dir};HDR=yes].[temp.csv]";
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
                        AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "done imported csv", CurrentTableName = tableName, CurrentTableCount = ++_importCSVCount });
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error bulk insert for {tableName}");
                        Logger.Log(ex);

                        if (tableName == "dbo_catch_len" ||
                            tableName == "dbo_catch_len_freq" ||
                            tableName == "dbo_catch_len_wt" ||
                            tableName == "dbo_catch_maturity")
                        {

                            _tablesWithErrorButResumed.Add(tableName);
                        }

                    }
                }

            }
            return success || _tablesWithErrorButResumed.Count > 0;

            //System.Data.OleDb.OleDbCommand AccessCommand = new System.Data.OleDb.OleDbCommand("SELECT * INTO [ImportTable] FROM [Text;FMT=Delimited;DATABASE=C:\\Documents and Settings\\...\\My Documents\\My Database\\Text;HDR=No].[x123456.csv]", AccessConnection);

            //AccessCommand.ExecuteNonQuery();
            //AccessConnection.Close();
        }

        private static bool DoInsertQuery(string tableName)
        {
            bool success = false;
            using (var con = new OleDbConnection(ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"INSERT INTO {tableName} SELECT * FROM [Text;FMT=Delimited;DATABASE={AppDomain.CurrentDomain.BaseDirectory};HDR=yes].[temp.csv]";
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
            return success;

            //System.Data.OleDb.OleDbCommand AccessCommand = new System.Data.OleDb.OleDbCommand("SELECT * INTO [ImportTable] FROM [Text;FMT=Delimited;DATABASE=C:\\Documents and Settings\\...\\My Documents\\My Database\\Text;HDR=No].[x123456.csv]", AccessConnection);

            //AccessCommand.ExecuteNonQuery();
            //AccessConnection.Close();
        }
        private static bool DoInsertQuery(string sql, List<string> col_values, List<AccessColumn> columns)
        {
            bool success = false;
            using (var con = new OleDbConnection(ConnectionString))
            {
                //var col_values = line.Split(',');
                //var col_values = SplitQualified(line, ',', '\'', true);



                using (var cmd = con.CreateCommand())
                {
                    int col_count = 0;
                    foreach (AccessColumn col in columns)
                    {
                        var arr_type = col.MySQLType.Split('(');
                        string column_value = col_values[col_count].Trim(new char[] { '\'', ')', '(' });
                        if (column_value == "NULL")
                        {
                            cmd.Parameters.AddWithValue($"@par_{col.AccessColumnName}", DBNull.Value);
                        }
                        else
                        {
                            try
                            {
                                switch (arr_type[0])
                                {
                                    case "int":
                                        cmd.Parameters.AddWithValue($"@par_{col.AccessColumnName}", int.Parse(column_value));
                                        break;
                                    case "varchar":
                                        cmd.Parameters.AddWithValue($"@par_{col.AccessColumnName}", column_value);
                                        break;
                                    case "tinyint":
                                        cmd.Parameters.AddWithValue($"@par_{col.AccessColumnName}", int.Parse(column_value));
                                        break;
                                    case "double":
                                        cmd.Parameters.AddWithValue($"@par_{col.AccessColumnName}", double.Parse(column_value));
                                        break;
                                    case "datetime":
                                    case "date":
                                        cmd.Parameters.AddWithValue($"@par_{col.AccessColumnName}", DateTime.Parse(column_value));
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }

                        col_count++;
                    }

                    cmd.CommandText = sql;
                    try
                    {
                        con.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (OleDbException oex)
                    {
                        Logger.Log(oex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
            return success;
        }
        private static string MakeInsertSQL(string tableName, List<AccessColumn> columns)
        {
            string sql = $"INSERT INTO {tableName} (";
            foreach (AccessColumn col in columns)
            {
                sql += $"{col.AccessColumnName}, ";
            }
            sql = sql.Trim(new char[] { ',', ' ' }) + ") VALUES (";
            foreach (AccessColumn col in columns)
            {
                sql += $"@par_{col.AccessColumnName}, ";
            }
            sql = sql.Trim(new char[] { ',', ' ' }) + ")";
            return sql;
        }



        /// <summary>
        /// Returns a collection of strings that are derived by splitting the given source string at
        /// characters given by the 'delimiter' parameter.  However, a substring may be enclosed between
        /// pairs of the 'qualifier' character so that instances of the delimiter can be taken as literal
        /// parts of the substring.  The method was originally developed to split comma-separated text
        /// where quotes could be used to qualify text that contains commas that are to be taken as literal
        /// parts of the substring.  For example, the following source:
        ///     A, B, "C, D", E, "F, G"
        /// would be split into 5 substrings:
        ///     A
        ///     B
        ///     C, D
        ///     E
        ///     F, G
        /// When enclosed inside of qualifiers, the literal for the qualifier character may be represented
        /// by two consecutive qualifiers.  The two consecutive qualifiers are distinguished from a closing
        /// qualifier character.  For example, the following source:
        ///     A, "B, ""C"""
        /// would be split into 2 substrings:
        ///     A
        ///     B, "C"
        /// </summary>
        /// <remarks>Originally based on: https://stackoverflow.com/a/43284485/2998072</remarks>
        /// <param name="source">The string that is to be split</param>
        /// <param name="delimiter">The character that separates the substrings</param>
        /// <param name="qualifier">The character that is used (in pairs) to enclose a substring</param>
        /// <param name="toTrim">If true, then whitespace is removed from the beginning and end of each
        /// substring.  If false, then whitespace is preserved at the beginning and end of each substring.
        /// </param>
        public static List<String> SplitQualified(this String source, Char delimiter, Char qualifier,
                                    Boolean toTrim)
        {
            // Avoid throwing exception if the source is null
            if (String.IsNullOrEmpty(source))
                return new List<String> { "" };

            var results = new List<String>();
            var result = new StringBuilder();
            Boolean inQualifier = false;

            // The algorithm is designed to expect a delimiter at the end of each substring, but the
            // expectation of the caller is that the final substring is not terminated by delimiter.
            // Therefore, we add an artificial delimiter at the end before looping through the source string.
            String sourceX = source + delimiter;

            // Loop through each character of the source
            for (var idx = 0; idx < sourceX.Length; idx++)
            {
                // If current character is a delimiter
                // (except if we're inside of qualifiers, we ignore the delimiter)
                if (sourceX[idx] == delimiter && inQualifier == false)
                {
                    // Terminate the current substring by adding it to the collection
                    // (trim if specified by the method parameter)
                    results.Add(toTrim ? result.ToString().Trim() : result.ToString());
                    result.Clear();
                }
                // If current character is a qualifier
                else if (sourceX[idx] == qualifier)
                {
                    // ...and we're already inside of qualifier
                    if (inQualifier)
                    {
                        // check for double-qualifiers, which is escape code for a single
                        // literal qualifier character.
                        if (idx + 1 < sourceX.Length && sourceX[idx + 1] == qualifier)
                        {
                            idx++;
                            result.Append(sourceX[idx]);
                            continue;
                        }
                        // Since we found only a single qualifier, that means that we've
                        // found the end of the enclosing qualifiers.
                        inQualifier = false;
                        continue;
                    }
                    else
                        // ...we found an opening qualifier
                        inQualifier = true;
                }
                // If current character is neither qualifier nor delimiter
                else
                    result.Append(sourceX[idx]);
            }

            return results;
        }

        public static Dictionary<string, List<string>> SQLDumpDictionary { get; set; }

    }
}
