﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using NSAP_ODK.NSAPMysql;
using MySql.Data.MySqlClient;

namespace NSAP_ODK.Entities
{
    class GPSRepository
    {
        public List<GPS> GPSes { get; set; }


        public GPSRepository()
        {
            GPSes = getGPSes();
        }
        private List<GPS> getGPSMySQL()
        {
            List<GPS> listGPS = new List<GPS>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from gps";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        GPS gps = new GPS();
                        gps.Code = dr["gps_code"].ToString();
                        gps.AssignedName = dr["assigned_name"].ToString();
                        gps.Brand = dr["brand"].ToString();
                        gps.Model = dr["model"].ToString();
                        if (dr["device_type"] != null && dr["device_type"].ToString().Length > 0)
                        {
                            gps.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), dr["device_type"].ToString());
                        }
                        else
                        {
                            gps.DeviceType = DeviceType.DeviceTypeGPS;
                        }
                        listGPS.Add(gps);
                    }
                }
            }
            return listGPS;
        }
        private List<GPS> getGPSes()
        {
            List<GPS> listGPS = new List<GPS>();

            if (Global.Settings.UsemySQL)
            {
                return getGPSMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    Logger.Log($"Global connection string is: {Global.ConnectionString}");
                    try
                    {
                        conection.Open();
                        string query = $"Select * from gps";


                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            listGPS.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                GPS gps = new GPS();
                                gps.Code = dr["GPSCode"].ToString();
                                gps.AssignedName = dr["AssignedName"].ToString();
                                gps.Brand = dr["Brand"].ToString();
                                gps.Model = dr["Model"].ToString();
                                if (dr["DeviceType"] != null && dr["DeviceType"].ToString().Length > 0)
                                {
                                    gps.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), dr["DeviceType"].ToString());
                                }
                                else
                                {
                                    gps.DeviceType = DeviceType.DeviceTypeGPS;
                                }
                                listGPS.Add(gps);
                            }
                        }
                    }
                    catch (OleDbException dbex)
                    {
                        //switch(dbex.HResult)
                        //{
                        //    case -1:
                        //        break;
                        //}
                    }
                    catch (Exception ex)
                    {
                        switch (ex.HResult)
                        {
                            case -2147024809:
                                if (AddColumn("DeviceType", "int") && UpdateAllDevicesToGPS())
                                {

                                    return getGPSes();
                                }

                                break;
                            default:
                                Logger.Log(ex);
                                break;
                        }

                    }
                }
                return listGPS;
            }
        }

        private bool UpdateAllDevicesToGPS()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update gps set DeviceType = 1";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }
        private bool AddColumn(string colName, string type, int? length = null)
        {
            string sql = "";
            if (type == "bool")
            {
                sql = $"ALTER TABLE gps ADD COLUMN {colName} BIT DEFAULT 0";
            }
            else
            {
                if (length == null)
                {
                    sql = $"ALTER TABLE gps ADD COLUMN {colName} {type}";
                }
                else
                {
                    sql = $"ALTER TABLE gps ADD COLUMN {colName} {type}({length})";
                }
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                con.Open();
                OleDbCommand myCommand = new OleDbCommand();
                myCommand.Connection = con;
                myCommand.CommandText = sql;
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return false;
                }
                myCommand.Connection.Close();
                return true;
            }
        }

        private bool AddToMySQL(GPS gps)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@code", gps.Code);
                    cmd.Parameters.AddWithValue("@name", gps.AssignedName);
                    cmd.Parameters.AddWithValue("@brand", gps.Brand);
                    cmd.Parameters.AddWithValue("@model", gps.Model);
                    cmd.Parameters.AddWithValue("@type", (int)gps.DeviceType);
                    cmd.CommandText = @"Insert into gps (gps_code, assigned_name, brand, model, device_type)
                                    values (@code, @name, @brand, @model, @type)";
                    conn.Open();
                    try
                    {
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch(MySqlException msex)
                    {

                    }
                    catch(Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Add(GPS gps)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(gps);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into gps(GPSCode,AssignedName,Brand,Model,DeviceType) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = gps.Code;
                        update.Parameters.Add("@name", OleDbType.VarChar).Value = gps.AssignedName;
                        update.Parameters.Add("@brand", OleDbType.VarChar).Value = gps.Brand;
                        update.Parameters.Add("@model", OleDbType.VarChar).Value = gps.Model;
                        update.Parameters.Add("@device_type", OleDbType.Integer).Value = (int)gps.DeviceType;
                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException dbex)
                        {
                            switch(dbex.ErrorCode)
                            {
                                case -2147467259:
                                    //duplicate value in index or primary key error
                                    if(dbex.Message.Contains("create duplicate values in the index, primary key, or relationship"))
                                    {
                                        //success would be false so just continue
                                    }
                                    break;
                                default:
                                    Logger.Log(dbex);
                                    break;
                            }
                            //Logger.Log(dbex);
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
        private bool UpdateMySQL(GPS gps)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@name", MySqlDbType.VarChar).Value = gps.AssignedName;
                    update.Parameters.Add("@brand", MySqlDbType.VarChar).Value = gps.Brand;
                    update.Parameters.Add("@model", MySqlDbType.VarChar).Value = gps.Model;
                    update.Parameters.Add("@device_type", MySqlDbType.Int32).Value = (int)gps.DeviceType;
                    update.Parameters.Add("@code", MySqlDbType.VarChar).Value = gps.Code;

                    update.CommandText = @"Update gps set
                                           assigned_name= @name,
                                           brand = @brand,
                                           model = @model,
                                           device_type = @device_type    
                                           WHERE gps_code = @code";

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
        public bool Update(GPS gps)
        {
            bool success = false; if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(gps);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@name", OleDbType.VarChar).Value = gps.AssignedName;
                        update.Parameters.Add("@brand", OleDbType.VarChar).Value = gps.Brand;
                        update.Parameters.Add("@model", OleDbType.VarChar).Value = gps.Model;
                        update.Parameters.Add("@device_type", OleDbType.Integer).Value = (int)gps.DeviceType;
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = gps.Code;
                        update.CommandText = @"Update gps set
                                           AssignedName= @name,
                                           Brand = @brand,
                                           Model = @model,
                                           DeviceType = @device_type    
                                           WHERE GPSCode = @code";
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
        private bool DeleteMySQL(string code)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    update.CommandText = "Delete  from gps where gps_code=@code";
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
                        update.CommandText = "Delete * from gps where GPSCode=@code";
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
    }
}
