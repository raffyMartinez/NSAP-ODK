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
    class FishingGroundGridRepository
    {
        public List<FishingGroundGrid> FishingGroundGrids { get; set; }

        public FishingGroundGridRepository(VesselUnload vu)
        {
            FishingGroundGrids = getFishingGroundGrids(vu);
        }
        public FishingGroundGridRepository(bool isNew=false)
        {
            if (!isNew)
            {
                FishingGroundGrids = getFishingGroundGrids();
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
                        cmd.CommandText = "SELECT Max(fg_grid_id) AS max_id FROM dbo_fg_grid";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(fg_grid_id) AS max_id FROM dbo_fg_grid";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        private List<FishingGroundGrid> getFromMySQL(VesselUnload vu = null)
        {
            List<FishingGroundGrid> thisList = new List<FishingGroundGrid>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"Select * from dbo_fg_grid";
                    if (vu != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vu.PK);
                        cmd.CommandText = "Select * from dbo_fg_grid where v_unload_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        FishingGroundGrid item = new FishingGroundGrid();
                        item.Parent = vu;
                        item.PK = (int)dr["fg_grid_id"];
                        item.VesselUnloadID = (int)dr["v_unload_id"];
                        item.UTMZoneText = dr["utm_zone"].ToString();
                        item.Grid = dr["grid25"].ToString();
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<FishingGroundGrid> getFishingGroundGrids(VesselUnload vu = null)
        {
            List<FishingGroundGrid> thisList = new List<FishingGroundGrid>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vu);
            }
            else
            {
                
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            cmd.CommandText = "Select * from dbo_fg_grid";
                            if (vu != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vu.PK);
                                cmd.CommandText = "Select * from dbo_fg_grid where v_unload_id=@parentID";
                            }


                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                FishingGroundGrid item = new FishingGroundGrid();
                                item.Parent = vu;
                                item.PK = (int)dr["fg_grid_id"];
                                item.VesselUnloadID = (int)dr["v_unload_id"];
                                item.UTMZoneText = dr["utm_zone"].ToString();
                                item.Grid = dr["grid25"].ToString();
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
        private bool AddToMySQL(FishingGroundGrid item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@unload_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@utm_zone", MySqlDbType.VarChar).Value = item.UTMZone.ToString();
                    update.Parameters.Add("@grid25", MySqlDbType.VarChar).Value = item.GridCell.ToString();
                    update.CommandText = @"Insert into dbo_fg_grid(fg_grid_id, v_unload_id, utm_zone,grid25) Values (@id,@unload_id,@utm_zone,@grid25)";
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
        public bool Add(FishingGroundGrid item)
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
            }
            return success;
        }
        private bool UpdateMySQL(FishingGroundGrid item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@unload_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@utm_zone", MySqlDbType.VarChar).Value = item.UTMZone.ToString();
                    update.Parameters.Add("@grid25", MySqlDbType.VarChar).Value = item.GridCell.ToString();
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_fg_grid set
                                        v_unload_id=@unload_id,
                                        utm_zone = @utm_zone,
                                        grid25 = @grid25
                                        WHERE fg_grid_id = @id";

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
        public bool Update(FishingGroundGrid item)
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
                        update.Parameters.Add("@utm_zone", OleDbType.VarChar).Value = item.UTMZone.ToString();
                        update.Parameters.Add("@grid25", OleDbType.VarChar).Value = item.GridCell.ToString();
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_fg_grid set
                                        v_unload_id=@unload_id,
                                        utm_zone = @utm_zone,
                                        grid25 = @grid25
                                        WHERE fg_grid_id = @id";

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
        public static bool ClearTable()
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete from dbo_fg_grid where fg_grid_id=@id";
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
                        update.CommandText = "Delete * from dbo_fg_grid where fg_grid_id=@id";
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
