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
                        g.IsGenericGear = (bool)dr["is_generic"];
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
                                g.CodeOfBaseGear = dr["GenericCode"].ToString().ToUpper();
                                g.IsGenericGear = (bool)dr["IsGeneric"];
                                g.Code = dr["GearCode"].ToString().ToUpper();
                                g.GearIsNotUsed = (bool)dr["GearIsNotUsed"];
                                g.IsUsedInLargeCommercial = (bool)dr["IsUsedInLargeCommercial"];
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

        private bool AddToMySQL(Gear g)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                conn.Open();
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@gearName", MySqlDbType.VarChar).Value = g.GearName;
                    update.Parameters.Add("@gearcode", MySqlDbType.VarChar).Value = g.Code.ToUpper();

                    if (g.BaseGear == null)
                    {
                        update.Parameters.Add("@genericcode", MySqlDbType.VarChar).Value = g.CodeOfBaseGear.ToUpper();
                    }
                    else
                    {
                        update.Parameters.Add("@genericcode", MySqlDbType.VarChar).Value = g.BaseGear.Code.ToUpper();
                    }
                    update.Parameters.AddWithValue("@isgeneric", g.IsGenericGear);
                    update.CommandText = @"Insert into gears (gear_name,gear_code,generic_code,is_generic) 
                                        Values (@gearName,@gearcode,@genericcode,@isgeneric)";


                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
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

        public static bool AddFieldToTable(string colName)
        {
            bool success = false;
            string sql = "";
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    switch (colName)
                    {
                        case "IsUsedInLargeCommercial":
                        case "GearIsNotUsed":
                            sql = $"ALTER TABLE gear ADD COLUMN {colName} YESNO";
                            break;

                    }

                    cmd.CommandText = sql;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Add(Gear g)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(g);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into gear (GearName,GearCode,GenericCode,IsGeneric,GearIsNotUsed,IsUsedInLargeCommercial) Values (?,?,?,?,?,?)";

                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@gearName", OleDbType.VarChar).Value = g.GearName;
                        update.Parameters.Add("@gearcode", OleDbType.VarChar).Value = g.Code.ToUpper();
                        if (g.BaseGear == null)
                        {
                            update.Parameters.Add("genericcode", OleDbType.VarChar).Value = g.CodeOfBaseGear.ToUpper();
                        }
                        else
                        {
                            update.Parameters.Add("@genericcode", OleDbType.VarChar).Value = g.BaseGear.Code.ToUpper();
                        }
                        update.Parameters.Add("@isgeneric", OleDbType.Boolean).Value = g.IsGenericGear;
                        update.Parameters.Add("@is_not_used", OleDbType.Boolean).Value = g.GearIsNotUsed;
                        update.Parameters.Add("@islargecommercial", OleDbType.Boolean).Value = g.IsUsedInLargeCommercial;
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException msex)
                        {
                            Logger.Log(msex);
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
        private bool UpdateMySQL(Gear g)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@gearname", MySqlDbType.VarChar).Value = g.GearName;
                    update.Parameters.Add("@genericcode", MySqlDbType.VarChar).Value = g.BaseGear.Code.ToUpper();
                    update.Parameters.AddWithValue("@isgeneric", g.IsGenericGear);
                    update.Parameters.Add("@gearcode", MySqlDbType.VarChar).Value = g.Code.ToUpper();
                    //update.Parameters.Add("@is_used", MySqlDbType.VarChar).Value = g.Code.ToUpper();

                    update.CommandText = @"UPDATE gears SET
                                           gear_name = @gearname,
                                           generic_code = @genericcode,
                                           is_generic = @isgeneric 
                                           WHERE gear_code = @gearcode";

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
        public bool Update(Gear g)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(g);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@gearname", OleDbType.VarChar).Value = g.GearName;
                        update.Parameters.Add("@genericcode", OleDbType.VarChar).Value = g.BaseGear.Code.ToUpper();
                        update.Parameters.Add("@isgeneric", OleDbType.Boolean).Value = g.IsGenericGear;
                        update.Parameters.Add("@is_not_used", OleDbType.Boolean).Value = g.GearIsNotUsed;
                        update.Parameters.Add("@islargecommercial", OleDbType.Boolean).Value = g.IsUsedInLargeCommercial;
                        update.Parameters.Add("@gearcode", OleDbType.VarChar).Value = g.Code.ToUpper();

                        update.CommandText = @"UPDATE gear SET
                                           GearName = @gearname,
                                           GenericCode = @genericcode,
                                           IsGeneric = @isgeneric,
                                           GearIsNotUsed = @is_not_used,
                                           IsUsedInLargeCommercial = @islargecommercial
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
            }
            return success;
        }
        private bool DeleteMySQL(string code)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    update.CommandText = "Delete from gears where gear_code=@code";
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
        public bool Delete(string code)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = DeleteMySQL(code);
            }
            else
            {
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
            }
            return success;
        }
    }
}