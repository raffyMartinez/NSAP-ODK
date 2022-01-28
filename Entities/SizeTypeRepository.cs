using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities
{
    class SizeTypeRepository
    {
        public List<SizeType> SizeTypes { get; set; }

        public SizeTypeRepository()
        {
            SizeTypes = getSizeTypes();
        }

        private List<SizeType> getSizeTypes()
        {
            List<SizeType> thisList= new List<SizeType>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from sizeTypes";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        thisList.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            SizeType st = new SizeType();
                            st.Code = dr["SizeTypeCode"].ToString();
                            st.Name = dr["SizeTypeName"].ToString();
                            thisList.Add(st);
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

        public bool Add(SizeType s)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into sizeTypes (SizeTypeCode, SizeTypeName) Values (?,?)";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@size_code", OleDbType.VarChar).Value = s.Code;
                    update.Parameters.Add("@size_name", OleDbType.VarChar).Value = s.Name;
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

        public bool Update(SizeType s)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();

                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@size_code", OleDbType.VarChar).Value = s.Code;
                    update.Parameters.Add("@size_name", OleDbType.VarChar).Value = s.Name;

                    update.CommandText = @"Update sizeType set
                                SizeTypeCode = @size_code,
                                SizeTypeName = @size_name
                            WHERE SizeTypeCode = @size_code";

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

        public bool Delete(string code)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", OleDbType.VarChar).Value = code;
                    update.CommandText="Delete * from sizeType  where TaxaCode=@code";
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
