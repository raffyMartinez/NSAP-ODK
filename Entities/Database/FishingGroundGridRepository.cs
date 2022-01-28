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
    class FishingGroundGridRepository
    {
        public List<FishingGroundGrid> FishingGroundGrids { get; set; }

        public FishingGroundGridRepository()
        {
            FishingGroundGrids = getFishingGroundGrids();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(fg_grid_id) AS max_id FROM dbo_fg_grid";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<FishingGroundGrid> getFishingGroundGrids()
        {
            List<FishingGroundGrid> thisList = new List<FishingGroundGrid>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_fg_grid";
                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            FishingGroundGrid item = new FishingGroundGrid();
                            item.PK = (int)dr["fg_grid_id"];
                            item.VesselUnloadID = (int)dr["v_unload_id"];
                            item.UTMZoneText = dr["utm_zone"].ToString();
                            item.Grid = dr["grid25"].ToString();
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

        public bool Add(FishingGroundGrid item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = @"Insert into dbo_fg_grid(fg_grid_id, v_unload_id, utm_zone,grid25) Values (?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                    update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                    update.Parameters.Add("@utm_zone", OleDbType.VarChar).Value = item.UTMZone.ToString();
                    update.Parameters.Add("@grid25", OleDbType.VarChar).Value = item.GridCell.ToString();
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

        public bool Update(FishingGroundGrid item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                    update.Parameters.Add("@utm_zone", OleDbType.VarChar).Value = item.UTMZone.ToString();
                    update.Parameters.Add("@grid25", OleDbType.VarChar).Value = item.GridCell.ToString();
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                    update.CommandText = @"Update dbo_fg_grid set
                                        v_unload_id=@unload_id,
                                        utm_zone = @utm_zone,
                                        grid25 = @grid25
                                        WHERE gear_soak_id = @id";

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
                var sql = $"Delete * from dbo_fg_grid";
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
                    update.CommandText="Delete * from dbo_fg_grid where gear_soak_id=@id";
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
