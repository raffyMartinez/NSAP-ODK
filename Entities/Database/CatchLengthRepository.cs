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
    public class CatchLengthRepository
    {
        public List<CatchLength> CatchLengths { get; set; }

        public CatchLengthRepository(VesselCatch vc)
        {
            CatchLengths = getCatchLengths(vc);
        }
        public CatchLengthRepository()
        {
            CatchLengths = getCatchLengths();
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
                        cmd.CommandText = "SELECT Max(catch_len_id) AS max_id FROM dbo_catch_length";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_len_id) AS max_id FROM dbo_catch_len";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        private List<CatchLength> getFromMySQL(VesselCatch vc = null)
        {
            List<CatchLength> thisList = new List<CatchLength>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_catch_length";
                    if (vc != null)
                    {
                        cmd.Parameters.AddWithValue("parentID", vc.PK);
                        cmd.CommandText = "Select * from dbo_catch_length where catch_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CatchLength item = new CatchLength();
                        item.Parent = vc;
                        item.PK = (int)dr["catch_len_id"];
                        item.VesselCatchID = (int)dr["catch_id"];
                        item.Length = (double)dr["length"];
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<CatchLength> getCatchLengths(VesselCatch vc = null)
        {
            List<CatchLength> thisList = new List<CatchLength>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vc);
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
                            cmd.CommandText = "Select * from dbo_catch_len";

                            if (vc != null)
                            {
                                cmd.Parameters.AddWithValue("parentID", vc.PK);
                                cmd.CommandText = "Select * from dbo_catch_len where catch_id=@parentID";
                            }
                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchLength item = new CatchLength();
                                item.Parent = vc;
                                item.PK = (int)dr["catch_len_id"];
                                item.VesselCatchID = (int)dr["catch_id"];
                                item.Length = (double)dr["length"];
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
        private bool AddToMySQL(CatchLength item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@length", MySqlDbType.Double).Value = item.Length;
                    update.CommandText = @"Insert into dbo_catch_length(catch_len_id, catch_id, length) 
                                         Values (@id,@catch_id,@length)";
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
        public bool Add(CatchLength item)
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
                    var sql = "Insert into dbo_catch_len(catch_len_id, catch_id, length) Values (?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@length", OleDbType.Double).Value = item.Length;
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
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
            }
            return success;
        }
        private bool UpdateMySQL(CatchLength item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@length", MySqlDbType.Double).Value = item.Length;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_catch_length set
                                        catch_id=@catch_id,
                                        length = @length
                                        WHERE catch_len_id = @id"; ;

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
        public bool Update(CatchLength item)
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

                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@length", OleDbType.Double).Value = item.Length;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_catch_len set
                                        catch_id=@catch_id,
                                        length = @length
                                        WHERE catch_len_id = @id";

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
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
            }
            return success;
        }
        public bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_catch_len";
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
                    update.CommandText = "Delete * from dbo_catch_len where catch_len_id=@id";
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
                        update.CommandText = "Delete * from dbo_catch_len where catch_len_id=@id";
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
