using NSAP_ODK.Entities.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.TreeViewModelControl;
using System.Windows;
using System.Data;

namespace NSAP_ODK.Entities.CrossTabBuilder
{
    public static class CrossTabGenerator
    {
        public static DataTable EffortDataTable { get; private set; }
        public static AllSamplingEntitiesEventHandler EntitiesOfMonth { get; set; }
        public static Dictionary<string, List<VesselUnload>> VesselUnloadsDictionary { get; private set; } = new Dictionary<string, List<VesselUnload>>();
        public static Dictionary<string, List<VesselEffortCrossTab>> VesselEffortDictionary { get; private set; } = new Dictionary<string, List<VesselEffortCrossTab>>();
        public static Dictionary<string, List<VesselUnload_FishingGear>> VesselUnloadGearDictionary { get; private set; } = new Dictionary<string, List<VesselUnload_FishingGear>>();
        public static Dictionary<string, List<VesselCatch>> VesselCatchDictionary { get; private set; } = new Dictionary<string, List<VesselCatch>>();
        public static List<VesselUnload> VesselUnloads { get; set; }
        public static List<VesselUnload_FishingGear> VesselUnload_FishingGears { get; set; }
        public static List<VesselEffortCrossTab> VesselEffortCrossTabs { get; set; }
        public static List<VesselCatch> VesselCatches { get; set; }
        public static List<VesselEffortCrossTab> GetVesselEffortCrossTabsFromRepository()
        {
            VesselEffortCrossTabs = VesselEffortRepository.GetEffortForCrossTab(EntitiesOfMonth);
            return VesselEffortCrossTabs;
        }
        public static void GetFromRepository()
        {
            VesselUnloads = VesselUnloadRepository.GetVesselUnloads(EntitiesOfMonth);
            var gear_unloads = NSAPEntities.SummaryItemViewModel.GetGearUnloadsFromTree(EntitiesOfMonth);
            foreach (VesselUnload vu in VesselUnloads)
            {
                vu.Parent = gear_unloads.Find(t => t.PK == vu.GearUnloadID);
                //gear_unloads.Remove(vu.Parent);
            }

            VesselEffortCrossTabs = VesselEffortRepository.GetEffortForCrossTab(EntitiesOfMonth);
            foreach (VesselEffortCrossTab vect in VesselEffortCrossTabs)
            {
                vect.VesselUnload = VesselUnloads.Find(t => t.PK == vect.VesselUnloadID);
            }
            VesselUnload_FishingGears = VesselUnload_FishingGearRepository.GetFishingGears(EntitiesOfMonth);
            foreach (VesselUnload_FishingGear vufg in VesselUnload_FishingGears)
            {
                vufg.Parent = VesselUnloads.Find(t => t.PK == vufg.ParentID);
            }
            VesselCatches = VesselCatchRepository.GetVesselCatchForCrosstab(EntitiesOfMonth);
            foreach (VesselCatch vc in VesselCatches)
            {
                vc.Parent = VesselUnloads.Find(t => t.PK == vc.VesselUnloadID);
            }
            AddToDictionary();
        }
        public static void GetVesselUnloadsFromSummaries()
        {
            var gus = NSAPEntities.SummaryItemViewModel.GetGearUnloadsFromTree(EntitiesOfMonth);
            if (gus?.Count > 0)
            {
                VesselUnloads = new List<VesselUnload>();
            }
            foreach (GearUnload gu in gus)
            {
                VesselUnloads.AddRange(gu.ListVesselUnload);
            }
            AddToDictionary();
        }
        public static void GenerateCrossTab(AllSamplingEntitiesEventHandler entitiesDefinition)
        {
            GetEntities(entitiesDefinition);
            //GenerateEffortDataTable();
        }
        public static void GenerateEffortSpeciesDataTable()
        {

        }
        private static void GenerateEffortDataTable()
        {
            EffortDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
            EffortDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Successful fishing operation", DataType = typeof(bool) };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            EffortDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Municipality" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator" };
            EffortDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Sector" };
            EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Grid location" };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            //EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Weight of catch of gear" };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "# species in catch of gear" };
            //EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Ref #" };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Has catch composition", DataType = typeof(bool) };
            EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Fishing vessel" };
            //EffortDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            //EffortDataTable.Columns.Add(dc);



            //dc = new DataColumn { ColumnName = "Catch composition count", DataType = typeof(int) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            //EffortDataTable.Columns.Add(dc);



            //dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
            //EffortDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Include effort indicators", DataType = typeof(bool) };
            //EffortDataTable.Columns.Add(dc);

            foreach (var spec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection.OrderBy(t => t.Name))
            {
                dc = new DataColumn { ColumnName = spec.Name.Replace("/", " or ") };
                switch (spec.ValueType)
                {
                    case ODKValueType.isBoolean:
                        dc.DataType = typeof(bool);
                        break;
                    case ODKValueType.isDecimal:
                        dc.DataType = typeof(double);
                        break;
                    case ODKValueType.isInteger:
                        dc.DataType = typeof(int);
                        break;
                    case ODKValueType.isText:
                    case ODKValueType.isUndefined:
                        dc.DataType = typeof(string);
                        break;
                }
                EffortDataTable.Columns.Add(dc);
            }

            foreach (var vu in VesselUnloads)
            {

                var row = EffortDataTable.NewRow();
                if (vu.GearSoakViewModel == null)
                {
                    vu.GearSoakViewModel = new GearSoakViewModel(vu);
                }
                var lss = vu.Parent.Parent;
                var ls = lss.LandingSite;
                row["Data ID"] = vu.PK;
                row["Successful fishing operation"] = vu.OperationIsSuccessful;
                row["Fishing ground"] = lss.FishingGround;
                row["Year"] = vu.SamplingDate.Year;
                row["Month"] = vu.SamplingDate.Month;
                row["Date"] = vu.SamplingDate.ToString("dd-MMM-yyyy");


                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;


                row["Landing site"] = ls;
                row["Enumerator"] = vu.NSAPEnumerator;

                row["Region"] = lss.NSAPRegion;
                row["FMA"] = lss.FMA;

                row["Sector"] = vu.Sector;


                //if (vu.FishingGroundGrid != null)
                //{
                //    row["Grid location"] = ctcp.FishingGroundGrid;
                //    row["Longitude"] = ctcp.xCoordinate;
                //    row["Latitude"] = ctcp.yCoordinate;
                //}


                row["Gear"] = vu.Parent.Gear;

                //if (ctcp.DateTimeGearSet == null || ctcp.DateTimeGearHaul == null)
                //{
                //    row["Date of set"] = DBNull.Value;
                //    row["Date of haul"] = DBNull.Value;
                //}
                //else
                //{
                //    row["Date of set"] = ctcp.DateTimeGearSet;
                //    row["Date of haul"] = ctcp.DateTimeGearHaul;
                //}

                //row["Weight of catch of gear"] = ctcp.GearCatchWeight;
                //row["# species in catch of gear"] = ctcp.GearCatchSpeciesCount;
                row["Ref #"] = vu.RefNo;
                row["Has catch composition"] = vu.HasCatchComposition;
                row["Is a fishing boat used"] = vu.IsBoatUsed;
                //row["Fishing vessel"] = vu.FBName;

                if (vu.NumberOfFishers == null)
                {
                    row["# of fishers"] = DBNull.Value;
                }
                else
                {
                    row["# of fishers"] = vu.NumberOfFishers;
                }

                //if (ctcp.FBL != null)
                //{
                //    row["Fishing vessels landed"] = ctcp.FBL;
                //}


                //row["Fishing vessels monitored"] = ctcp.FBM;



                //row["Catch composition count"] = ctcp.VesselUnload.ListVesselCatch.Count;
                //if (ctcp.TotalWeight == null)
                //{
                //    row["Total weight of catch"] = DBNull.Value;
                //}
                //else
                //{
                //    row["Total weight of catch"] = ctcp.TotalWeight;
                //}

                //row["Is the catch sold"] = ctcp.IsCatchSold;
                //row["Notes"] = ctcp.Notes;
                //row["Include effort indicators"] = ctcp.IncludeEffortIndicators;
                foreach (var vufg in vu.ListUnloadFishingGears)
                {
                    foreach (var gs in vufg.VesselUnload_Gear_Specs_ViewModel.VesselUnload_Gear_SpecCollection)
                    {
                        string spec_name = "";
                        try
                        {
                            if (gs.EffortSpecification != null)
                            {
                                spec_name = gs.EffortSpecification.Name.Replace("/", " or ");

                                switch (gs.EffortSpecification.ValueType)
                                {
                                    case ODKValueType.isBoolean:

                                        row[spec_name] = bool.Parse(gs.EffortValue);
                                        break;
                                    case ODKValueType.isDecimal:

                                        row[spec_name] = double.Parse(gs.EffortValue);
                                        break;
                                    case ODKValueType.isInteger:

                                        row[spec_name] = int.Parse(gs.EffortValue);
                                        break;
                                    case ODKValueType.isText:
                                    case ODKValueType.isUndefined:

                                        row[spec_name] = gs.EffortValue;
                                        break;
                                }
                            }
                        }
                        catch (System.FormatException)
                        {
                            if (gs.EffortValue.Length > 0 && gs.EffortSpecification.ValueType == ODKValueType.isInteger)
                            {
                                if (double.TryParse(gs.EffortValue, out double r))
                                {
                                    row[spec_name] = (int)r;
                                }
                                else
                                {
                                    row[spec_name] = DBNull.Value;
                                }
                            }
                            else
                            {
                                row[spec_name] = DBNull.Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.Logger.Log(ex);
                        }
                    }
                    EffortDataTable.Rows.Add(row);
                }

            }
        }
        public static void GetEntities(AllSamplingEntitiesEventHandler entities)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            EntitiesOfMonth = entities;
            VesselUnloads = new List<VesselUnload>();
            VesselEffortCrossTabs = new List<VesselEffortCrossTab>();
            VesselUnload_FishingGears = new List<VesselUnload_FishingGear>();
            VesselCatches = new List<VesselCatch>();
            try
            {
                VesselUnloads = VesselUnloadsDictionary[EntitiesOfMonth.GUID];
                VesselEffortCrossTabs = VesselEffortDictionary[EntitiesOfMonth.GUID];
                VesselUnload_FishingGears = VesselUnloadGearDictionary[EntitiesOfMonth.GUID];
                VesselCatches = VesselCatchDictionary[EntitiesOfMonth.GUID];
            }
            catch
            {
                GetFromRepository();
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            MessageBox.Show($"execution time:{elapsedMs} ms");

        }
        public static void AddToDictionary()
        {

            if (VesselUnloadsDictionary.Keys.Count == 0 || !VesselUnloadsDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                VesselUnloadsDictionary.Add(EntitiesOfMonth.GUID, VesselUnloads);
            }
            if (VesselEffortDictionary.Keys.Count == 0 || !VesselEffortDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                VesselEffortDictionary.Add(EntitiesOfMonth.GUID, VesselEffortCrossTabs);
            }
            if (VesselUnloadGearDictionary.Keys.Count == 0 || !VesselUnloadGearDictionary.Keys.Contains(EntitiesOfMonth.GUID))
            {
                VesselUnloadGearDictionary.Add(EntitiesOfMonth.GUID, VesselUnload_FishingGears);
            }

        }
    }
}
