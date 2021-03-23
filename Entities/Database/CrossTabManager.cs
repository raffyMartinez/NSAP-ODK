using System;
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
        public static Dictionary<int, CrossTabCommonProperties> UnloadCrossTabCommonPropertyDictionary { get; set; } = new Dictionary<int, CrossTabCommonProperties>();

        public static event EventHandler<CrossTabReportEventArg> CrossTabEvent;

        public static Task<int> GearByMonthYearAsync(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            return Task.Run(() => GearByMonthYear(sev));
        }
        private static int GearByMonthYear(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            int counter = 0;
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
                     .Where(t => t.ListVesselUnload.Count > 0 &&
                                t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
                               t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
                               t.Parent.LandingSiteName == _sev.LandingSiteText).ToList();
                    break;
                case "contextMenuCrosstabMonth":
                    _gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                     .Where(t => t.ListVesselUnload.Count > 0 &&
                                t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
                               t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
                               t.Parent.LandingSiteName == _sev.LandingSiteText &&
                               t.Parent.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
                               t.Parent.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();
                    break;
                case "contextMenuCrosstabGear":
                    _gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                     .Where(t => t.ListVesselUnload.Count > 0 &&
                               t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
                               t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
                               t.Parent.LandingSiteName == _sev.LandingSiteText &&
                               t.GearUsedName == _sev.GearUsed &&
                               t.Parent.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
                               t.Parent.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();
                    break;
            }



            List<VesselUnload> unloads = new List<VesselUnload>();

            foreach (var gu in _gearUnloads)
            {
                unloads.AddRange(gu.ListVesselUnload);
            }

            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { RowsToPrepare = unloads.Count, Context = "Start" });

            UnloadCrossTabCommonPropertyDictionary.Clear();
            foreach (var unload in unloads)
            {

                CrossTabCommon ctc = new CrossTabCommon(unload);
                UnloadCrossTabCommonPropertyDictionary.Add(unload.PK, ctc.CommonProperties);
                _crossTabEffortsAll.Add(new CrossTabEffortAll { CrossTabCommon = ctc, VesselUnload = unload });


                List<VesselCatch> vesselCatch = unload.ListVesselCatch;
                foreach (var vc in vesselCatch)
                {
                    ctc = new CrossTabCommon(vc);
                    _crossTabEfforts.Add(new CrossTabEffort { CrossTabCommon = ctc, VesselUnload = unload });


                    foreach (var clf in vc.ListCatchLenFreq)
                    {
                        ctc = new CrossTabCommon(clf);
                        _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc, Length = clf.LengthClass, Freq = clf.Frequency });
                    }

                    foreach (var cm in vc.ListCatchMaturity)
                    {
                        ctc = new CrossTabCommon(cm);
                        _crossTabMaturities.Add(new CrossTabMaturity
                        {
                            CrossTabCommon = ctc,
                            Length = cm.Length,
                            Weight = cm.Weight,
                            Sex = cm.Sex,
                            MaturityStage = cm.Maturity,
                            GutContent = cm.GutContentClassification,
                            GonadWeight = cm.GonadWeight
                        });
                    }

                    foreach (var cl in vc.ListCatchLength)
                    {
                        ctc = new CrossTabCommon(cl);
                        _crossTabLengths.Add(new CrossTabLength { CrossTabCommon = ctc, Length = cl.Length });
                    }

                }
                counter++;
                CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { RowsToPrepare = unloads.Count, RowsPrepared = counter, Context = "AddingRows" });
            }

            BuildEffortCrossTabDataTable();
            BuildEffortSpeciesCrossTabDataTable();


            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { IsDone = true, Context = "DoneAddingRows" });
            return counter;
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

            dc = new DataColumn { ColumnName = "Enumerator" };
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
                CrossTabCommonProperties ctcp = item.CrossTabCommon.CommonProperties;
                row["Data ID"] = ctcp.DataID;
                row["Fishing ground"] = ctcp.FishingGround;
                row["Year"] = ctcp.Year;
                row["Month"] = ctcp.Month;
                row["Date"] = ctcp.SamplingDate.ToString("dd-MMM-yyyy");


                row["Province"] = ctcp.Province;
                row["Municipality"] = ctcp.Municipality;


                row["Landing site"] = ctcp.LandingSite;
                row["Enumerator"] = ctcp.Enumerator;

                row["Region"] = ctcp.Region;
                row["FMA"] = ctcp.FMA;


                row["Sector"] = ctcp.Sector;


                if (ctcp.FishingGroundGrid != null)
                {
                    row["Grid location"] = ctcp.FishingGroundGrid;
                    row["Longitude"] = ctcp.xCoordinate;
                    row["Latitude"] = ctcp.yCoordinate;
                }


                row["Gear"] = ctcp.Gear;
                row["Fishing vessel"] = ctcp.FBName;

                if (ctcp.FBL != null)
                {
                    row["Fishing vessels landded"] = ctcp.FBL;
                }


                row["Fishing vessels monitored"] = ctcp.FBM;


                row["Sampling day"] = ctcp.SamplingDay;
                row["Catch composition count"] = ctcp.VesselUnload.ListVesselCatch.Count;
                if (ctcp.TotalWeight == null)
                {
                    row["Total weight of catch"] = DBNull.Value;
                }
                else
                {
                    row["Total weight of catch"] = ctcp.TotalWeight;
                }

                foreach (var ve in ctcp.VesselUnload.ListVesselEffort)
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

            dc = new DataColumn { ColumnName = "Enumerator" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Grid location" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
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
                CrossTabCommonProperties ctcp = UnloadCrossTabCommonPropertyDictionary[item.VesselUnload.PK];
                row["Data ID"] = ctcp.DataID;
                row["Fishing ground"] = ctcp.FishingGround;
                row["Year"] = ctcp.Year;
                row["Month"] = ctcp.Month;
                row["Date"] = ctcp.SamplingDate.ToString("dd-MMM-yyyy");


                row["Province"] = ctcp.Province;
                row["Municipality"] = ctcp.Municipality;


                row["Landing site"] = ctcp.LandingSite;
                row["Enumerator"] = ctcp.Enumerator;

                row["Region"] = ctcp.Region;
                row["FMA"] = ctcp.FMA;
                row["Sector"] = ctcp.Sector;

                if (ctcp.FishingGroundGrid != null)
                {
                    row["Grid location"] = ctcp.FishingGroundGrid;
                    row["Longitude"] = ctcp.xCoordinate;
                    row["Latitude"] = ctcp.yCoordinate;
                }

                row["Gear"] = ctcp.Gear;
                row["Fishing vessel"] = ctcp.FBName;

                if (ctcp.FBL != null)
                {
                    row["Fishing vessels landded"] = ctcp.FBL;
                }

                if (ctcp.FBM != null)
                {
                    row["Fishing vessels monitored"] = ctcp.FBM;
                }

                row["Sampling day"] = ctcp.SamplingDay;
                row["Family"] = item.CrossTabCommon.Family;
                row["Species"] = item.CrossTabCommon.SN;
                row["Total weight of catch"] = ctcp.TotalWeight;
                row["Weight of species"] = item.CrossTabCommon.SpeciesWeight;

                foreach (var ve in ctcp.VesselUnload.ListVesselEffort)
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
                            if (pt.Name == "CommonProperties")
                            {
                                foreach (PropertyDescriptor ptd in pt.GetChildProperties())
                                {
                                    if (ptd.Name != "VesselUnload")
                                    {
                                        table.Columns.Add(ptd.Name, Nullable.GetUnderlyingType(ptd.PropertyType) ?? ptd.PropertyType);
                                    }
                                }
                            }
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
                            foreach (PropertyDescriptor ptd in prop.GetChildProperties())
                            {
                                if (ptd.Name == "CommonProperties")
                                {
                                    foreach (PropertyDescriptor ptdd in ptd.GetChildProperties())
                                    {
                                        if (ptdd.Name != "VesselUnload")
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


                                            row[ptdd.Name] = ptdd.GetValue(ctc.CommonProperties) ?? DBNull.Value;

                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            try
                            {
                                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                            }
                            catch 
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
