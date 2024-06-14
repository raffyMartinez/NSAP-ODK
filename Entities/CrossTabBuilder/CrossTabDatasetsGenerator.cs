using NSAP_ODK.Entities.Database;
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
        public static DataTable EffortSpeciesDataTable { get; private set; }
        public static DataTable SpeciesLengthsDataTable { get; private set; }
        public static DataTable SpeciesLengthWeightDataTable { get; private set; }
        public static DataTable SpeciesLengthFreqDataTable { get; private set; }
        public static DataTable SpeciesMaturityDataTable { get; private set; }
        public static bool GenerateDatasets()
        {
            return false;
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

    dc = new DataColumn { ColumnName = "Weight of catch of gear" };
    EffortDataTable.Columns.Add(dc);

    dc = new DataColumn { ColumnName = "# species in catch of gear" };
    EffortDataTable.Columns.Add(dc);

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

    foreach (var vu in CrossTabGenerator.VesselUnloads)
    {
        var row = EffortDataTable.NewRow();
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


        if (!string.IsNullOrEmpty(vu.FirstFishingGround))
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
        foreach (var vufg in vu.ListUnloadFishingGearsEx)
        {
             row["Gear"] = vufg.Gear;
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
                //catch (System.FormatException)
                //{
                //    if (gs.EffortValue.Length > 0 && gs.EffortSpecification.ValueType == ODKValueType.isInteger)
                //    {
                //        if (double.TryParse(gs.EffortValue, out double r))
                //        {
                //            row[spec_name] = (int)r;
                //        }
                //        else
                //        {
                //            row[spec_name] = DBNull.Value;
                //        }
                //    }
                //    else
                //    {
                //        row[spec_name] = DBNull.Value;
                //    }
                //}
                catch (Exception ex)
                {
                    Utilities.Logger.Log(ex);
                }
            }
            EffortDataTable.Rows.Add(row);
        }


    }
}

    }
}
