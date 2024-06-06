using NSAP_ODK.TreeViewModelControl;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using RestSharp;

namespace NSAP_ODK.Entities.Database
{
    public class FishingCalendarRepository
    {
        public HashSet<FishingGearAndSector> UniqueGearSectorList { get; set; } = new HashSet<FishingGearAndSector>();

        public Task<List<FishingCalendarDayEx>> GetCalendarDaysAsync(AllSamplingEntitiesEventHandler selectedMonth)
        {
            return Task.Run(() => GetCalendarDays(selectedMonth));
        }

        public List<int>GetVesselUnloadIDsOfCalendar(AllSamplingEntitiesEventHandler selectedMonth)
        {
            List<int> unloadIDs = new List<int>();
            if(Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        DateTime month_start = (DateTime)selectedMonth.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.ToString("MM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).ToString("MM/dd/yyyy"));
                        cmd.Parameters.Add("@nsapRegion", OleDbType.VarChar).Value = selectedMonth.NSAPRegion.Code;
                        cmd.Parameters.AddWithValue("@fma", selectedMonth.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", selectedMonth.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@landing_site", selectedMonth.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");



                        cmd.CommandText = @"SELECT dbo_vessel_unload.v_unload_id
                                            FROM
                                                landingSite INNER JOIN 
                                                (gear RIGHT JOIN 
                                                (((dbo_LC_FG_sample_day LEFT JOIN 
                                                (dbo_gear_unload LEFT JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN 
                                                dbo_LC_FG_sample_day_1 ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) LEFT JOIN 
                                                dbo_vessel_unload_1 ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON 
                                                gear.GearCode = dbo_gear_unload.gr_id) ON 
                                                landingSite.LandingSiteID = dbo_LC_FG_sample_day.land_ctr_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.sdate >= @month_start AND 
                                                dbo_LC_FG_sample_day.sdate < @month_end AND 
                                                dbo_gear_unload.gr_id Is Not Null AND 
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND 
                                                dbo_LC_FG_sample_day.fma=@fma AND 
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND 
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND 
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            ORDER BY 
                                                dbo_LC_FG_sample_day.sdate";
                        try
                        {
                            con.Open();
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                unloadIDs.Add((int)reader["v_unload_id"]);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            return unloadIDs;
        }
        public List<FishingCalendarDayEx> GetCalendarDays(AllSamplingEntitiesEventHandler selectedMonth)
        {
            UniqueGearSectorList.Clear();
            List<FishingCalendarDayEx> days = new List<FishingCalendarDayEx>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@nsapRegion", OleDbType.VarChar).Value = selectedMonth.NSAPRegion.Code;
                        cmd.Parameters.AddWithValue("@fma", selectedMonth.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", selectedMonth.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@landing_site", selectedMonth.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");
                        DateTime month_start = (DateTime)selectedMonth.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.ToString("MM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).ToString("MM/dd/yyyy"));

                        cmd.CommandText = @"SELECT 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.unload_gr_id, 
                                                dbo_LC_FG_sample_day_1.number_gear_types_in_landingsite, 
                                                dbo_LC_FG_sample_day_1.number_landings, 
                                                dbo_LC_FG_sample_day_1.number_landings_sampled, 
                                                dbo_gear_unload.gr_id, 
                                                dbo_LC_FG_sample_day.sampleday, 
                                                dbo_LC_FG_sample_day.has_fishing_operation, 
                                                dbo_LC_FG_sample_day.remarks, 
                                                dbo_vessel_unload_1.sector_code, 
                                                Count(dbo_vessel_unload.v_unload_id) AS count_unloads, 
                                                Sum(dbo_vessel_unload.catch_total) AS sum_catch_total, 
                                                First(dbo_gear_unload.gear_count_municipal) AS count_municipal, 
                                                First(dbo_gear_unload.gear_count_commercial) AS count_commercial, 
                                                First(dbo_gear_unload.gear_catch_municipal) AS catch_municipal, 
                                                First(dbo_gear_unload.gear_catch_commercial) AS catch_commercial
                                            FROM ((
                                                dbo_LC_FG_sample_day LEFT JOIN 
                                                (dbo_gear_unload LEFT JOIN 
                                                dbo_vessel_unload ON 
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON 
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_LC_FG_sample_day_1 ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_LC_FG_sample_day_1.unload_day_id) LEFT JOIN
                                                dbo_vessel_unload_1 ON dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                            GROUP BY 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.unload_gr_id, 
                                                dbo_LC_FG_sample_day_1.number_gear_types_in_landingsite, 
                                                dbo_LC_FG_sample_day_1.number_landings, 
                                                dbo_LC_FG_sample_day_1.number_landings_sampled, 
                                                dbo_gear_unload.gr_id, 
                                                dbo_LC_FG_sample_day.sampleday, 
                                                dbo_LC_FG_sample_day.has_fishing_operation, 
                                                dbo_LC_FG_sample_day.remarks, 
                                                dbo_vessel_unload_1.sector_code, 
                                                dbo_LC_FG_sample_day.region_id, 
                                                dbo_LC_FG_sample_day.fma, 
                                                dbo_LC_FG_sample_day.ground_id, 
                                                dbo_LC_FG_sample_day.land_ctr_id, 
                                                dbo_LC_FG_sample_day.type_of_sampling, 
                                                dbo_LC_FG_sample_day.sdate
                                            HAVING 
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND 
                                                dbo_LC_FG_sample_day.fma=@fma AND 
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND 
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND 
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type AND 
                                                dbo_LC_FG_sample_day.sdate>=@month_start AND 
                                                dbo_LC_FG_sample_day.sdate<@month_end";
                                                //AND dbo_gear_unload.gr_id Is Not Null";
                        try
                        {
                            con.Open();
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                try
                                {
                                    var day = new FishingCalendarDayEx
                                    {
                                        MonthViewModel = selectedMonth,

                                        IsSamplingDay = (bool)reader["sampleday"],
                                        Remarks = reader["remarks"].ToString(),
                                        HasFishingOperation = (bool)reader["has_fishing_operation"],
                                        SamplingDate = (DateTime)reader["sdate"]

                                    };

                                    if (day.HasFishingOperation && reader["unload_gr_id"] != DBNull.Value)
                                    {
                                        day.GearUnload = NSAPEntities.SummaryItemViewModel.GetGearUnload((int)reader["unload_gr_id"]);
                                        day.CountVesselUnloads = (int)reader["count_unloads"];
                                        if (reader["sum_catch_total"] != DBNull.Value)
                                        {
                                            day.TotalWeightOfCatch = (double)reader["sum_catch_total"];
                                        }
                                        if (reader["count_commercial"] != DBNull.Value || reader["count_municipal"] != DBNull.Value)
                                        {
                                            day.CountCommercialLandings = (int)reader["count_commercial"];
                                            day.CountMunicipalLandings = (int)reader["count_municipal"];
                                        }
                                        if (reader["catch_commercial"] != DBNull.Value)
                                        {
                                            day.TotalWeightCommercialLandings = (double)reader["catch_commercial"];
                                        }
                                        if (reader["catch_municipal"] != DBNull.Value)
                                        {
                                            day.TotalWeightMunicipalLandings = (double)reader["catch_municipal"];
                                        }
                                        day.SectorCode = reader["sector_code"].ToString();
                                        if (reader["gr_id"] != DBNull.Value)
                                        {
                                            day.Gear = NSAPEntities.GearViewModel.GetGear(reader["gr_id"].ToString());
                                            FishingGearAndSector fgs = new FishingGearAndSector(day.Gear, day.SectorCode);
                                            if (!UniqueGearSectorList.Contains(fgs))
                                            {
                                                UniqueGearSectorList.Add(fgs);
                                            }
                                        }
                                        if (reader["number_landings"]!=DBNull.Value)
                                        {
                                            day.TotalNumberOfLandings = (int)reader["number_landings"];
                                        }
                                        if (reader["number_landings_sampled"] != DBNull.Value)
                                        {
                                            day.TotalNumberOfLandingsSampled = (int)reader["number_landings_sampled"];
                                        }
                                        
                                        
                                    }
                                    if(day.TotalWeightCommercialLandings==0 && day.TotalWeightMunicipalLandings==0 && day.TotalWeightOfCatch>0)
                                    {
                                        day.TotalWeightCommercialLandings = null;
                                        day.TotalWeightMunicipalLandings = null;
                                    }
                                    days.Add(day);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log(ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return days;
        }
    }
}
