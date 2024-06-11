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
using System.ComponentModel;

namespace NSAP_ODK.Entities.Database
{
    class LandingSiteSamplingRepository
    {
        //private static int _maxRecordNumber;
        //private bool _addHasOperationColumn = false;
        //private string _dateFormat = "MMM-dd-yyyy";

        //public static Task<bool>UpdateColumnsAsync()
        //{
        //    return Task.Run(()=>UpdateColumns());
        //}
        public static async void UpdateColumns()
        {
            bool proceed = false;
            int fieldSize = 0;
            if (TotalWtSpRepository.CheckForTWSPTable() &&
                VesselUnloadRepository.CheckForWtValidationTable() &&
                LandingSiteSamplingSubmissionRepository.CheckForLSS_SubmissionIDTable() &&
                UnmatchedFieldsFromJSONFileRepository.CheckTableExist() &&
                CarrierLandingRepository.CheckTableExist() &&
                CarrierBoatLanding_FishingGroundRepository.CheckTableExist() &&
                CatcherBoatOperationRepository.CheckTableExist() &&
                VesselUnload_FishingGearRepository.CheckTableExists() &&
                LandingSite_FishingVessel_Repository.CheckTableExists())
            //LandingSiteFishingGroundRepository.CheckForLandingSiteFishingGroundTable()
            //  )
            {
                var cols = CreateTablesInAccess.GetColumnNames("dbo_LC_FG_sample_day");
                if (cols.Contains("type_of_sampling") || UpdateTableDefinition("type_of_sampling"))
                {
                    //proceed = true;
                    proceed = cols.Contains("has_fishing_operation") || UpdateTableDefinition("has_fishing_operation") &&
                     UpdateHasFishingOperationField();
                }
                if (proceed)
                {
                    cols = CreateTablesInAccess.GetColumnNames("dbo_LC_FG_sample_day_1");
                    if (cols.Contains("is_multivessel") || UpdateTableDefinition("is_multivessel"))
                    {
                        if (cols.Contains("number_landings_sampled") || UpdateTableDefinition("number_landings_sampled"))
                        {
                            if (cols.Contains("number_gear_types_in_landingsite") || UpdateTableDefinition("number_gear_types_in_landingsite"))
                            {
                                if (cols.Contains("number_landings") || UpdateTableDefinition("number_landings"))
                                {
                                    if (cols.Contains("json_filename") || UpdateTableDefinition("json_filename"))
                                    {
                                        if (cols.Contains("can_sample_from_catch_composition") || UpdateTableDefinition("can_sample_from_catch_composition"))
                                        {
                                            if (cols.Contains("submission_id") || UpdateTableDefinition("submission_id"))
                                            {
                                                if (cols.Contains("date_deleted_from_server") || UpdateTableDefinition("date_deleted_from_server"))
                                                {
                                                    if (cols.Contains("count_carrier_landings") || UpdateTableDefinition("count_carrier_landings"))
                                                    {
                                                        proceed = cols.Contains("count_carrier_sampling") || UpdateTableDefinition("count_carrier_sampling");


                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //delete has_fishing_operation field
                    if (cols.Contains("has_fishing_operation"))
                    {
                        DeleteField("has_fishing_operation");
                    }

                    if (proceed)
                    {
                        cols = CreateTablesInAccess.GetColumnNames("gear");
                        if (cols.Contains("GearIsNotUsed") || GearRepository.AddFieldToTable("GearIsNotUsed"))
                        {
                            proceed = cols.Contains("IsUsedInLargeCommercial") || GearRepository.AddFieldToTable("IsUsedInLargeCommercial");
                        }
                    }

                    if (proceed)
                    {
                        cols = CreateTablesInAccess.GetColumnNames("dbo_gear_unload");
                        if (cols.Contains("sp_twsp_count") || GearUnloadRepository.AddFieldToTable("sp_twsp_count"))
                        {
                            if (cols.Contains("sector") || GearUnloadRepository.AddFieldToTable("sector"))
                            {
                                if (cols.Contains("gear_sequence") || GearUnloadRepository.AddFieldToTable("gear_sequence"))
                                {
                                    if (cols.Contains("gear_count_municipal") || GearUnloadRepository.AddFieldToTable("gear_count_municipal"))
                                    {
                                        if (cols.Contains("gear_count_commercial") || GearUnloadRepository.AddFieldToTable("gear_count_commercial"))
                                        {
                                            if (cols.Contains("gear_catch_municipal") || GearUnloadRepository.AddFieldToTable("gear_catch_municipal"))
                                            {
                                                if (cols.Contains("gear_catch_commercial") || GearUnloadRepository.AddFieldToTable("gear_catch_commercial"))
                                                {
                                                    proceed = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (proceed)
                    {
                        cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_catch");

                        if (cols.Contains("weighing_unit") || VesselCatchRepository.UpdateTableDefinition("weighing_unit"))
                        {
                            if (cols.Contains("from_total_catch") || VesselCatchRepository.UpdateTableDefinition("from_total_catch"))
                                if (cols.Contains("gear_code") || VesselCatchRepository.UpdateTableDefinition("gear_code"))
                                    if (cols.Contains("gear_text") || VesselCatchRepository.UpdateTableDefinition("gear_text"))
                                    {
                                        //proceed = cols.Contains("landingsitesampling_id") || VesselCatchRepository.UpdateTableDefinition("landingsitesampling_id"));
                                        proceed = true;
                                    }
                        }

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_unload_1");
                            if (cols.Contains("json_filename"))
                            {
                                fieldSize = VesselUnloadRepository.GetFieldSize("json_filename");
                                if (fieldSize != 255)
                                {
                                    proceed = VesselUnloadRepository.UpdateTableDefinitionEx("json_filename", 255);
                                }
                            }
                            else
                            {
                                proceed = VesselUnloadRepository.UpdateTableDefinitionEx("json_filename");
                            }
                            if (proceed)
                            {
                                if (cols.Contains("sampling_sequence") || VesselUnloadRepository.UpdateTableDefinitionEx("sampling_sequence"))
                                {
                                    if (cols.Contains("is_multigear") || VesselUnloadRepository.UpdateTableDefinitionEx("is_multigear"))
                                    {
                                        if (cols.Contains("count_gear_types") || VesselUnloadRepository.UpdateTableDefinitionEx("count_gear_types"))
                                        {
                                            if (cols.Contains("number_species_catch_composition") || VesselUnloadRepository.UpdateTableDefinitionEx("number_species_catch_composition"))
                                            {
                                                if (cols.Contains("include_effort_indicators") || VesselUnloadRepository.UpdateTableDefinitionEx("include_effort_indicators"))
                                                {

                                                    if (cols.Contains("lss_submisionID") || VesselUnloadRepository.UpdateTableDefinitionEx("lss_submisionID"))
                                                    {
                                                        proceed = cols.Contains("submission_id") || VesselUnloadRepository.UpdateTableDefinitionEx("submission_id");

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (proceed)
                        {
                            //cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_unload_1");


                            proceed = cols.Contains("ref_no") || VesselUnloadRepository.AddFieldToTable1("ref_no");


                            if (proceed)
                            {
                                proceed = cols.Contains("is_catch_sold") || VesselUnloadRepository.AddFieldToTable1("is_catch_sold");
                                if (proceed)
                                {
                                    VesselUnloadRepository.RefNoFieldSize = VesselUnloadRepository.GetFieldSize("ref_no");
                                    if (VesselUnloadRepository.RefNoFieldSize < 150)
                                    {
                                        proceed = VesselUnloadRepository.UpdateTableDefinitionEx("ref_no", 150);
                                        VesselUnloadRepository.RefNoFieldSize = VesselUnloadRepository.GetFieldSize("ref_no");
                                    }
                                }
                                if (proceed)
                                {
                                    if (VesselUnloadRepository.UpdateTableDefinitionEx(relationshipToRemove: "fishingVesseldbo_vessel_unload"))
                                    {
                                        proceed = VesselUnloadRepository.UpdateTableDefinitionEx(relationshipToRemove: "gpsdbo_vessel_unload_1");
                                    }
                                }
                            }
                        }


                        //check for pricing columns in catch composition table
                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_catch");
                            if (cols.Contains("price_of_species") || VesselCatchRepository.AddFieldToTable("price_of_species"))
                            {
                                if (cols.Contains("price_unit") || VesselCatchRepository.AddFieldToTable("price_unit"))
                                {
                                    if (cols.Contains("other_price_unit") || VesselCatchRepository.AddFieldToTable("other_price_unit"))
                                    {
                                        if (cols.Contains("vessel_unload_gear_id") || VesselCatchRepository.AddFieldToTable("vessel_unload_gear_id"))
                                        {
                                            if (cols.Contains("is_catch_sold") || VesselCatchRepository.AddFieldToTable("is_catch_sold"))
                                            {
                                                proceed = cols.Contains("carrierlanding_id") || VesselCatchRepository.AddFieldToTable("carrierlanding_id");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vesselunload_fishinggear");
                            if (cols.Contains("catch_weight") || VesselUnload_FishingGearRepository.AddFieldToTable("catch_weight"))
                            {
                                if (cols.Contains("sample_weight") || VesselUnload_FishingGearRepository.AddFieldToTable("sample_weight"))
                                {
                                    proceed = cols.Contains("species_comp_count") || VesselUnload_FishingGearRepository.AddFieldToTable("species_comp_count");
                                }
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

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_catch_maturity");
                            proceed = cols.Contains("gonadWt") || CatchMaturityRepository.AddFieldToTable("gonadWt");
                        }
                        //if (proceed)
                        //{
                        //    proceed = VesselUnload_FishingGearRepository.CheckTableExists();
                        //}

                        //if (proceed)
                        //{
                        //    proceed = LandingSite_FishingVessel_Repository.CheckTableExists();
                        //}

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_effort");
                            proceed = cols.Contains("vessel_unload_fishing_gear_id") || VesselUnload_Gear_Spec_Repository.UpdateTable();
                        }


                        if (proceed)
                        {
                            //drop multifield index in effort table
                            if (VesselCatchRepository.UpdateTableDefinition(removeIndex: true, indexName: "alt_key"))
                            {
                                proceed = VesselEffortRepository.UpdateTableDefinition(removeMultiFieldIndex: true, indexName: "alt_key");
                            }
                        }

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_json_fields_mismatch");
                            proceed = cols.Contains("json_file_id") || UnmatchedFieldsFromJSONFileRepository.AddFieldToTable("json_file_id");
                        }

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("JSONFIle");
                            proceed = cols.Contains("version") || JSONFileRepository.AddFieldToTable("version");
                        }

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_unload_weight_validation");
                            if (cols.Contains("unload_gear") || await (VesselUnloadRepository.AddFieldToWeightValidationTableAsync("unload_gear")))
                            {
                                if (cols.Contains("row_id") || await VesselUnloadRepository.AddFieldToWeightValidationTableAsync("row_id"))
                                {
                                    proceed = true;
                                }
                            }

                        }
                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("dbo_vessel_unload_stats");
                            {
                                if (cols.Contains("unload_gear") || await VesselUnloadRepository.AddFieldToStatsTableAsync("unload_gear"))
                                {
                                    proceed = cols.Contains("row_id") || await VesselUnloadRepository.AddFieldToStatsTableAsync("row_id");
                                }
                            }
                        }

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("nsapRegion");
                            if (cols.Contains("IsTotalEnumerationOnly") || await NSAPRegionRepository.AddFieldToTableAsync("IsTotalEnumerationOnly"))
                            {
                                proceed = cols.Contains("IsRegularSamplingOnly") || await NSAPRegionRepository.AddFieldToTableAsync("IsRegularSamplingOnly");
                            }
                        }

                        if (proceed)
                        {
                            cols = CreateTablesInAccess.GetColumnNames("landingSite");
                            proceed = cols.Contains("TypeOfSampling") || await LandingSiteRepository.AddFieldToTableAsync("TypeOfSampling");
                        }

                    }


                }
            }




            // return proceed;
        }
        public LandingSiteSampling Create(int lss_id)
        {
            var lss = getLandingSiteSamplings(lss_id).First();

            return lss;
        }

        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

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
                        try
                        {
                            max_rec_no = (int)getMax.ExecuteScalar();
                        }
                        catch
                        {
                            max_rec_no = 0;
                        }
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

        private bool UpdateXFormIDInTable(string xFormID)
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
                        cmd.Parameters.AddWithValue("@id", xFormID);
                        cmd.CommandText = @"UPDATE ((dbo_gear_unload INNER JOIN 
                                                        dbo_LC_FG_sample_day_1 ON 
                                                        dbo_gear_unload.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) INNER JOIN 
                                                        dbo_vessel_unload ON 
                                                        dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN 
                                                        dbo_vessel_unload_1 ON 
                                                        dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id 
                                                        SET dbo_LC_FG_sample_day_1.XFormIdentifier = @id
                                                    WHERE dbo_vessel_unload_1.XFormIdentifier = @id";
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
            }

            return success;
        }

        public static int? GetLandingSiteSamplingID(FishingGround fg, LandingSite ls, DateTime sampling_date)
        {
            int? lss_id = null;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@ls", ls.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg", fg.Code);
                        cmd.Parameters.AddWithValue("@sd", sampling_date);

                        cmd.CommandText = @"SELECT unload_day_id
                                            FROM dbo_LC_FG_sample_day
                                            WHERE land_ctr_id = @ls AND
                                                    ground_id = @fg AND 
                                                    sdate = @sd";

                        try
                        {
                            con.Open();
                            lss_id = (int)cmd.ExecuteScalar();

                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return lss_id;
        }
        private List<LandingSiteSampling> getLandingSiteSamplings(int? lss_id = null)
        {

            List<LandingSiteSampling> thisList = new List<LandingSiteSampling>();
            if (Global.Settings.UsemySQL)
            {
                thisList = getFromMySQL();
            }
            else
            {
                using (var conection = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = conection.CreateCommand())
                    {
                        int loopCount = 0;
                        try
                        {

                            cmd.CommandText = @"SELECT dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                    FROM dbo_LC_FG_sample_day 
                                                    LEFT JOIN dbo_LC_FG_sample_day_1 
                                                    ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id";


                            if (lss_id != null)
                            {
                                cmd.Parameters.AddWithValue("@id", (int)lss_id);
                                cmd.CommandText += $" WHERE dbo_LC_FG_sample_day.unload_day_id = @id";
                            }
                            else if (Global.Filter1 != null)
                            {
                                cmd.Parameters.AddWithValue("@d1", Global.Filter1DateString());
                                if (Global.Filter2 != null)
                                {
                                    cmd.Parameters.AddWithValue("@d2", Global.Filter2DateString());
                                    cmd.CommandText += $" WHERE sDate >= @d1 AND sDate < @d2";
                                }
                                else
                                {
                                    cmd.CommandText += $" WHERE sDate >= @d1";
                                }
                            }
                            else if (!string.IsNullOrEmpty(Global.FilterServerID))
                            {
                                if (UpdateXFormIDInTable(Global.FilterServerID))
                                {
                                    cmd.Parameters.AddWithValue("@srv", Global.FilterServerID);
                                    cmd.CommandText += $" WHERE XFormIdentifier = @srv";
                                }
                            }


                            thisList.Clear();
                            conection.Open();
                            OleDbDataReader dr = cmd.ExecuteReader();

                            while (dr.Read())
                            {
                                if (dr["fma"] != DBNull.Value)
                                {
                                    LandingSiteSampling item = new LandingSiteSampling();
                                    item.PK = (int)dr["dbo_LC_FG_sample_day.unload_day_id"];
                                    item.NSAPRegionID = dr["region_id"].ToString();
                                    item.SamplingDate = (DateTime)dr["sdate"];
                                    item.LandingSiteID = dr["land_ctr_id"] == DBNull.Value ? null : (int?)dr["land_ctr_id"];
                                    item.FishingGroundID = dr["ground_id"].ToString();
                                    item.Remarks = dr["remarks"].ToString();
                                    item.IsSamplingDay = (bool)dr["sampleday"];
                                    item.LandingSiteText = dr["land_ctr_text"].ToString();
                                    item.HasFishingOperation = (bool)dr["has_fishing_operation"];
                                    if (dr["is_multivessel"] == DBNull.Value)
                                    {
                                        item.IsMultiVessel = false;
                                    }
                                    else
                                    {
                                        item.IsMultiVessel = (bool)dr["is_multivessel"];
                                    }
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

                                    if (dr["date_deleted_from_server"] != DBNull.Value)
                                    {
                                        item.DateDeletedFromServer = (DateTime)dr["date_deleted_from_server"];
                                    }

                                    int? submission_id = null;
                                    if (dr["submission_id"] != DBNull.Value)
                                    {
                                        submission_id = (int)dr["submission_id"];
                                    }
                                    item.Submission_id = submission_id;

                                    if (item.IsMultiVessel)
                                    {
                                        if (dr["number_gear_types_in_landingsite"] != DBNull.Value)
                                        {
                                            item.NumberOfGearTypesInLandingSite = (int)dr["number_gear_types_in_landingsite"];
                                        }
                                        if (dr["number_landings_sampled"] != DBNull.Value)
                                        {
                                            item.NumberOfLandingsSampled = (int)dr["number_landings_sampled"];
                                        }
                                        if (dr["number_landings"] != DBNull.Value)
                                        {
                                            item.NumberOfLandings = (int)dr["number_landings"];
                                        }
                                    }
                                    item.GearsInLandingSite = null;//GetGearsInLandingSite(item);
                                    if (dr["can_sample_from_catch_composition"] == DBNull.Value)
                                    {
                                        item.SamplingFromCatchCompositionIsAllowed = false;
                                    }
                                    else
                                    {
                                        item.SamplingFromCatchCompositionIsAllowed = (bool)dr["can_sample_from_catch_composition"];
                                    }

                                    item.LandingSiteTypeOfSampling = dr["type_of_sampling"].ToString();

                                    if (item.LandingSiteTypeOfSampling == "cbl")
                                    {
                                        if (dr["count_carrier_landings"] != DBNull.Value)
                                        {
                                            item.CountCarrierLandings = (int)dr["count_carrier_landings"];
                                        }
                                        item.CountCarrierSamplings = (int)dr["count_carrier_sampling"];
                                        //item.CarrierBoatLanding_FishingGround_ViewModel = new CarrierBoatLanding_FishingGround_ViewModel(item);
                                        //item.CatcherBoatOperation_ViewModel = new CatcherBoatOperation_ViewModel(item);
                                    }
                                    //item.VesselCatchViewModel = new VesselCatchViewModel(item);

                                    thisList.Add(item);
                                    loopCount++;
                                }
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
                                    //_addHasOperationColumn = true;
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
        public List<GearInLandingSite> GetGearsInLandingSite(LandingSiteSampling lss)
        {
            List<GearInLandingSite> listGears = new List<GearInLandingSite>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@id", lss.PK);
                        cmd.CommandText = "Select * from temp_GearUnload where unload_day_id=@id";
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            int? boat_count = null;
                            double? catch_wt = null;
                            if (dr["boats"] != DBNull.Value)
                            {
                                boat_count = (int)dr["boats"];
                            }
                            if (dr["catch"] != DBNull.Value)
                            {
                                catch_wt = (double)dr["catch"];
                            }
                            FisheriesSector s;
                            switch (dr["sector"].ToString())
                            {
                                case "c":
                                    s = FisheriesSector.Commercial;
                                    break;
                                case "m":
                                    s = FisheriesSector.Municipal;
                                    break;
                                case "a":
                                    s = FisheriesSector.Aquaculture;
                                    break;
                                default:
                                    s = FisheriesSector.Unknown;
                                    break;
                            }

                            GearInLandingSite gear = new GearInLandingSite
                            {
                                Parent = lss,
                                GearCode = dr["gr_id"].ToString(),
                                GearText = dr["gr_text"].ToString(),
                                Sector = s,

                            };
                            if (gear.Sector == FisheriesSector.Municipal)
                            {
                                gear.CountLandingsMunicipal = boat_count;
                                gear.WeightCatchMunicipal = catch_wt;
                            }
                            else if (gear.Sector == FisheriesSector.Commercial)
                            {
                                gear.CountLandingsCommercial = boat_count;
                                gear.WeightCatchCommercial = catch_wt;
                            }
                            listGears.Add(gear);
                        }
                    }
                }
            }
            return listGears;
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
                    cmd.ExecuteNonQuery();
                    success = true;

                }
                catch (Exception ex)
                {

                }
                cmd.Connection.Close();
                conn.Close();
            }
            return success;

        }

        private static bool DeleteField(string colName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"ALTER TABLE dbo_LC_FG_sample_day_1 DROP COLUMN {colName}";

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
        private static bool UpdateTableDefinition(string colName)
        {
            bool success = false;
            using (var conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = "";
                switch (colName)
                {
                    case "date_deleted_from_server":
                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day_1 ADD COLUMN {colName} DATETIME";
                        break;
                    case "json_filename":
                    case "notes":
                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day_1 ADD COLUMN {colName} VARCHAR";
                        break;
                    case "number_gear_types_in_landingsite":
                    case "submission_id":
                    case "count_carrier_landings":
                    case "count_carrier_sampling":
                    case "number_landings_sampled":
                    case "number_landings":
                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day_1 ADD COLUMN {colName} INTEGER DEFAULT NULL";
                        break;
                    //case "number_landings_sampled":
                    //    sql = $@"ALTER TABLE dbo_LC_FG_sample_day_1 ADD COLUMN {colName} INTEGER DEFAULT NULL";
                    //    break;
                    //case "number_landings":
                    //    sql = $@"ALTER TABLE dbo_LC_FG_sample_day_1 ADD COLUMN {colName} INTEGER DEFAULT NULL";
                    //    break;
                    case "type_of_sampling":
                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day ADD COLUMN {colName} VARCHAR(3)";
                        break;
                    case "has_fishing_operation":
                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day ADD COLUMN {colName} YESNO";
                        break;
                    case "can_sample_from_catch_composition":
                    case "is_multivessel":
                        sql = $@"ALTER TABLE dbo_LC_FG_sample_day_1 ADD COLUMN {colName} YESNO";
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
                        if (colName == "type_of_sampling")
                        {
                            sql = "UPDATE dbo_LC_FG_sample_day SET type_of_sampling='rs' WHERE type_of_sampling IS NULL";
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                            success = true;
                        }
                        else
                        {
                            success = true;
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
                                        EnumeratorText,
                                        can_sample_from_catch_composition,
                                        submission_id
                                    ) Values (?,?,?,?,?,?,?,?,?,?,?,?,?)";


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
                                    update1.Parameters.Add("@can_sample", OleDbType.Boolean).Value = item.SamplingFromCatchCompositionIsAllowed;

                                    if (item.Submission_id == null)
                                    {
                                        update1.Parameters.Add("@_id", OleDbType.Integer).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        update1.Parameters.Add("@_id", OleDbType.Integer).Value = item.Submission_id;
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

        public static bool? UpdateSubmissionID(string formID, string submissionUID, int id)
        {
            bool success = false;
            bool isnull_submission_id=false;
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    con.Open();
                    using (var c = con.CreateCommand())
                    {
                        c.Parameters.AddWithValue("@xf_id", formID);
                        c.Parameters.AddWithValue("@row_id", $"{{{submissionUID}}}");

                        c.CommandText = @"SELECT 
                                            unload_day_id, 
                                            submission_id 
                                        FROM 
                                            dbo_LC_FG_sample_day_1 
                                        WHERE 
                                            XFormIdentifier=@xf_id AND
                                            RowID=@row_id";
                        try
                        {
                            var dr = c.ExecuteReader();
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    isnull_submission_id = string.IsNullOrEmpty(dr["submission_id"].ToString());
                                    break;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }

                    if (isnull_submission_id)
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            //cmd.Parameters.AddWithValue("@id", id);
                            //cmd.Parameters.AddWithValue("@form_id", formID);
                            //cmd.Parameters.AddWithValue("@submission_uid", $"{{{submissionUID}}}");
                            string s_uid = $"{{{submissionUID}}}";

                            //cmd.CommandText = $@"UPDATE dbo_LC_FG_sample_day_1
                            //                SET submission_id = @id
                            //                WHERE XFormIdentifier = @form_id AND RowID = @submission_uid";
                            //cmd.CommandText = $@"UPDATE dbo_LC_FG_sample_day_1
                            //                SET submission_id = {id}
                            //                WHERE XFormIdentifier = '{formID}' AND RowID = '{submissionUID}'";
                            cmd.CommandText = $@"UPDATE dbo_LC_FG_sample_day_1
                                                SET submission_id = {id}
                                                WHERE XFormIdentifier = '{formID}' AND RowID = '{s_uid}'";

                            try
                            {
                                int r = (int)cmd.ExecuteNonQuery();
                                success = r > 0;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return success;
        }
        public static bool DeleteSamplingWithOrphanedLandingSite()
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
                        cmd.Parameters.AddWithValue("@remark", "orphaned landing site, could be removed");

                        cmd.CommandText = @"DELETE  dbo_gear_unload.*
                                            FROM dbo_LC_FG_sample_day INNER JOIN 
                                                dbo_gear_unload ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE dbo_LC_FG_sample_day.remarks=@remark";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"DELETE  dbo_LC_FG_sample_day.*, dbo_LC_FG_sample_day_1.*
                                                FROM dbo_LC_FG_sample_day INNER JOIN 
                                                    dbo_LC_FG_sample_day_1 ON dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id
                                                    WHERE dbo_LC_FG_sample_day.remarks = @remark";

                        cmd.ExecuteNonQuery();
                        success = true;
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
                                update1.Parameters.Add("@can_sample", OleDbType.Boolean).Value = item.SamplingFromCatchCompositionIsAllowed;

                                if (item.DateDeletedFromServer == null)
                                {
                                    update1.Parameters.Add("@date_delete_server", OleDbType.Date).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@date_delete_server", OleDbType.Date).Value = (DateTime)item.DateDeletedFromServer;
                                }

                                if (item.Submission_id == null)
                                {
                                    update1.Parameters.Add("@_id", OleDbType.Integer).Value = DBNull.Value;
                                }
                                else
                                {
                                    update1.Parameters.Add("@_id", OleDbType.Integer).Value = item.Submission_id;
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
                                                    EnumeratorText = @enum_text,
                                                    can_sample_from_catch_composition = @can_sample,
                                                    date_deleted_from_server = @date_delete_server,
                                                    submission_id=@id
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
