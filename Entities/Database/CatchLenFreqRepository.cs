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
    public class CatchLenFreqRepository
    {
        public List<CatchLenFreq> CatchLenFreqs { get; set; }

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
                        cmd.CommandText = @"DELETE  dbo_catch_len_freq.*
                                                FROM ((((dbo_gear_unload INNER JOIN dbo_LC_FG_sample_day_1 ON dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN
                                                    dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vesselunload_fishinggear ON dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN 
                                                    dbo_vessel_catch ON dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) INNER JOIN 
                                                    dbo_catch_len_freq ON dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id
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
        public CatchLenFreqRepository(VesselCatch vc)
        {
            CatchLenFreqs = getCatchLenFreqs(vc);
        }

        public static bool AddFieldToTable(string fieldName)
        {
            bool success = false;
            string sql = "";
            switch (fieldName)
            {
                case "sex":
                    sql = "ALTER TABLE dbo_catch_len_freq ADD COLUMN sex varchar(2)";
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
        public CatchLenFreqRepository(bool isNew=false)
        {
            if (!isNew)
            {
                CatchLenFreqs = getCatchLenFreqs();
            }
        }
        private List<CatchLenFreq> getFromMySQL(VesselCatch vc = null)
        {
            List<CatchLenFreq> thisList = new List<CatchLenFreq>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_catch_len_freq";
                    if (vc != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vc.PK);
                        cmd.CommandText = $"Select * from dbo_catch_len_freq where catch_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CatchLenFreq item = new CatchLenFreq();
                        item.Parent = vc;
                        item.PK = (int)dr["catch_lf_id"];
                        item.VesselCatchID = (int)dr["catch_id"];
                        item.LengthClass = (double)dr["length"];
                        item.Frequency = (int)dr["freq"];
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }
        private List<CatchLenFreq> getCatchLenFreqs(VesselCatch vc = null)
        {
            List<CatchLenFreq> thisList = new List<CatchLenFreq>();
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
                            cmd.CommandText = $"Select * from dbo_catch_len_freq";

                            if (vc != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vc.PK);
                                cmd.CommandText = $"Select * from dbo_catch_len_freq where catch_id=@parentID";
                            }
                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                CatchLenFreq item = new CatchLenFreq();
                                item.Parent = vc;
                                item.PK = (int)dr["catch_len_freq_id"];
                                item.VesselCatchID = (int)dr["catch_id"];
                                item.LengthClass = (double)dr["len_class"];
                                item.Frequency = (int)dr["freq"];
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

        private bool AddToMySQL(CatchLenFreq item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@len_class", MySqlDbType.Double).Value = item.LengthClass;
                    update.Parameters.Add("@freq", MySqlDbType.Int32).Value = item.Frequency;
                    update.Parameters.Add("@sex", MySqlDbType.VarChar).Value = item.Sex;
                    update.CommandText = @"Insert into dbo_catch_len_freq(catch_lf_id, catch_id, length,freq) 
                                        Values (@id,@catch_id,@len_class,@freq,@sex)";
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
        public bool Add(CatchLenFreq item)
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
                    var sql = "Insert into dbo_catch_len_freq(catch_len_freq_id, catch_id, len_class,freq,sex) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@len_class", OleDbType.Double).Value = item.LengthClass;
                        update.Parameters.Add("@freq", OleDbType.Integer).Value = item.Frequency;
                        update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
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
                        cmd.CommandText = "SELECT Max(catch_lf_id) AS max_id FROM dbo_catch_len_freq";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(catch_len_freq_id) AS max_id FROM dbo_catch_len_freq";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
        private bool UpdateMySQL(CatchLenFreq item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@catch_id", MySqlDbType.Int32).Value = item.Parent.PK;
                    update.Parameters.Add("@len_class", MySqlDbType.Double).Value = item.LengthClass;
                    update.Parameters.Add("@freq", MySqlDbType.Int32).Value = item.Frequency;
                    update.Parameters.Add("@sex", MySqlDbType.VarChar).Value = item.Sex;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_catch_len_freq set
                                        catch_id=@catch_id,
                                        length = @len_class,
                                        freq = @freq,
                                        sex=@sex
                                        WHERE catch_lf_id = @id";

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
        public bool Update(CatchLenFreq item)
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
                        update.Parameters.Add("@len_class", OleDbType.Double).Value = item.LengthClass;
                        update.Parameters.Add("@freq", OleDbType.Integer).Value = item.Frequency;
                        update.Parameters.Add("@sex", OleDbType.VarChar).Value = item.Sex;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_catch_len_freq set
                                        catch_id=@catch_id,
                                        len_class = @len_class,
                                        freq = @freq,
                                        sex = @sex
                                        WHERE catch_len_freq_id = @id";

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
                var sql = $"Delete * from dbo_catch_len_freq";
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
                    update.CommandText = "Delete  from dbo_catch_len_freq where catch_lf_id=@id";
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
                        update.CommandText = "Delete * from dbo_catch_len_freq where catch_len_freq_id=@id";
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
