using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.NSAPMysql;
using System.Data;
using MySql.Data.MySqlClient;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities.Database
{
    public class TotalWtSpRepository
    {

        public static bool CheckForTWSPTable()
        {
            bool tableExists = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (schema.Rows
                    .OfType<DataRow>()
                    .Any(r => r.ItemArray[2].ToString().ToLower() == "dbo_total_wt_sp"))
                {
                    tableExists = true;
                }

                if (!tableExists)
                {
                    return CreateTWSPTable();
                }
                else
                {
                    return true;
                }
            }
        }

        public List<TotalWtSp> TotalWtSps { get; set; }
        public static bool ClearTable(string otherConnectionString = "")
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
                var sql = $"Delete * from dbo_total_wt_sp";
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
        public static bool CreateTWSPTable()
        {
            bool success = false;
            using (var conn = new OleDbConnection(Utilities.Global.ConnectionString))
            {
                using (var cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"CREATE TABLE dbo_total_wt_sp (
                                                row_id Int NOT NULL PRIMARY KEY,
                                                sp_id Int,
                                                sp_text VarChar,
                                                total_wt_sp Double,
                                                gear_unload Int,
                                                taxa VarChar,
                                                CONSTRAINT FK_twsp
                                                    FOREIGN KEY(gear_unload) REFERENCES dbo_gear_unload(unload_gr_id)
                                                )";
                    //@"CREATE TABLE dbo_vessel_unload_stats (
                    //                            v_unload_id INTEGER,
                    //                            count_effort INTEGER,
                    //                            count_grid INTEGER,
                    //                            count_soak INTEGER,
                    //                            count_catch_composition INTEGER,
                    //                            count_lengths INTEGER,
                    //                            count_lenfreq INTEGER,
                    //                            count_lenwt INTEGER,
                    //                            count_maturity INTEGER,
                    //                            CONSTRAINT PrimaryKey PRIMARY KEY (v_unload_id),
                    //                            CONSTRAINT FK_unload_stats
                    //                                FOREIGN KEY (v_unload_id) REFERENCES
                    //                                dbo_vessel_unload (v_unload_id)
                    //                            )";



                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        success = true;

                    }
                    catch (OleDbException odx)
                    {
                        Utilities.Logger.Log(odx);
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Log(ex);
                    }
                }
            }

            return success;
        }

        public static int MaxRecordNumberFromDB()
        {
            int maxRecNo = 0;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "SELECT Max(row_id) AS max_id FROM dbo_total_wt_sp";
                        maxRecNo = (int)cmd.ExecuteScalar();
                    }
                }
            }
            return maxRecNo;
        }

        public static int TWSpCount()
        {
            int count = 0;
            if (Utilities.Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        //cmd.CommandText = "Select count(*) from dbo_gear_unload";
                        try
                        {
                            conn.Open();
                            count = (int)(long)cmd.ExecuteScalar();
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
                using (var conn = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select count(*) from dbo_total_wt_sp";
                        try
                        {
                            conn.Open();
                            count = (int)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }

            }
            return count;
        }
        public TotalWtSpRepository() { }
        public TotalWtSpRepository(GearUnload parent)
        {
            TotalWtSps = getTotalWtSps(parent);
        }

        public bool Delete(int rowID)
        {
            bool success = false;
            return success;
        }
        public bool Update(TotalWtSp twsp)
        {
            bool success = false;
            return success;
        }

        public bool Add(TotalWtSp twsp)
        {
            bool success = false;
            if (Utilities.Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Utilities.Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@gear_unload_id", twsp.Parent.PK);
                        if (twsp.SpeciesID == null)
                        {
                            cmd.Parameters.AddWithValue("@sp_id", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@sp_id", (int)twsp.SpeciesID);
                        }
                        if (string.IsNullOrEmpty(twsp.SpeciesText))
                        {
                            cmd.Parameters.AddWithValue("@sp_text", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@sp_text", twsp.SpeciesText);
                        }
                        cmd.Parameters.AddWithValue("@taxa", twsp.Taxa.Code);
                        cmd.Parameters.AddWithValue("@twsp", twsp.TWSP);
                        cmd.Parameters.AddWithValue("@id", twsp.RowID);

                        cmd.CommandText = @"INSERT INTO dbo_total_wt_sp (gear_unload, sp_id, sp_text,taxa,total_wt_sp,row_id)
                                            values (@gear_unload_id,@sp_id,@sp_text,@taxa,@twsp,@id)";
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() > 0;
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return success;
        }
        private List<TotalWtSp> getTotalWtSps(GearUnload parent = null)
        {
            List<TotalWtSp> totalWtSps = new List<TotalWtSp>();
            if (Utilities.Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Utilities.Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from dbo_total_wt_sp";
                        if (parent != null)
                        {
                            cmd.Parameters.AddWithValue("@parent_id", parent.PK);
                            cmd.CommandText = "Select * from dbo_total_wt_sp WHERE gear_unload=@parent_id";
                        }
                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                int? sp_id = null;
                                if (dr["sp_id"] != DBNull.Value)
                                {
                                    sp_id = (int)dr["sp_id"];
                                }

                                TotalWtSp twsp = new TotalWtSp
                                {
                                    RowID = (int)dr["row_id"],
                                    Parent = parent,
                                    SpeciesID = sp_id,
                                    SpeciesText = dr["sp_text"].ToString(),
                                    Taxa = NSAPEntities.TaxaViewModel.GetTaxa(dr["taxa"].ToString()),
                                    TWSP = (double)dr["total_wt_sp"]
                                };
                                totalWtSps.Add(twsp);
                            }

                        }
                        catch (Exception ex)
                        {
                            Utilities.Logger.Log(ex);
                        }
                    }
                }
            }
            return totalWtSps;
        }
    }
}
