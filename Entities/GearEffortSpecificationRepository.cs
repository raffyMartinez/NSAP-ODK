using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    internal class GearEffortSpecificationRepository
    {
        public List<GearEffortSpecification> GearEffortSpecifications { get; set; }

        public GearEffortSpecificationRepository(Gear gear)
        {
            GearEffortSpecifications = getGearEffortSpecifications(gear);
        }

        public static int AllGearEffortSpecificationMaxRecordNumber()
        {
            int maxRecordNumber = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText =  "SELECT Max([row_id]) AS MaxRowID FROM gear_effort_specification";
                        try
                        {
                            conn.Open();
                            maxRecordNumber = (int)cmd.ExecuteScalar();
                        }
                        catch(MySqlException msx)
                        {
                            Logger.Log(msx);
                        }
                        catch(Exception ex)
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
                    conn.Open();
                    const string sql = "SELECT Max([RowId]) AS MaxRowID FROM GearEffortSpecification";

                    using (OleDbCommand getCount = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            maxRecordNumber = (int)getCount.ExecuteScalar();
                        }
                        catch (OleDbException oex)
                        {
                            Logger.Log(oex);
                            maxRecordNumber = 0;
                        }
                        catch
                        {
                            maxRecordNumber = 0;
                        }
                    }
                }
            }
            return maxRecordNumber;
        }

        private List<GearEffortSpecification> getFromMySQL(Gear gear)
        {
            List<GearEffortSpecification> thisList = new List<GearEffortSpecification>();

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.AddWithValue("@gear", gear.Code);
                    cmd.CommandText = "Select * from gear_effort_specification where gear=@gear";
                    conn.Open();
                    try
                    {
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            GearEffortSpecification ges = new GearEffortSpecification();
                            ges.EffortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(Convert.ToInt32(dr["effort_spec"]));
                            ges.Gear = gear;
                            ges.RowID = Convert.ToInt32(dr["row_id"]);
                            thisList.Add(ges);
                        }
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return thisList;
        }
        private List<GearEffortSpecification> getGearEffortSpecifications(Gear gear)
        {
            List<GearEffortSpecification> thisList = new List<GearEffortSpecification>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(gear);
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = $"Select * from GearEffortSpecification where GearCode='{gear.Code}'";

                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            thisList.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                GearEffortSpecification ges = new GearEffortSpecification();
                                ges.EffortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(Convert.ToInt32(dr["EffortSpec"]));
                                ges.Gear = gear;
                                ges.RowID = Convert.ToInt32(dr["RowId"]);
                                thisList.Add(ges);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

            }
            return thisList;
        }
        private bool AddToMySQL(GearEffortSpecification ges)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Parameters.Add("@gear", MySqlDbType.VarChar).Value = ges.Gear.Code;
                    cmd.Parameters.Add("@spec", MySqlDbType.Int32).Value = ges.EffortSpecification.ID;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = ges.RowID;
                    cmd.CommandText = "Insert into gear_effort_specification (gear, effort_spec, row_id) Values (@gear,@spec,@id)";
                    try
                    {
                        conn.Open();
                        success = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (MySqlException msex)
                    {
                        switch (msex.ErrorCode)
                        {
                            case -2147467259:
                                //duplicated unique index error
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
        public bool Add(GearEffortSpecification ges)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(ges);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into GearEffortSpecification (GearCode, EffortSpec, RowId) Values (?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = ges.Gear.Code;
                        update.Parameters.Add("@spec", OleDbType.Integer).Value = ges.EffortSpecification.ID;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = ges.RowID;
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
        private bool UpdateMySQL(GearEffortSpecification ges)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@code", MySqlDbType.VarChar).Value = ges.Gear.Code;
                    update.Parameters.Add("@spec", MySqlDbType.Int32).Value = ges.EffortSpecification.ID;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = ges.RowID;

                    update.CommandText = @"Update gear_effort_specification set
                                            gear = @code,
                                            effort_spec =@spec
                                            WHERE row_id=@id";

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
        public bool Update(GearEffortSpecification ges)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(ges);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@code", OleDbType.VarChar).Value = ges.Gear.Code;
                        update.Parameters.Add("@spec", OleDbType.Integer).Value = ges.EffortSpecification.ID;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = ges.RowID;

                        update.CommandText = @"Update GearEffortSpecification set
                                            GearCode = @code,
                                            EffortSpec =@spec
                                            WHERE RowdId=@id";

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
                    update.CommandText = "Delete * from gear_effort_specification where row_id=@id";
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
                        update.CommandText = "Delete * from GearEffortSpecification where RowId=@id";
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
                        cmd.CommandText = "SELECT Max(row_id) AS max_id FROM gear_effort_specification";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(RowId) AS max_record_no FROM GearEffortSpecification";
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