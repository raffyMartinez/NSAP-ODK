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
        private List<FishingCalendarDayEx> _fishingCalendarDays;
        private int _numberOfDays;
        private List<FishingGearAndSector> _gearsAndSectors;
        private AllSamplingEntitiesEventHandler _selectedMonth;
        public CalendarViewType CalendarViewType { get; set; }
        public int TotalVesselUnloadCount { get; set; }
        public int TotalLandingCount { get; set; }
        public double TotalLandedCatchWeight { get; set; }
        public FishingCalendarRepository Repository { get; set; }
        public Dictionary<string, List<FishingCalendarDayEx>> CalendarDaysDictionary { get; private set; } = new Dictionary<string, List<FishingCalendarDayEx>>();
        public Dictionary<string, List<FishingGearAndSector>> UniqueGearListDictionary { get; private set; } = new Dictionary<string, List<FishingGearAndSector>>();
        public FishingCalendarDayExViewModel()
        {
            Repository = new FishingCalendarRepository();
        }
        public DataTable DataTable { get; set; }

        public string LocationLabel
        {
            get
            {
                var day = _fishingCalendarDays.First();
                return $"{_selectedMonth.LandingSite}, {_selectedMonth.FishingGround}, {_selectedMonth.FMA}, {_selectedMonth.NSAPRegion}";
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
                }
                string restDayIndicator = "";
                if (_fishingCalendarDays.Where(t => !t.IsSamplingDay).Count() > 0)
                {
                    restDayIndicator = "(Blue columns represent rest day)";
                }
                return $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";

            }
        }
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
        public void MakeCalendar()
        {
            DataRow row;
            TotalVesselUnloadCount = 0;
            TotalLandingCount = 0;
            TotalLandedCatchWeight = 0;
            DateTime samplingMonthYear = (DateTime)_fishingCalendarDays.First().MonthViewModel.MonthSampled;
            _numberOfDays = DateTime.DaysInMonth(samplingMonthYear.Year, samplingMonthYear.Month);

            DataTable = new DataTable();
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
                                row[x.ToString()] = day.CountVesselUnloads;
                                TotalVesselUnloadCount += day.CountVesselUnloads;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
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
                                int landedCount;
                                if (day.CountCommercialLandings != null && day.CountMunicipalLandings != null)
                                {
                                    landedCount = (int)day.CountCommercialLandings + (int)day.CountMunicipalLandings;
                                }
                                else
                                {
                                    landedCount = (int)day.TotalNumberOfLandings;
                                }
                                row[x.ToString()] = landedCount;
                                TotalLandingCount += landedCount;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
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
                                double landedWeight;
                                if (day.TotalWeightCommercialLandings != null && day.TotalWeightMunicipalLandings != null)
                                {
                                    landedWeight = (double)day.TotalWeightCommercialLandings + (double)day.TotalWeightMunicipalLandings;
                                }
                                else
                                {
                                    landedWeight = (double)day.TotalWeightOfCatch;
                                }
                                row[x.ToString()] = landedWeight;
                                TotalLandedCatchWeight += landedWeight;
                            }
                        }
                        DataTable.Rows.Add(row);
                    }
                    break;
                case CalendarViewType.calendarViewTypeCountAllLandings:
                    row = DataTable.NewRow();
                    row["GearName"] = "All gears";
                    row["GearCode"] = "All sectors";
                    row["Sector"] = "";
                    row["Month"] = samplingMonthYear.ToString("MMM-yyyy");

                    for (int x = 1; x <= _numberOfDays; x++)
                    {
                        int landingCount = 0;
                        var day_gears = _fishingCalendarDays.Where(t => t.SamplingDate.Day == x).ToList();
                        foreach (var d in day_gears)
                        {
                            if (d.SectorCode == "m")
                            {
                                landingCount += (int)d.CountMunicipalLandings;
                            }
                            else if (d.SectorCode == "c")
                            {
                                landingCount += (int)d.CountCommercialLandings;
                            }
                        }
                        if (landingCount > 0)
                        {
                            row[x.ToString()] = landingCount;
                            TotalLandingCount += landingCount;
                        }
                    }

                    DataTable.Rows.Add(row);
                    break;

            }

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
        public GearUnload GetGearUnload(string gearName, string sector, int day)
        {
            //var l = _fishingCalendarDays.Where(t => t.Gear != null && t.Gear.GearName == gearName && t.Sector == sector && t.SamplingDate.Day == day).ToList();
            GearUnload gu = _fishingCalendarDays.Where(t => t.Gear != null && t.Gear.GearName == gearName && t.Sector == sector && t.SamplingDate.Day == day).First().GearUnload;
            return gu;
            //return new GearUnload();
        }
        public async Task<List<FishingCalendarDayEx>> GetCalendarDaysForMonth(AllSamplingEntitiesEventHandler selectedMonth)
        {
            _selectedMonth = selectedMonth;
            if (CalendarDaysDictionary.Keys.Count == 0 || !CalendarDaysDictionary.Keys.Contains(selectedMonth.GUID))
            {
                CalendarDaysDictionary.Add(selectedMonth.GUID, await Repository.GetCalendarDaysAsync(selectedMonth));
                UniqueGearListDictionary.Add(selectedMonth.GUID, Repository.UniqueGearSectorList.ToList());
            }
            _fishingCalendarDays = CalendarDaysDictionary[selectedMonth.GUID];
            _gearsAndSectors = UniqueGearListDictionary[selectedMonth.GUID];
            return _fishingCalendarDays;
        }
    }
}
