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
        private static List<CrossTabEffort_VesselUnloadGear> _crossTabEfforts_vesselunloadGear;  // crosstab with species composition and will only
                                                                                                 // contain sampled landings with species composition
        private static List<CrossTabLengthWeight> _crossTabLenWts;
        private static List<CrossTabEffortAll_VesselUnloadGear> _crossTabEffortsAll_vesselUnloadGear; //crostab without species comp
                                                                                                      //and will contain all sampled landings
        private static List<CrossTabEffortAll> _crossTabEffortsAll;
        private static List<CrossTabLenFreq> _crossTabLenFreqs;
        private static List<CrossTabMaturity> _crossTabMaturities;
        private static List<CrossTabLength> _crossTabLengths;
        private static DataTable _effortSpeciesCrostabDataTable;
        private static DataTable _effortCrostabDataTable;
        private static List<GearUnload> _gearUnloads;
        private static List<LandingSiteSampling> _landingSiteSamplings;
        private static TreeViewModelControl.AllSamplingEntitiesEventHandler _sev;
        private static DataSet _crossTabDataSet;
        //public static Dictionary<int, CrossTabCommonProperties> UnloadCrossTabCommonPropertyDictionary { get; set; } = new Dictionary<int, CrossTabCommonProperties>();


        public static Dictionary<string, CrossTabCommonProperties> UnloadCrossTabCommonPropertyDictionary { get; set; } = new Dictionary<string, CrossTabCommonProperties>();

        public static event EventHandler<CrossTabReportEventArg> CrossTabEvent;

        public static Task<int> GearByMonthYearAsync(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            return Task.Run(() => GearByMonthYear(sev));
        }

        private static int GearByMonthYear(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            int counter = 0;
            _sev = sev;
            _crossTabEfforts_vesselunloadGear = new List<CrossTabEffort_VesselUnloadGear>();
            _crossTabEffortsAll_vesselUnloadGear = new List<CrossTabEffortAll_VesselUnloadGear>();
            _crossTabEfforts = new List<CrossTabEffort>();
            _crossTabLenFreqs = new List<CrossTabLenFreq>();
            _crossTabMaturities = new List<CrossTabMaturity>();
            _crossTabLengths = new List<CrossTabLength>();
            _crossTabEffortsAll = new List<CrossTabEffortAll>();
            _crossTabLenWts = new List<CrossTabLengthWeight>();

            string topic = _sev.ContextMenuTopic;

            _landingSiteSamplings = new List<LandingSiteSampling>();
            _gearUnloads = new List<GearUnload>();

            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { Context = "FilteringCatchData" });

            switch (topic)
            {
                case "contextMenuCrosstabLandingSite":
                    _landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                        .Where(t => t.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.FMA.FMAID == _sev.FMA.FMAID &&
                               t.FishingGround.Code == _sev.FishingGround.Code &&
                               t.LandingSiteName == _sev.LandingSiteText).ToList();

                    foreach (var ls in _landingSiteSamplings)
                    {
                        _gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.Where(r => r.ListVesselUnload.Count > 0).ToList());
                    }

                    break;
                case "contextMenuCrosstabMonth":
                    _landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                        .Where(t => t.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.FMA.FMAID == _sev.FMA.FMAID &&
                               t.FishingGround.Code == _sev.FishingGround.Code &&
                               t.LandingSiteID == _sev.LandingSite.LandingSiteID &&
                               t.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
                               t.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();

                    foreach (var ls in _landingSiteSamplings)
                    {
                        if (ls.GearUnloadViewModel == null)
                        {
                            ls.GearUnloadViewModel = new GearUnloadViewModel(ls);
                        }
                        _gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.Where(r => r.ListVesselUnload.Count > 0).ToList());
                    }
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
            //enumerate all the vessel unload for the month
            foreach (var unload in unloads)
            {
                VesselUnloadViewModel.SetUpFishingGearSubModel(unload);
                foreach (VesselUnload_FishingGear vufg in unload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                {
                    CrossTabCommon ctc = new CrossTabCommon(vufg);


                    //CrossTabCommon ctc = new CrossTabCommon(unload);
                    UnloadCrossTabCommonPropertyDictionary.Add(vufg.Guid.ToString(), ctc.CommonProperties);
                    //UnloadCrossTabCommonPropertyDictionary.Add(vufg.RowID, ctc.CommonProperties);

                    _crossTabEffortsAll_vesselUnloadGear.Add(new CrossTabEffortAll_VesselUnloadGear { CrossTabCommon = ctc, VesselUnload_FishingGear = vufg });

                    //List<VesselCatch> vesselCatch =  unload.ListVesselCatch;
                    List<VesselCatch> vesselCatch = unload.ListVesselCatch.Where(t => t.GearNameUsedEx == vufg.GearUsedName).ToList();

                    if (UnloadCrossTabCommonPropertyDictionary.Count == 1)
                    {
                        CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { Context = "RowsPrepared" });
                    }

                    foreach (var vc in vesselCatch)
                    {
                        ctc = new CrossTabCommon(vc, vufg);
                        //_crossTabEfforts.Add(new CrossTabEffort { CrossTabCommon = ctc, VesselUnload = unload });
                        _crossTabEfforts_vesselunloadGear.Add(new CrossTabEffort_VesselUnloadGear { CrossTabCommon = ctc, VesselUnload_FishingGear = vufg });

                        foreach (var clf in vc.ListCatchLenFreq)
                        {
                            //ctc = new CrossTabCommon(clf, vufg);
                            _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc, Length = clf.LengthClass, Freq = clf.Frequency, Sex = clf.Sex });
                        }

                        foreach (var cm in vc.ListCatchMaturity)
                        {
                            //ctc = new CrossTabCommon(cm, vufg);
                            _crossTabMaturities.Add(new CrossTabMaturity
                            {
                                CrossTabCommon = ctc,
                                Length = cm.Length,
                                Weight = cm.Weight,
                                //WeightUnit = ctc.WeightUnit,
                                Sex = cm.Sex,
                                MaturityStage = cm.Maturity,
                                GutContent = cm.GutContentClassification,
                                GonadWeight = cm.GonadWeight
                            });
                        }

                        foreach (var cl in vc.ListCatchLength)
                        {
                            //ctc = new CrossTabCommon(cl, vufg);
                            _crossTabLengths.Add(new CrossTabLength { CrossTabCommon = ctc, Length = cl.Length, Sex = cl.Sex });
                        }

                        foreach (var clw in vc.ListCatchLengthWeight)
                        {
                            //ctc = new CrossTabCommon(clw, vufg);
                            _crossTabLenWts.Add(new CrossTabLengthWeight
                            {
                                Length = clw.Length,
                                Weight = clw.Weight,
                                //WeightUnit = ctc.WeightUnit,
                                Sex = clw.Sex,
                                CrossTabCommon = ctc,
                            });
                        }

                    }
                    counter++;
                    CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { RowsToPrepare = unloads.Count, RowsPrepared = counter, Context = "AddingRows" });
                }
            }

            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { IsDone = true, Context = "PreparingDisplayRows" });
            BuildEffortCrossTabDataTable();
            BuildEffortSpeciesCrossTabDataTable();


            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { IsDone = true, Context = "DoneAddingRows" });
            return counter;
        }
        //private static int GearByMonthYear1(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        //{
        //    int counter = 0;
        //    _sev = sev;
        //    _crossTabEfforts = new List<CrossTabEffort>();
        //    _crossTabLenFreqs = new List<CrossTabLenFreq>();
        //    _crossTabMaturities = new List<CrossTabMaturity>();
        //    _crossTabLengths = new List<CrossTabLength>();
        //    _crossTabEffortsAll = new List<CrossTabEffortAll>();
        //    _crossTabLenWts = new List<CrossTabLengthWeight>();

        //    string topic = _sev.ContextMenuTopic;

        //    _landingSiteSamplings = new List<LandingSiteSampling>();
        //    _gearUnloads = new List<GearUnload>();

        //    CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { Context = "FilteringCatchData" });

        //    switch (topic)
        //    {
        //        case "contextMenuCrosstabLandingSite":
        //            _landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
        //                .Where(t => t.NSAPRegion.Code == _sev.NSAPRegion.Code &&
        //                       t.FMA.FMAID == _sev.FMA.FMAID &&
        //                       t.FishingGround.Code == _sev.FishingGround.Code &&
        //                       t.LandingSiteName == _sev.LandingSiteText).ToList();

        //            foreach (var ls in _landingSiteSamplings)
        //            {
        //                _gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.Where(r => r.ListVesselUnload.Count > 0).ToList());
        //            }

        //            break;
        //        case "contextMenuCrosstabMonth":
        //            _landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
        //                .Where(t => t.NSAPRegion.Code == _sev.NSAPRegion.Code &&
        //                       t.FMA.FMAID == _sev.FMA.FMAID &&
        //                       t.FishingGround.Code == _sev.FishingGround.Code &&
        //                       t.LandingSiteID == _sev.LandingSite.LandingSiteID &&
        //                       t.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
        //                       t.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();

        //            foreach (var ls in _landingSiteSamplings)
        //            {
        //                if (ls.GearUnloadViewModel == null)
        //                {
        //                    ls.GearUnloadViewModel = new GearUnloadViewModel(ls);
        //                }
        //                _gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.Where(r => r.ListVesselUnload.Count > 0).ToList());
        //            }
        //            break;
        //        case "contextMenuCrosstabGear":
        //            _gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
        //             .Where(t => t.ListVesselUnload.Count > 0 &&
        //                       t.Parent.NSAPRegion.Code == _sev.NSAPRegion.Code &&
        //                       t.Parent.FMA.FMAID == _sev.FMA.FMAID &&
        //                       t.Parent.FishingGround.Code == _sev.FishingGround.Code &&
        //                       t.Parent.LandingSiteName == _sev.LandingSiteText &&
        //                       t.GearUsedName == _sev.GearUsed &&
        //                       t.Parent.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
        //                       t.Parent.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1)).ToList();
        //            break;
        //    }



        //    List<VesselUnload> unloads = new List<VesselUnload>();

        //    foreach (var gu in _gearUnloads)
        //    {
        //        unloads.AddRange(gu.ListVesselUnload);
        //    }

        //    CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { RowsToPrepare = unloads.Count, Context = "Start" });

        //    UnloadCrossTabCommonPropertyDictionary.Clear();
        //    foreach (var unload in unloads)
        //    {
        //        VesselUnloadViewModel.SetUpFishingGearSubModel(unload);
        //        //foreach (VesselUnload_FishingGear vufg in unload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
        //        //{
        //        CrossTabCommon ctc = new CrossTabCommon(unload);


        //        //CrossTabCommon ctc = new CrossTabCommon(unload);
        //        UnloadCrossTabCommonPropertyDictionary.Add("", ctc.CommonProperties);
        //        //UnloadCrossTabCommonPropertyDictionary.Add(vufg.RowID, ctc.CommonProperties);

        //        _crossTabEffortsAll.Add(new CrossTabEffortAll { CrossTabCommon = ctc, VesselUnload = unload });

        //        List<VesselCatch> vesselCatch = unload.ListVesselCatch;

        //        if (UnloadCrossTabCommonPropertyDictionary.Count == 1)
        //        {
        //            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { Context = "RowsPrepared" });
        //        }

        //        foreach (var vc in vesselCatch)
        //        {
        //            ctc = new CrossTabCommon(vc);
        //            _crossTabEfforts.Add(new CrossTabEffort { CrossTabCommon = ctc, VesselUnload = unload });


        //            foreach (var clf in vc.ListCatchLenFreq)
        //            {
        //                ctc = new CrossTabCommon(clf);
        //                _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc, Length = clf.LengthClass, Freq = clf.Frequency, Sex = clf.Sex });
        //            }

        //            foreach (var cm in vc.ListCatchMaturity)
        //            {
        //                ctc = new CrossTabCommon(cm);
        //                _crossTabMaturities.Add(new CrossTabMaturity
        //                {
        //                    CrossTabCommon = ctc,
        //                    Length = cm.Length,
        //                    Weight = cm.Weight,
        //                    //WeightUnit = ctc.WeightUnit,
        //                    Sex = cm.Sex,
        //                    MaturityStage = cm.Maturity,
        //                    GutContent = cm.GutContentClassification,
        //                    GonadWeight = cm.GonadWeight
        //                });
        //            }

        //            foreach (var cl in vc.ListCatchLength)
        //            {
        //                ctc = new CrossTabCommon(cl);
        //                _crossTabLengths.Add(new CrossTabLength { CrossTabCommon = ctc, Length = cl.Length, Sex = cl.Sex });
        //            }

        //            foreach (var clw in vc.ListCatchLengthWeight)
        //            {
        //                ctc = new CrossTabCommon(clw);
        //                _crossTabLenWts.Add(new CrossTabLengthWeight
        //                {
        //                    Length = clw.Length,
        //                    Weight = clw.Weight,
        //                    //WeightUnit = ctc.WeightUnit,
        //                    Sex = clw.Sex,
        //                    CrossTabCommon = ctc,
        //                });
        //            }

        //        }
        //        counter++;
        //        CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { RowsToPrepare = unloads.Count, RowsPrepared = counter, Context = "AddingRows" });
        //        //}
        //    }

        //    CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { IsDone = true, Context = "PreparingDisplayRows" });
        //    BuildEffortCrossTabDataTable();
        //    BuildEffortSpeciesCrossTabDataTable();


        //    CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { IsDone = true, Context = "DoneAddingRows" });
        //    return counter;
        //}

        public static TreeViewModelControl.AllSamplingEntitiesEventHandler AllSamplingEntitiesEventHandler { get { return _sev; } }


        private static void BuildEffortCrossTabDataTable()
        {
            _effortCrostabDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
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

            dc = new DataColumn { ColumnName = "Ref #" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
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



            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
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

            //foreach (var item in _crossTabEffortsAll)
            foreach (var item in _crossTabEffortsAll_vesselUnloadGear)
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


                //row["Gears"] = ctcp.Gears;
                row["Gear"] = ctcp.Gear;
                row["Ref #"] = ctcp.RefNo;
                row["Is a fishing boat used"] = ctcp.IsBoatUsed;
                row["Fishing vessel"] = ctcp.FBName;

                if (ctcp.NumberOfFishers == null)
                {
                    row["# of fishers"] = DBNull.Value;
                }
                else
                {
                    row["# of fishers"] = ctcp.NumberOfFishers;
                }

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

                row["Is the catch sold"] = ctcp.IsCatchSold;
                row["Notes"] = ctcp.Notes;

                //foreach (var ve in ctcp.VesselUnload.ListVesselEffort)
                foreach (var ve in ctcp.VesselUnload_FishingGear.VesselUnload_Gear_Specs_ViewModel.VesselUnload_Gear_SpecCollection.ToList())
                {
                    try
                    {
                        if (ve.EffortSpecification != null)
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
                    }
                    catch (System.FormatException)
                    {
                        row[ve.EffortSpecification.Name] = DBNull.Value;
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Log(ex);
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

            dc = new DataColumn { ColumnName = "Ref #" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);



            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Price" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of species" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "TWS" };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight unit" };
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

            //foreach (var item in _crossTabEfforts)
            //foreach(var item in _crossTabEffortsAll_UnloadGear)
            foreach (var item in _crossTabEfforts_vesselunloadGear)
            {
                var row = _effortSpeciesCrostabDataTable.NewRow();
                //CrossTabCommonProperties ctcp = UnloadCrossTabCommonPropertyDictionary[item.VesselUnload.PK];
                CrossTabCommonProperties ctcp = UnloadCrossTabCommonPropertyDictionary[item.VesselUnload_FishingGear.Guid.ToString()];
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

                //row["Gear"] = ctcp.Gears;
                row["Gear"] = ctcp.Gear;

                row["Ref #"] = ctcp.RefNo;
                row["Is a fishing boat used"] = ctcp.IsBoatUsed;
                row["Fishing vessel"] = ctcp.FBName;

                if (ctcp.NumberOfFishers == null)
                {
                    row["# of fishers"] = DBNull.Value;
                }
                else
                {
                    row["# of fishers"] = ctcp.NumberOfFishers;
                }

                if (ctcp.FBL != null)
                {
                    row["Fishing vessels landed"] = ctcp.FBL;
                }

                if (ctcp.FBM != null)
                {
                    row["Fishing vessels monitored"] = ctcp.FBM;
                }

                row["Sampling day"] = ctcp.SamplingDay;
                row["Family"] = item.CrossTabCommon.Family;
                row["Species"] = item.CrossTabCommon.SN;
                row["Total weight of catch"] = ctcp.TotalWeight;
                row["Is the catch sold"] = ctcp.IsCatchSold;
                row["Price"] = item.CrossTabCommon.Price;
                row["Unit"] = item.CrossTabCommon.Unit;
                row["Notes"] = ctcp.Notes;
                row["Weight of species"] = item.CrossTabCommon.SpeciesWeight;
                row["TWS"] = item.CrossTabCommon.TWS;
                row["Weight unit"] = item.CrossTabCommon.WeightUnit;


                //foreach (var ve in ctcp.VesselUnload.ListVesselEffort)
                foreach (var ve in ctcp.VesselUnload_FishingGear.VesselUnload_Gear_Specs_ViewModel.VesselUnload_Gear_SpecCollection.ToList())
                {
                    try
                    {
                        if (ve.EffortSpecification != null)
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
                    }
                    catch (FormatException)
                    {
                        row[ve.EffortSpecification.Name] = DBNull.Value;
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Log(ex);
                    }
                }

                _effortSpeciesCrostabDataTable.Rows.Add(row);
            }


        }

        public static List<CrossTabLengthWeight> CrossTabLengthWeights { get { return _crossTabLenWts; } }
        public static List<CrossTabLength> CrossTabLengths { get { return _crossTabLengths; } }
        public static List<CrossTabMaturity> CrossTabMaturities { get { return _crossTabMaturities; } }

        public static List<CrossTabLenFreq> CrossTabLenFreqs { get { return _crossTabLenFreqs; } }

        public static DataTable CrossTabEfforts { get { return _effortSpeciesCrostabDataTable; } }

        public static DataTable CrossTabAllEfforts { get { return _effortCrostabDataTable; } }

        public static string ErrorMessage { get; private set; }
        public static DataSet CrossTabDataSet
        {
            get
            {
                ErrorMessage = "";
                _crossTabDataSet = new DataSet();
                try
                {
                    _effortSpeciesCrostabDataTable.TableName = "Effort";
                    _effortCrostabDataTable.TableName = "Effort (all)";

                    _crossTabDataSet.Tables.Add(_effortCrostabDataTable);
                    _crossTabDataSet.Tables.Add(_effortSpeciesCrostabDataTable);
                    _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLengths, "Length"));
                    _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLengthWeights, "Length-Weight"));
                    _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabMaturities, "Maturity"));
                    _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLenFreqs, "Len-Freq"));

                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    Utilities.Logger.Log(ex);
                }
                return _crossTabDataSet;
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
                                    if (ptd.Name != "VesselUnload" && ptd.Name != "VesselUnload_FishingGear")
                                    {
                                        table.Columns.Add(ptd.Name, Nullable.GetUnderlyingType(ptd.PropertyType) ?? ptd.PropertyType);
                                    }
                                    //if(ptd.Name != "VesselUnload_FishingGear")
                                    //{
                                    //    table.Columns.Add(ptd.Name, Nullable.GetUnderlyingType(ptd.PropertyType) ?? ptd.PropertyType);
                                    //}
                                }
                            }
                            else
                            {
                                table.Columns.Add(pt.Name, Nullable.GetUnderlyingType(pt.PropertyType) ?? pt.PropertyType);
                            }
                        }
                    }
                    else
                    {
                        if (prop.Name != "WeightUnit")
                        {
                            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        }
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
                                case "CrossTabLengthWeight":
                                    ctc = (item as CrossTabLengthWeight).CrossTabCommon;
                                    break;
                            }
                            foreach (PropertyDescriptor ptd in prop.GetChildProperties())
                            {
                                if (ptd.Name == "CommonProperties")
                                {
                                    foreach (PropertyDescriptor ptdd in ptd.GetChildProperties())
                                    {
                                        if (ptdd.Name != "VesselUnload" && ptdd.Name != "VesselUnload_FishingGear")
                                        {
                                            row[ptdd.Name] = ptdd.GetValue(ctc.CommonProperties) ?? DBNull.Value;
                                        }
                                    }
                                }
                                else
                                {
                                    switch (ptd.Name)
                                    {
                                        case "Family":
                                            row[ptd.Name] = ctc.Family;
                                            break;
                                        case "SN":
                                            row[ptd.Name] = ctc.SN;
                                            break;
                                        case "SpeciesWeight":
                                            row[ptd.Name] = ctc.SpeciesWeight;
                                            break;
                                        case "WeightUnit":
                                            row[ptd.Name] = ctc.WeightUnit;
                                            break;
                                            //case "TWS":
                                            //    if (ctc.TWS == null)
                                            //    {
                                            //        row[ptd.Name] = DBNull.Value;
                                            //    }
                                            //    else
                                            //    {
                                            //        row[ptd.Name] = (double)ctc.TWS;
                                            //    }
                                            //    break;
                                    }
                                }
                            }

                        }
                        else
                        {
                            switch (prop.ComponentType.Name)
                            {
                                case "CrossTabLength":
                                    if (prop.Name == "Length")
                                    {
                                        row[prop.Name] = (item as CrossTabLength).Length;
                                    }
                                    else if (prop.Name == "Sex")
                                    {
                                        row[prop.Name] = (item as CrossTabLength).Sex;
                                    }
                                    break;
                                case "CrossTabLengthWeight":
                                    switch (prop.Name)
                                    {
                                        case "Length":
                                            row[prop.Name] = (item as CrossTabLengthWeight).Length;
                                            break;
                                        case "Weight":
                                            row[prop.Name] = (item as CrossTabLengthWeight).Weight;
                                            break;
                                        //case "WeightUnit":
                                        //    row[prop.Name] = (item as CrossTabLengthWeight).WeightUnit;
                                        //    break;
                                        case "Sex":
                                            row[prop.Name] = (item as CrossTabLengthWeight).Sex;
                                            break;
                                    }

                                    break;
                                case "CrossTabLenFreq":
                                    switch (prop.Name)
                                    {
                                        case "Length":
                                            row[prop.Name] = (item as CrossTabLenFreq).Length;
                                            break;
                                        case "Frequency":

                                            row[prop.Name] = (item as CrossTabLenFreq).Freq;
                                            break;
                                        case "Sex":
                                            row[prop.Name] = (item as CrossTabLenFreq).Sex;
                                            break;
                                    }
                                    break;
                                case "CrossTabMaturity":
                                    switch (prop.Name)
                                    {
                                        case "Length":
                                            double? len = (item as CrossTabMaturity).Length;
                                            if (len == null)
                                            {
                                                row[prop.Name] = DBNull.Value;
                                            }
                                            else
                                            {
                                                row[prop.Name] = len;
                                            }
                                            break;
                                        case "Weight":
                                            double? wt = (item as CrossTabMaturity).Weight;
                                            if (wt == null)
                                            {
                                                row[prop.Name] = DBNull.Value;
                                            }
                                            else
                                            {
                                                row[prop.Name] = wt;
                                            }
                                            break;
                                        //case "WeightUnit":
                                        //    row[prop.Name] = (item as CrossTabLengthWeight).WeightUnit;
                                        //    break;
                                        case "Sex":
                                            row[prop.Name] = (item as CrossTabMaturity)?.Sex;
                                            break;
                                        case "MaturityStage":
                                            row[prop.Name] = (item as CrossTabMaturity)?.MaturityStage;
                                            break;
                                        case "GutContent":
                                            row[prop.Name] = (item as CrossTabMaturity)?.GutContent;
                                            break;
                                        case "GonadWeight":
                                            double? gwt = (item as CrossTabMaturity).GonadWeight;
                                            if (gwt == null)
                                            {
                                                row[prop.Name] = DBNull.Value;
                                            }
                                            else
                                            {
                                                row[prop.Name] = gwt;
                                            }
                                            break;
                                    }
                                    break;
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
