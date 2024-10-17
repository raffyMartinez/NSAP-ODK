using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NSAP_ODK.TreeViewModelControl;
using System.Data;
namespace NSAP_ODK.Entities.Database
{
    public class CalendarMonthViewModel
    {
        private CalendarMonthRepository _calendarMonthRepository;
        private AllSamplingEntitiesEventHandler _allSamplingEntitiesEventHandler;
        public DataTable LandingsCountDataTable { get; private set; }
        public DataTable LandingsWeightDataTable { get; private set; }

        public bool DayIsRestDay(int day)
        {
            try
            {
                var d = GearSectors.Find(t => t.NoFishing).CalendarDays.Find(t => t.Day.Day == day);
                var result = d != null && d.IsSamplingDay == false;
                return result;
            }
            catch
            {
                return false;
            }
        }
        private void SetupDataTableColumns()
        {
            LandingsCountDataTable = new DataTable();
            LandingsWeightDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Gear name", DataType = typeof(string) };
            LandingsCountDataTable.Columns.Add(dc);
            //LandingsWeightDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "GearCode", DataType = typeof(string) };
            LandingsCountDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector", DataType = typeof(string) };
            LandingsCountDataTable.Columns.Add(dc);
            //LandingsWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month", DataType = typeof(string) };
            LandingsCountDataTable.Columns.Add(dc);

            DateTime monthSampled = (DateTime)_allSamplingEntitiesEventHandler.MonthSampled;
            int daysInMonth = DateTime.DaysInMonth(monthSampled.Year, monthSampled.Month);
            for (int x = 1; x <= daysInMonth; x++)
            {
                LandingsCountDataTable.Columns.Add(new DataColumn { ColumnName = x.ToString(), DataType = typeof(int) });
                //LandingsWeightDataTable.Columns.Add(new DataColumn { ColumnName = x.ToString(), DataType = typeof(double) });
            }
            foreach (var gearSector in GearSectors.Where(t=>t.NoFishing==false))
            {
                var row = LandingsCountDataTable.NewRow();
                row["Gear name"] = gearSector.Gear.GearName;
                row["GearCode"] = gearSector.Gear.Code;
                row["Sector"] = gearSector.Sector;
                row["Month"] = ((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMM-yyyy");
                for (int x = 1; x <= daysInMonth; x++)
                {

                    var gs = gearSector.CalendarDays.Find(t => t.Day.Day == x);
                    if (gs != null)
                    {

                        //row[x.ToString()] = gearSector.CalendarDays.Find(t => t.Day.Day == x).CountOfFishingOperations;
                        row[x.ToString()] = gs.CountOfFishingOperations;
                    }
                }
                LandingsCountDataTable.Rows.Add(row);
                //row = LandingsWeightDataTable.NewRow();
                //row["Gear"] = gearSector.Gear.GearName;
                //row["Sector"] = gearSector.Sector;
                //for (int x = 1; x <= daysInMonth; x++)
                //{
                //    row[x] = gearSector.CalendarDays.Find(t => t.Day.Day == x).TotalWeightOfCatch;
                //}
            }

        }
        public CalendarMonthViewModel(AllSamplingEntitiesEventHandler e)
        {
            _allSamplingEntitiesEventHandler = e;
            _calendarMonthRepository = new CalendarMonthRepository(_allSamplingEntitiesEventHandler);
            GearSectors = _calendarMonthRepository.CalendarGearSectors;
            SetupDataTableColumns();

        }

        public List<CalendarGearSector> GearSectors { get; }
    }
}
