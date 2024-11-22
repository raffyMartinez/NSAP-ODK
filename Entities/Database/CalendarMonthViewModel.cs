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
        private Dictionary<string, string> _speciesTaxa = new Dictionary<string, string>();
        public DataTable SamplingCalendarDataTable { get; private set; }
        public DataTable LandingsWeightDataTable { get; private set; }

        public bool CalendarHasData { get; set; }
        public CalendarViewType CalendarOption { get; set; }

        public int TotalVesselUnloadCount { get; set; }

        public int TotalLandingCount { get; set; }

        public double TotalLandedCatchWeight { get; set; }

        public double TotalWeightLanded { get; set; }

        public bool CalendarHasValue { get; set; }

        public int TotalMeasurementCount { get; set; }

        public string LocationLabel
        {
            get
            {
                return $"{_allSamplingEntitiesEventHandler.LandingSite}, {_allSamplingEntitiesEventHandler.FishingGround}, {_allSamplingEntitiesEventHandler.FMA}, {_allSamplingEntitiesEventHandler.NSAPRegion}";
            }
        }
        public string SamplingCalendarTitle
        {
            get
            {
                int rows = 0;
                string labelCountWeight = "";
                switch (CalendarOption)
                {
                    case CalendarViewType.calendarViewTypeSampledLandings:
                        labelCountWeight = $"Total number of sampled landings: {_calendarMonthRepository.TotalLandingsSampled}";
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                        labelCountWeight = $"Total number of landings by gear and sector: {_calendarMonthRepository.TotalCountAllLandedGears}";
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                        labelCountWeight = $"Total weight of catch of sampled landings by gear and sector: {_calendarMonthRepository.TotalLandedCatchWeight}";
                        break;
                    case CalendarViewType.calendarViewTypeCountAllLandings:
                        labelCountWeight = $"Total number of landings for all gears and sectors: {_calendarMonthRepository.TotalLandingCount}";
                        break;
                    case CalendarViewType.calendarViewTypeWeightAllLandings:
                        labelCountWeight = $"Total weight of catch of landings for all gears and sectors: {_calendarMonthRepository.TotalWeightLanded}";
                        break;
                    case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                        labelCountWeight = $"Total weight of watched species of landings for all gears and sectors: {((double)TotalWeightLanded).ToString("N2")}";
                        break;
                    case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                        //labelCountWeight = $"Total weight of watched species of landings for all gears and sectors: {TotalWeightLanded}";
                        break;
                    case CalendarViewType.calendarViewTypeLengthMeasurement:
                        if (CalendarHasData)
                        {
                            labelCountWeight = $"Number of length measurements: {_calendarMonthRepository.TotalCountMeasurements}";
                        }
                        else
                        {
                            labelCountWeight = "No length measurements were done";
                        }
                        
                        break;
                    case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                        if (CalendarHasData)
                        {
                            labelCountWeight = $"Number of length weight measurements: {_calendarMonthRepository.TotalCountMeasurements}";
                        }
                        else
                        {
                            labelCountWeight = "No length weight measurements were done";
                        }
                        
                        break;
                    case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                        if (CalendarHasData)
                        {
                            labelCountWeight = $"Number of length frequency measurements: {_calendarMonthRepository.TotalCountMeasurements}";
                        }
                        else
                        {
                            labelCountWeight = "No length frequency measurements were done";
                        }
                        
                        break;
                    case CalendarViewType.calendarViewTypeMaturityMeasurement:
                        string female_maturity = "";
                        if (IsFemaleMaturity)
                        {
                            female_maturity = "for females ";
                        }
                        if (CalendarHasData)
                        {
                            labelCountWeight = $"Number of maturity measurements {female_maturity}(length, weight, sex, and maturity stage): {_calendarMonthRepository.TotalCountMeasurements}";
                        }
                        else
                        {
                            labelCountWeight = $"No maturity measurements {female_maturity}(length, weight, sex, and maturity stage) were done";
                        }
                        
                        
                        break;
                }
                rows = _calendarMonthRepository.RowCount;
                string restDayIndicator = "";

                if (SamplingRestDays.Count > 0)
                {
                    restDayIndicator = "(Blue columns represent rest day)";
                }


                //var label = $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";
                if (_calendarMonthRepository.CalendarHasData)
                {
                    //return $"Rows: {_gearsAndSectors.Where(t => t.Sector != "Other").Count()}, {labelCountWeight} {restDayIndicator}";
                    return $"Rows: {rows}, {labelCountWeight} {restDayIndicator}";
                }
                else
                {
                    if (IsWatchedSpeciesCalendar)
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

        public List<int> SamplingRestDays { get; private set; }
        public bool IsWatchedSpeciesCalendar { get; set; }

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

        private void CapitalizeFirstLetter(ref string[] input)
        {

            for (int x = 0; x < input.Length; x++)
            {
                input[x] = char.ToUpper(input[x][0]) + input[x].Substring(1);
            }

        }
        private void SetupDataTableColumns()
        {
            SamplingCalendarDataTable = new DataTable();
            LandingsWeightDataTable = new DataTable();
            string measurement = "";

            DataColumn dc;
            if (IsWatchedSpeciesCalendar)
            {
                dc = new DataColumn { ColumnName = "Taxa", DataType = typeof(string) };
                SamplingCalendarDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
                SamplingCalendarDataTable.Columns.Add(dc);
                if (MeasurementType.Length > 0)
                {
                    dc = new DataColumn { ColumnName = "Measurement", DataType = typeof(string) };
                    SamplingCalendarDataTable.Columns.Add(dc);
                    var arr = MeasurementType.Split(new char[] { ' ', '-' });
                    CapitalizeFirstLetter(ref arr);
                    measurement = string.Join(" ", arr);

                    if (IsFemaleMaturity)
                    {
                        dc = new DataColumn { ColumnName = "Stage", DataType = typeof(string) };
                        SamplingCalendarDataTable.Columns.Add(dc);
                    }
                }
            }

            dc = new DataColumn { ColumnName = "Gear name", DataType = typeof(string) };
            SamplingCalendarDataTable.Columns.Add(dc);
            //LandingsWeightDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "GearCode", DataType = typeof(string) };
            SamplingCalendarDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector", DataType = typeof(string) };
            SamplingCalendarDataTable.Columns.Add(dc);
            //LandingsWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month", DataType = typeof(string) };
            SamplingCalendarDataTable.Columns.Add(dc);



            DateTime monthSampled = (DateTime)_allSamplingEntitiesEventHandler.MonthSampled;
            int daysInMonth = DateTime.DaysInMonth(monthSampled.Year, monthSampled.Month);
            for (int x = 1; x <= daysInMonth; x++)
            {
                //SamplingCalendarDataTable.Columns.Add(new DataColumn { ColumnName = x.ToString(), DataType = typeof(int) });
                SamplingCalendarDataTable.Columns.Add(new DataColumn { ColumnName = x.ToString(), DataType = typeof(double) });
                //LandingsWeightDataTable.Columns.Add(new DataColumn { ColumnName = x.ToString(), DataType = typeof(double) });
            }
            DataRow row = null;
            switch (CalendarOption)
            {
                case CalendarViewType.calendarViewTypeCountAllLandings:
                case CalendarViewType.calendarViewTypeWeightAllLandings:
                    row = SamplingCalendarDataTable.NewRow();
                    row["Gear name"] = "All gears";
                    row["GearCode"] = "";
                    row["Sector"] = "";
                    row["Month"] = ((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMM-yyyy");
                    for (int x = 1; x <= daysInMonth; x++)
                    {
                        var day = CalendarDailySummary.CalendarDays.Find(t => t.Day.Day == x);
                        if (day != null)
                        {
                            switch (CalendarOption)
                            {
                                case CalendarViewType.calendarViewTypeCountAllLandings:
                                    row[x.ToString()] = day.CountOfFishingOperations;
                                    break;
                                case CalendarViewType.calendarViewTypeWeightAllLandings:
                                    row[x.ToString()] = day.TotalWeightOfCatch;
                                    break;
                            }
                        }
                    }
                    SamplingCalendarDataTable.Rows.Add(row);
                    break;
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                    if (IsFemaleMaturity)
                    {
                        var stages = GetStages();
                        foreach (var gearSector in GearSectors.Where(t => t.NoFishing == false))
                        {
                            foreach (var sp in ListSpeciesCaught(gearSector))
                            {
                                bool dayFound = false;
                                foreach (var stage in stages.Where(t => t != null))
                                {
                                    row = SamplingCalendarDataTable.NewRow();
                                    row["Taxa"] = _speciesTaxa[sp];
                                    row["Species"] = sp;
                                    row["Measurement"] = measurement;
                                    row["Stage"] = Utilities.Global.MaturityStageFromCode(stage);
                                    row["Gear name"] = gearSector.Gear.GearName;
                                    row["GearCode"] = gearSector.Gear.Code;
                                    row["Sector"] = gearSector.Sector;
                                    row["Month"] = ((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMM-yyyy");
                                    for (int x = 1; x <= daysInMonth; x++)
                                    {
                                        var gs = gearSector.CalendarDays.Find(t => t.Day.Day == x);
                                        if (gs != null && gs.CalendarDaySpeciesesMeasuredFemale != null)
                                        {
                                            {
                                                var species_measured = gs.CalendarDaySpeciesesMeasuredFemale.Find(
                                                    t => t.SpeciesName == sp &&
                                                    t.MaturityStage != null &&
                                                    t.MaturityStage == stage);
                                                if (species_measured != null)
                                                {
                                                    row[x.ToString()] = species_measured.CountMeasured;
                                                    dayFound = true;
                                                }
                                            }
                                        }
                                    }
                                    if (dayFound)
                                    {
                                        SamplingCalendarDataTable.Rows.Add(row);
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        foreach (var gearSector in GearSectors.Where(t => t.NoFishing == false))
                        {
                            foreach (var sp in ListSpeciesCaught(gearSector))
                            {
                                row = SamplingCalendarDataTable.NewRow();
                                row["Taxa"] = _speciesTaxa[sp];
                                row["Species"] = sp;
                                if (MeasurementType.Length > 0)
                                {
                                    row["Measurement"] = measurement;
                                }
                                row["Gear name"] = gearSector.Gear.GearName;
                                row["GearCode"] = gearSector.Gear.Code;
                                row["Sector"] = gearSector.Sector;
                                row["Month"] = ((DateTime)_allSamplingEntitiesEventHandler.MonthSampled).ToString("MMM-yyyy");

                                for (int x = 1; x <= daysInMonth; x++)
                                {
                                    var gs = gearSector.CalendarDays.Find(t => t.Day.Day == x);
                                    if (gs != null && (gs.CalendarDaySpecieses != null || gs.CalendarDaySpeciesesMeasured != null))
                                    {

                                        CalendarDaySpecies species = null;
                                        CalendarDaySpeciesMeasured species_measured = null;
                                        if (MeasurementType.Length > 0)
                                        {
                                            if (gs.CalendarDaySpeciesesMeasured != null)
                                            {
                                                species_measured = gs.CalendarDaySpeciesesMeasured.Find(t => t.SpeciesName == sp);
                                            }
                                        }
                                        else
                                        {
                                            if (gs.CalendarDaySpecieses != null)
                                            {
                                                species = gs.CalendarDaySpecieses.Find(t => t.SpeciesName == sp);
                                            }
                                        }
                                        if (species != null || species_measured != null)
                                        {
                                            switch (CalendarOption)
                                            {
                                                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                                                    row[x.ToString()] = species.CountLandings;
                                                    break;
                                                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                                                    row[x.ToString()] = species.WeightLanded;
                                                    break;
                                                case CalendarViewType.calendarViewTypeLengthMeasurement:
                                                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                                                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                                                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                                                    if (IsFemaleMaturity)
                                                    {
                                                        row["Stage"] = species_measured.MaturityStage;
                                                    }
                                                    row[x.ToString()] = species_measured.CountMeasured;

                                                    break;
                                            }
                                        }
                                    }
                                }
                                SamplingCalendarDataTable.Rows.Add(row);
                            }
                        }
                    }
                    break;
                default:
                    foreach (var gearSector in GearSectors.Where(t => t.NoFishing == false))
                    {
                        row = SamplingCalendarDataTable.NewRow();
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
                                switch (CalendarOption)
                                {
                                    case CalendarViewType.calendarViewTypeSampledLandings:
                                        row[x.ToString()] = gs.CountOfFishingOperations;
                                        break;
                                    case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                                        row[x.ToString()] = gs.TotalWeightOfCatch;
                                        break;
                                    case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                                        row[x.ToString()] = gs.CountNumberOfLandings;
                                        break;
                                }

                            }
                        }

                        SamplingCalendarDataTable.Rows.Add(row);

                    }
                    break;
                    //row = LandingsWeightDataTable.NewRow();
                    //row["Gear"] = gearSector.Gear.GearName;
                    //row["Sector"] = gearSector.Sector;
                    //for (int x = 1; x <= daysInMonth; x++)
                    //{
                    //    row[x] = gearSector.CalendarDays.Find(t => t.Day.Day == x).TotalWeightOfCatch;
                    //}
            }

        }
        public List<string> GetStages()
        {
            HashSet<string> stages = new HashSet<string>();
            foreach (var item in GearSectors)
            {
                foreach (CalendarDay day in item.CalendarDays)
                {
                    if (day.CalendarDaySpeciesesMeasuredFemale != null)
                    {
                        foreach (CalendarDaySpeciesMeasured cdsm in day.CalendarDaySpeciesesMeasuredFemale)
                        {
                            stages.Add(cdsm.MaturityStage);
                        }
                    }
                }
            }
            return stages.OrderBy(t => t).ToList();
        }
        public List<string> ListSpeciesCaught(CalendarGearSector gearSector)
        {
            _speciesTaxa = new Dictionary<string, string>();
            HashSet<string> speciesNames = new HashSet<string>();
            foreach (var day in gearSector.CalendarDays)
            {
                if (MeasurementType.Length > 0)
                {
                    if (IsFemaleMaturity)
                    {
                        if (day.CalendarDaySpeciesesMeasuredFemale != null)
                        {
                            foreach (var sp in day.CalendarDaySpeciesesMeasuredFemale)
                            {
                                string sp_name = "";
                                bool addSuccess = false;
                                string taxa = "";
                                if (sp.Taxa_code == "FIS")
                                {
                                    sp_name = sp.FishSpecies.ToString();
                                    addSuccess = speciesNames.Add(sp_name);
                                    taxa = "Fish";
                                }
                                else
                                {
                                    sp_name = sp.NotFishSpecies.ToString();
                                    addSuccess = speciesNames.Add(sp_name);
                                    taxa = NSAPEntities.TaxaViewModel.GetTaxa(sp.Taxa_code).ToString();
                                }
                                if (addSuccess)
                                {
                                    _speciesTaxa.Add(sp_name, taxa);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (day.CalendarDaySpeciesesMeasured != null)
                        {
                            foreach (var sp in day.CalendarDaySpeciesesMeasured)
                            {
                                string sp_name = "";
                                bool addSuccess = false;
                                string taxa = "";
                                if (sp.Taxa_code == "FIS")
                                {
                                    sp_name = sp.FishSpecies.ToString();
                                    addSuccess = speciesNames.Add(sp_name);
                                    taxa = "Fish";
                                }
                                else
                                {
                                    sp_name = sp.NotFishSpecies.ToString();
                                    addSuccess = speciesNames.Add(sp_name);
                                    taxa = NSAPEntities.TaxaViewModel.GetTaxa(sp.Taxa_code).ToString();
                                }
                                if (addSuccess)
                                {
                                    _speciesTaxa.Add(sp_name, taxa);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (day.CalendarDaySpecieses != null)
                    {
                        foreach (var sp in day.CalendarDaySpecieses)
                        {
                            string sp_name = "";
                            bool addSuccess = false;
                            string taxa = "";
                            if (sp.Taxa_code == "FIS")
                            {
                                sp_name = sp.FishSpecies.ToString();
                                addSuccess = speciesNames.Add(sp_name);
                                taxa = "Fish";
                            }
                            else
                            {
                                sp_name = sp.NotFishSpecies.ToString();
                                addSuccess = speciesNames.Add(sp_name);
                                taxa = NSAPEntities.TaxaViewModel.GetTaxa(sp.Taxa_code).ToString();
                            }
                            if (addSuccess)
                            {
                                _speciesTaxa.Add(sp_name, taxa);
                            }
                        }
                    }
                }
            }
            return speciesNames.OrderBy(t => t).ToList();
        }
        public CalendarDailySummary CalendarDailySummary { get; set; }

        public string MeasurementType { get; private set; }
        public bool IsFemaleMaturity { get; private set; }
        public bool CanCreateCalendar
        {
            get
            {
                return GearSectors != null && GearSectors.Count > 0;
            }
        }
        public CalendarMonthViewModel(
            AllSamplingEntitiesEventHandler e,
            CalendarViewType viewOption,
            bool isWatchedSpeciesCalendar = false,
            string measurementType = "",
            bool isFemaleMaturity = false
            )
        {
            IsWatchedSpeciesCalendar = isWatchedSpeciesCalendar;
            CalendarOption = viewOption;
            MeasurementType = measurementType;
            IsFemaleMaturity = isFemaleMaturity;
            _allSamplingEntitiesEventHandler = e;
            _calendarMonthRepository = new CalendarMonthRepository(_allSamplingEntitiesEventHandler, CalendarOption, IsFemaleMaturity);

            switch (CalendarOption)
            {
                case CalendarViewType.calendarViewTypeSampledLandings:
                case CalendarViewType.calendarViewTypeWeightAllLandingsByGear:
                case CalendarViewType.calendarViewTypeCountAllLandingsByGear:
                    GearSectors = _calendarMonthRepository.CalendarGearSectors;
                    break;
                case CalendarViewType.calendarViewTypeWeightAllLandings:
                case CalendarViewType.calendarViewTypeCountAllLandings:
                    CalendarDailySummary = _calendarMonthRepository.CalendarDailySummary;
                    break;

                case CalendarViewType.calendarViewTypeWatchedSpeciesLandings:
                case CalendarViewType.calendarViewTypeWatchedSpeciesLandedWeight:
                    GearSectors = _calendarMonthRepository.CalendarGearSectors;
                    break;
                case CalendarViewType.calendarViewTypeLengthMeasurement:
                case CalendarViewType.calendarViewTypeLengthFrequencyMeasurement:
                case CalendarViewType.calendarViewTypeLengthWeightMeasurement:
                case CalendarViewType.calendarViewTypeMaturityMeasurement:
                    GearSectors = _calendarMonthRepository.CalendarGearSectors;
                    break;
            }

            CalendarHasData = _calendarMonthRepository.CalendarHasData;
            SamplingRestDays = CalendarMonthRepository.SamplingRestDays;
            if (CalendarHasData)
            {
                SetupDataTableColumns();
            }


        }

        public List<CalendarGearSector> GearSectors { get; private set; }
    }
}
