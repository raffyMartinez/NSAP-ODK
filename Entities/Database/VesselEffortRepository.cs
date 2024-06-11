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
using NSAP_ODK.TreeViewModelControl;
namespace NSAP_ODK.Entities.Database
{
    public class VesselEffortRepository
    {
        public List<VesselEffort> VesselEfforts { get; set; }

        public static bool UpdateTableDefinition(bool removeMultiFieldIndex = false, string indexName = "")
        {
            bool success = false;
            string sql = "";
            if (removeMultiFieldIndex)
            {
                sql = $"DROP INDEX {indexName} on dbo_vessel_effort";
            }
            using (var con = new OleDbConnection(Global.ConnectionString))
            {
                using (var cmd = con.CreateCommand())
                {
                    con.Open();
                    if (sql.Length > 0)
                    {
                        cmd.CommandText = sql;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("No such index 'alt_key' on table 'dbo_vessel_effort'"))
                            {
                                success = true;
                            }
                            else
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                }
            }
            return success;
        }



        public static Task<bool> DeleteServerDataAsync(string serverID, bool isMultiVessel)
        {
            return Task.Run(() => DeleteServerData(serverID, isMultiVessel));
        }
        private static bool DeleteServerData(string serverID, bool isMultiVessel)
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
                        con.Open();
                        cmd.Parameters.AddWithValue("@id", serverID);
                        if (isMultiVessel)
                        {
                            cmd.CommandText = @"DELETE dbo_vessel_effort.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON 
                                                    dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    (dbo_vesselunload_fishinggear INNER JOIN 
                                                    dbo_vessel_effort ON dbo_vesselunload_fishinggear.row_id = dbo_vessel_effort.vessel_unload_fishing_gear_id) ON 
                                                    dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                        }
                        else
                        {
                            cmd.CommandText = @"DELETE  dbo_vessel_effort.*
                                                FROM ((dbo_gear_unload INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON 
                                                    dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                    dbo_vessel_unload ON dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                    dbo_vessel_effort ON dbo_vessel_unload.v_unload_id = dbo_vessel_effort.v_unload_id
                                                WHERE dbo_LC_FG_sample_day_1.XFormIdentifier=@id";
                        }
                        try
                        {
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
        public static Task<bool> DeleteMultivesselDataAsync(bool isMultivessel)
        {
            return Task.Run(() => DeleteMultivesselData(isMultivessel));
        }
        public static bool DeleteMultivesselData(bool isMultivessel)
        {
            bool success = false;
            if (isMultivessel)
            {
                success = true;
            }
            else
            {
                if (Global.Settings.UsemySQL)
                {

                }
                else
                {
                    using (var con = new OleDbConnection(Global.ConnectionString))
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "DELETE * from dbo_vessel_effort where v_unload_id IS NOT NULL";
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
            }
            return success;
        }

        public VesselEffortRepository()
        {

        }
        public VesselEffortRepository(VesselUnload vu)
        {
            VesselEfforts = getVesselEfforts(vu);
        }
        public VesselEffortRepository(bool isNew = false)
        {
            if (!isNew)
            {
                VesselEfforts = getVesselEfforts();
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
                        conn.Open();
                        cmd.CommandText = "SELECT Max(effort_row_id) AS max_id FROM dbo_vessel_effort";
                        max_rec_no = (int)cmd.ExecuteScalar();
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
        private List<VesselEffort> getFromMySQL(VesselUnload vu = null)
        {
            List<VesselEffort> thisList = new List<VesselEffort>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "Select * from dbo_vessel_effort";
                    if (vu != null)
                    {
                        cmd.Parameters.AddWithValue("@parentID", vu.PK);
                        cmd.CommandText = "Select * from dbo_vessel_effort where v_unload_id=@parentID";
                    }

                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        VesselEffort item = new VesselEffort();
                        item.Parent = vu;
                        item.PK = (int)dr["effort_row_id"];
                        item.VesselUnloadID = (int)dr["v_unload_id"];
                        item.EffortSpecID = (int)dr["effort_spec_id"];
                        item.EffortValueNumeric = string.IsNullOrEmpty(dr["effort_value_numeric"].ToString()) ? null : (double?)dr["effort_value_numeric"];
                        item.EffortValueText = dr["effort_value_text"].ToString();
                        thisList.Add(item);
                    }
                }
            }
            return thisList;
        }

        public static List<VesselEffortCrossTab>GetEffortForCrossTab(AllSamplingEntitiesEventHandler e)
        {
            List<VesselEffortCrossTab> ects = new List<VesselEffortCrossTab>();
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con =  new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@reg", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@ls", e.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        DateTime sDate = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", sDate.ToString("MMM-dd-yyyy"));
                        cmd.Parameters.AddWithValue("@end", sDate.AddMonths(1).ToString("MMM-dd-yyyy"));

                        cmd.CommandText = @"SELECT 
                                                dbo_gear_unload.gr_id as gear_code, 
                                                dbo_vessel_unload.v_unload_id, 
                                                dbo_vessel_effort.effort_spec_id, 
                                                dbo_vessel_effort.effort_value_numeric, 
                                                dbo_vessel_effort.effort_value_text, 
                                                's' AS unload_type
                                            FROM 
                                                dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN 
                                                (dbo_vessel_unload INNER JOIN 
                                                dbo_vessel_effort ON 
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_effort.v_unload_id) ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id = @reg AND 
                                                dbo_LC_FG_sample_day.land_ctr_id = @ls AND 
                                                dbo_LC_FG_sample_day.ground_id = @fg AND 
                                                dbo_LC_FG_sample_day.fma = @fma AND 
                                                dbo_LC_FG_sample_day.sdate >=@start AND 
                                                dbo_LC_FG_sample_day.sdate<@end
                                            ORDER BY 
                                                dbo_vessel_unload.v_unload_id, 
                                                dbo_gear_unload.gr_id;


                                            UNION ALL

                                            SELECT 
                                                dbo_vesselunload_fishinggear.gear_code, 
                                                dbo_vessel_unload.v_unload_id, 
                                                dbo_vessel_effort.effort_spec_id, 
                                                dbo_vessel_effort.effort_value_numeric, 
                                                dbo_vessel_effort.effort_value_text, 'm' AS unload_type
                                            FROM 
                                                dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN 
                                                ((dbo_vessel_unload LEFT JOIN 
                                                dbo_vesselunload_fishinggear ON 
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) 
                                            LEFT JOIN 
                                                dbo_vessel_effort ON 
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_effort.vessel_unload_fishing_gear_id) ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE
                                                dbo_LC_FG_sample_day.region_id = @reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id = @ls AND                                                
                                                dbo_LC_FG_sample_day.ground_id = @fg AND
                                                dbo_LC_FG_sample_day.fma = @fma AND
                                                dbo_LC_FG_sample_day.sdate >=@start AND 
                                                dbo_LC_FG_sample_day.sdate<@end";
                        con.Open();

                        try
                        {
                            OleDbDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                VesselEffortCrossTab vect = new VesselEffortCrossTab
                                {
                                    VesselUnloadID = (int)reader["v_unload_id"],
                                    GearCode = reader["gear_code"].ToString(),
                                    EffortID = (int)reader["effort_spec_id"],
                                    EffortValueText = reader["effort_value_text"].ToString(),
                                    UnloadGearsCategory = reader["unload_type"].ToString()
                                    
                                };
                                if (reader["effort_value_numeric"]!=DBNull.Value)
                                {
                                    vect.EffortValue = (double)reader["effort_value_numeric"];
                                }
                                ects.Add(vect);
                            }
                        }
                        catch(Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return ects;
        }
        private List<VesselEffort> getVesselEfforts(VesselUnload vu = null)
        {
            List<VesselEffort> thisList = new List<VesselEffort>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL(vu);
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
                            cmd.CommandText = "Select * from dbo_vessel_effort";

                            if (vu != null)
                            {
                                cmd.Parameters.AddWithValue("@parentID", vu.PK);
                                cmd.CommandText = "Select * from dbo_vessel_effort where v_unload_id=@parentID";
                            }

                            OleDbDataReader dr = cmd.ExecuteReader();
                            thisList.Clear();
                            while (dr.Read())
                            {
                                VesselEffort item = new VesselEffort();
                                item.Parent = vu;
                                item.PK = (int)dr["effort_row_id"];
                                item.VesselUnloadID = (int)dr["v_unload_id"];
                                item.EffortSpecID = (int)dr["effort_spec_id"];
                                item.EffortValueNumeric = string.IsNullOrEmpty(dr["effort_value_numeric"].ToString()) ? null : (double?)dr["effort_value_numeric"];
                                item.EffortValueText = dr["effort_value_text"].ToString();
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
        private bool AddToMySQL(VesselEffort item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@unload_id", MySqlDbType.Int32).Value = item.VesselUnloadID;
                    update.Parameters.Add("@effort_id", MySqlDbType.Int32).Value = item.EffortSpecID;
                    if (item.EffortValueNumeric == null)
                    {
                        update.Parameters.Add("@numeric_value", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@numeric_value", MySqlDbType.Double).Value = item.EffortValueNumeric;
                    }
                    if (item.EffortValueText == null)
                    {
                        update.Parameters.Add("@text_value", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@text_value", MySqlDbType.VarChar).Value = item.EffortValueText;
                    }
                    update.CommandText = @"Insert into dbo_vessel_effort(effort_row_id, v_unload_id,effort_spec_id, effort_value_numeric,effort_value_text) 
                                        Values (@id,@unload_id,@effort_id,@numeric_value,@text_value)";
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
        public bool Add(VesselEffort item)
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

                    var sql = "Insert into dbo_vessel_effort(effort_row_id, v_unload_id,effort_spec_id, effort_value_numeric,effort_value_text) Values (?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                            if (item.Parent != null)
                            {
                                   update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                            }
                            else
                            {
                                update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.VesselUnloadID;
                            }
                            update.Parameters.Add("@effort_id", OleDbType.Integer).Value = item.EffortSpecID;
                            if (item.EffortValueNumeric == null)
                            {
                                update.Parameters.Add("@numeric_value", OleDbType.Double).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@numeric_value", OleDbType.Double).Value = item.EffortValueNumeric;
                            }
                            if (item.EffortValueText == null)
                            {
                                update.Parameters.Add("@text_value", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@text_value", OleDbType.VarChar).Value = item.EffortValueText;
                            }


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
        private bool UpdateMySQL(VesselEffort item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@unload_id", MySqlDbType.Int32).Value = item.VesselUnloadID;
                    update.Parameters.Add("@effort_id", MySqlDbType.Int32).Value = item.EffortSpecID;
                    if (item.EffortValueNumeric == null)
                    {
                        update.Parameters.Add("@numeric_value", MySqlDbType.Double).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@numeric_value", MySqlDbType.Double).Value = item.EffortValueNumeric;
                    }
                    update.Parameters.Add("@text_value", MySqlDbType.VarChar).Value = item.EffortValueText;
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = item.PK;

                    update.CommandText = @"Update dbo_vessel_effort set
                                            v_unload_id=@unload_id,
                                            effort_spec_id = @effort_id,
                                            effort_value_numeric = @numeric_value,
                                            effort_value_text = @text_value
                                            WHERE effort_row_id = @id";

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
        public bool Update(VesselEffort item)
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
                        update.Parameters.Add("@unload_id", OleDbType.Integer).Value = item.Parent.PK;
                        update.Parameters.Add("@effort_id", OleDbType.Integer).Value = item.EffortSpecID;
                        if (item.EffortValueNumeric == null)
                        {
                            update.Parameters.Add("@numeric_value", OleDbType.Double).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@numeric_value", OleDbType.Double).Value = item.EffortValueNumeric;
                        }
                        if (item.EffortValueText == null)
                        {
                               update.Parameters.Add("@text_value", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@text_value", OleDbType.VarChar).Value = item.EffortValueText;
                        }
                        update.Parameters.Add("@vu_gear_id", OleDbType.Integer).Value = DBNull.Value;
                        update.Parameters.Add("@id", OleDbType.Integer).Value = item.PK;
                        update.CommandText = @"Update dbo_vessel_effort set
                                            v_unload_id=@unload_id,
                                            effort_spec_id = @effort_id,
                                            effort_value_numeric = @numeric_value,
                                            effort_value_text = @text_value,
                                            vessel_unload_fishing_gear_id = @vu_gear_id
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
        private bool DeleteMySQL(int id)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    update.CommandText = "Delete  from dbo_vessel_effort where effort_row_id=@id";
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
    }
}
