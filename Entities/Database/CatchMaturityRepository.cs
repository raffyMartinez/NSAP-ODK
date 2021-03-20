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
    public class CatchMaturityRepository
    {
        public List<CatchMaturity> CatchMaturities { get; set; }

        public CatchMaturityRepository()
        {
            CatchMaturities = getCatchMaturites();
        }
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(catch_maturity_id) AS max_id FROM dbo_catch_maturity";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        private List<CatchMaturity> getCatchMaturites()
        {
            List<CatchMaturity> thisList = new List<CatchMaturity>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_catch_maturity";
                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            CatchMaturity item = new CatchMaturity();
                            item.PK = (int)dr["catch_maturity_id"];
                            item.VesselCatchID = (int)dr["catch_id"];
                            item.GonadWeight = dr["gonadWt"]==DBNull.Value?null:(double?)dr["gonadWt"];
                            item.Length = dr["length"]==DBNull.Value?null:(double?)dr["length"];
                            item.Weight = dr["weight"] == DBNull.Value ? null : (double?)dr["weight"];
                            item.SexCode = dr["sex"].ToString();
                            item.MaturityCode = dr["maturity"].ToString();
                            item.WeightGutContent = dr["gut_content_wt"] == DBNull.Value ? null : (double?)dr["gut_content_wt"];
                            item.GutContentCode = dr["gut_content_code"].ToString();
                            thisList.Add(item);
                        }
                    }
                }
                catch(OleDbException dbex)
                {
                    Logger.Log(dbex);
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2147024809)
                    {
                        conection.Close();
                        UpdateTable();
                    }
                    else
                    {
                        Logger.Log(ex);
                    }

                }
                return thisList;
            }
        }

        private void UpdateTable()
        {
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = @"ALTER TABLE dbo_catch_maturity ADD COLUMN gonadWt FLOAT";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                try
                {
                    cmd.ExecuteNonQuery();

                }
                catch (OleDbException dbex)
                {
                    Logger.Log(dbex);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                cmd.Connection.Close();
                conn.Close();
            }
            getCatchMaturites();
        }
        public bool Add(CatchMaturity item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into dbo_catch_maturity(catch_maturity_id, catch_id, length,weight,sex,maturity,gut_content_wt,gut_content_code,gonadWt)
                           Values (
                                {item.PK}, 
                                {item.VesselCatchID},
                                {(item.Length==null?"null":item.Length.ToString())},
                                {(item.Weight == null ? "null" : item.Weight.ToString())},
                                '{item.SexCode}',
                                '{item.MaturityCode}',
                                {(item.WeightGutContent == null ? "null" : item.WeightGutContent.ToString())},
                                '{item.GutContentCode}',
                                {(item.GonadWeight== null ? "null" : item.GonadWeight.ToString())},
                                )";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
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
            return success;
        }

        public bool Update(CatchMaturity item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update dbo_catch_maturity set
                                catch_id={item.VesselCatchID},
                                length = {(item.Length == null ? "null" : item.Length.ToString())},
                                weight = {(item.Weight == null ? "null" : item.Weight.ToString())},
                                sex = '{item.SexCode}',
                                maturity='{item.MaturityCode}',
                                gut_content_wt =  {(item.WeightGutContent == null ? "null" : item.WeightGutContent.ToString())},
                                gut_content_code = '{item.GutContentCode}',
                                gonadWt = {(item.GonadWeight == null ? "null" : item.GonadWeight.ToString())},    
                            WHERE catch_maturity_id = {item.PK}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
                var sql = $"Delete * from dbo_catch_maturity";
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
                var sql = $"Delete * from dbo_catch_maturity where catch_maturity_id={id}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
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
