using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    public class GearRepository
    {
        public List<Gear> Gears { get; set; }

        public GearRepository()
        {
            Gears = getGears();
        }

        private List<Gear> getGearsMySQL()
        {
            List<Gear> thisList = new List<Gear>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from gears";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Gear g = new Gear();
                        g.Code = dr["gear_code"].ToString();
                        g.GearName = dr["gear_name"].ToString();
                        g.CodeOfBaseGear = dr["generic_code"].ToString();
                        g.IsGenericGear = Convert.ToBoolean(dr["is_generic"]);
                        thisList.Add(g);
                    }
                }
            }
            return thisList;
        }
        private List<Gear> getGears()
        {
            List<Gear> listGears = new List<Gear>();
            if (Global.Settings.UsemySQL)
            {
                listGears = getGearsMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from gear";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listGears.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                Gear g = new Gear();
                                g.GearName = dr["GearName"].ToString();
                                g.CodeOfBaseGear = dr["GenericCode"].ToString();
                                g.IsGenericGear = (bool)dr["IsGeneric"];
                                g.Code = dr["GearCode"].ToString();
                                listGears.Add(g);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
            return listGears;
        }
        public bool Add(Gear g)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "Insert into gear (GearName,GearCode,GenericCode,IsGeneric) Values (?,?,?,?)";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    update.Parameters.Add("@gearName", OleDbType.VarChar).Value = g.GearName;
                    update.Parameters.Add("@gearcode", OleDbType.VarChar).Value = g.Code;
                    if (g.BaseGear == null)
                    {
                        update.Parameters.Add("genericcode", OleDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@genericcode", OleDbType.VarChar).Value = g.BaseGear.Code;
                    }
                    update.Parameters.Add("@isgeneric", OleDbType.Boolean).Value = g.IsGenericGear;
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

        public bool Update(Gear g)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                using (OleDbCommand update = conn.CreateCommand())
                {
                    update.Parameters.Add("@gearname", OleDbType.VarChar).Value = g.GearName;
                    update.Parameters.Add("@genericcode", OleDbType.VarChar).Value = g.BaseGear.Code;
                    update.Parameters.Add("@isgeneric", OleDbType.Boolean).Value = g.IsGenericGear;
                    update.Parameters.Add("@gearcode", OleDbType.VarChar).Value = g.Code;

                    update.CommandText = @"UPDATE gear SET
                                           GearName = @gearname,
                                           GenericCode = @genericcode,
                                           IsGeneric = @isgeneric 
                                           WHERE GearCode = @gearcode";
                    try
                    {
                        //string commandString = update.CommandText;
                        //foreach (OleDbParameter parameter in update.Parameters)
                        //    commandString = commandString.Replace(parameter.ParameterName.ToString(), parameter.Value.ToString());

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
                    update.CommandText = "Delete * from gear where GearCode=@code";
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