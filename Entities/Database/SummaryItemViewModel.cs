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
        public SummaryItemViewModel()
        {
            SummaryItems = new SummaryItemRepository();
            SummaryItemCollection = new ObservableCollection<SummaryItem>(SummaryItems.SummaryItems);
            SummaryItemCollection.CollectionChanged += SummaryItemCollection_CollectionChanged;
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
        public void ResetResults()
        {
            SummaryResults = new List<SummaryResults>();
        }
        public List<SummaryResults> SummaryResults { get; private set; }
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
