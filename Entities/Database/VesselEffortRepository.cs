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

                var sql = "Insert into dbo_vessel_effort(effort_row_id, v_unload_id,effort_spec_id, effort_value_numeric,effort_value_text) Values (?,?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.VesselUnloadID;
                        update.Parameters.Add("@effort_id", OleDbType.Integer).Value = item.EffortSpecID;
                        if (item.EffortValueNumeric == null)
                        {
                            update.Parameters.Add("@numeric_value", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@numeric_value", OleDbType.Double).Value = item.EffortValueNumeric;
                        }
                        if (item.EffortValueText == null)
                        {
                            update.Parameters.Add("@text_value", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@text_value", OleDbType.VarChar).Value = item.EffortValueText;
                        }


                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            switch(dbex.ErrorCode)
                            {
                                case -2147467259:
                                    //error due to duplicated key or index
                                    break;
                                default:
                                    Logger.Log(dbex);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
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

                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.VesselUnloadID;
                    update.Parameters.Add("@effort_id", OleDbType.Integer).Value = item.EffortSpecID;
                    if (item.EffortValueNumeric == null)
                    {
                        update.Parameters.Add("@numeric_value", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@numeric_value", OleDbType.Double).Value = item.EffortValueNumeric;
                    }
                    update.Parameters.Add("@text_value", OleDbType.VarChar).Value = item.EffortValueText;
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                    update.CommandText = @"Update dbo_vessel_effort set
                                            v_unload_id=@unload_id,
                                            effort_spec_id = @effort_id,
                                            effort_value_numeric = @numeric_value,
                                            effort_value_text = @text_value
                                        WHERE effort_row_id = @id";

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
                
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText="Delete * from dbo_vessel_effort where effort_row_id=@id";
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
