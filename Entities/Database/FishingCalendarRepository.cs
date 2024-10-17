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

        public List<int> GetVesselUnloadIDsOfCalendar(AllSamplingEntitiesEventHandler selectedMonth)
        {
            List<int> unloadIDs = new List<int>();
            if (Global.Settings.UsemySQL)
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
        public List<SpeciesCalendarDay> GetMeasuredSpeciesCalendarDays(
            AllSamplingEntitiesEventHandler selectedMonth,
            CalendarViewType viewType,
            bool getMatureGravidFemaleMaturity = false)
        {
            string m_table = "";
            string where_female = "";
            //string having_female = "";
            string select_maturity = "";
            switch (viewType)
            {
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                    m_table = "dbo_catch_len_freq";
                    break;
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                    m_table = "dbo_catch_len";
                    break;
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                    m_table = "dbo_catch_len_wt";
                    break;
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    m_table = "dbo_catch_maturity";
                    if (getMatureGravidFemaleMaturity)
                    {
                        select_maturity = ", dbo_catch_maturity.maturity";
                        where_female = " AND dbo_catch_maturity.sex='f' AND dbo_catch_maturity.maturity IS NOT NULL";
                        // having_female = " AND dbo_catch_maturity.maturity In ('mt','ri','gr','spw')";
                    }
                    break;
                default:
                    break;
            }
            List<SpeciesCalendarDay> days = new List<SpeciesCalendarDay>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@reg", OleDbType.VarChar).Value = selectedMonth.NSAPRegion.Code;
                        cmd.Parameters.AddWithValue("@ls", selectedMonth.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg", selectedMonth.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@fma", selectedMonth.FMA.FMAID);
                        DateTime month_start = (DateTime)selectedMonth.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", month_start.ToString("MM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@end", month_start.AddMonths(1).ToString("MM/dd/yyyy"));

                        cmd.CommandText = $@"SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id,
                                                Count({m_table}.catch_id) AS n
                                                {select_maturity}
                                             FROM
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                {m_table} ON
                                                dbo_vessel_catch.catch_id = {m_table}.catch_id) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id
                                             WHERE
                                                dbo_vessel_unload_1.is_multigear=False AND
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                                dbo_LC_FG_sample_day.ground_id=@fg AND
                                                dbo_LC_FG_sample_day.fma=@fma
                                                {where_female}
                                             GROUP BY
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                                {select_maturity}
                                             HAVING
                                                dbo_LC_FG_sample_day.sdate>=@start AND
                                                dbo_LC_FG_sample_day.sdate<@end AND
                                                Count({m_table}.catch_id)>0

                                             ORDER BY dbo_LC_FG_sample_day.sdate;

                                             UNION ALL

                                             SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id,
                                                Count({m_table}.catch_id) AS n
                                                {select_maturity}
                                             FROM
                                                (((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                (dbo_vessel_catch INNER JOIN
                                                {m_table} ON
                                                dbo_vessel_catch.catch_id = {m_table}.catch_id) ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id
                                             WHERE
                                                dbo_vessel_unload_1.is_multigear=True AND
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                                dbo_LC_FG_sample_day.ground_id=@fg AND
                                                dbo_LC_FG_sample_day.fma=@fma
                                                {where_female}
                                             GROUP BY
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                                {select_maturity}
                                             HAVING
                                                dbo_LC_FG_sample_day.sdate>=@start AND
                                                dbo_LC_FG_sample_day.sdate<@end AND
                                                Count({m_table}.catch_id)>0";

                        con.Open();

                        var qryText = cmd.CommandText
                        .Replace("@reg", $"'{selectedMonth.NSAPRegion.Code}'")
                        .Replace("@ls", $"{selectedMonth.LandingSite.LandingSiteID}")
                        .Replace("@fg", $"'{selectedMonth.FishingGround.Code}'")
                        .Replace("@fma", $"{selectedMonth.FMA.FMAID}")
                        .Replace("@start", $"#{month_start.ToString("M/d/yyyy")}#")
                        .Replace("@end", $"#{month_start.AddMonths(1).ToString("M/d/yyyy")}#");
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                SpeciesCalendarDay scd = new SpeciesCalendarDay
                                {
                                    MonthViewModel = selectedMonth,
                                    TaxaCode = dr["taxa"].ToString(),
                                    SamplingDate = (DateTime)dr["sdate"],
                                    GearCode = dr["gr_id"].ToString(),
                                    SectorCode = dr["sector_code"].ToString(),
                                };
                                if (dr["species_id"] != DBNull.Value)
                                {
                                    scd.SpeciesID = (int)dr["species_id"];

                                    int counts = (int)dr["n"];

                                    switch (viewType)
                                    {
                                        case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                                            scd.CountLenFreqMeas = counts;
                                            break;
                                        case CalendarViewType.calendarViewTypeLengthMeasurement:
                                            scd.CountLenMeas = counts;
                                            break;
                                        case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                                            scd.CountLenWtMeas = counts;
                                            break;
                                        case CalendarViewType.calendarViewTypeMaturityMeasurement:
                                            scd.CountMaturityMeas = counts;
                                            if (getMatureGravidFemaleMaturity)
                                            {
                                                scd.MaturityStageCode = dr["maturity"].ToString();
                                                scd.MaturityStageEnum = CatchMaturity.GetStageFromCode(scd.MaturityStageCode);
                                            }
                                            break;
                                    }
                                    days.Add(scd);
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
        public List<SpeciesCalendarDay> GetMeasuredWatchedSpeciesCalendarDays(AllSamplingEntitiesEventHandler selectedMonth)
        {
            List<SpeciesCalendarDay> speciesCalendarDays = new List<SpeciesCalendarDay>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.Add("@reg", OleDbType.VarChar).Value = selectedMonth.NSAPRegion.Code;
                        cmd.Parameters.AddWithValue("@ls", selectedMonth.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fg", selectedMonth.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@fma", selectedMonth.FMA.FMAID);
                        DateTime month_start = (DateTime)selectedMonth.MonthSampled;
                        cmd.Parameters.AddWithValue("@start", month_start.ToString("MM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@end", month_start.AddMonths(1).ToString("MM/dd/yyyy"));

                        cmd.CommandText = @"SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id,
                                                Count(dbo_catch_len.catch_len_id) AS CountOfcatch_len_id,
                                                Count(dbo_catch_len_wt.catch_len_wt_id) AS CountOfcatch_len_wt_id,
                                                Count(dbo_catch_len_freq.catch_len_freq_id) AS CountOfcatch_len_freq_id,
                                                Count(dbo_catch_maturity.catch_maturity_id) AS CountOfcatch_maturity_id
                                            FROM
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) INNER JOIN
                                                (((((dbo_vessel_catch INNER JOIN
                                                dbo_region_watched_species ON
                                                dbo_vessel_catch.species_id = dbo_region_watched_species.sp_id) LEFT JOIN
                                                dbo_catch_len_freq ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) LEFT JOIN
                                                dbo_catch_len_wt ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) LEFT JOIN
                                                dbo_catch_len ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) LEFT JOIN
                                                dbo_catch_maturity ON
                                                dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_catch.v_unload_id
                                            WHERE
                                                dbo_vessel_unload_1.is_multigear=False AND
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                                dbo_LC_FG_sample_day.ground_id=@fg AND
                                                dbo_LC_FG_sample_day.fma=@fma
                                            GROUP BY
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            HAVING
                                                dbo_LC_FG_sample_day.sdate>=@start AND
                                                dbo_LC_FG_sample_day.sdate<@end

                                            UNION ALL

                                            SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id,
                                                Count(dbo_catch_len.catch_len_id) AS CountOfcatch_len_id,
                                                Count(dbo_catch_len_wt.catch_len_wt_id) AS CountOfcatch_len_wt_id,
                                                Count(dbo_catch_len_freq.catch_len_freq_id) AS CountOfcatch_len_freq_id,
                                                Count(dbo_catch_maturity.catch_maturity_id) AS CountOfcatch_maturity_id
                                            FROM
                                                ((((((((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                dbo_vessel_catch ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) LEFT JOIN
                                                dbo_catch_len ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len.catch_id) LEFT JOIN 
                                                dbo_catch_len_wt ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_wt.catch_id) INNER JOIN
                                                dbo_region_watched_species ON
                                                dbo_vessel_catch.species_id = dbo_region_watched_species.sp_id) LEFT JOIN 
                                                dbo_catch_len_freq ON
                                                dbo_vessel_catch.catch_id = dbo_catch_len_freq.catch_id) LEFT JOIN 
                                                dbo_catch_maturity ON
                                                dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id
                                            WHERE
                                                dbo_vessel_unload_1.is_multigear=True AND
                                                dbo_LC_FG_sample_day.region_id=@reg AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@ls AND
                                                dbo_LC_FG_sample_day.ground_id=@fg AND
                                                dbo_LC_FG_sample_day.fma=@fma
                                            GROUP BY
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                            HAVING
                                                dbo_LC_FG_sample_day.sdate>=@start AND
                                                dbo_LC_FG_sample_day.sdate<@end";

                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                SpeciesCalendarDay scd = new SpeciesCalendarDay
                                {
                                    MonthViewModel = selectedMonth,
                                    SpeciesID = (int)dr["species_id"],
                                    TaxaCode = dr["taxa"].ToString(),
                                    SamplingDate = (DateTime)dr["sdate"],
                                    GearCode = dr["gr_id"].ToString(),
                                    SectorCode = dr["sector_code"].ToString(),
                                    CountLenFreqMeas = (int)dr["CountOfcatch_len_freq_id"],
                                    CountLenMeas = (int)dr["CountOfcatch_len_id"],
                                    CountLenWtMeas = (int)dr["CountOfcatch_len_wt_id"],
                                    CountMaturityMeas = (int)dr["CountOfcatch_maturity_id"],
                                };
                                if (scd.CountLenFreqMeas + scd.CountLenMeas + scd.CountLenWtMeas + scd.CountMaturityMeas > 0)
                                {
                                    speciesCalendarDays.Add(scd);
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
            return speciesCalendarDays;
        }

        public List<SpeciesCalendarDay> GetWatchedSpeciesCalendarDays(AllSamplingEntitiesEventHandler selectedMonth)
        {
            List<SpeciesCalendarDay> speciesCalendarDays = new List<SpeciesCalendarDay>();
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
                        cmd.Parameters.AddWithValue("@landing_site", selectedMonth.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@fishing_ground", selectedMonth.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@fma", selectedMonth.FMA.FMAID);
                        DateTime month_start = (DateTime)selectedMonth.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.ToString("MM/dd/yyyy"));
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).ToString("MM/dd/yyyy"));

                        cmd.CommandText = @" SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id,
                                                Sum(dbo_vessel_catch.catch_kg) AS SumOfcatch_kg,
                                                Count(dbo_vessel_unload.v_unload_id) AS CountOfv_unload_id
                                             FROM
                                                (dbo_vessel_catch INNER JOIN
                                                dbo_region_watched_species ON
                                                dbo_vessel_catch.species_id = dbo_region_watched_species.sp_id) INNER JOIN
                                                ((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) ON
                                                dbo_vessel_catch.v_unload_id = dbo_vessel_unload.v_unload_id
                                             WHERE
                                                dbo_vessel_unload_1.is_multigear=FALSE AND
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                dbo_LC_FG_sample_day.land_ctr_id =@landing_site  AND
                                                dbo_LC_FG_sample_day.ground_id =@fishing_ground AND
                                                dbo_LC_FG_sample_day.fma = @fma AND
                                                dbo_region_watched_species.region_code = @nsapRegion
                                             GROUP BY 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                             HAVING
                                                dbo_LC_FG_sample_day.sdate >= @month_start AND
                                                dbo_LC_FG_sample_day.sdate < @month_end

                                             UNION ALL

                                             SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id,
                                                Sum(dbo_vessel_catch.catch_kg) AS SumOfcatch_kg,
                                                Count(dbo_vessel_unload.v_unload_id) AS CountOfv_unload_id
                                             FROM
                                                (((((dbo_LC_FG_sample_day INNER JOIN
                                                dbo_gear_unload ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                dbo_vessel_catch ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) INNER JOIN
                                                dbo_region_watched_species ON
                                                dbo_vessel_catch.species_id = dbo_region_watched_species.sp_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id
                                             WHERE
                                                dbo_vessel_unload_1.is_multigear=TRUE AND
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                dbo_LC_FG_sample_day.land_ctr_id =@landing_site  AND
                                                dbo_LC_FG_sample_day.ground_id =@fishing_ground AND
                                                dbo_LC_FG_sample_day.fma = @fma AND
                                                dbo_region_watched_species.region_code = @nsapRegion
                                             GROUP BY 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_gear_unload.gr_id,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_vessel_catch.species_id
                                             HAVING
                                                dbo_LC_FG_sample_day.sdate >= @month_start AND
                                                dbo_LC_FG_sample_day.sdate < @month_end";

                        con.Open();
                        try
                        {
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                SpeciesCalendarDay scd = new SpeciesCalendarDay
                                {
                                    MonthViewModel = selectedMonth,
                                    SpeciesID = (int)dr["species_id"],
                                    TaxaCode = dr["taxa"].ToString(),
                                    SamplingDate = (DateTime)dr["sdate"],
                                    NumberOfLandingsOfSpecies = (int)dr["CountOfv_unload_id"],
                                    WeightOfSpeciesLanded = (double)dr["SumOfcatch_kg"],
                                    GearCode = dr["gr_id"].ToString(),
                                    SectorCode = dr["sector_code"].ToString()
                                };
                                speciesCalendarDays.Add(scd);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                }
            }
            return speciesCalendarDays;
        }
        public List<FishingCalendarDayEx> GetCalendarDays(AllSamplingEntitiesEventHandler selectedMonth)
        {
            Logger.LogCalendar("start FishingCalendarRepository.GetCalendarDays()");
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

                        string qry = cmd.CommandText
                            .Replace("@nsapRegion", $"'{selectedMonth.NSAPRegion.Code}'")
                            .Replace("@fma", selectedMonth.FMA.FMAID.ToString())
                            .Replace("@fishing_ground", $"'{selectedMonth.FishingGround.Code}'")
                            .Replace("@landing_site", selectedMonth.LandingSite.LandingSiteID.ToString())
                            .Replace("@sampling_type", "'rs'")
                            .Replace("@month_start", $"#{month_start.ToString("MM/d/yyyy")}#")
                            .Replace("@month_end", $"#{month_start.AddMonths(1).ToString("MM/d/yyyy")}#");

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
                                        if (reader["number_landings"] != DBNull.Value)
                                        {
                                            day.TotalNumberOfLandings = (int)reader["number_landings"];
                                        }
                                        if (reader["number_landings_sampled"] != DBNull.Value)
                                        {
                                            day.TotalNumberOfLandingsSampled = (int)reader["number_landings_sampled"];
                                        }


                                    }
                                    if (day.TotalWeightCommercialLandings == 0 && day.TotalWeightMunicipalLandings == 0 && day.TotalWeightOfCatch > 0)
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
            Logger.LogCalendar($"Fishing calendar days created with {days.Count} in list");
            Logger.LogCalendar("end FishingCalendarRepository.GetCalendarDays()");
            return days;
        }
    }
}
