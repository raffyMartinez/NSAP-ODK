using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;
using NSAP_ODK.TreeViewModelControl;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.CrossTabBuilder;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Mapping
{
    public static class FishingGroundPointsFromCalendarMappingManager
    {
        public static DateTime? _samplingDate;
        public static event EventHandler<CrossTabReportEventArg> FishingGroundMappingEvent;
        public static AllSamplingEntitiesEventHandler EntitiesMonth { get; set; }
        private static MapInterActionHandler _mapInterActionHandler;

        public static void SetSamplingDate(int? calendarDay)
        {
            if(calendarDay!=null)
            {
                _samplingDate = ((DateTime)EntitiesMonth.MonthSampled).AddDays((int)calendarDay - 1);
            }
        }

        public static string SpeciesName { get; set; }
        public static string MaturityStage { get; set; }
        public static string GearName { get; set; }
        public static string Sector { get; set; }
        public static int? CalendarDay { get; set; }
        public static int NumberOfLandings { get; set; }
        public static double WeightLandedCatch { get; set; }
        public static MapInterActionHandler MapInteractionHandler
        {
            get { return _mapInterActionHandler; }
            set
            {
                _mapInterActionHandler = value;
                _mapInterActionHandler.ShapesSelected += _mapInterActionHandler_ShapesSelected;
            }
        }

        public static void Cleanup()
        {
            _mapInterActionHandler.ShapesSelected -= _mapInterActionHandler_ShapesSelected;
            _mapInterActionHandler = null;
        }
        private static void _mapInterActionHandler_ShapesSelected(MapInterActionHandler s, LayerEventArg e)
        {

        }

        public static void CreateConvexHullFromFishingGroundPoints(MapLayer fishingGroundPoints)
        {
            List<ShapefileFieldTypeValue> values = new List<ShapefileFieldTypeValue>();
            Shapefile sf_lo = (Shapefile)fishingGroundPoints.LayerObject;

            string level = sf_lo.get_CellValue(sf_lo.FieldIndexByName["Level"], 0);
            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.STRING_FIELD,
                    Name = "Level",
                    Value = level,
                    Precision = 1,
                    Width = 1
                });

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.STRING_FIELD,
                    Name = "AreaName",
                    Alias = "Area name",
                    Value = fishingGroundPoints.Name,
                    Precision = 1,
                    Width = 1
                });

            if (level == "Month" || level == "MonthGear")
            {
                values.Add(
                    new ShapefileFieldTypeValue
                    {
                        FieldType = FieldType.DATE_FIELD,
                        Name = "MonthSampled",
                        Alias = "Month of sampling",
                        Precision = 1,
                        Width = 1,
                        Value = ((DateTime)sf_lo.get_CellValue(sf_lo.FieldIndexByName["SamplingDate"], 0)).ToString("MMM/yyyy")
                    });
            }
            else
            {
                values.Add(
                    new ShapefileFieldTypeValue
                    {
                        FieldType = FieldType.DATE_FIELD,
                        Name = "SamplingDate",
                        Alias = "Sampling date",
                        Precision = 1,
                        Width = 1,
                        Value = (DateTime)sf_lo.get_CellValue(sf_lo.FieldIndexByName["SamplingDate"], 0)
                    });
            }

            if (level != "Month")
            {
                values.Add(
                    new ShapefileFieldTypeValue
                    {
                        FieldType = FieldType.STRING_FIELD,
                        Name = "Gear",
                        Precision = 1,
                        Width = 1,
                        Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["Gear"], 0)
                    });
            }

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.STRING_FIELD,
                    Name = "Region",
                    Precision = 1,
                    Width = 1,
                    Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["Region"], 0)
                });

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.STRING_FIELD,
                    Name = "FMA",
                    Precision = 1,
                    Width = 1,
                    Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["FMA"], 0)
                });

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.STRING_FIELD,
                    Name = "FishGrnd",
                    Alias = "Fishing ground",
                    Precision = 1,
                    Width = 1,
                    Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["FishGrnd"], 0)
                });

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.STRING_FIELD,
                    Name = "LandingSite",
                    Alias = "Landing site",
                    Precision = 1,
                    Width = 1,
                    Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["LndgSite"], 0)
                });

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.INTEGER_FIELD,
                    Name = "CtAllLd",
                    Alias = "Number of sampled landings",
                    Precision = 1,
                    Width = 1,
                    Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["CtAllLd"], 0)
                });

            values.Add(
                new ShapefileFieldTypeValue
                {
                    FieldType = FieldType.DOUBLE_FIELD,
                    Name = "TotCatch",
                    Alias = "Weight of sampled landings",
                    Precision = 5,
                    Width = 7,
                    Value = sf_lo.get_CellValue(sf_lo.FieldIndexByName["TotCatch"], 0)
                });

            var sf = ShapefileFactory.ConvexHull(ShapefileFactory.PointsToPolyline((Shapefile)fishingGroundPoints.LayerObject), values,lineColor:sf_lo.DefaultDrawingOptions.FillColor);
            sf.Key = "convex hull from fg points";
            _mapInterActionHandler.MapLayersHandler.AddLayer(sf, $"Convex hull of {fishingGroundPoints.Name}", layerKey: sf.Key);
        }
        public static string MappingContext2 { get; set; }
        public static string MappingContext { get; set; }
        public static string Description { get; set; }
        public static async Task<bool> MapFishingGroundPoint(string gearName = "", string sector = "", int? calendarDay = null, List<int> vesselUnloadIDs = null)
        {
            bool success = false;
            string layerName = "";
            if (_mapInterActionHandler == null)
            {
                throw new ArgumentNullException("MapInteractionHandler is null");
            }

            await CrossTabGenerator.GenerateCrossTabTask(EntitiesMonth, generateDataset: false);

            List<int> handles;
            List<VesselUnload> vus = null;

            vus = new List<VesselUnload>();
            DateTime? samplingDate = null;
            if (CalendarDay != null)
            {
                samplingDate = new DateTime(
                        ((DateTime)EntitiesMonth.MonthSampled).Year,
                        ((DateTime)EntitiesMonth.MonthSampled).Month,
                        (int)CalendarDay);
            }
            string description = Description.Replace("Map fishing", "Fishing");
            //if (!string.IsNullOrEmpty(MappingContext2))
            //{
            //    var arr = MappingContext2.Split(':');
            //    if(MappingContext2.Contains("maturity"))
            //    {
            //        if (arr.Length == 3)
            //        {
            //            if (arr[2] == "by_day_species")
            //            {
            //                layerName = $"Fishing ground of {SpeciesName} with {arr[1].ToLower()} maturity stage at {EntitiesMonth.LandingSite} on {((DateTime)_samplingDate).ToString("MMM dd, yyyy")}";
            //            }
            //        }
            //        else
            //        {
            //            layerName = $"Fishing ground of {SpeciesName} with {arr[1].ToLower()} maturity stage at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //        }
            //    }
            //    else if(MappingContext2.Contains("measured"))
            //    {
            //        if(arr.Length==3)
            //        {
            //            if(arr[2]=="by_day_species")
            //            {
            //                layerName = $"Fishing ground of {SpeciesName} with {arr[1]} measurements at {EntitiesMonth.LandingSite} on {((DateTime)_samplingDate).ToString("MMM dd, yyyy")}";
            //            }
            //        }
            //        else
            //        {
            //            layerName = $"Fishing ground of {SpeciesName} with {arr[1]} measurements at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //        }
            //    }
            //     switch (MappingContext2)
            //    {
            //        case "sector_month_allspecies":
            //            layerName = $"Fishing ground of {Sector.ToLower()} fishing gears at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //            break;
            //        //case "measured:length":
            //        //    layerName = $"Fishing ground of {SpeciesName} with length measurements at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //        //    break;
            //        //case "measured:length:by_day_species":
            //        //    break;
            //        //case "measured:length-weight":
            //        //    layerName = $"Fishing ground of {SpeciesName} with length-weight measurements at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //        //    break;
            //        //case "measured:length-weight:by_day_species":
            //        //    break;
            //        //case "measured:length frequency":
            //        //    layerName = $"Fishing ground of {SpeciesName} with length frequency measurements at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //        //    break;
            //        //case "measured:length frequency:by_day_species":
            //        //    break;
            //        //case "measured:maturity":
            //        //    layerName = $"Fishing ground of {SpeciesName} with maturity measurements at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //        //    break;
            //        //case "measured:maturity:by_day_species":
            //        //    break;
            //        case "occurence":
            //            if (string.IsNullOrEmpty(GearName))
            //            {
            //                layerName = $"Fishing ground of {SpeciesName} caught using {GearName.ToLower()} at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //            }
            //            else
            //            {
            //                layerName = $"Fishing ground of {SpeciesName} at {EntitiesMonth.LandingSite} on {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
            //            }
            //            break;
            //        //case "maturity:Premature":
            //        //    break;
            //        //case "maturity:Premature:by_day_species":
            //        //    break;
            //        //case "maturity:Immature":
            //        //    break;
            //        //case "maturity:Immature:by_day_species":
            //        //    break;
            //        //case "maturity:Developing":
            //        //    break;
            //        //case "maturity:Developing:by_day_species":
            //        //    break;
            //        //case "maturity:Maturing":
            //        //    break;
            //        //case "maturity:Maturing:by_day_species":
            //        //    break;
            //        //case "maturity:Mature":
            //        //    break;
            //        //case "maturity:Mature:by_day_species":
            //        //    break;
            //        //case "maturity:Ripening":
            //        //    break;
            //        //case "maturity:Ripening:by_day_species":
            //        //    break;
            //        //case "maturity:Gravid":
            //        //    break;
            //        //case "maturity:Gravid:by_day_species":
            //        //    break;
            //        //case "maturity:Spawning":
            //        //    break;
            //        //case "maturity:Spawning:by_day_species":
            //        //    break;
            //        //case "maturity:Spent":
            //        //    break;
            //        //case "maturity:Spent:by_day_species":
            //        //    break;
            //    }
            //}
            switch (MappingContext)
            {
                case "menuCalendarDaySpeciesGearMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    break;
                case "menuCalendarGearMapping":
                    vus = CrossTabGenerator.VesselUnloads.Where(t => t.FirstFishingGroundCoordinate != null && t.Parent.GearUsedName == gearName && t.Sector == sector).ToList();
                    //layerName = $"{gearName} ({sector}) {EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
                    break;
                case "menuCalendarDayGearMapping":

                    vus = CrossTabGenerator.VesselUnloads.Where(
                        t => t.FirstFishingGroundCoordinate != null &&
                        t.Parent.GearUsedName == gearName &&
                        t.Sector == sector &&
                        t.SamplingDate.Date == samplingDate)
                     .ToList();

                    //if (string.IsNullOrEmpty(layerName))
                    //{
                    //    layerName = $"{gearName} ({sector}) {EntitiesMonth.LandingSite}, {((DateTime)samplingDate).ToString("MMM-dd-yyyy")}";
                    //}
                    break;
                case "menuCalendarDayFemaleMaturityMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    //layerName = $"{SpeciesName} ({MaturityStage}) {GearName} {EntitiesMonth.LandingSite}, {((DateTime)samplingDate).ToString("MMM-dd-yyyy")}";
                    break;
                case "menuCalendarGearSpeciesMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    //if (string.IsNullOrEmpty(layerName))
                    //{
                    //    layerName = $"{SpeciesName}  {GearName} {EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM - yyyy")}";
                    //}
                    break;
                case "menuCalendarSpeciesMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    //layerName = $"{SpeciesName} {EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM - yyyy")}";
                    break;
                case "contextMenuMapMonth":
                    vus = CrossTabGenerator.VesselUnloads.Where(t => t.FirstFishingGroundCoordinate != null).ToList();
                    //layerName = $"{EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM - yyyy")}";
                    break;
            }
            if (vus.Count > 0)
            {
                NumberOfLandings = vus.Count;
                WeightLandedCatch = vus.Sum(t => t.WeightOfCatch ?? 0);
                FishingGroundMappingEvent?.Invoke(null, new CrossTabReportEventArg { Context = "creating fishing ground point shapefile" });
                var fishingGroundPointsSF = ShapefileFactory.FishingGroundPointsFromCalendarSampledMonth(vus, EntitiesMonth, out handles);

                //_mapInterActionHandler.MapLayersHandler.AddLayer(fishingGroundPointsSF, layerName, layerKey: fishingGroundPointsSF.Key);
                _mapInterActionHandler.MapLayersHandler.AddLayer(fishingGroundPointsSF, description, layerKey: fishingGroundPointsSF.Key);
                FishingGroundMappingEvent?.Invoke(null, new CrossTabReportEventArg { Context = "fishing ground point shapefile created" });
                success = true;
            }
            else
            {
                success = false;
            }
            return success;
        }
    }
}
