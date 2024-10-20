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
        private bool _isWatchedSpeciesCalendar;
        private List<SpeciesCalendarDay> _speciesCalendarDays;
        private List<SpeciesCalendarDay> _speciesMeasurementCalendarDays;
        private int _numberOfDays;
        private List<SpeciesFishingGearAndSector> _speciesFishingGearAndSectors;
        private List<SpeciesFishingGearAndSector> _measured_speciesFishingGearAndSectors;
        private List<FishingGearAndSector> _gearsAndSectors;
        private List<int> _vesselUnloads;
        private AllSamplingEntitiesEventHandler _selectedMonth;
        private bool _getFemaleMaturity;
        public CalendarViewType CalendarViewType { get; set; }
        public int TotalVesselUnloadCount { get; set; }

        public int? TotalMeasurementCount { get; set; }
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
                int rows = 0;
                string labelCountWeight = "";
                switch (CalendarViewType)
                {
                    case CalendarViewType.calendarViewTypeSampledLandings:
                        labelCountWeight = $"Total sampled landings: {TotalVesselUnloadCount}";
                        rows = _gearsAndSectors.Where(t => t.Sector != "Other").Count();
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                        labelCountWeight = $"Total number of landings by gear and sector: {TotalLandingCount}";
                        rows = _gearsAndSectors.Where(t => t.Sector != "Other").Count();
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                        labelCountWeight = $"Total weight of catch of landings by gear and sector: {TotalLandedCatchWeight}";
                        rows = _gearsAndSectors.Where(t => t.Sector != "Other").Count();
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandings:
                        labelCountWeight = $"Total number of landings for all gears and sectors: {TotalLandingCount}";
                        rows = _gearsAndSectors.Where(t => t.Sector != "Other").Count();
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandings:
                        labelCountWeight = $"Total weight of catch of landings for all gears and sectors: {TotalWeightLanded}";
                        break;
                    case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                        rows = _speciesFishingGearAndSectors.Count();
                        labelCountWeight = $"Total weight of watched species of landings for all gears and sectors: {((double)TotalWeightLanded).ToString("N2")}";
                        break;
                    case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                        //labelCountWeight = $"Total weight of watched species of landings for all gears and sectors: {TotalWeightLanded}";
                        rows = _speciesFishingGearAndSectors.Count();
                        break;
                    case CalendarViewType.calendarViewTypeLengthMeasurement:
                        if (CalendarHasValue)
                        {
                            labelCountWeight = $"Number of length measurements: {TotalMeasurementCount}";
                        }
                        else
                        {
                            labelCountWeight = "No length measurements were done";
                        }
                        rows = _measured_speciesFishingGearAndSectors.Count();
                        break;
                    case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                        if (CalendarHasValue)
                        {
                            labelCountWeight = $"Number of length weight measurements: {TotalMeasurementCount}";
                        }
                        else
                        {
                            labelCountWeight = "No length weight measurements were done";
                        }
                        rows = _measured_speciesFishingGearAndSectors.Count();
                        break;
                    case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                        if (CalendarHasValue)
                        {
                            labelCountWeight = $"Number of length frequency measurements: {TotalMeasurementCount}";
                        }
                        else
                        {
                            labelCountWeight = "No length frequency measurements were done";
                        }
                        rows = _measured_speciesFishingGearAndSectors.Count();
                        break;
                    case CalendarViewType.calendarViewTypeMaturityMeasurement:
                        string female_maturity = "";
                        if (_getFemaleMaturity)
                        {
                            female_maturity = "for females ";
                        }
                        if (CalendarHasValue)
                        {
                            labelCountWeight = $"Number of maturity measurements {female_maturity}(length, weight, sex, and maturity stage): {TotalMeasurementCount}";
                        }
                        else
                        {
                            labelCountWeight = $"No maturity measurements {female_maturity}(length, weight, sex, and maturity stage) were done";
                        }
                        rows = _measured_speciesFishingGearAndSectors.Count();
                        break;
                }
                string restDayIndicator = "";
                if (_fishingCalendarDays.Where(t => !t.IsSamplingDay).Count() > 0)
                {
                    restDayIndicator = "(Blue columns represent rest day)";
                }
                //var label = $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";
                if (CalendarHasValue)
                {
                    //return $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";
                    return $"Rows: {rows}, {labelCountWeight} {restDayIndicator}";
                }
                else
                {
                    if (_isWatchedSpeciesCalendar)
                    {
                        return $"{labelCountWeight}";
                    }
                    else
                    {
                        return "The version of the electronic form cannot display the information requested";
                    }
                }

            }
        }
        public bool GetFemaleMaturity { get; set; }
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
        public async Task<bool> MakeCalendar(bool isWatchedSpeciesCalendar = false, bool getFemaleMaturity = false)
        {
            Utilities.Logger.LogCalendar("start FishingCalendarDayExViewModel.MakeCalendar()");
            _getFemaleMaturity = getFemaleMaturity;
            _isWatchedSpeciesCalendar = isWatchedSpeciesCalendar;
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

                    foreach (var item in _speciesCalendarDays.Where(t => t.Gear != null))
                    {
                        try

                        {
                            FishingGearAndSector fgs = new FishingGearAndSector(g: item.Gear, sector_code: item.SectorCode);
                            sfgss.Add(new SpeciesFishingGearAndSector(fgs, item.SpeciesID, item.SpeciesName, item.TaxaCode));
                        }
                        catch (Exception ex)
                        {
                            Utilities.Logger.Log(ex);
                        }
                    }
                    _speciesFishingGearAndSectors = sfgss.ToList();
                }
                else if (CalendarViewType == CalendarViewType.calendarViewTypeLengthFrequencyMeasurement ||
                    CalendarViewType == CalendarViewType.calendarViewTypeLengthMeasurement ||
                    CalendarViewType == CalendarViewType.calendarViewTypeLengthWeightMeasurement ||
                    CalendarViewType == CalendarViewType.calendarViewTypeMaturityMeasurement)
                {
                    _speciesMeasurementCalendarDays = await GetMeasuredSpeciesCalendarDayForMonthTask(_selectedMonth, CalendarViewType, getFemaleMaturity: _getFemaleMaturity);
                    HashSet<SpeciesFishingGearAndSector> msfgss = new HashSet<SpeciesFishingGearAndSector>();
                    foreach (var item in _speciesMeasurementCalendarDays)
                    {
                        FishingGearAndSector fgs = new FishingGearAndSector(g: item.Gear, sector_code: item.SectorCode);
                        var msfgs = new SpeciesFishingGearAndSector(fgs, item.SpeciesID, item.SpeciesName, item.TaxaCode);
                        if (_getFemaleMaturity)
                        {
                            msfgs.MaturityStage = item.MaturityStage;
                            msfgs.MaturityStageEnum = item.MaturityStageEnum;
                        }
                        //msfgss.Add(new SpeciesFishingGearAndSector(fgs, item.SpeciesID, item.SpeciesName, item.TaxaCode ));
                        msfgss.Add(msfgs);
                    }
                    if (_getFemaleMaturity)
                    {
                        try
                        {
                            _measured_speciesFishingGearAndSectors = msfgss
                                .OrderBy(t => t.Taxa)
                                .ThenBy(t => t.SpeciesName)
                                .ThenBy(t => t.MaturityStageEnum)
                                .ThenBy(t => t.FishingGearAndSector.Gear.GearName)
                                .ThenBy(t => t.FishingGearAndSector.Sector)
                                .ToList();
                        }
                        catch//(Exception ex)
                        {
                            //Utilities.Logger.Log(ex);
                        }
                    }
                    else
                    {
                        _measured_speciesFishingGearAndSectors = msfgss.ToList();
                    }
                }

                DataTable.Columns.Add("Taxa");
                DataTable.Columns.Add("Species");
                if (_getFemaleMaturity)
                {
                    DataTable.Columns.Add("Maturity stage");
                }
            }
            DataTable.Columns.Add("Gear name");
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
                    TotalMeasurementCount = 0;
                    SpeciesCalendarDay species_day = null;
                    foreach (var msp_gr_sec in _measured_speciesFishingGearAndSectors.Where(t => t.FishingGearAndSector.SectorCode == "c" || t.FishingGearAndSector.SectorCode == "m").OrderBy(t => t.TaxaCode).ThenBy(t => t.SpeciesName).ThenBy(t => t.FishingGearAndSector.Gear.GearName))
                    {
                        row = DataTable.NewRow();
                        row["Taxa"] = msp_gr_sec.Taxa;
                        row["Species"] = msp_gr_sec.SpeciesName;
                        if (_getFemaleMaturity)
                        {
                            row["Maturity stage"] = msp_gr_sec.MaturityStage;
                        }
                        row["Gear name"] = msp_gr_sec.FishingGearAndSector.Gear.GearName;
                        row["GearCode"] = msp_gr_sec.FishingGearAndSector.Gear.Code;
                        row["Sector"] = msp_gr_sec.FishingGearAndSector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
                            //day = _speciesMeasurementCalendarDays.FirstOrDefault(
                            //    t => t.SamplingDate.Day == x && 
                            //    t.SpeciesID == msp_gr_sec.SpeciesID && 
                            //    t.GearCode == msp_gr_sec.FishingGearAndSector.Gear.Code && 
                            //    t.SectorCode == msp_gr_sec.FishingGearAndSector.SectorCode);
                            if (_getFemaleMaturity)
                            {
                                species_day = _speciesMeasurementCalendarDays.FirstOrDefault(
                                    t => t.SamplingDate.Day == x &&
                                    t.SpeciesID == msp_gr_sec.SpeciesID &&
                                    t.GearCode == msp_gr_sec.FishingGearAndSector.Gear.Code &&
                                    t.SectorCode == msp_gr_sec.FishingGearAndSector.SectorCode &&
                                    t.MaturityStage == msp_gr_sec.MaturityStage);
                            }
                            else
                            {
                                species_day = _speciesMeasurementCalendarDays.FirstOrDefault(
                                    t => t.SamplingDate.Day == x &&
                                    t.SpeciesID == msp_gr_sec.SpeciesID &&
                                    t.GearCode == msp_gr_sec.FishingGearAndSector.Gear.Code &&
                                    t.SectorCode == msp_gr_sec.FishingGearAndSector.SectorCode);
                            }
                            if (species_day == null)
                            {
                                row[x.ToString()] = null;
                            }
                            else
                            {
                                CalendarHasValue = true;
                                switch (CalendarViewType)
                                {
                                    case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                                        if (species_day.CountLenFreqMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = species_day.CountLenFreqMeas;
                                            TotalMeasurementCount += species_day.CountLenFreqMeas;
                                        }
                                        break;
                                    case CalendarViewType.calendarViewTypeLengthMeasurement:
                                        if (species_day.CountLenMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = species_day.CountLenMeas;
                                            TotalMeasurementCount += species_day.CountLenMeas;
                                        }
                                        break;
                                    case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                                        if (species_day.CountLenWtMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = species_day.CountLenWtMeas;
                                            TotalMeasurementCount += species_day.CountLenWtMeas;
                                        }
                                        break;
                                    case CalendarViewType.calendarViewTypeMaturityMeasurement:
                                        if (species_day.CountMaturityMeas == 0)
                                        {
                                            row[x.ToString()] = null;
                                        }
                                        else
                                        {
                                            row[x.ToString()] = species_day.CountMaturityMeas;
                                            TotalMeasurementCount += species_day.CountMaturityMeas;
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
                    {
                        row = DataTable.NewRow();
                        row["Taxa"] = sp_gr_sec.Taxa;
                        row["Species"] = sp_gr_sec.SpeciesName;
                        row["Gear name"] = sp_gr_sec.FishingGearAndSector.Gear.GearName;
                        row["GearCode"] = sp_gr_sec.FishingGearAndSector.Gear.Code;
                        row["Sector"] = sp_gr_sec.FishingGearAndSector.Sector;
                        row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                        for (int x = 1; x <= _numberOfDays; x++)
                        {
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
                    TotalWeightLanded = null;
                    foreach (var sp_gr_sec in _speciesFishingGearAndSectors.Where(t => t.FishingGearAndSector.SectorCode == "c" || t.FishingGearAndSector.SectorCode == "m").OrderBy(t => t.TaxaCode).ThenBy(t => t.SpeciesName).ThenBy(t => t.FishingGearAndSector.Gear.GearName))
                    {
                        row = DataTable.NewRow();
                        row["Taxa"] = sp_gr_sec.Taxa;
                        row["Species"] = sp_gr_sec.SpeciesName;
                        row["Gear name"] = sp_gr_sec.FishingGearAndSector.Gear.GearName;
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
                                row[x.ToString()] = day.WeightOfSpeciesLanded.ToString("N2");
                                TotalWeightLanded = (TotalWeightLanded ?? 0) + day.WeightOfSpeciesLanded;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeSampledLandings:
                    Utilities.Logger.LogCalendar("starting FishingCalendarDayExViewModel.MakeCalendar() switch case CalendarViewType.calendarViewTypeSampledLandings");
                    foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
                    {
                        row = DataTable.NewRow();
                        row["Gear name"] = gear_sector.Gear.GearName;
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
                    Utilities.Logger.LogCalendar($"finished adding rows to datatable with { DataTable.Rows.Count} rows added");
                    Utilities.Logger.LogCalendar("ending FishingCalendarDayExViewModel.MakeCalendar() switch case CalendarViewType.calendarViewTypeSampledLandings");
                    break;

                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                    TotalLandingCount = null;
                    foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
                    {
                        row = DataTable.NewRow();
                        row["Gear name"] = gear_sector.Gear.GearName;
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
                    var list_gear_sector = _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode).ToList();
                    foreach (var gear_sector in list_gear_sector)
                    //foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
                    {
                        row = DataTable.NewRow();
                        row["Gear name"] = gear_sector.Gear.GearName;
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
                                    landedWeight = (day.TotalWeightCommercialLandings ?? 0) + (day.TotalWeightMunicipalLandings ?? 0);
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
                    row["Gear name"] = "All gears";
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
                    row["Gear name"] = "All gears";
                    row["GearCode"] = "All sectors";
                    row["Sector"] = "";
                    row["Month"] = samplingMonthYear.ToString("MMM-yyyy");

                    for (int x = 1; x <= _numberOfDays; x++)
                    {
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

            Utilities.Logger.LogCalendar("end FishingCalendarDayExViewModel.MakeCalendar()");
            return true;
        }

        public List<LandingSiteSampling> LandingSiteSamplings
        {
            get
            {
                HashSet<LandingSiteSampling> llss = new HashSet<LandingSiteSampling>();
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


        public Task<List<SpeciesCalendarDay>> GetMeasuredSpeciesCalendarDayForMonthTask(AllSamplingEntitiesEventHandler e, CalendarViewType viewType, bool getFemaleMaturity)
        {
            return Task.Run(() => GetMeasuredSpeciesCalendarDayForMonth(e, viewType, getFemaleMaturity));
        }
        public List<SpeciesCalendarDay> GetMeasuredSpeciesCalendarDayForMonth(AllSamplingEntitiesEventHandler e, CalendarViewType viewType, bool getFemaleMaturity)
        {
            return Repository.GetMeasuredSpeciesCalendarDays(e, viewType, getFemaleMaturity);
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
            Utilities.Logger.LogCalendar("start FishingCalendarDayExViewModel.GetCalendarDaysForMonth");
            CalendarEvent?.Invoke(null, new MakeCalendarEventArg { Context = "Fetching landing data from database" });
            _selectedMonth = selectedMonth;

            Utilities.Logger.LogCalendar("start building CalendarDaysDictionary");
            if (CalendarDaysDictionary.Keys.Count == 0 || !CalendarDaysDictionary.Keys.Contains(selectedMonth.GUID))
            {
                //var result = await Repository.GetCalendarDaysAsync(selectedMonth);
                //if(result.Count==0)
                //{
                //    Utilities.Logger.LogCalendar("Getting calendar days direct method (not await-async)");
                //    result = Repository.GetCalendarDays(selectedMonth);
                //}
                CalendarDaysDictionary.Add(selectedMonth.GUID, await Repository.GetCalendarDaysAsync(selectedMonth));
                //CalendarDaysDictionary.Add(selectedMonth.GUID, result);
                UniqueGearListDictionary.Add(selectedMonth.GUID, Repository.UniqueGearSectorList.ToList());
                VesselUnloadIDsDictionary.Add(selectedMonth.GUID, Repository.GetVesselUnloadIDsOfCalendar(selectedMonth));
            }
            Utilities.Logger.LogCalendar($"end building CalendarDaysDictionary with {CalendarDaysDictionary.Count} items");

            _fishingCalendarDays = CalendarDaysDictionary[selectedMonth.GUID];
            _gearsAndSectors = UniqueGearListDictionary[selectedMonth.GUID];
            _vesselUnloads = VesselUnloadIDsDictionary[selectedMonth.GUID];
            CalendarEvent?.Invoke(null, new MakeCalendarEventArg { Context = "Fetched landing data from database" });

            Utilities.Logger.LogCalendar($"FishingCalendarDayExViewModel.GetCalendarDaysForMonth returning with {_fishingCalendarDays.Count} items");
            Utilities.Logger.LogCalendar("end FishingCalendarDayExViewModel.GetCalendarDaysForMonth");
            return _fishingCalendarDays;
        }
    }
}
