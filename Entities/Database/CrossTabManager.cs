using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Web.UI;

namespace NSAP_ODK.Entities.Database
{
    public static class CrossTabManager
    {
        private static List<CrossTabDailyGearLanding> _crosstab_DailyGearLandings;
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
        private static List<CrossTabCarrierLandingCommon> _crossTabCommonCarrierLandings;
        private static List<CrossTabCarrierLandingVesselCatchCommon> _crossTabCommonCarrierLandingVesselCatches;


        private static DataTable _dailyCarrierLandingsDataTable;
        private static DataTable _dailyCarrierCatchCompositionTable;
        private static DataTable _dailyCarrierLenFreqTable;
        private static DataTable _dailyCarrierLengthTable;
        private static DataTable _dailyCarrierMaturityTable;
        private static DataTable _dailyCarrierLengthWeightsDataTable;

        private static DataTable _dailyLandingDataTable;
        private static DataTable _effortSpeciesCrostabDataTable;
        private static DataTable _effortCrostabDataTable;
        private static List<GearUnload> _gearUnloads;
        private static List<LandingSiteSampling> _landingSiteSamplings;
        private static TreeViewModelControl.AllSamplingEntitiesEventHandler _sev;
        private static DataSet _crossTabDataSet;


        public static Dictionary<string, CrossTabCommonProperties> UnloadCrossTabCommonPropertyDictionary { get; set; } = new Dictionary<string, CrossTabCommonProperties>();

        public static event EventHandler<CrossTabReportEventArg> CrossTabEvent;

        public static LandingSite LandingSite { get; set; }

        public static DateTime MonthYear { get; set; }
        public static Task<int> CarrierBoatLandingsByMonthAsync(LandingSite ls, DateTime monthYear)
        {
            return Task.Run(() => CarrierBoatLandingsByMonth(ls, monthYear));
        }
        public static int CarrierBoatLandingsByMonth(LandingSite ls, DateTime monthYear)
        {
            LandingSite = ls;
            MonthYear = monthYear;
            _crossTabLenWts = new List<CrossTabLengthWeight>();
            _crossTabLengths = new List<CrossTabLength>();
            _crossTabLenFreqs = new List<CrossTabLenFreq>();
            _crossTabMaturities = new List<CrossTabMaturity>();
            _crossTabCommonCarrierLandings = new List<CrossTabCarrierLandingCommon>();
            _crossTabCommonCarrierLandingVesselCatches = new List<CrossTabCarrierLandingVesselCatchCommon>();

            var cbls = NSAPEntities.LandingSiteSamplingViewModel.CarrierBoatLandings(ls, monthYear);
            foreach (CarrierLanding cl in cbls)
            {
                CrossTabCarrierLandingCommon ctclc = new CrossTabCarrierLandingCommon(cl);
                _crossTabCommonCarrierLandings.Add(ctclc);
                if (cl.VesselCatchViewModel == null)
                {
                    cl.VesselCatchViewModel = new VesselCatchViewModel(cl);

                }
                foreach (VesselCatch vc in cl.VesselCatchViewModel.VesselCatchCollection)
                {
                    CrossTabCarrierLandingVesselCatchCommon vcc = new CrossTabCarrierLandingVesselCatchCommon(vc, ctclc);
                    _crossTabCommonCarrierLandingVesselCatches.Add(vcc);

                    if (vc.CatchLengthWeightViewModel == null)
                    {
                        vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(vc);
                    }
                    foreach (CatchLengthWeight clw in vc.CatchLengthWeightViewModel.CatchLengthWeightCollection)
                    {
                        var crossTabLenWt = new CrossTabLengthWeight
                        {
                            CrossTabCarrierLandingVesselCatchCommon = vcc,
                            Length = clw.Length,
                            Weight = clw.Weight,
                            WeightUnit = vc.WeighingUnit
                        };
                        _crossTabLenWts.Add(crossTabLenWt);
                    }

                    if (vc.CatchLengthViewModel == null)
                    {
                        vc.CatchLengthViewModel = new CatchLengthViewModel(vc);
                    }
                    foreach (CatchLength clen in vc.CatchLengthViewModel.CatchLengthCollection)
                    {
                        var crosstabLen = new CrossTabLength
                        {
                            CrossTabCarrierLandingVesselCatchCommon = vcc,
                            Length = clen.Length
                        };
                        _crossTabLengths.Add(crosstabLen);
                    }

                    if (vc.CatchLenFreqViewModel == null)
                    {
                        vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(vc);
                    }
                    foreach (CatchLenFreq clf in vc.CatchLenFreqViewModel.CatchLenFreqCollection)
                    {
                        CrossTabLenFreq ctlf = new CrossTabLenFreq
                        {
                            CrossTabCarrierLandingVesselCatchCommon = vcc,
                            Length = clf.LengthClass,
                            Freq = clf.Frequency
                        };
                        _crossTabLenFreqs.Add(ctlf);
                    }

                    if (vc.CatchMaturityViewModel == null)
                    {
                        vc.CatchMaturityViewModel = new CatchMaturityViewModel(vc);
                    }
                    foreach (CatchMaturity cm in vc.CatchMaturityViewModel.CatchMaturityCollection)
                    {
                        CrossTabMaturity ctm = new CrossTabMaturity
                        {
                            CrossTabCarrierLandingVesselCatchCommon = vcc,
                            Length = cm.Length,
                            Weight = cm.Weight,
                            Sex = cm.Sex,
                            MaturityStage = cm.Maturity,
                            GutContent = cm.GutContentClassification,
                            GonadWeight = cm.GonadWeight,
                            WeightUnit = vc.WeighingUnit,
                            GutContentWeight = cm.WeightGutContent
                        };
                        _crossTabMaturities.Add(ctm);
                    }

                }
            }
            BuildDailyCarrierLandingsDataTable();
            BuildDailyCarrierCatchCompositionDataTable();
            BuildDailyCarrierLenFreqDataTable();
            BuildDailyCarrierLengthDataTable();
            BuildDailyCarrierLengthWeightDataTable();
            BuildDailyCarrierMaturityDataTable();
            return 0;
        }


        public static Task<int> GearByMonthYearAsync(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            return Task.Run(() => GearByMonthYear(sev));
        }

        private static int GearByMonthYear(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            int counter = 0;
            _sev = sev;
            _crosstab_DailyGearLandings = new List<CrossTabDailyGearLanding>();
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
                    _landingSiteSamplings = NSAPEntities.FishingCalendarDayExViewModel.LandingSiteSamplings;
                    //_landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                    //    .Where(t => t.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                    //           t.FMA.FMAID == _sev.FMA.FMAID &&
                    //           t.FishingGround.Code == _sev.FishingGround.Code &&
                    //           t.LandingSiteName == _sev.LandingSiteText).ToList();

                    foreach (var ls in _landingSiteSamplings)
                    {
                        if (ls.HasFishingOperation)
                        {
                            if (ls.GearsInLandingSite == null)
                            {
                                ls.GearsInLandingSite = NSAPEntities.LandingSiteSamplingViewModel.GetGearsInLandingSiteSampling(ls);
                            }
                            _gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.Where(r => r.ListVesselUnload.Count > 0).ToList());
                        }

                    }

                    break;
                case "contextMenuCrosstabMonth":
                    _landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                        .Where(t => t.LandingSiteTypeOfSampling == "rs" &&
                                t.NSAPRegion.Code == _sev.NSAPRegion.Code &&
                               t.FMA.FMAID == _sev.FMA.FMAID &&
                               t.FishingGround.Code == _sev.FishingGround.Code &&
                               t.LandingSiteID == _sev.LandingSite.LandingSiteID &&
                               t.SamplingDate.Date >= (DateTime)_sev.MonthSampled &&
                               t.SamplingDate.Date < ((DateTime)_sev.MonthSampled).AddMonths(1))
                               .OrderBy(t => t.SamplingDate)
                               .ToList();
                    //_landingSiteSamplings = NSAPEntities.FishingCalendarDayExViewModel.LandingSiteSamplings;

                    foreach (var ls in _landingSiteSamplings)
                    {
                        if (ls.HasFishingOperation && ls.GearsInLandingSite == null)
                        {
                            ls.GearsInLandingSite = NSAPEntities.LandingSiteSamplingViewModel.GetGearsInLandingSiteSampling(ls);
                        }
                        if (ls.GearUnloadViewModel == null)
                        {

                            ls.GearUnloadViewModel = new GearUnloadViewModel(ls);
                        }
                        if (ls.GearUnloadViewModel.GearUnloadCollection.Count == 0)
                        {
                            GearUnload gu = new GearUnload();
                            gu.Parent = ls;
                            _gearUnloads.Add(gu);
                        }
                        else
                        {
                            _gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection);
                        }
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
            int counter_1 = 0;
            foreach (LandingSiteSampling lss in _landingSiteSamplings)
            {
                CrossTabDailyGearLanding ctdgl = null;
                if (lss.GearsInLandingSite.Count == 0)
                {
                    //ctdgl = new CrossTabDailyGearLanding(lss);
                    //ctdgl.Sequence = counter_1++;
                    //_crosstab_DailyGearLandings.Add(ctdgl);
                    if (lss.GearUnloadViewModel.Count > 0)
                    {
                        foreach (GearUnload gu in lss.GearUnloadViewModel.GearUnloadCollection)
                        {
                            ctdgl = new CrossTabDailyGearLanding(gu);
                            ctdgl.Sequence = counter_1++;
                            _crosstab_DailyGearLandings.Add(ctdgl);
                        }
                    }
                    else
                    {
                        ctdgl = new CrossTabDailyGearLanding(lss);
                        ctdgl.Sequence = counter_1++;
                        _crosstab_DailyGearLandings.Add(ctdgl);
                    }
                }
                else
                {
                    foreach (GearInLandingSite gls in lss.GearsInLandingSite
                        .OrderBy(t => t.GearUsedName)
                        .ThenBy(t => t.SectorCode)
                        )
                    {
                        ctdgl = new CrossTabDailyGearLanding(gls);
                        ctdgl.Sequence = counter_1++;

                        _crosstab_DailyGearLandings.Add(ctdgl);
                    }
                }

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
                if (unload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection == null)
                {
                    unload.VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(unload);
                }
                foreach (VesselUnload_FishingGear vufg in unload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                {
                    CrossTabCommon ctc = new CrossTabCommon(vufg);


                    UnloadCrossTabCommonPropertyDictionary.Add(vufg.Guid.ToString(), ctc.CommonProperties);
                    _crossTabEffortsAll_vesselUnloadGear.Add(new CrossTabEffortAll_VesselUnloadGear { CrossTabCommon = ctc, VesselUnload_FishingGear = vufg });

                    List<VesselCatch> vesselCatch = new List<VesselCatch>();
                    if (unload.HasCatchComposition)
                    {
                        vesselCatch = unload.ListVesselCatch.Where(t => t.GearNameUsedEx == vufg.GearUsedName).ToList();
                    }

                    if (UnloadCrossTabCommonPropertyDictionary.Count == 1)
                    {
                        CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { Context = "RowsPrepared" });
                    }

                    foreach (var vc in vesselCatch)
                    {
                        ctc = new CrossTabCommon(vc, vufg);
                        _crossTabEfforts_vesselunloadGear.Add(new CrossTabEffort_VesselUnloadGear { CrossTabCommon = ctc, VesselUnload_FishingGear = vufg });

                        foreach (var clf in vc.ListCatchLenFreq)
                        {
                            _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc, Length = clf.LengthClass, Freq = clf.Frequency, Sex = clf.Sex });
                        }

                        foreach (var cm in vc.ListCatchMaturity)
                        {
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
                            _crossTabLengths.Add(new CrossTabLength { CrossTabCommon = ctc, Length = cl.Length, Sex = cl.Sex });
                        }

                        foreach (var clw in vc.ListCatchLengthWeight)
                        {
                            _crossTabLenWts.Add(new CrossTabLengthWeight
                            {
                                Length = clw.Length,
                                Weight = clw.Weight,
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
            BuildDailyLandingCrossTabDataTable();
            BuildEffortCrossTabDataTable();
            BuildEffortSpeciesCrossTabDataTable();


            CrossTabEvent?.Invoke(null, new CrossTabReportEventArg { IsDone = true, Context = "DoneAddingRows" });
            return counter;
        }


        public static TreeViewModelControl.AllSamplingEntitiesEventHandler AllSamplingEntitiesEventHandler { get { return _sev; } }



        private static void BuildDailyCarrierLengthWeightDataTable()
        {
            _dailyCarrierLengthWeightsDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Carrier name", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing grounds", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catcher boats", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of landed catch", DataType = typeof(double) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Count species in catch", DataType = typeof(int) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species name", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species weight", DataType = typeof(double) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight", DataType = typeof(double) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
            _dailyCarrierLengthWeightsDataTable.Columns.Add(dc);

            int seq = 1;
            foreach (var item in _crossTabLenWts)
            {
                var row = _dailyCarrierLengthWeightsDataTable.NewRow();
                row["Sequence"] = seq;
                row["Sampling date"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.SamplingDate.ToString("MMM-dd-yyyy");
                row["Landing site"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.LandingSite.ToString();
                row["Carrier name"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CarrierLanding.CarrierName;
                row["Fishing grounds"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.FishingGroundNames;
                row["Catcher boats"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CatcherBoatNames;
                if (item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch == null)
                {
                    row["Weight of landed catch"] = DBNull.Value;
                }
                else
                {
                    row["Weight of landed catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch;
                }

                row["Count species in catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.NumberOfSpeciesInCatch;
                row["Species name"] = item.CrossTabCarrierLandingVesselCatchCommon.CatchName;
                if (item.CrossTabCarrierLandingVesselCatchCommon.Weight == null)
                {
                    row["Species weight"] = DBNull.Value;
                }
                else
                {
                    row["Species weight"] = item.CrossTabCarrierLandingVesselCatchCommon.Weight;
                }
                row["Length"] = item.Length;
                row["Weight"] = item.Weight;
                row["Unit"] = item.CrossTabCarrierLandingVesselCatchCommon.WeightUnit;
                _dailyCarrierLengthWeightsDataTable.Rows.Add(row);
                seq++;
            }

        }
        private static void BuildDailyCarrierMaturityDataTable()
        {
            _dailyCarrierMaturityTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Carrier name", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing grounds", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catcher boats", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of landed catch", DataType = typeof(double) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Count species in catch", DataType = typeof(int) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species name", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species weight", DataType = typeof(double) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight", DataType = typeof(double) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Unit", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sex", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Maturity stage", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gonad weight", DataType = typeof(double) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gut content category", DataType = typeof(string) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gut content weight", DataType = typeof(double) };
            _dailyCarrierMaturityTable.Columns.Add(dc);

            int seq = 1;
            foreach (var item in _crossTabMaturities)
            {
                var row = _dailyCarrierMaturityTable.NewRow();
                row["Sequence"] = seq;
                row["Sampling date"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.SamplingDate.ToString("MMM-dd-yyyy");
                row["Landing site"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.LandingSite.ToString();
                row["Carrier name"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CarrierLanding.CarrierName;
                row["Fishing grounds"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.FishingGroundNames;
                row["Catcher boats"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CatcherBoatNames;
                if (item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch == null)
                {
                    row["Weight of landed catch"] = DBNull.Value;
                }
                else
                {
                    row["Weight of landed catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch;
                }

                row["Count species in catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.NumberOfSpeciesInCatch;
                row["Species name"] = item.CrossTabCarrierLandingVesselCatchCommon.CatchName;
                if (item.CrossTabCarrierLandingVesselCatchCommon.Weight == null)
                {
                    row["Species weight"] = DBNull.Value;
                }
                else
                {
                    row["Species weight"] = item.CrossTabCarrierLandingVesselCatchCommon.Weight;
                }

                if (item.Length == null)
                {
                    row["Length"] = DBNull.Value;
                }
                else
                {
                    row["Length"] = item.Length;
                }

                if (item.Weight == null)
                {
                    row["Weight"] = DBNull.Value;
                }
                else
                {
                    row["Weight"] = item.Weight;
                }

                row["Unit"] = item.WeightUnit;

                row["Sex"] = item.Sex;
                row["Maturity stage"] = item.MaturityStage;

                if (item.GonadWeight == null)
                {
                    row["Gonad weight"] = DBNull.Value;
                }
                else
                {
                    row["Gonad weight"] = item.GonadWeight;
                }

                row["Gut content category"] = item.GutContent;
                if (item.GutContentWeight == null)
                {
                    row["Gut content weight"] = DBNull.Value;
                }
                else
                {
                    row["Gut content weight"] = item.GutContentWeight;
                }
                _dailyCarrierMaturityTable.Rows.Add(row);
                seq++;
            }
        }

        private static void BuildDailyCarrierLenFreqDataTable()
        {
            _dailyCarrierLenFreqTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Carrier name", DataType = typeof(string) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing grounds", DataType = typeof(string) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catcher boats", DataType = typeof(string) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of landed catch", DataType = typeof(double) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Count species in catch", DataType = typeof(int) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species name", DataType = typeof(string) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species weight", DataType = typeof(double) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Frequency", DataType = typeof(double) };
            _dailyCarrierLenFreqTable.Columns.Add(dc);

            int seq = 1;
            foreach (var item in _crossTabLenFreqs)
            {
                var row = _dailyCarrierLenFreqTable.NewRow();
                row["Sequence"] = seq;
                row["Sampling date"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.SamplingDate.ToString("MMM-dd-yyyy");
                row["Landing site"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.LandingSite.ToString();
                row["Carrier name"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CarrierLanding.CarrierName;
                row["Fishing grounds"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.FishingGroundNames;
                row["Catcher boats"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CatcherBoatNames;
                if (item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch == null)
                {
                    row["Weight of landed catch"] = DBNull.Value;
                }
                else
                {
                    row["Weight of landed catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch;
                }

                row["Count species in catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.NumberOfSpeciesInCatch;
                row["Species name"] = item.CrossTabCarrierLandingVesselCatchCommon.CatchName;
                row["Length"] = item.Length;
                row["Frequency"] = item.Freq;
                _dailyCarrierLenFreqTable.Rows.Add(row);
                seq++;
            }
        }
        private static void BuildDailyCarrierLengthDataTable()
        {
            _dailyCarrierLengthTable = new DataTable();
            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Carrier name", DataType = typeof(string) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing grounds", DataType = typeof(string) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catcher boats", DataType = typeof(string) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of landed catch", DataType = typeof(double) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Count species in catch", DataType = typeof(int) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species name", DataType = typeof(string) };
            _dailyCarrierLengthTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length", DataType = typeof(double) };
            _dailyCarrierLengthTable.Columns.Add(dc);


            int seq = 1;
            foreach (var item in _crossTabLengths)
            {
                var row = _dailyCarrierLengthTable.NewRow();
                row["Sequence"] = seq;
                row["Sampling date"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.SamplingDate.ToString("MMM-dd-yyyy");
                row["Landing site"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.LandingSite.ToString();
                row["Carrier name"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CarrierLanding.CarrierName;
                row["Fishing grounds"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.FishingGroundNames;
                row["Catcher boats"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.CatcherBoatNames;
                if (item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch == null)
                {
                    row["Weight of landed catch"] = DBNull.Value;
                }
                else
                {
                    row["Weight of landed catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.WeightOfCatch;
                }

                row["Count species in catch"] = item.CrossTabCarrierLandingVesselCatchCommon.CrossTabCarrierLandingCommon.NumberOfSpeciesInCatch;
                row["Species name"] = item.CrossTabCarrierLandingVesselCatchCommon.CatchName;
                row["Length"] = item.Length;
                _dailyCarrierLengthTable.Rows.Add(row);
                seq++;
            }
        }
        private static void BuildDailyCarrierCatchCompositionDataTable()
        {
            _dailyCarrierCatchCompositionTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Carrier name", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing grounds", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catcher boats", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of landed catch", DataType = typeof(double) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Count species in catch", DataType = typeof(int) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Taxa", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species name", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Family", DataType = typeof(string) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species weight", DataType = typeof(double) };
            _dailyCarrierCatchCompositionTable.Columns.Add(dc);

            int seq = 1;
            foreach (var item in _crossTabCommonCarrierLandingVesselCatches)
            {
                var row = _dailyCarrierCatchCompositionTable.NewRow();
                row["Sequence"] = seq;
                row["Sampling date"] = item.CrossTabCarrierLandingCommon.SamplingDate.ToString("MMM-dd-yyyy");
                row["Landing site"] = item.CrossTabCarrierLandingCommon.LandingSite.ToString();
                row["Carrier name"] = item.CrossTabCarrierLandingCommon.CarrierLanding.CarrierName;
                row["Fishing grounds"] = item.CrossTabCarrierLandingCommon.FishingGroundNames;
                row["Catcher boats"] = item.CrossTabCarrierLandingCommon.CatcherBoatNames;
                if (item.CrossTabCarrierLandingCommon.WeightOfCatch == null)
                {
                    row["Weight of landed catch"] = DBNull.Value;
                }
                else
                {
                    row["Weight of landed catch"] = item.CrossTabCarrierLandingCommon.WeightOfCatch;
                }

                row["Count species in catch"] = item.CrossTabCarrierLandingCommon.NumberOfSpeciesInCatch;
                row["Taxa"] = item.Taxa;
                row["Species name"] = item.CatchName;
                row["Family"] = item.Family;
                if (item.Weight == null)
                {
                    row["Species weight"] = DBNull.Value;
                }
                else
                {
                    row["Species weight"] = item.Weight;
                }

                seq++;

                _dailyCarrierCatchCompositionTable.Rows.Add(row);
            }

        }
        private static void BuildDailyCarrierLandingsDataTable()
        {
            _dailyCarrierLandingsDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Carrier name", DataType = typeof(string) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing grounds", DataType = typeof(string) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Catcher boats", DataType = typeof(string) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of landed catch", DataType = typeof(double) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Count species in catch", DataType = typeof(int) };
            _dailyCarrierLandingsDataTable.Columns.Add(dc);

            int seq = 1;
            foreach (var item in _crossTabCommonCarrierLandings)
            {
                var row = _dailyCarrierLandingsDataTable.NewRow();
                row["Sequence"] = seq;
                row["Sampling date"] = item.SamplingDate.ToString("MMM-dd-yyyy");
                row["Landing site"] = item.LandingSite.ToString();
                row["Carrier name"] = item.CarrierLanding.CarrierName;
                row["Fishing grounds"] = item.FishingGroundNames;
                row["Catcher boats"] = item.CatcherBoatNames;
                if (item.WeightOfCatch == null)
                {
                    row["Weight of landed catch"] = DBNull.Value;
                }
                else
                {
                    row["Weight of landed catch"] = item.WeightOfCatch;
                }

                row["Count species in catch"] = item.NumberOfSpeciesInCatch;
                seq++;

                _dailyCarrierLandingsDataTable.Rows.Add(row);
            }

        }

        private static void BuildEffortCrossTabDataTable()
        {
            _effortCrostabDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Data ID", DataType = typeof(string) };
            _effortCrostabDataTable.Columns.Add(dc);


            dc = new DataColumn { ColumnName = "Successful fishing operation", DataType = typeof(bool) };
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

            dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Weight of catch of gear" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# species in catch of gear" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Ref #" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Has catch composition", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is a fishing boat used", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessel" };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of fishers", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels landed", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing vessels monitored", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);



            dc = new DataColumn { ColumnName = "Catch composition count", DataType = typeof(int) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch", DataType = typeof(double) };
            _effortCrostabDataTable.Columns.Add(dc);



            dc = new DataColumn { ColumnName = "Is the catch sold", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Notes", DataType = typeof(string) };
            _effortCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Include effort indicators", DataType = typeof(bool) };
            _effortCrostabDataTable.Columns.Add(dc);

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
                _effortCrostabDataTable.Columns.Add(dc);
            }

            foreach (var item in _crossTabEffortsAll_vesselUnloadGear)
            {

                var row = _effortCrostabDataTable.NewRow();
                if (item.CrossTabCommon.VesselUnload.GearSoakViewModel == null)
                {
                    item.CrossTabCommon.VesselUnload.GearSoakViewModel = new GearSoakViewModel(item.CrossTabCommon.VesselUnload);
                }
                CrossTabCommonProperties ctcp = item.CrossTabCommon.CommonProperties;
                row["Data ID"] = ctcp.DataID;
                row["Successful fishing operation"] = ctcp.OperationSuccessful;
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


                row["Sector"] = ctcp.UnloadSector;


                if (ctcp.FishingGroundGrid != null)
                {
                    row["Grid location"] = ctcp.FishingGroundGrid;
                    row["Longitude"] = ctcp.xCoordinate;
                    row["Latitude"] = ctcp.yCoordinate;
                }


                row["Gear"] = ctcp.Gear;

                if (ctcp.DateTimeGearSet == null || ctcp.DateTimeGearHaul == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = ctcp.DateTimeGearSet;
                    row["Date of haul"] = ctcp.DateTimeGearHaul;
                }

                row["Weight of catch of gear"] = ctcp.GearCatchWeight;
                row["# species in catch of gear"] = ctcp.GearCatchSpeciesCount;
                row["Ref #"] = ctcp.RefNo;
                row["Has catch composition"] = ctcp.HasCatchComposition;
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


                row["Fishing vessels monitored"] = ctcp.FBM;



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
                row["Include effort indicators"] = ctcp.IncludeEffortIndicators;

                foreach (var ve in ctcp.VesselUnload_FishingGear.VesselUnload_Gear_Specs_ViewModel.VesselUnload_Gear_SpecCollection.ToList())
                {
                    string spec_name = "";
                    try
                    {
                        if (ve.EffortSpecification != null)
                        {
                            spec_name = ve.EffortSpecification.Name.Replace("/", " or ");

                            switch (ve.EffortSpecification.ValueType)
                            {
                                case ODKValueType.isBoolean:

                                    row[spec_name] = bool.Parse(ve.EffortValue);
                                    break;
                                case ODKValueType.isDecimal:

                                    row[spec_name] = double.Parse(ve.EffortValue);
                                    break;
                                case ODKValueType.isInteger:

                                    row[spec_name] = int.Parse(ve.EffortValue);
                                    break;
                                case ODKValueType.isText:
                                case ODKValueType.isUndefined:

                                    row[spec_name] = ve.EffortValue;
                                    break;
                            }
                        }
                    }
                    catch (System.FormatException)
                    {
                        if (ve.EffortValue.Length > 0 && ve.EffortSpecification.ValueType == ODKValueType.isInteger)
                        {
                            if (double.TryParse(ve.EffortValue, out double r))
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

                _effortCrostabDataTable.Rows.Add(row);
            }


        }
        private static void BuildDailyLandingCrossTabDataTable()
        {
            _dailyLandingDataTable = new DataTable();

            DataColumn dc = new DataColumn { ColumnName = "Sequence", DataType = typeof(int) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sampling date", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Region", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "FMA", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Fishing ground", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Enumerator", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Has landings", DataType = typeof(bool) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Note", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of landings", DataType = typeof(int) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Is sampling day", DataType = typeof(bool) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of landings monitored", DataType = typeof(int) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "# of gear types", DataType = typeof(int) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Gear", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Sector", DataType = typeof(string) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Number of landings of gear", DataType = typeof(int) };
            _dailyLandingDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Total weight of catch of gear", DataType = typeof(int) };
            _dailyLandingDataTable.Columns.Add(dc);

            foreach (var item in _crosstab_DailyGearLandings
                                   .Where(t => t.LandingSiteSampling.IsMultiVessel)
                                   .OrderBy(t => t.Sequence))
            {
                var row = _dailyLandingDataTable.NewRow();
                row["Sequence"] = item.Sequence + 1;
                row["Sampling date"] = item.LandingSiteSampling.SamplingDate.ToString("MMM dd, yyyy");
                row["Region"] = item.LandingSiteSampling.NSAPRegion.ShortName;
                row["FMA"] = item.LandingSiteSampling.FMA;
                row["Fishing ground"] = item.LandingSiteSampling.FishingGround;
                row["Landing site"] = item.LandingSiteSampling.LandingSiteName;
                row["Enumerator"] = item.LandingSiteSampling.EnumeratorName;
                row["Has landings"] = item.LandingSiteSampling.HasFishingOperation;
                row["Note"] = item.LandingSiteSampling.Remarks;
                row["Is sampling day"] = item.LandingSiteSampling.IsSamplingDay;
                if (item.LandingSiteSampling.HasFishingOperation)
                {
                    if (item.LandingSiteSampling.NumberOfLandings == null)
                    {
                        row["# of landings"] = DBNull.Value;
                    }
                    else
                    {
                        row["# of landings"] = item.LandingSiteSampling.NumberOfLandings;
                    }

                    if (item.LandingSiteSampling.IsSamplingDay)
                    {
                        if (item.LandingSiteSampling.NumberOfLandingsSampled == null)
                        {
                            row["# of landings monitored"] = DBNull.Value;
                        }
                        else
                        {
                            row["# of landings monitored"] = item.LandingSiteSampling.NumberOfLandingsSampled;
                        }
                    }
                    else
                    {
                        row["# of landings monitored"] = DBNull.Value;
                    }
                    if (item.LandingSiteSampling.NumberOfGearTypesInLandingSite == null)
                    {
                        row["# of gear types"] = DBNull.Value;
                    }
                    else
                    {
                        row["# of gear types"] = item.LandingSiteSampling.NumberOfGearTypesInLandingSite;
                    }
                }
                else
                {
                    row["# of landings"] = DBNull.Value;
                    row["# of landings monitored"] = DBNull.Value; ;
                    row["# of gear types"] = DBNull.Value;
                }

                if (item.GearInLandingSite != null)
                {
                    row["Gear"] = item.GearInLandingSite.GearUsedName;
                    row["Sector"] = item.GearInLandingSite.SectorName;
                    row["Number of landings of gear"] = item.GearInLandingSite.CountGearLandings;
                    row["Total weight of catch of gear"] = item.GearInLandingSite.WeightGearLandings;

                }
                else if (item.GearUnload != null)
                {
                    row["Gear"] = item.GearUnload.GearUsedName;
                    row["Sector"] = item.GearUnload.Sector;
                }
                else
                {
                    row["Gear"] = DBNull.Value;
                    row["Sector"] = DBNull.Value;
                    row["Number of landings of gear"] = DBNull.Value;
                    row["Total weight of catch of gear"] = DBNull.Value;
                }
                _dailyLandingDataTable.Rows.Add(row);

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

            dc = new DataColumn { ColumnName = "Date of set", DataType = typeof(DateTime) };
            _effortSpeciesCrostabDataTable.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Date of haul", DataType = typeof(DateTime) };
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
                _effortSpeciesCrostabDataTable.Columns.Add(dc);
            }

            foreach (var item in _crossTabEfforts_vesselunloadGear)
            {
                var row = _effortSpeciesCrostabDataTable.NewRow();
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
                row["Sector"] = ctcp.UnloadSector;

                if (ctcp.FishingGroundGrid != null)
                {
                    row["Grid location"] = ctcp.FishingGroundGrid;
                    row["Longitude"] = ctcp.xCoordinate;
                    row["Latitude"] = ctcp.yCoordinate;
                }

                row["Gear"] = ctcp.Gear;

                if (ctcp.DateTimeGearSet == null || ctcp.DateTimeGearHaul == null)
                {
                    row["Date of set"] = DBNull.Value;
                    row["Date of haul"] = DBNull.Value;
                }
                else
                {
                    row["Date of set"] = ctcp.DateTimeGearSet;
                    row["Date of haul"] = ctcp.DateTimeGearHaul;
                }

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


                foreach (var ve in ctcp.VesselUnload_FishingGear.VesselUnload_Gear_Specs_ViewModel.VesselUnload_Gear_SpecCollection.ToList())
                {
                    string spec_name = "";
                    try
                    {
                        if (ve.EffortSpecification != null)
                        {
                            spec_name = ve.EffortSpecification.Name.Replace("/", " or ");
                            switch (ve.EffortSpecification.ValueType)
                            {
                                case ODKValueType.isBoolean:
                                    row[spec_name] = bool.Parse(ve.EffortValue);
                                    break;
                                case ODKValueType.isDecimal:
                                    row[spec_name] = double.Parse(ve.EffortValue);
                                    break;
                                case ODKValueType.isInteger:
                                    row[spec_name] = int.Parse(ve.EffortValue);
                                    break;
                                case ODKValueType.isText:
                                case ODKValueType.isUndefined:
                                    row[spec_name] = ve.EffortValue;
                                    break;
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        //row[ve.EffortSpecification.Name] = DBNull.Value;
                        if (ve.EffortValue.Length > 0 && ve.EffortSpecification.ValueType == ODKValueType.isInteger)
                        {
                            if(double.TryParse(ve.EffortValue,out double r))
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

                _effortSpeciesCrostabDataTable.Rows.Add(row);
            }


        }


        public static List<CrossTabLengthWeight> CrossTabLengthWeights { get { return _crossTabLenWts; } }
        public static List<CrossTabLength> CrossTabLengths { get { return _crossTabLengths; } }
        public static List<CrossTabMaturity> CrossTabMaturities { get { return _crossTabMaturities; } }

        public static List<CrossTabLenFreq> CrossTabLenFreqs { get { return _crossTabLenFreqs; } }
        public static DataTable DataTableCarrierMaturities { get { return _dailyCarrierMaturityTable; } }
        public static DataTable DataTableCarrierDailyLandings { get { return _dailyCarrierLandingsDataTable; } }

        public static DataTable DataTableCarrierCatchComposition { get { return _dailyCarrierCatchCompositionTable; } }
        public static DataTable DataTableCarrierLengthWeigths { get { return _dailyCarrierLengthWeightsDataTable; } }

        public static DataTable DataTableCarrierLengths { get { return _dailyCarrierLengthTable; } }

        public static DataTable DataTableCarrierLengthFreq { get { return _dailyCarrierLenFreqTable; } }
        public static DataTable CrossTabDailyLandings { get { return _dailyLandingDataTable; } }
        public static DataTable CrossTabEfforts { get { return _effortSpeciesCrostabDataTable; } }

        public static DataTable CrossTabAllEfforts { get { return _effortCrostabDataTable; } }

        public static bool IsCarrierLandding { get; set; }
        public static string ErrorMessage { get; private set; }
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
                        _dailyCarrierLandingsDataTable.TableName = "Carrier landings";
                        _dailyCarrierCatchCompositionTable.TableName = "Catch composition";
                        _dailyCarrierLengthWeightsDataTable.TableName = "Length weight";
                        _dailyCarrierLenFreqTable.TableName = "Length frequency";
                        _dailyCarrierLengthTable.TableName = "Length";
                        _dailyCarrierMaturityTable.TableName = "Maturity";


                        _crossTabDataSet.Tables.Add(_dailyCarrierLandingsDataTable);
                        _crossTabDataSet.Tables.Add(_dailyCarrierCatchCompositionTable);
                        _crossTabDataSet.Tables.Add(_dailyCarrierLengthWeightsDataTable);
                        _crossTabDataSet.Tables.Add(_dailyCarrierLenFreqTable);
                        _crossTabDataSet.Tables.Add(_dailyCarrierLengthTable);
                        _crossTabDataSet.Tables.Add(_dailyCarrierMaturityTable);
                    }
                    else
                    {
                        _effortSpeciesCrostabDataTable.TableName = "Effort";
                        _effortCrostabDataTable.TableName = "Effort (all)";
                        _dailyLandingDataTable.TableName = "Daily landings";

                        _crossTabDataSet.Tables.Add(_dailyLandingDataTable);
                        _crossTabDataSet.Tables.Add(_effortCrostabDataTable);
                        _crossTabDataSet.Tables.Add(_effortSpeciesCrostabDataTable);
                        _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLengths, "Length"));
                        _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLengthWeights, "Length-Weight"));
                        _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabMaturities, "Maturity"));
                        _crossTabDataSet.Tables.Add(ListToDataTable(CrossTabLenFreqs, "Len-Freq"));
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
