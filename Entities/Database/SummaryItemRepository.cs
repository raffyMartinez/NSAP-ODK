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
                    cmd.CommandText =
                                    @"SELECT 
                                        sd.unload_day_id, 
                                        ls.landing_site_id,  
                                        ls.landing_site_name, 
                                        sd.land_ctr_text AS landing_site_text, 
                                        sd.region_id, 
                                        sd.fma AS fma_id, 
                                        sd.ground_id AS fishing_ground_code, 
                                        gr.gear_code, 
                                        gr.gear_name, 
                                        gu.unload_gr_id,
                                        gu.gr_text AS gear_text, 
                                        gu.boats, 
                                        gu.catch,
                                        vu.v_unload_id, 
                                        fv.vessel_name, 
                                        fv.vessel_id, 
                                        vu.boat_text AS vessel_text, 
                                        vu1.success AS is_success, 
                                        vu1.tracked AS is_tracked, 
                                        vu1.sector_code, 
                                        vu1.datetime_submitted, 
                                        vu1.form_version, 
                                        vu1.enumerator_text, 
                                        en.enumerator_id,
                                        en.enumerator_name, 
                                        vu1.sampling_date,
                                        vu1.date_added, 
                                        vu1.has_catch_composition, 
                                        vu1.trip_is_completed, 
                                        vu1.number_of_fishers AS no_fishers, 
                                        Count(vc.v_unload_id) AS catch_comp_rows
                                        FROM (
                                            nsap_enumerators AS en 
                                            RIGHT JOIN (fishing_vessel AS fv RIGHT JOIN (gears AS gr 
                                            RIGHT JOIN (landing_sites AS ls 
                                            RIGHT JOIN (dbo_lc_fg_sample_day AS sd 
                                            INNER JOIN (dbo_gear_unload AS gu 
                                            INNER JOIN (dbo_vessel_unload AS vu 
                                            INNER JOIN dbo_vessel_unload_1 AS vu1 
                                                ON vu.v_unload_id = vu1.v_unload_id) 
                                                ON gu.unload_gr_id = vu.unload_gr_id) 
                                                ON sd.unload_day_id = gu.unload_day_id) 
                                                ON ls.landing_site_id = sd.land_ctr_id) 
                                                ON gr.gear_code = gu.gr_id) 
                                                ON fv.vessel_id = vu.boat_id) 
                                                ON en.enumerator_id = vu1.enumerator_id) 
                                            LEFT JOIN dbo_vessel_catch AS vc 
                                                ON vu.v_unload_id = vc.v_unload_id
                                        GROUP BY 
                                            sd.unload_day_id, 
                                            ls.landing_site_id, 
                                            ls.landing_site_name, 
                                            sd.land_ctr_text, 
                                            sd.region_id, 
                                            sd.fma, 
                                            sd.ground_id, 
                                            gr.gear_code, 
                                            gr.gear_name, 
                                            gu.unload_gr_id,
                                            gu.gr_text, 
                                            gu.boats, 
                                            gu.catch,
                                            vu.v_unload_id, 
                                            fv.vessel_name, 
                                            fv.vessel_id, 
                                            vu.boat_text, 
                                            vu1.success, 
                                            vu1.tracked, 
                                            vu1.sector_code, 
                                            vu1.datetime_submitted, 
                                            vu1.form_version, 
                                            vu1.enumerator_text, 
                                            en.enumerator_id,
                                            en.enumerator_name, 
                                            vu1.sampling_date,
                                            vu1.date_added, 
                                            vu1.has_catch_composition, 
                                            vu1.trip_is_completed, 
                                            vu1.number_of_fishers";
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
                            if (dr["catch_comp_rows"] != DBNull.Value)
                            {
                                catch_comp_rows = int.Parse(dr["catch_comp_rows"].ToString());
                            }

                            SummaryItem si = new SummaryItem()
                            {

                                ID = ++id,
                                SamplingDayID = (int)dr["unload_day_id"],
                                GearUnloadID = (int)dr["unload_gr_id"],
                                VesselUnloadID = (int)dr["v_unload_id"],
                                RegionID = dr["region_id"].ToString(),
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
                                SamplingDate = (DateTime)dr["sampling_date"],
                                DateAdded = (DateTime)dr["date_added"],
                                IsSuccess = (bool)dr["is_success"],
                                IsTracked = (bool)dr["is_tracked"],
                                SectorCode = dr["sector_code"].ToString(),
                                HasCatchComposition = (bool)dr["has_catch_composition"],
                                IsTripCompleted = (bool)dr["trip_is_completed"],
                                CatchCompositionRows = catch_comp_rows
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
                                            sd.region_id, 
                                            sd.fma AS fma_id, 
                                            sd.ground_id AS fishing_ground_code, 
                                            gr.GearCode AS gear_code, 
                                            gr.GearName AS gear_name, 
                                            gu.unload_gr_id,
                                            gu.gr_text AS gear_text, 
                                            gu.boats, 
                                            gu.catch,
                                            vu.v_unload_id, 
                                            fv.VesselName AS vessel_name, 
                                            fv.VesselID AS vessel_id, 
                                            vu.boat_text AS vessel_text, 
                                            vu1.Success AS is_success, 
                                            vu1.Tracked AS is_tracked, 
                                            vu1.sector_code, 
                                            vu1.datetime_submitted, 
                                            vu1.form_version, 
                                            vu1.EnumeratorText AS enumerator_text, 
                                            en.EnumeratorID AS enumerator_id,
                                            en.EnumeratorName AS enumerator_name, 
                                            vu1.SamplingDate AS sampling_date,
                                            vu1.DateAdded AS date_added, 
                                            vu1.HasCatchComposition AS has_catch_composition, 
                                            vu1.trip_is_completed, 
                                            vu1.NumberOfFishers AS no_fishers, 
                                            Count(vc.v_unload_id) AS catch_comp_rows
                                            FROM (
                                                NSAPEnumerator AS en 
                                                RIGHT JOIN (fishingVessel AS fv RIGHT JOIN (gear AS gr 
                                                RIGHT JOIN (landingSite AS ls 
                                                RIGHT JOIN (dbo_LC_FG_sample_day AS sd 
                                                INNER JOIN (dbo_gear_unload AS gu 
                                                INNER JOIN (dbo_vessel_unload AS vu 
                                                INNER JOIN dbo_vessel_unload_1 AS vu1 
                                                    ON vu.v_unload_id = vu1.v_unload_id) 
                                                    ON gu.unload_gr_id = vu.unload_gr_id) 
                                                    ON sd.unload_day_id = gu.unload_day_id) 
                                                    ON ls.LandingSiteID = sd.land_ctr_id) 
                                                    ON gr.GearCode = gu.gr_id) 
                                                    ON fv.VesselID = vu.boat_id) 
                                                    ON en.EnumeratorID = vu1.EnumeratorID) 
                                                LEFT JOIN dbo_vessel_catch AS vc 
                                                    ON vu.v_unload_id = vc.v_unload_id
                                            GROUP BY 
                                                sd.unload_day_id, 
                                                ls.LandingSiteID, 
                                                ls.LandingSiteName, 
                                                sd.land_ctr_text, 
                                                sd.region_id, 
                                                sd.fma, 
                                                sd.ground_id, 
                                                gr.GearCode, 
                                                gr.GearName, 
                                                gu.unload_gr_id,
                                                gu.gr_text, 
                                                gu.boats, 
                                                gu.catch,
                                                vu.v_unload_id, 
                                                fv.VesselName, 
                                                fv.VesselID, 
                                                vu.boat_text, 
                                                vu1.Success, 
                                                vu1.Tracked, 
                                                vu1.sector_code, 
                                                vu1.datetime_submitted, 
                                                vu1.form_version, 
                                                vu1.EnumeratorText, 
                                                en.EnumeratorID,
                                                en.EnumeratorName, 
                                                vu1.SamplingDate,
                                                vu1.DateAdded, 
                                                vu1.HasCatchComposition, 
                                                vu1.trip_is_completed, 
                                                vu1.NumberOfFishers";
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


                                SummaryItem si = new SummaryItem
                                {
                                    ID = ++id,
                                    SamplingDayID = (int)dr["unload_day_id"],
                                    GearUnloadID = (int)dr["unload_gr_id"],
                                    VesselUnloadID = (int)dr["v_unload_id"],
                                    RegionID = dr["region_id"].ToString(),
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
                                    SamplingDate = (DateTime)dr["sampling_date"],
                                    DateAdded = (DateTime)dr["date_added"],
                                    IsSuccess = (bool)dr["is_success"],
                                    IsTracked = (bool)dr["is_tracked"],
                                    SectorCode = dr["sector_code"].ToString(),
                                    HasCatchComposition = (bool)dr["has_catch_composition"],
                                    IsTripCompleted = (bool)dr["trip_is_completed"],
                                    CatchCompositionRows = (int)dr["catch_comp_rows"]
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
