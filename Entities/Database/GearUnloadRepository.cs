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
    class GearUnloadRepository
    {
        public List<GearUnload> GearUnloads { get; set; }

        public GearUnloadRepository(LandingSiteSampling ls)
        {
            GearUnloads = getGearUnloads(ls);
        }
        public GearUnloadRepository()
        {
            GearUnloads = getGearUnloads();
        }

        public int MaxRecordNumber()
        {
            return NSAPEntities.SummaryItemViewModel.GetGearUnloadMaxRecordNumber();
        }
        public int MaxRecordNumber1()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(unload_gr_id) AS max_id FROM dbo_gear_unload";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(unload_gr_id) AS max_id FROM dbo_gear_unload";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            var r = getMax.ExecuteScalar();
                            if (r != DBNull.Value)
                            {
                                max_rec_no = (int)r;
                            }
                        }
                        catch (OleDbException olx)
                        {
                            Logger.Log(olx);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return max_rec_no;
        }

        private List<GearUnload> getFromMySQL(LandingSiteSampling ls = null)
        {
            List<GearUnload> thisList = new List<GearUnload>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_gear_unload";
                    if (ls != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", ls.PK);
                        cmd.CommandText = $"Select * from dbo_gear_unload where unload_day_id=@parentID";
                    }
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        GearUnload item = new GearUnload();
                        item.Parent = ls;
                        item.PK = (int)dr["unload_gr_id"];
                        item.LandingSiteSamplingID = (int)dr["unload_day_id"];
                        item.GearID = dr["gr_id"].ToString();
                        item.Boats = string.IsNullOrEmpty(dr["boats"].ToString()) ? null : (int?)dr["boats"];
                        item.Catch = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"];
                        item.GearUsedText = dr["gr_text"].ToString();
                        item.Remarks = dr["remarks"].ToString();
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }



        public static int GearUnloadCount(bool countCompleted = false)
        {
            int count = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select count(*) from dbo_gear_unload";
                        if (countCompleted)
                        {
                            cmd.CommandText = "Select count(*) from dbo_gear_unload where boats Is Not null And catch Is Not Null";
                        }
                        try
                        {
                            conn.Open();
                            count = (int)(long)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
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
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select count(*) from dbo_gear_unload";
                        if (countCompleted)
                        {
                            cmd.CommandText = "Select count(*) from dbo_gear_unload where boats Is Not null And catch Is Not Null";
                        }
                        try
                        {
                            conn.Open();
                            count = (int)cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }

            }
            return count;
        }

        private List<GearUnload> getGearUnloads(LandingSiteSampling ls = null)
        {
            List<GearUnload> thisList = new List<GearUnload>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(ls);
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
                            cmd.CommandText = $"Select * from dbo_gear_unload";

                            if (ls != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", ls.PK);
                                cmd.CommandText = $"Select * from dbo_gear_unload where unload_day_id=@parentID";
                            }


                            OleDbDataReader dr = cmd.ExecuteReader();
                            thisList.Clear();
                            while (dr.Read())
                            {
                                GearUnload item = new GearUnload();
                                item.Parent = ls;
                                item.PK = (int)dr["unload_gr_id"];
                                item.LandingSiteSamplingID = (int)dr["unload_day_id"];
                                item.GearID = dr["gr_id"].ToString();
                                item.Boats = string.IsNullOrEmpty(dr["boats"].ToString()) ? null : (int?)dr["boats"];
                                item.Catch = string.IsNullOrEmpty(dr["catch"].ToString()) ? null : (double?)dr["catch"];
                                item.GearUsedText = dr["gr_text"].ToString();
                                item.Remarks = dr["remarks"].ToString();
                                //item.VesselUnloadViewModel = new VesselUnloadViewModel(item);
                                thisList.Add(item);

                            }
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

        private bool AddToMySQL(GearUnload item)
        {
            bool success = false;

            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@parent", MySqlDbType.Int32).Value = item.LandingSiteSamplingID;
                    //success = false;
                    if (item.GearID == null || item.GearID.Length == 0)
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.VarChar).Value = item.GearID;
                    }

                    if (item.Boats == null)
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = item.Boats;
                    }

                    if (item.Catch == null)
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = item.Catch;
                    }

                    if (item.GearUsedText == null)
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = item.GearUsedText;
                    }

                    if (item.Remarks == null)
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = item.Remarks;
                    }
                    update.CommandText = @"Insert into dbo_gear_unload(unload_gr_id, unload_day_id, gr_id,boats,catch,gr_text,remarks) 
                                       Values (@pk,@parent,@gear_id,@boats,@catch,@gear_text,@remarks)";
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
        public bool Add(GearUnload item)
        {

            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = AddToMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();


                    var sql = "Insert into dbo_gear_unload(unload_gr_id, unload_day_id, gr_id,boats,catch,gr_text,remarks) Values (?,?,?,?,?,?,?)";

                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                        update.Parameters.Add("@parent", OleDbType.Integer).Value = item.LandingSiteSamplingID;

                        if (item.GearID == null)
                        {
                            update.Parameters.Add("@gear_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_id", OleDbType.VarChar).Value = item.GearID;
                        }
                        //var gearID = item.GearID == null ? DBNull.Value : item.GearID; 

                        //}

                        if (item.Boats == null)
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = item.Boats;
                        }

                        if (item.Catch == null)
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = item.Catch;
                        }

                        if (item.GearUsedText == null)
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearUsedText;
                        }

                        if (item.Remarks == null)
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                        }


                        try
                        {
                            success = update.ExecuteNonQuery() > 0;
                        }
                        catch (OleDbException odbex)
                        {
                            switch (odbex.ErrorCode)
                            {
                                case -2147467259:
                                    //duplicate index or unique keys error
                                    break;
                                default:
                                    success = false;
                                    Logger.Log(odbex);
                                    break;
                            }

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
        private bool UpdateMySQL(GearUnload item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@parent", MySqlDbType.Int32).Value = item.LandingSiteSamplingID;

                    if (item.GearID == null)
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_id", MySqlDbType.VarChar).Value = item.GearID;
                    }

                    if (item.Boats == null)
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@boats", MySqlDbType.Int32).Value = item.Boats;
                    }

                    if (item.Catch == null)
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@catch", MySqlDbType.Double).Value = item.Catch;
                    }

                    if (item.GearUsedText == null)
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@gear_text", MySqlDbType.VarChar).Value = item.GearUsedText;
                    }

                    if (item.Remarks == null)
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = item.Remarks;
                    }
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_gear_unload set
                            unload_day_id=@parent,
                            gr_id = @gear_id,
                            boats = @boats,
                            catch = @catch,
                            gr_text = @gear_text,
                            remarks = @remarks
                        WHERE unload_gr_id = @pk";

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
        public bool Update(GearUnload item)
        {
            bool success = false;
            if (Global.Settings.UsemySQL)
            {
                success = UpdateMySQL(item);
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();

                    using (OleDbCommand update = conn.CreateCommand())
                    {

                        update.Parameters.Add("@parent", OleDbType.Integer).Value = item.LandingSiteSamplingID;

                        if (item.GearID == null)
                        {
                            update.Parameters.Add("@gear_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_id", OleDbType.VarChar).Value = item.GearID;
                        }

                        if (item.Boats == null)
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@boats", OleDbType.Integer).Value = item.Boats;
                        }

                        if (item.Catch == null)
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@catch", OleDbType.Double).Value = item.Catch;
                        }

                        if (item.GearUsedText == null)
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@gear_text", OleDbType.VarChar).Value = item.GearUsedText;
                        }

                        if (item.Remarks == null)
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                        }
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;

                        update.CommandText = @"Update dbo_gear_unload set
                            unload_day_id=@parent,
                            gr_id = @gear_id,
                            boats = @boats,
                            catch = @catch,
                            gr_text = @gear_text,
                            remarks = @remarks
                        WHERE unload_gr_id = @pk";

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
            }
            return success;
        }
        public static bool ClearTable()
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from dbo_gear_unload";
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

        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete * from dbo_gear_unload where unload_gr_id=@id";
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
                        update.CommandText = "Delete * from dbo_gear_unload where unload_gr_id=@id";
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
