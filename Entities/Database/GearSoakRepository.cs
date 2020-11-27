using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
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
        private List<GearSoak> getGearSoaks()
        {
            List<GearSoak> thisList = new List<GearSoak>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_gear_soak";
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
                return thisList;
            }
        }

        public bool Add(GearSoak item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into dbo_gear_soak(gear_soak_id, v_unload_id, time_set,time_hauled,wpt_set,wpt_haul)
                           Values (
                                {item.PK}, 
                                {item.VesselUnloadID},
                                '{item.TimeAtSet}',
                                '{item.TimeAtHaul}',
                                '{item.WaypointAtSet}',
                                '{item.WaypointAtHaul}'
                                )";
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
                var sql = $@"Update dbo_gear_soak set
                                v_unload_id={item.VesselUnloadID},
                                time_set = '{item.TimeAtSet}',
                                time_hauled = '{item.TimeAtHaul}',
                                wpt_set = '{item.WaypointAtSet}',
                                wpt_haul='{item.WaypointAtHaul}'
                            WHERE gear_soak_id = {item.PK}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
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
                var sql = $"Delete * from dbo_gear_soak where gear_soak_id={id}";
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
