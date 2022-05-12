using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Database
{
    public class SummaryItemViewModel
    {
        private bool _editSuccess;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeViewData;
        public ObservableCollection<SummaryItem> SummaryItemCollection { get; set; }
        private SummaryItemRepository SummaryItems { get; set; }

        public List<GearUnload> GearUnloadsByMonth(DateTime monthOfSampling)
        {
            var g = _month_summaryItems.FirstOrDefault(t => t.Key == monthOfSampling).GroupBy(t => t.GearUnloadID);
            List<GearUnload> lg = new List<GearUnload>();
            foreach (var item in g)
            {
                var gu = item.FirstOrDefault().GearUnload;
                gu.Parent = new LandingSiteSampling
                {
                    SamplingDate = item.FirstOrDefault().SamplingDate
                };

                List<VesselUnload> lvu = new List<VesselUnload>();
                var vus = item.GroupBy(t => t.VesselUnloadID);
                foreach (var v in vus)
                {
                    lvu.Add(new VesselUnload
                    {
                        PK = v.Key,
                    });
                }
                gu.AttachedVesselUnloads = lvu;
                lg.Add(gu);

            }
            return lg;

        }

        
        public VesselUnload LastVesselUnload()
        {
            int lastPK = SummaryItemCollection.Max(i => i.VesselUnloadID);
            SummaryItem item = SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == lastPK);
            VesselUnload lastVU = null;
            foreach (var lss in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.Where(t => t.SamplingDate.Date == item.SamplingDate.Date))
            {
                foreach (var gu in lss.GearUnloadViewModel.GearUnloadCollection)
                {
                    if (gu.VesselUnloadViewModel == null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, updatesubViewModels: true);
                        lastVU = gu.VesselUnloadViewModel.VesselUnloadCollection.FirstOrDefault(t => t.PK == lastPK);
                        if (lastVU != null) break;
                    }
                    
                }
                if (lastVU != null) break;
            }
            return lastVU;
        }
        public int GetNextRecordNumber()
        {
            return SummaryItemCollection.Max(t => t.VesselUnloadID) + 1;
        }
        public int CountRecordsByFormID(string xlsFormIDString)
        {
            int ct = SummaryItemCollection.Count(t => t.XFormIdentifier == xlsFormIDString);
            if (ct == 0)
            {
                ct = SummaryItemCollection.Count;
            }
            return ct;
        }
        public DateTime LastSavedDateInDatabase(string xlsFormIDString)
        {
            DateTime? lastDate;

            try
            {
                lastDate = SummaryItemCollection
                   .Where(t => t.XFormIdentifier == xlsFormIDString)
                   .Max(t => t.DateAdded);
            }
            catch
            {
                lastDate = SummaryItemCollection
                   .Max(t => t.DateAdded);
            }

            return (DateTime)lastDate;

        }
        public List<GearUnload> GetGearUnloads(DateTime date_download)
        {
            List<GearUnload> gus = new List<GearUnload>();

            var lss_items = SummaryItemCollection.Where(t => t.DateAdded.Date == date_download.Date).OrderBy(t => t.SamplingDayID).GroupBy(t => t.SamplingDayID);


            //var lss = lss_items.First().First();
            foreach (var lss_group in lss_items)
            {
                var lss = lss_group.First();
                LandingSite lst = lss.LandingSiteID == null ? null : NSAPEntities.LandingSiteViewModel.GetLandingSite((int)lss.LandingSiteID);

                LandingSiteSampling ls = new LandingSiteSampling
                {
                    NSAPRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(lss.RegionID),
                    FMA = NSAPEntities.FMAViewModel.GetFMA(lss.FMAId),
                    LandingSiteID = lss.LandingSiteID,
                    LandingSite = lst,
                    LandingSiteText = lss.LandingSiteText,
                    FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(lss.FishingGroundID),
                    PK = lss.SamplingDayID,
                    SamplingDate = date_download
                };

                foreach (var gu_group in lss_group.GroupBy(t => t.GearUnloadID))
                {
                    GearUnload gu = new GearUnload
                    {
                        GearID = gu_group.First().GearCode,
                        PK = gu_group.First().GearUnloadID,
                        Parent = ls,
                        Boats = gu_group.First().GearUnloadBoats,
                        Catch = gu_group.First().GearUnloadCatch,
                        Gear = NSAPEntities.GearViewModel.GetGear(gu_group.First().GearCode),
                        GearUsedText = gu_group.First().GearText
                    };
                    gus.Add(gu);
                }
            }

            return gus;
        }
        public List<SummaryResults> GetEnumeratorSummaryByMonth(NSAPEnumerator en, DateTime monthSampled)
        {
            List<SummaryResults> results = new List<SummaryResults>();
            int seq = 0;
            foreach (var em_group in SummaryItemCollection.Where(t => t.EnumeratorID == en.ID && t.MonthSampled == monthSampled)
                        .OrderBy(t => t.SamplingDate)
                        .GroupBy(t => t.LandingSiteNameText))
            {
                foreach (var gr_group in em_group.GroupBy(t => t.GearUsedName))
                {
                    var gr = gr_group.First();
                    DBSummary summ = new DBSummary
                    {
                        LandingSiteName = gr.LandingSiteName,
                        GearName = gr.GearUsedName,
                        VesselUnloadCount = gr_group.Count(),
                        CountLandingsWithCatchComposition = gr_group.Count(t => t.HasCatchComposition == true),
                        TrackedOperationsCount = gr_group.Count(t => t.IsTracked == true),
                        FirstLandingFormattedDate = gr_group.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                        LastLandingFormattedDate = gr_group.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                        LatestDownloadFormattedDate = gr_group.Max(t => t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                        LatestEformVersion = gr_group.Last().FormVersion
                    };
                    results.Add(
                        new SummaryResults
                        {
                            Sequence = ++seq,
                            DBSummary = summ,
                            SummaryLevelType = SummaryLevelType.EnumeratedMonth
                        });
                }
            }
            return results;
        }
        public List<DateTime> GetMonthsSampledByEnumerator(NSAPEnumerator en)
        {
            List<DateTime> results = new List<DateTime>();
            foreach (var em_group in SummaryItemCollection.Where(t => t.EnumeratorID == en.ID)
                .OrderBy(t => t.SamplingDate)
                .GroupBy(t => t.SamplingDate.ToString("MMM-yyyy")))
            {
                results.Add(new DateTime(em_group.First().SamplingDate.Year, em_group.First().SamplingDate.Month, 1));
            }
            return results;
        }
        public List<GearUnload> GetGearUnloads(string gearUsedName, int offsetDays)
        {
            string lsName = _treeViewData.LandingSiteText;
            DateTime sDate = ((DateTime)_treeViewData.MonthSampled).AddDays(offsetDays);
            if (_treeViewData.LandingSite != null)
            {
                lsName = _treeViewData.LandingSite.LandingSiteName;
            }

            var summaryItems = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                t.FMAId == _treeViewData.FMA.FMAID &&
                                                t.FishingGroundID == _treeViewData.FishingGround.Code &&
                                                t.LandingSiteName == lsName &&
                                                t.GearUsedName == gearUsedName &&
                                                t.SamplingDate.Date == sDate.Date).ToList();

            HashSet<GearUnload> gear_unloads = new HashSet<GearUnload>(new GearUnloadComparer());
            foreach (var item in summaryItems)
            {

                gear_unloads.Add(item.GearUnload);
            }

            return gear_unloads.ToList();
        }

        public List<SummaryResults> GetRegionOverallSummary()
        {
            List<SummaryResults> ls = new List<SummaryResults>();
            var regionGroups = SummaryItemCollection.OrderBy(t => t.RegionSequence).GroupBy(t => t.RegionID);
            int seq = 0;
            foreach (var reg in regionGroups)
            {
                SummaryItem i = reg.OrderByDescending(t => t.SamplingDate).First();
                var gus = reg.GroupBy(t => t.GearUnloadID);
                DBSummary summ = new DBSummary
                {

                    NSAPRegionCode = i.RegionID,
                    GearUnloadCount = reg.GroupBy(t => t.GearUnloadID).Count(),
                    CountCompleteGearUnload = reg.Where(t => t.GearUnloadBoats != null && t.GearUnloadCatch != null).GroupBy(t => t.GearUnloadID).Count(),
                    VesselUnloadCount = reg.Count(),
                    CountLandingsWithCatchComposition = reg.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = reg.Count(t => t.IsTracked == true),
                    FirstSampledLandingDate = reg.Min(t => t.SamplingDate),
                    LastSampledLandingDate = reg.Max(t => t.SamplingDate),
                    FirstLandingFormattedDate = reg.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = reg.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    DateLastDownload = reg.Max(t => t.DateAdded),
                    LatestDownloadFormattedDate = reg.Max(t => t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = i.FormVersion,
                    FMACount = reg.GroupBy(t => t.FMAId).Count(),
                    FishingGroundCount = reg.GroupBy(t => t.FishingGroundID).Count(),
                    LandingSiteCount = reg.GroupBy(t => t.LandingSiteNameText).Count(),
                    FishingGearCount = reg.GroupBy(t => t.GearUsedName).Count(),
                    EnumeratorCount = reg.GroupBy(t => t.EnumeratorNameToUse).Count(),
                    FishingVesselCount = reg.GroupBy(t => t.VesselNameToUse).Count()
                };

                ls.Add(new SummaryResults
                {
                    Sequence = summ.NSAPRegion.Sequence,
                    DBSummary = summ,
                    SummaryLevelType = SummaryLevelType.RegionOverall
                });
            }

            if (ls.Count < NSAPEntities.NSAPRegionViewModel.Count)
            {
                foreach (NSAPRegion r in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
                {
                    if (ls.FirstOrDefault(t => t.DBSummary.NSAPRegionCode == r.Code) == null)
                    {
                        int fg_count = 0;
                        int ls_count = 0;


                        foreach (NSAPRegionFMA rf in r.FMAs)
                        {

                            fg_count += rf.FishingGroundCount;
                            foreach (NSAPRegionFMAFishingGround fg in rf.FishingGrounds)
                            {
                                ls_count += fg.LandingSiteCount;
                            }
                        }

                        ls.Add(new SummaryResults
                        {

                            Sequence = r.Sequence,
                            DBSummary = new DBSummary
                            {
                                NSAPRegionCode = r.Code,
                                FMACount = r.FMAs.Count,
                                FishingGroundCount = fg_count,
                                LandingSiteCount = ls_count,
                                FishingGearCount = r.Gears.Count,
                                EnumeratorCount = r.NSAPEnumerators.Count
                            },
                            SummaryLevelType = SummaryLevelType.RegionOverall
                        });
                    }
                }
            }
            return ls;
        }

        public List<SummaryResults> GetEnumeratorSummaryLatestUpload(NSAPRegion reg, string enumerator, DateTime lastSampling)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            var lastSamplings = SummaryItemCollection
                                .Where(t => t.Region.Code == reg.Code && t.EnumeratorNameToUse == enumerator && t.SamplingDate == lastSampling)
                                .OrderBy(t => t.SamplingDate)
                                .GroupBy(t => t.LandingSiteNameText);
            int seq = 0;
            foreach (var ls in lastSamplings)
            {
                foreach (var lss in ls.GroupBy(t => t.GearUsedName))
                {
                    DBSummary summ = new DBSummary
                    {
                        EnumeratorName = enumerator,
                        LandingSiteName = ls.First().LandingSiteNameText,
                        GearName = lss.First().GearUsedName,
                        VesselUnloadCount = lss.Count(),
                        LastLandingFormattedDate = lss.Last().SamplingDate.ToString("MMM-dd-yyyy HH:mm"),
                        LatestDownloadFormattedDate = lss.Last().DateAdded.ToString("MMM-dd-yyyy HH:mm"),
                        LatestEformVersion = lss.Last().FormVersion
                    };
                    resuts.Add
                        (
                        new SummaryResults
                        {
                            Sequence = ++seq,
                            DBSummary = summ,
                            SummaryLevelType = SummaryLevelType.SummaryOfEnumerators
                        });
                }
            }
            return resuts;
        }
        public List<SummaryResults> GetEnumeratorSummaryLatestUpload(NSAPRegion reg)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var enuData in SummaryItemCollection.Where(t => t.RegionID == reg.Code)
                        .OrderByDescending(t => t.SamplingDate)
                        .GroupBy(t => t.EnumeratorNameToUse))
            {
                var lastSamplingDate = enuData.First().SamplingDate;
                resuts.AddRange(GetEnumeratorSummaryLatestUpload(reg, enuData.First().EnumeratorNameToUse, lastSamplingDate));
            }
            return resuts;
        }
        public List<SampledLandingSite> GetSampledLandingSites(FishingGround fg, FMA fma, NSAPRegion nsapRegion)
        {
            List<SampledLandingSite> results = new List<SampledLandingSite>();
            foreach (var ls_group in SummaryItemCollection
                .Where(t => t.RegionID == nsapRegion.Code && t.FMAId == fma.FMAID && t.FishingGroundID == fg.Code && t.LandingSiteID != null)
                .GroupBy(t => t.LandingSiteID))
            {
                var ls = ls_group.First();
                LandingSite landingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite((int)ls_group.Key);
                SampledLandingSite sls = new SampledLandingSite
                {
                    LandingSiteID = landingSite.LandingSiteID,
                    LandingSiteName = landingSite.LandingSiteName,
                    Barangay = landingSite.Barangay,
                    Municipality = landingSite.Municipality,
                    Province = landingSite.Municipality.Province,
                    FishingGround = fg
                };
                results.Add(sls);
            }
            return results;
        }
        public List<SummaryResults> GetEnumeratorSummary(NSAPRegion reg, string enumeratorName)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var enuData in SummaryItemCollection.Where(t => t.RegionID == reg.Code && t.EnumeratorNameToUse == enumeratorName)
                        .OrderBy(t => t.SamplingDate)
                        .GroupBy(t => t.EnumeratorNameToUse))
            {
                foreach (var enu_ls in enuData.GroupBy(t => t.LandingSiteNameText))
                {
                    foreach (var enu_gear in enu_ls.GroupBy(t => t.GearUsedName))
                    {
                        DBSummary summ = new DBSummary
                        {
                            EnumeratorName = enumeratorName,
                            LandingSiteName = enu_ls.First().LandingSiteNameText,
                            GearName = enu_gear.First().GearUsedName,
                            VesselUnloadCount = enu_gear.Count(),
                            CountLandingsWithCatchComposition = enu_gear.Count(t => t.HasCatchComposition == true),
                            TrackedOperationsCount = enu_gear.Count(t => t.IsTracked == true),
                            FirstLandingFormattedDate = enu_gear.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                            LastLandingFormattedDate = enu_gear.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                            LatestDownloadFormattedDate = enu_gear.Max(t => t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                            LatestEformVersion = enu_gear.Last().FormVersion,
                        };

                        resuts.Add(
                            new SummaryResults
                            {
                                Sequence = ++seq,
                                DBSummary = summ,
                                SummaryLevelType = SummaryLevelType.Enumerator
                            });
                    }
                }
            }
            return resuts;
        }
        public List<SummaryResults> GetEnumeratorSummary(NSAPRegion reg)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var enuData in SummaryItemCollection.Where(t => t.RegionID == reg.Code)
                    .OrderBy(t => t.SamplingDate)
                    .GroupBy(t => t.EnumeratorNameToUse))
            {

                DBSummary summ = new DBSummary
                {
                    EnumeratorName = enuData.First().EnumeratorNameToUse,
                    VesselUnloadCount = enuData.Count(),
                    CountLandingsWithCatchComposition = enuData.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = enuData.Count(t => t.IsTracked == true),
                    FirstLandingFormattedDate = enuData.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = enuData.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = enuData.Max(t => t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = enuData.Last().FormVersion
                };
                SummaryResults sr = new SummaryResults
                {
                    Sequence = ++seq,
                    DBSummary = summ,
                    SummaryLevelType = SummaryLevelType.FishingGround
                };

                resuts.Add(sr);

            }
            return resuts;
        }
        public List<SummaryResults> GetRegionFishingGroundSummary(NSAPRegion region, FishingGround fishingGround)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var fgData in SummaryItemCollection.Where(t => t.RegionID == region.Code && t.FishingGroundID == fishingGround.Code)
                .OrderBy(t => t.SamplingDate)
                .GroupBy(t => t.LandingSiteNameText))
            {
                DBSummary summ = new DBSummary
                {
                    LandingSiteName = fgData.First().LandingSiteNameText,
                    GearUnloadCount = fgData.GroupBy(t => t.GearUnloadID).Count(),
                    CountCompleteGearUnload = fgData.Where(t => t.GearUnloadBoats != null && t.GearUnloadCatch != null).Count(),
                    VesselUnloadCount = fgData.Count(),
                    CountLandingsWithCatchComposition = fgData.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = fgData.Count(t => t.IsTracked == true),
                    FirstLandingFormattedDate = fgData.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = fgData.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = fgData.Max(t => t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = fgData.Last().FormVersion
                };
                SummaryResults sr = new SummaryResults
                {
                    Sequence = ++seq,
                    DBSummary = summ,
                    SummaryLevelType = SummaryLevelType.FishingGround
                };

                resuts.Add(sr);
            }
            return resuts.OrderBy(t => t.DBSummary.LandingSiteName).ToList();
        }
        public List<SummaryResults> GetRegionSummary(NSAPRegion region)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var fgData in SummaryItemCollection.Where(t => t.RegionID == region.Code)
                .OrderBy(t => t.SamplingDate)
                .GroupBy(t => t.FishingGroundID))
            {
                SummaryItem fg = fgData.First();
                DBSummary summ = new DBSummary
                {
                    FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(fgData.Key),
                    FMA = fg.FMA,
                    GearUnloadCount = fgData.GroupBy(t => t.GearUnloadID).Count(),
                    CountCompleteGearUnload = fgData.Where(t => t.GearUnloadCatch != null && t.GearUnloadBoats != null).Count(),
                    VesselUnloadCount = fgData.Count(),
                    CountLandingsWithCatchComposition = fgData.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = fgData.Count(t => t.IsTracked == true),
                    FirstLandingFormattedDate = fgData.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = fgData.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = fgData.Max(t => t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = fgData.Last().FormVersion
                };
                resuts.Add(

                    new SummaryResults
                    {
                        Sequence = ++seq,
                        DBSummary = summ,
                        SummaryLevelType = SummaryLevelType.Region
                    }
                );



            }
            return resuts;
        }
        public int GetEnumertorUnloadCount()
        {
            return SummaryItemCollection.GroupBy(t => t.EnumeratorID).Count();
        }
        public GearUnload GetGearUnload(string gearUsedName, int offsetDays)
        {
            string lsName = _treeViewData.LandingSiteText;
            DateTime sDate = ((DateTime)_treeViewData.MonthSampled).AddDays(offsetDays);
            if (_treeViewData.LandingSite != null)
            {
                lsName = _treeViewData.LandingSite.LandingSiteName;
            }
            var gu = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                t.FMAId == _treeViewData.FMA.FMAID &&
                                                t.FishingGroundID == _treeViewData.FishingGround.Code &&
                                                t.LandingSiteName == lsName &&
                                                t.GearUsedName == gearUsedName &&
                                                t.SamplingDate.Date == sDate.Date).FirstOrDefault().GearUnload;

            gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
            return gu;
        }

        public void RefreshLastPrimaryLeys()
        {
            LastPrimaryKeys = SummaryItems.GetLastPrimaryKeys();
        }
        public LastPrimaryKeys LastPrimaryKeys { get; set; }
        public SummaryItemViewModel()
        {
            SummaryItems = new SummaryItemRepository();
            SummaryItemCollection = new ObservableCollection<SummaryItem>(SummaryItems.SummaryItems);
            SummaryItemCollection.CollectionChanged += SummaryItemCollection_CollectionChanged;
            RefreshLastPrimaryLeys();
        }


        public VesselUnloadSummary GetVesselUnloadSummary()
        {
            VesselUnloadSummary vs = new VesselUnloadSummary();

            if (SummaryItemCollection.Count > 0)
            {
                vs.FirstSamplingDate = SummaryItemCollection.Min(t => t.SamplingDate);
                vs.LastSamplingDate = SummaryItemCollection.Max(t => t.SamplingDate);
                vs.LatestDownloadDate = SummaryItemCollection.Max(t => t.DateAdded);
                vs.CountUnloadsWithCatchComposition = SummaryItemCollection.Count(t => t.HasCatchComposition == true);

                return vs;
            }
            else
            {
                return null;
            }
        }
        private List<IGrouping<string, SummaryItem>> _fishing_ground_summaryItems;
        private List<IGrouping<string, SummaryItem>> _fma_summaryItems;
        private List<IGrouping<int, SummaryItem>> _region_summaryItems;
        private List<IGrouping<DateTime, SummaryItem>> _month_summaryItems;
        public TreeViewModelControl.AllSamplingEntitiesEventHandler TreeViewData
        {
            get
            {
                return _treeViewData;
            }
            set
            {
                DBSummary summary;
                List<SummaryItem> items;
                SummaryResults = new List<SummaryResults>();
                int counter = 0;
                _treeViewData = value;
                switch (_treeViewData.TreeViewEntity)
                {
                    case "tv_NSAPRegionViewModel":
                        //items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                        //            t.LandingSiteID != null).ToList();
                        items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code).ToList();

                        _region_summaryItems = items.GroupBy(t => t.FMAId)
                                                            .OrderBy(t => t.First().FMA.FMAID).ToList();
                        foreach (var rg in _region_summaryItems)
                        {
                            var rgn = rg.First();
                            summary = new DBSummary
                            {
                                NSAPRegionCode = rgn.RegionID,
                                FMA = rgn.FMA,
                                GearUnloadCount = rg.GroupBy(t => t.GearUnloadID).Count(),
                                CountCompleteGearUnload = rg.Where(t => t.GearUnloadBoats != null && t.GearUnloadCatch != null).GroupBy(t => t.GearUnloadID).Count(),
                                VesselUnloadCount = rg.Count(),
                                CountLandingsWithCatchComposition = rg.Count(t => t.HasCatchComposition == true),
                                TrackedOperationsCount = rg.Count(t => t.IsTracked == true),
                                FirstLandingFormattedDate = rg.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LastLandingFormattedDate = rg.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LatestDownloadFormattedDate = ((DateTime)rg.OrderByDescending(t => t.DateAdded).FirstOrDefault().DateAdded).ToString("MMM-dd-yyyy")
                            };
                            SummaryResults.Add(new SummaryResults
                            {
                                SummaryLevelType = SummaryLevelType.FMA,
                                DBSummary = summary,
                                Sequence = ++counter
                            });
                        }
                        if (SummaryResults.Count < _treeViewData.NSAPRegion.FMAs.Count)
                        {
                            foreach (var f in _treeViewData.NSAPRegion.FMAs)
                            {
                                if (SummaryResults.Where(t => t.DBSummary.FMA.FMAID == f.FMAID).ToList().Count == 0)
                                {
                                    SummaryResults.Add(new SummaryResults
                                    {
                                        SummaryLevelType = SummaryLevelType.FMA,
                                        DBSummary = new DBSummary { FMA = f.FMA },
                                        Sequence = ++counter
                                    });
                                }
                            }
                        }

                        break;
                    case "tv_FMAViewModel":
                        //items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                        //                                    t.FMAId == _treeViewData.FMA.FMAID &&
                        //                                    t.LandingSiteID != null).ToList();

                        items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                            t.FMAId == _treeViewData.FMA.FMAID).ToList();

                        _fma_summaryItems = items.GroupBy(t => t.FishingGroundID)
                                                            .OrderBy(t => t.First().FishingGround.Name).ToList();

                        foreach (var fm in _fma_summaryItems)
                        {
                            var fg = fm.First();
                            summary = new DBSummary
                            {
                                FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(fm.Key),
                                FMA = fg.FMA,
                                GearUnloadCount = fm.GroupBy(t => t.GearUnloadID).Count(),
                                CountCompleteGearUnload = fm.Where(t => t.GearUnloadBoats != null && t.GearUnloadCatch != null).GroupBy(t => t.GearUnloadID).Count(),
                                VesselUnloadCount = fm.Count(),
                                CountLandingsWithCatchComposition = fm.Count(t => t.HasCatchComposition == true),
                                TrackedOperationsCount = fm.Count(t => t.IsTracked == true),
                                FirstLandingFormattedDate = fm.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LastLandingFormattedDate = fm.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LatestDownloadFormattedDate = ((DateTime)fm.OrderByDescending(t => t.DateAdded).FirstOrDefault().DateAdded).ToString("MMM-dd-yyyy")
                            };
                            SummaryResults.Add(new SummaryResults
                            {
                                SummaryLevelType = SummaryLevelType.FMA,
                                DBSummary = summary,
                                Sequence = ++counter
                            });
                        }
                        var fgs = NSAPEntities.NSAPRegionViewModel.GetRegionFMAFishingGrounds(_treeViewData.NSAPRegion.Code, _treeViewData.FMA.FMAID);

                        if (SummaryResults.Count < fgs.Count)
                        {
                            foreach (var fg in fgs)
                            {
                                if (SummaryResults.Where(t => t.DBSummary.FishingGround.Code == fg.Code).ToList().Count == 0)
                                {
                                    SummaryResults.Add(new SummaryResults
                                    {
                                        SummaryLevelType = SummaryLevelType.FMA,
                                        DBSummary = new DBSummary { FishingGround = fg },
                                        Sequence = ++counter
                                    });
                                }
                            }
                        }


                        break;
                    case "tv_FishingGroundViewModel":
                        //items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                        //                                    t.FMAId == _treeViewData.FMA.FMAID &&
                        //                                    t.FishingGroundID == _treeViewData.FishingGround.Code &&
                        //                                    t.LandingSiteID != null).ToList();

                        items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                            t.FMAId == _treeViewData.FMA.FMAID &&
                                                            t.FishingGroundID == _treeViewData.FishingGround.Code).ToList();

                        //_fishing_ground_summaryItems = items.GroupBy(t => (int)t.LandingSiteID)
                        //                                    .OrderBy(t => t.First().LandingSiteName).ToList();


                        _fishing_ground_summaryItems = items.GroupBy(t => t.LandingSiteNameText.Trim())
                                                            .OrderBy(t => t.First().LandingSiteName).ToList();
                        foreach (var fg in _fishing_ground_summaryItems)
                        {
                            var ls = fg.First();
                            summary = new DBSummary
                            {
                                LandingSiteName = ls.LandingSiteID == null ? $"{fg.Key}*" : fg.Key,
                                FMA = ls.FMA,
                                GearUnloadCount = fg.GroupBy(t => t.GearUnloadID).Count(),
                                CountCompleteGearUnload = fg.Where(t => t.GearUnloadBoats != null && t.GearUnloadCatch != null).GroupBy(t => t.GearUnloadID).Count(),
                                VesselUnloadCount = fg.Count(),
                                CountLandingsWithCatchComposition = fg.Count(t => t.HasCatchComposition == true),
                                TrackedOperationsCount = fg.Count(t => t.IsTracked == true),
                                FirstLandingFormattedDate = fg.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LastLandingFormattedDate = fg.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LatestDownloadFormattedDate = ((DateTime)fg.OrderByDescending(t => t.DateAdded).FirstOrDefault().DateAdded).ToString("MMM-dd-yyyy")
                            };
                            SummaryResults.Add(new SummaryResults
                            {
                                SummaryLevelType = SummaryLevelType.FishingGround,
                                DBSummary = summary,
                                Sequence = ++counter
                            });
                        }
                        break;
                    case "tv_LandingSiteViewModel":
                    case "tv_MonthViewModel":

                        if (_treeViewData.LandingSite != null)
                        {
                            items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                                                                t.FMAId == _treeViewData.FMA.FMAID &&
                                                                                                t.FishingGroundID == _treeViewData.FishingGround.Code &&
                                                                                                t.LandingSiteID == _treeViewData.LandingSite.LandingSiteID).ToList();
                        }
                        else
                        {
                            items = SummaryItemCollection.Where(t => t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                                    t.FMAId == _treeViewData.FMA.FMAID &&
                                                                    t.FishingGroundID == _treeViewData.FishingGround.Code &&
                                                                    t.LandingSiteNameText == _treeViewData.LandingSiteText).ToList();
                        }


                        _month_summaryItems = items.GroupBy(t => t.MonthSampled)
                                              .OrderBy(t => t.Key)
                                              .ToList();

                        foreach (var month in _month_summaryItems)
                        {
                            summary = new DBSummary()
                            {
                                MonthSampled = month.Key.ToString("MMM-yyyy"),
                                SampledMonth = month.Key,
                                GearUnloadCount = month.GroupBy(t => t.GearUnloadID).Count(),
                                VesselUnloadCount = month.Count(),
                                LastLandingFormattedDate = month.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                FirstLandingFormattedDate = month.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy"),
                                LatestDownloadFormattedDate = ((DateTime)month.OrderByDescending(t => t.DateAdded).FirstOrDefault().DateAdded).ToString("MMM-dd-yyyy"),
                                TrackedOperationsCount = month.Count(t => t.IsTracked == true),
                                CountLandingsWithCatchComposition = month.Count(t => t.HasCatchComposition == true),
                                CountCompleteGearUnload = month.Where(t => t.GearUnloadBoats != null && t.GearUnloadCatch != null).GroupBy(t => t.GearUnloadID).Count()
                            };

                            SummaryResults.Add(new SummaryResults
                            {
                                SummaryLevelType = SummaryLevelType.LandingSite,
                                DBSummary = summary,
                                Sequence = ++counter
                            });
                        }

                        break;
                        //case "tv_MonthViewModel":
                        //    break;
                }
            }
        }

        public List<int> GearUnloadPKs(DateTime date_downloaded)
        {
            List<int> keys = new List<int>();
            foreach (var item in SummaryItemCollection.Where(t => t.DateAdded.Date == date_downloaded.Date).GroupBy(t => t.GearUnloadID))
            {
                keys.Add(item.Key);
            }
            return keys;
        }
        public List<SummaryItem> GetUnloadStatisticsByDate(DateTime date_download)
        {
            return SummaryItemCollection
                .Where(t => t.DateAdded.Date == date_download)
                .OrderBy(t => t.EnumeratorNameToUse)
                .ThenBy(t => t.GearUsedName)
                .ToList();
        }
        public int CountLandingsWithCatchComposition()
        {
            return SummaryItemCollection.Count(t => t.HasCatchComposition == true);
        }
        public List<SummaryItem> GetDownloadDetailsByDate(DateTime date_download, bool isTracked = false)
        {
            return SummaryItemCollection
                .Where(t => t.DateAdded.Date == date_download && t.IsTracked == isTracked)
                .OrderBy(t => t.EnumeratorNameToUse)
                .ThenBy(t => t.GearUsedName)
                .ToList();
        }
        public List<SummaryResults> GetDownloadSummaryByDate(DateTime date_download)
        {
            var v = from e_d in SummaryItemCollection.Where(t => t.DateAdded.Date == date_download).OrderBy(t => t.SamplingDate)
                    group e_d by new
                    {
                        e_d.EnumeratorNameToUse,
                        e_d.GearUsedName,
                    } into e_d2

                    select new DBSummary()
                    {
                        EnumeratorName = e_d2.First().EnumeratorNameToUse,
                        GearName = e_d2.First().GearUsedName,
                        FirstLandingFormattedDate = e_d2.Min(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                        LastLandingFormattedDate = e_d2.Max(t => t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                        LatestDownloadFormattedDate = date_download.ToString("MMM-dd-yyyy"),
                        VesselUnloadCount = e_d2.Count(),
                        CountLandingsWithCatchComposition = e_d2.Count(t => t.HasCatchComposition == true),
                        TrackedOperationsCount = e_d2.Count(t => t.IsTracked == true),
                        LatestEformVersion = e_d2.Last().FormVersion
                    };

            List<SummaryResults> se = new List<SummaryResults>();
            int count = 0;
            foreach (var item in v)
            {
                SummaryResults sr = new SummaryResults
                {
                    Sequence = ++count,
                    DBSummary = item,
                    SummaryLevelType = SummaryLevelType.SummaryOfDownloadDate
                };
                se.Add(sr);
            }
            return se;
        }
        public void ResetResults()
        {
            SummaryResults = new List<SummaryResults>();
        }
        public List<SummaryResults> SummaryResults { get; private set; }

        public void Clear()
        {
            SummaryItemCollection.Clear();
        }
        public bool AddRecordToRepo(VesselUnload vu)
        {
            SummaryItem si = new SummaryItem
            {
                ID=SummaryItemCollection.Count+1,
                
                SamplingDayID = vu.Parent.Parent.PK,
                LandingSiteID = vu.Parent.Parent.LandingSiteID,
                LandingSiteText = vu.Parent.Parent.LandingSiteText,
                FMAId = vu.Parent.Parent.FMAID,
                RegionSequence = vu.Parent.Parent.NSAPRegion.Sequence,
                RegionID = vu.Parent.Parent.NSAPRegionID,
                FishingGroundID = vu.Parent.Parent.FishingGroundID,

                GearUnloadID = vu.Parent.PK,
                GearUnloadBoats = vu.Parent.Boats,
                GearUnloadCatch = vu.Parent.Catch,
                GearCode = vu.Parent.GearID,
                GearText = vu.Parent.GearUsedText,

                VesselUnloadID=vu.PK,    
                XFormIdentifier = vu.XFormIdentifier,
                ODKRowID = vu.ODKRowID,
                GPSCode = vu.GPSCode,
                NumberOfFishers = vu.NumberOfFishers,
                FormVersion = vu.FormVersion,
                VesselName = vu.VesselName,
                VesselID=vu.VesselID,
                VesselText=vu.VesselText,
                HasCatchComposition = vu.HasCatchComposition,
                IsTracked = vu.OperationIsTracked,
                IsTripCompleted = vu.FishingTripIsCompleted,
                EnumeratorID=vu.NSAPEnumeratorID,
                EnumeratorText=vu.EnumeratorText,
                SamplingDate = vu.SamplingDate,
                IsSuccess = vu.OperationIsSuccessful,
                SectorCode= vu.SectorCode,
                DateAdded = (DateTime)vu.DateAddedToDatabase,
                FishingGridRows=vu.CountGrids,
                GearSoakRows = vu.CountGearSoak,
                VesselEffortRows = vu.CountEffortIndicators,
                CatchCompositionRows=vu.CountCatchCompositionItems,
                LenFreqRows = vu.CountLenFreqRows,
                LengthRows = vu.CountLengthRows,
                LenWtRows = vu.CountLenWtRows,
                CatchMaturityRows = vu.CountMaturityRows
            };

            SummaryItemCollection.Add(si);
            return _editSuccess;
        }
        public bool AddRecordToRepo(SummaryItem item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            SummaryItemCollection.Add(item);
            return _editSuccess;
        }
        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < SummaryItemCollection.Count)
            {
                if (SummaryItemCollection[index].ID == id)
                {
                    SummaryItemCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public bool UpdateRecordInRepo(SummaryItem item)
        {
            if (item.ID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < SummaryItemCollection.Count)
            {
                if (SummaryItemCollection[index].ID == item.ID)
                {
                    SummaryItemCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        private void SummaryItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = SummaryItems.Add(SummaryItemCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<SummaryItem> tempListOfRemovedItems = e.OldItems.OfType<SummaryItem>().ToList();
                        _editSuccess = SummaryItems.Delete(tempListOfRemovedItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<SummaryItem> tempList = e.NewItems.OfType<SummaryItem>().ToList();
                        _editSuccess = SummaryItems.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get
            {
                return SummaryItemCollection.Count;
            }
        }
    }
}
