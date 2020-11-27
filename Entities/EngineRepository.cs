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
    public class EngineRepository
    {
        public List<Engine> Engines{ get; set; }

        public EngineRepository()
        {
            Engines = getEngines();
        }

        private List<Engine> getEngines()
        {
            List<Engine> listEngines = new List<Engine>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from engine";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listEngines.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Engine en = new Engine();
                            en.EngineID= Convert.ToInt32( dr["EngineID"]);
                            en.HorsePower = Convert.ToDouble(dr["Horsepower"]);
                            en.ManufacturerName = dr["ManufacturerName"].ToString();
                            en.ModelName = dr["ModelName"].ToString();
                            listEngines.Add(en);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return listEngines;
            }
        }

        public bool Add(Engine en)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into engine(EngineID, Horsepower,ManufacturerName, ModelName)
                           Values ({en.EngineID},{en.HorsePower},'{en.ManufacturerName}','{en.ModelName}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(Engine en)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update engine set
                                Horsepower = {en.HorsePower},
                                ManufacturerName='{en.ManufacturerName}',
                                ModelName = '{en.ModelName}'
                            WHERE EngineID = {en.EngineID}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
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
                var sql = $"Delete * from engine where EngineID={id}";
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
        public int MaxRecordNumber()
        {
            int max_rec_no = 0;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                const string sql = "SELECT Max(EngineID) AS max_id FROM engine";
                using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                {
                    max_rec_no = (int)getMax.ExecuteScalar();
                }
            }
            return max_rec_no;
        }
    }
}
