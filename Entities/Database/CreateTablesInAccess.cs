using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using Microsoft.VisualBasic.FileIO;
namespace NSAP_ODK.Entities.Database
{
    public static class CreateTablesInAccess
    {
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

        public static Task<bool> DropTablesAsync()
        {
            return Task.Run(() => DropTables());
        }
        public static bool DropTables()
        {
            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent="dropping tables" });
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
            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent="tables dropped" });
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

            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { TotalTableCount = tablesForImporting.Count ,Intent="begin import"}); ;

            int currentTableCount = 0;
            foreach (var item in tablesForImporting)
            {
                string sql = MakeInsertSQL(item.AccessTableName, item.Columns);
                currentTableCount++;
                AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent="parsing table", CurrentTableName = item.AccessTableName, CurrentRowCount = 0, CurrentTableCount = currentTableCount });
                int count = 0;
                foreach (var line in item.DataLines)
                {
                    foreach (string lineItem in line.Split(new[] { "),(" }, StringSplitOptions.None))
                    {
                        if (DoInsertQuery(sql, lineItem.Trim(new char[] { '(', ')' }), item.Columns))
                        {
                            count++;
                            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { CurrentTableName=item.AccessTableName, Intent="row imported", CurrentRowCount = count }); ;
                        }
                    }
                }
            }
            AccessTableEvent?.Invoke(null, new CreateTablesInAccessEventArgs { Intent = "import done" });
            return currentTableCount == tablesForImporting.Count;
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
        private static bool DoInsertQuery(string sql, string line, List<AccessColumn> columns)
        {
            bool success = false;
            using (var con = new OleDbConnection(ConnectionString))
            {
                //var col_values = line.Split(',');
                var col_values = SplitQualified(line, ',', '\'', true);



                using (var cmd = con.CreateCommand())
                {
                    int col_count = 0;
                    foreach (AccessColumn col in columns)
                    {
                        var arr_type = col.MySQLType.Split('(');
                        string column_value = col_values[col_count].Trim('\'');
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
        public static Dictionary<string, List<string>> SQLDumpDictionary { get; set; }

    }
}
