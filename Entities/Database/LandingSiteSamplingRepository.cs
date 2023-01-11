using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;

namespace NSAP_ODK.Entities.Database
{
    class LandingSiteSamplingRepository
    {
        private static int _maxRecordNumber;
        private bool _addHasOperationColumn = false;
        //private string _dateFormat = "MMM-dd-yyyy";
        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

        public static bool UpdateColumns()
        {
            bool proceed = false;
            //if (TotalWtSpRepository.CheckForTWSPTable() && GearAtLandingSiteDaysPerMonthRepository.CheckForGearLandingSiteTable())
            if (TotalWtSpRepository.CheckForTWSPTable() && VesselUnloadRepository.CheckForWtValidationTable())// && GearAtLandingSiteDaysPerMonthRepository.CheckForGearLandingSiteTable())
            {
                var cols = CreateTablesInAccess.GetColumnNames("dbo_LC_FG_sample_day");
                if (cols.Contains("has_fishing_operation") || UpdateTableDefinition("has_fishing_operation") &&
                 UpdateHasFishingOperationField())
                {
                    cols = CreateTablesInAccess.GetColumnNames("dbo_gear_unload");
                    if (cols.Contains("sp_twsp_count") || GearUnloadRepository.AddFieldToTable("sp_twsp_count"))
                    {
                        if (cols.Contains("sector") || GearUnloadRepository.AddFieldToTable("sector"))
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_catch");

                            if (cols.Contains("weighing_unit") || VesselCatchRepository.UpdateTableDefinition("weighing_unit"))
                            {
                                if (cols.Contains("from_total_catch") || VesselCatchRepository.UpdateTableDefinition("from_total_catch"))
                                    proceed = true;
                            }

                            if(proceed)
                            {
                                cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_unload_1");
                                proceed = cols.Contains("ref_no") || VesselUnloadRepository.AddFieldToTable1("ref_no");
                                if(proceed)
                                {
                                    proceed = cols.Contains("is_catch_sold") || VesselUnloadRepository.AddFieldToTable1("is_catch_sold");
                                }
                            }


                            //check for pricing columns in catch composition table
                            if(proceed)
                            {
                                cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_catch");
                                if( cols.Contains("price_of_species") || VesselCatchRepository.AddFieldToTable("price_of_species"))
                                {
                                    proceed = cols.Contains("price_unit") || VesselCatchRepository.AddFieldToTable("price_unit");
                                }
                            }

                            //check and and sex column for length table
                            if (proceed)
                            {
                                cols = CreateTablesInAccess.GetColumnNames("dbo_catch_len");
                                proceed = cols.Contains("sex") || CatchLengthRepository.AddFieldToTable("sex");
                            }

                            if (proceed)
                            {
                                cols = CreateTablesInAccess.GetColumnNames("dbo_catch_len_freq");
                                proceed = cols.Contains("sex") || CatchLenFreqRepository.AddFieldToTable("sex");
                            }

                            if (proceed)
                            {
                                cols = CreateTablesInAccess.GetColumnNames("dbo_catch_len_wt");
                                proceed = cols.Contains("sex") || CatchLenWeightRepository.AddFieldToTable("sex");
                            }



                        }
                    }
                }
            }
            return proceed;
        }
        public LandingSiteSamplingRepository()
        {


            LandingSiteSamplings = getLandingSiteSamplings();

            //if(LandingSiteSamplings.Count==0 && _addHasOperationColumn && UpdateTableDefinition("has_fishing_operation") && UpdateHasFishingOperationField())
            //{

            //    LandingSiteSamplings = getLandingSiteSamplings();
            //}
        }

        public int MaxRecordNumber()
        {
            //return NSAPEntities.SummaryItemViewModel.GetLandingSiteSamplingMaxRecordNumber();
            return MaxRecordNumber_from_db();
        }
        public static int MaxRecordNumber_from_db()
        {
            int max_rec_no = 0;
            if (Global.Settings.UsemySQL)
            {
                using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "SELECT Max(unload_day_id) AS max_id FROM dbo_lc_fg_sample_day";
                        max_rec_no = (int)cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
                {
                    conn.Open();
                    const string sql = "SELECT Max(unload_day_id) AS max_id FROM dbo_LC_FG_sample_day";
                    using (OleDbCommand getMax = new OleDbCommand(sql, conn))
                    {
                        max_rec_no = (int)getMax.ExecuteScalar();
                    }
                }
            }
            return max_rec_no;
        }

        private List<LandingSiteSampling> getFromMySQL()
        {
            List<LandingSiteSampling> thisList = new List<LandingSiteSampling>();
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = @"SELECT lc.*, 
                                        lc1.datetime_submitted, 
                                        lc1.user_name, 
                                        lc1.device_id, 
                                        lc1.xform_identifier, 
                                        lc1.date_added, 
                                        lc1.from_excel_download, 
                                        lc1.form_version, 
                                        lc1.row_id, 
                                        lc1.enumerator_id, 
                                        lc1.enumerator_text
                                        FROM dbo_LC_FG_sample_day As lc
                                            LEFT JOIN dbo_lc_fg_sample_day_1 AS lc1
                                            ON lc.unload_day_id = lc1.unload_day_id";

                    MySqlDataReader dr = cmd.ExecuteReader();
                    try
                    {
                        while (dr.Read())
                        {
                            LandingSiteSampling item = new LandingSiteSampling();
                            item.PK = (int)dr["unload_day_id"];
                            item.NSAPRegionID = dr["region_id"].ToString();
                            item.SamplingDate = (DateTime)dr["sdate"];
                            //item.LandingSiteID = string.IsNullOrEmpty( dr["land_ctr_id"].ToString())?null:(int?)dr["land_ctr_id"];
                            item.LandingSiteID = dr["land_ctr_id"] == DBNull.Value ? null : (int?)dr["land_ctr_id"];
                            item.FishingGroundID = dr["ground_id"].ToString();
                            item.Remarks = dr["remarks"].ToString();
                            item.IsSamplingDay = Convert.ToBoolean(dr["is_sample_day"]);
                            item.LandingSiteText = dr["land_ctr_text"].ToString();
                            item.HasFishingOperation = (bool)dr["has_fishing_operation"];
                            item.FMAID = (int)dr["fma"];
                            item.DateSubmitted = dr["datetime_submitted"] == DBNull.Value ? null : (DateTime?)dr["datetime_submitted"];
                            item.UserName = dr["user_name"].ToString();
                            item.DeviceID = dr["device_id"].ToString();
                            item.XFormIdentifier = dr["xform_identifier"].ToString();
                            item.DateAdded = dr["date_added"] == DBNull.Value ? null : (DateTime?)dr["date_added"];
                            item.FromExcelDownload = dr["from_excel_download"] == DBNull.Value ? false : (bool)dr["from_excel_download"];
                            item.FormVersion = dr["form_version"].ToString();
                            item.RowID = dr["row_id"].ToString();
                            item.EnumeratorID = dr["enumerator_id"] == DBNull.Value ? null : (int?)int.Parse(dr["enumerator_id"].ToString());
                            item.EnumeratorText = dr["enumerator_text"].ToString();
                            item.GearUnloadViewModel = new GearUnloadViewModel(item);
                            thisList.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return thisList;
        }
        private List<LandingSiteSampling> getLandingSiteSamplings()
        {
            List<LandingSiteSampling> thisList = new List<LandingSiteSampling>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL();
            }
            else
            {
                var dt = new DataTable();
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        try
                        {
                            conection.Open();
                            cmd.CommandText = @"SELECT dbo_LC_FG_sample_day.*, 
                                        dbo_LC_FG_sample_day_1.datetime_submitted, 
                                        dbo_LC_FG_sample_day_1.user_name, 
                                        dbo_LC_FG_sample_day_1.device_id, 
                                        dbo_LC_FG_sample_day_1.XFormIdentifier, 
                                        dbo_LC_FG_sample_day_1.DateAdded, 
                                        dbo_LC_FG_sample_day_1.FromExcelDownload, 
                                        dbo_LC_FG_sample_day_1.form_version, 
                                        dbo_LC_FG_sample_day_1.RowID, 
                                        dbo_LC_FG_sample_day_1.EnumeratorID, 
                                        dbo_LC_FG_sample_day_1.EnumeratorText
                                        FROM dbo_LC_FG_sample_day 
                                            LEFT JOIN dbo_LC_FG_sample_day_1 
                                            ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id";




                            thisList.Clear();
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                LandingSiteSampling item = new LandingSiteSampling();
                                item.PK = (int)dr["unload_day_id"];
                                item.NSAPRegionID = dr["region_id"].ToString();
                                item.SamplingDate = (DateTime)dr["sdate"];
                                item.LandingSiteID = dr["land_ctr_id"] == DBNull.Value ? null : (int?)dr["land_ctr_id"];
                                item.FishingGroundID = dr["ground_id"].ToString();
                                item.Remarks = dr["remarks"].ToString();
                                item.IsSamplingDay = (bool)dr["sampleday"];
                                item.LandingSiteText = dr["land_ctr_text"].ToString();
                                item.HasFishingOperation = (bool)dr["has_fishing_operation"];
                                item.FMAID = (int)dr["fma"];
                                item.DateSubmitted = dr["datetime_submitted"] == DBNull.Value ? null : (DateTime?)dr["datetime_submitted"];
                                item.UserName = dr["user_name"].ToString();
                                item.DeviceID = dr["device_id"].ToString();
                                item.XFormIdentifier = dr["XFormIdentifier"].ToString();
                                item.DateAdded = dr["DateAdded"] == DBNull.Value ? null : (DateTime?)dr["DateAdded"];
                                item.FromExcelDownload = dr["FromExcelDownload"] == DBNull.Value ? false : (bool)dr["FromExcelDownload"];
                                item.FormVersion = dr["form_version"].ToString();
                                item.RowID = dr["RowID"].ToString();
                                item.EnumeratorID = dr["EnumeratorID"] == DBNull.Value ? null : (int?)int.Parse(dr["EnumeratorID"].ToString());
                                item.EnumeratorText = dr["EnumeratorText"].ToString();
                                item.GearUnloadViewModel = new GearUnloadViewModel(item);
                                thisList.Add(item);
                            }

                        }
                        catch (OleDbException olx)
                        {

                        }
                        catch (Exception ex)
                        {
                            switch (ex.Message)
                            {
                                case "has_fishing_operation":
                                    _addHasOperationColumn = true;
                                    //conection.Close();
                                    //if (UpdateTableDefinition(ex.Message) && UpdateHasFishingOperationField())
                                    //{
                                    //    //return getLandingSiteSamplings();
                                    //    //LandingSiteSamplings = getLandingSiteSamplings();
                                    //    _readAgain = true;
                                    //    return null;
                                    //}
                                    break;
                                default:
                                    Logger.Log(ex);
                                    break;

                            }

                        }
                    }
                }
            }
            return thisList;
        }

        private static bool UpdateHasFishingOperationField()
        {
            bool success = false;
            string sql = $"Update dbo_LC_FG_sample_day SET has_fishing_operation=-1";
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                try
                {
                    success = cmd.ExecuteNonQuery() > 0;

                }
                catch (Exception ex)
                {

                }
                cmd.Connection.Close();
                conn.Close();
            }
            return success;

        }
        private static bool UpdateTableDefinition(string colName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "";
                switch (colName)
                {
                    case "has_fishing_operation":

                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day ADD COLUMN {colName} YESNO";
                        break;
                    case "NumberOfFishers":
                        //sql = $@"ALTER TABLE dbo_vessel_unload_1 ADD COLUMN {colName} INTEGER DEFAULT NULL";
                        break;
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        success = true;

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
        private bool AddToMySQL(LandingSiteSampling item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.Add("@nsap_region", MySqlDbType.VarChar).Value = item.NSAPRegionID;
                    update.Parameters.Add("@sampling_date", MySqlDbType.Date).Value = item.SamplingDate;

                    if (item.LandingSiteID == null)
                    {
                        update.Parameters.Add("@landing_site_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@landing_site_id", MySqlDbType.Int32).Value = item.LandingSiteID;
                    }

                    update.Parameters.Add("@fg", MySqlDbType.VarChar).Value = item.FishingGroundID;
                    if (item.Remarks == null)
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = "";
                    }
                    else
                    {
                        update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = item.Remarks;
                    }
                    update.Parameters.AddWithValue("@is_sampling_day", item.IsSamplingDay);

                    if (item.LandingSiteText == null)
                    {
                        update.Parameters.Add("@landing_site_text", MySqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@landing_site_text", MySqlDbType.VarChar).Value = item.LandingSiteText;
                    }
                    update.Parameters.Add("@fma_id", MySqlDbType.Int32).Value = item.FMAID;
                    update.Parameters.AddWithValue("@has_operations", item.HasFishingOperation);

                    update.CommandText = @"Insert into dbo_lc_fg_sample_day(
                                unload_day_id, region_id,sdate, land_ctr_id,ground_id,
                                remarks,is_sample_day,land_ctr_text,fma,has_fishing_operation)
                                Values (@pk,@nsap_region,@sampling_date,@landing_site_id,@fg,@remarks,@is_sampling_day,@landing_site_text,@fma_id,@has_operations)";
                    try
                    {
                        conn.Open();
                        if (update.ExecuteNonQuery() > 0)
                        {
                            using (var update1 = conn.CreateCommand())
                            {
                                update1.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;

                                if (item.DateSubmitted == null)
                                {
                                    update1.Parameters.Add("@date_submitted", MySqlDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@date_submitted", MySqlDbType.Date).Value = item.DateSubmitted;
                                }

                                if (item.UserName == null)
                                {
                                    update1.Parameters.Add("@device_user", MySqlDbType.VarChar).Value = "";
                                }
                                else
                                {
                                    update1.Parameters.Add("@device_user", MySqlDbType.VarChar).Value = item.UserName;
                                }

                                if (item.DeviceID == null)
                                {
                                    update1.Parameters.Add("@device_id", MySqlDbType.VarChar).Value = "";
                                }
                                else
                                {
                                    update1.Parameters.Add("@device_id", MySqlDbType.VarChar).Value = item.DeviceID;
                                }

                                if (item.XFormIdentifier == null)
                                {
                                    update1.Parameters.Add("@xform_id", MySqlDbType.VarChar).Value = "";
                                }
                                else
                                {
                                    update1.Parameters.Add("@xform_id", MySqlDbType.VarChar).Value = item.XFormIdentifier;
                                }

                                //update1.Parameters.Add("@date_added", MySqlDbType.VarChar).Value = dateAdded;

                                if (item.DateAdded == null)
                                {
                                    update1.Parameters.Add("@date_added", MySqlDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@date_added", MySqlDbType.Date).Value = item.DateAdded;
                                }

                                update1.Parameters.AddWithValue("@fromExcel", item.FromExcelDownload);

                                if (item.FormVersion == null)
                                {
                                    update1.Parameters.Add("@form_version", MySqlDbType.VarChar).Value = "";
                                }
                                else
                                {
                                    update1.Parameters.Add("@form_version", MySqlDbType.VarChar).Value = item.FormVersion;
                                }

                                if (item.RowID == null || item.RowID.Length == 0)
                                {
                                    update1.Parameters.Add("@row_id", MySqlDbType.Guid).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@row_id", MySqlDbType.Guid).Value = Guid.Parse(item.RowID);
                                }

                                if (item.EnumeratorID == null)
                                {
                                    update1.Parameters.Add("@enum_id", MySqlDbType.Int32).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@enum_id", MySqlDbType.Int32).Value = item.EnumeratorID;
                                }


                                if (item.EnumeratorText == null)
                                {
                                    update1.Parameters.Add("@enum_text", MySqlDbType.VarChar).Value = "";
                                }
                                else
                                {
                                    update1.Parameters.Add("@enum_text", MySqlDbType.VarChar).Value = item.EnumeratorText;
                                }
                                update1.CommandText = @"Insert into dbo_lc_fg_sample_day_1  (
                                                        unload_day_id,
                                                        datetime_submitted,
                                                        user_name,
                                                        device_id,
                                                        xform_identifier,
                                                        date_added,
                                                        from_excel_download,
                                                        form_version,
                                                        row_id,
                                                        enumerator_id,
                                                        enumerator_text
                                                    ) Values (@pk,@date_submitted,@device_user,@device_id,@xform_id,@date_added,@fromExcel,@form_version,@row_id,@enum_id,@enum_text)";

                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
                                }
                                catch (MySqlException msex)
                                {
                                    Logger.Log(msex.Message);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);

                                }
                            }
                        }
                    }
                    catch (MySqlException msex)
                    {
                        Logger.Log(msex.Message);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return success;
        }
        public bool Add(LandingSiteSampling item)
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

                    var sql = @"Insert into dbo_LC_FG_sample_day(
                                unload_day_id, region_id,sdate, land_ctr_id,ground_id,
                                remarks,sampleday,land_ctr_text,fma,has_fishing_operation)
                           Values (?,?,?,?,?,?,?,?,?,?)";
                    using (OleDbCommand update = new OleDbCommand(sql, conn))
                    {
                        try
                        {
                            update.Parameters.Clear();
                            update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;
                            update.Parameters.Add("@nsap_region", OleDbType.VarChar).Value = item.NSAPRegionID;
                            update.Parameters.Add("@sampling_date", OleDbType.Date).Value = item.SamplingDate;

                            if (item.LandingSiteID == null)
                            {
                                update.Parameters.Add("@landing_site_id", OleDbType.Integer).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@landing_site_id", OleDbType.Integer).Value = item.LandingSiteID;
                            }

                            update.Parameters.Add("@fg", OleDbType.VarChar).Value = item.FishingGroundID;
                            if (item.Remarks == null)
                            {
                                update.Parameters.Add("@remarks", OleDbType.VarChar).Value = "";
                            }
                            else
                            {
                                update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                            }
                            update.Parameters.Add("@is_sampling_day", OleDbType.Boolean).Value = item.IsSamplingDay;

                            if (item.LandingSiteText == null)
                            {
                                update.Parameters.Add("@landing_site_text", OleDbType.VarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                update.Parameters.Add("@landing_site_text", OleDbType.VarChar).Value = item.LandingSiteText;
                            }
                            update.Parameters.Add("@fma_id", OleDbType.Integer).Value = item.FMAID;
                            update.Parameters.Add("@has_operation", OleDbType.Boolean).Value = item.HasFishingOperation;

                            success = update.ExecuteNonQuery() > 0;
                            if (success)
                            {
                                string dateSubmitted = item.DateSubmitted == null ? "null" : $@"'{item.DateSubmitted.ToString()}'";
                                string dateAdded = item.DateAdded == null ? "null" : $@"'{item.DateAdded.ToString()}'";


                                sql = @"Insert into dbo_LC_FG_sample_day_1  (
                                        unload_day_id,
                                        datetime_submitted,
                                        user_name,
                                        device_id,
                                        XFormIdentifier,
                                        DateAdded,
                                        FromExcelDownload,
                                        form_version,
                                        RowID,
                                        EnumeratorID,
                                        EnumeratorText
                                    ) Values (?,?,?,?,?,?,?,?,?,?,?)";


                                using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                                {
                                    update1.Parameters.Clear();
                                    update1.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;

                                    if (item.DateSubmitted == null)
                                    {
                                        update1.Parameters.Add("@date_submitted", OleDbType.Date).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@date_submitted", OleDbType.Date).Value = item.DateSubmitted;
                                    }

                                    if (item.UserName == null)
                                    {
                                        update1.Parameters.Add("@device_user", OleDbType.VarChar).Value = "";
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@device_user", OleDbType.VarChar).Value = item.UserName;
                                    }

                                    if (item.DeviceID == null)
                                    {
                                        update1.Parameters.Add("@device_id", OleDbType.VarChar).Value = "";
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@device_id", OleDbType.VarChar).Value = item.DeviceID;
                                    }

                                    if (item.XFormIdentifier == null)
                                    {
                                        update1.Parameters.Add("@xform_id", OleDbType.VarChar).Value = "";
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@xform_id", OleDbType.VarChar).Value = item.XFormIdentifier;
                                    }

                                    //update1.Parameters.Add("@date_added", OleDbType.VarChar).Value = dateAdded;

                                    if (item.DateAdded == null)
                                    {
                                        update1.Parameters.Add("@date_added", OleDbType.Date).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@date_added", OleDbType.Date).Value = item.DateAdded;
                                    }

                                    update1.Parameters.Add("@fromExcel", OleDbType.Boolean).Value = item.FromExcelDownload;

                                    if (item.FormVersion == null)
                                    {
                                        update1.Parameters.Add("@form_version", OleDbType.VarChar).Value = "";
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@form_version", OleDbType.VarChar).Value = item.FormVersion;
                                    }

                                    if (item.RowID == null)
                                    {
                                        update1.Parameters.Add("@row_id", OleDbType.Guid).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@row_id", OleDbType.Guid).Value = Guid.Parse(item.RowID);
                                    }

                                    if (item.EnumeratorID == null)
                                    {
                                        update1.Parameters.Add("@enum_id", OleDbType.Integer).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@enum_id", OleDbType.Integer).Value = item.EnumeratorID;
                                    }


                                    if (item.EnumeratorText == null)
                                    {
                                        update1.Parameters.Add("@enum_text", OleDbType.VarChar).Value = "";
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@enum_text", OleDbType.VarChar).Value = item.EnumeratorText;
                                    }

                                    try
                                    {
                                        success = update1.ExecuteNonQuery() > 0;
                                    }
                                    catch (OleDbException odbex)
                                    {
                                        Logger.Log(odbex);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }
                                }

                            }
                        }
                        catch (OleDbException odbex)
                        {
                            switch (odbex.ErrorCode)
                            {
                                case -2147467259:
                                    Logger.Log(odbex);
                                    break;
                                default:
                                    Logger.Log(odbex);
                                    success = false;
                                    break;
                            }

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

        private bool UpdateMySQL(LandingSiteSampling item)
        {
            bool success = false;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var update = conn.CreateCommand())
                {
                    update.Parameters.Add("@region_id", MySqlDbType.Int32).Value = item.NSAPRegionID;
                    update.Parameters.Add("@sdate", MySqlDbType.Date).Value = item.SamplingDate;
                    if (item.LandingSiteID == null)
                    {
                        update.Parameters.Add("@landing_site_id", MySqlDbType.Int32).Value = DBNull.Value;
                    }
                    else
                    {
                        update.Parameters.Add("@landing_site_id", MySqlDbType.Int32).Value = item.LandingSiteID;
                    }
                    update.Parameters.Add("@ground_id", MySqlDbType.VarChar).Value = item.FishingGroundID;
                    update.Parameters.Add("@remarks", MySqlDbType.VarChar).Value = item.Remarks;
                    update.Parameters.AddWithValue("@issampling_day", item.IsSamplingDay);
                    update.Parameters.Add("landing_site_text", MySqlDbType.VarChar).Value = item.LandingSiteText;
                    update.Parameters.Add("@fma", MySqlDbType.Int32).Value = item.FMAID;
                    update.Parameters.Add("@pk", MySqlDbType.Int32).Value = item.PK;
                    update.Parameters.AddWithValue("@has_operations", item.HasFishingOperation);

                    update.CommandText = @"Update dbo_lc_fg_sample_day set
                                        region_id=@region_id,
                                        sdate = @sdate,
                                        land_ctr_id = @landing_site_id,
                                        ground_id = @ground_id,
                                        remarks = @remarks,
                                        is_sample_day = @issampling_day,
                                        land_ctr_text = @landing_site_text,
                                        fma = @fma,     
                                        has_fishing_operation = @has_operations
                                    WHERE unload_day_id = @pk";

                    try
                    {
                        conn.Open();
                        success = update.ExecuteNonQuery() > 0;
                        if (success && (item.XFormIdentifier != null && item.XFormIdentifier.Length > 0) || (item.Remarks != null && item.Remarks.Contains("orphaned")))
                        {

                            using (var update1 = conn.CreateCommand())
                            {
                                if (item.DateSubmitted == null)
                                {
                                    update.Parameters.Add("@submitted", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    update.Parameters.Add("@submitted", MySqlDbType.DateTime).Value = item.DateSubmitted;
                                }
                                update.Parameters.Add("@user", MySqlDbType.VarChar).Value = item.UserName;
                                update.Parameters.Add("@device_id", MySqlDbType.VarChar).Value = item.DeviceID;
                                update.Parameters.Add("@xform_id", MySqlDbType.VarChar).Value = item.XFormIdentifier;
                                if (item.DateSubmitted == null)
                                {
                                    update.Parameters.Add("@added", MySqlDbType.DateTime).Value = DBNull.Value;
                                }
                                else
                                {
                                    update.Parameters.Add("@added", MySqlDbType.DateTime).Value = item.DateAdded;
                                }
                                update.Parameters.AddWithValue("@from_excel", item.FromExcelDownload);
                                update.Parameters.Add("@form_version", MySqlDbType.VarChar).Value = item.FormVersion;
                                update.Parameters.Add("@row_id", MySqlDbType.VarChar).Value = item.RowID;
                                if (item.EnumeratorID == null)
                                {
                                    update.Parameters.Add("@enum_id", MySqlDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    update.Parameters.Add("@enum_id", MySqlDbType.VarChar).Value = item.EnumeratorID;
                                }
                                update.Parameters.Add("@enum_text", MySqlDbType.VarChar).Value = item.EnumeratorText;
                                update.Parameters.Add("@pk1", MySqlDbType.Int32).Value = item.PK;

                                update.CommandText = @"Update dbo_lc_fg_sample_day_1 set
                                                    datetime_submitted = @submitted,
                                                    user_name = @user,
                                                    device_id = @device_id,
                                                    xform_identifier = @xform_id,
                                                    date_added = @added,
                                                    from_excel_download = @from_excel,
                                                    form_version = @form_version,
                                                    row_id = @row_id,
                                                    enumerator_id = @enum_id,
                                                    enumerator_text = @enum_text
                                                 WHERE unload_day_id = @pk1";

                                success = false;

                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
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
        public bool Update(LandingSiteSampling item)
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
                        update.Parameters.Add("@region_id", OleDbType.VarChar).Value = item.NSAPRegionID;
                        update.Parameters.Add("@sdate", OleDbType.Date).Value = item.SamplingDate;
                        if (item.LandingSiteID == null)
                        {
                            update.Parameters.Add("@landing_site_id", OleDbType.Integer).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@landing_site_id", OleDbType.Integer).Value = item.LandingSiteID;
                        }
                        update.Parameters.Add("@ground_id", OleDbType.VarChar).Value = item.FishingGroundID;
                        if (string.IsNullOrEmpty(item.Remarks))
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@remarks", OleDbType.VarChar).Value = item.Remarks;
                        }
                        update.Parameters.Add("@issampling_day", OleDbType.Boolean).Value = item.IsSamplingDay;
                        if (string.IsNullOrEmpty(item.LandingSiteText))
                        {
                            update.Parameters.Add("@landing_site_text", OleDbType.VarChar).Value = DBNull.Value;
                        }
                        else
                        {
                            update.Parameters.Add("@landing_site_text", OleDbType.VarChar).Value = item.LandingSiteText;
                        }
                        update.Parameters.Add("@fma", OleDbType.Integer).Value = item.FMAID;
                        update.Parameters.Add("@has_operations", OleDbType.Boolean).Value = item.HasFishingOperation;
                        update.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;


                        update.CommandText = @"Update dbo_LC_FG_sample_day set
                                        region_id=@region_id,
                                        sdate = @sdate,
                                        land_ctr_id = @land_site_id,
                                        ground_id = @ground_id,
                                        remarks = @remarks,
                                        sampleday = @issampling_day,
                                        land_ctr_text = @landing_site_text,
                                        fma = @fma,
                                        has_fishing_operation = @has_operations
                                    WHERE unload_day_id = @pk";
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

                        if (success && (item.XFormIdentifier != null && item.XFormIdentifier.Length > 0) || (item.Remarks != null && item.Remarks.Contains("orphaned")))
                        {


                            using (OleDbCommand update1 = conn.CreateCommand())
                            {
                                if (item.DateSubmitted == null)
                                {
                                    update1.Parameters.Add("@submitted", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@submitted", OleDbType.Date).Value = item.DateSubmitted;
                                }
                                update1.Parameters.Add("@user", OleDbType.VarChar).Value = item.UserName;
                                update1.Parameters.Add("@device_id", OleDbType.VarChar).Value = item.DeviceID;
                                update1.Parameters.Add("@xform_id", OleDbType.VarChar).Value = item.XFormIdentifier;
                                if (item.DateSubmitted == null)
                                {
                                    update1.Parameters.Add("@added", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@added", OleDbType.Date).Value = item.DateAdded;
                                }
                                update1.Parameters.Add("@from_excel", OleDbType.Boolean).Value = item.FromExcelDownload;
                                update1.Parameters.Add("@form_version", OleDbType.VarChar).Value = item.FormVersion;
                                update1.Parameters.Add("@row_id", OleDbType.VarChar).Value = item.RowID;
                                if (item.EnumeratorID == null)
                                {
                                    update1.Parameters.Add("@enum_id", OleDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@enum_id", OleDbType.VarChar).Value = item.EnumeratorID;
                                }
                                if (string.IsNullOrEmpty(item.EnumeratorText))
                                {
                                    update1.Parameters.Add("@enum_text", OleDbType.VarChar).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@enum_text", OleDbType.VarChar).Value = item.EnumeratorText;
                                }
                                update1.Parameters.Add("@pk", OleDbType.Integer).Value = item.PK;

                                update1.CommandText = @"Update dbo_LC_FG_sample_day_1 set
                                                    datetime_submitted = @submitted,
                                                    user_name = @user,
                                                    device_id = @device_id,
                                                    XFormIdentifier = @xform_id,
                                                    DateAdded = @added,
                                                    FromExcelDownload = @from_excel,
                                                    form_version = @form_version,
                                                    RowID = @row_id,
                                                    EnumeratorID = @enum_id,
                                                    EnumeratorText = @enum_text
                                                 WHERE unload_day_id = @pk";

                                success = false;

                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
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
                var sql = "Delete * from dbo_LC_FG_sample_day_1";

                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        update.ExecuteNonQuery();
                        success = true;
                        if (success)
                        {
                            sql = "Delete * from dbo_LC_FG_sample_day";
                            using (OleDbCommand update1 = new OleDbCommand(sql, conn))
                            {
                                try
                                {
                                    update1.ExecuteNonQuery();
                                    success = true;
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
                    update.CommandText = "Delete  from dbo_lc_fg_sample_day_1 where unload_day_id=@id";
                    try
                    {
                        conn.Open();
                        if (update.ExecuteNonQuery() > 0)
                        {
                            using (var update1 = conn.CreateCommand())
                            {
                                update1.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                                update1.CommandText = "Delete  from dbo_lc_fg_sample_day where unload_day_id=@id";
                                try
                                {
                                    success = update1.ExecuteNonQuery() > 0;
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
                        update.CommandText = "Delete * from dbo_LC_FG_sample_day_1 where unload_day_id=@id";
                        try
                        {
                            if (update.ExecuteNonQuery() > 0)
                            {
                                using (OleDbCommand update1 = conn.CreateCommand())
                                {
                                    update1.Parameters.Add("@id", OleDbType.Integer).Value = id;
                                    update1.CommandText = "Delete * from dbo_LC_FG_sample_day where unload_day_id=@id";
                                    try
                                    {
                                        success = update1.ExecuteNonQuery() > 0;
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
