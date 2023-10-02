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
    class CatchLenWeightRepository
    {
        public List<CatchLengthWeight> CatchLengthWeights { get; set; }

        public static Task<bool> DeleteMultivesselDataAsync(bool isMultivessel)
        {
            return Task.Run(() => DeleteMultivesselData(isMultivessel));
        }
        public static bool DeleteMultivesselData(bool isMultivessel)
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
                        if (isMultivessel)
                        {
                            cmd.CommandText = @"DELETE dbo_catch_len_wt.* 
                                                FROM dbo_vessel_catch 
                                                INNER JOIN dbo_catch_len_wt ON dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id
                                                WHERE dbo_vessel_catch.v_unload_id Is Null";
                        }
                        else
                        {
                            cmd.CommandText = @"DELETE dbo_catch_len_wt.* 
                                                FROM dbo_vessel_catch 
                                                INNER JOIN dbo_catch_len_wt ON dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id
                                                WHERE dbo_vessel_catch.v_unload_id Is Not Null";
                        }
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
        public CatchLenWeightRepository(VesselCatch vc)
        {
            CatchLengthWeights = getCatchLengthWeights(vc);
        }

        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "sex":
                    sql = "ALTER TABLE dbo_catch_len_wt ADD COLUMN sex varchar(2)";
                    break;
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public CatchLenWeightRepository(bool isNew=false)
        {
            if (!isNew)
            {
                CatchLengthWeights = getCatchLengthWeights();
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
                        cmd.CommandText = "SELECT Max(catch_lw_id) AS max_id FROM dbo_catch_len_wt";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_len_wt_id) AS max_id FROM dbo_catch_len_wt";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }

        private List<CatchLengthWeight> getFromMySQL(VesselCatch vc = null)
        {
            List<CatchLengthWeight> thisList = new List<CatchLengthWeight>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_catch_len_wt";
                    if (vc != null)
                    {
                        cmd.Parameters.AddWithValue("@parent_id", vc.PK);
                        cmd.CommandText = "Select * from dbo_catch_len_wt where catch_id=@parent_id";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CatchLengthWeight item = new CatchLengthWeight();
                        item.Parent = vc;
                        item.PK = (int)dr["catch_lw_id"];
                        item.VesselCatchID = (int)dr["catch_id"];
                        item.Length = (double)dr["length"];
                        item.Weight = (double)dr["weight"];
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<CatchLengthWeight> getCatchLengthWeights(VesselCatch vc = null)
        {
            List<CatchLengthWeight> thisList = new List<CatchLengthWeight>();
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
                            cmd.CommandText = "Select * from dbo_catch_len_wt";

                            if (vc != null)
                            {
                                cmd.Parameters.AddWithValue("@parent_id", vc.PK);
                                cmd.CommandText = "Select * from dbo_catch_len_wt where catch_id=@parent_id";
                            }

                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchLengthWeight item = new CatchLengthWeight();
                                item.Parent = vc;
                                item.PK = (int)dr["catch_len_wt_id"];
                                item.VesselCatchID = (int)dr["catch_id"];
                                item.Length = (double)dr["length"];
                                item.Weight = (double)dr["weight"];
                                item.Sex = dr["sex"].ToString();
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

        private bool AddToMySQL(CatchLengthWeight item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@length", MySqlDbType.Double).Value = item.Length;
                    update.Parameters.Add("@weight", MySqlDbType.Double).Value = item.Weight;
                    update.CommandText = @"Insert into dbo_catch_len_wt(catch_lw_id, catch_id, length,weight) 
                                          Values (@id,@catch_id,@length,@weight)";
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
        public bool Add(CatchLengthWeight item)
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
                    var sql = "Insert into dbo_catch_len_wt(catch_len_wt_id, catch_id, length,weight,sex) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@length", OleDbType.Double).Value = item.Length;
                        update.Parameters.Add("@weight", OleDbType.Double).Value = item.Weight;
                        update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            Logger.Log(dbex);
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
        private bool UpdateMySQL(CatchLengthWeight item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@length", MySqlDbType.Double).Value = item.Length;
                    update.Parameters.Add("@weight", MySqlDbType.Int32).Value = item.Weight;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_catch_len_wt set
                                        catch_id=@catch_id,
                                        length = @length,
                                        weight = @weight
                                        WHERE catch_lw_id = @id";

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
        public bool Update(CatchLengthWeight item)
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
                        update.Parameters.Add("@weight", OleDbType.Double).Value = item.Weight;
                        update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_catch_len_wt set
                                        catch_id=@catch_id,
                                        length = @length,
                                        weight = @weight,
                                        sex=@sex
                                        WHERE catch_len_wt_id = @id";

                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            Logger.Log(dbex);
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
                var sql = $"Delete * from dbo_catch_len_wt";
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
                    update.CommandText = "Delete  from dbo_catch_len_wt where catch_lw_id=@id";
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
                        update.CommandText = "Delete * from dbo_catch_len_wt where catch_len_wt_id=@id";
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
