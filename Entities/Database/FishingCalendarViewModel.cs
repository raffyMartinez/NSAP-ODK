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

    class FishingCalendarDay
    {
        public List<bool> IsProcessed { get; set; }
        public Gear Gear { get; set; }
        public string GearName { get; set; }
        public DateTime MonthYear { get; set; }
        public List<GearUnload> GearUnloads { get; set; }
        public List<int?> NumberOfBoatsPerDay { get; set; }
        public List<double?> TotalCatchPerDay { get; set; }
        public List<bool> SamplingDay { get; set; }

        public List<int?> NumberOfSampledLandings { get; set; }

        public override string ToString()
        {
            return $"{GearName} - {GearUnloads.Where(t => t != null).Count()} days";
        }
    }
    class FishingCalendarViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


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

        public DataTable DataTable { get; set; }

        public void BuildCalendar(string view = "numberOfSampledLandings")
        {
            DataTable = new DataTable();
            DataTable.Columns.Add("GearName");
            DataTable.Columns.Add("GearCode");
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
                row["Month"] = item.MonthYear.ToString("MMM-yyyy");
                int counter = 1;
                switch (view)
                {
                    case "numberOfSampledLandings":

                        foreach (var day in item.NumberOfSampledLandings)
                        {
                            if (day != null)
                            {

                            }
                            row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case "numberOfBoatsLanded":
                        foreach (var day in item.NumberOfBoatsPerDay)
                        {
                            row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case "totalCatchPerDay":
                        foreach (var day in item.TotalCatchPerDay)
                        {
                            row[counter.ToString()] = day == null ? "" : day.ToString();
                            counter++;
                        }
                        break;
                    case "gearUnloadItem":

                        break;
                }
                DataTable.Rows.Add(row);
            }
        }

        public List<GearUnload> UnloadList { get; private set; }
        public FishingCalendarViewModel(List<GearUnload> unloadList)
        {
            UnloadList = unloadList;

            if (UnloadList.Count > 0)
            {

                int c = 0;
                List<string> gearNames = new List<string>();
                var fishingCalendarDays = new ObservableCollection<FishingCalendarDay>();

                DateTime samplingMonthYear = UnloadList[0].Parent.SamplingDate;
                _numberOfDays = DateTime.DaysInMonth(samplingMonthYear.Year, samplingMonthYear.Month);


                FishingCalendarDay calendarDay = null;
                foreach (var item in UnloadList.OrderBy(t => t.GearUsedName).ThenBy(t => t.Parent.SamplingDate))
                {

                    if (!gearNames.Contains(item.GearUsedName))
                    {

                        calendarDay = new FishingCalendarDay
                        {
                            Gear = item.Gear,
                            MonthYear = samplingMonthYear,
                            GearName = item.GearUsedName
                        };

                        calendarDay.GearUnloads = new List<GearUnload>();
                        calendarDay.NumberOfBoatsPerDay = new List<int?>();
                        calendarDay.TotalCatchPerDay = new List<double?>();
                        calendarDay.IsProcessed = new List<bool>();
                        calendarDay.NumberOfSampledLandings = new List<int?>();
                        
                        gearNames.Add(item.GearUsedName);

                        for (int n = 1; n <= _numberOfDays; n++)
                        {

                            if (item.Parent.SamplingDate.Day == n)
                            {
                                calendarDay.GearUnloads.Add(item);
                                calendarDay.NumberOfBoatsPerDay.Add(item.Boats);
                                calendarDay.TotalCatchPerDay.Add(item.Catch);
                                //calendarDay.NumberOfSampledLandings.Add(item.ListVesselUnload.Count);
                                calendarDay.NumberOfSampledLandings.Add(VesselUnloadCount(n, item.GearUsedName));
                                calendarDay.IsProcessed.Add(true);
                            }
                            else
                            {
                                calendarDay.GearUnloads.Add(null);
                                calendarDay.NumberOfBoatsPerDay.Add(null);
                                calendarDay.TotalCatchPerDay.Add(null);
                                calendarDay.NumberOfSampledLandings.Add(null);
                                calendarDay.IsProcessed.Add(false);
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
                            calendarDay.TotalCatchPerDay[day] = item.Catch;
                            //calendarDay.NumberOfSampledLandings[day] = item.ListVesselUnload.Count;
                            calendarDay.NumberOfSampledLandings[day] = VesselUnloadCount(day+1, item.GearUsedName);
                            calendarDay.IsProcessed[day] = true;
                        }
                    }
                }

                FishingCalendarList = fishingCalendarDays;
                BuildCalendar();
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
            
            return UnloadList.Where(t=>t.Parent.SamplingDate.Day==day && t.GearUsedName==gear).Sum(t=>t.ListVesselUnload.Count);

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
