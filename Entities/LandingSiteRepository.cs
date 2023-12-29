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
namespace NSAP_ODK.Entities
{
    public class LandingSiteRepository

    {
        public List<LandingSite> landingSites { get; set; }

        public LandingSiteRepository()
        {
            landingSites = getLandingSites();
        }
        private List<LandingSite> getFromMySQL()
        {
            List<LandingSite> thisList = new List<LandingSite>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"SELECT ls.landing_site_id, ls.landing_site_name, 
                                    ls.municipality, mun.prov_no,
                                    ls.latitude, ls.longitude, ls.barangay
                                    FROM municipalities as mun INNER JOIN landing_sites as ls ON mun.mun_no = ls.municipality";

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Province province = NSAPEntities.ProvinceViewModel.GetProvince(Convert.ToInt32(dr["prov_no"]));
                        LandingSite ls = new LandingSite();
                        ls.LandingSiteID = Convert.ToInt32(dr["landing_site_id"]);
                        ls.LandingSiteName = dr["landing_site_name"].ToString();
                        if (dr["municipality"].ToString().Length > 0)
                        {
                            ls.Municipality = province.Municipalities.GetMunicipality(Convert.ToInt32(dr["municipality"]));
                        }

                        if (dr["barangay"].GetType().Name != "DBNull")
                        {
                            ls.Barangay = dr["barangay"].ToString();
                        }

                        if (dr["latitude"].GetType().Name != "DBNull")
                        {
                            ls.Latitude = Convert.ToDouble(dr["latitude"]);
                        }
                        if (dr["longitude"].GetType().Name != "DBNull")
                        {
                            ls.Longitude = Convert.ToDouble(dr["longitude"]);
                        }
                        thisList.Add(ls);
                    }
                }
            }
            return thisList;
        }

        public static Task<bool> AddFieldToTableAsync(string fieldName)
        {
            return Task.Run(() => AddFieldToTable(fieldName));
        }
        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "TypeOfSampling":
                    sql = $"ALTER TABLE landingSite ADD COLUMN {fieldName} INTEGER";
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
                        if(success && fieldName=="TypeOfSampling")
                        {
                            success = UpdateSamplingTypeField();
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

        private static bool UpdateSamplingTypeField()
        {
            bool success = false;
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE landingSite SET TypeOfSampling=0 WHERE TypeOfSampling IS NULL";
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
                return success;
        }
        private List<LandingSite> getLandingSites()
        {
            List<LandingSite> listLandingSites = new List<LandingSite>();
            if (Global.Settings.UsemySQL)
            {
                listLandingSites = getFromMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = @"SELECT landingSite.LandingSiteID, landingSite.LandingSiteName, 
                                    landingSite.Municipality, Municipalities.ProvNo,
                                    landingSite.Latitude, landingSite.Longitude, landingSite.Barangay, landingsite.TypeOfSampling
                                    FROM Municipalities INNER JOIN landingSite ON Municipalities.MunNo = landingSite.Municipality";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listLandingSites.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                Province province = NSAPEntities.ProvinceViewModel.GetProvince(Convert.ToInt32(dr["ProvNo"]));
                                LandingSite ls = new LandingSite();
                                ls.LandingSiteID = Convert.ToInt32(dr["LandingSiteId"]);
                                ls.LandingSiteName = dr["LandingSiteName"].ToString();
                                if (dr["Municipality"].ToString().Length > 0)
                                {
                                    ls.Municipality = province.Municipalities.GetMunicipality(Convert.ToInt32(dr["Municipality"]));
                                }

                                if (dr["Barangay"].GetType().Name != "DBNull")
                                {
                                    ls.Barangay = dr["Barangay"].ToString();
                                }

                                if (dr["Latitude"].GetType().Name != "DBNull")
                                {
                                    ls.Latitude = Convert.ToDouble(dr["Latitude"]);
                                }
                                if (dr["Longitude"].GetType().Name != "DBNull")
                                {
                                    ls.Longitude = Convert.ToDouble(dr["Longitude"]);
                                }
                                ls.LandingSiteTypeOfSampling = (LandingSiteTypeOfSampling)(int)dr["TypeOfSampling"];
                                //ls.LandingSite_FishingVesselViewModel = null;
                                //ls.LandingSiteFishingGroundViewModel = new LandingSiteFishingGroundViewModel(ls);
                                listLandingSites.Add(ls);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }

            return listLandingSites;
        }

        private bool AddToMySQL(LandingSite ls)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = ls.LandingSiteID;
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = ls.LandingSiteName;
                    update.Parameters.Add("@muni", MySqlDbType.Int32).Value = ls.Municipality.MunicipalityID;
                    if (ls.Longitude == null || ls.Latitude == null)
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = DBNull.Value;
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = ls.Longitude;
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = ls.Latitude;
                    }
                    if (ls.Barangay == null)
                    {
                        update.Parameters.Add("@brgy", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@brgy", MySqlDbType.VarChar).Value = ls.Barangay;
                    }
                    update.CommandText =@"Insert into landing_sites (landing_site_id, landing_site_name,municipality,longitude,latitude,barangay) 
                                         Values (@id,@name,@muni,@lon,@lat,@brgy)";
                    conn.Open();
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                }
            }
            return success;
        }
        public bool Add(LandingSite ls)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(ls);
            }
            else
            {

                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into LandingSite (LandingSiteID, LandingSiteName,Municipality,Longitude,Latitude,Barangay,TypeOfSampling) Values (?,?,?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = ls.LandingSiteID;
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = ls.LandingSiteName;
                        update.Parameters.Add("@muni", OleDbType.Integer).Value = ls.Municipality.MunicipalityID;
                        if (ls.Longitude == null || ls.Latitude == null)
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = DBNull.Value;
                            update.Parameters.Add("@lat", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = ls.Longitude;
                            update.Parameters.Add("@lat", OleDbType.Double).Value = ls.Latitude;
                        }
                        if (ls.Barangay == null)
                        {
                            update.Parameters.Add("@brgy", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@brgy", OleDbType.VarChar).Value = ls.Barangay;
                        }
                        update.Parameters.Add("@sampling_type", OleDbType.Integer).Value = (int)ls.LandingSiteTypeOfSampling;
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            Logger.Log(dbex);
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

       
        private bool UpdateMySQL(LandingSite ls)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = ls.LandingSiteName;
                    update.Parameters.Add("@muni", MySqlDbType.Int32).Value = ls.Municipality.MunicipalityID;
                    if (ls.Longitude == null || ls.Latitude == null)
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = DBNull.Value;
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@lon", MySqlDbType.Double).Value = ls.Longitude;
                        update.Parameters.Add("@lat", MySqlDbType.Double).Value = ls.Latitude;
                    }
                    if (ls.Barangay == null)
                    {
                        update.Parameters.Add("@brgy", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@brgy", MySqlDbType.VarChar).Value = ls.Barangay;
                    }
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = ls.LandingSiteID;

                    update.CommandText = @"Update landing_sites set
                                            landing_site_name = @name,
                                            municipality = @muni,
                                            longitude = @lon,
                                            latitude = @lat,
                                            barangay = @brgy
                                            WHERE landing_site_id=@id";

                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Update(LandingSite ls)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(ls);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@name", OleDbType.VarChar).Value = ls.LandingSiteName;
                        update.Parameters.Add("@muni", OleDbType.Integer).Value = ls.Municipality.MunicipalityID;
                        if (ls.Longitude == null || ls.Latitude == null)
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = DBNull.Value;
                            update.Parameters.Add("@lat", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@lon", OleDbType.Double).Value = ls.Longitude;
                            update.Parameters.Add("@lat", OleDbType.Double).Value = ls.Latitude;
                        }
                        if (ls.Barangay == null)
                        {
                            update.Parameters.Add("@brgy", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@brgy", OleDbType.VarChar).Value = ls.Barangay;
                        }
                        update.Parameters.Add("@sampling_type", OleDbType.Integer).Value = (int)ls.LandingSiteTypeOfSampling;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = ls.LandingSiteID;

                        update.CommandText = @"Update LandingSite set
                                            LandingSiteName = @name,
                                            Municipality = @muni,
                                            Longitude = @lon,
                                            Latitude = @lat,
                                            Barangay = @brgy,
                                            TypeOfSampling = @sampling_type
                                            WHERE LandingSiteID=@id";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            Logger.Log(dbex);
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from landing_sites where landing_site_id=@id";
                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex);
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
                success = DeleteMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from LandingSite where LandingSiteId=@id";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            Logger.Log(dbex);
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
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(landing_site_id) AS max_id FROM landing_sites";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(LandingSiteId) AS max_record_no FROM LandingSite";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
    }
}