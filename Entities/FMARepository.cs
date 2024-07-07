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
    public class FMARepository
    {
        public List<FMA> FMAs { get; set; }

        public FMARepository()
        {
            FMAs = getFMAs();
        }

        public static  List<NSAPRegionFMA> GetNSAPRegionFMAs()
        {
            List<NSAPRegionFMA> nsapRegionFMAs = new List<NSAPRegionFMA>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from NSAPRegionFMA";
                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                NSAPRegionFMA nrfma = new NSAPRegionFMA
                                {
                                    RowID = (int)dr["RowID"],
                                    NSAPRegionCode = dr["NSAPRegion"].ToString(),
                                    FMAID = (int)dr["FMA"]
                                };
                                nsapRegionFMAs.Add(nrfma);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            return nsapRegionFMAs;
        }
        private List<FMA> getFromMySQL()
        {
            List<FMA> thisList = new List<FMA>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from fma";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        FMA fma = new FMA();
                        fma.FMAID = Convert.ToInt32(dr["fma_id"]);
                        fma.Name = dr["fma_name"].ToString();
                        thisList.Add(fma);
                    }
                }
            }
            return thisList;
        }

        public int RecordCountMDB()
        {
             int n = 0;
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = conection.CreateCommand())
                {
                    cmd.CommandText = "Select count(*) from fma";
                    try
                    {
                        conection.Open();
                        n = (int)cmd.ExecuteScalar();
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
            return n;
        }
        public int RecordCountMySQL()
        {
            int n = 0;

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select count(*) from fma";
                    try
                    {
                        conn.Open();
                        n = (int)cmd.ExecuteScalar();
                    }
                    catch (MySqlException mex)
                    {
                        Logger.Log(mex);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                }
            }

            return n;
        }
        public bool Add(FMA fma)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = fma.FMAID;
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = fma.Name;
                    cmd.CommandText = "Insert into fma (fma_id, fma_name) Values (@id, @name)";
                    try
                    {
                        conn.Open();
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
        private List<FMA> getFMAs()
        {
            List<FMA> thisList = new List<FMA>();
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
                        string query = $"Select * from fma";


                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            thisList.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                FMA fma = new FMA();
                                fma.FMAID = Convert.ToInt32(dr["FMAID"]);
                                fma.Name = dr["FMAName"].ToString();
                                thisList.Add(fma);
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
    }
}
