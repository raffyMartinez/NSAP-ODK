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
    class GearUnloadRepository
    {
        public List<GearUnload> GearUnloads { get; set; }

        public GearUnloadRepository()
        {
            GearUnloads = getGearUnloads();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(unload_gr_id) AS max_id FROM dbo_gear_unload";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<GearUnload> getGearUnloads()
        {
            List<GearUnload> thisList = new List<GearUnload>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_gear_unload";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            GearUnload item = new GearUnload();
                            item.PK = (int)dr["unload_gr_id"];
                            item.LandingSiteSamplingID = (int)dr["unload_day_id"];
                            item.GearID = dr["gr_id"].ToString();
                            item.Boats = string.IsNullOrEmpty(dr["boats"].ToString()) ? null: (int?)dr["boats"];
                            item.Catch = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"];
                            item.GearUsedText = dr["gr_text"].ToString();
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

        public bool Add(GearUnload item)
        {
            string gearID = string.IsNullOrEmpty(item.GearID) ? "null" : $"'{item.GearID}'";
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into dbo_gear_unload(unload_gr_id, unload_day_id, gr_id,boats,catch,gr_text,remarks)
                           Values (
                                {item.PK}, 
                                {item.LandingSiteSamplingID},
                                {gearID},
                                {(item.Boats == null ? "null" : item.Boats.ToString())},
                                {(item.Catch == null ? "null" : item.Catch.ToString())},
                                '{item.GearUsedText}',
                                '{item.Remarks}'
                                )";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch(OleDbException)
                    {
                        success = false;
                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }

        public bool Update(GearUnload item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                string gearID = item.GearID==null || item.GearID.Length == 0 ? "null" :$"'{item.GearID}'";
                var sql = $@"Update dbo_gear_unload set
                            unload_day_id={item.LandingSiteSamplingID},
                            gr_id = {gearID},
                            boats = {(item.Boats == null ? "null" : item.Boats.ToString())},
                            catch = {(item.Catch == null ? "null" : item.Catch.ToString())},
                            gr_text = '{item.GearUsedText}'
                        WHERE unload_gr_id = {item.PK}";

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
                var sql = $"Delete * from dbo_gear_unload";
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
                var sql = $"Delete * from dbo_gear_unload where unload_gr_id={id}";
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
