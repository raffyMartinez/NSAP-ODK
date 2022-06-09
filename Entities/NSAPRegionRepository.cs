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
    public class NSAPRegionRepository
    {
        public List<NSAPRegion> NSAPRegions { get; set; }

        public NSAPRegionRepository()
        {
            NSAPRegions = getNSAPRegions();
        }
        private List<NSAPRegion> getFromMySQL()
        {
            List<NSAPRegion> thisList = new List<NSAPRegion>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from nsap_region";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        NSAPRegion nsr = new NSAPRegion();
                        nsr.Code = dr["code"].ToString();
                        nsr.Name = dr["region_name"].ToString();
                        nsr.ShortName = dr["short_name"].ToString();
                        nsr.Sequence = Convert.ToInt32(dr["sequence"]);
                        thisList.Add(nsr);
                    }
                }
            }
            return thisList;
        }
        private List<NSAPRegion> getNSAPRegions()
        {
            List<NSAPRegion> thisList = new List<NSAPRegion>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $@"SELECT * from nsapRegion order by Sequence";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            thisList.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                NSAPRegion nsr = new NSAPRegion();
                                nsr.Code = dr["Code"].ToString();
                                nsr.Name = dr["RegionName"].ToString();
                                nsr.ShortName = dr["ShortName"].ToString();
                                nsr.Sequence = Convert.ToInt32(dr["Sequence"]);
                                thisList.Add(nsr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);

                    }
                }
            }
            return thisList;
        }

        private bool AddToMySQL(NSAPRegion nsr)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    conn.Open();
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = nsr.Name;
                    update.Parameters.Add("@short_name", MySqlDbType.VarChar).Value = nsr.ShortName;
                    update.Parameters.Add("@seq", MySqlDbType.VarChar).Value = nsr.Sequence;
                    update.Parameters.Add("@code", MySqlDbType.VarChar).Value = nsr.Code;
                    update.CommandText= @"Insert into nsap_region (region_name, short_name,sequence,code) Values (@name,@short_name,@seq,@code)";
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
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
        
        public bool Add(NSAPRegion nsr)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(nsr);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = @"Insert into nsapRegion (RegionName, ShortName,Sequence,Code) Values (?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = nsr.Name;
                        update.Parameters.Add("@short_name", OleDbType.VarChar).Value = nsr.ShortName;
                        update.Parameters.Add("@seq", OleDbType.VarChar).Value = nsr.Sequence;
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = nsr.Code;
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

        public bool Update(NSAPRegion nsr)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@name", MySqlDbType.VarChar).Value = nsr.Name;
                        update.Parameters.Add("@short_name", MySqlDbType.VarChar).Value = nsr.ShortName;
                        update.Parameters.Add("@seq", MySqlDbType.VarChar).Value = nsr.Sequence;
                        update.Parameters.Add("@code", MySqlDbType.VarChar).Value = nsr.Code;

                        update.CommandText = @"Update nsap_region set 
                                              region_name = @name, 
                                              short_name=@short_name, 
                                              sequence=@seq
                                            WHERE code=@code";

                        try
                        {
                            conn.Open();
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException msx)
                        {
                            Logger.Log(msx);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = nsr.Name;
                        update.Parameters.Add("@short_name", OleDbType.VarChar).Value = nsr.ShortName;
                        update.Parameters.Add("@seq", OleDbType.VarChar).Value = nsr.Sequence;
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = nsr.Code;


                        update.CommandText = @"Update nsapRegion set 
                                              RegionName = @name, 
                                              ShortName=@short_name, 
                                              Sequence=@seq
                                            WHERE Code=@code";

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

        public bool Delete(string code)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                        update.CommandText = "Delete  from nsap_region where code=@code";

                        try
                        {
                            conn.Open();
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (MySqlException msx)
                        {
                            Logger.Log(msx);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }

                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = code;
                        update.CommandText = "Delete * from nsapRegion where Code=@code";
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException oex)
                        {
                            Logger.Log(oex);
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
    }
}
