using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
namespace NSAP_ODK.Entities.Database
{
    public enum CalendarViewType
    {
        calendarViewTypeSampledLandings,
        calendarViewTypeCountAllLandingsByGear,
        calendarViewTypeWeightAllLandings,
        calendarViewTypeWeightAllLandingsByGear,
        calendarViewTypeCountAllLandings,
        calendarViewTypeGearDailyLandings,
        calendarViewTypeCarrierLandings
    }


    public class FishingCalendarDay
    {
        public List<bool> HasLanding { get; set; }
        public string CarrierBoatName { get; set; }
        public List<bool> IsProcessed { get; set; }
        public Gear Gear { get; set; }

        public string Sector { get; set; } //added oct 21 2022
        public string GearName { get; set; }
        public DateTime MonthYear { get; set; }

        public List<CarrierLanding> CarrierLandings { get; set; }
        public List<GearUnload> GearUnloads { get; set; }
        public List<int?> CountGearTypes { get; set; }
        public List<int?> NumberOfBoatsPerDay { get; set; }
        public List<double?> TotalCatchPerDay { get; set; }
        public List<bool?> SamplingDay { get; set; }

        public List<int?> NumberOfSampledLandings { get; set; }

        public override string ToString()
        {
            return $"{GearName} - Sector {Sector} - {GearUnloads.Where(t => t != null).Count()} days";
        }
    }
    public class FishingCalendarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TreeViewModelControl.AllSamplingEntitiesEventHandler _e;
        private ObservableCollection<FishingCalendarDay> _fishingCalendarList;
        private int _numberOfDays;
        public ObservableCollection<FishingCalendarDay> FishingCalendarList
        {

            get { return _fishingCalendarList; }
            set
            {
                if (_fishingCalendarList != value)
                {
                    _fishingCalendarList = value;
                    OnPropertyChanged("FishingCalendarList");
                }
            }
        }
        public int CountVesselUnloads { get; set; }
        public DataTable DataTable { get; set; }

        public void BuildCarrierLandingCalendar()
        {
            DataTable = new DataTable();
            DataTable.Columns.Add("CarrierName");
            for (int n = 1; n <= _numberOfDays; n++)
            {
                DataTable.Columns.Add(n.ToString());
            }
            foreach (var item in _fishingCalendarList)
            {
                var row = DataTable.NewRow();
                row["CarrierName"] = item.CarrierBoatName;
                int counter = 1;
                foreach(var day in item.HasLanding)
                {
                    if (day)
                    {
                        row[counter.ToString()] = "x";
                    }
                    else
                    {
                        row[counter.ToString()] = "";
                    }
                    counter++;
                }
                DataTable.Rows.Add(row);
            }
        }
        public void BuildCalendar(CalendarViewType calendarView)
        {
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

            foreach (var item in _fishingCalendarList)
            {
                var row = DataTable.NewRow();
                row["GearName"] = item.GearName;
                row["GearCode"] = item.Gear != null ? item.Gear.Code : string.Empty;
                row["Sector"] = item.Sector;
                row["Month"] = item.MonthYear.ToString("MMM-yyyy");
                int counter = 1;




                switch (calendarView)
                {
                    case CalendarViewType.calendarViewTypeGearDailyLandings:
                        foreach (var day in item.NumberOfBoatsPerDay)
                        {
                            row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case CalendarViewType.calendarViewTypeSampledLandings:

                        foreach (var day in item.NumberOfSampledLandings)
                        {
                            if (day != null)
                            {
                                CountVesselUnloads += (int)day;
                                row[counter.ToString()] = day.ToString();
                            }
                            else
                            {
                                row[counter.ToString()] = "";
                            }
                            //row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                        foreach (var day in item.NumberOfBoatsPerDay)
                        {
                            if (day != null)
                            {
                                CountVesselUnloads += (int)day;
                                row[counter.ToString()] = day.ToString();
                            }
                            else
                            {
                                row[counter.ToString()] = "";
                            }
                            //row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                        foreach (var day in item.TotalCatchPerDay)
                        {
                            row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandings:
                        foreach (var day in item.NumberOfBoatsPerDay)
                        {
                            row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;

                }
                DataTable.Rows.Add(row);
            }
        }

        public List<bool?> ListDayIsSamplingDay { get; set; }
        public List<GearUnload> UnloadList { get; private set; }
        public List<Day_GearLanded> Day_GearLandedList { get; private set; }

        public FishingCalendarViewModel(List<CarrierLanding> landings, DateTime monthYear, LandingSite ls)
        {
            var fishingCalendarDays = new ObservableCollection<FishingCalendarDay>();
            _numberOfDays = DateTime.DaysInMonth(monthYear.Year, monthYear.Month);


            FishingCalendarDay fishingCalendarDay = null;
            List<string> carrierNames = new List<string>();
            foreach (CarrierLanding cl in landings.OrderBy(t => t.CarrierName).ThenBy(t => t.SamplingDate))
            {
                if (!carrierNames.Contains(cl.CarrierName))
                {
                    fishingCalendarDay = new FishingCalendarDay
                    {
                        CarrierBoatName = cl.CarrierName,
                        MonthYear = monthYear
                    };
                    fishingCalendarDay.HasLanding = new List<bool>();
                    fishingCalendarDay.IsProcessed = new List<bool>();
                    fishingCalendarDay.CarrierLandings = new List<CarrierLanding>();
                    
                    for (int n = 1; n <= _numberOfDays; n++)
                    {
                        if(cl.SamplingDate.Day==n)
                        {
                            fishingCalendarDay.HasLanding.Add(true);
                            fishingCalendarDay.IsProcessed.Add(true);
                            fishingCalendarDay.CarrierLandings.Add(cl);
                        }
                        else
                        {
                            fishingCalendarDay.HasLanding.Add(false);
                            fishingCalendarDay.IsProcessed.Add(false);
                            fishingCalendarDay.CarrierLandings.Add(null);
                        }
                    }
                    fishingCalendarDays.Add(fishingCalendarDay);
                    carrierNames.Add(cl.CarrierName);
                }
                else
                {
                    int day = cl.SamplingDate.Day - 1;
                    //if (!fishingCalendarDay.IsProcessed[day])
                    //{
                    //    //fishingCalendarDay.HasLanding[day] = true;
                    //}
                    //else
                    //{

                    //}
                    fishingCalendarDay.CarrierLandings[day] = cl;
                     fishingCalendarDay.HasLanding[day] = true;
                }

            }

            FishingCalendarList = fishingCalendarDays;
            //BuildCalendar(CalendarViewType.calendarViewTypeCarrierLandings);
            BuildCarrierLandingCalendar();
        }
        public FishingCalendarViewModel(List<Day_GearLanded> day_GearLandedList, DateTime monthYear, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            _e = e;
            Day_GearLandedList = day_GearLandedList;
            var fishingCalendarDays = new ObservableCollection<FishingCalendarDay>();
            DateTime samplingMonthYear = monthYear;
            _numberOfDays = DateTime.DaysInMonth(samplingMonthYear.Year, samplingMonthYear.Month);

            ListDayIsSamplingDay = new List<bool?>();
            List<LandingSiteSampling> list_lss = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSamplings(_e.FMA, _e.FishingGround, _e.LandingSite, (DateTime)_e.MonthSampled);
            for (int n = 1; n <= _numberOfDays; n++)
            {
                var lss = list_lss.FirstOrDefault(t => t.SamplingDate.Day == n);
                if (lss != null)
                {
                    ListDayIsSamplingDay.Add(lss.IsSamplingDay);
                }
                else
                {
                    ListDayIsSamplingDay.Add(null);
                }

            }


            if (day_GearLandedList.Count > 0)
            {
                List<string> gearNames = new List<string>();
                FishingCalendarDay calendarDay = null;
                string sector = "";
                foreach (var item in Day_GearLandedList
                    .Where(t => t.GearInLandingSite != null)
                    .OrderBy(t => t.GearInLandingSite.GearUsedName)
                    .ThenBy(t => t.GearInLandingSite.SectorCode)
                    .ThenBy(t => t.LandingSiteSampling.SamplingDate))
                {
                    //if (item.GearInLandingSite != null)
                    //{
                    if (!gearNames.Contains(item.GearInLandingSite.GearAndSector))
                    {
                        switch (item.GearInLandingSite.SectorCode)
                        {
                            case "c":
                                sector = "Commercial";
                                break;
                            case "m":
                                sector = "Municipal";
                                break;
                            default:
                                sector = "Not defined";
                                break;
                        }


                        calendarDay = new FishingCalendarDay
                        {
                            Gear = item.GearInLandingSite.Gear,
                            MonthYear = samplingMonthYear,
                            GearName = item.GearInLandingSite.GearUsedName,
                            Sector = sector,
                        };
                        calendarDay.NumberOfBoatsPerDay = new List<int?>();
                        calendarDay.TotalCatchPerDay = new List<double?>();
                        calendarDay.IsProcessed = new List<bool>();

                        gearNames.Add(item.GearAndSector);

                        for (int n = 1; n <= _numberOfDays; n++)
                        {
                            if (item.LandingSiteSampling.SamplingDate.Day == n)
                            {
                                calendarDay.NumberOfBoatsPerDay.Add(item.Boats);
                                calendarDay.TotalCatchPerDay.Add(item.CatchWt);
                                calendarDay.IsProcessed.Add(true);

                            }
                            else
                            {
                                calendarDay.NumberOfBoatsPerDay.Add(null);
                                calendarDay.TotalCatchPerDay.Add(null);
                                calendarDay.IsProcessed.Add(false);
                            }

                        }
                        fishingCalendarDays.Add(calendarDay);
                    }
                    else
                    {
                        int day = item.LandingSiteSampling.SamplingDate.Day - 1;
                        if (!calendarDay.IsProcessed[day])
                        {
                            calendarDay.NumberOfBoatsPerDay[day] = item.Boats;
                            calendarDay.TotalCatchPerDay[day] = item.CatchWt;
                            calendarDay.IsProcessed[day] = true;
                        }
                    }
                }
                FishingCalendarList = fishingCalendarDays;
                BuildCalendar(CalendarViewType.calendarViewTypeGearDailyLandings);
            }
        }

        public void UpdateCalendarDay(int numberOfBoats, Gear gear, int dayOfMonth)
        {

        }

        
        public FishingCalendarViewModel(List<GearUnload> unloadList, CalendarViewType calendarView, DateTime monthYear, TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            _e = e;
            UnloadList = unloadList;


            var fishingCalendarDays = new ObservableCollection<FishingCalendarDay>();
            DateTime samplingMonthYear = monthYear;

            _numberOfDays = DateTime.DaysInMonth(samplingMonthYear.Year, samplingMonthYear.Month);

            ListDayIsSamplingDay = new List<bool?>();
            List<LandingSiteSampling> list_lss = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSamplings(_e.FMA, _e.FishingGround, _e.LandingSite, (DateTime)_e.MonthSampled);
            for (int n = 1; n <= _numberOfDays; n++)
            {
                var lss = list_lss.FirstOrDefault(t => t.SamplingDate.Day == n);
                if (lss != null)
                {
                    ListDayIsSamplingDay.Add(lss.IsSamplingDay);
                }
                else
                {
                    ListDayIsSamplingDay.Add(null);
                }
            }




            if (UnloadList.Count > 0)
            {
                int c = 0;
                List<string> gearNames = new List<string>();


                //DateTime samplingMonthYear = UnloadList[0].Parent.SamplingDate;
                //_numberOfDays = DateTime.DaysInMonth(samplingMonthYear.Year, samplingMonthYear.Month);


                FishingCalendarDay calendarDay = null;
                foreach (var item in UnloadList.OrderBy(t => t.GearUsedName).ThenBy(t => t.SectorCode).ThenBy(t => t.Parent.SamplingDate))
                {

                    //if (!gearNames.Contains(item.GearUsedName))
                    if (!gearNames.Contains(item.GearAndSector))
                    {

                        string sector;
                        switch (item.SectorCode)
                        {
                            case "c":
                                sector = "Commercial";
                                break;
                            case "m":
                                sector = "Municipal";
                                break;
                            default:
                                sector = "Not defined";
                                break;
                        }


                        calendarDay = new FishingCalendarDay
                        {
                            Gear = item.Gear,
                            MonthYear = samplingMonthYear,
                            GearName = item.GearUsedName,
                            Sector = sector,
                        };

                        calendarDay.GearUnloads = new List<GearUnload>();
                        calendarDay.SamplingDay = new List<bool?>();
                        calendarDay.NumberOfBoatsPerDay = new List<int?>();
                        calendarDay.TotalCatchPerDay = new List<double?>();
                        calendarDay.IsProcessed = new List<bool>();
                        calendarDay.NumberOfSampledLandings = new List<int?>();

                        //gearNames.Add(item.GearUsedName);
                        gearNames.Add(item.GearAndSector);

                        for (int n = 1; n <= _numberOfDays; n++)
                        {
                            if (item.Parent.SamplingDate.Day == n)
                            {
                                calendarDay.GearUnloads.Add(item);
                                calendarDay.NumberOfBoatsPerDay.Add(item.Boats);
                                calendarDay.TotalCatchPerDay.Add(item.Catch);
                                calendarDay.NumberOfSampledLandings.Add(item.ListVesselUnload.Count);
                                //calendarDay.NumberOfSampledLandings.Add(VesselUnloadCount(n, item.GearUsedName));
                                //calendarDay.NumberOfSampledLandings.Add(item.ListVesselUnload.Count);
                                calendarDay.IsProcessed.Add(true);
                                //calendarDay.SamplingDay.Add(item.Parent.IsSamplingDay);
                            }
                            else
                            {
                                calendarDay.GearUnloads.Add(null);
                                calendarDay.NumberOfBoatsPerDay.Add(null);
                                calendarDay.TotalCatchPerDay.Add(null);
                                calendarDay.NumberOfSampledLandings.Add(null);
                                calendarDay.IsProcessed.Add(false);
                                //calendarDay.SamplingDay.Add(null);
                                //break;
                            }

                        }

                        fishingCalendarDays.Add(calendarDay);

                    }
                    else
                    {
                        int day = item.Parent.SamplingDate.Day - 1;
                        if (!calendarDay.IsProcessed[day])
                        {
                            calendarDay.GearUnloads[day] = item;
                            calendarDay.NumberOfBoatsPerDay[day] = item.Boats;
                            if (item.Boats == null && item.Parent.GearUnloadViewModel != null)
                            {
                                calendarDay.NumberOfBoatsPerDay[day] = item.Parent.GearUnloadViewModel.Count;
                            }
                            calendarDay.TotalCatchPerDay[day] = item.Catch;
                            calendarDay.NumberOfSampledLandings[day] = item.ListVesselUnload.Count;
                            //calendarDay.NumberOfSampledLandings[day] = VesselUnloadCount(day + 1, item.GearUsedName);
                            calendarDay.IsProcessed[day] = true;
                            //calendarDay.SamplingDay[day] = item.Parent.IsSamplingDay;
                        }
                        else
                        {

                            //fishingCalendarDays[day].GearUnloads.Add(item);
                            //var g = calendarDay.GearUnloads[day];
                            //g.ListVesselUnload.AddRange(item.ListVesselUnload);
                            //calendarDay.GearUnloads[day].ListVesselUnload.AddRange(item.ListVesselUnload);
                            calendarDay.NumberOfBoatsPerDay[day] += item.Boats ?? 0;
                            calendarDay.TotalCatchPerDay[day] += item.Catch ?? 0;
                            calendarDay.NumberOfSampledLandings[day] += item.NumberOfSampledLandings;
                        }

                    }
                }

                FishingCalendarList = fishingCalendarDays;
                BuildCalendar(calendarView);
            }
            else
            {
                //for building empty calendar with one or two empty rows
            }
        }


        private int VesselUnloadCount(int day, string gear)
        {
            //int count = 0;
            //foreach(var unload in UnloadList.Where(t=>t.Parent.SamplingDate.Day==day && t.GearUsedName==gear))
            //{
            //    count += unload.ListVesselUnload.Count;
            //}
            //return count;

            return UnloadList.Where(t => t.Parent.SamplingDate.Day == day && t.GearUsedName == gear).Sum(t => t.ListVesselUnload.Count);
            //return UnloadList.Where(t => t.Parent.SamplingDate.Day == day && t.GearUsedName == gear).Sum(t => t.AttachedVesselUnloads.Count);

        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
