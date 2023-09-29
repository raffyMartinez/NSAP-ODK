using System;
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

        public static Task<bool> DeleteMultivesselDataAsync()
        {
            return Task.Run(() => DeleteMultivesselData());
        }
        public static bool DeleteMultivesselData()
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
                        cmd.CommandText = @"DELETE dbo_gear_soak.*
                                            FROM ((dbo_gear_unload INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                dbo_gear_soak ON dbo_vessel_unload.v_unload_id = dbo_gear_soak.v_unload_id
                                            WHERE dbo_LC_FG_sample_day_1.is_multivessel = True";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() >= 0;
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
        public GearSoakRepository(VesselUnload vu)
        {
            GearSoaks = getGearSoaks(vu);
        }
        public GearSoakRepository(bool isNew = false)
        {
            if (!isNew)
            {
                GearSoaks = getGearSoaks();
            }
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
                        cmd.CommandText = "SELECT Max(gear_soak_id) AS max_id FROM dbo_gear_soak";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(gear_soak_id) AS max_id FROM dbo_gear_soak";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        private List<GearSoak> getFromMySQL(VesselUnload vu = null)
        {
            List<GearSoak> thisList = new List<GearSoak>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_gear_soak";
                    if (vu != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vu.PK);
                        cmd.CommandText = "Select * from dbo_gear_soak where v_unload_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        GearSoak item = new GearSoak();
                        item.Parent = vu;
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
        private List<GearSoak> getGearSoaks(VesselUnload vu = null)
        {
            List<GearSoak> thisList = new List<GearSoak>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vu);
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            cmd.CommandText = "Select * from dbo_gear_soak";
                            if (vu != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vu.PK);
                                cmd.CommandText = "Select * from dbo_gear_soak where v_unload_id=@parentID";
                            }

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                GearSoak item = new GearSoak();
                                item.Parent = vu;
                                item.PK = (int)dr["gear_soak_id"];
                                item.VesselUnloadID = (int)dr["v_unload_id"];
                                item.TimeAtSet = (DateTime)dr["time_set"];
                                item.TimeAtHaul = (DateTime)dr["time_hauled"];
                                item.WaypointAtSet = dr["wpt_set"].ToString();
                                item.WaypointAtHaul = dr["wpt_haul"].ToString();
                                thisList.Add(item);
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

        private bool AddToMySQL(GearSoak item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@unload_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@time_set", MySqlDbType.DateTime).Value = item.TimeAtSet;
                    update.Parameters.Add("@time_haul", MySqlDbType.DateTime).Value = item.TimeAtHaul;
                    update.Parameters.Add("@wpt_set", MySqlDbType.VarChar).Value = item.WaypointAtSet;
                    update.Parameters.Add("@wpt_haul", MySqlDbType.VarChar).Value = item.WaypointAtHaul;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.CommandText = @"Insert into dbo_gear_soak(v_unload_id, time_set,time_hauled,wpt_set,wpt_haul,gear_soak_id) 
                                        Values (@unload_id,@time_set,@time_haul,@wpt_set,@wpt_haul,@id)";
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
        public bool Add(GearSoak item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into dbo_gear_soak(v_unload_id, time_set,time_hauled,wpt_set,wpt_haul,gear_soak_id) Values (@,@,@,@,@,@)";
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
            }
            return success;
        }
        private bool UpdateMySQL(GearSoak item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@unload_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@time_set", MySqlDbType.DateTime).Value = item.TimeAtSet;
                    update.Parameters.Add("@time_haul", MySqlDbType.DateTime).Value = item.TimeAtHaul;
                    update.Parameters.Add("@wpt_set", MySqlDbType.VarChar).Value = item.WaypointAtSet;
                    update.Parameters.Add("@wpt_haul", MySqlDbType.VarChar).Value = item.WaypointAtHaul;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_gear_soak set
                                            v_unload_id=@unload_id,
                                            time_set = @time_set,
                                            time_hauled = @time_haul,
                                            wpt_set = @wpt_set,
                                            wpt_haul=@wpt_haul
                                        WHERE gear_soak_id = @id";

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
        public bool Update(GearSoak item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(item);
            }
            else
            {
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
            }
            return success;
        }
        public static bool ClearTable(string otherConnectionString="")
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

        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from dbo_gear_soak where gear_soak_id=@id";
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
            }
            return success;
        }

    }
}
