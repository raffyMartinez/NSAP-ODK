using NPOI.SS.Formula.Functions;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.TreeViewModelControl;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.CrossTabBuilder
{
    public static class CrossTabDatasetsGenerator
    {
        private static int _datatsetProcessedCount;
        public static event EventHandler<CrossTabReportEventArg> CrossTabDatasetEvent;
        public static DataTable EffortDataTable { get; private set; }
        public static DataTable DailyLandingsDataTable { get; private set; }
        public static DataTable EffortSpeciesDataTable { get; private set; }
        public static DataTable SpeciesLengthsDataTable { get; private set; }
        public static DataTable SpeciesLengthWeightDataTable { get; private set; }
        public static DataTable SpeciesLengthFreqDataTable { get; private set; }
        public static DataTable SpeciesMaturityDataTable { get; private set; }

        public static DataTable GearETPInteractionDataTable { get; private set; }

        private static DataSet _crossTabDataSet;
        public static string ErrorMessage { get; private set; }
        public static bool IsCarrierLandding { get; set; }
        public static DataSet CrossTabDataSet
        {
            get
            {
                ErrorMessage = "";
                _crossTabDataSet = new DataSet();
                try
                {
                    if (IsCarrierLandding)
                    {
                        //_dailyCarrierLandingsDataTable.TableName = "Carrier landings";
                        //_dailyCarrierCatchCompositionTable.TableName = "Catch composition";
                        //_dailyCarrierLengthWeightsDataTable.TableName = "Length weight";
                        //_dailyCarrierLenFreqTable.TableName = "Length frequency";
                        //_dailyCarrierLengthTable.TableName = "Length";
                        //_dailyCarrierMaturityTable.TableName = "Maturity";


                        //_crossTabDataSet.Tables.Add(_dailyCarrierLandingsDataTable);
                        //_crossTabDataSet.Tables.Add(_dailyCarrierCatchCompositionTable);
                        //_crossTabDataSet.Tables.Add(_dailyCarrierLengthWeightsDataTable);
                        //_crossTabDataSet.Tables.Add(_dailyCarrierLenFreqTable);
                        //_crossTabDataSet.Tables.Add(_dailyCarrierLengthTable);
                        //_crossTabDataSet.Tables.Add(_dailyCarrierMaturityTable);
                    }
                    else
                    {
                        //_effortSpeciesCrostabDataTable.TableName = "Effort";
                        EffortDataTable.TableName = "Effort (all)";
                        EffortSpeciesDataTable.TableName = "Effort";
                        DailyLandingsDataTable.TableName = "Daily landings";
                        SpeciesLengthsDataTable.TableName = "Length";
                        SpeciesLengthWeightDataTable.TableName = "Length-Weight";
                        SpeciesLengthFreqDataTable.TableName = "Length-frequency";
                        SpeciesMaturityDataTable.TableName = "Maturity";
                        GearETPInteractionDataTable.TableName = "ETP-Gear interaction";

                        _crossTabDataSet.Tables.Add(DailyLandingsDataTable);
                        _crossTabDataSet.Tables.Add(EffortDataTable);
                        _crossTabDataSet.Tables.Add(EffortSpeciesDataTable);
                        _crossTabDataSet.Tables.Add(SpeciesLengthsDataTable);
                        _crossTabDataSet.Tables.Add(SpeciesLengthWeightDataTable);
                        _crossTabDataSet.Tables.Add(SpeciesLengthFreqDataTable);
                        _crossTabDataSet.Tables.Add(SpeciesMaturityDataTable);
                        _crossTabDataSet.Tables.Add(GearETPInteractionDataTable);

                        //_crossTabDataSet.Tables.Add(_dailyLandingDataTable);
                        //_crossTabDataSet.Tables.Add(_effortCrostabDataTable);
                        //_crossTabDataSet.Tables.Add(_effortSpeciesCrostabDataTable);
                        //_crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLengths, "Length"));
                        //_crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLengthWeights, "Length-Weight"));
                        //_crossTabDataSet.Tables.Add(ListToDataTable(CrossTabMaturities, "Maturity"));
                        //_crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLenFreqs, "Len-Freq"));
                    }

                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    Logger.Log(ex);
                }
                return _crossTabDataSet;
            }
        }
        private static void GenerateEffortSpeciesDataTable()
        {

            EffortSpeciesDataTable = new DataTable();
            try
            {
                DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Month" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Province" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Municipality" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Region" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "FMA" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing ground" };
                EffortSpeciesDataTable.Columns.Add(dc);


                dc = new DataColumn { ColumnName = "Landing site" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Enumerator" };
                EffortSpeciesDataTable.Columns.Add(dc);


                dc = new DataColumn { ColumnName = "Sector" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Grid location" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Gear" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Weight of catch of gear" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "# species in catch of gear" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Ref #" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessel" };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Price", DataType = typeof(double) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Weight of species", DataType = typeof(double) };
                EffortSpeciesDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Sample weight of species (TWS)", DataType = typeof(double) };
                EffortSpeciesDataTable.Columns.Add(dc);


                dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
                EffortSpeciesDataTable.Columns.Add(dc);

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
                    EffortSpeciesDataTable.Columns.Add(dc);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            foreach (VesselCatch vc in CrossTabGenerator.VesselCatches.OrderBy(t => t.Parent.SamplingDate.Date)
                .ThenBy(t => t.GearNameUsed)
                .ThenBy(t => t.VesselUnloadID)
                .ThenBy(t => t.CatchName)
                )
            {
                var row = EffortSpeciesDataTable.NewRow();
                var ls = CrossTabGenerator.EntitiesOfMonth.LandingSite;
                row["Data ID"] = vc.VesselUnloadID;
                row["Year"] = vc.Parent.SamplingDate.Year;
                row["Month"] = vc.Parent.SamplingDate.ToString("MMMM");
                row["Date"] = vc.Parent.SamplingDate.ToString("MMMM-dd-yyyy");
                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;
                row["Ref #"] = vc.Parent.RefNo;
                row["Region"] = CrossTabGenerator.EntitiesOfMonth.NSAPRegion;
                row["FMA"] = CrossTabGenerator.EntitiesOfMonth.FMA;
                row["Fishing ground"] = CrossTabGenerator.EntitiesOfMonth.FishingGround;
                row["Landing site"] = ls;
                row["Enumerator"] = vc.Parent.EnumeratorName;
                row["Sector"] = vc.Parent.Sector;
                if (vc.Parent.FirstFishingGround != " - ")
                {
                    row["Grid location"] = vc.Parent.FirstFishingGround;
                    row["Longitude"] = vc.Parent.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = vc.Parent.FirstFishingGroundCoordinate.Latitude;
                }
                row["Gear"] = vc.ParentFishingGear.Gear;
                row["# species in catch of gear"] = vc.ParentFishingGear.CountItemsInCatchComposition;
                row["Is a fishing boat used"] = vc.Parent.IsBoatUsed;
                row["Fishing vessel"] = vc.Parent.VesselText;
                if (vc.Parent.NumberOfFishers != null)
                {
                    row["# of fishers"] = vc.Parent.NumberOfFishers;
                }
                row["Weight of catch of gear"] = vc.GearCatchWeight;
                if (vc.Parent.GearSettingFirst == null || vc.Parent.GearHaulingFirst == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = vc.Parent.GearSettingFirst;
                    row["Date of haul"] = vc.Parent.GearHaulingFirst;
                }
                int landed = vc.Parent.Parent.NumberOfCommercialLandings ?? 0 + vc.Parent.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed > 0)
                {
                    row["Fishing vessels landed"] = landed;
                }
                else
                {
                    row["Fishing vessels landed"] = DBNull.Value;
                }
                if (vc.Parent.Parent.NumberOfSampledLandingsEx == 0)
                {
                    row["Fishing vessels monitored"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels monitored"] = vc.Parent.Parent.NumberOfSampledLandingsEx;
                }
                row["Total weight of catch"] = vc.Parent.WeightOfCatch;
                row["Is the catch sold"] = vc.IsCatchSold;
                if (vc.PriceOfSpecies == null)
                {
                    row["Price"] = DBNull.Value;
                }
                else
                {
                    row["Price"] = vc.PriceOfSpecies;
                    row["Unit"] = vc.PriceUnit == "other" ? vc.OtherPriceUnit : vc.PriceUnit;
                }
                row["Notes"] = vc.Parent.Notes;
                row["Family"] = vc.Family;
                row["Species"] = vc.CatchName;
                row["Weight of species"] = ((double)vc.Catch_kg).ToString("N1");
                if (vc.Sample_kg == null)
                {
                    row["Sample weight of species (TWS)"] = DBNull.Value;
                }
                else
                {
                    row["Sample weight of species (TWS)"] = ((double)vc.Sample_kg).ToString("N1");
                }
                foreach (var gs in vc.ParentFishingGear.ListOfSpecsForCrossTab)
                {
                    string spec_name = "";
                    try
                    {
                        spec_name = gs.EffortSpecification.Name.Replace("/", " or ");
                        switch (gs.EffortSpecification.ValueType)
                        {
                            case ODKValueType.isBoolean:

                                //row[spec_name] = bool.Parse(gs.ef);
                                break;
                            case ODKValueType.isDecimal:
                                row[spec_name] = gs.EffortValue;
                                break;
                            case ODKValueType.isInteger:
                                row[spec_name] = (int)gs.EffortValue;
                                break;
                            case ODKValueType.isText:
                            case ODKValueType.isUndefined:
                                row[spec_name] = gs.EffortValueText;
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                EffortSpeciesDataTable.Rows.Add(row);
            }
        }

        private static void GenerateGearETPInteractionDataTable(AllSamplingEntitiesEventHandler EntitiesOfMonth)
        {
            var vus = VesselUnloadRepository.GetVesselUnloads(EntitiesOfMonth, true);
            GearETPInteractionDataTable = new DataTable();
            try
            {
                DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Month" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Province" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Municipality" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Region" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "FMA" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing ground" };
                GearETPInteractionDataTable.Columns.Add(dc);


                dc = new DataColumn { ColumnName = "Landing site" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Enumerator" };
                GearETPInteractionDataTable.Columns.Add(dc);


                dc = new DataColumn { ColumnName = "Sector" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Grid location" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Gear" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Weight of catch of gear" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "# species in catch of gear" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Ref #" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessel" };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
                GearETPInteractionDataTable.Columns.Add(dc);

                //dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
                //GearETPInteractionDataTable.Columns.Add(dc);

                //dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
                //GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Has Gear-ETP interaction", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Marine mammals", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Sea turtles", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Sharks", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Rays", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Escape from gear", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Release", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Injury and release", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "With mortality", DataType = typeof(bool) };
                GearETPInteractionDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Other interaction" };
                GearETPInteractionDataTable.Columns.Add(dc);

                foreach(var item in vus)
                {
                    var row = GearETPInteractionDataTable.NewRow();
                    var lss = item.Parent.Parent;
                    row["Data ID"] = item.PK;
                    row["Year"] = lss.SamplingDate.Year;
                    row["Month"] = lss.SamplingDate.Month;
                    row["Date"] = lss.SamplingDate;
                    row["Province"] = lss.LandingSite.Municipality.Province.ProvinceName;
                    row["Municipality"] = lss.LandingSite.Municipality.MunicipalityName;
                    row["Region"] = lss.NSAPRegion.Name;
                    row["FMA"] = lss.FMA.Name;
                    row["Fishing ground"] = lss.FishingGround.Name;
                    row["Landing site"] = lss.LandingSite.LandingSiteName;
                    row["Enumerator"] = lss.EnumeratorName;
                    row["Sector"] = item.Sector;
                    if (item.FirstFishingGround != " - ")
                    {
                        row["Grid location"] = item.FirstFishingGround;
                        row["Longitude"] = item.FirstFishingGroundCoordinate.Longitude;
                        row["Latitude"] = item.FirstFishingGroundCoordinate.Latitude;
                    }
                    row["Gear"] = item.GearUsed;
                    row["Weight of catch of gear"] = item.WeightOfCatch;
                    row["# species in catch of gear"] = item.NumberOfSpeciesInCatchComposition;
                    if (item.GearSettingFirst == null || item.GearHaulingFirst == null)
                    {
                        row["Date of set"] = DBNull.Value;
                        row["Date of haul"] = DBNull.Value;
                    }
                    else
                    {
                        row["Date of set"] = item.GearSettingFirst;
                        row["Date of haul"] = item.GearHaulingFirst;
                    }
                    row["Ref #"] = item.RefNo;
                    row["Is a fishing boat used"] = item.IsBoatUsed;
                    row["Fishing vessel"] = item.VesselName;
                    row["# of fishers"] = item.NumberOfFishers;
                    //row["Fishing vessels landed"] = item.FirstFishingGroundCoordinate.Latitude;
                    //row["Fishing vessels monitored"] = item.FirstFishingGroundCoordinate.Latitude;

                    row["Has Gear-ETP interaction"] = item.HasInteractionWithETPs;
                    row["Marine mammals"] = item.VesselUnload_ETP_Interaction.HasMarineMammal;
                    row["Sea turtles"] = item.VesselUnload_ETP_Interaction.HasSeaTurtle;
                    row["Sharks"] = item.VesselUnload_ETP_Interaction.HasShark;
                    row["Rays"] = item.VesselUnload_ETP_Interaction.HasRay;
                    row["Escape from gear"] = item.VesselUnload_ETP_Interaction.HasETPEscapeFromGear;
                    row["Release"] = item.VesselUnload_ETP_Interaction.HasETPReleaseFromGear;
                    row["Injury and release"] = item.VesselUnload_ETP_Interaction.HasETPInjuryAndReleaseFromGear;
                    row["With mortality"] = item.VesselUnload_ETP_Interaction.HasETPMortality;
                    row["Other interaction"] = item.VesselUnload_ETP_Interaction.ETPOtherInteraction;
                    GearETPInteractionDataTable.Rows.Add(row);

                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        private static void GenerateDailyLandingsDataTabe()
        {
            DailyLandingsDataTable = new DataTable();
            var props = typeof(LandingSiteSamplingForCrosstab).GetProperties();
            DataColumn dc;
            foreach (var p in props)
            {
                string typeName = p.PropertyType.FullName;
                string colName = LandingSiteSamplingForCrosstab.GetPropertyAlias(p.Name);
                if (typeName.Contains("System.Nullable"))
                {
                    dc = new DataColumn { ColumnName = colName, DataType = p.PropertyType.GenericTypeArguments[0] };
                }
                else
                {
                    dc = new DataColumn { ColumnName = colName, DataType = p.GetMethod.ReturnType };
                }
                DailyLandingsDataTable.Columns.Add(dc);
            }

            foreach (var item in CrossTabGenerator.LandingSiteSamplingForCrosstabs)
            {
                var row = DailyLandingsDataTable.NewRow();
                foreach (var prop in item.GetType().GetProperties())
                {
                    row[LandingSiteSamplingForCrosstab.GetPropertyAlias(prop.Name)] = prop.GetValue(item, null) == null ? DBNull.Value : prop.GetValue(item, null);
                }
                DailyLandingsDataTable.Rows.Add(row);
            }
        }
        private static void AnnounceDataSetCreated(bool start = false, bool end = false)
        {
            if (start)
            {
                CrossTabDatasetEvent?.Invoke(null, new CrossTabReportEventArg { Context = "Creating datasets", DataSetsToProcessCount = 7 });
            }
            else if (end)
            {
                CrossTabDatasetEvent?.Invoke(null, new CrossTabReportEventArg { Context = "Done creating datasets" });
            }
            else
            {
                CrossTabDatasetEvent?.Invoke(null, new CrossTabReportEventArg { Context = "Created datasets", DataSetsProcessedCount = ++_datatsetProcessedCount });
            }
        }
        public static bool GenerateDatasets(AllSamplingEntitiesEventHandler EntitiesOfMonth)
        {
            _datatsetProcessedCount = 0;
            AnnounceDataSetCreated(start: true);

            GenerateDailyLandingsDataTabe();
            AnnounceDataSetCreated();

            GenerateEffortDataTable();
            AnnounceDataSetCreated();

            GenerateEffortSpeciesDataTable();
            AnnounceDataSetCreated();

            GenerateSpeciesLenFreqDataTable();
            AnnounceDataSetCreated();

            GenerateSpeciesLengthDataTable();
            AnnounceDataSetCreated();

            GenerateSpeciesLenWeightDataTable();
            AnnounceDataSetCreated();

            GenerateSpeciesMaturityDataTable();
            AnnounceDataSetCreated();

            GenerateGearETPInteractionDataTable(EntitiesOfMonth);
            AnnounceDataSetCreated();

            AnnounceDataSetCreated(end: true);


            return true;
        }
        private static void GenerateSpeciesLengthDataTable()
        {
            SpeciesLengthsDataTable = new DataTable();
            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Municipality" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            SpeciesLengthsDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Landing site" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator" };
            SpeciesLengthsDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Sector" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of catch of gear" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "# species in catch of gear" };
            //SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Ref #" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Price", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of species", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sample weight of species (TWS)", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            SpeciesLengthsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sex" };
            SpeciesLengthsDataTable.Columns.Add(dc);

            foreach (var sl in CrossTabGenerator.CatchLengthCrossTabs
                .OrderBy(t => t.Parent.Parent.SamplingDate.Date)
                .ThenBy(t => t.Parent.Parent.GearUsed)
                .ThenBy(t => t.Parent.Parent.PK)
                .ThenBy(t => t.Parent.CatchName)
                )
            {
                var row = SpeciesLengthsDataTable.NewRow();
                var ls = CrossTabGenerator.EntitiesOfMonth.LandingSite;
                row["Data ID"] = sl.VesselUnload.PK;
                row["Year"] = sl.VesselUnload.SamplingDate.Year;
                row["Month"] = sl.VesselUnload.SamplingDate.ToString("MMMM");
                row["Date"] = sl.VesselUnload.SamplingDate;
                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;
                row["Region"] = CrossTabGenerator.EntitiesOfMonth.NSAPRegion;
                row["FMA"] = CrossTabGenerator.EntitiesOfMonth.FMA;
                row["Fishing ground"] = CrossTabGenerator.EntitiesOfMonth.FishingGround;
                row["Landing site"] = ls;
                row["Enumerator"] = sl.VesselUnload.EnumeratorName;
                row["Sector"] = sl.VesselUnload.Sector;

                if (sl.VesselUnload.FirstFishingGround != " - ")
                {
                    row["Grid location"] = sl.VesselUnload.FirstFishingGround;
                    row["Longitude"] = sl.VesselUnload.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = sl.VesselUnload.FirstFishingGroundCoordinate.Latitude;
                }
                row["Gear"] = sl.GearName;
                row["Weight of catch of gear"] = sl.WeightGearCatch;
                //row["# species in catch of gear"] = sl.;
                if (sl.VesselUnload.GearSettingFirst == null || sl.VesselUnload.GearHaulingFirst == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = sl.VesselUnload.GearSettingFirst;
                    row["Date of haul"] = sl.VesselUnload.GearHaulingFirst;
                }
                row["Ref #"] = sl.VesselUnload.RefNo;
                row["Is a fishing boat used"] = sl.VesselUnload.IsBoatUsed;
                row["Fishing vessel"] = sl.VesselUnload.VesselName;
                if (sl.VesselUnload.NumberOfFishers != null)
                {
                    row["# of fishers"] = sl.VesselUnload.NumberOfFishers;
                }
                int landed = sl.VesselUnload.Parent.NumberOfCommercialLandings ?? 0 + sl.VesselUnload.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed > 0)
                {
                    row["Fishing vessels landed"] = landed;
                }
                else
                {
                    row["Fishing vessels landed"] = DBNull.Value;
                }
                if (sl.VesselUnload.Parent.NumberOfSampledLandingsEx == 0)
                {
                    row["Fishing vessels monitored"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels monitored"] = sl.VesselUnload.Parent.NumberOfSampledLandingsEx;
                }
                row["Total weight of catch"] = sl.VesselUnload.WeightOfCatch;
                row["Is the catch sold"] = sl.Parent.IsCatchSold;
                if (sl.Parent.PriceOfSpecies == null)
                {
                    row["Price"] = DBNull.Value;
                }
                else
                {
                    row["Price"] = sl.Parent.PriceOfSpecies;
                    row["Unit"] = sl.Parent.PriceUnit == "other" ? sl.Parent.OtherPriceUnit : sl.Parent.PriceUnit;
                }
                row["Family"] = sl.Parent.Family;
                row["Species"] = sl.Parent.CatchName;
                row["Weight of species"] = sl.Parent.Catch_kg;
                if (sl.Parent.Sample_kg == null)
                {
                    row["Sample weight of species (TWS)"] = DBNull.Value;
                }
                else
                {
                    row["Sample weight of species (TWS)"] = ((double)sl.Parent.Sample_kg).ToString("N1");
                }
                row["Length"] = sl.Length;
                row["Sex"] = sl.Sex;

                SpeciesLengthsDataTable.Rows.Add(row);
            }


        }

        private static void GenerateSpeciesLenWeightDataTable()
        {
            SpeciesLengthWeightDataTable = new DataTable();
            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Municipality" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Landing site" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Sector" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of catch of gear" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "# species in catch of gear" };
            //SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Ref #" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Price", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of species", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sample weight of species (TWS)", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight", DataType = typeof(double) };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit of weight" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sex" };
            SpeciesLengthWeightDataTable.Columns.Add(dc);

            foreach (var slw in CrossTabGenerator.CatchLengthWeightCrossTabs
                .OrderBy(t => t.Parent.Parent.SamplingDate.Date)
                .ThenBy(t => t.Parent.Parent.GearUsed)
                .ThenBy(t => t.Parent.Parent.PK)
                .ThenBy(t => t.Parent.CatchName)
                )
            {
                var row = SpeciesLengthWeightDataTable.NewRow();
                var ls = CrossTabGenerator.EntitiesOfMonth.LandingSite;
                row["Data ID"] = slw.VesselUnload.PK;
                row["Year"] = slw.VesselUnload.SamplingDate.Year;
                row["Month"] = slw.VesselUnload.SamplingDate.ToString("MMMM");
                row["Date"] = slw.VesselUnload.SamplingDate;
                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;
                row["Region"] = CrossTabGenerator.EntitiesOfMonth.NSAPRegion;
                row["FMA"] = CrossTabGenerator.EntitiesOfMonth.FMA;
                row["Fishing ground"] = CrossTabGenerator.EntitiesOfMonth.FishingGround;
                row["Landing site"] = ls;
                row["Enumerator"] = slw.VesselUnload.EnumeratorName;
                row["Sector"] = slw.VesselUnload.Sector;
                if (slw.VesselUnload.FirstFishingGround != " - ")
                {
                    row["Grid location"] = slw.VesselUnload.FirstFishingGround;
                    row["Longitude"] = slw.VesselUnload.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = slw.VesselUnload.FirstFishingGroundCoordinate.Latitude;
                }
                row["Gear"] = slw.GearName;
                row["Weight of catch of gear"] = slw.WeightGearCatch;
                //row["# species in catch of gear"] = slw.;
                if (slw.VesselUnload.GearSettingFirst == null || slw.VesselUnload.GearHaulingFirst == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = slw.VesselUnload.GearSettingFirst;
                    row["Date of haul"] = slw.VesselUnload.GearHaulingFirst;
                }
                row["Ref #"] = slw.VesselUnload.RefNo;
                row["Is a fishing boat used"] = slw.VesselUnload.IsBoatUsed;
                row["Fishing vessel"] = slw.VesselUnload.VesselName;
                if (slw.VesselUnload.NumberOfFishers != null)
                {
                    row["# of fishers"] = slw.VesselUnload.NumberOfFishers;
                }
                int landed = slw.VesselUnload.Parent.NumberOfCommercialLandings ?? 0 + slw.VesselUnload.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed > 0)
                {
                    row["Fishing vessels landed"] = landed;
                }
                else
                {
                    row["Fishing vessels landed"] = DBNull.Value;
                }
                if (slw.VesselUnload.Parent.NumberOfSampledLandingsEx == 0)
                {
                    row["Fishing vessels monitored"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels monitored"] = slw.VesselUnload.Parent.NumberOfSampledLandingsEx;
                }
                row["Total weight of catch"] = slw.VesselUnload.WeightOfCatch;
                row["Is the catch sold"] = slw.Parent.IsCatchSold;
                if (slw.Parent.PriceOfSpecies == null)
                {
                    row["Price"] = DBNull.Value;
                }
                else
                {
                    row["Price"] = slw.Parent.PriceOfSpecies;
                    row["Unit"] = slw.Parent.PriceUnit == "other" ? slw.Parent.OtherPriceUnit : slw.Parent.PriceUnit;
                }
                row["Family"] = slw.Parent.Family;
                row["Species"] = slw.Parent.CatchName;
                row["Weight of species"] = slw.Parent.Catch_kg;
                if (slw.Parent.Sample_kg == null)
                {
                    row["Sample weight of species (TWS)"] = DBNull.Value;
                }
                else
                {
                    row["Sample weight of species (TWS)"] = ((double)slw.Parent.Sample_kg).ToString("N1");
                }
                row["Length"] = slw.Length;
                row["Weight"] = slw.Weight;
                row["Unit of weight"] = slw.Parent.WeighingUnit;
                row["Sex"] = slw.Sex;

                SpeciesLengthWeightDataTable.Rows.Add(row);
            }
        }
        private static void GenerateSpeciesLenFreqDataTable()
        {
            SpeciesLengthFreqDataTable = new DataTable();
            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Municipality" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Landing site" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Sector" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of catch of gear" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "# species in catch of gear" };
            //SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Ref #" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Price", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of species", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sample weight of species (TWS)", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Frequency", DataType = typeof(double) };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sex" };
            SpeciesLengthFreqDataTable.Columns.Add(dc);

            foreach (var slf in CrossTabGenerator.CatchLengthFreqCrossTabs
                .OrderBy(t => t.Parent.Parent.SamplingDate.Date)
                .ThenBy(t => t.Parent.Parent.GearUsed)
                .ThenBy(t => t.Parent.Parent.PK)
                .ThenBy(t => t.Parent.CatchName)
                )
            {
                var row = SpeciesLengthFreqDataTable.NewRow();
                var ls = CrossTabGenerator.EntitiesOfMonth.LandingSite;
                row["Data ID"] = slf.VesselUnload.PK;
                row["Year"] = slf.VesselUnload.SamplingDate.Year;
                row["Month"] = slf.VesselUnload.SamplingDate.ToString("MMMM");
                row["Date"] = slf.VesselUnload.SamplingDate;
                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;
                row["Region"] = CrossTabGenerator.EntitiesOfMonth.NSAPRegion;
                row["FMA"] = CrossTabGenerator.EntitiesOfMonth.FMA;
                row["Fishing ground"] = CrossTabGenerator.EntitiesOfMonth.FishingGround;
                row["Landing site"] = ls;
                row["Enumerator"] = slf.VesselUnload.EnumeratorName;
                row["Sector"] = slf.VesselUnload.Sector;
                if (slf.VesselUnload.FirstFishingGround != " - ")
                {
                    row["Grid location"] = slf.VesselUnload.FirstFishingGround;
                    row["Longitude"] = slf.VesselUnload.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = slf.VesselUnload.FirstFishingGroundCoordinate.Latitude;
                }
                row["Gear"] = slf.GearName;
                row["Weight of catch of gear"] = slf.WeightGearCatch;
                //row["# species in catch of gear"] = slf.;
                if (slf.VesselUnload.GearSettingFirst == null || slf.VesselUnload.GearHaulingFirst == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = slf.VesselUnload.GearSettingFirst;
                    row["Date of haul"] = slf.VesselUnload.GearHaulingFirst;
                }
                row["Ref #"] = slf.VesselUnload.RefNo;
                row["Is a fishing boat used"] = slf.VesselUnload.IsBoatUsed;
                row["Fishing vessel"] = slf.VesselUnload.VesselName;
                if (slf.VesselUnload.NumberOfFishers != null)
                {
                    row["# of fishers"] = slf.VesselUnload.NumberOfFishers;
                }
                int landed = slf.VesselUnload.Parent.NumberOfCommercialLandings ?? 0 + slf.VesselUnload.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed > 0)
                {
                    row["Fishing vessels landed"] = landed;
                }
                else
                {
                    row["Fishing vessels landed"] = DBNull.Value;
                }
                if (slf.VesselUnload.Parent.NumberOfSampledLandingsEx == 0)
                {
                    row["Fishing vessels monitored"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels monitored"] = slf.VesselUnload.Parent.NumberOfSampledLandingsEx;
                }
                row["Total weight of catch"] = slf.VesselUnload.WeightOfCatch;
                row["Is the catch sold"] = slf.Parent.IsCatchSold;
                if (slf.Parent.PriceOfSpecies == null)
                {
                    row["Price"] = DBNull.Value;
                }
                else
                {
                    row["Price"] = slf.Parent.PriceOfSpecies;
                    row["Unit"] = slf.Parent.PriceUnit == "other" ? slf.Parent.OtherPriceUnit : slf.Parent.PriceUnit;
                }
                row["Family"] = slf.Parent.Family;
                row["Species"] = slf.Parent.CatchName;
                row["Weight of species"] = slf.Parent.Catch_kg;
                if (slf.Parent.Sample_kg == null)
                {
                    row["Sample weight of species (TWS)"] = DBNull.Value;
                }
                else
                {
                    row["Sample weight of species (TWS)"] = ((double)slf.Parent.Sample_kg).ToString("N1");
                }
                row["Length"] = slf.Length;
                row["Frequency"] = slf.Frequency;
                row["Sex"] = slf.Sex;

                SpeciesLengthFreqDataTable.Rows.Add(row);
            }
        }
        private static void GenerateSpeciesMaturityDataTable()
        {
            SpeciesMaturityDataTable = new DataTable();
            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Municipality" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            SpeciesMaturityDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Landing site" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator" };
            SpeciesMaturityDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Sector" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of catch of gear" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "# species in catch of gear" };
            //SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Ref #" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Price", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of species", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sample weight of species (TWS)", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight", DataType = typeof(double) };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit of weight" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sex" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Maturity stage" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gonad weight" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gut content" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gut content category" };
            SpeciesMaturityDataTable.Columns.Add(dc);

            foreach (var scm in CrossTabGenerator.CatchMaturityCrossTabs
                .OrderBy(t => t.Parent.Parent.SamplingDate.Date)
                .ThenBy(t => t.Parent.Parent.GearUsed)
                .ThenBy(t => t.Parent.Parent.PK)
                .ThenBy(t => t.Parent.CatchName)
                )
            {
                var row = SpeciesMaturityDataTable.NewRow();
                var ls = CrossTabGenerator.EntitiesOfMonth.LandingSite;
                row["Data ID"] = scm.VesselUnload.PK;
                row["Year"] = scm.VesselUnload.SamplingDate.Year;
                row["Month"] = scm.VesselUnload.SamplingDate.ToString("MMMM");
                row["Date"] = scm.VesselUnload.SamplingDate;
                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;
                row["Region"] = CrossTabGenerator.EntitiesOfMonth.NSAPRegion;
                row["FMA"] = CrossTabGenerator.EntitiesOfMonth.FMA;
                row["Fishing ground"] = CrossTabGenerator.EntitiesOfMonth.FishingGround;
                row["Landing site"] = ls;
                row["Enumerator"] = scm.VesselUnload.EnumeratorName;
                row["Sector"] = scm.VesselUnload.Sector;
                if (scm.VesselUnload.FirstFishingGround != " - ")
                {
                    row["Grid location"] = scm.VesselUnload.FirstFishingGround;
                    row["Longitude"] = scm.VesselUnload.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = scm.VesselUnload.FirstFishingGroundCoordinate.Latitude;
                }
                row["Gear"] = scm.GearName;
                row["Weight of catch of gear"] = scm.WeightGearCatch;
                //row["# species in catch of gear"] = scm.;
                if (scm.VesselUnload.GearSettingFirst == null || scm.VesselUnload.GearHaulingFirst == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = scm.VesselUnload.GearSettingFirst;
                    row["Date of haul"] = scm.VesselUnload.GearHaulingFirst;
                }
                row["Ref #"] = scm.VesselUnload.RefNo;
                row["Is a fishing boat used"] = scm.VesselUnload.IsBoatUsed;
                row["Fishing vessel"] = scm.VesselUnload.VesselName;
                if (scm.VesselUnload.NumberOfFishers != null)
                {
                    row["# of fishers"] = scm.VesselUnload.NumberOfFishers;
                }
                int landed = scm.VesselUnload.Parent.NumberOfCommercialLandings ?? 0 + scm.VesselUnload.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed > 0)
                {
                    row["Fishing vessels landed"] = landed;
                }
                else
                {
                    row["Fishing vessels landed"] = DBNull.Value;
                }
                if (scm.VesselUnload.Parent.NumberOfSampledLandingsEx == 0)
                {
                    row["Fishing vessels monitored"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels monitored"] = scm.VesselUnload.Parent.NumberOfSampledLandingsEx;
                }
                row["Total weight of catch"] = scm.VesselUnload.WeightOfCatch;
                row["Is the catch sold"] = scm.Parent.IsCatchSold;
                if (scm.Parent.PriceOfSpecies == null)
                {
                    row["Price"] = DBNull.Value;
                }
                else
                {
                    row["Price"] = scm.Parent.PriceOfSpecies;
                    row["Unit"] = scm.Parent.PriceUnit == "other" ? scm.Parent.OtherPriceUnit : scm.Parent.PriceUnit;
                }
                row["Family"] = scm.Parent.Family;
                row["Species"] = scm.Parent.CatchName;
                row["Weight of species"] = scm.Parent.Catch_kg;
                if (scm.Parent.Sample_kg == null)
                {
                    row["Sample weight of species (TWS)"] = DBNull.Value;
                }
                else
                {
                    row["Sample weight of species (TWS)"] = ((double)scm.Parent.Sample_kg).ToString("N1");
                }
                if (scm.Length != null)
                {
                    row["Length"] = scm.Length;
                }
                if (scm.Weight != null)
                {
                    row["Weight"] = scm.Weight;
                }
                row["Unit of weight"] = scm.Parent.WeighingUnit;
                row["Sex"] = scm.Sex;
                row["Maturity stage"] = scm.Maturity;
                row["Gonad weight"] = scm.GonadWeight;
                row["Gut content"] = scm.GutContentWeight;
                row["Gut content category"] = scm.GutContentClassification;
                SpeciesMaturityDataTable.Rows.Add(row);
            }
        }
        private static void GenerateEffortDataTable()
        {
            EffortDataTable = new DataTable();
            try
            {
                DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
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

                dc = new DataColumn { ColumnName = "Grid location" };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Gear" };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Number of gears used", DataType = typeof(int) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Weight of catch of gear" };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Ref #" };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Has catch composition", DataType = typeof(bool) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessel" };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Catch composition count", DataType = typeof(int) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "ETP-Gear interaction", DataType = typeof(bool) };
                EffortDataTable.Columns.Add(dc);

                dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
                EffortDataTable.Columns.Add(dc);

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
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            foreach (var vu in CrossTabGenerator.VesselUnloads
                .OrderBy(t => t.SamplingDate.Date)
                .ThenBy(t => t.GearUsed)
                )
            {
                var row = EffortDataTable.NewRow();

                var ls = CrossTabGenerator.EntitiesOfMonth.LandingSite;
                row["Data ID"] = vu.PK;
                row["Fishing ground"] = CrossTabGenerator.EntitiesOfMonth.FishingGround;
                row["Year"] = vu.SamplingDate.Year;
                row["Month"] = vu.SamplingDate.Month;
                row["Date"] = vu.SamplingDate.ToString("dd-MMM-yyyy");
                row["Province"] = ls.Municipality.Province;
                row["Municipality"] = ls.Municipality;
                row["Landing site"] = ls;
                row["Enumerator"] = vu.NSAPEnumerator;
                row["Region"] = CrossTabGenerator.EntitiesOfMonth.NSAPRegion;
                row["FMA"] = CrossTabGenerator.EntitiesOfMonth.FMA;
                row["Sector"] = vu.Sector;
                if (vu.FirstFishingGround != " - ")
                {
                    row["Grid location"] = vu.FirstFishingGround;
                    row["Longitude"] = vu.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = vu.FirstFishingGroundCoordinate.Latitude;
                }

                row["Number of gears used"] = vu.ListUnloadFishingGearsEx.Count;
                if (vu.GearSettingFirst == null || vu.GearHaulingFirst == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = vu.GearSettingFirst;
                    row["Date of haul"] = vu.GearHaulingFirst;
                }
                row["Weight of catch of gear"] = vu.ListUnloadFishingGearsEx.Sum(t => t.WeightOfCatch);

                row["Ref #"] = vu.RefNo;
                row["Has catch composition"] = vu.HasCatchComposition;
                row["Is a fishing boat used"] = vu.IsBoatUsed;
                row["Fishing vessel"] = vu.VesselText;
                if (vu.NumberOfFishers == null)
                {
                    row["# of fishers"] = DBNull.Value;
                }
                else
                {
                    row["# of fishers"] = vu.NumberOfFishers;
                }

                int landed = vu.Parent.NumberOfCommercialLandings ?? 0 + vu.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed == 0)
                {
                    row["Fishing vessels landed"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels landed"] = landed;
                }

                if (vu.Parent.NumberOfSampledLandingsEx == 0)
                {
                    row["Fishing vessels monitored"] = DBNull.Value;
                }
                else
                {
                    row["Fishing vessels monitored"] = vu.Parent.NumberOfSampledLandingsEx;
                }

                row["Catch composition count"] = vu.CountCatchCompositionItems;

                if (vu.WeightOfCatch != null)
                {
                    row["Total weight of catch"] = vu.WeightOfCatch;
                }
                else
                {
                    row["Total weight of catch"] = DBNull.Value;
                }

                row["Is the catch sold"] = vu.IsCatchSold;
                row["ETP-Gear interaction"] = vu.HasInteractionWithETPs;
                row["Notes"] = vu.Notes;
                foreach (var vufg in vu.ListUnloadFishingGearsEx)
                {
                    row["Gear"] = vufg.Gear;
                    row["Weight of catch of gear"] = vufg.WeightOfCatch;
                    if (vufg.CountItemsInCatchComposition == null)
                    {
                        row["Catch composition count"] = vufg.ListOfCatchForCrossTab.Count;
                    }
                    else
                    {
                        row["Catch composition count"] = vufg.CountItemsInCatchComposition;
                    }
                    foreach (var gs in vufg.ListOfSpecsForCrossTab)
                    {
                        string spec_name = "";
                        try
                        {

                            spec_name = gs.EffortSpecification.Name.Replace("/", " or ");

                            switch (gs.EffortSpecification.ValueType)
                            {
                                case ODKValueType.isBoolean:
                                    //row[spec_name] = bool.Parse(gs.ef);
                                    break;
                                case ODKValueType.isDecimal:
                                    row[spec_name] = gs.EffortValue;
                                    break;
                                case ODKValueType.isInteger:
                                    row[spec_name] = (int)gs.EffortValue;
                                    break;
                                case ODKValueType.isText:
                                case ODKValueType.isUndefined:
                                    row[spec_name] = gs.EffortValueText;
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex);
                        }
                    }
                    try
                    {
                        EffortDataTable.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }


            }
        }

    }
}
