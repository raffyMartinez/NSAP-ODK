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
        public static event EventHandler<CrossTabReportEventArg> FishingGroundMappingEvent;
        public static AllSamplingEntitiesEventHandler EntitiesMonth { get; set; }
        private static MapInterActionHandler _mapInterActionHandler;

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

            var sf = ShapefileFactory.ConvexHull(ShapefileFactory.PointsToPolyline((Shapefile)fishingGroundPoints.LayerObject), values);
            sf.Key = "convex hull from fg points";
            _mapInterActionHandler.MapLayersHandler.AddLayer(sf, $"Convex hull of {fishingGroundPoints.Name}", layerKey: sf.Key);
        }

        public static string MappingContext { get; set; }
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
                    layerName = $"{gearName} ({sector}) {EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM-yyyy")}";
                    break;
                case "menuCalendarDayGearMapping":

                    vus = CrossTabGenerator.VesselUnloads.Where(
                        t => t.FirstFishingGroundCoordinate != null &&
                        t.Parent.GearUsedName == gearName &&
                        t.Sector == sector &&
                        t.SamplingDate.Date == samplingDate)
                     .ToList();

                    layerName = $"{gearName} ({sector}) {EntitiesMonth.LandingSite}, {((DateTime)samplingDate).ToString("MMM-dd-yyyy")}";
                    break;
                case "menuCalendarDayFemaleMaturityMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    layerName = $"{SpeciesName} ({MaturityStage}) {GearName} {EntitiesMonth.LandingSite}, {((DateTime)samplingDate).ToString("MMM-dd-yyyy")}";
                    break;
                case "menuCalendarGearSpeciesMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    layerName = $"{SpeciesName}  {GearName} {EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM - yyyy")}";
                    break;
                case "menuCalendarSpeciesMapping":
                    foreach (int id in vesselUnloadIDs)
                    {
                        vus.Add(CrossTabGenerator.VesselUnloads.Find(t => t.PK == id));
                    }
                    layerName = $"{SpeciesName} {EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM - yyyy")}";
                    break;
                case "contextMenuMapMonth":
                    vus = CrossTabGenerator.VesselUnloads.Where(t => t.FirstFishingGroundCoordinate != null).ToList();
                    layerName = $"{EntitiesMonth.LandingSite}, {((DateTime)EntitiesMonth.MonthSampled).ToString("MMM - yyyy")}";
                    break;
            }
            if (vus.Count > 0)
            {
                NumberOfLandings = vus.Count;
                WeightLandedCatch = vus.Sum(t => t.WeightOfCatch ?? 0);
                FishingGroundMappingEvent?.Invoke(null, new CrossTabReportEventArg { Context = "creating fishing ground point shapefile" });
                var fishingGroundPointsSF = ShapefileFactory.FishingGroundPointsFromCalendarSampledMonth(vus, EntitiesMonth, out handles);

                _mapInterActionHandler.MapLayersHandler.AddLayer(fishingGroundPointsSF, layerName, layerKey: fishingGroundPointsSF.Key);
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
