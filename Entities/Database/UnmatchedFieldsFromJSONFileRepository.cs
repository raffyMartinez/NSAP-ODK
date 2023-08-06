using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using Microsoft.VisualBasic.FileIO;

namespace NSAP_ODK.Entities.Database
{
    public class UnmatchedFieldsFromJSONFileRepository
    {
        public UnmatchedFieldsFromJSONFileRepository()
        {
            UnmatchedFieldsFromJSONFiles = getItems();
        }

        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "json_file_id":
                    sql = $"ALTER TABLE dbo_json_fields_mismatch ADD COLUMN {fieldName} int";
                    break;
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sql;
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
                var sql = $"Delete * from dbo_json_fields_mismatch";
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
        public List<UnmatchedFieldsFromJSONFile> UnmatchedFieldsFromJSONFiles { get; private set; }
        private List<UnmatchedFieldsFromJSONFile> getItems()
        {
            List<UnmatchedFieldsFromJSONFile> this_list = new List<UnmatchedFieldsFromJSONFile>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_json_fields_mismatch Order by row_id";
                        try
                        {
                            con.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                UnmatchedFieldsFromJSONFile item = new UnmatchedFieldsFromJSONFile
                                {
                                    RowID = (int)dr["row_id"],
                                    LandingSiteIDs = ListFromStringOfInt(dr["landing_site_ids"].ToString()),
                                    LandingSiteNames = ListFromStringOfStrings(dr["landing_site_names"].ToString()),
                                    EnumeratorIDs = ListFromStringOfInt(dr["enumerator_ids"].ToString()),
                                    EnumeratorNames = ListFromStringOfStrings(dr["enumerator_names"].ToString()),
                                    FishingGearCodes = ListFromStringOfStrings(dr["gear_codes"].ToString()),
                                    FishingGearNames = ListFromStringOfStrings(dr["gear_names"].ToString()),
                                    SpeciesIDs = ListFromStringOfInt(dr["species_ids"].ToString()),
                                    SpeciesNamesTaxa = ListFromStringOfStrings(dr["species_names"].ToString()),
                                    JSONFileName = dr["json_filename"].ToString(),
                                    NSAPRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(dr["region_id"].ToString()),
                                    DateStart = (DateTime)dr["date_start"],
                                    DateEnd = (DateTime)dr["date_end"],
                                    DateOfParsing = (DateTime)dr["date_parsed"]
                                };
                                this_list.Add(item);
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

        private List<string> ListFromStringOfStrings(string strings)
        {
            List<string> this_list = new List<string>();
            using (var stream = Global.GenerateStreamFromString(strings))
            {
                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Process row
                        string[] fields = parser.ReadFields();
                        foreach (string field in fields)
                        {
                            this_list.Add(field);
                        }
                    }
                }
            }
            //foreach (var item in strings.Split(','))
            //{
            //    this_list.Add(item.Split('\"')[0]);
            //}
            return this_list;
        }
        private List<int> ListFromStringOfInt(string ints)
        {
            List<int> this_list = new List<int>();
            using (var stream = Global.GenerateStreamFromString(ints))
            {
                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Process row
                        string[] fields = parser.ReadFields();
                        foreach (string field in fields)
                        {
                            this_list.Add(int.Parse(field));
                        }
                    }
                }
            }



            //List<int> this_list = new List<int>();
            //foreach (var item in ints.Split(','))
            //{
            //    this_list.Add(int.Parse(item));
            //}

            return this_list;
        }

        public bool AddItem(UnmatchedFieldsFromJSONFile item)
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
                        //cmd.Parameters.Add("@id", OleDbType. item.RowID);
                        cmd.Parameters.Add("@id", OleDbType.Integer).Value = item.RowID;
                        cmd.Parameters.Add("@ls_ids", OleDbType.VarChar).Value = item.AllLandingSiteIDs;
                        cmd.Parameters.Add("@ls_names", OleDbType.VarChar).Value = item.AllLandingSiteNames;
                        cmd.Parameters.Add("@enum_ids", OleDbType.VarChar).Value = item.AllEnumeratorIDs;
                        cmd.Parameters.Add("@enum_names", OleDbType.VarChar).Value = item.AllEnumeratorNames;
                        cmd.Parameters.Add("@gear_ids", OleDbType.VarChar).Value = item.AllGearIDs;
                        cmd.Parameters.Add("@gear_names", OleDbType.VarChar).Value = item.AllGearNames;
                        cmd.Parameters.Add("@species_names_taxa", OleDbType.VarChar).Value = item.AllSpeciesTaxa;
                        cmd.Parameters.Add("@date_start", OleDbType.Date).Value = item.DateStart;
                        cmd.Parameters.Add("@date_end", OleDbType.Date).Value = item.DateEnd;
                        cmd.Parameters.Add("@date_parse", OleDbType.Date).Value = item.DateOfParsing;
                        cmd.Parameters.Add("@json_filename", OleDbType.VarChar).Value = item.JSONFileName;
                        cmd.Parameters.Add("@jsonfile_id", OleDbType.Integer).Value = item.JsonFileID;
                        cmd.Parameters.Add("@region_id", OleDbType.VarChar).Value = item.NSAPRegion.Code;
                        // cmd.Parameters.AddWithValue("@ls_ids", item.AllLandingSiteIDs);
                        // cmd.Parameters.AddWithValue("@ls_names", item.AllLandingSiteNames);
                        // cmd.Parameters.AddWithValue("@enum_ids", item.AllEnumeratorIDs);
                        // cmd.Parameters.AddWithValue("@enum_names", item.AllEnumeratorNames);
                        // cmd.Parameters.AddWithValue("@gear_ids", item.AllGearIDs);
                        // cmd.Parameters.AddWithValue("@gear_names", item.AllGearNames);
                        //// cmd.Parameters.AddWithValue("@species_ids", item.AllSpeciesIDs);
                        // cmd.Parameters.AddWithValue("@species_names_taxa", item.AllSpeciesTaxa);
                        // cmd.Parameters.AddWithValue("@date_start", item.DateStart);
                        // cmd.Parameters.AddWithValue("@date_end", item.DateEnd);
                        // cmd.Parameters.AddWithValue("@date_parse", item.DateOfParsing);
                        // cmd.Parameters.AddWithValue("@json_filename", item.JSONFileName);
                        // cmd.Parameters.AddWithValue("@region_id", item.NSAPRegion.Code);

                        cmd.CommandText = @"INSERT INTO dbo_json_fields_mismatch 
                                            (row_id,
                                            landing_site_ids,
                                            landing_site_names,
                                            enumerator_ids,
                                            enumerator_names,
                                            gear_codes,
                                            gear_names,
                                            species_names,
                                            date_start,
                                            date_end,
                                            date_parsed,
                                            json_filename,
                                            json_file_id,
                                            region_id)
                                            VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
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
        public bool Update(UnmatchedFieldsFromJSONFile item)
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
                        cmd.Parameters.AddWithValue("@ls_ids", item.AllLandingSiteIDs);
                        cmd.Parameters.AddWithValue("@ls_names", item.AllLandingSiteNames);
                        cmd.Parameters.AddWithValue("@enum_ids", item.AllEnumeratorIDs);
                        cmd.Parameters.AddWithValue("@enum_names", item.AllEnumeratorNames);
                        cmd.Parameters.AddWithValue("@gear_ids", item.AllGearIDs);
                        cmd.Parameters.AddWithValue("@gear_names", item.AllEnumeratorIDs);
                        cmd.Parameters.AddWithValue("@species_ids", item.AllSpeciesIDs);
                        cmd.Parameters.AddWithValue("@species_names", item.AllSpeciesNames);
                        cmd.Parameters.AddWithValue("@date_start", item.DateStart);
                        cmd.Parameters.AddWithValue("@date_end", item.DateEnd);
                        cmd.Parameters.AddWithValue("@date_parse", item.DateOfParsing);
                        cmd.Parameters.AddWithValue("@json_filename", item.JSONFileName);
                        cmd.Parameters.AddWithValue("@region_id", item.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@id", item.RowID);

                        cmd.CommandText = @"UPDATE dbo_json_fields_mismatch SET
                                            landing_site_ids = @ls_ids,
                                            landing_site_names = @ls_names,
                                            enumerator_ids = @enum_ids,
                                            enumerator_names = @enum_names,
                                            gear_codes = @gear_ids,
                                            gear_names = @gear_names,
                                            species_ids = @species_ids,
                                            species_names = @species_names,
                                            date_start = @date_start,
                                            date_end = @date_end,
                                            date_parsed = @date_parse,
                                            json_filename = @json_filename,
                                            region_id = @region_id,
                                            WHERE row_id = @id";

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
        public bool Delete(int id)
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
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.CommandText = "Delete * from dbo_json_fields_mismatch where row_id=@id";
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

        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(row_id) AS max_record_no FROM dbo_json_fields_mismatch";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return max_rec_no;
        }
        public static bool CheckTableExist()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_json_fields_mismatch"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    tableExists = CreateTable();
                }

                return tableExists;
            }
        }

        public static bool CreateTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE dbo_json_fields_mismatch (
                                                row_id INTEGER, 
                                                landing_site_ids MEMO,
                                                landing_site_names MEMO,
                                                enumerator_ids MEMO,
                                                enumerator_names MEMO,
                                                gear_codes MEMO,
                                                gear_names MEMO,
                                                species_ids MEMO,
                                                species_names MEMO,
                                                date_start DATETIME,
                                                date_end DATETIME,
                                                date_parsed DATETIME,
                                                json_filename MEMO,
                                                region_id VARCHAR(6),
                                                CONSTRAINT PrimaryKey PRIMARY KEY (row_id),
                                                CONSTRAINT FK_region_id
                                                    FOREIGN KEY (region_id) REFERENCES
                                                    nsapRegion (code)
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
    }
}
