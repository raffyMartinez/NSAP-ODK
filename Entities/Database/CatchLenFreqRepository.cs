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
    public class CatchLenFreqRepository
    {
        public List<CatchLenFreq> CatchLenFreqs { get; set; }

        public CatchLenFreqRepository()
        {
            CatchLenFreqs = getCatchLenFreqs();
        }

        private List<CatchLenFreq> getCatchLenFreqs()
        {
            List<CatchLenFreq> thisList = new List<CatchLenFreq>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from dbo_catch_len_freq";
                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            CatchLenFreq item = new CatchLenFreq();
                            item.PK = (int)dr["catch_len_freq_id"];
                            item.VesselCatchID = (int)dr["catch_id"];
                            item.LengthClass = (double)dr["len_class"];
                            item.Frequency = (int)dr["freq"];
                            thisList.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return thisList;
            }
        }
        public bool Add(CatchLenFreq item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into dbo_catch_len_freq(catch_len_freq_id, catch_id, len_class,freq) Values (?,?,?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                    update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                    update.Parameters.Add("@len_class", OleDbType.Double).Value = item.LengthClass;
                    update.Parameters.Add("@freq", OleDbType.Integer).Value = item.Frequency;
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
            return success;
        }


        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(catch_len_freq_id) AS max_id FROM dbo_catch_len_freq";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
        public bool Update(CatchLenFreq item)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {

                    update.Parameters.Add("@catch_id", OleDbType.Integer).Value = item.Parent.PK;
                    update.Parameters.Add("@len_class", OleDbType.Double).Value = item.LengthClass;
                    update.Parameters.Add("@freq", OleDbType.Integer).Value = item.Frequency;
                    update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;

                    update.CommandText = @"Update dbo_catch_len_freq set
                                        catch_id=@catch_id,
                                        len_class = @len_class,
                                        freq = @freq
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
            return success;
        }
        public bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
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
        public bool Delete(int id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    update.CommandText="Delete * from dbo_catch_len_freq where catch_len_freq_id=@id";
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
