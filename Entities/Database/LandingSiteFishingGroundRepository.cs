using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;
using System.Linq;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteFishingGroundRepository
    {
        public List<LandingSiteFishingGround> LandingSiteFishingGrounds { get; set; }
        private LandingSite _ls;
        public LandingSiteFishingGroundRepository(LandingSite ls)
        {
            _ls = ls;
            LandingSiteFishingGrounds = GetLandingSiteFishingGrounds();
        }

        private List<LandingSiteFishingGround> GetLandingSiteFishingGrounds()
        {
            List<LandingSiteFishingGround> this_list = new List<LandingSiteFishingGround>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@ls_id", _ls.LandingSiteID);
                        cmd.CommandText = "Select * from landingsite_fishinggrounds where landingsite_id=@ls_id";
                        try
                        {
                            con.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                LandingSiteFishingGround lsfg = new LandingSiteFishingGround
                                {
                                    RowID = (int)dr["row_id"],
                                    LandingSiteID = (int)dr["landingsite_id"],
                                    FishingGroundCode = dr["fishingground_code"].ToString(),
                                    DateAdded = (DateTime)dr["date_added"]
                                };
                                if (dr["date_removed"] != DBNull.Value)
                                {
                                    lsfg.DateRemoved = (DateTime)dr["date_removed"];
                                }
                                this_list.Add(lsfg);

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

        public static bool CheckForLandingSiteFishingGroundTable()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "landingsite_fishinggrounds"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    tableExists = CreateTable("landingsite_fishinggrounds");
                }
            }
            return tableExists;
        }

        private static bool CreateTable(string tableName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    switch (tableName)
                    {
                        case "landingsite_fishinggrounds":
                            cmd.CommandText = @"CREATE TABLE landingsite_fishinggrounds (
                                         row_id INTEGER, 
                                         landingsite_id INTEGER,
                                         fishingground_code VARCHAR(5),
                                         date_added DATETIME,
                                         date_removed DATETIME,
                                         CONSTRAINT PrimaryKey PRIMARY KEY (row_id),
                                         CONSTRAINT FK_lsfg_landingsite
                                             FOREIGN KEY (landingsite_id) REFERENCES
                                             landingSite (LandingSiteID),
                                         CONSTRAINT FK_lsfg_fishinggrounnd
                                             FOREIGN KEY (fishingground_code) REFERENCES
                                             fishingGround (FishingGroundCode)    
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

        public static int MaxRowID()
        {
            int? maxID = null;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT MAX(row_ID) FROM landingsite_fishinggrounds";
                        try
                        {
                            con.Open();
                            var r = cmd.ExecuteScalar();
                            if (r != DBNull.Value)
                            {
                                maxID= (int)r;
                            };
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != "Specified cast is not valid.")
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                }
            }
            if (maxID == null)
            {
                return 0;
            }
            else
            {
                return (int)maxID;
            }
        }
        public bool Delete(int row_id)
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
                        cmd.Parameters.AddWithValue("@id", row_id);
                        cmd.CommandText = @"DELETE * from landingsite_fishinggrounds WHERE row_id = @id";
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            return success;
        }
        public bool Update(LandingSiteFishingGround lsfg)
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
                        cmd.Parameters.AddWithValue("@ls_id", lsfg.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg_code", lsfg.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@added", lsfg.DateAdded);
                        if (lsfg.DateRemoved == null)
                        {
                            cmd.Parameters.AddWithValue("removed", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("removed", lsfg.DateRemoved);
                        }
                        cmd.Parameters.AddWithValue("@id", lsfg.RowID);
                        cmd.CommandText = @"UPDATE landingsite_fishinggrounds SET
                                                landingsite_id = @ls_id,
                                                fishingground_code = @fg_code,
                                                date_added = @added,
                                                date_removed = @removed
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
        public bool Add(LandingSiteFishingGround lsfg)
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
                        cmd.Parameters.Add("@id", OleDbType.Integer).Value = lsfg.RowID;
                        cmd.Parameters.Add("@ls_id", OleDbType.Integer).Value = lsfg.LandingSite.LandingSiteID;
                        cmd.Parameters.Add("@fg_code", OleDbType.VarChar).Value = lsfg.FishingGroundCode;
                        cmd.Parameters.Add("@added", OleDbType.Date).Value = lsfg.DateAdded;
                        if (lsfg.DateRemoved == null)
                        {
                            cmd.Parameters.Add("@removed", OleDbType.Date).Value = DBNull.Value;
                        }
                        else
                        {
                            cmd.Parameters.Add("@removed", OleDbType.Date).Value = lsfg.DateRemoved;
                        }
                        cmd.CommandText = @"INSERT INTO landingsite_fishinggrounds (
                                                row_id,
                                                landingsite_id,
                                                fishingground_code,
                                                date_added,
                                                date_removed
                                                )
                                             VALUES (?,?,?,?,?)";
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            success = true;
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
