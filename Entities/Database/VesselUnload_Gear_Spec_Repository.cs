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

namespace NSAP_ODK.Entities.Database
{
    class VesselUnload_Gear_Spec_Repository
    {
        VesselUnload_FishingGear _parent;
        public List<VesselUnload_Gear_Spec> VesselUnload_Gear_Specses { get; set; }

        public static bool UpdateTable(bool addFKConstraint = false)

        {
            bool success = false;
            string sql = "ALTER TABLE dbo_vessel_effort ADD COLUMN vessel_unload_fishing_gear_id int";

            if (addFKConstraint)
            {
                sql = @"ALTER TABLE dbo_vessel_effort ADD CONSTRAINT FK_vufg_gears_efforts
                                                    FOREIGN KEY(vessel_unload_fishing_gear_id) REFERENCES dbo_vesselunload_fishinggear(row_id)";
            }

            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        if (!addFKConstraint)
                        {
                            UpdateTable(addFKConstraint: true);
                        }
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

        public static Task<bool> DeleteMultivesselDataAsync(bool isMultivessel)
        {
            return Task.Run(() => DeleteMultivesselData(isMultivessel));
        }
        public static bool DeleteMultivesselData(bool isMultivessel)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        if (isMultivessel)
                        {
                            cmd.CommandText = @"Delete * from dbo_vessel_effort WHERE v_unload_id IS NULL";
                        }
                        else
                        {
                            cmd.CommandText = "Delete * from dbo_vessel_effort WHERE v_unload_id IS NOT NULL";
                        }
                        try
                        {
                            con.Open();
                            success = cmd.ExecuteNonQuery() >= 0;

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
        public static bool ClearTable(string otherConnectionString = "")
        {
            bool success = false;
            string con_string = Global.ConnectionString;
            if (otherConnectionString.Length > 0)
            {
                con_string = otherConnectionString;
            }

            using (OleDbConnection conn = new OleDbConnection(con_string))
            {
                conn.Open();
                var sql = $"Delete * from dbo_vessel_effort";
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
        public VesselUnload_Gear_Spec_Repository(VesselUnload_FishingGear parent)
        {
            if (parent != null)
            {
                _parent = parent;
                VesselUnload_Gear_Specses = getGears_Specs();
            }
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
                        //conn.Open();
                        //cmd.CommandText = "SELECT Max(effort_row_id) AS max_id FROM dbo_vessel_effort";
                        //max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(effort_row_id) AS max_id FROM dbo_vessel_effort";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        var r = getMax.ExecuteScalar();
                        if (r != DBNull.Value)
                        {
                            max_rec_no = (int)r;
                        }
                    }
                }
            }
            return max_rec_no;
        }

        public bool Add(VesselUnload_Gear_Spec item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                //success = AddToMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    
                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        try
                        {
                            update.Parameters.Add("@id", OleDbType.Integer).Value = item.RowID;
                            update.Parameters.Add("@unload_gear_id", OleDbType.Integer).Value = item.Parent.RowID;
                            update.Parameters.Add("@v_unload_id", OleDbType.Integer).Value = DBNull.Value;
                            update.Parameters.Add("@spec_id", OleDbType.Integer).Value = item.EffortSpecID;
                            if (item.EffortValueNumeric == null)
                            {
                                update.Parameters.Add("@effort_value_numeric", OleDbType.Double).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@effort_value_numeric", OleDbType.VarChar).Value = item.EffortValueNumeric;
                            }
                            if (string.IsNullOrEmpty(item.EffortValueText))
                            {
                                update.Parameters.Add("@effort_value_text", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@effort_value_text", OleDbType.VarChar).Value = item.EffortValueText;
                            }

                            update.CommandText= @"Insert into dbo_vessel_effort (
                                                    effort_row_id, 
                                                    vessel_unload_fishing_gear_id,
                                                    v_unload_id,
                                                    effort_spec_id,
                                                    effort_value_numeric,
                                                    effort_value_text) Values (?,?,?,?,?,?)";

                            try
                            {
                                success = update.ExecuteNonQuery() > 0;
                            }
                            catch (OleDbException dbex)
                            {
                                switch (dbex.ErrorCode)
                                {
                                    case -2147467259:
                                        //error due to duplicated key or index
                                        break;
                                    default:
                                        Logger.Log(dbex);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                        catch (OleDbException odbex)
                        {
                            Logger.Log(odbex);
                            success = false;
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

        public bool Update(VesselUnload_Gear_Spec item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                //success = UpdateMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@unload_gear_id", OleDbType.Integer).Value = item.Parent.RowID;
                        update.Parameters.Add("@spec_id", OleDbType.Integer).Value = item.EffortSpecID;
                        if (item.EffortValueNumeric == null)
                        {
                            update.Parameters.Add("@effort_value_numeric", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@effort_value_numeric", OleDbType.Double).Value = item.EffortValueNumeric;
                        }

                        if (string.IsNullOrEmpty(item.EffortValueText))
                        {
                            update.Parameters.Add("@effort_value_text", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@effort_value_text", OleDbType.Double).Value = item.EffortValueText;
                        }
                        update.Parameters.Add("@unload_id", OleDbType.Integer).Value = DBNull.Value;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.RowID;


                        update.CommandText = @"Update dbo_vessel_effort set
                                            vessel_unload_fishing_gear_id=@unload_gear_id,
                                            effort_spec_id=@spec_id,
                                            effort_value_numeric = @effort_value_numeric,
                                            effort_value_text = @effort_value_text,
                                            v_unload_id = @unload_id    
                                        WHERE effort_row_id = @id";

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

        public bool Delete(int id)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                //success = DeleteMySQL(id);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = id;
                        update.CommandText = "Delete * from dbo_vessel_effort where effort_row_id=@id";
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

        public List<VesselUnload_Gear_Spec> getGears_Specs()
        {
            List<VesselUnload_Gear_Spec> thisList = new List<VesselUnload_Gear_Spec>();
            if (Global.Settings.UsemySQL)
            {
                //thisList = getFromMySQL(vu);
            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            //cmd.CommandText = "Select * from dbo_vessel_catch";


                            cmd.Parameters.AddWithValue("@parentID", _parent.RowID);
                            cmd.CommandText = "Select * from dbo_vessel_effort where vessel_unload_fishing_gear_id=@parentID";


                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                double? evn = null;
                                if (dr["effort_value_numeric"] != DBNull.Value)
                                {
                                    evn = (double)dr["effort_value_numeric"];
                                }
                                VesselUnload_Gear_Spec vugs = new VesselUnload_Gear_Spec
                                {
                                    RowID = (int)dr["effort_row_id"],
                                    VesselUnload_FishingGears_ID = (int)dr["vessel_unload_fishing_gear_id"],
                                    Parent = _parent,
                                    EffortSpecID = (int)dr["effort_spec_id"],
                                    EffortValueNumeric = evn,
                                    EffortValueText = dr["effort_value_text"].ToString()
                                };
                                thisList.Add(vugs);
                            }

                        }
                        catch (OleDbException ole_ex)
                        {
                            //
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }


                    }
                }
            }
            return thisList;
        }

    }
}
