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

                var sql = @"Insert into dbo_catch_maturity(catch_maturity_id, catch_id, length, weight, sex, maturity, gut_content_wt, gut_content_code, gonadWt)
                           Values (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                    update.Parameters.Add("@parent_id", OleDbType.Integer).Value = item.VesselCatchID;
                    if(item.Length==null)
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = item.Length;
                    }
                    if (item.Weight == null)
                    {
                        update.Parameters.Add("@wt", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt", OleDbType.Double).Value = item.Weight;
                    }
                    if (item.SexCode == null)
                    {
                        update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = item.SexCode.ToString();
                    }

                    if (item.MaturityCode == null)
                    {
                        update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = item.MaturityCode.ToString();
                    }
                    if (item.WeightGutContent == null)
                    {
                        update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = item.WeightGutContent;
                    }
                    if (item.GutContentCode == null)
                    {
                        update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = item.GutContentCode.ToString();
                    }
                    if (item.GonadWeight == null)
                    {
                        update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = item.GonadWeight;
                    }

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

        public bool Update(CatchMaturity item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                    update.Parameters.Add("@parent_id", OleDbType.Integer).Value = item.VesselCatchID;
                    if (item.Length == null)
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@len", OleDbType.Double).Value = item.Length;
                    }
                    if (item.Weight == null)
                    {
                        update.Parameters.Add("@wt", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt", OleDbType.Double).Value = item.Weight;
                    }
                    if (item.SexCode == null)
                    {
                        update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@sex_code", OleDbType.VarChar).Value = item.SexCode.ToString();
                    }

                    if (item.MaturityCode == null)
                    {
                        update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@maturity_code", OleDbType.VarChar).Value = item.MaturityCode.ToString();
                    }
                    if (item.WeightGutContent == null)
                    {
                        update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gut_content", OleDbType.Double).Value = item.WeightGutContent;
                    }
                    if (item.GutContentCode == null)
                    {
                        update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gut_content_code", OleDbType.VarChar).Value = item.GutContentCode.ToString();
                    }
                    if (item.GonadWeight == null)
                    {
                        update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@wt_gonad", OleDbType.Double).Value = item.GonadWeight;
                    }

                    update.CommandText = @"Update dbo_catch_maturity set
                                catch_id=@parent_id,
                                length = @len,
                                weight = @wt,
                                sex = @sex_code,
                                maturity = @maturity_code,
                                gut_content_wt =  @wt_gut_content,
                                gut_content_code = @gut_content_code,
                                gonadWt = @wt_gonad,    
                            WHERE catch_maturity_id = @pk";

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
                using (OleDbCommand update =conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText="Delete * from dbo_catch_maturity where catch_maturity_id=@id";
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
