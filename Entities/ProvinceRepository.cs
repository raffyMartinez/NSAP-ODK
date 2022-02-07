using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    internal class ProvinceRepository
    {
        public List<Province> Provinces { get; set; }

        public ProvinceRepository()
        {
            Provinces = getProvinces();
        }
        private List<Province> getFromMySQL()
        {
            List<Province> thisList = new List<Province>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from provinces";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Province p = new Province();
                        p.ProvinceID = Convert.ToInt32(dr["prov_no"]);
                        p.ProvinceName = dr["province_name"].ToString();
                        p.RegionCode = dr["nsap_region"].ToString();
                        p.SetMunicipalities();
                        thisList.Add(p);
                    }
                }
            }
            return thisList;
        }
        private List<Province> getProvinces()
        {
            List<Province> listProvinces = new List<Province>();
            if (Global.Settings.UsemySQL)
            {
                listProvinces = getFromMySQL();
            }
            else
            {
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
                }

            }
            return listProvinces;
        }
        private bool AddToMySQL(Province p)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@prov_no", MySqlDbType.Int32).Value = p.ProvinceID;
                    update.Parameters.Add("@prov_name", MySqlDbType.VarChar).Value = p.ProvinceName;
                    update.Parameters.Add("@prov_region", MySqlDbType.VarChar).Value = p.NSAPRegion.Code;
                    update.CommandText = "Insert into Provinces (prov_no, province_name, nsap_region) Values (@prov_no,@prov_name,@prov_region)";
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
        public bool Add(Province p)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(p);
            }
            else
            {
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

        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText = "Delete * from Provinces where ProvNo=@id";
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
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(prov_no) AS max_id FROM provinces";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(ProvNo) AS max_record_no FROM Provinces";
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