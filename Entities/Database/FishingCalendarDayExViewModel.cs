using NSAP_ODK.TreeViewModelControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace NSAP_ODK.Entities.Database
{
    public class FishingCalendarDayExViewModel
    {
        public event EventHandler<MakeCalendarEventArg> CalendarEvent;
        private List<FishingCalendarDayEx> _fishingCalendarDays;
        private List<SpeciesCalendarDay> _speciesCalendarDays;
        private List<SpeciesCalendarDay> _speciesMeasurementCalendarDays;
        private int _numberOfDays;
        private List<SpeciesFishingGearAndSector> _speciesFishingGearAndSectors;
        private List<SpeciesFishingGearAndSector> _measured_speciesFishingGearAndSectors;
        private List<FishingGearAndSector> _gearsAndSectors;
        private List<int> _vesselUnloads;
        private AllSamplingEntitiesEventHandler _selectedMonth;
        public CalendarViewType CalendarViewType { get; set; }
        public int TotalVesselUnloadCount { get; set; }
        public int? TotalLandingCount { get; set; }
        public double? TotalWeightLanded { get; set; }
        public double? TotalLandedCatchWeight { get; set; }
        public FishingCalendarRepository Repository { get; set; }

        public Dictionary<string, List<SpeciesCalendarDay>> MeasuredSpeciesCalendarDayDictionary { get; private set; } = new Dictionary<string, List<SpeciesCalendarDay>>();
        public Dictionary<string, List<SpeciesCalendarDay>> SpeciesCalendarDayDictionary { get; private set; } = new Dictionary<string, List<SpeciesCalendarDay>>();
        public Dictionary<string, List<FishingCalendarDayEx>> CalendarDaysDictionary { get; private set; } = new Dictionary<string, List<FishingCalendarDayEx>>();
        public Dictionary<string, List<FishingGearAndSector>> UniqueGearListDictionary { get; private set; } = new Dictionary<string, List<FishingGearAndSector>>();
        public Dictionary<string, List<int>> VesselUnloadIDsDictionary { get; private set; } = new Dictionary<string, List<int>>();
        public FishingCalendarDayExViewModel()
        {
            Repository = new FishingCalendarRepository();
        }
        public DataTable DataTable { get; set; }

        public List<CrossTabCommon> ListCrossTabCommon
        {
            get
            {
                List<CrossTabCommon> this_list = new List<CrossTabCommon>();
                foreach (int vu_id in _vesselUnloads)
                {
                    VesselUnload vu = NSAPEntities.SummaryItemViewModel.GetVesselUnload(vu_id);
                    CrossTabCommon ctb = new CrossTabCommon(vu);
                }
                return this_list;
            }
        }

        public string LocationLabel
        {
            get
            {
                if (_fishingCalendarDays.Count == 0)
                {
                    return "";
                }
                else
                {
                    var day = _fishingCalendarDays.First();
                    return $"{_selectedMonth.LandingSite}, {_selectedMonth.FishingGround}, {_selectedMonth.FMA}, {_selectedMonth.NSAPRegion}";
                }
            }
        }
        public string SamplingCalendarTitle
        {
            get
            {
                string labelCountWeight = "";
                switch (CalendarViewType)
                {
                    case CalendarViewType.calendarViewTypeSampledLandings:
                        labelCountWeight = $"Total sampled landings: {TotalVesselUnloadCount}";
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                        labelCountWeight = $"Total number of landings by gear and sector: {TotalLandingCount}";
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                        labelCountWeight = $"Total weight of catch of landings by gear and sector: {TotalLandedCatchWeight}";
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandings:
                        labelCountWeight = $"Total number of landings for all gears and sectors: {TotalLandingCount}";
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandings:
                        labelCountWeight = $"Total weight of catch of landings for all gears and sectors: {TotalWeightLanded}";
                        break;
                }
                string restDayIndicator = "";
                if (_fishingCalendarDays.Where(t => !t.IsSamplingDay).Count() > 0)
                {
                    restDayIndicator = "(Blue columns represent rest day)";
                }
                var label = $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";
                if (CalendarHasValue)
                {
                    return $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";
                }
                else
                {
                    return "The version of the electronic form cannot display the information requested";
                }

            }
        }
        public bool HasEformNeedingUpdate { get; set; }
        public bool CalendarHasValue { get; set; }
        public bool DayIsRestDay(int day)
        {
            var sd = _fishingCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == day);
            if (sd == null)
            {
                return false;
            }
            else
            {
                return !sd.IsSamplingDay;
            }
        }
        public Task MakeCalendarTask(bool isWatchedSpeciesCalendar = false)
        {
            return Task.Run(() => MakeCalendar(isWatchedSpeciesCalendar));
        }
        //public async void MakeCalendar(bool isWatchedSpeciesCalendar = false)
        public async Task<bool> MakeCalendar(bool isWatchedSpeciesCalendar = false)
        {
            CalendarEvent?.Invoke(null, new MakeCalendarEventArg { Context = "Preparing calendar data" });

            CalendarHasValue = false;
            if (_fishingCalendarDays.Count == 0)
            {
                return false;
            }

            DataRow row;
            TotalVesselUnloadCount = 0;
            TotalLandingCount = 0;
            TotalLandedCatchWeight = 0;
            DateTime samplingMonthYear = (DateTime)_fishingCalendarDays.First().MonthViewModel.MonthSampled;
            _numberOfDays = DateTime.DaysInMonth(samplingMonthYear.Year, samplingMonthYear.Month);

            DataTable = new DataTable();
            if (isWatchedSpeciesCalendar)
            {
                if (CalendarViewType == CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight ||
                    CalendarViewType == CalendarViewType.calendarViewTypeWatchedSpeciesLandings)
                {
                    _speciesCalendarDays = await GetSpeciesCalendarDayForMonthTask(_selectedMonth);

                    HashSet<SpeciesFishingGearAndSector> sfgss = new HashSet<SpeciesFishingGearAndSector>();
                    foreach (var item in _speciesCalendarDays)
                    {
                        FishingGearAndSector fgs = new FishingGearAndSector(g: item.Gear, sector_code: item.SectorCode);
                        sfgss.Add(new SpeciesFishingGearAndSector(fgs, item.SpeciesID, item.SpeciesName, item.TaxaCode));
                    }
                    _speciesFishingGearAndSectors = sfgss.ToList();
                }
                else if (CalendarViewType == CalendarViewType.calendarViewTypeLengthFrequencyMeasurement ||
                    CalendarViewType == CalendarViewType.calendarViewTypeLengthMeasurement ||
                    CalendarViewType == CalendarViewType.calendarViewTypeLengthWeightMeasurement ||
                    CalendarViewType == CalendarViewType.calendarViewTypeMaturityMeasurement)
                {
                    _speciesMeasurementCalendarDays = await GetMeasuredSpeciesCalendarDayForMonthTask(_selectedMonth, CalendarViewType);
                    HashSet<SpeciesFishingGearAndSector> msfgss = new HashSet<SpeciesFishingGearAndSector>();
                    foreach (var item in _speciesMeasurementCalendarDays)
                    {
                        FishingGearAndSector fgs = new FishingGearAndSector(g: item.Gear, sector_code: item.SectorCode);
                        msfgss.Add(new SpeciesFishingGearAndSector(fgs, item.SpeciesID, item.SpeciesName, item.TaxaCode));
                    }
                    _measured_speciesFishingGearAndSectors = msfgss.ToList();
                }

                DataTable.Columns.Add("Taxa");
                DataTable.Columns.Add("Species");
            }
            DataTable.Columns.Add("GearName");
            DataTable.Columns.Add("GearCode");

            //added oct 21 2022
            DataTable.Columns.Add("Sector");

            DataTable.Columns.Add("Month");

            for (int n = 1; n <= _numberOfDays; n++)
            {
                DataTable.Columns.Add(n.ToString());
            }


            switch (CalendarViewType)
            {
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    foreach (var msp_gr_sec in _measured_speciesFishingGearAndSectors.Where(t => t.FishingGearAndSector.SectorCode == "c" || t.FishingGearAndSector.SectorCode == "m").OrderBy(t => t.TaxaCode).ThenBy(t => t.SpeciesName).ThenBy(t => t.FishingGearAndSector.Gear.GearName))
                    //foreach (var sp_gr_sec in _speciesFishingGearAndSectors)
                    {
                        row = DataTable.NewRow();
                        row["Taxa"] = msp_gr_sec.Taxa;
                        row["Species"] = msp_gr_sec.SpeciesName;
                        row["GearName"] = msp_gr_sec.FishingGearAndSector.Gear.GearName;
                        row["GearCode"] = msp_gr_sec.FishingGearAndSector.Gear.Code;
                        row["Sector"] = msp_gr_sec.FishingGearAndSector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            //var day = _speciesCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.SpeciesID == sp_gr_sec.SpeciesID && t.Gear == sp_gr_sec.FishingGearAndSector.Gear && t.SectorCode == sp_gr_sec.FishingGearAndSector.SectorCode);
                            var day = _speciesMeasurementCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.SpeciesID == msp_gr_sec.SpeciesID && t.GearCode == msp_gr_sec.FishingGearAndSector.Gear.Code && t.SectorCode == msp_gr_sec.FishingGearAndSector.SectorCode);
                            if (day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                CalendarHasValue = true;
                                switch (CalendarViewType)
                                {
                                    case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                                        if (day.CountLenFreqMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = day.CountLenFreqMeas;
                                        }
                                        break;
                                    case CalendarViewType.calendarViewTypeLengthMeasurement:
                                        if (day.CountLenMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = day.CountLenMeas;
                                        }
                                        break;
                                    case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                                        if (day.CountLenWtMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = day.CountLenWtMeas;
                                        }
                                        break;
                                    case CalendarViewType.calendarViewTypeMaturityMeasurement:
                                        if (day.CountMaturityMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = day.CountMaturityMeas;
                                        }
                                        break;
                                }

                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;

                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                    foreach (var sp_gr_sec in _speciesFishingGearAndSectors.Where(t => t.FishingGearAndSector.SectorCode == "c" || t.FishingGearAndSector.SectorCode == "m").OrderBy(t => t.TaxaCode).ThenBy(t => t.SpeciesName).ThenBy(t => t.FishingGearAndSector.Gear.GearName))
                    //foreach (var sp_gr_sec in _speciesFishingGearAndSectors)
                    {
                        row = DataTable.NewRow();
                        row["Taxa"] = sp_gr_sec.Taxa;
                        row["Species"] = sp_gr_sec.SpeciesName;
                        row["GearName"] = sp_gr_sec.FishingGearAndSector.Gear.GearName;
                        row["GearCode"] = sp_gr_sec.FishingGearAndSector.Gear.Code;
                        row["Sector"] = sp_gr_sec.FishingGearAndSector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            //var day = _speciesCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.SpeciesID == sp_gr_sec.SpeciesID && t.Gear == sp_gr_sec.FishingGearAndSector.Gear && t.SectorCode == sp_gr_sec.FishingGearAndSector.SectorCode);
                            var day = _speciesCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.SpeciesID == sp_gr_sec.SpeciesID && t.GearCode == sp_gr_sec.FishingGearAndSector.Gear.Code && t.SectorCode == sp_gr_sec.FishingGearAndSector.SectorCode);
                            if (day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                CalendarHasValue = true;
                                row[x.ToString()] = day.NumberOfLandingsOfSpecies;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                    foreach (var sp_gr_sec in _speciesFishingGearAndSectors.Where(t => t.FishingGearAndSector.SectorCode == "c" || t.FishingGearAndSector.SectorCode == "m").OrderBy(t => t.TaxaCode).ThenBy(t => t.SpeciesName).ThenBy(t => t.FishingGearAndSector.Gear.GearName))
                    //foreach (var sp_gr_sec in _speciesFishingGearAndSectors)
                    {
                        row = DataTable.NewRow();
                        row["Taxa"] = sp_gr_sec.Taxa;
                        row["Species"] = sp_gr_sec.SpeciesName;
                        row["GearName"] = sp_gr_sec.FishingGearAndSector.Gear.GearName;
                        row["GearCode"] = sp_gr_sec.FishingGearAndSector.Gear.Code;
                        row["Sector"] = sp_gr_sec.FishingGearAndSector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            var day = _speciesCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.SpeciesID == sp_gr_sec.SpeciesID && t.Gear == sp_gr_sec.FishingGearAndSector.Gear && t.SectorCode == sp_gr_sec.FishingGearAndSector.SectorCode);
                            if (day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                CalendarHasValue = true;
                                row[x.ToString()] = day.WeightOfSpeciesLanded;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeSampledLandings:

                    foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
                    {
                        row = DataTable.NewRow();
                        row["GearName"] = gear_sector.Gear.GearName;
                        row["GearCode"] = gear_sector.Gear.Code;
                        row["Sector"] = gear_sector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");

                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            var day = _fishingCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.Gear == gear_sector.Gear && t.SectorCode == gear_sector.SectorCode);
                            if (day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                CalendarHasValue = true;
                                row[x.ToString()] = day.CountVesselUnloads;
                                TotalVesselUnloadCount += day.CountVesselUnloads;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;

                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                    TotalLandingCount = null;
                    foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
                    {
                        row = DataTable.NewRow();
                        row["GearName"] = gear_sector.Gear.GearName;
                        row["GearCode"] = gear_sector.Gear.Code;
                        row["Sector"] = gear_sector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            var day = _fishingCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.Gear == gear_sector.Gear && t.SectorCode == gear_sector.SectorCode);
                            if (day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                int? landedCount = null;
                                if (day.CountCommercialLandings != null && day.CountMunicipalLandings != null)
                                {
                                    landedCount = (int)day.CountCommercialLandings + (int)day.CountMunicipalLandings;
                                }
                                else if (day.TotalNumberOfLandings != null)
                                {
                                    landedCount = (int)day.TotalNumberOfLandings;
                                }
                                else
                                {
                                    landedCount = day.CountVesselUnloads;
                                }
                                row[x.ToString()] = landedCount;
                                if (landedCount != null)
                                {
                                    TotalLandingCount = (TotalLandingCount ?? 0) + (int)landedCount;
                                }
                                if (CalendarHasValue == false)
                                {
                                    CalendarHasValue = landedCount != null;
                                }
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                    TotalLandedCatchWeight = null;
                    foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
                    {
                        row = DataTable.NewRow();
                        row["GearName"] = gear_sector.Gear.GearName;
                        row["GearCode"] = gear_sector.Gear.Code;
                        row["Sector"] = gear_sector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            var day = _fishingCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.Gear == gear_sector.Gear && t.SectorCode == gear_sector.SectorCode);
                            if (day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                double? landedWeight = null;
                                if (day.TotalWeightCommercialLandings != null || day.TotalWeightMunicipalLandings != null)
                                {
                                    landedWeight = (day.TotalWeightCommercialLandings ?? 0) + day.TotalWeightMunicipalLandings ?? 0;
                                }
                                else
                                {
                                    landedWeight = (double)day.TotalWeightOfCatch;
                                }
                                row[x.ToString()] = landedWeight;
                                if (landedWeight != null)
                                {
                                    TotalLandedCatchWeight = (TotalLandedCatchWeight ?? 0) + (landedWeight ?? 0);
                                    if (!CalendarHasValue)
                                    {
                                        CalendarHasValue = true;
                                    }
                                }
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandings:
                    TotalLandingCount = null;
                    row = DataTable.NewRow();
                    row["GearName"] = "All gears";
                    row["GearCode"] = "All sectors";
                    row["Sector"] = "";
                    row["Month"] = samplingMonthYear.ToString("MMM-yyyy");

                    int? landingCount = null;
                    for (int x = 1; x <= _numberOfDays; x++)
                    {
                        //row[x.ToString()] = "-";
                        var day_gears = _fishingCalendarDays.Where(t => t.SamplingDate.Day == x).ToList();
                        landingCount = null;
                        foreach (var d in day_gears)
                        {
                            if (!d.HasFishingOperation)
                            {
                                row[x.ToString()] = "x";
                            }
                            else if (d.SectorCode == "m" && d.CountMunicipalLandings != null)
                            {
                                landingCount = (landingCount ?? 0) + d.CountMunicipalLandings;
                            }
                            else if (d.SectorCode == "c" && d.CountCommercialLandings != null)
                            {
                                landingCount = (landingCount ?? 0) + d.CountCommercialLandings;
                            }
                            else
                            {
                                landingCount = (landingCount ?? 0) + d.CountVesselUnloads;
                            }
                        }
                        if (landingCount != null)
                        {
                            row[x.ToString()] = landingCount;
                            TotalLandingCount = (TotalLandingCount ?? 0) + landingCount;
                            CalendarHasValue = true;
                        }
                    }

                    DataTable.Rows.Add(row);
                    break;
                case CalendarViewType.calendarViewTypeWeightAllLandings:
                    TotalWeightLanded = null;
                    row = DataTable.NewRow();
                    row["GearName"] = "All gears";
                    row["GearCode"] = "All sectors";
                    row["Sector"] = "";
                    row["Month"] = samplingMonthYear.ToString("MMM-yyyy");

                    for (int x = 1; x <= _numberOfDays; x++)
                    {
                        //row[x.ToString()] = "-";
                        double? weight_catch = null;
                        var day_gears = _fishingCalendarDays.Where(t => t.SamplingDate.Day == x).ToList();
                        foreach (var d in day_gears)
                        {
                            if (!d.HasFishingOperation)
                            {
                                row[x.ToString()] = "x";
                            }
                            else if (d.TotalWeightMunicipalLandings != null | d.TotalWeightCommercialLandings != null)
                            {
                                if (d.SectorCode == "m")
                                {
                                    weight_catch = (weight_catch ?? 0) + d.TotalWeightMunicipalLandings;
                                }
                                else if (d.SectorCode == "c")
                                {
                                    weight_catch = (weight_catch ?? 0) + d.TotalWeightCommercialLandings;
                                }
                            }
                            else if (weight_catch == null && d.TotalWeightOfCatch != null)
                            {
                                weight_catch = (weight_catch ?? 0) + d.TotalWeightOfCatch;
                            }
                        }
                        if (weight_catch != null)
                        {
                            CalendarHasValue = true;
                            row[x.ToString()] = ((double)weight_catch).ToString("N1");
                            TotalWeightLanded = (TotalWeightLanded ?? 0) + weight_catch;
                        }
                    }

                    DataTable.Rows.Add(row);
                    break;
            }
            CalendarEvent?.Invoke(null, new MakeCalendarEventArg { Context = "Calendar data created" });
            return true;
        }

        public List<LandingSiteSampling> LandingSiteSamplings
        {
            get
            {
                HashSet<LandingSiteSampling> llss = new HashSet<LandingSiteSampling>();
                //foreach (var fcd in _fishingCalendarDays.GroupBy(t => t.GearUnload.PK))
                //{
                //    llss.Add(fcd.First().GearUnload.Parent);
                //}
                foreach (var fcd in _fishingCalendarDays)
                {
                    var parent = fcd.GearUnload.Parent;
                    if (!llss.Contains(parent))
                    {
                        llss.Add(fcd.GearUnload.Parent);
                    }
                    else
                    {

                    }
                }
                return llss.ToList();
            }
        }
        public bool CanCreateCalendar
        {
            get
            {
                return _fishingCalendarDays != null && _fishingCalendarDays.Count > 0;
            }
        }
        public GearUnload GetGearUnload(string gearName, string sector, int day, CalendarViewType calendarView)
        {
            //var l = _fishingCalendarDays.Where(t => t.Gear != null && t.Gear.GearName == gearName && t.Sector == sector && t.SamplingDate.Day == day).ToList();
            GearUnload gu = null;
            if (calendarView == CalendarViewType.calendarViewTypeCountAllLandings)
            {

            }
            else
            {
                try
                {
                    gu = _fishingCalendarDays.Where(t => t.Gear != null && t.Gear.GearName == gearName && t.Sector == sector && t.SamplingDate.Day == day).First().GearUnload;
                }
                catch
                {
                    //ignore
                }
            }
            return gu;
            //return new GearUnload();
        }

        public Task<List<SpeciesCalendarDay>> GetMeasuredSpeciesCalendarDayForMonthTask(AllSamplingEntitiesEventHandler e, CalendarViewType viewType)
        {
            return Task.Run(() => GetMeasuredSpeciesCalendarDayForMonth(e, viewType));
        }
        public List<SpeciesCalendarDay> GetMeasuredSpeciesCalendarDayForMonth(AllSamplingEntitiesEventHandler e, CalendarViewType viewType)
        {
            return Repository.GetMeasuredSpeciesCalendarDays(e, viewType);
        }
        public Task<List<SpeciesCalendarDay>> GetMeasuredSpeciesCalendarDayForMonthTask(AllSamplingEntitiesEventHandler selectedMonth)
        {
            return Task.Run(() => GetMeasuredSpeciesCalendarDayForMonth(selectedMonth));
        }
        public List<SpeciesCalendarDay> GetMeasuredSpeciesCalendarDayForMonth(AllSamplingEntitiesEventHandler selectedMonth)
        {

            try
            {
                _speciesMeasurementCalendarDays = MeasuredSpeciesCalendarDayDictionary[selectedMonth.GUID];
            }
            catch
            {
                _speciesMeasurementCalendarDays = Repository.GetMeasuredWatchedSpeciesCalendarDays(selectedMonth);
                MeasuredSpeciesCalendarDayDictionary.Add(selectedMonth.GUID, _speciesMeasurementCalendarDays);
            }
            return _speciesMeasurementCalendarDays;
        }

        public Task<List<SpeciesCalendarDay>> GetSpeciesCalendarDayForMonthTask(AllSamplingEntitiesEventHandler selectedMonth)
        {
            return Task.Run(() => GetSpeciesCalendarDayForMonth(selectedMonth));
        }
        public List<SpeciesCalendarDay> GetSpeciesCalendarDayForMonth(AllSamplingEntitiesEventHandler selectedMonth)
        {

            try
            {
                _speciesCalendarDays = SpeciesCalendarDayDictionary[selectedMonth.GUID];
            }
            catch
            {
                _speciesCalendarDays = Repository.GetWatchedSpeciesCalendarDays(selectedMonth);
                SpeciesCalendarDayDictionary.Add(selectedMonth.GUID, _speciesCalendarDays);
            }
            return _speciesCalendarDays;
        }
        public async Task<List<FishingCalendarDayEx>> GetCalendarDaysForMonth(AllSamplingEntitiesEventHandler selectedMonth)
        {
            CalendarEvent?.Invoke(null, new MakeCalendarEventArg { Context = "Fetching landing data from database" });
            _selectedMonth = selectedMonth;
            if (CalendarDaysDictionary.Keys.Count == 0 || !CalendarDaysDictionary.Keys.Contains(selectedMonth.GUID))
            {
                CalendarDaysDictionary.Add(selectedMonth.GUID, await Repository.GetCalendarDaysAsync(selectedMonth));
                UniqueGearListDictionary.Add(selectedMonth.GUID, Repository.UniqueGearSectorList.ToList());
                VesselUnloadIDsDictionary.Add(selectedMonth.GUID, Repository.GetVesselUnloadIDsOfCalendar(selectedMonth));
            }
            _fishingCalendarDays = CalendarDaysDictionary[selectedMonth.GUID];
            _gearsAndSectors = UniqueGearListDictionary[selectedMonth.GUID];
            _vesselUnloads = VesselUnloadIDsDictionary[selectedMonth.GUID];
            CalendarEvent?.Invoke(null, new MakeCalendarEventArg { Context = "Fetched landing data from database" });
            return _fishingCalendarDays;
        }
    }
}
