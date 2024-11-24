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
    public class GettingCalendarEventArgs : EventArgs
    {
        public string Context { get; set; }
    }
    public class CalendarMonthRepository
    {
        public event EventHandler<GettingCalendarEventArgs> GettingCalendar;

        private static HashSet<string> _maturityMeasurementsFemale = new HashSet<string>();
        private static HashSet<string> _lenMeasurements = new HashSet<string>();
        private static HashSet<string> _lenWtMeasurements = new HashSet<string>();
        private static HashSet<string> _lenFreqMeasurements = new HashSet<string>();
        private static HashSet<string> _maturityMeasurements = new HashSet<string>();
        private static HashSet<string> _watchedSpeciesWeights = new HashSet<string>();
        public static Dictionary<string, List<CalendarGearSector>> CalendarGearSectorDictionary { get; private set; } = new Dictionary<string, List<CalendarGearSector>>();
        public List<CalendarGearSector> CalendarGearSectors { get; private set; }

        public static List<int> SamplingRestDays { get; private set; }
        public CalendarDailySummary CalendarDailySummary { get; private set; }
        public int TotalLandingsSampled { get; private set; }
        public int TotalCountAllLandedGears { get; private set; }

        public int TotalCountMeasurements { get; private set; }
        public int RowCount { get; private set; }

        public int TotalLandingCount { get; private set; }
        public double TotalWeightLanded { get; private set; }

        public CalendarViewType ViewOption { get; private set; }

        public bool CalendarHasData { get; private set; }
        //public CalendarMonthRepository(AllSamplingEntitiesEventHandler e, CalendarViewType viewOption, bool isFemaleMaturity = false)
        public CalendarMonthRepository()
        {
            CalendarHasData = false;
            //ViewOption = viewOption;

            TotalLandingsSampled = 0;
            TotalLandedCatchWeight = 0;
            TotalVesselUnloadCount = 0;
            TotalCountAllLandedGears = 0;
            TotalLandingCount = 0;
            TotalWeightLanded = 0;
            TotalCountMeasurements = 0;
            RowCount = 0;

            //switch (ViewOption)
            //{
            //    case CalendarViewType.calendarViewTypeSampledLandings:
            //    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
            //        if (!CalendarGearSectorDictionary.Keys.Contains(e.GUID))
            //        {
            //            CalendarGearSectorDictionary.Add(e.GUID, GetCalendarGearSectors(e));
            //        }
            //        CalendarGearSectors = CalendarGearSectorDictionary[e.GUID];


            //        foreach (var sector in CalendarGearSectors)
            //        {
            //            TotalLandingsSampled += sector.CalendarDays.Sum(t => t.CountOfFishingOperations);
            //            TotalLandedCatchWeight += sector.CalendarDays.Sum(t => t.TotalWeightOfCatch);
            //        }
            //        CalendarHasData = TotalLandingsSampled > 0 || TotalLandedCatchWeight > 0;

            //        if (ViewOption == CalendarViewType.calendarViewTypeSampledLandings)
            //        {
            //            SamplingRestDays = new List<int>();
            //            if (CalendarHasData)
            //            {
            //                RowCount = CalendarGearSectors.Where(t => t.NoFishing == false).Count();


            //                HashSet<int> restDays = new HashSet<int>();
            //                SamplingRestDays.Clear();
            //                try
            //                {
            //                    foreach (var gs in CalendarGearSectors)
            //                    {
            //                        foreach (var day in gs.CalendarDays)
            //                        {
            //                            if (!day.IsSamplingDay)
            //                            {
            //                                restDays.Add(day.Day.Day);
            //                            }
            //                        }
            //                    }
            //                }
            //                catch
            //                {
            //                    //ignore
            //                }
            //                if (restDays.Count > 0)
            //                {
            //                    SamplingRestDays.AddRange(restDays.ToList());
            //                }
            //            }
            //        }
            //        break;
            //    case CalendarViewType.calendarViewTypeCountAllLandings:
            //    case CalendarViewType.calendarViewTypeWeightAllLandings:

            //        CalendarDailySummary = GetCalendarDailySummary(e);

            //        TotalLandingCount = CalendarDailySummary.CalendarDays.Sum(t => t.CountOfFishingOperations);
            //        TotalWeightLanded = CalendarDailySummary.CalendarDays.Sum(t => t.TotalWeightOfCatch);
            //        CalendarHasData = TotalLandingCount > 0 || TotalWeightLanded > 0;
            //        if (CalendarHasData)
            //        {
            //            RowCount = 1;
            //        }
            //        break;
            //    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
            //        var cgs = CalendarGearSectorDictionary[e.GUID];
            //        GetDailyTotalLandings(e, ref cgs);
            //        CalendarGearSectors = cgs;
            //        foreach (var sector in CalendarGearSectors)
            //        {
            //            TotalCountAllLandedGears += sector.CalendarDays.Sum(t => t.CountNumberOfLandings);
            //        }
            //        CalendarHasData = TotalCountAllLandedGears > 0;
            //        if (CalendarHasData)
            //        {
            //            RowCount = CalendarGearSectors.Where(t => t.NoFishing == false).Count();
            //        }
            //        break;
            //    case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
            //    case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
            //        cgs = CalendarGearSectorDictionary[e.GUID];
            //        GetDailyWatchedSpecies(e, ref cgs);
            //        CalendarGearSectors = cgs;

            //        species_gear_sector = new HashSet<string>();

            //        foreach (var sector in CalendarGearSectors.Where(t => t.NoFishing == false))
            //        {
            //            foreach (var day in sector.CalendarDays.Where(t => t.CalendarDaySpecieses != null))
            //            {
            //                foreach (var item in day.CalendarDaySpecieses)
            //                {
            //                    species_gear_sector.Add($"{item.SpeciesName}-{sector.Gear}-{sector.SectorCode}");
            //                }
            //            }
            //        }
            //        RowCount = species_gear_sector.Count();
            //        CalendarHasData = RowCount > 0;
            //        break;
            //    case CalendarViewType.calendarViewTypeLengthMeasurement:
            //    case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
            //    case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
            //    case CalendarViewType.calendarViewTypeMaturityMeasurement:

            //        var measurementType = GetMeasurementType(viewOption);

            //        cgs = CalendarGearSectorDictionary[e.GUID];
            //        GetMeasuredWatchedSpecies(e, ref cgs, isFemaleMaturity);
            //        CalendarGearSectors = cgs;

            //        species_gear_sector = new HashSet<string>();

            //        foreach (var sector in CalendarGearSectors.Where(t => t.NoFishing == false))
            //        {
            //            if (isFemaleMaturity)
            //            {
            //                foreach (var day in sector.CalendarDays.Where(t => t.CalendarDaySpeciesesMeasuredFemale != null))
            //                {
            //                    foreach (var item in day.CalendarDaySpeciesesMeasuredFemale)
            //                    {
            //                        species_gear_sector.Add($"{item.SpeciesName}-{sector.Gear}-{sector.SectorCode}-{item.MaturityStage}");
            //                        TotalCountMeasurements += item.CountMeasured;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (var day in sector.CalendarDays.Where(t => t.CalendarDaySpeciesesMeasured != null))
            //                {
            //                    foreach (var item in day.CalendarDaySpeciesesMeasured.Where(t => t.MeasurementType == measurementType))
            //                    {
            //                        species_gear_sector.Add($"{item.SpeciesName}-{sector.Gear}-{sector.SectorCode}");
            //                        TotalCountMeasurements += item.CountMeasured;
            //                    }
            //                }
            //            }
            //        }
            //        RowCount = species_gear_sector.Count();
            //        CalendarHasData = RowCount > 0;
            //        break;
            //}

        }

        public async Task GetCalendarsAsync(AllSamplingEntitiesEventHandler e, CalendarViewType viewOption, bool isFemaleMaturity = false)
        {
            await Task.Run(() => GetCalendars(e, viewOption, isFemaleMaturity));
        }
        public void GetCalendars(AllSamplingEntitiesEventHandler e, CalendarViewType viewOption, bool isFemaleMaturity = false)
        {
            GettingCalendar?.Invoke(null, new GettingCalendarEventArgs { Context = "fetching" });
            HashSet<string> species_gear_sector;
            ViewOption = viewOption;

            switch (ViewOption)
            {
                case CalendarViewType.calendarViewTypeSampledLandings:
                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                    if (!CalendarGearSectorDictionary.Keys.Contains(e.GUID))
                    {
                        CalendarGearSectorDictionary.Add(e.GUID, GetCalendarGearSectors(e));
                    }
                    CalendarGearSectors = CalendarGearSectorDictionary[e.GUID];


                    foreach (var sector in CalendarGearSectors)
                    {
                        TotalLandingsSampled += sector.CalendarDays.Sum(t => t.CountOfFishingOperations);
                        TotalLandedCatchWeight += sector.CalendarDays.Sum(t => t.TotalWeightOfCatch);
                    }
                    CalendarHasData = TotalLandingsSampled > 0 || TotalLandedCatchWeight > 0;

                    if (ViewOption == CalendarViewType.calendarViewTypeSampledLandings)
                    {
                        SamplingRestDays = new List<int>();
                        if (CalendarHasData)
                        {
                            RowCount = CalendarGearSectors.Where(t => t.NoFishing == false).Count();


                            HashSet<int> restDays = new HashSet<int>();
                            SamplingRestDays.Clear();
                            try
                            {
                                foreach (var gs in CalendarGearSectors)
                                {
                                    foreach (var day in gs.CalendarDays)
                                    {
                                        if (!day.IsSamplingDay)
                                        {
                                            restDays.Add(day.Day.Day);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                //ignore
                            }
                            if (restDays.Count > 0)
                            {
                                SamplingRestDays.AddRange(restDays.ToList());
                            }
                        }
                    }
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandings:
                case CalendarViewType.calendarViewTypeWeightAllLandings:

                    CalendarDailySummary = GetCalendarDailySummary(e);

                    TotalLandingCount = CalendarDailySummary.CalendarDays.Sum(t => t.CountOfFishingOperations);
                    TotalWeightLanded = CalendarDailySummary.CalendarDays.Sum(t => t.TotalWeightOfCatch);
                    CalendarHasData = TotalLandingCount > 0 || TotalWeightLanded > 0;
                    if (CalendarHasData)
                    {
                        RowCount = 1;
                    }
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                    var cgs = CalendarGearSectorDictionary[e.GUID];
                    GetDailyTotalLandings(e, ref cgs);
                    CalendarGearSectors = cgs;
                    foreach (var sector in CalendarGearSectors)
                    {
                        TotalCountAllLandedGears += sector.CalendarDays.Sum(t => t.CountNumberOfLandings);
                    }
                    CalendarHasData = TotalCountAllLandedGears > 0;
                    if (CalendarHasData)
                    {
                        RowCount = CalendarGearSectors.Where(t => t.NoFishing == false).Count();
                    }
                    break;
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                    cgs = CalendarGearSectorDictionary[e.GUID];
                    GetDailyWatchedSpecies(e, ref cgs);
                    CalendarGearSectors = cgs;

                    species_gear_sector = new HashSet<string>();

                    foreach (var sector in CalendarGearSectors.Where(t => t.NoFishing == false))
                    {
                        foreach (var day in sector.CalendarDays.Where(t => t.CalendarDaySpecieses != null))
                        {
                            foreach (var item in day.CalendarDaySpecieses)
                            {
                                species_gear_sector.Add($"{item.SpeciesName}-{sector.Gear}-{sector.SectorCode}");
                                TotalLandedCatchWeight += item.WeightLanded;
                            }
                        }
                    }
                    RowCount = species_gear_sector.Count();
                    CalendarHasData = RowCount > 0;
                    break;
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                case CalendarViewType.calendarViewTypeMaturityMeasurement:

                    var measurementType = GetMeasurementType(viewOption);

                    cgs = CalendarGearSectorDictionary[e.GUID];
                    GetMeasuredWatchedSpecies(e, ref cgs, isFemaleMaturity);
                    //CalendarGearSectors = cgs;
                    CalendarGearSectors = GetCalendarGearSectorsFilteredByMeasurement(cgs);

                    species_gear_sector = new HashSet<string>();

                    foreach (var sector in CalendarGearSectors.Where(t => t.NoFishing == false))
                    {
                        if (isFemaleMaturity)
                        {
                            foreach (var day in sector.CalendarDays.Where(t => t.CalendarDaySpeciesesMeasuredFemale != null))
                            {
                                foreach (var item in day.CalendarDaySpeciesesMeasuredFemale)
                                {
                                    species_gear_sector.Add($"{item.SpeciesName}-{sector.Gear}-{sector.SectorCode}-{item.MaturityStage}");
                                    TotalCountMeasurements += item.CountMeasured;
                                }
                            }
                        }
                        else
                        {
                            foreach (var day in sector.CalendarDays.Where(t => t.CalendarDaySpeciesesMeasured != null))
                            {
                                foreach (var item in day.CalendarDaySpeciesesMeasured.Where(t => t.MeasurementType == measurementType))
                                {
                                    species_gear_sector.Add($"{item.SpeciesName}-{sector.Gear}-{sector.SectorCode}");
                                    TotalCountMeasurements += item.CountMeasured;
                                }
                            }
                        }
                    }
                    RowCount = species_gear_sector.Count();
                    CalendarHasData = RowCount > 0;
                    break;
            }
            GettingCalendar?.Invoke(null, new GettingCalendarEventArgs { Context = "fetching done" });
        }

        public string GetMeasurementType(CalendarViewType viewType)
        {
            string m_type = "";
            switch (viewType)
            {
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                    m_type = "len";
                    break;
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                    m_type = "len_freq";
                    break;
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                    m_type = "len_wt";
                    break;
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    m_type = "mat";
                    break;
            }
            return m_type;
        }

        private List<CalendarGearSector> GetCalendarGearSectorsFilteredByMeasurement(List<CalendarGearSector> cgs)
        {
            List<CalendarGearSector> list = new List<CalendarGearSector>();
            string m_type = GetMeasurementType(ViewOption);
            bool break_loop = false;
            foreach (CalendarGearSector item in cgs.Where(t => t.NoFishing == false))
            {
                foreach (CalendarDay day in item.CalendarDays.Where(t => t.CalendarDaySpeciesesMeasured != null))
                {
                    foreach (CalendarDaySpeciesMeasured spm in day.CalendarDaySpeciesesMeasured.Where(t => t.MeasurementType == m_type))
                    {
                        if (spm != null)
                        {
                            break_loop = true;
                            break;
                        }
                    }
                }
                if (break_loop)
                {
                    break_loop = false;
                    list.Add(item);


                }
            }
            return list;
        }
        private static void AddToMeasurement(AllSamplingEntitiesEventHandler e, CalendarViewType viewType, bool isFemaleMeasurement = false)
        {
            switch (viewType)
            {
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                    _watchedSpeciesWeights.Add(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                    _lenMeasurements.Add(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                    _lenWtMeasurements.Add(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                    _lenFreqMeasurements.Add(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    if (isFemaleMeasurement)
                    {
                        _maturityMeasurementsFemale.Add(e.GUID);
                    }
                    else
                    {
                        _maturityMeasurements.Add(e.GUID);
                    }
                    break;
            }
        }
        private static bool MeasurementExist(AllSamplingEntitiesEventHandler e, CalendarViewType viewType, bool isFemaleMeasurement = false)
        {
            bool exist = false;
            switch (viewType)
            {
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                    exist = _watchedSpeciesWeights.Contains(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                    exist = _lenMeasurements.Contains(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                    exist = _lenWtMeasurements.Contains(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                    exist = _lenFreqMeasurements.Contains(e.GUID);
                    break;
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    if (isFemaleMeasurement)
                    {
                        exist = _maturityMeasurementsFemale.Contains(e.GUID);
                    }
                    else
                    {
                        exist = _maturityMeasurements.Contains(e.GUID);
                    }
                    break;
            }
            return exist;
        }
        private void GetMeasuredWatchedSpecies(AllSamplingEntitiesEventHandler e, ref List<CalendarGearSector> lcgs, bool isFemaleMaturity = false)
        {
            string expanded_query = "";
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        string measurement_type = GetMeasurementType(ViewOption);
                        string table_name = "";
                        switch (ViewOption)
                        {
                            case CalendarViewType.calendarViewTypeLengthMeasurement:
                                table_name = "dbo_catch_len";
                                break;
                            case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                                table_name = "dbo_catch_len_wt";
                                break;
                            case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                                table_name = "dbo_catch_len_freq";
                                break;
                            case CalendarViewType.calendarViewTypeMaturityMeasurement:
                                table_name = "dbo_catch_maturity";
                                if (isFemaleMaturity)
                                {
                                    cmd.Parameters.AddWithValue("@sex_f", "f");
                                }
                                break;
                        }
                        cmd.Parameters.AddWithValue("@nsapRegion", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@landing_site", e.LandingSite.LandingSiteID);


                        cmd.Parameters.AddWithValue("@sampling_type", "rs");

                        DateTime month_start = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.Date);
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).Date);

                        if (isFemaleMaturity)
                        {
                            cmd.CommandText = @"SELECT
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_region_watched_species.sp_id,
                                                [phFish].[Genus] & ' ' & [phFish].[Species] AS fish_species,
                                                [notFishSpecies].[Genus] & ' ' & [notFishSpecies].[Species] AS not_fish_species,
                                                dbo_catch_maturity.maturity AS stage,
                                                Count(dbo_catch_maturity.maturity) AS n
                                            FROM
                                                ((((dbo_LC_FG_sample_day INNER JOIN
                                                (dbo_gear_unload INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                (((dbo_vessel_catch INNER JOIN
                                                dbo_region_watched_species ON
                                                dbo_vessel_catch.species_id = dbo_region_watched_species.sp_id) LEFT JOIN phFish ON
                                                dbo_region_watched_species.sp_id = phFish.SpeciesID) LEFT JOIN notFishSpecies ON
                                                dbo_region_watched_species.sp_id = notFishSpecies.SpeciesID) ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) INNER JOIN
                                                dbo_catch_maturity ON
                                                dbo_vessel_catch.catch_id = dbo_catch_maturity.catch_id
                                            WHERE
                                                dbo_catch_maturity.sex = @sex_f AND
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                dbo_region_watched_species.region_code=@nsapRegion AND
                                                dbo_LC_FG_sample_day.fma=@fma AND
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            GROUP BY
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_region_watched_species.sp_id,
                                                [phFish].[Genus] & ' ' & [phFish].[Species],
                                                [notFishSpecies].[Genus] & ' ' & [notFishSpecies].[Species],
                                                dbo_catch_maturity.maturity
                                            HAVING
                                                dbo_LC_FG_sample_day.sdate >=@month_start AND
                                                dbo_LC_FG_sample_day.sdate <@month_end
                                            ORDER BY 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code";
                        }
                        else
                        {
                            cmd.CommandText = $@"SELECT
                                                 dbo_LC_FG_sample_day.sdate,
                                                 dbo_vesselunload_fishinggear.gear_code,
                                                 dbo_vessel_unload_1.sector_code,
                                                 dbo_vessel_catch.taxa,
                                                 dbo_region_watched_species.sp_id,
                                                 Count({table_name}.catch_id) AS n,
                                                 [phFish].[Genus] & ' ' & [phFish].[Species] AS fish_species,
                                                 [notFishSpecies].[Genus] & ' ' & [notFishSpecies].[Species] AS not_fish_species
                                            FROM
 
                                                ((((((((dbo_LC_FG_sample_day INNER JOIN
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
                                                {table_name} ON
                                                    dbo_vessel_catch.catch_id = {table_name}.catch_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                    dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) LEFT JOIN phFish ON
                                                    dbo_region_watched_species.sp_id = phFish.SpeciesID) LEFT JOIN notFishSpecies ON
                                                    dbo_region_watched_species.sp_id = notFishSpecies.SpeciesID
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                dbo_region_watched_species.region_code=@nsapRegion AND
                                                dbo_LC_FG_sample_day.fma=@fma AND
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            GROUP BY
 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_vessel_catch.taxa,
                                                dbo_region_watched_species.sp_id,
                                                [phFish].[Genus] & ' ' & [phFish].[Species],
                                                [notFishSpecies].[Genus] & ' ' & [notFishSpecies].[Species]
                                            HAVING
                                                dbo_LC_FG_sample_day.sdate >=@month_start AND
                                                dbo_LC_FG_sample_day.sdate<@month_end
                                            ORDER BY 
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code";


                        }

                        expanded_query = cmd.CommandText.Replace("@nsapRegion", $"'{e.NSAPRegion.Code}'")
                                .Replace("@fma", e.FMA.FMAID.ToString())
                                .Replace("@fishing_ground", $"'{e.FishingGround.Code}'")
                                .Replace("@landing_site", $"{e.LandingSite.LandingSiteID.ToString()}")
                                .Replace("@sampling_type", "'rs'")
                                .Replace("@month_start", $"#{month_start.ToString("MM/d/yyyy")}#")
                                .Replace("@month_end", $"#{month_start.AddMonths(1).ToString("MM/d/yyyy")}#");

                        if (isFemaleMaturity)
                        {
                            expanded_query = expanded_query.Replace("@sex_f", "'f'");
                        }

                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                try
                                {
                                    CalendarDay day;
                                    string gear_code = dr["gear_code"].ToString();
                                    DateTime sampling_date = (DateTime)dr["sdate"];
                                    FishSpecies fishSpecies = null;
                                    NotFishSpecies notFishSpecies = null;
                                    CalendarDaySpeciesMeasured cdsm;
                                    CalendarDaySpeciesMeasured cdsmf;
                                    string taxaCode = (string)dr["taxa"];
                                    if (taxaCode == "FIS")
                                    {
                                        fishSpecies = NSAPEntities.FishSpeciesViewModel.GetSpecies((int)dr["sp_id"]);
                                    }
                                    else
                                    {
                                        notFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)dr["sp_id"]);
                                    }

                                    var cgs = lcgs.Find(t => t.Gear != null && t.Gear.Code == gear_code && t.SectorCode != null && t.SectorCode == "m");
                                    if (cgs != null)
                                    {
                                        day = cgs.CalendarDays.Find(t => t.Day == sampling_date);

                                        if (day != null)
                                        {
                                            if (isFemaleMaturity)
                                            {
                                                if (!MeasurementExist(e, ViewOption, isFemaleMaturity))
                                                {
                                                    cdsmf = new CalendarDaySpeciesMeasured(day)
                                                    {
                                                        FishSpecies = fishSpecies,
                                                        NotFishSpecies = notFishSpecies,
                                                        CountMeasured = (int)dr["n"],
                                                        MeasurementType = measurement_type,
                                                        Taxa_code = taxaCode,
                                                        IsFemaleMaturity = true,
                                                        MaturityStage = (string)dr["stage"]
                                                    };

                                                    if (day.CalendarDaySpeciesesMeasuredFemale == null)
                                                    {
                                                        day.CalendarDaySpeciesesMeasuredFemale = new List<CalendarDaySpeciesMeasured>();
                                                    }
                                                    day.CalendarDaySpeciesesMeasuredFemale.Add(cdsmf);

                                                }
                                            }
                                            else
                                            {
                                                if (!MeasurementExist(e, ViewOption))
                                                {
                                                    cdsm = new CalendarDaySpeciesMeasured(day)
                                                    {

                                                        FishSpecies = fishSpecies,
                                                        NotFishSpecies = notFishSpecies,
                                                        CountMeasured = (int)dr["n"],
                                                        MeasurementType = measurement_type,
                                                        Taxa_code = taxaCode
                                                    };
                                                    if (day.CalendarDaySpeciesesMeasured == null)
                                                    {
                                                        day.CalendarDaySpeciesesMeasured = new List<CalendarDaySpeciesMeasured>();
                                                    }
                                                    day.CalendarDaySpeciesesMeasured.Add(cdsm);


                                                }
                                            }
                                        }
                                    }

                                    cgs = lcgs.Find(t => t.Gear != null && t.Gear.Code == gear_code && t.SectorCode != null && t.SectorCode == "c");
                                    if (cgs != null)
                                    {
                                        day = cgs.CalendarDays.Find(t => t.Day == sampling_date);
                                        if (day != null)
                                        {
                                            if (isFemaleMaturity)
                                            {
                                                if (!MeasurementExist(e, ViewOption, isFemaleMaturity))
                                                {
                                                    cdsmf = new CalendarDaySpeciesMeasured(day)
                                                    {
                                                        FishSpecies = fishSpecies,
                                                        NotFishSpecies = notFishSpecies,
                                                        CountMeasured = (int)dr["n"],
                                                        MeasurementType = measurement_type,
                                                        Taxa_code = taxaCode,
                                                        IsFemaleMaturity = true,
                                                        MaturityStage = (string)dr["stage"]
                                                    };


                                                    if (day.CalendarDaySpeciesesMeasuredFemale == null)
                                                    {
                                                        day.CalendarDaySpeciesesMeasuredFemale = new List<CalendarDaySpeciesMeasured>();
                                                    }
                                                    day.CalendarDaySpeciesesMeasuredFemale.Add(cdsmf);
                                                }
                                            }
                                            else
                                            {
                                                if (!MeasurementExist(e, ViewOption))
                                                {
                                                    cdsm = new CalendarDaySpeciesMeasured(day)
                                                    {
                                                        FishSpecies = fishSpecies,
                                                        NotFishSpecies = notFishSpecies,
                                                        CountMeasured = (int)dr["n"],
                                                        MeasurementType = measurement_type,
                                                        Taxa_code = taxaCode
                                                    };

                                                    if (day.CalendarDaySpeciesesMeasured == null)
                                                    {
                                                        day.CalendarDaySpeciesesMeasured = new List<CalendarDaySpeciesMeasured>();
                                                    }
                                                    day.CalendarDaySpeciesesMeasured.Add(cdsm);
                                                }

                                            }

                                        }
                                    }
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
            AddToMeasurement(e, ViewOption, isFemaleMaturity);
        }
        private void GetDailyWatchedSpecies(AllSamplingEntitiesEventHandler e, ref List<CalendarGearSector> lcgs)
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
                        cmd.Parameters.AddWithValue("@nsapRegion", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@landing_site", e.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");

                        DateTime month_start = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.Date);
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).Date);

                        cmd.CommandText = @"SELECT
                                                 dbo_LC_FG_sample_day.sdate,
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                dbo_region_watched_species.taxa_code,
                                                dbo_vessel_catch.species_id,
                                                [phFish].[Genus] & ' ' & [phFish].[Species] AS fish_species,
                                                [notFishSpecies].[Genus] & ' ' & [notFishSpecies].[Species] AS not_fish_species,
                                                Count(dbo_vessel_catch.species_id) AS n,
                                                Sum(dbo_vessel_catch.catch_kg) AS wt_catch
                                            FROM
                                                notFishSpecies RIGHT JOIN 
                                                (((((((dbo_LC_FG_sample_day INNER JOIN
                                                dbo_gear_unload ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) INNER JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) INNER JOIN
                                                dbo_vesselunload_fishinggear ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id) INNER JOIN
                                                dbo_vessel_catch ON
                                                dbo_vesselunload_fishinggear.row_id = dbo_vessel_catch.vessel_unload_gear_id) INNER JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) INNER JOIN
                                                dbo_region_watched_species ON
                                                dbo_vessel_catch.species_id = dbo_region_watched_species.sp_id) LEFT JOIN 
                                                phFish ON
                                                dbo_region_watched_species.sp_id = phFish.SpeciesID) ON
                                                notFishSpecies.SpeciesID = dbo_region_watched_species.sp_id
                                            WHERE 
                                                 dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                 dbo_region_watched_species.region_code=@nsapRegion AND
                                                 dbo_LC_FG_sample_day.fma=@fma   AND
                                                 dbo_LC_FG_sample_day.ground_id=@fishing_ground AND
                                                 dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND
                                                 dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            GROUP BY
                                                  dbo_LC_FG_sample_day.sdate,
                                                 dbo_vesselunload_fishinggear.gear_code,
                                                 dbo_vessel_unload_1.sector_code,
                                                 dbo_region_watched_species.taxa_code,
                                                 dbo_vessel_catch.species_id,
                                                 [phFish].[Genus] & ' ' & [phFish].[Species],
                                                 [notFishSpecies].[Genus] & ' ' & [notFishSpecies].[Species]
                                            HAVING
                                                dbo_LC_FG_sample_day.sdate >=@month_start AND
                                                dbo_LC_FG_sample_day.sdate<@month_end
                                            ORDER BY 
                                                 dbo_LC_FG_sample_day.sdate,
                                                 dbo_vesselunload_fishinggear.gear_code,
                                                 dbo_vessel_unload_1.sector_code";

                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                try
                                {
                                    CalendarDay day;
                                    string gear_code = dr["gear_code"].ToString();
                                    DateTime sampling_date = (DateTime)dr["sdate"];
                                    FishSpecies fishSpecies = null;
                                    NotFishSpecies notFishSpecies = null;
                                    CalendarDaySpecies cds;
                                    string taxaCode = (string)dr["taxa_code"];
                                    if (taxaCode == "FIS")
                                    {
                                        fishSpecies = NSAPEntities.FishSpeciesViewModel.GetSpecies((int)dr["species_id"]);
                                    }
                                    else
                                    {
                                        notFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)dr["species_id"]);
                                    }

                                    var cgs = lcgs.Find(t => t.Gear != null && t.Gear.Code == gear_code && t.SectorCode != null && t.SectorCode == "m");
                                    if (cgs != null)
                                    {
                                        day = cgs.CalendarDays.Find(t => t.Day == sampling_date);
                                        if (day != null)
                                        {
                                            if (!MeasurementExist(e, ViewOption))
                                            {
                                                cds = new CalendarDaySpecies(day)
                                                {
                                                    FishSpecies = fishSpecies,
                                                    NotFishSpecies = notFishSpecies,
                                                    CountLandings = (int)dr["n"],
                                                    WeightLanded = (double)dr["wt_catch"],
                                                    Taxa_code = taxaCode
                                                };
                                                if (day.CalendarDaySpecieses == null)
                                                {
                                                    day.CalendarDaySpecieses = new List<CalendarDaySpecies>();
                                                }
                                                day.CalendarDaySpecieses.Add(cds);
                                            }

                                        }
                                    }

                                    cgs = lcgs.Find(t => t.Gear != null && t.Gear.Code == gear_code && t.SectorCode != null && t.SectorCode == "c");
                                    if (cgs != null)
                                    {
                                        day = cgs.CalendarDays.Find(t => t.Day == sampling_date);
                                        if (day != null)
                                        {
                                            if (!MeasurementExist(e, ViewOption))
                                            {
                                                cds = new CalendarDaySpecies(day)
                                                {
                                                    FishSpecies = fishSpecies,
                                                    NotFishSpecies = notFishSpecies,
                                                    CountLandings = (int)dr["n"],
                                                    WeightLanded = (double)dr["wt_catch"],
                                                    Taxa_code = taxaCode
                                                };
                                                if (day.CalendarDaySpecieses == null)
                                                {
                                                    day.CalendarDaySpecieses = new List<CalendarDaySpecies>();
                                                }
                                                day.CalendarDaySpecieses.Add(cds);
                                            }

                                        }
                                    }
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
            AddToMeasurement(e, ViewOption);
        }
        private void GetDailyTotalLandings(AllSamplingEntitiesEventHandler e, ref List<CalendarGearSector> lcgs)
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
                        cmd.Parameters.AddWithValue("@nsapRegion", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@landing_site", e.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");

                        DateTime month_start = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.Date);
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).Date);

                        cmd.CommandText = @"SELECT
                                                 gear.GearName,
                                                 dbo_LC_FG_sample_day.sdate,
                                                 dbo_gear_unload.gr_id,
                                                 First(dbo_LC_FG_sample_day.has_fishing_operation) AS has_operation,
                                                 First(dbo_LC_FG_sample_day.sampleday) AS sampling_day,
                                                 Sum(dbo_gear_unload.gear_count_municipal) AS n_municipal,
                                                 Sum(dbo_gear_unload.gear_count_commercial) AS n_commercial
                                            FROM
                                                gear 
                                                INNER JOIN (dbo_LC_FG_sample_day 
                                                INNER JOIN dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) ON 
                                                    gear.GearCode = dbo_gear_unload.gr_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND 
                                                dbo_LC_FG_sample_day.fma=@fma AND 
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND 
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND 
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            GROUP BY
                                                 gear.GearName,
                                                 dbo_LC_FG_sample_day.sdate,
                                                 dbo_gear_unload.gr_id
                                            HAVING 
                                                dbo_LC_FG_sample_day.sdate >=@month_start AND 
                                                dbo_LC_FG_sample_day.sdate<@month_end
                                            ORDER BY 
                                                gear.GearName,
                                                 dbo_LC_FG_sample_day.sdate; ";

                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                try
                                {
                                    CalendarDay day;
                                    string gear_code = dr["gr_id"].ToString();
                                    DateTime sampling_date = (DateTime)dr["sdate"];
                                    var cgs = lcgs.Find(t => t.Gear != null && t.Gear.Code == gear_code && t.SectorCode != null && t.SectorCode == "m");
                                    if (cgs != null)
                                    {
                                        day = cgs.CalendarDays.Find(t => t.Day == sampling_date);
                                        if (day != null)
                                        {
                                            day.CountNumberOfLandings = (int)(double)dr["n_municipal"];

                                        }
                                    }
                                    cgs = lcgs.Find(t => t.Gear != null && t.Gear.Code == gear_code && t.SectorCode != null && t.SectorCode == "c");
                                    if (cgs != null)
                                    {
                                        day = cgs.CalendarDays.Find(t => t.Day == sampling_date);
                                        if (day != null)
                                        {
                                            day.CountNumberOfLandings = (int)(double)dr["n_commercial"];

                                        }
                                    }
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

        }
        private CalendarDailySummary GetCalendarDailySummary(AllSamplingEntitiesEventHandler e)
        {
            CalendarDailySummary summary = new CalendarDailySummary();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");
                        DateTime month_start = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.Date);
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).Date);
                        cmd.Parameters.AddWithValue("@nsapRegion", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@landing_site", e.LandingSite.LandingSiteID);

                        cmd.CommandText = @"SELECT
                                                 dbo_LC_FG_sample_day.sdate,
                                                 First(dbo_LC_FG_sample_day.sampleday) AS is_sampling_day,
                                                 First(dbo_LC_FG_sample_day.has_fishing_operation) AS has_operation,
                                                 Sum(dbo_gear_unload.gear_count_municipal) AS n_municipal,
                                                 Sum(dbo_gear_unload.gear_count_commercial) AS n_commercial,
                                                 Sum(dbo_gear_unload.gear_catch_municipal) AS wt_municipal,
                                                 Sum(dbo_gear_unload.gear_catch_commercial) AS wt_commercial
                                            FROM
                                                 dbo_LC_FG_sample_day INNER JOIN 
                                                dbo_gear_unload ON 
                                                    dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type AND 
                                                dbo_LC_FG_sample_day.sdate>=@month_start AND 
                                                dbo_LC_FG_sample_day.sdate<@month_end AND 
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND 
                                                dbo_LC_FG_sample_day.fma=@fma AND 
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND 
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site
                                            GROUP BY
                                                 dbo_LC_FG_sample_day.sdate";

                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                summary.CalendarDays = new List<CalendarDay>();
                                while (dr.Read())
                                {
                                    int count_operations = 0;
                                    double weight_catch = 0;
                                    CalendarDay cd = new CalendarDay
                                    {
                                        Day = (DateTime)dr["sdate"],
                                        IsSamplingDay = (Int16)dr["is_sampling_day"] == -1,
                                        HasFishingOperation = (Int16)dr["has_operation"] == -1

                                    };
                                    if (cd.HasFishingOperation)
                                    {
                                        if (dr["n_municipal"] != DBNull.Value)
                                        {
                                            count_operations += (int)(double)dr["n_municipal"];
                                        }
                                        if (dr["n_commercial"] != DBNull.Value)
                                        {
                                            count_operations += (int)(double)dr["n_commercial"];
                                        }
                                        if (dr["wt_municipal"] != DBNull.Value)
                                        {
                                            weight_catch += (double)dr["wt_municipal"];
                                        }
                                        if (dr["wt_commercial"] != DBNull.Value)
                                        {
                                            weight_catch += (double)dr["wt_commercial"];
                                        }
                                        cd.CountOfFishingOperations = count_operations;
                                        cd.TotalWeightOfCatch = weight_catch;

                                    }
                                    summary.CalendarDays.Add(cd);
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
            return summary;

        }

        public int TotalVesselUnloadCount { get; private set; }
        public double TotalLandedCatchWeight { get; private set; }

        private List<CalendarGearSector> GetCalendarGearSectors(AllSamplingEntitiesEventHandler e)
        {
            string expandedQuery = "";
            HashSet<CalendarGearSector> this_list = new HashSet<CalendarGearSector>();
            if (Global.Settings.UsemySQL)
            {

            }
            else
            {
                using (var con = new OleDbConnection(Global.ConnectionString))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("@landing_site", e.LandingSite.LandingSiteID);
                        cmd.Parameters.AddWithValue("@nsapRegion", e.NSAPRegion.Code);
                        cmd.Parameters.AddWithValue("@fma", e.FMA.FMAID);
                        cmd.Parameters.AddWithValue("@fishing_ground", e.FishingGround.Code);
                        cmd.Parameters.AddWithValue("@sampling_type", "rs");

                        DateTime month_start = (DateTime)e.MonthSampled;
                        cmd.Parameters.AddWithValue("@month_start", month_start.Date);
                        cmd.Parameters.AddWithValue("@month_end", month_start.AddMonths(1).Date);


                        cmd.CommandText = @"SELECT
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                gear.GearName,
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_LC_FG_sample_day.sampleday,
                                                dbo_LC_FG_sample_day.has_fishing_operation,
                                                Count(dbo_vesselunload_fishinggear.gear_code) AS n,
                                                Sum(dbo_vesselunload_fishinggear.catch_weight) AS sum_wt
                                            FROM
                                                 ((dbo_LC_FG_sample_day LEFT JOIN
                                                (dbo_gear_unload LEFT JOIN
                                                dbo_vessel_unload ON
                                                dbo_gear_unload.unload_gr_id = dbo_vessel_unload.unload_gr_id) ON
                                                dbo_LC_FG_sample_day.unload_day_id = dbo_gear_unload.unload_day_id) LEFT JOIN
                                                dbo_vessel_unload_1 ON
                                                dbo_vessel_unload.v_unload_id = dbo_vessel_unload_1.v_unload_id) LEFT JOIN
                                                (gear RIGHT JOIN
                                                dbo_vesselunload_fishinggear ON
                                                gear.GearCode = dbo_vesselunload_fishinggear.gear_code) ON
                                                dbo_vessel_unload.v_unload_id = dbo_vesselunload_fishinggear.vessel_unload_id
                                            WHERE 
                                                dbo_LC_FG_sample_day.land_ctr_id=@landing_site AND
                                                dbo_gear_unload.gr_id Is Not Null AND
                                                dbo_LC_FG_sample_day.region_id=@nsapRegion AND
                                                dbo_LC_FG_sample_day.fma=@fma AND
                                                dbo_LC_FG_sample_day.ground_id=@fishing_ground AND
                                                dbo_LC_FG_sample_day.type_of_sampling=@sampling_type
                                            GROUP BY
                                                 dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code,
                                                gear.GearName,
                                                dbo_LC_FG_sample_day.sdate,
                                                dbo_LC_FG_sample_day.sampleday,
                                                dbo_LC_FG_sample_day.has_fishing_operation
                                            HAVING 
                                                dbo_LC_FG_sample_day.sdate>=@month_start AND
                                                dbo_LC_FG_sample_day.sdate<@month_end
                                            ORDER BY 
                                                dbo_vesselunload_fishinggear.gear_code,
                                                dbo_vessel_unload_1.sector_code DESC,
                                                dbo_LC_FG_sample_day.sdate";



                        expandedQuery = cmd.CommandText
                            .Replace("@nsapRegion", $"'{e.NSAPRegion.Code}'")
                            .Replace("@fma", e.FMA.FMAID.ToString())
                            .Replace("@fishing_ground", $"'{e.FishingGround.Code}'")
                            .Replace("@landing_site", e.LandingSite.LandingSiteID.ToString())
                            .Replace("@sampling_type", "'rs'")
                            .Replace("@month_start", $"#{month_start.ToString("MM/d/yyyy")}#")
                            .Replace("@month_end", $"#{month_start.AddMonths(1).ToString("MM/d/yyyy")}#");


                        try
                        {
                            con.Open();
                            var dr = cmd.ExecuteReader();
                            CalendarDay cd;
                            while (dr.Read())
                            {
                                bool isSamplingDay = (bool)dr["sampleday"];
                                bool hasOperation = (bool)dr["has_fishing_operation"];

                                CalendarGearSector cgs = new CalendarGearSector();
                                if (isSamplingDay && hasOperation && dr["gear_code"] != DBNull.Value)
                                {
                                    cgs.Gear = NSAPEntities.GearViewModel.GetGear(dr["gear_code"].ToString());
                                    cgs.SectorCode = dr["sector_code"].ToString();


                                    cd = new CalendarDay
                                    {
                                        CountOfFishingOperations = (int)dr["n"],
                                        TotalWeightOfCatch = (double)dr["sum_wt"],
                                        Day = (DateTime)dr["sdate"],
                                        IsSamplingDay = isSamplingDay,
                                        HasFishingOperation = hasOperation
                                    };

                                    if (!this_list.Contains(cgs))
                                    {
                                        this_list.Add(cgs);
                                    }
                                    else
                                    {
                                        cgs = this_list.First(t => t.SectorCode == cgs.SectorCode && t.Gear.Code == cgs.Gear.Code);
                                    }

                                    cd.CalendarGearSector_Parent = cgs;
                                    cgs.CalendarDays.Add(cd);

                                }
                                else
                                {
                                    cgs.NoFishing = true;
                                    if (!this_list.Contains(cgs))
                                    {
                                        this_list.Add(cgs);
                                    }
                                    else
                                    {
                                        cgs = this_list.First(t => t.NoFishing);
                                    }

                                    cd = new CalendarDay
                                    {
                                        Day = (DateTime)dr["sdate"],
                                        IsSamplingDay = isSamplingDay,
                                        HasFishingOperation = hasOperation,
                                        CalendarGearSector_Parent = cgs
                                    };
                                    cgs.CalendarDays.Add(cd);

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

            return this_list.ToList();
        }
    }
}
