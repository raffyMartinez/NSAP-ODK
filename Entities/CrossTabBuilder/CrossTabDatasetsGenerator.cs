using NPOI.SS.Formula.Functions;
using NSAP_ODK.Entities.Database;
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
        public static DataTable EffortDataTable { get; private set; }
        public static DataTable DailyLandingsDataTable { get; private set; }
        public static DataTable EffortSpeciesDataTable { get; private set; }
        public static DataTable SpeciesLengthsDataTable { get; private set; }
        public static DataTable SpeciesLengthWeightDataTable { get; private set; }
        public static DataTable SpeciesLengthFreqDataTable { get; private set; }
        public static DataTable SpeciesMaturityDataTable { get; private set; }
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
                    Utilities.Logger.Log(ex);
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
            foreach (VesselCatch vc in CrossTabGenerator.VesselCatches.OrderBy(t => t.Parent.SamplingDate).ThenBy(t => t.PK))
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
                row["Grid location"] = vc.Parent.FirstFishingGround;
                if (vc.Parent.FirstFishingGroundCoordinate != null)
                {
                    row["Longitude"] = vc.Parent.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = vc.Parent.FirstFishingGroundCoordinate.Latitude;
                }
                else
                {
                    row["Longitude"] = DBNull.Value;
                    row["Latitude"] = DBNull.Value;
                }
                row["Gear"] = vc.ParentFishingGear.Gear;
                row["# species in catch of gear"] = vc.ParentFishingGear.CountItemsInCatchComposition;
                row["Is a fishing boat used"] = vc.Parent.IsBoatUsed;
                row["Fishing vessel"] = vc.Parent.VesselText;
                row["# of fishers"] = vc.Parent.NumberOfFishers;
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
                row["Fishing vessels landed"] = vc.Parent.Parent.NumberOfCommercialLandings ?? 0 + vc.Parent.Parent.NumberOfMunicipalLandings ?? 0;
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
        public static bool GenerateDatasets()
        {
            GenerateDailyLandingsDataTabe();
            GenerateEffortDataTable();
            GenerateEffortSpeciesDataTable();
            GenerateSpeciesLenFreqDataTable();
            GenerateSpeciesLengthDataTable();
            GenerateSpeciesLenWeightDataTable();
            GenerateSpeciesMaturityDataTable();
            return true;
        }
        private static void GenerateSpeciesLengthDataTable()
        {
            SpeciesLengthsDataTable = new DataTable();

        }

        private static void GenerateSpeciesLenWeightDataTable()
        {
            SpeciesLengthWeightDataTable = new DataTable();
        }
        private static void GenerateSpeciesLenFreqDataTable()
        {
            SpeciesLengthFreqDataTable = new DataTable();
        }
        private static void GenerateSpeciesMaturityDataTable()
        {
            SpeciesMaturityDataTable = new DataTable();
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

            foreach (var vu in CrossTabGenerator.VesselUnloads)
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
                if (!string.IsNullOrEmpty(vu.FirstFishingGround) && vu.FirstFishingGround.Length > 3)
                {
                    row["Grid location"] = vu.FirstFishingGround;
                    row["Longitude"] = vu.FirstFishingGroundCoordinate.Longitude;
                    row["Latitude"] = vu.FirstFishingGroundCoordinate.Latitude;
                }
                else
                {
                    row["Grid location"] = DBNull.Value;
                    row["Longitude"] = DBNull.Value;
                    row["Latitude"] = DBNull.Value;
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
                int landed=vu.Parent.NumberOfCommercialLandings ?? 0 + vu.Parent.NumberOfMunicipalLandings ?? 0;
                if (landed == 0)
                {
                    row["Fishing vessels landed"]=DBNull.Value;
                }
                else
                {
                    row["Fishing vessels landed"] = vu.Parent.NumberOfCommercialLandings ?? 0 + vu.Parent.NumberOfMunicipalLandings ?? 0;
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
                row["Total weight of catch"] = vu.WeightOfCatch;
                row["Is the catch sold"] = vu.IsCatchSold;
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
