﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;

namespace NSAP_ODK.Entities.Database
{
    public static class CrossTabManager
    {
        private static List<CrossTabEffort> _crossTabEfforts;
        private static List<CrossTabEffortAll> _crossTabEffortsAll;
        private static List<CrossTabLenFreq> _crossTabLenFreqs;
        private static List<CrossTabMaturity> _crossTabMaturities;
        private static List<CrossTabLength> _crossTabLengths;
        private static DataTable _effortSpeciesCrostabDataTable;
        private static DataTable _effortCrostabDataTable;
        private static List<GearUnload> _gearUnloads;
        private static TreeViewModelControl.AllSamplingEntitiesEventHandler _sev;
        public static void GearByMonthYear(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            _sev = sev;
            _crossTabEfforts = new List<CrossTabEffort>();
            _crossTabLenFreqs = new List<CrossTabLenFreq>();
            _crossTabMaturities = new List<CrossTabMaturity>();
            _crossTabLengths = new List<CrossTabLength>();
            _crossTabEffortsAll = new List<CrossTabEffortAll>();

            string topic = _sev.ContextMenuTopic;



            _gearUnloads = new List<GearUnload>();

            switch (topic)
            {
                case "contextMenuCrosstabLandingSite":
                    _gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                     .Where(t => t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
                               t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
                               t.Parent.LandingSiteName == _sev.LandingSiteText).ToList();
                    break;
                case "contextMenuCrosstabMonth":
                    _gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                     .Where(t => t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
                               t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
                               t.Parent.LandingSiteName == _sev.LandingSiteText &&
                               t.Parent.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
                               t.Parent.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();
                    break;
                case "contextMenuCrosstabGear":
                    _gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                     .Where(t => t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
                               t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
                               t.Parent.LandingSiteName == _sev.LandingSiteText &&
                               t.GearUsedName == _sev.GearUsed &&
                               t.Parent.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
                               t.Parent.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();
                    break;
            }

            foreach (var gu in _gearUnloads)
            {
                foreach (var item in NSAPEntities.VesselCatchViewModel.VesselCatchCollection
                    .Where(t => t.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabEfforts.Add(new CrossTabEffort { CrossTabCommon = ctc });
                }

                foreach (var item in NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                    .Where(t => t.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabEffortsAll.Add(new CrossTabEffortAll { CrossTabCommon = ctc, VesselUnload = item });
                }


                foreach (var item in NSAPEntities.CatchLenFreqViewModel.CatchLenFreqCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc, Length = item.LengthClass, Freq = item.Frequency });
                }

                foreach (var item in NSAPEntities.CatchMaturityViewModel.CatchMaturityCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabMaturities.Add(new CrossTabMaturity
                    {
                        CrossTabCommon = ctc,
                        Length = item.Length,
                        Weight = item.Weight,
                        Sex = item.Sex,
                        MaturityStage = item.Maturity,
                        GutContent = item.GutContentClassification
                    });
                }

                foreach (var item in NSAPEntities.CatchLengthViewModel.CatchLengthCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabLengths.Add(new CrossTabLength { CrossTabCommon = ctc, Length = item.Length });
                }

            }

            BuildEffortSpeciesCrossTabDataTable();
            BuildEffortCrossTabDataTable();
        }

        public static TreeViewModelControl.AllSamplingEntitiesEventHandler AllSamplingEntitiesEventHandler { get { return _sev; } }

        private static void BuildEffortCrossTabDataTable()
        {
            _effortCrostabDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Data ID" };
            _effortCrostabDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            _effortCrostabDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Municipality" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landded", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling day", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catch composition count", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            _effortCrostabDataTable.Columns.Add(dc);

            foreach (var spec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection.OrderBy(t => t.Name))
            {
                dc = new DataColumn { ColumnName = spec.Name };
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
                _effortCrostabDataTable.Columns.Add(dc);
            }

            foreach (var item in _crossTabEffortsAll)
            {
                var row = _effortCrostabDataTable.NewRow();
                row["Data ID"] = item.CrossTabCommon.DataID;
                row["Fishing ground"] = item.CrossTabCommon.FishingGround;
                row["Year"] = item.CrossTabCommon.MonthSampled.Year;
                row["Month"] = item.CrossTabCommon.MonthSampled.ToString("MMM");
                row["Date"] = item.CrossTabCommon.SamplingDate.ToString("dd-MMM-yyyy");

                if (item.CrossTabCommon.LandingSite != null)
                {
                    row["Province"] = item.CrossTabCommon.Province;
                    row["Municipality"] = item.CrossTabCommon.Municipality;
                }

                row["Landing site"] = item.CrossTabCommon.LandingSite;
                row["Region"] = item.CrossTabCommon.Region;
                row["FMA"] = item.CrossTabCommon.FMA;


                row["Sector"] = item.CrossTabCommon.Sector;

                if (item.CrossTabCommon.FishingGroundGrid != null)
                {
                    row["Grid location"] = $"{item.CrossTabCommon.FishingGroundGrid.ToString()}";
                    row["Longitude"] = item.CrossTabCommon.xCoordinate;
                    row["Latitude"] = item.CrossTabCommon.yCoordinate;
                }

                row["Gear"] = item.CrossTabCommon.Gear;
                row["Fishing vessel"] = item.CrossTabCommon.FBName;

                if (item.CrossTabCommon.FBL != null)
                {
                    row["Fishing vessels landded"] = item.CrossTabCommon.FBL;
                }

                if (item.CrossTabCommon.FBM != null)
                {
                    row["Fishing vessels monitored"] = item.CrossTabCommon.FBM;
                }

                row["Sampling day"] = item.CrossTabCommon.SamplingDay;
                row["Catch composition count"] = item.VesselUnload.ListVesselCatch.Count;
                if (item.CrossTabCommon.TotalWeight == null)
                {
                    row["Total weight of catch"] = DBNull.Value;
                }
                else
                {
                    row["Total weight of catch"] = item.CrossTabCommon?.TotalWeight;
                }

                foreach (var ve in NSAPEntities.VesselEffortViewModel.VesselEffortCollection
                    .Where(t => t.Parent.PK == item.CrossTabCommon.DataID)
                    .OrderBy(t => t.EffortSpecification.Name)
                    .ToList())
                {
                    switch (ve.EffortSpecification.ValueType)
                    {
                        case ODKValueType.isBoolean:
                            row[ve.EffortSpecification.Name] = bool.Parse(ve.EffortValue);
                            break;
                        case ODKValueType.isDecimal:
                            row[ve.EffortSpecification.Name] = double.Parse(ve.EffortValue);
                            break;
                        case ODKValueType.isInteger:
                            row[ve.EffortSpecification.Name] = int.Parse(ve.EffortValue);
                            break;
                        case ODKValueType.isText:
                        case ODKValueType.isUndefined:
                            row[ve.EffortSpecification.Name] = ve.EffortValue;
                            break;
                    }
                }

                _effortCrostabDataTable.Rows.Add(row);
            }


        }
        private static void BuildEffortSpeciesCrossTabDataTable()
        {
            _effortSpeciesCrostabDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Data ID" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Year", DataType = typeof(int) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Month" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date", DataType = typeof(DateTime) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling day", DataType = typeof(bool) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Municipality" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude" ,DataType = typeof(double)};
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landded", DataType = typeof(int) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);



            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of species" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);


            //dc = new DataColumn { ColumnName = "Family", DataType = typeof(double) };
            //_effortCrostabDataTable.Columns.Add(dc);

            //dc = new DataColumn { ColumnName = "Catch weight", DataType = typeof(double) };
            //_effortCrostabDataTable.Columns.Add(dc);

            foreach (var spec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection.OrderBy(t => t.Name))
            {
                dc = new DataColumn { ColumnName = spec.Name };
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
                _effortSpeciesCrostabDataTable.Columns.Add(dc);
            }

            foreach (var item in _crossTabEfforts)
            {
                var row = _effortSpeciesCrostabDataTable.NewRow();
                row["Data ID"] = item.CrossTabCommon.DataID;
                row["Fishing ground"] = item.CrossTabCommon.FishingGround;
                row["Year"] = item.CrossTabCommon.MonthSampled.Year;
                row["Month"] = item.CrossTabCommon.MonthSampled.ToString("MMM");
                row["Date"] = item.CrossTabCommon.SamplingDate.ToString("dd-MMM-yyyy");

                if (item.CrossTabCommon.LandingSite != null)
                {
                    row["Province"] = item.CrossTabCommon.Province;
                    row["Municipality"] = item.CrossTabCommon.Municipality;
                }

                row["Landing site"] = item.CrossTabCommon.LandingSite;
                
                row["Region"] = item.CrossTabCommon.Region;
                row["FMA"] = item.CrossTabCommon.FMA;
                row["Sector"] = item.CrossTabCommon.Sector;

                if (item.CrossTabCommon.FishingGroundGrid != null)
                {
                    row["Grid location"] = $"{item.CrossTabCommon.FishingGroundGrid.ToString()}";
                    row["Longitude"] = item.CrossTabCommon.xCoordinate;
                    row["Latitude"] = item.CrossTabCommon.yCoordinate;
                }

                row["Gear"] = item.CrossTabCommon.Gear;
                row["Fishing vessel"] = item.CrossTabCommon.FBName;

                if (item.CrossTabCommon.FBL != null)
                {
                    row["Fishing vessels landded"] = item.CrossTabCommon.FBL;
                }

                if (item.CrossTabCommon.FBM != null)
                {
                    row["Fishing vessels monitored"] = item.CrossTabCommon.FBM;
                }

                row["Sampling day"] = item.CrossTabCommon.SamplingDay;
                row["Family"] = item.CrossTabCommon.Family;
                row["Species"] = item.CrossTabCommon.SN;
                row["Total weight of catch"] = item.CrossTabCommon.TotalWeight;
                row["Weight of species"] = item.CrossTabCommon.SpeciesWeight;

                foreach (var ve in NSAPEntities.VesselEffortViewModel.VesselEffortCollection
                    .Where(t => t.Parent.PK == item.CrossTabCommon.DataID)
                    .OrderBy(t => t.EffortSpecification.Name)
                    .ToList())
                {
                    switch (ve.EffortSpecification.ValueType)
                    {
                        case ODKValueType.isBoolean:
                            row[ve.EffortSpecification.Name] = bool.Parse(ve.EffortValue);
                            break;
                        case ODKValueType.isDecimal:
                            row[ve.EffortSpecification.Name] = double.Parse(ve.EffortValue);
                            break;
                        case ODKValueType.isInteger:
                            row[ve.EffortSpecification.Name] = int.Parse(ve.EffortValue);
                            break;
                        case ODKValueType.isText:
                        case ODKValueType.isUndefined:
                            row[ve.EffortSpecification.Name] = ve.EffortValue;
                            break;
                    }
                }

                _effortSpeciesCrostabDataTable.Rows.Add(row);
            }


        }
        public static List<CrossTabLength> CrossTabLengths { get { return _crossTabLengths; } }
        public static List<CrossTabMaturity> CrossTabMaturities { get { return _crossTabMaturities; } }

        public static List<CrossTabLenFreq> CrossTabLenFreqs { get { return _crossTabLenFreqs; } }

        public static DataTable CrossTabEfforts { get { return _effortSpeciesCrostabDataTable; } }

        public static DataTable CrossTabAllEfforts { get { return _effortCrostabDataTable; } }

        public static DataSet CrossTabDataSet
        {
            get
            {
                DataSet ds = new DataSet();
                _effortSpeciesCrostabDataTable.TableName = "Effort";
                _effortCrostabDataTable.TableName = "Effort (all)";
                ds.Tables.Add(_effortCrostabDataTable);
                ds.Tables.Add(_effortSpeciesCrostabDataTable);
                ds.Tables.Add(ListToDataTable(CrossTabLengths, "Length"));
                ds.Tables.Add(ListToDataTable(CrossTabMaturities, "Maturity"));
                ds.Tables.Add(ListToDataTable(CrossTabLenFreqs, "Len-Freq"));
                return ds;
            }
        }

        public static DataTable ListToDataTable<T>(IList<T> data, string tableName)
        {
            DataTable table = new DataTable();
            table.TableName = tableName;

            //special handling for value types and string
            if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
            {

                DataColumn dc = new DataColumn("Value", typeof(T));
                table.Columns.Add(dc);
                foreach (T item in data)
                {
                    DataRow dr = table.NewRow();
                    dr[0] = item;
                    table.Rows.Add(dr);
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.Name == "CrossTabCommon")
                    {
                        foreach (PropertyDescriptor pt in prop.GetChildProperties())
                        {
                            table.Columns.Add(pt.Name, Nullable.GetUnderlyingType(pt.PropertyType) ?? pt.PropertyType);
                        }
                    }
                    else
                    {
                        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                }
                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        if (prop.Name == "CrossTabCommon")
                        {
                            CrossTabCommon ctc = null;
                            switch (prop.ComponentType.Name)
                            {
                                case "CrossTabLength":
                                    ctc = (item as CrossTabLength).CrossTabCommon;
                                    break;
                                case "CrossTabLenFreq":
                                    ctc = (item as CrossTabLenFreq).CrossTabCommon;
                                    break;
                                case "CrossTabMaturity":
                                    ctc = (item as CrossTabMaturity).CrossTabCommon;
                                    break;
                            }

                            foreach (PropertyDescriptor pt in prop.GetChildProperties())
                            {
                                row[pt.Name] = pt.GetValue(ctc) ?? DBNull.Value;
                            }

                        }
                        else
                        {
                            try
                            {
                                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                            }
                            catch (Exception ex)
                            {
                                row[prop.Name] = DBNull.Value;
                            }
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }


    }
}
