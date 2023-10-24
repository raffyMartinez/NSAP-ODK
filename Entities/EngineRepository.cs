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
    public class EngineRepository
    {
        public List<Engine> Engines { get; set; }

        public EngineRepository()
        {
            Engines = getEngines();
        }

        private List<Engine> getEnginesMySQL()
        {
            List<Engine> thisList = new List<Engine>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from engines";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Engine engine = new Engine();
                        engine.EngineID= Convert.ToInt32(dr["engine_id"]);
                        engine.ManufacturerName = dr["manufacturer_name"].ToString();
                        engine.ModelName = dr["model_name"].ToString();
                        if (double.TryParse(dr["horsepower"].ToString(),out double v))
                        {
                            engine.HorsePower = v;
                        }
                        thisList.Add(engine);
                    }
                }
            }
            return thisList;
        }
        private List<Engine> getEngines()
        {
            List<Engine> listEngines = new List<Engine>();
            if (Global.Settings.UsemySQL)
            {
                listEngines = getEnginesMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    //Logger.Log($"engine repositiry connection string is: {Global.ConnectionString}");
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
                                en.EngineID = Convert.ToInt32(dr["EngineID"]);
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
                }

            }
            return listEngines;
        }
        private bool AddToMySQL(Engine en)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = en.EngineID;
                    update.Parameters.Add("@hp", MySqlDbType.Double).Value = en.HorsePower;
                    update.Parameters.Add("@manuf", MySqlDbType.VarChar).Value = en.ManufacturerName;
                    update.Parameters.Add("@model", MySqlDbType.VarChar).Value = en.ModelName;
                    update.CommandText = "Insert into engine(engine_id, horsepower,manufacturer_name, model_name) Values (@id,@hp,@manuf,@model)";
                    conn.Open();
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
        public bool Add(Engine en)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(en);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into engine(EngineID, Horsepower,ManufacturerName, ModelName) Values (?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = en.EngineID;
                        update.Parameters.Add("@hp", OleDbType.Double).Value = en.HorsePower;
                        update.Parameters.Add("@manuf", OleDbType.VarChar).Value = en.ManufacturerName;
                        update.Parameters.Add("@model", OleDbType.VarChar).Value = en.ModelName;
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
        private bool UpdateMySQL(Engine en)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@hp", MySqlDbType.Double).Value = en.HorsePower;
                    update.Parameters.Add("@manuf", MySqlDbType.VarChar).Value = en.ManufacturerName;
                    update.Parameters.Add("@model", MySqlDbType.VarChar).Value = en.ModelName;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = en.EngineID;

                    update.CommandText = @"Update engines set
                                horsepower = @hp,
                                manufacturer_name = @manuf,
                                model_name = @model
                            WHERE engine_id = @id";

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
        public bool Update(Engine en)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(en);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@hp", OleDbType.Double).Value = en.HorsePower;
                        update.Parameters.Add("@manuf", OleDbType.VarChar).Value = en.ManufacturerName;
                        update.Parameters.Add("@model", OleDbType.VarChar).Value = en.ModelName;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = en.EngineID;

                        update.CommandText = @"Update engine set
                                Horsepower = @hp,
                                ManufacturerName = @manuf,
                                ModelName = @model
                            WHERE EngineID = @id";
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from engines where engine_id=@id";
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
                        update.CommandText = "Delete * from engine where EngineID=@id";
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
                        cmd.CommandText = "SELECT Max(engine_id) AS max_id FROM engines";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(EngineID) AS max_id FROM engine";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }
    }
}
