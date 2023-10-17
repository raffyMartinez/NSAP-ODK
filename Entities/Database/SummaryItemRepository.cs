using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using NSAP_ODK.Utilities;
using MySql.Data.MySqlClient;
using NSAP_ODK.NSAPMysql;
namespace NSAP_ODK.Entities.Database
{
    public class SummaryItemRepository
    {
        public List<SummaryItem> SummaryItems { get; set; }

        public SummaryItemRepository()
        {
            SummaryItems = getSummaryItems();
        }

        public bool Add(SummaryItem si)
        {
            return true;
        }

        public bool Update(SummaryItem si)
        {
            return true;
        }

        public bool Delete(SummaryItem si)
        {
            return true;
        }

        private List<SummaryItem> getSummaryItemsFromMySQL()
        {
            List<SummaryItem> items = new List<SummaryItem>();
            using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                                 sd.unload_day_id,
                                                 ls.landing_site_id,
                                                 ls.landing_site_name,
                                                 sd.land_ctr_text AS landing_site_text,
                                                 nr.sequence AS reg_seq,
                                                 nr.short_name AS reg_shortname,
                                                 nr.region_name AS reg_name,
                                                 nr.code AS reg_code,
                                                 sd.fma AS fma_id,
                                                 sd.ground_id AS fishing_ground_code,
                                                 gu.unload_gr_id,
                                                 gr.gear_code,
                                                 gr.gear_name,
                                                 gu.gr_text AS gear_text,
                                                 gu.boats,
                                                 gu.catch,
                                                 vu.v_unload_id,
                                                 fv.vessel_name,
                                                 fv.vessel_id,
                                                 vu1.sampling_date,
                                                 vu.boat_text AS vessel_text,
                                                 vu1.user_name,
                                                 vu1.success AS is_success,
                                                 vu1.tracked AS is_tracked,
                                                 vu1.gps,
                                                 vu1.sector_code,
                                                 vu1.datetime_submitted,
                                                 vu1.form_version,
                                                 vu1.row_id,   
                                                 vu1.xform_identifier,
                                                 vu1.datetime_submitted,
                                                 vu1.is_multigear,
                                                 vu1.is_catch_sold,
                                                 vu1.ref_no   
                                                 en.enumerator_id,
                                                 vu1.enumerator_text,
                                                 en.enumerator_name,
                                                 vu1.date_added,
                                                 vu1.has_catch_composition,
                                                 vu1.trip_is_completed,
                                                 vu1.number_of_fishers AS no_fishers,
                                                 vu_st.count_effort,
                                                 vu_st.count_grid,
                                                 vu_st.count_soak,
                                                 vu_st.count_catch_composition,
                                                 vu_st.count_lengths,
                                                 vu_st.count_lenfreq,
                                                 vu_st.count_lenwt,
                                                 vu_st.count_maturity
                                            FROM 
                                                nsap_region AS nr 
                                                INNER JOIN (landing_sites AS ls 
                                                RIGHT JOIN (gears AS gr 
                                                RIGHT JOIN ((fishing_vessel AS fv 
                                                RIGHT JOIN ((dbo_lc_fg_sample_day AS sd 
                                                INNER JOIN (dbo_gear_unload AS gu 
                                                INNER JOIN dbo_vessel_unload AS vu 
                                                    ON gu.unload_gr_id = vu.unload_gr_id) 
                                                    ON sd.unload_day_id = gu.unload_day_id) 
                                                    INNER JOIN (nsap_enumerators AS en 
                                                    RIGHT JOIN dbo_vessel_unload_1 AS vu1 
                                                        ON en.enumerator_id = vu1.enumerator_id) 
                                                        ON vu.v_unload_id = vu1.v_unload_id) 
                                                        ON fv.vessel_id = vu.boat_id) 
                                                        LEFT JOIN dbo_vessel_unload_stats AS vu_st 
                                                            ON vu.v_unload_id = vu_st.v_unload_id) 
                                                            ON gr.gear_code = gu.gr_id) 
                                                            ON ls.landing_site_id = sd.land_ctr_id) 
                                                            ON nr.code = sd.region_id
                                            ORDER BY 
                                                vu1.sampling_date";
                    try
                    {
                        con.Open();
                        var dr = cmd.ExecuteReader();
                        int id = 0;
                        while (dr.Read())
                        {
                            int? ls_id = null;
                            int? en_id = null;
                            int? gu_boats = null;
                            int catch_comp_rows = 0;
                            double? gu_catch = null;
                            int? vessel_id = null;
                            int? no_fishers = null;
                            int? count_catch_comp = null;
                            int? count_efforts = null;
                            int? count_gear_soaks = null;
                            int? count_grids = null;
                            int? count_len_wt = null;
                            int? count_len = null;
                            int? count_len_freq = null;
                            int? count_maturity = null;

                            if (dr["landing_site_id"] != DBNull.Value)
                            {
                                ls_id = (int)dr["landing_site_id"];
                            }

                            if (dr["enumerator_id"] != DBNull.Value)
                            {
                                en_id = (int)dr["enumerator_id"];
                            }

                            if (dr["boats"] != DBNull.Value)
                            {
                                gu_boats = (int)dr["boats"];
                            }

                            if (dr["catch"] != DBNull.Value)
                            {
                                gu_catch = (double)dr["catch"];
                            }

                            if (dr["vessel_id"] != DBNull.Value)
                            {
                                vessel_id = (int)dr["vessel_id"];
                            }
                            if (dr["no_fishers"] != DBNull.Value)
                            {
                                no_fishers = (int)dr["no_fishers"];
                            }
                            if (dr["count_catch_composition"] != DBNull.Value)
                            {
                                count_catch_comp = (int)dr["count_catch_composition"];
                            }

                            if (dr["count_effort"] != DBNull.Value)
                            {
                                count_efforts = (int)dr["count_effort"];
                            }

                            if (dr["count_grid"] != DBNull.Value)
                            {
                                count_grids = (int)dr["count_grid"];
                            }

                            if (dr["count_soak"] != DBNull.Value)
                            {
                                count_gear_soaks = (int)dr["count_soak"];
                            }

                            if (dr["count_lengths"] != DBNull.Value)
                            {
                                count_len = (int)dr["count_lengths"];
                            }

                            if (dr["count_lenfreq"] != DBNull.Value)
                            {
                                count_len_freq = (int)dr["count_lenfreq"];
                            }

                            if (dr["count_lenwt"] != DBNull.Value)
                            {
                                count_len_wt = (int)dr["count_lenwt"];
                            }

                            if (dr["count_maturity"] != DBNull.Value)
                            {
                                count_maturity = (int)dr["count_maturity"];
                            }
                            SummaryItem si = new SummaryItem()
                            {

                                ID = ++id,
                                XFormIdentifier = dr["xform_identifier"].ToString(),
                                ODKRowID = dr["row_id"].ToString(),
                                SamplingDayID = (int)dr["unload_day_id"],
                                GearUnloadID = (int)dr["unload_gr_id"],
                                VesselUnloadID = (int)dr["v_unload_id"],
                                RegionID = dr["reg_code"].ToString(),
                                RegionSequence = (int)dr["reg_seq"],
                                FMAId = (int)dr["fma_id"],
                                FishingGroundID = dr["fishing_ground_code"].ToString(),
                                LandingSiteID = ls_id,
                                LandingSiteName = dr["landing_site_name"].ToString(),
                                LandingSiteText = dr["landing_site_text"].ToString(),
                                GearCode = dr["gear_code"].ToString(),
                                GearName = dr["gear_name"].ToString(),
                                GearText = dr["gear_text"].ToString(),
                                GearUnloadBoats = gu_boats,
                                GearUnloadCatch = gu_catch,
                                UserName = dr["user_name"].ToString(),
                                EnumeratorID = en_id,
                                EnumeratorName = dr["enumerator_name"].ToString(),
                                EnumeratorText = dr["enumerator_text"].ToString(),
                                FormVersion = dr["form_version"].ToString().Replace("Version ", ""),
                                SamplingDate = (DateTime)dr["sampling_date"],
                                DateAdded = (DateTime)dr["date_added"],
                                IsSuccess = (bool)dr["is_success"],
                                IsTracked = (bool)dr["is_tracked"],
                                IsMultiGear = (bool)dr["is_multigear"],
                                IsCatchSold = (bool)dr["is_catch_sold"],
                                RefNo = dr["ref_no"].ToString(),
                                SectorCode = dr["sector_code"].ToString(),
                                HasCatchComposition = (bool)dr["has_catch_composition"],
                                IsTripCompleted = (bool)dr["trip_is_completed"],
                                CatchCompositionRows = catch_comp_rows,
                                VesselID = vessel_id,
                                VesselName = dr["vessel_name"].ToString(),
                                VesselText = dr["vessel_text"].ToString(),
                                NumberOfFishers = no_fishers,
                                GPSCode = dr["gps"].ToString(),
                                CatchMaturityRows = count_maturity,
                                FishingGridRows = count_grids,
                                GearSoakRows = count_gear_soaks,
                                LenFreqRows = count_len_freq,
                                LengthRows = count_len,
                                LenWtRows = count_len_wt,
                                VesselEffortRows = count_efforts,
                                DateSubmitted = (DateTime)dr["datetime_submitted"]
                            };
                            if (string.IsNullOrEmpty(si.EnumeratorText))
                            {
                                si.EnumeratorText = si.UserName;
                            }
                            items.Add(si);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            return items;
        }
        public bool DelayedSave { get; set; }

        private LastPrimaryKeys RefreshFromDB(bool catchSubEntitiesOnly = false)
        {
            LastPrimaryKeys lpks = new LastPrimaryKeys();
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    con.Open();
                    if (!catchSubEntitiesOnly)
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(v_unload_id) FROM dbo_vessel_unload;";
                            try
                            {
                                lpks.LastVesselUnloadPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(fg_grid_id) FROM dbo_fg_grid;";
                            try
                            {
                                lpks.LastFishingGridsPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(effort_row_id) FROM dbo_vessel_effort;";
                            try
                            {
                                lpks.LastVesselEffortsPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(gear_soak_id) FROM dbo_gear_soak;";
                            try
                            {
                                lpks.LastGearSoaksPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(catch_id) FROM dbo_vessel_catch;";
                            try
                            {
                                lpks.LastVesselCatchPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_len_id) FROM dbo_catch_length;";
                        try
                        {
                            lpks.LastLengthsPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_maturity_id) FROM dbo_catch_maturity;";
                        try
                        {
                            lpks.LastMaturityPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_lf_id) FROM dbo_catch_len_freq;";
                        try
                        {
                            lpks.LastLenFreqPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_lw_id) FROM dbo_catch_len_wt;";
                        try
                        {
                            lpks.LastLenWtPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }
                }
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    con.Open();
                    if (!catchSubEntitiesOnly)
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(v_unload_id) FROM dbo_vessel_unload;";
                            try
                            {
                                lpks.LastVesselUnloadPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(unload_gr_id) FROM dbo_gear_unload;";
                            try
                            {
                                lpks.LastGearUnloadPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT Max(unload_day_id) AS max_id FROM dbo_LC_FG_sample_day;";
                            try
                            {
                                lpks.LastLandingSiteSamplingPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT Max(row_id) AS max_id FROM dbo_vessel_unload_stats;";
                            try
                            {
                                lpks.LastUnloadStatPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT Max(row_id) AS max_id FROM dbo_vessel_unload_weight_validation;";
                            try
                            {
                                lpks.LastWeightValidationPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT Max(row_id) AS max_id FROM dbo_vesselunload_fishinggear;";
                            try
                            {
                                lpks.LastVesselUnloadGearPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT Max(effort_row_id) AS max_id FROM dbo_vessel_effort;";
                            try
                            {
                                lpks.LastVesselUnloadGearSpecPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(fg_grid_id) FROM dbo_fg_grid;";
                            try
                            {
                                lpks.LastFishingGridsPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(effort_row_id) FROM dbo_vessel_effort;";
                            try
                            {
                                lpks.LastVesselEffortsPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(gear_soak_id) FROM dbo_gear_soak;";
                            try
                            {
                                lpks.LastGearSoaksPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "SELECT max(catch_id) FROM dbo_vessel_catch;";
                            try
                            {
                                lpks.LastVesselCatchPK = (int)cmd.ExecuteScalar();
                            }
                            catch { }
                        }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_len_id) FROM dbo_catch_len";
                        try
                        {
                            lpks.LastLengthsPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_maturity_id) FROM dbo_catch_maturity;";
                        try
                        {
                            lpks.LastMaturityPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_len_freq_id) FROM dbo_catch_len_freq;";
                        try
                        {
                            lpks.LastLenFreqPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT max(catch_len_wt_id) FROM dbo_catch_len_wt;";
                        try
                        {
                            lpks.LastLenWtPK = (int)cmd.ExecuteScalar();
                        }
                        catch { }
                    }
                }
            }

            return lpks;
        }
        public LastPrimaryKeys GetLastPrimaryKeys(bool delayedSave = false)
        {
            LastPrimaryKeys lpks = new LastPrimaryKeys();
            if (delayedSave)
            {
                lpks.LastGearUnloadPK = GearUnloadViewModel.CurrentIDNumber;
                lpks.LastVesselUnloadPK = VesselUnloadViewModel.CurrentIDNumber;
                lpks.LastFishingGridsPK = FishingGroundGridViewModel.CurrentIDNumber;
                lpks.LastGearSoaksPK = GearSoakViewModel.CurrentIDNumber;
                lpks.LastVesselEffortsPK = VesselEffortViewModel.CurrentIDNumber; //> VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber ? VesselEffortViewModel.CurrentIDNumber : VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber;
                lpks.LastVesselCatchPK = VesselCatchViewModel.CurrentIDNumber;
                lpks.LastLenWtPK = CatchLengthWeightViewModel.CurrentIDNumber;
                lpks.LastLengthsPK = CatchLengthViewModel.CurrentIDNumber;
                lpks.LastLenFreqPK = CatchLenFreqViewModel.CurrentIDNumber;
                lpks.LastMaturityPK = CatchMaturityViewModel.CurrentIDNumber;
                lpks.LastLandingSiteSamplingPK = LandingSiteSamplingViewModel.CurrentIDNumber;
                lpks.LastVesselUnloadGearPK = VesselUnload_FishingGearViewModel.CurrentIDNumber;
                lpks.LastUnloadStatPK = (int)VesselUnloadViewModel.CurrentUnloadStatIDNumber;
                lpks.LastWeightValidationPK = (int)VesselUnloadViewModel.CurrentWeightValidationIDNumber;

                if (lpks.LastVesselUnloadPK == 0 && NSAPEntities.SummaryItemViewModel.Count > 0)
                {
                    lpks = GetLastPrimaryKeys();
                }

                if (lpks.LastLenFreqPK == 0 || lpks.LastLengthsPK == 0 || lpks.LastLenWtPK == 0 || lpks.LastMaturityPK == 0)
                {
                    var lpks1 = RefreshFromDB(catchSubEntitiesOnly: true);
                    lpks.LastLenWtPK = lpks1.LastLenWtPK;
                    lpks.LastLengthsPK = lpks1.LastLengthsPK;
                    lpks.LastLenFreqPK = lpks1.LastLenFreqPK;
                    lpks.LastMaturityPK = lpks1.LastMaturityPK;
                }
            }
            else
            {
                lpks = RefreshFromDB();
            }
            return lpks;
        }
        private List<SummaryItem> getSummaryItems()
        {

            int count = 0;
            List<SummaryItem> items = new List<SummaryItem>();
            if (Global.Settings.UsemySQL)
            {
                items = getSummaryItemsFromMySQL();
            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {


                    using (var cmd = con.CreateCommand())
                    {
                        bool proceed = false;
                        con.Open();
                        cmd.CommandText = @"DROP TABLE  temp_GearUnload";
                        try
                        {
                            cmd.ExecuteNonQuery();
                            proceed = true;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "Table 'temp_GearUnload' does not exist.")
                            {
                                proceed = true;
                            }
                            else
                            {
                                Logger.Log(ex);
                            }

                        }
                        if (proceed)
                        {
                            cmd.CommandText = @"CREATE TABLE temp_GearUnload
                                                    (unload_gr_id INT,
                                                    unload_day_id INT,
                                                    sector char(1),
                                                    boats INT,
                                                    catch DOUBLE,
                                                    gr_id varchar(10),
                                                    gr_text varchar(255),
                                                    remarks varchar(255))";
                            try
                            {
                                cmd.ExecuteNonQuery();
                                proceed = true;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                        if (proceed)
                        {
                            try
                            {
                                cmd.CommandText = @"INSERT INTO temp_GearUnload ( unload_gr_id, unload_day_id, sector, boats, catch, gr_id, gr_text, remarks )
                                                    SELECT 
                                                        dbo_gear_unload.unload_gr_id, 
                                                        dbo_gear_unload.unload_day_id, 
                                                        'c' AS sector, 
                                                        dbo_gear_unload.gear_count_commercial, 
                                                        dbo_gear_unload.gear_catch_commercial, 
                                                        dbo_gear_unload.gr_id, 
                                                        dbo_gear_unload.gr_text, 
                                                        dbo_gear_unload.remarks
                                                        FROM dbo_gear_unload
                                                     WHERE dbo_gear_unload.gear_count_commercial>0";
                                cmd.ExecuteNonQuery();
                                proceed = true;

                                cmd.CommandText = @"INSERT INTO temp_GearUnload ( unload_gr_id, unload_day_id, sector, boats, catch, gr_id, gr_text, remarks )
                                                    SELECT 
                                                        dbo_gear_unload.unload_gr_id, 
                                                        dbo_gear_unload.unload_day_id, 
                                                        'm' AS Expr1, 
                                                        dbo_gear_unload.gear_count_municipal, 
                                                        dbo_gear_unload.gear_catch_municipal, 
                                                        dbo_gear_unload.gr_id, 
                                                        dbo_gear_unload.gr_text, 
                                                        dbo_gear_unload.remarks
                                                        FROM dbo_gear_unload
                                                     WHERE dbo_gear_unload.gear_count_municipal>0";

                                cmd.ExecuteNonQuery();
                                proceed = true;

                                cmd.CommandText = @"INSERT INTO temp_GearUnload ( unload_gr_id, unload_day_id, sector, boats, catch, gr_id, gr_text, remarks )
                                                    SELECT 
                                                        dbo_gear_unload.unload_gr_id,                                                         
                                                        dbo_gear_unload.unload_day_id,
                                                        dbo_gear_unload.sector, 
                                                        dbo_gear_unload.boats, 
                                                        dbo_gear_unload.catch, 
                                                        dbo_gear_unload.gr_id, 
                                                        dbo_gear_unload.gr_text,
                                                        dbo_gear_unload.remarks
                                                    FROM dbo_gear_unload
                                                    WHERE sector = 'c'";

                                cmd.ExecuteNonQuery();
                                proceed = true;

                                cmd.CommandText = @"INSERT INTO temp_GearUnload ( unload_gr_id, unload_day_id, sector, boats, catch, gr_id, gr_text, remarks )
                                                    SELECT 
                                                        dbo_gear_unload.unload_gr_id,                                                         
                                                        dbo_gear_unload.unload_day_id,
                                                        dbo_gear_unload.sector, 
                                                        dbo_gear_unload.boats, 
                                                        dbo_gear_unload.catch, 
                                                        dbo_gear_unload.gr_id, 
                                                        dbo_gear_unload.gr_text,
                                                        dbo_gear_unload.remarks
                                                    FROM dbo_gear_unload
                                                    WHERE sector = 'm'";

                                cmd.ExecuteNonQuery();
                                proceed = true;

                                //union

                                //SELECT unload_day_id, unload_gr_id, 'm' AS sector_name, gear_count_municipal, gear_catch_municipal, gr_id, gr_text, remarks
                                //    FROM dbo_gear_unload
                                //    WHERE gear_count_municipal > 0

                                //    union

                                //    SELECT unload_day_id, unload_gr_id, sector, boats, catch, gr_id, gr_text,remarks
                                //    FROM dbo_gear_unload
                                //    WHERE sector = 'c'


                                //    union

                                //    SELECT unload_day_id, unload_gr_id, sector, boats, catch, gr_id, gr_text,remarks
                                //    FROM dbo_gear_unload
                                //    WHERE sector = 'm'

                                //    INTO temp_GearUnload";

                                //cmd.ExecuteNonQuery();
                                //proceed = true;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex);
                            }
                        }
                        if (proceed)
                        {
                            cmd.CommandText = @"SELECT 
                                                sd.sampleday AS is_sampling_day,    
                                                sd.unload_day_id,
                                                sd.sdate AS SamplingDayDate,
                                                sd1.device_id,
                                                ls.LandingSiteID AS landing_site_id,
                                                ls.LandingSiteName AS landing_site_name,
                                                sd.land_ctr_text AS landing_site_text,
                                                nr.Sequence AS reg_seq,
                                                nr.ShortName AS reg_shortname,
                                                nr.RegionName AS reg_name,
                                                nr.Code AS reg_code,
                                                sd.fma AS fma_id,
                                                sd.ground_id AS fishing_ground_code,
                                                sd.has_fishing_operation,
                                                sd.remarks,
                                                gu.unload_gr_id,
                                                gr.GearCode AS gear_code,
                                                gr.GearName AS gear_name,
                                                gu.gr_text AS gear_text,
                                                gu.gear_count_municipal,
                                                gu.gear_count_commercial,
                                                gu.gear_catch_municipal,
                                                gear_catch_commercial,
                                                gu.boats,
                                                gu.catch,
                                                vu.v_unload_id,
                                                vu.catch_total,
                                                vu.catch_samp,
                                                fv.VesselName AS vessel_name,
                                                fv.VesselID AS vessel_id,
                                                vu1.SamplingDate AS sampling_date,
                                                vu1.json_filename as json_filename,
                                                vu.boat_text AS vessel_text,
                                                vu1.user_name,
                                                vu1.Success AS is_success,
                                                vu1.Tracked AS is_tracked,
                                                vu1.GPS,
                                                vu1.sector_code,
                                                vu1.datetime_submitted,
                                                vu1.form_version,
                                                vu1.RowID,
                                                sd1.RowID AS sd_rowid,
                                                sd1.is_multivessel,
                                                sd1.XFormIdentifier AS lss_xformid,
                                                sd1.datetime_submitted as lss_date_submitted,
                                                sd1.can_sample_from_catch_composition,
                                                vu1.lss_submisionID,
                                                vu1.XFormIdentifier AS xform_identifier,
                                                vu1.datetime_submitted,
                                                vu1.is_multigear,
                                                vu1.is_catch_sold,
                                                vu1.count_gear_types,
                                                en.EnumeratorID AS enumerator_id,
                                                vu1.EnumeratorText AS enumerator_text,
                                                en.EnumeratorName AS enumerator_name,
                                                vu1.DateAdded AS date_added,
                                                vu1.HasCatchComposition AS has_catch_composition,
                                                vu1.ref_no,
                                                vu1.trip_is_completed,
                                                vu1.NumberOfFishers AS no_fishers,
                                                vu_st.count_effort,
                                                vu_st.count_grid,
                                                vu_st.count_soak,
                                                vu_st.count_catch_composition,
                                                vu_st.count_lengths,
                                                vu_st.count_lenfreq,
                                                vu_st.count_lenwt,
                                                vu_st.count_maturity,
                                                vu_wv.total_wt_catch_composition,
                                                vu_wv.total_wt_sampled_species,
                                                vu_wv.validity_flag,
                                                vu_wv.type_of_sampling_flag,
                                                vu_wv.weight_difference,
                                                vu_wv.raising_factor
                                            FROM (
                                                nsapRegion AS nr 
                                                INNER JOIN (landingSite AS ls 
                                                RIGHT JOIN (gear AS gr 
                                                RIGHT JOIN ((fishingVessel AS fv 
                                                RIGHT JOIN (((dbo_LC_FG_sample_day AS sd 
                                                LEFT JOIN (dbo_gear_unload AS gu 
                                                LEFT JOIN dbo_vessel_unload AS vu ON gu.unload_gr_id = vu.unload_gr_id) ON sd.unload_day_id = gu.unload_day_id) 
                                                INNER JOIN dbo_LC_FG_sample_day_1 as sd1 ON sd.unload_day_id = sd1.unload_day_id) 
                                                LEFT JOIN (NSAPEnumerator AS en 
                                                RIGHT JOIN dbo_vessel_unload_1 AS vu1 ON en.EnumeratorID = vu1.EnumeratorID) ON vu.v_unload_id = vu1.v_unload_id) ON fv.VesselID = vu.boat_id) 
                                                LEFT JOIN dbo_vessel_unload_stats AS vu_st ON vu.v_unload_id = vu_st.v_unload_id) ON gr.GearCode = gu.gr_id) ON ls.LandingSiteID = sd.land_ctr_id) ON nr.Code = sd.region_id) 
                                                LEFT JOIN dbo_vessel_unload_weight_validation AS vu_wv ON vu.v_unload_id = vu_wv.v_unload_id";
                                                                                               
                                                //ORDER BY vu1.SamplingDate";
                            //WHERE vu.v_unload_id Is Not Null  removed befoe ORDER BY clause


                            if(Global.Filter1!=null)
                            {
                                cmd.Parameters.AddWithValue("@d1", Global.Filter1DateString());
                                if(Global.Filter2!=null)
                                {
                                    cmd.Parameters.AddWithValue("@d2", Global.Filter2DateString());
                                    cmd.CommandText += " WHERE sd.sdate>=@d1 AND sd.sdate <@d2";
                                }
                                else
                                {
                                    cmd.CommandText += " WHERE sd.sdate>=@d1";
                                }
                            }

                            cmd.CommandText += " ORDER BY vu1.SamplingDate";

                            try
                            {
                                //con.Open();
                                var dr = cmd.ExecuteReader();
                                int id = 0;

                                while (dr.Read())
                                {
                                    //int? gr_unload_id = null;
                                    //int? vs_unload_id = null;
                                    int? ls_id = null;
                                    int? en_id = null;
                                    int? gu_boats = null;
                                    double? gu_catch = null;
                                    int? vessel_id = null;
                                    int? no_fishers = null;
                                    int? count_catch_comp = null;
                                    int? count_efforts = null;
                                    int? count_gear_soaks = null;
                                    int? count_grids = null;
                                    int? count_len_wt = null;
                                    int? count_len = null;
                                    int? count_len_freq = null;
                                    int? count_maturity = null;
                                    int? count_twsp = null;
                                    int? count_gear_types = null;

                                    string xform_id = dr["xform_identifier"].ToString();

                                    double? weight_catch = null;
                                    double? weight_catch_sample = null;

                                    double? total_catch_comp_wt = null;
                                    double? total_catch_comp_sample_wt = null;
                                    bool isSamplingDay = false;

                                    if (dr["catch_total"] != DBNull.Value)
                                    {
                                        weight_catch = (double)dr["catch_total"];
                                    }

                                    if (dr["is_sampling_day"] != DBNull.Value)
                                    {
                                        isSamplingDay = (bool)dr["is_sampling_day"];
                                    }

                                    if (dr["catch_samp"] != DBNull.Value)
                                    {
                                        weight_catch_sample = (double)dr["catch_samp"];
                                    }

                                    if (dr["landing_site_id"] != DBNull.Value)
                                    {
                                        ls_id = (int)dr["landing_site_id"];
                                    }

                                    if (dr["enumerator_id"] != DBNull.Value)
                                    {
                                        en_id = (int)dr["enumerator_id"];
                                    }

                                    if (dr["count_gear_types"] != DBNull.Value)
                                    {
                                        count_gear_types = (int)dr["count_gear_types"];
                                    }

                                    if (dr["boats"] != DBNull.Value)
                                    {
                                        gu_boats = (int)dr["boats"];
                                    }

                                    if (dr["catch"] != DBNull.Value)
                                    {
                                        gu_catch = (double)dr["catch"];
                                    }
                                    //if (dr["sp_twsp_count"] != DBNull.Value)
                                    //{
                                    //    count_twsp = (int)dr["sp_twsp_count"];
                                    //}
                                    if (dr["vessel_id"] != DBNull.Value)
                                    {
                                        vessel_id = (int)dr["vessel_id"];
                                    }
                                    if (dr["no_fishers"] != DBNull.Value)
                                    {
                                        no_fishers = (int)dr["no_fishers"];
                                    }
                                    if (dr["count_catch_composition"] != DBNull.Value)
                                    {
                                        count_catch_comp = (int)dr["count_catch_composition"];
                                    }

                                    if (dr["count_effort"] != DBNull.Value)
                                    {
                                        count_efforts = (int)dr["count_effort"];
                                    }

                                    if (dr["count_grid"] != DBNull.Value)
                                    {
                                        count_grids = (int)dr["count_grid"];
                                    }

                                    if (dr["count_soak"] != DBNull.Value)
                                    {
                                        count_gear_soaks = (int)dr["count_soak"];
                                    }

                                    if (dr["count_lengths"] != DBNull.Value)
                                    {
                                        count_len = (int)dr["count_lengths"];
                                    }

                                    if (dr["count_lenfreq"] != DBNull.Value)
                                    {
                                        count_len_freq = (int)dr["count_lenfreq"];
                                    }

                                    if (dr["count_lenwt"] != DBNull.Value)
                                    {
                                        count_len_wt = (int)dr["count_lenwt"];
                                    }

                                    if (dr["count_maturity"] != DBNull.Value)
                                    {
                                        count_maturity = (int)dr["count_maturity"];
                                    }

                                    if (dr["total_wt_catch_composition"] != DBNull.Value)
                                    {
                                        total_catch_comp_wt = (double)dr["total_wt_catch_composition"];
                                    }

                                    if (dr["total_wt_sampled_species"] != DBNull.Value)
                                    {
                                        total_catch_comp_sample_wt = (double)dr["total_wt_sampled_species"];
                                    }


                                    SummaryItem si = new SummaryItem();
                                    si.ID = ++id;
                                    si.ODKRowID = dr["RowID"].ToString();
                                    si.SamplingDayUUID = dr["sd_rowid"].ToString();
                                    si.SamplingDayID = (int)dr["unload_day_id"];
                                    si.SamplingDayDate = (DateTime)dr["SamplingDayDate"];
                                    si.IsSamplingDay = isSamplingDay;
                                    si.IsMultiVessel = (bool)dr["is_multivessel"];
                                    si.SamplingFromCatchCompositionAllowed = (bool)dr["can_sample_from_catch_composition"];
                                    si.LandingSiteSamplingSubmissionId = dr["lss_submisionID"].ToString();
                                    if (si.IsMultiVessel)
                                    {
                                        xform_id = dr["lss_xformid"].ToString();
                                    }
                                    //si.XFormIdentifier = dr["xform_identifier"].ToString();
                                    si.XFormIdentifier = xform_id;
                                    si.LandingSiteSamplingNotes = dr["remarks"].ToString();
                                    if (dr["unload_gr_id"] != DBNull.Value)
                                    {
                                        si.GearUnloadID = (int)dr["unload_gr_id"];
                                    }

                                    if (dr["v_unload_id"] != DBNull.Value)
                                    {
                                        si.VesselUnloadID = (int)dr["v_unload_id"];
                                    }
                                    else
                                    {

                                    }

                                    if (dr["type_of_sampling_flag"] != DBNull.Value)
                                    {
                                        si.SamplingTypeFlag = (SamplingTypeFlag)(int)dr["type_of_sampling_flag"];
                                    }

                                    if (dr["validity_flag"] != DBNull.Value)
                                    {
                                        si.WeightValidationFlag = (WeightValidationFlag)(int)dr["validity_flag"];
                                    }

                                    si.WeightOfCatch = weight_catch;
                                    si.WeightOfCatchSample = weight_catch_sample;
                                    si.SumOfCatchCompositionWeight = total_catch_comp_wt;
                                    si.SumOfCatchCompositionSampleWeight = total_catch_comp_sample_wt;
                                    if (dr["weight_difference"] != DBNull.Value)
                                    {
                                        si.DifferenceCatchWtandSumCatchCompWeight = (double)dr["weight_difference"];
                                    }
                                    if (dr["raising_factor"] != DBNull.Value)
                                    {
                                        si.RaisingFactor = (double)dr["raising_factor"];
                                    }

                                    si.RegionID = dr["reg_code"].ToString();
                                    si.RegionSequence = (int)dr["reg_seq"];
                                    si.FMAId = (int)dr["fma_id"];
                                    si.FishingGroundID = dr["fishing_ground_code"].ToString();
                                    si.LandingSiteID = ls_id;
                                    si.LandingSiteName = dr["landing_site_name"].ToString();
                                    si.LandingSiteText = dr["landing_site_text"].ToString();
                                    si.LandingSiteHasOperation = (bool)dr["has_fishing_operation"];
                                    si.GearCode = dr["gear_code"].ToString();
                                    si.GearName = dr["gear_name"].ToString();
                                    si.GearText = dr["gear_text"].ToString();
                                    si.GearUnloadBoats = gu_boats;
                                    si.GearUnloadCatch = gu_catch;

                                    if (dr["gear_count_municipal"] != DBNull.Value)
                                    {
                                        si.GearUnloadNumberMunicipalLandings = (int)dr["gear_count_municipal"];
                                    }

                                    if (dr["gear_count_commercial"] != DBNull.Value)
                                    {
                                        si.GearUnloadNumberCommercialLandings = (int)dr["gear_count_commercial"];
                                    }

                                    if (dr["gear_catch_municipal"] != DBNull.Value)
                                    {
                                        si.GearUnloadWeightMunicipalLandings = (double)dr["gear_catch_municipal"];
                                    }

                                    if (dr["gear_catch_commercial"] != DBNull.Value)
                                    {
                                        si.GearUnloadWeightCommercialLandings = (double)dr["gear_catch_commercial"];
                                    }

                                    //si.TWSpCount = count_twsp;
                                    si.UserName = dr["user_name"].ToString();
                                    si.EnumeratorID = en_id;
                                    si.EnumeratorName = dr["enumerator_name"].ToString();
                                    si.EnumeratorText = dr["enumerator_text"].ToString();
                                    si.FormVersion = dr["form_version"].ToString().Replace("Version ", "");

                                    if (dr["sampling_date"] != DBNull.Value)
                                    {
                                        si.SamplingDate = (DateTime)dr["sampling_date"];
                                    }

                                    si.DateAdded = dr["date_added"] == DBNull.Value ? DateTime.Now : (DateTime)dr["date_added"];
                                    si.IsSuccess = (bool)dr["is_success"];
                                    si.IsTracked = (bool)dr["is_tracked"];
                                    si.SectorCode = dr["sector_code"].ToString();
                                    si.HasCatchComposition = (bool)dr["has_catch_composition"];
                                    si.IsMultiGear = (bool)dr["is_multigear"];
                                    si.IsCatchSold = (bool)dr["is_catch_sold"];
                                    si.IsTripCompleted = (bool)dr["trip_is_completed"];
                                    si.CatchCompositionRows = count_catch_comp;
                                    si.VesselID = vessel_id;
                                    si.VesselName = dr["vessel_name"].ToString();
                                    si.VesselText = dr["vessel_text"].ToString();
                                    si.NumberOfFishers = no_fishers;
                                    si.GPSCode = dr["gps"].ToString();
                                    si.CatchMaturityRows = count_maturity;
                                    si.FishingGridRows = count_grids;
                                    si.GearSoakRows = count_gear_soaks;
                                    si.LenFreqRows = count_len_freq;
                                    si.LengthRows = count_len;
                                    si.LenWtRows = count_len_wt;
                                    si.VesselEffortRows = count_efforts;
                                    si.RefNo = dr["ref_no"].ToString();
                                    si.JSONFileName = dr["json_filename"].ToString();
                                    if (dr["datetime_submitted"] != DBNull.Value)
                                    {
                                        si.DateSubmitted = (DateTime)dr["datetime_submitted"];
                                    }
                                    else if (dr["lss_date_submitted"] != DBNull.Value)
                                    {
                                        si.DateSubmitted = (DateTime)dr["lss_date_submitted"];
                                    }
                                    if (string.IsNullOrEmpty(si.EnumeratorText))
                                    {
                                        si.EnumeratorText = si.UserName;
                                    }

                                    if (si.IsMultiGear)
                                    {
                                        si.CountFishingGearTypesUsed = count_gear_types;
                                    }
                                    else
                                    {
                                        si.CountFishingGearTypesUsed = 1;
                                    }

                                    items.Add(si);
                                    count++;
                                    //if (count == 21537)
                                    //{

                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"error at loop {count}");
                                Logger.Log(ex);
                            }
                        }
                    }
                }
            }
            return items;
        }
    }
}
