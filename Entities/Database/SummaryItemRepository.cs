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
                                                 vu1.success AS is_success,
                                                 vu1.tracked AS is_tracked,
                                                 vu1.gps,
                                                 vu1.sector_code,
                                                 vu1.datetime_submitted,
                                                 vu1.form_version,
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
                            };
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
        private List<SummaryItem> getSummaryItems()
        {


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
                                                 gu.unload_gr_id,
                                                 gr.GearCode AS gear_code,
                                                 gr.GearName AS gear_name,
                                                 gu.gr_text AS gear_text,
                                                 gu.boats,
                                                 gu.catch,
                                                 vu.v_unload_id,
                                                 fv.VesselName AS vessel_name,
                                                 fv.VesselID AS vessel_id,
                                                 vu1.SamplingDate AS sampling_date,
                                                 vu.boat_text AS vessel_text,
                                                 vu1.Success AS is_success,
                                                 vu1.Tracked AS is_tracked,
                                                 vu1.GPS,
                                                 vu1.sector_code,
                                                 vu1.datetime_submitted,
                                                 vu1.form_version,
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
                                if(dr["count_catch_composition"] !=DBNull.Value)
                                {
                                    count_catch_comp = (int)dr["count_catch_composition"];
                                }

                                if (dr["count_effort"] != DBNull.Value)
                                {
                                    count_efforts = (int)dr["count_effort"];
                                }

                                if(dr["count_grid"]!=DBNull.Value)
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

                                SummaryItem si = new SummaryItem
                                {
                                    ID = ++id,
                                    SamplingDayID = (int)dr["unload_day_id"],
                                    GearUnloadID = (int)dr["unload_gr_id"],
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    RegionID = dr["region_id"].ToString(),
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
                                    CatchCompositionRows = count_catch_comp,
                                    VesselID = vessel_id,
                                    VesselName = dr["vessel_name"].ToString(),
                                    VesselText = dr["vessel_text"].ToString(),
                                    NumberOfFishers = no_fishers,
                                    GPSCode = dr["gps"].ToString(),
                                    CatchMaturityRows = count_maturity,
                                    FishingGridRows=count_grids,
                                    GearSoakRows=count_gear_soaks,
                                    LenFreqRows=count_len_freq,
                                    LengthRows=count_len,
                                    LenWtRows=count_len_wt,
                                    VesselEffortRows=count_efforts,
                                    
                                };
                                items.Add(si);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return items;
        }
    }
}
