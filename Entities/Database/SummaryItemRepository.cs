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

        public LastPrimaryKeys GetLastPrimaryKeys(bool delayedSave = false)
        {
            LastPrimaryKeys lpks = new LastPrimaryKeys();
            if (delayedSave)
            {
                lpks.LastVesselUnloadPK = VesselUnloadViewModel.CurrentIDNumber;
                lpks.LastFishingGridsPK = FishingGroundGridViewModel.CurrentIDNumber;
                lpks.LastGearSoaksPK = GearSoakViewModel.CurrentIDNumber;
                lpks.LastVesselEffortsPK = VesselEffortViewModel.CurrentIDNumber;
                lpks.LastVesselCatchPK = VesselCatchViewModel.CurrentIDNumber;
                lpks.LastLenWtPK = CatchLengthWeightViewModel.CurrentIDNumber;
                lpks.LastLengthsPK = CatchLengthViewModel.CurrentIDNumber;
                lpks.LastLenFreqPK = CatchLenFreqViewModel.CurrentIDNumber;
                lpks.LastMaturityPK = CatchMaturityViewModel.CurrentIDNumber;

                if (lpks.LastVesselUnloadPK == 0 && NSAPEntities.SummaryItemViewModel.Count > 0)
                {
                    lpks = GetLastPrimaryKeys();
                }
            }
            else
            {
                if (Global.Settings.UsemySQL)
                {
                    using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                    {
                        con.Open();
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
            }
            return lpks;
        }
        public LastPrimaryKeys GetLastPrimaryKeys1(bool delayedSave = false)
        {
            LastPrimaryKeys lpks = new LastPrimaryKeys();
            if (Global.Settings.UsemySQL)
            {
                using (var con = new MySqlConnection(MySQLConnect.ConnectionString()))
                {
                    con.Open();
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
                if (delayedSave)
                {
                    lpks.LastVesselUnloadPK = VesselUnloadViewModel.CurrentIDNumber;
                    lpks.LastFishingGridsPK = FishingGroundGridViewModel.CurrentIDNumber;
                    lpks.LastGearSoaksPK = GearSoakViewModel.CurrentIDNumber;
                    lpks.LastVesselEffortsPK = VesselEffortViewModel.CurrentIDNumber;
                    lpks.LastVesselCatchPK = VesselCatchViewModel.CurrentIDNumber;
                    lpks.LastLenWtPK = CatchLengthWeightViewModel.CurrentIDNumber;
                    lpks.LastLengthsPK = CatchLengthViewModel.CurrentIDNumber;
                    lpks.LastLenFreqPK = CatchLenFreqViewModel.CurrentIDNumber;
                    lpks.LastMaturityPK = CatchMaturityViewModel.CurrentIDNumber;

                    if (lpks.LastVesselUnloadPK == 0 && NSAPEntities.SummaryItemViewModel.Count > 0)
                    {
                        lpks = GetLastPrimaryKeys();
                    }

                }
                else
                {
                    using (var con = new OleDbConnection(Global.ConnectionString))
                    {
                        con.Open();
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
                        //cmd.CommandText = @"SELECT
                        //    sd.unload_day_id, 
                        //    ls.LandingSiteID AS landing_site_id, 
                        //    ls.LandingSiteName AS landing_site_name, 
                        //    sd.land_ctr_text AS landing_site_text, 
                        //    nr.Sequence AS reg_seq, 
                        //    nr.ShortName AS reg_shortname, 
                        //    nr.RegionName AS reg_name, 
                        //    nr.Code AS reg_code, 
                        //    sd.fma AS fma_id, 
                        //    sd.ground_id AS fishing_ground_code, 
                        //    sd.has_fishing_operation, 
                        //    sd.remarks, 
                        //    gu.unload_gr_id, 
                        //    gr.GearCode AS gear_code, 
                        //    gr.GearName AS gear_name, 
                        //    gu.gr_text AS gear_text, 
                        //    gu.boats, gu.catch, 
                        //    vu.v_unload_id, 
                        //    fv.VesselName AS vessel_name, 
                        //    fv.VesselID AS vessel_id, 
                        //    vu1.SamplingDate AS sampling_date, 
                        //    vu.boat_text AS vessel_text, 
                        //    vu1.user_name, 
                        //    vu1.Success AS is_success, 
                        //    vu1.Tracked AS is_tracked, 
                        //    vu1.GPS, 
                        //    vu1.sector_code, 
                        //    vu1.datetime_submitted, 
                        //    vu1.form_version, 
                        //    vu1.RowID, 
                        //    vu1.XFormIdentifier AS xform_identifier, 
                        //    vu1.datetime_submitted, 
                        //    en.EnumeratorID AS enumerator_id, 
                        //    vu1.EnumeratorText AS enumerator_text, 
                        //    en.EnumeratorName AS enumerator_name, 
                        //    vu1.DateAdded AS date_added, 
                        //    vu1.HasCatchComposition AS has_catch_composition, 
                        //    vu1.trip_is_completed, 
                        //    vu1.NumberOfFishers AS no_fishers, 
                        //    vu_st.count_effort, 
                        //    vu_st.count_grid, 
                        //    vu_st.count_soak, 
                        //    vu_st.count_catch_composition, 
                        //    vu_st.count_lengths, 
                        //    vu_st.count_lenfreq, 
                        //    vu_st.count_lenwt, 
                        //    vu_st.count_maturity
                        //    FROM 
                        //        nsapRegion AS nr 
                        //        INNER JOIN (landingSite AS ls 
                        //        RIGHT JOIN (gear AS gr 
                        //        RIGHT JOIN ((fishingVessel AS fv 
                        //        RIGHT JOIN ((dbo_LC_FG_sample_day AS sd 
                        //        LEFT JOIN (dbo_gear_unload AS gu 
                        //        LEFT JOIN dbo_vessel_unload AS vu 
                        //            ON gu.unload_gr_id = vu.unload_gr_id) 
                        //            ON sd.unload_day_id = gu.unload_day_id) 
                        //            LEFT JOIN (NSAPEnumerator AS en 
                        //            RIGHT JOIN dbo_vessel_unload_1 AS vu1 
                        //                ON en.EnumeratorID = vu1.EnumeratorID) 
                        //                ON vu.v_unload_id = vu1.v_unload_id) 
                        //                ON fv.VesselID = vu.boat_id) 
                        //                    LEFT JOIN dbo_vessel_unload_stats AS vu_st 
                        //                        ON vu.v_unload_id = vu_st.v_unload_id) 
                        //                        ON gr.GearCode = gu.gr_id) 
                        //                        ON ls.LandingSiteID = sd.land_ctr_id) 
                        //                        ON nr.Code = sd.region_id
                        //    ORDER BY sd.unload_day_id, vu1.SamplingDate";
                        cmd.CommandText = @"SELECT 
                                                 sd.unload_day_id,
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
                                                 gu.boats,
                                                 gu.catch,
                                                 gu.sp_twsp_count,   
                                                 vu.v_unload_id,
                                                 fv.VesselName AS vessel_name,
                                                 fv.VesselID AS vessel_id,
                                                 vu1.SamplingDate AS sampling_date,
                                                 vu.boat_text AS vessel_text,
                                                 vu1.user_name,
                                                 vu1.Success AS is_success,
                                                 vu1.Tracked AS is_tracked,
                                                 vu1.GPS,
                                                 vu1.sector_code,
                                                 vu1.datetime_submitted,
                                                 vu1.form_version,
                                                 vu1.RowID,   
                                                 vu1.XFormIdentifier AS xform_identifier, 
                                                 vu1.datetime_submitted,
                                                 en.EnumeratorID AS enumerator_id,
                                                 vu1.EnumeratorText AS enumerator_text,
                                                 en.EnumeratorName AS enumerator_name,
                                                 vu1.DateAdded AS date_added,
                                                 vu1.HasCatchComposition AS has_catch_composition,
                                                 vu1.trip_is_completed,
                                                 vu1.NumberOfFishers AS no_fishers,
                                                 vu_st.count_effort,
                                                 vu_st.count_grid,
                                                 vu_st.count_soak,
                                                 vu_st.count_catch_composition,
                                                 vu_st.count_lengths,
                                                 vu_st.count_lenfreq,
                                                 vu_st.count_lenwt,
                                                 vu_st.count_maturity
                                            FROM 
                                                nsapRegion AS nr 
                                                INNER JOIN (landingSite AS ls 
                                                RIGHT JOIN (gear AS gr 
                                                RIGHT JOIN ((fishingVessel AS fv 
                                                RIGHT JOIN ((dbo_LC_FG_sample_day AS sd 
                                                INNER JOIN (dbo_gear_unload AS gu 
                                                INNER JOIN dbo_vessel_unload AS vu 
                                                    ON gu.unload_gr_id = vu.unload_gr_id) 
                                                    ON sd.unload_day_id = gu.unload_day_id) 
                                                    INNER JOIN (NSAPEnumerator AS en 
                                                    RIGHT JOIN dbo_vessel_unload_1 AS vu1 
                                                        ON en.EnumeratorID = vu1.EnumeratorID) 
                                                        ON vu.v_unload_id = vu1.v_unload_id) 
                                                        ON fv.VesselID = vu.boat_id) 
                                                        LEFT JOIN dbo_vessel_unload_stats AS vu_st 
                                                            ON vu.v_unload_id = vu_st.v_unload_id) 
                                                            ON gr.GearCode = gu.gr_id) 
                                                            ON ls.LandingSiteID = sd.land_ctr_id) 
                                                            ON nr.Code = sd.region_id
                                            ORDER BY 
                                                vu1.SamplingDate";
                        try
                        {
                            con.Open();
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
                                if(dr["sp_twsp_count"] != DBNull.Value)
                                {
                                    count_twsp = (int)dr["sp_twsp_count"];
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
                                SummaryItem si = new SummaryItem();
                                si.ID = ++id;
                                si.ODKRowID = dr["RowID"].ToString();
                                si.XFormIdentifier = dr["xform_identifier"].ToString();
                                si.SamplingDayID = (int)dr["unload_day_id"];

                                si.GearUnloadID = (int)dr["unload_gr_id"];
                                si.VesselUnloadID = (int)dr["v_unload_id"];
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
                                si.TWSpCount = count_twsp;
                                si.UserName = dr["user_name"].ToString();
                                si.EnumeratorID = en_id;
                                si.EnumeratorName = dr["enumerator_name"].ToString();
                                si.EnumeratorText = dr["enumerator_text"].ToString();
                                si.FormVersion = dr["form_version"].ToString().Replace("Version ", "");
                                si.SamplingDate = (DateTime)dr["sampling_date"];
                                si.DateAdded = dr["date_added"] == DBNull.Value ? DateTime.Now : (DateTime)dr["date_added"];
                                si.IsSuccess = (bool)dr["is_success"];
                                si.IsTracked = (bool)dr["is_tracked"];
                                si.SectorCode = dr["sector_code"].ToString();
                                si.HasCatchComposition = (bool)dr["has_catch_composition"];
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
                                si.DateSubmitted = (DateTime)dr["datetime_submitted"];

                                //SummaryItem si = new SummaryItem
                                //{
                                //    ID = ++id,
                                //    ODKRowID = dr["RowID"].ToString(),
                                //    XFormIdentifier = dr["xform_identifier"].ToString(),
                                //    SamplingDayID = (int)dr["unload_day_id"],
                                //    GearUnloadID = (int)dr["unload_gr_id"],
                                //    VesselUnloadID = (int)dr["v_unload_id"],
                                //    RegionID = dr["reg_code"].ToString(),
                                //    RegionSequence = (int)dr["reg_seq"],
                                //    FMAId = (int)dr["fma_id"],
                                //    FishingGroundID = dr["fishing_ground_code"].ToString(),
                                //    LandingSiteID = ls_id,
                                //    LandingSiteName = dr["landing_site_name"].ToString(),
                                //    LandingSiteText = dr["landing_site_text"].ToString(),
                                //    GearCode = dr["gear_code"].ToString(),
                                //    GearName = dr["gear_name"].ToString(),
                                //    GearText = dr["gear_text"].ToString(),
                                //    GearUnloadBoats = gu_boats,
                                //    GearUnloadCatch = gu_catch,
                                //    UserName = dr["user_name"].ToString(),
                                //    EnumeratorID = en_id,
                                //    EnumeratorName = dr["enumerator_name"].ToString(),
                                //    EnumeratorText = dr["enumerator_text"].ToString(),
                                //    FormVersion = dr["form_version"].ToString().Replace("Version ", ""),
                                //    SamplingDate = (DateTime)dr["sampling_date"],
                                //    DateAdded = (DateTime)dr["date_added"],
                                //    IsSuccess = (bool)dr["is_success"],
                                //    IsTracked = (bool)dr["is_tracked"],
                                //    SectorCode = dr["sector_code"].ToString(),
                                //    HasCatchComposition = (bool)dr["has_catch_composition"],
                                //    IsTripCompleted = (bool)dr["trip_is_completed"],
                                //    CatchCompositionRows = count_catch_comp,
                                //    VesselID = vessel_id,
                                //    VesselName = dr["vessel_name"].ToString(),
                                //    VesselText = dr["vessel_text"].ToString(),
                                //    NumberOfFishers = no_fishers,
                                //    GPSCode = dr["gps"].ToString(),
                                //    CatchMaturityRows = count_maturity,
                                //    FishingGridRows = count_grids,
                                //    GearSoakRows = count_gear_soaks,
                                //    LenFreqRows = count_len_freq,
                                //    LengthRows = count_len,
                                //    LenWtRows = count_len_wt,
                                //    VesselEffortRows = count_efforts,
                                //    DateSubmitted=(DateTime)dr["datetime_submitted"]

                                //};
                                if (string.IsNullOrEmpty(si.EnumeratorText))
                                {
                                    si.EnumeratorText = si.UserName;
                                }
                                items.Add(si);
                                count++;
                                if (count == 21537)
                                {

                                }
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
            return items;
        }
    }
}
