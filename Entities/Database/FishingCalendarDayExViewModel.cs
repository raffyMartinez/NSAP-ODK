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

        public FishingCalendarRepository Repository { get; set; }
        public Dictionary<string, List<FishingCalendarDayEx>> CalendarDaysDictionary { get; private set; } = new Dictionary<string, List<FishingCalendarDayEx>>();
        public Dictionary<string, List<FishingGearAndSector>> UniqueGearListDictionary { get; private set; } = new Dictionary<string, List<FishingGearAndSector>>();
        public FishingCalendarDayExViewModel()
        {
            Repository = new FishingCalendarRepository();
        }
        public DataTable DataTable { get; set; }

        public bool DayIsSamplingDay(int day)
        {
            var sd = _fishingCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == day);
            if (sd == null)
            {
                return true;
            }
            else
            {
                return sd.IsSamplingDay;
            }
        }
        public void MakeCalendar()
        {
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

            foreach (var gear_sector in _gearsAndSectors.Where(t => t.SectorCode == "c" || t.SectorCode == "m").OrderBy(t => t.Gear.GearName).ThenBy(t => t.SectorCode))
            {
                var row = DataTable.NewRow();
                row["GearName"] = gear_sector.Gear.GearName;
                row["GearCode"] = gear_sector.Gear.Code;
                row["Sector"] = gear_sector.Sector;
                row["Month"] = samplingMonthYear.ToString("MMM-yyyy");
                int counter = 1;

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
                    }
                    //int? samplings = _fishingCalendarDays.FirstOrDefault(t => t.SamplingDate.Day == x && t.Gear == gear_sector.Gear && t.SectorCode == gear_sector.SectorCode).TotalNumberOfLandingsSampled;
                    //if(samplings==null)
                    //{
                    //    row[x] = null;
                    //}
                    //else
                    //{
                    //    row[x]= samplings;
                    //}
                    //row[x] = samplings;


                }
                DataTable.Rows.Add(row);
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
