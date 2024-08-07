﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using System.Data;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities
{
    public class EffortSpecificationRepository
    {
        public List<EffortSpecification> EffortSpecifications { get; set; }

        public EffortSpecificationRepository()
        {
            EffortSpecifications = getEffortSpecifications();
        }

        public static List<GearEffortSpecification> GetGearEffortSpecifications()
        {
            List<GearEffortSpecification> specs = new List<GearEffortSpecification>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "Select * from GearEffortSpecification";
                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                GearEffortSpecification ges = new GearEffortSpecification
                                {
                                    RowID=(int)dr["RowID"],
                                    EffortSpecificationID=(int)dr["EffortSpec"],
                                    GearCode = dr["GearCode"].ToString()
                                };
                                specs.Add(ges);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return specs;
        }
        private List<EffortSpecification> getEffortSpecificationsMySQL()
        {
            List<EffortSpecification> thisList = new List<EffortSpecification>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select * from effort_specification";
                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        EffortSpecification es = new EffortSpecification();
                        es.ID = Convert.ToInt32(dr["effort_specification_id"]);
                        es.Name = dr["effort_specification"].ToString();
                        es.IsForAllTypesFishing = Convert.ToBoolean(dr["for_all_types_of_fishing"]);
                        es.ValueType = (ODKValueType)Convert.ToInt32(dr["value_type"]);
                        thisList.Add(es);
                    }
                }
            }
            return thisList;
        }
        private List<EffortSpecification> getEffortSpecifications()
        {
            List<EffortSpecification> theList = new List<EffortSpecification>();
            if (Global.Settings.UsemySQL)
            {
                theList = getEffortSpecificationsMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    try
                    {
                        conection.Open();
                        string query = "Select * from effortSpecification";


                        var adapter = new OleDbDataAdapter(query, conection);
                        adapter.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            theList.Clear();
                            foreach (DataRow dr in dt.Rows)
                            {
                                EffortSpecification es = new EffortSpecification();
                                es.ID = Convert.ToInt32(dr["EffortSpecificationID"]);
                                es.IsForAllTypesFishing = (bool)dr["ForAllTypeOfFishing"];
                                es.Name = dr["EffortSpecification"].ToString();
                                es.ValueType = (ODKValueType)Convert.ToInt32(dr["ValueType"]);
                                theList.Add(es);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);

                    }
                }

            }
            return theList;
        }
        private bool AddToMySQL(EffortSpecification es)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                conn.Open();
                using (var update = conn.CreateCommand())
                {


                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = es.ID;
                    update.Parameters.Add("@effort_spec", MySqlDbType.VarChar).Value = es.Name;
                    update.Parameters.AddWithValue("@is_for_all", es.IsForAllTypesFishing);
                    update.Parameters.Add("@type", MySqlDbType.Int32).Value = (int)es.ValueType;
                    update.CommandText = @"Insert into effort_specification(effort_specification_id, effort_specification, for_all_types_of_fishing, value_type)
                                            Values (@id,@effort_spec,@is_for_all,@type)";
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
        public bool Add(EffortSpecification es)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(es);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    var sql = "Insert into effortSpecification(EffortSpecificationID, EffortSpecification, ForAllTypeOfFishing, ValueType) Values (?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@id", OleDbType.Integer).Value = es.ID;
                        update.Parameters.Add("@effort_spec", OleDbType.VarChar).Value = es.Name;
                        update.Parameters.Add("@is_for_all", OleDbType.Boolean).Value = es.IsForAllTypesFishing;
                        update.Parameters.Add("@type", OleDbType.Integer).Value = (int)es.ValueType;
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
        private bool UpdateMySQL(EffortSpecification es)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@effort_spec", MySqlDbType.VarChar).Value = es.Name;
                    update.Parameters.AddWithValue("@is_for_all", es.IsForAllTypesFishing);
                    update.Parameters.Add("@type", MySqlDbType.Int32).Value = (int)es.ValueType;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = es.ID;

                    update.CommandText = @"Update effort_specification set
                                effort_specification = @effort_spec,
                                for_all_types_of_fishing=@is_for_all,
                                value_type = @type
                                WHERE effort_specification_id = @id";

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
        public bool Update(EffortSpecification es)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(es);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@effort_spec", OleDbType.VarChar).Value = es.Name;
                        update.Parameters.Add("@is_for_all", OleDbType.Boolean).Value = es.IsForAllTypesFishing;
                        update.Parameters.Add("@type", OleDbType.Integer).Value = (int)es.ValueType;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = es.ID;

                        update.CommandText = @"Update effortSpecification set
                                EffortSpecification = @effort_spec,
                                ForAllTypeOfFishing=@is_for_all,
                                ValueType = @type
                                WHERE EffortSpecificationID = @id";

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
                    update.CommandText = "Delete  from effort_specification where effort_specification_id=@id";
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
                        update.CommandText = "Delete * from effortSpecification where EffortSpecificationID=@id";
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
                        cmd.CommandText = "SELECT Max(effort_specification_id) AS max_id FROM effort_specification";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(EffortSpecificationID) AS max_id FROM effortSpecification";
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
