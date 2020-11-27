using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace NSAP_ODK.Entities
{
    public class LandingSiteRepository

    {
        public List<LandingSite> landingSites { get; set; }

        public LandingSiteRepository()
        {
            landingSites = getLandingSites();
        }

        private List<LandingSite> getLandingSites()
        {
            List<LandingSite> listLandingSites = new List<LandingSite>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = @"SELECT landingSite.LandingSiteID, landingSite.LandingSiteName, 
                                    landingSite.Municipality, Municipalities.ProvNo,
                                    landingSite.Latitude, landingSite.Longitude, landingSite.Barangay
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
                            listLandingSites.Add(ls);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return listLandingSites;
        }

        public bool Add(LandingSite ls)
        {
            string lat = ls.Latitude == null ? "null" : $"{ls.Latitude.ToString()}";
            string lon = ls.Longitude == null ? "null" : $"{ls.Longitude.ToString()}";
            string brgy = ls.Barangay == null ? "null" : $"'{ls.Barangay}'";
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into LandingSite (LandingSiteID, LandingSiteName,Municipality,Longitude,Latitude,Barangay)
                           Values
                           ({ls.LandingSiteID},'{ls.LandingSiteName}',{ls.Municipality.MunicipalityID},{lon},{lat},{brgy})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(LandingSite ls)
        {
            string lat = ls.Latitude == null ? "null" : $"{ls.Latitude.ToString()}";
            string lon = ls.Longitude == null ? "null" : $"{ls.Longitude.ToString()}";
            string brgy = ls.Barangay == null ? "null" : $"'{ls.Barangay}'";
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update LandingSite set
                                LandingSiteName = '{ls.LandingSiteName}',
                                Municipality = {ls.Municipality.MunicipalityID},
                                Longitude = {lon},
                                Latitude = {lat},
                                Barangay = {brgy}
                            WHERE LandingSiteID={ls.LandingSiteID}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from LandingSite where LandingSiteId={id}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
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

        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(LandingSiteId) AS max_record_no FROM LandingSite";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}