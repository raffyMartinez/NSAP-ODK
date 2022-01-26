﻿using NSAP_ODK.Utilities;
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
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into LandingSite (LandingSiteID, LandingSiteName,Municipality,Longitude,Latitude,Barangay) Values (?,?,?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = ls.LandingSiteID;
                    update.Parameters.Add("@name", OleDbType.VarChar).Value = ls.LandingSiteName;
                    update.Parameters.Add("@muni", OleDbType.Integer).Value = ls.Municipality.MunicipalityID;
                    if(ls.Longitude==null || ls.Latitude==null)
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
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch(OleDbException dbex)
                    {
                        Logger.Log(dbex);
                    }
                    catch(Exception ex)
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
                    update.Parameters.Add("@id", OleDbType.Integer).Value = ls.LandingSiteID;
                    
                    update.CommandText = @"Update LandingSite set
                                            LandingSiteName = @name,
                                            Municipality = @muni,
                                            Longitude = @lon,
                                            Latitude = @lat,
                                            Barangay = @brgy
                                            WHERE LandingSiteID=@id";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch(OleDbException dbex)
                    {
                        Logger.Log(dbex);
                    }
                    catch(Exception ex)
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