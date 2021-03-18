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
   public class VesselEffortRepository
    {
        public List<VesselEffort> VesselEfforts { get; set; }

        public VesselEffortRepository()
        {
            VesselEfforts = getVesselEfforts();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(effort_row_id) AS max_id FROM dbo_vessel_effort";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<VesselEffort> getVesselEfforts()
        {
            List<VesselEffort> thisList = new List<VesselEffort>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_vessel_effort";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            VesselEffort item = new VesselEffort();
                            item.PK = (int)dr["effort_row_id"];
                            item.VesselUnloadID = (int)dr["v_unload_id"];
                            item.EffortSpecID = (int)dr["effort_spec_id"];
                            item.EffortValueNumeric = string.IsNullOrEmpty(dr["effort_value_numeric"].ToString()) ? null : (double?)dr["effort_value_numeric"];
                            item.EffortValueText = dr["effort_value_text"].ToString();
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

        public bool Add(VesselEffort item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into dbo_vessel_effort(effort_row_id, v_unload_id,effort_spec_id, effort_value_numeric,effort_value_text)
                           Values ({item.PK},{item.VesselUnloadID},{item.EffortSpecID},
                                    {(item.EffortValueNumeric==null?"null":item.EffortValueNumeric.ToString())},'{item.EffortValueText}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                        if(success)
                        {

                        }
                    }
                    catch (OleDbException odbex)
                    {
                        Logger.Log(odbex);
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

        public bool Update(VesselEffort item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update dbo_vessel_effort set
                                v_unload_id='{item.VesselUnloadID}',
                                effort_spec_id = {item.EffortSpecID},
                                effort_value_numeric = {(item.EffortValueNumeric == null ? "null" : item.EffortValueNumeric.ToString())},
                                effort_value_text = {item.EffortValueText}
                            WHERE effort_row_id = {item.PK}";
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
                var sql = $"Delete * from dbo_vessel_effort";
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
                var sql = $"Delete * from dbo_vessel_effort where effort_row_id={id}";
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
