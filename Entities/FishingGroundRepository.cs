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

namespace NSAP_ODK.Entities
{
    public class FishingGroundRepository
    {
        public List<FishingGround> FishingGrounds { get; set; }

        public FishingGroundRepository()
        {
            FishingGrounds = getFishingGrounds();
        }

        private List<FishingGround>getFishingGroundsMySQL()
        {
            List<FishingGround> thisList = new List<FishingGround>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from fishing_grounds";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        FishingGround fg = new FishingGround();
                        fg.Code = dr["fishing_ground_code"].ToString();
                        fg.Name = dr["fishing_ground_name"].ToString();
                        thisList.Add(fg);
                    }
                }
            }
            return thisList;
        }
        private List<FishingGround> getFishingGrounds()
        {
            List<FishingGround> listFishingGrounds = new List<FishingGround>();
            if (Global.Settings.UsemySQL)
            {
                listFishingGrounds = getFishingGroundsMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from fishingGround";


                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listFishingGrounds.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                FishingGround fg = new FishingGround();
                                fg.Code = dr["FishingGroundCode"].ToString();
                                fg.Name = dr["FishingGroundName"].ToString();
                                listFishingGrounds.Add(fg);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);

                    }
                }

            }
            return listFishingGrounds;
        }

        private bool AddToMySQL(FishingGround fg)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.Parameters.Add("@fg_name", MySqlDbType.VarChar).Value = fg.Name;
                    cmd.Parameters.Add("@fg_code", MySqlDbType.VarChar).Value = fg.Code.ToUpper();
                    cmd.CommandText= "Insert into fishing_grounds(fishing_ground_name,fishing_ground_code) Values (@fg_name, @fg_code)";
                    try
                    {
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
                                break;
                            default:
                                Logger.Log(msex);
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
                return success;
        }
        public bool Add(FishingGround fg)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(fg);
            }
            else

            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into fishingGround(FishingGroundName,FishingGroundCode) Values (?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@fg_name", OleDbType.VarChar).Value = fg.Name;
                        update.Parameters.Add("@fg_code", OleDbType.VarChar).Value = fg.Code.ToUpper();
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            switch (dbex.ErrorCode)
                            {
                                case -2147467259:
                                    //database is corrupt
                                    CorruptedDatabase = true;
                                    break;
                            }
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

        public bool CorruptedDatabase { get; private set; }

        private bool UpdateMySQL(FishingGround fg)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@fgname", MySqlDbType.VarChar).Value = fg.Name;
                    update.Parameters.Add("@fgcode", MySqlDbType.VarChar).Value = fg.Code.ToUpper();
                    update.CommandText = @"UPDATE fishing_grounds SET
                                           fishing_ground_name=@fgname
                                           WHERE fishing_ground_code=@fgcode";

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
        public bool Update(FishingGround fg)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(fg);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@fgname", OleDbType.VarChar).Value = fg.Name;
                        update.Parameters.Add("@fgcode", OleDbType.VarChar).Value = fg.Code.ToUpper();
                        update.CommandText = @"UPDATE fishingground SET
                                           FishingGroundName=@fgname
                                           WHERE FishingGroundCode=@fgcode";
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
        private bool DeleteMySQL(string code)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    update.CommandText = "Delete from fishing_grounds where fishing_ground_code=@code";
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
        public bool Delete(string code)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteMySQL(code);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = code;
                        update.CommandText = "Delete * from fishingGround where FishingGroundCode=@code";
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
