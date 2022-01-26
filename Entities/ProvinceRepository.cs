using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace NSAP_ODK.Entities
{
    internal class ProvinceRepository
    {
        public List<Province> Provinces { get; set; }

        public ProvinceRepository()
        {
            Provinces = getProvinces();
        }

        private List<Province> getProvinces()
        {
            List<Province> listProvinces = new List<Province>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from Provinces order by ProvinceName";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listProvinces.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Province p = new Province();
                            p.ProvinceID = Convert.ToInt32(dr["ProvNo"]);
                            p.ProvinceName = dr["ProvinceName"].ToString();
                            p.RegionCode = dr["NSAPRegion"].ToString();
                            p.SetMunicipalities();
                            listProvinces.Add(p);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                return listProvinces;
            }
        }

        public bool Add(Province p)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into Provinces (ProvNo, ProvinceName, NSAPRegion) Values (?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@prov_no", OleDbType.Integer).Value = p.ProvinceID;
                    update.Parameters.Add("@prov_name", OleDbType.VarChar).Value = p.ProvinceName;
                    update.Parameters.Add("@prov_region", OleDbType.VarChar).Value = p.NSAPRegion.Code;
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

        public bool Update(Province p)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@prov_name", OleDbType.VarChar).Value = p.ProvinceName;
                    update.Parameters.Add("@prov_region", OleDbType.VarChar).Value = p.NSAPRegion.Code;
                    update.Parameters.Add("@prov_no", OleDbType.Integer).Value = p.ProvinceID;

                    update.CommandText = @"Update Provinces set
                                            ProvinceName = @prov_name,
                                            NSAPRegion = @prov_region
                                            WHERE ProvNo = @prov_no";
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

        public bool Delete(int ID)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from Provinces where ProvNo={ID}";
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
                const string sql = "SELECT Max(ProvNo) AS max_record_no FROM Provinces";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}