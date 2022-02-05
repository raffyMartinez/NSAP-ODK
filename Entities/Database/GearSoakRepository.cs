﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities.Database
{
    public class GearSoakRepository
    {
        public List<GearSoak> GearSoaks { get; set; }

        public GearSoakRepository()
        {
            GearSoaks = getGearSoaks();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(gear_soak_id) AS max_id FROM dbo_gear_soak";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<GearSoak> getFromMySQL()
        {
            List<GearSoak> thisList = new List<GearSoak>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_gear_soak";

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        GearSoak item = new GearSoak();
                        item.PK = (int)dr["gear_soak_id"];
                        item.VesselUnloadID = (int)dr["v_unload_id"];
                        item.TimeAtSet = (DateTime)dr["time_set"];
                        item.TimeAtHaul = (DateTime)dr["time_hauled"];
                        item.WaypointAtSet = dr["wpt_set"].ToString();
                        item.WaypointAtHaul = dr["wpt_haul"].ToString();
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<GearSoak> getGearSoaks()
        {
            List<GearSoak> thisList = new List<GearSoak>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = "Select * from dbo_gear_soak";
                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            thisList.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                GearSoak item = new GearSoak();
                                item.PK = (int)dr["gear_soak_id"];
                                item.VesselUnloadID = (int)dr["v_unload_id"];
                                item.TimeAtSet = (DateTime)dr["time_set"];
                                item.TimeAtHaul = (DateTime)dr["time_hauled"];
                                item.WaypointAtSet = dr["wpt_set"].ToString();
                                item.WaypointAtHaul = dr["wpt_haul"].ToString();
                                thisList.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);

                    }

                }
            }
            return thisList;
        }

        public bool Add(GearSoak item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into dbo_gear_soak(v_unload_id, time_set,time_hauled,wpt_set,wpt_haul,gear_soak_id) Values (?,?,?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {

                    update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                    update.Parameters.Add("@time_set", OleDbType.Date).Value = item.TimeAtSet;
                    update.Parameters.Add("@time_haul", OleDbType.Date).Value = item.TimeAtHaul;
                    update.Parameters.Add("@wpt_set", OleDbType.VarChar).Value = item.WaypointAtSet;
                    update.Parameters.Add("@wpt_haul", OleDbType.VarChar).Value = item.WaypointAtHaul;
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
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
                    }
                }
            }
            return success;
        }

        public bool Update(GearSoak item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                    update.Parameters.Add("@time_set", OleDbType.Date).Value = item.TimeAtSet;
                    update.Parameters.Add("@time_haul", OleDbType.Date).Value = item.TimeAtHaul;
                    update.Parameters.Add("@wpt_set", OleDbType.VarChar).Value = item.WaypointAtSet;
                    update.Parameters.Add("@wpt_haul", OleDbType.VarChar).Value = item.WaypointAtHaul;
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                    update.CommandText = @"Update dbo_gear_soak set
                                            v_unload_id=@unload_id,
                                            time_set = @time_set,
                                            time_hauled = @time_haul,
                                            wpt_set = @wpt_set,
                                            wpt_haul=@wpt_haul
                                        WHERE gear_soak_id = @id";

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
                    }
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }
        public bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_gear_soak";
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
        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText = "Delete * from dbo_gear_soak where gear_soak_id=@id";
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
