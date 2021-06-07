using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities
{
    class GPSRepository
    {
        public List<GPS> GPSes { get; set; }

        public GPSRepository()
        {
            GPSes = getGPSes();
        }

        private List<GPS> getGPSes()
        {
            List<GPS> listGPS = new List<GPS>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from gps";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listGPS.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            GPS gps = new GPS();
                            gps.Code = dr["GPSCode"].ToString();
                            gps.AssignedName = dr["AssignedName"].ToString();
                            gps.Brand = dr["Brand"].ToString();
                            gps.Model = dr["Model"].ToString();
                            if (dr["DeviceType"] != null && dr["DeviceType"].ToString().Length > 0)
                            {
                                gps.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), dr["DeviceType"].ToString());
                            }
                            else
                            {
                                gps.DeviceType = DeviceType.DeviceTypeGPS;
                            }
                            listGPS.Add(gps);
                        }
                    }
                }
                catch (OleDbException dbex)
                {
                    //switch(dbex.HResult)
                    //{
                    //    case -1:
                    //        break;
                    //}
                }
                catch (Exception ex)
                {
                    switch (ex.HResult)
                    {
                        case -2147024809:
                            if (AddColumn("DeviceType", "int") && UpdateAllDevicesToGPS())
                            {

                                return getGPSes();
                            }

                            break;
                        default:
                            Logger.Log(ex);
                            break;
                    }

                }
                return listGPS;
            }
        }

        private bool UpdateAllDevicesToGPS()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update gps set DeviceType = 1";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }
        private bool AddColumn(string colName, string type, int? length = null)
        {
            string sql = "";
            if (type == "bool")
            {
                sql = $"ALTER TABLE gps ADD COLUMN {colName} BIT DEFAULT 0";
            }
            else
            {
                if (length == null)
                {
                    sql = $"ALTER TABLE gps ADD COLUMN {colName} {type}";
                }
                else
                {
                    sql = $"ALTER TABLE gps ADD COLUMN {colName} {type}({length})";
                }
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                con.Open();
                OleDbCommand myCommand = new OleDbCommand();
                myCommand.Connection = con;
                myCommand.CommandText = sql;
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return false;
                }
                myCommand.Connection.Close();
                return true;
            }
        }
        public bool Add(GPS gps)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into gps(GPSCode,AssignedName,Brand,Model,DeviceType)
                           Values ('{gps.Code}','{gps.AssignedName}', '{gps.Brand}','{gps.Model}',{(int)gps.DeviceType})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(GPS gps)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update gps set
                                AssignedName= '{gps.AssignedName}',
                                Brand = '{gps.Brand}',
                                Model = '{gps.Model}',
                                DeviceType = {(int)gps.DeviceType}    
                            WHERE GPSCode = '{gps.Code}'";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(string code)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from gps where GPSCode='{code}'";
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
    }
}
