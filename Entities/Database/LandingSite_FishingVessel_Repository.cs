using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using System.Data;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSite_FishingVessel_Repository
    {
        public List<LandingSite_FishingVessel> LandingSite_FishingVessels { get; set; }
        public LandingSite_FishingVessel_Repository(LandingSite ls)
        {
            LandingSite_FishingVessels = getLandingSite_FishingVessels(ls);
        }

        public bool RemoveFishingVessel(LandingSite_FishingVessel lsfv,DateTime dateRemoved)
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
                        cmd.Parameters.Add("@dateEnd", OleDbType.Date).Value = dateRemoved;
                        cmd.Parameters.Add("@id", OleDbType.Integer).Value = lsfv.PK;
                        cmd.CommandText = "Update landingsite_fishingvessel set date_removed=@dateEnd Where row_id=@id";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                            if (success)
                            {
                                lsfv.LandingSite.LandingSite_FishingVesselViewModel.LandingSite_FishingVessel_Collection.FirstOrDefault(t => t.PK == lsfv.PK).DateRemoved = dateRemoved;
                                //fg.RegionFMA.FishingGrounds.FirstOrDefault(t => t.FishingGroundCode == fg.FishingGroundCode).DateEnd = dateRemoved;
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

        public bool UnremoveFishingVessel(LandingSite_FishingVessel fv)
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
                        cmd.Parameters.Add("@dateEnd", OleDbType.Date).Value = DBNull.Value;
                        cmd.Parameters.Add("@id", OleDbType.Integer).Value = fv.PK;
                        cmd.CommandText = "Update landingsite_fishingvessel set date_removed=@dateEnd Where row_id=@id";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                            if (success)
                            {
                                fv.LandingSite.LandingSite_FishingVesselViewModel.LandingSite_FishingVessel_Collection.FirstOrDefault(t => t.PK == fv.PK).DateRemoved = null;
                                //fg.RegionFMA.FishingGrounds.FirstOrDefault(t => t.FishingGroundCode == fg.FishingGroundCode).DateEnd = dateRemoved;
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

        public static int GetMaxRecordNumber()
        {
            int maxRecordNumber = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select Max(row_id) from landingsite_fishingvessel";
                        try
                        {
                            con.Open();
                            maxRecordNumber = (int)cmd.ExecuteScalar();

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return maxRecordNumber;
        }
        public static bool CheckTableExists()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "landingsite_fishingvessel"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    return CreateTable();
                }
                else
                {
                    return true;
                }
            }
        }
        public static bool CreateTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE landingsite_fishingvessel (
                                            row_id Int NOT NULL PRIMARY KEY,
                                            landing_site_id Int,
                                            fishing_vessel_id Int,
                                            date_added DateTime,
                                            date_removed DateTime,
                                            CONSTRAINT FK_ls_id
                                                FOREIGN KEY(landing_site_id) REFERENCES landingSite(LandingSiteID),
                                            CONSTRAINT FK_fv_id   
                                                FOREIGN KEY (fishing_vessel_id) REFERENCES fishingVessel(VesselID)
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
                        cmd.CommandText = "DELETE * FROM landingsite_fishingvessel WHERE row_id=@id";
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
        public bool Update(LandingSite_FishingVessel lf)
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
                        cmd.Parameters.AddWithValue("@ls_id", lf.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fv_id", lf.FishingVessel.ID);
                        cmd.Parameters.AddWithValue("@id", lf.PK);
                        cmd.CommandText = @"UPDATE landingsite_fishingvessel SET
                                                landing_site_id = @ls_id,
                                                fishing_vessel_id = @fv_id
                                            WHERE
                                                row_id = @id";

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
        public bool Add(LandingSite_FishingVessel lf)
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
                        cmd.Parameters.AddWithValue("@ls_id", lf.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fv_id", lf.FishingVessel.ID);
                        cmd.Parameters.AddWithValue("@id", lf.PK);
                        cmd.Parameters.Add("@added", OleDbType.Date).Value = lf.DateAdded;
                        cmd.CommandText = @"INSERT INTO landingsite_fishingvessel (
                                                landing_site_id,
                                                fishing_vessel_id,
                                                row_id,
                                                date_added
                                                )
                                             VALUES(?,?,?,?)";
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
        public List<LandingSite_FishingVessel> getLandingSite_FishingVessels(LandingSite ls)
        {
            List<LandingSite_FishingVessel> thisList = new List<LandingSite_FishingVessel>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@ls_id", ls.LandingSiteID);
                        cmd.CommandText = "Select * from landingsite_fishingvessel where landing_site_id=@ls_id";
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                DateTime? removed = null;
                                if(dr["date_removed"]!=DBNull.Value)
                                {
                                    removed = (DateTime)dr["date_removed"];
                                }
                                LandingSite_FishingVessel lf = new LandingSite_FishingVessel
                                {
                                    FishingVessel = NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)dr["fishing_vessel_id"]),
                                    LandingSite = ls,
                                    PK = (int)dr["row_id"],
                                    DateAdded = (DateTime)dr["date_added"],
                                    DateRemoved = removed
                                };
                                thisList.Add(lf);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }

            }
            return thisList;
        }
    }
}
