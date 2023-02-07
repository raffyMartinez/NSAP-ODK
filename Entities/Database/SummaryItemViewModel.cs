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
        //private readonly object collectionLock = new object();


        private bool _editSuccess;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeViewData;
        private NSAPEntity _orphanedEntity;
        private List<OrphanedEnumerator> _orphanedEnumerators;
        private List<OrphanedLandingSite> _orphanedLandingSites;
        private List<OrphanedFishingGear> _orphanedFishingGears;
        public ObservableCollection<SummaryItem> SummaryItemCollection { get; set; }
        private SummaryItemRepository SummaryItems { get; set; }

        public event EventHandler<BuildSummaryReportEventArg> BuildingSummaryTable;
        public event EventHandler<BuildOrphanedEntityEventArg> BuildingOrphanedEntity;

        public SummaryItem CurrentEntity { get; private set; }
        public SummaryItem GetItem(VesselUnload vu)
        {
            return SummaryItemCollection.ToList().FirstOrDefault(t => t.VesselUnloadID == vu.PK);
        }
        public SummaryItem GetItem(string odkROWID)
        {
            return SummaryItemCollection.ToList().FirstOrDefault(t => t.ODKRowID == odkROWID);
            //lock (collectionLock)
            //{
            //    return SummaryItemCollection.ToList().FirstOrDefault(t => t.ODKRowID == odkROWID);
            //}
            //foreach(var item in SummaryItemCollection.ToList())
            //{
            //    if(item.ODKRowID==odkROWID)
            //    {
            //        return item;
            //    }
            //}
            //return null;
        }
        public bool UpdateRecordsInRepo(GearUnload gu)
        {
            int counter = 0;
            foreach (SummaryItem si in SummaryItemCollection
                        .Where(t => t.GearCode?.Length == 0 && t.GearText == gu.GearUsedText))
            {
                si.SamplingDayID = gu.Parent.PK;
                si.GearCode = gu.GearID;
                si.GearText = gu.GearUsedText;
                si.GearUnloadBoats = gu.Boats;
                si.GearUnloadCatch = gu.Catch;
                counter++;
            }
            return counter > 0;
        }

        public bool UpdateRecordsInRepo(string landingSiteText, int landingSiteID)
        {
            int count = 0;
            foreach (SummaryItem si in SummaryItemCollection
                .Where(t => t.LandingSiteText == landingSiteText && t.LandingSiteID == null)
                )
            {
                si.LandingSiteID = landingSiteID;
                count++;
            }
            return count > 0;
        }

        public Task<List<OrphanedFishingGear>> GetOrphanedFishingGearsAsync()
        {
            return Task.Run(() => GetOrphanedFishingGears());
        }

        public List<string> RowIDs_SamplingsWithMissingLandingSiteInformation()
        {
            List<string> row_ids = new List<string>();
            foreach (var item in SummaryItemCollection.Where(t => t.LandingSiteID == null && string.IsNullOrEmpty(t.LandingSiteText)))
            {
                row_ids.Add(item.ODKRowID);
            }
            return row_ids;
        }

        public Dictionary<LandingSite, UnloadMeasureSummaryForMonth> GetUnloadMonthlySummaries()
        {
            Dictionary<LandingSite, UnloadMeasureSummaryForMonth> d = new Dictionary<LandingSite, UnloadMeasureSummaryForMonth>();
            foreach (LandingSite ls in NSAPEntities.LandingSiteViewModel.LandingSiteCollection)
            {
                foreach (var item in SummaryItemCollection
                    .Where(t => t.LandingSiteID == ls.LandingSiteID)
                    .GroupBy(t => t.SamplingMonthYear())
                    )
                {

                    //UnloadMeasureSummaryForMonth ums;

                    UnloadMeasureSummaryForMonth ums = new UnloadMeasureSummaryForMonth();
                    d.Add(ls, ums);
                    //d[ls].Month = item.First(t => (DateTime)t.SamplingMonthYear());
                    d[ls].CountGMS = item.Sum(t => t.CatchMaturityRows);
                    d[ls].CountL = item.Sum(t => t.LengthRows);
                    d[ls].CountLF = item.Sum(t => t.LengthRows);
                    d[ls].CountLW += item.Sum(t => t.LenWtRows);
                }
            }
            return d;
        }
        public List<OrphanedFishingGear> GetOrphanedFishingGears()
        {
            //var items = GearUnloadCollection
            //    .Where(t => t.GearID!=null && t.GearID.Length == 0 && t.GearUsedText!=null && t.GearUsedText.Length>0)
            //    .OrderBy(t => t.GearUsedText)
            //    .GroupBy(t => t.GearUsedText).ToList();
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildStart, isIndeterminate: true);

            var items = SummaryItemCollection
                .Where(t => (t.GearCode == null || t.GearCode.Length == 0) && t.GearText != null && t.GearText.Length > 0)
                .OrderBy(t => t.GearText)
                .GroupBy(t => t.GearText).ToList();

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, totalRows: items.Count());
            int counter = 0;
            var list = new List<OrphanedFishingGear>();
            foreach (var item in items)
            {

                //var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(item.First().SamplingDayID);
                //if(landingSiteSampling.GearUnloadViewModel==null)
                //{
                //    landingSiteSampling.GearUnloadViewModel = new GearUnloadViewModel(landingSiteSampling);
                //}
                //var lss= item.GroupBy(t => t.SamplingDayID);
                var orphan = new OrphanedFishingGear
                {
                    Name = item.Key,
                    //GearUnloads = GearUnloadCollection.Where(t => t.GearUsedText == item.Key).ToList()
                    //GearUnloads = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(item.First().SamplingDayID).GearUnloadViewModel.GearUnloadCollection.ToList()
                    GearUnloads = GetGearUnloads(item.Key)
                };
                list.Add(orphan);
                counter++;
                ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildEnd, totalRowsFetched: counter);
            return list;

        }


        public bool UpdateRecordsInRepo(LandingSiteSampling lss)
        {
            int counter = 0;
            foreach (SummaryItem si in SummaryItemCollection
                .Where(t => t.SamplingDayID == lss.PK))
            {
                si.RegionID = lss.NSAPRegionID;
                si.FMAId = lss.FMAID;
                si.FishingGroundID = lss.FishingGroundID;
                si.LandingSiteID = lss.LandingSiteID;
                counter++;
            }
            return counter > 0;
        }

        public Task<List<OrphanedLandingSite>> GetOrphanedLandingSitesAsync()
        {
            return Task.Run(() => GetOrphanedLandingSites());
        }

        public List<OrphanedLandingSite> GetOrphanedLandingSites()
        {
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildStart, isIndeterminate: true);
            List<OrphanedLandingSite> thisList = new List<OrphanedLandingSite>();
            int counter = 0;


            var orphanedLandingSites = SummaryItemCollection.Where(t => t.LandingSiteID == null)
                .OrderBy(t => t.LandingSiteText)
                .GroupBy(t => new
                {
                    LandingSiteName = t.LandingSiteText,
                    Enumerator = t.EnumeratorNameToUse,
                    Region = t.Region,
                    FMA = t.FMA,
                    FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(t.FishingGroundID),
                })
                .Select(ls => new
                {
                    LandingSiteName = ls.Key.LandingSiteName,
                    Enumerator = ls.Key.Enumerator,
                    Region = ls.Key.Region,
                    FMA = ls.Key.FMA,
                    FishingGround = ls.Key.FishingGround
                }).ToList();

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, totalRows: orphanedLandingSites.Count());


            foreach (var item in orphanedLandingSites)
            {
                //var landingSiteSamplings = new List<LandingSiteSampling>();
                var lss = GetLandingSiteSamplings(item.Enumerator, item.LandingSiteName);
                var orphan = new OrphanedLandingSite
                {
                    EnumeratorName = item.Enumerator,
                    LandingSiteName = item.LandingSiteName,
                    Region = item.Region,
                    FMA = item.FMA,
                    FishingGround = item.FishingGround,
                    //LandingSiteSamplings = GetLandingSiteSamplings(item.Enumerator, item.LandingSiteName),
                    LandingSiteSamplings = lss,
                    CanBeDeletedNow = OrphanLandingSiteCanBeDeleted(lss),
                    NumberOfVesselLandings = SummaryItemCollection.Where(t => t.LandingSiteID == null && t.VesselUnloadID != null && t.LandingSiteText == item.LandingSiteName && t.EnumeratorNameToUse == item.Enumerator).Count()
                };

                thisList.Add(orphan);
                counter++;
                ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildEnd, totalRowsFetched: counter);
            return thisList;
        }

        private bool OrphanLandingSiteCanBeDeleted(List<LandingSiteSampling> lss)
        {
            bool canBeDeletedNow = true;
            foreach (var item in lss)
            {
                if (item.GearUnloadViewModel.Count > 0)
                {
                    canBeDeletedNow = false;
                    break;
                }
            }
            return canBeDeletedNow;
        }

        public List<LandingSiteSampling> GetLandingSiteSamplings(string enumerator, string landingSiteName)
        {
            List<int> samplingDayIDs = new List<int>();
            List<LandingSiteSampling> lss = new List<LandingSiteSampling>();
            foreach (var item in SummaryItemCollection.Where(t => t.EnumeratorNameToUse == enumerator && t.LandingSiteNameText == landingSiteName))
            {
                if (!samplingDayIDs.Contains(item.SamplingDayID))
                {
                    samplingDayIDs.Add(item.SamplingDayID);
                }
            }

            foreach (var item in samplingDayIDs)
            {
                lss.Add(NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(item));
            }
            return lss;
        }
        public List<OrphanedLandingSite> GetOrphanedLandingSitesex()
        {
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildStart, isIndeterminate: true);
            List<OrphanedLandingSite> thisList = new List<OrphanedLandingSite>();
            int counter = 0;


            var orphanedLandingSites = SummaryItemCollection.Where(t => t.LandingSiteID == null)
                .OrderBy(t => t.LandingSiteText)
                .GroupBy(t => t.LandingSiteText);

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, totalRows: orphanedLandingSites.Count());


            foreach (var item in orphanedLandingSites)
            {
                var landingSiteSamplings = new List<LandingSiteSampling>();
                var orphan = new OrphanedLandingSite { LandingSiteName = item.Key };
                foreach (var sd in item)
                {
                    landingSiteSamplings.Add(NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.FirstOrDefault(t => t.PK == sd.SamplingDayID));
                }
                orphan.LandingSiteSamplings = landingSiteSamplings;
                thisList.Add(orphan);
                counter++;
                ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildEnd, totalRowsFetched: counter);
            return thisList;
        }
        public List<OrphanedLandingSite> GetOrphanedLandingSites1()
        {
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildStart, isIndeterminate: true);
            List<OrphanedLandingSite> thisList = new List<OrphanedLandingSite>();
            int counter = 0;


            var orphanedLandingSites = SummaryItemCollection.Where(t => t.LandingSiteID == null)
                .OrderBy(t => t.LandingSiteText)
                .GroupBy(t => t.LandingSiteText);

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, totalRows: orphanedLandingSites.Count());


            foreach (var item in orphanedLandingSites)
            {
                var orphan = new OrphanedLandingSite
                {
                    LandingSiteName = item.Key,
                    LandingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.Where(t => t.LandingSiteName == item.Key).ToList()
                };
                thisList.Add(orphan);
                counter++;
                ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildEnd, totalRowsFetched: counter);
            return thisList;
        }
        public int GetLandingSiteSamplingMaxRecordNumber()
        {
            if (SummaryItemCollection.Count == 0)
            {
                return 0;
            }
            else
            {

                //var max=SummaryItemCollection.Max(t => t.SamplingDayID);
                //return SummaryItemCollection.Max(t => t.SamplingDayID);
                return LandingSiteSamplingRepository.MaxRecordNumber_from_db();
            }
        }
        public int GetGearUnloadMaxRecordNumber()
        {
            if (SummaryItemCollection.Count == 0)
            {
                return 0;
            }
            else
            {
                return SummaryItemCollection.Where(t => t.GearUnloadID != null).Max(t => (int)t.GearUnloadID);
                //return SummaryItemCollection.Max(t => (int)t.GearUnloadID);
            }
        }

        public int GetVesselUnloadMaxRecordNumber()
        {
            if (SummaryItemCollection.Count == 0)
            {
                return 0;
            }
            else
            {
                return SummaryItemCollection.Max(t => (int)t.VesselUnloadID);
            }
        }

        public DateTime GetLastSubmissionDate()
        {
            return SummaryItemCollection.Max(t => (DateTime)t.DateSubmitted);
        }

        public DateTime GetFirstSubmissionDate()
        {
            return SummaryItemCollection.Min(t => (DateTime)t.DateSubmitted);
        }

        private string GetSectorCode()
        {
            return "";
        }

        public List<GearUnload> GearUnloadsByMonth(TreeViewModelControl.AllSamplingEntitiesEventHandler e, bool bySector = false)
        {
            List<GearUnload> lg = new List<GearUnload>();
            return lg;
        }

        private int GetSectorCount(List<VesselUnload> vu)
        {
            return vu.GroupBy(t => t.SectorCode).Count();

        }
        public List<GearUnload> GearUnloadsByMonth(DateTime monthOfSampling, bool bySector = false)
        {
            List<GearUnload> gear_unloads = new List<GearUnload>();
            if (bySector)
            {
                //var list_gu = _month_summaryItems.FirstOrDefault(t => t.Key == monthOfSampling).GroupBy(t => t.GearUnloadID).ToList();
                var list_gu = _month_summaryItems.FirstOrDefault(t => t.Key == monthOfSampling).GroupBy(x => new { x.GearUnloadID, x.SectorCode }).ToList();

                //int loopcount = 0;
                foreach (var item in list_gu)
                {

                    GearUnload gu;
                    //var items = item.ToList();
                    //int loop = 0;
                    gu = item.FirstOrDefault().GearUnload;
                    gu.SectorCode = item.FirstOrDefault().SectorCode;
                    if (GetSectorCount(gu.ListVesselUnload) == 1)
                    {
                        gear_unloads.Add(gu);
                    }
                    else
                    {
                        List<VesselUnload> list_vu = new List<VesselUnload>();
                        foreach (var vu in gu.ListVesselUnload.Where(t => t.SectorCode == gu.SectorCode).ToList())
                        {
                            list_vu.Add(vu);
                        }
                        gu.ListVesselUnload = list_vu;
                        gear_unloads.Add(gu);
                    }

                    //gu_1.SectorCode = items.FirstOrDefault().SectorCode;
                    //lg.Add(gu_1);
                    //var gu_group = from gu_item in item.ToList() 
                    //               group gu_item by gu_item.SectorCode 
                    //               into gu_item_group 
                    //               select new { gu_sector = gu_item_group.Key, gu_count = gu_item_group.Count(), vessel_unloads = gu_item_group };

                    //foreach (var gu_item in gu_group)
                    //{
                    //    var gu = gu_item.vessel_unloads.FirstOrDefault().GearUnload;
                    //    gu.SectorCode = gu_item.gu_sector;
                    //    lg.Add(gu);
                    //}
                    //loopcount++;
                }
            }
            else
            {
                gear_unloads = GearUnloadsByMonth(monthOfSampling);
            }
            return gear_unloads;
        }
        public List<GearUnload> GearUnloadsByMonth(DateTime monthOfSampling)
        {
            var g = _month_summaryItems.FirstOrDefault(t => t.Key == monthOfSampling).GroupBy(t => t.GearUnloadID);
            List<GearUnload> lg = new List<GearUnload>();
            foreach (var item in g)
            {
                var gu = item.FirstOrDefault().GearUnload;
                //gu.Parent = new LandingSiteSampling
                //{
                //    SamplingDate = item.FirstOrDefault().SamplingDate
                //};
                gu.Parent.SamplingDate = (DateTime)item.FirstOrDefault().SamplingDate;

                //var ii = from ix in item.ToList() group ix by ix.SectorCode into ix_group select new { sc = ix_group.Key, ct = ix_group.Count() };

                //if (ii.Count() == 2)
                //{
                //    gu.SectorCode = "cm";
                //}
                //else
                //{
                gu.SectorCode = item.FirstOrDefault().SectorCode;
                //}



                List<VesselUnload> lvu = new List<VesselUnload>();
                var vus = item.GroupBy(t => t.VesselUnloadID);
                foreach (var v in vus)
                {
                    lvu.Add(new VesselUnload
                    {
                        PK = (int)v.Key,
                    });
                }
                gu.AttachedVesselUnloads = lvu;
                lg.Add(gu);

            }
            return lg;

        }


        public VesselUnload LastVesselUnload()
        {
            int lastPK = SummaryItemCollection.Max(i => (int)i.VesselUnloadID);
            SummaryItem item = SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == lastPK);
            VesselUnload last_vessel_unload = null;
            foreach (var lss in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection.Where(t => t.SamplingDate.Date == ((DateTime)item.SamplingDate).Date))
            {
                foreach (var gu in lss.GearUnloadViewModel.GearUnloadCollection)
                {
                    if (gu.VesselUnloadViewModel == null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, updatesubViewModels: true);
                        last_vessel_unload = gu.VesselUnloadViewModel.VesselUnloadCollection.FirstOrDefault(t => t.PK == lastPK);
                        if (last_vessel_unload != null) break;
                    }

                }
                if (last_vessel_unload != null) break;
            }
            return last_vessel_unload;
        }
        public int CountByFormID(string xFormID)
        {
            return SummaryItemCollection.Count(t => t.XFormIdentifier == xFormID);
        }

        public int CountLandingsWithOrphanedEnumerators()
        {
            //return SummaryItemCollection.Count(t => t.EnumeratorID == null && t.EnumeratorText.Length > 0);
            return SummaryItemCollection.Count(t => t.EnumeratorID == null && string.IsNullOrEmpty(t.EnumeratorText));
        }

        public int CountLandingsWithOrphanedGears()
        {
            int count = 0;
            foreach (var grouping_gu in SummaryItemCollection.GroupBy(t => t.GearUnloadID))
            {
                var gu_first = grouping_gu.First();
                if (string.IsNullOrEmpty(gu_first.GearCode) && gu_first.GearText?.Length > 0)
                {
                    count++;
                }
            }
            return count;
        }

        public int CountLandingsWithOrphanedLandingSites()
        {
            int count = 0;
            foreach (var grouping_ls in SummaryItemCollection.Where(t => t.LandingSiteID == null).GroupBy(t => t.LandingSiteText))
            {
                count += grouping_ls.Count();
            }
            return count;
        }

        public int CountLandingsWithOrphanedFishingVessels()
        {
            return SummaryItemCollection.Count(t => t.VesselID == null && t.VesselText?.Length > 0);
        }
        public bool UpdateRecordInRepo(string rowID, string xFormID)
        {
            bool success = false;
            try
            {
                SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == rowID).XFormIdentifier = xFormID;
                success = true;
            }
            catch
            {
                //ignore
            }
            return success;
        }
        public bool UpdateRecordInRepo(VesselUnload vu, bool updateXFormID = false, bool updateEnumerators = false)
        {
            bool success = false;
            SummaryItem item = null;
            if (updateXFormID)
            {
                try
                {
                    SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == vu.ODKRowID).XFormIdentifier = vu.XFormIdentifier;
                    success = true;
                }
                catch
                {
                    //ignore
                }
            }
            else if (updateEnumerators)
            {
                var si = SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == vu.ODKRowID);
                si.EnumeratorID = vu.NSAPEnumeratorID;
                success = true;
            }
            else
            {
                item = SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == vu.PK);
                if (SummaryItemCollection.Remove(item))
                {
                    success = AddRecordToRepo(vu);
                }
            }
            return success;
        }
        public int GetNextRecordNumber()
        {
            return SummaryItemCollection.Max(t => (int)t.VesselUnloadID) + 1;
        }
        public int CountRecordsByFormID(string xlsFormIDString)
        {
            int ct = SummaryItemCollection.Count(t => t.XFormIdentifier == xlsFormIDString);
            //if (ct == 0)
            //{
            //    ct = SummaryItemCollection.Count;
            //}
            return ct;
        }
        public List<SummaryItem> GetSummaryItems(string enumenratorName, string landingSiteName)
        {
            return SummaryItemCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == enumenratorName && t.LandingSiteNameText == landingSiteName).ToList();
        }

        public List<VesselUnload> GetSampledVesselUnloads(string enumeratorName, string landingSiteName)
        {
            List<VesselUnload> this_list = new List<VesselUnload>();
            foreach (var item in SummaryItemCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == enumeratorName && t.LandingSiteNameText == landingSiteName))
            {

                this_list.Add(new VesselUnload { PK = (int)item.VesselUnloadID, ODKRowID = item.ODKRowID, GearUnloadID = (int)item.GearUnloadID });
                //this_list.Add(item.VesselUnload);
            }

            return this_list;
        }
        public List<VesselUnload> GetSampledVesselUnloads1(string enumeratorName, string landingSiteName)
        {
            List<VesselUnload> this_list = new List<VesselUnload>();
            foreach (var item in SummaryItemCollection.Where(t => t.EnumeratorText == enumeratorName && t.LandingSiteNameText == landingSiteName))
            {
                this_list.Add(NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(item.SamplingDayID)
                    .GearUnloadViewModel.GetGearUnload((int)item.GearUnloadID, loadVesselViewModel: true)
                    .VesselUnloadViewModel.getVesselUnload((int)item.VesselUnloadID));
            }

            return this_list;
        }

        public VesselUnload VesselUnload { get; private set; }
        public List<OrphanedEnumerator> OrphanedEnumerators
        {
            get { return _orphanedEnumerators; }
        }

        public List<OrphanedLandingSite> OrphanedLandingSites
        {
            get { return _orphanedLandingSites; }
        }
        public List<OrphanedFishingGear> OrphanedFishingGears
        {
            get { return _orphanedFishingGears; }
        }
        public async Task SetOrphanedEntityAsync(NSAPEntity orphanedEntity)
        {
            switch (orphanedEntity)
            {
                case NSAPEntity.Enumerator:
                    _orphanedEnumerators = await GetOrphanedEnumeratorsAsync();
                    break;
                case NSAPEntity.LandingSite:
                    _orphanedLandingSites = await GetOrphanedLandingSitesAsync();
                    break;
                case NSAPEntity.FishingGear:
                    _orphanedFishingGears = await GetOrphanedFishingGearsAsync();
                    break;
            }
        }
        public Task<List<OrphanedEnumerator>> GetOrphanedEnumeratorsAsync()
        {
            return Task.Run(() => GetOrphanedEnumerators());
        }

        public List<OrphanedEnumerator> GetOrphanedEnumerators()
        {
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildStart, isIndeterminate: true);
            List<OrphanedEnumerator> a_list = new List<OrphanedEnumerator>();
            var vesselUnloadsWithOrphanedEnumerators = SummaryItemCollection
               .Where(t => t.EnumeratorID == null && t.EnumeratorText.Length > 0)
               .GroupBy(t => new
               {
                   LandingSiteName = t.LandingSiteNameText,
                   EnumeratorName = t.EnumeratorNameToUse,
                   FMA = t.FMA,
                   Region = t.Region,
                   FishingGround = t.FishingGround
               })
               .Select(enumerator => new
               {
                   LandingSiteName = enumerator.Key.LandingSiteName,
                   EnumeratorName = enumerator.Key.EnumeratorName,
                   FMA = enumerator.Key.FMA,
                   Region = enumerator.Key.Region,
                   FishingGround = enumerator.Key.FishingGround,
               }).ToList();

            int counter = 0;
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, totalRows: vesselUnloadsWithOrphanedEnumerators.Count);
            foreach (var item in vesselUnloadsWithOrphanedEnumerators)
            {
                var orphan = new OrphanedEnumerator
                {
                    Name = item.EnumeratorName,
                    SampledLandings = GetSampledVesselUnloads(item.EnumeratorName, item.LandingSiteName),
                    Region = item.Region,
                    FMA = item.FMA,
                    FishingGround = item.FishingGround,
                    LandingSiteName = item.LandingSiteName,
                    SummaryItems = SummaryItemCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == item.EnumeratorName && t.LandingSiteNameText == item.LandingSiteName).ToList()
                };
                a_list.Add(orphan);
                counter++;
                ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildEnd, totalRowsFetched: counter);
            return a_list.OrderBy(t => t.Name.Trim()).ToList();
        }
        public List<OrphanedEnumerator> GetOrphanedEnumerators1()
        {
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildStart, isIndeterminate: true);
            List<OrphanedEnumerator> a_list = new List<OrphanedEnumerator>();
            var vesselUnloadsWithOrphanedEnumerators = SummaryItemCollection
               .Where(t => t.EnumeratorID == null && t.EnumeratorText.Length > 0)
               .GroupBy(t => new { LandingSiteName = t.LandingSiteNameText, EnumeratorName = t.EnumeratorNameToUse })
               .Select(enumerator => new
               {
                   LandingSiteName = enumerator.Key.LandingSiteName,
                   EnumeratorName = enumerator.Key.EnumeratorName
               }).ToList();

            int counter = 0;
            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, totalRows: vesselUnloadsWithOrphanedEnumerators.Count);
            foreach (var item in vesselUnloadsWithOrphanedEnumerators)
            {
                var orphan = new OrphanedEnumerator
                {
                    Name = item.EnumeratorName,
                    SampledLandings = GetSampledVesselUnloads(item.EnumeratorName, item.LandingSiteName)
                };
                a_list.Add(orphan);
                counter++;
                ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProceessBuildOrphanedEntitiesEvent(status: BuildOrphanedEntityStatus.StatusBuildEnd, totalRowsFetched: counter);
            return a_list.OrderBy(t => t.Name.Trim()).ToList();
        }

        public DateTime? LastSubmittedDateInDatabase(string xlsFormIDString)
        {
            DateTime? lastDate;

            try
            {
                lastDate = SummaryItemCollection
                    .Where(t => t.XFormIdentifier == xlsFormIDString)
                    .Max(t => t.DateSubmitted);
            }
            catch
            {
                lastDate = null;
            }

            return lastDate;
        }
        public DateTime? LastSavedDateInDatabase(string xlsFormIDString)
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
                lastDate = null;
            }

            return lastDate;

        }
        public Task<List<GearUnload>> GetGearUnloadsAsync(DateTime date_download)
        {
            return Task.Run(() => GetGearUnloads(date_download));
        }
        public List<GearUnload> GetGearUnloads(DateTime date_download)
        {

            List<GearUnload> gus = new List<GearUnload>();

            var ls_gu = SummaryItemCollection
                .Where(t => t.GearUnloadID != null && t.DateAdded != null && ((DateTime)t.DateAdded).Date == date_download.Date)
                .OrderBy(t => t.SamplingDayID)
                .GroupBy(x => new { x.SamplingDayID, x.GearUnloadID });


            ProcessBuildEvent(BuildSummaryReportStatus.StatusBuildStart, totalRows: ls_gu.Count());

            int counter = 0;
            foreach (var item in ls_gu)
            {
                string gear_code = item.First().GearCode;
                GearUnload gu = new GearUnload
                {
                    GearID = gear_code,
                    PK = (int)item.Key.GearUnloadID,
                    Parent = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(item.Key.SamplingDayID),
                    Boats = item.First().GearUnloadBoats,
                    Catch = item.First().GearUnloadCatch,
                    Gear = NSAPEntities.GearViewModel.GetGear(gear_code),
                    GearUsedText = item.First().GearText,
                    NumberOfSampledLandingsEx = item.Count()
                };
                //if (gu.VesselUnloadViewModel == null)
                //{
                //    gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
                //}
                gus.Add(gu);
                counter++;

                ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildFetchedRow, currentRow: counter);
            }

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: counter);
            return gus;
        }

        public Task<List<SummaryResults>> GetEnumeratorSummaryByMonthAsync(NSAPRegion reg, NSAPEnumerator en, DateTime monthSampled)
        {
            return Task.Run(() => GetEnumeratorSummaryByMonth(reg, en, monthSampled));
        }

        public List<SummaryResults> GetEnumeratorSummaryByMonth(NSAPRegion reg, NSAPEnumerator en, DateTime monthSampled)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> results = new List<SummaryResults>();
            int seq = 0;

            var enumeratorSummaryGroup =
                from e in SummaryItemCollection.Where(t => t.RegionID == reg.Code && t.EnumeratorNameToUse == en.Name && t.MonthSampled == monthSampled)
                     .OrderBy(t => t.SamplingDate)
                group e by new
                {
                    e.FMA,
                    e.FishingGround,
                    e.LandingSite,
                    e.EnumeratorNameToUse,
                    e.GearName,
                };

            foreach (var item in enumeratorSummaryGroup)
            {
                DBSummary summ = new DBSummary
                {
                    FMA = item.First().FMA,
                    EnumeratorName = en.Name,
                    FishingGround = item.First().FishingGround,
                    LandingSiteName = item.First().LandingSiteNameText,
                    GearName = item.First().GearUsedName,
                    VesselUnloadCount = item.Count(),
                    CountLandingsWithCatchComposition = item.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = item.Count(t => t.IsTracked == true),
                    FirstLandingFormattedDate = item.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = item.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = item.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = item.Last().FormVersion,
                };

                results.Add(
                    new SummaryResults
                    {
                        Sequence = ++seq,
                        DBSummary = summ,
                        SummaryLevelType = SummaryLevelType.Enumerator
                    });
            }

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: results.Count);
            return results;
        }
        public List<SummaryResults> GetEnumeratorSummaryByMonth1(NSAPEnumerator en, DateTime monthSampled)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
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
                        EnumeratorName = en.Name,
                        LandingSiteName = gr.LandingSiteNameText,
                        GearName = gr.GearUsedName,
                        VesselUnloadCount = gr_group.Count(),
                        CountLandingsWithCatchComposition = gr_group.Count(t => t.HasCatchComposition == true),
                        TrackedOperationsCount = gr_group.Count(t => t.IsTracked == true),
                        FirstLandingFormattedDate = gr_group.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                        LastLandingFormattedDate = gr_group.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                        LatestDownloadFormattedDate = gr_group.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
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
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: results.Count);
            return results;
        }

        public List<VesselUnload> GetVesselUnloads(SummaryResults sr)
        {
            DBSummary dbs = sr.DBSummary;
            DateTime fs = DateTime.Parse(dbs.FirstLandingFormattedDate);
            DateTime ls = DateTime.Parse(dbs.LastLandingFormattedDate);

            List<VesselUnload> unloads = new List<VesselUnload>();

            foreach (SummaryItem si in SummaryItemCollection.Where(t => t.EnumeratorNameToUse == dbs.EnumeratorName &&
                                         t.GearUsedName == dbs.GearName &&
                                         t.SamplingDate >= fs &&
                                         t.SamplingDate <= ls))
            {
                unloads.Add(GetVesselUnload(si));
            }

            return unloads;
        }
        public VesselUnload GetVesselUnload(SummaryItem si)
        {
            if (si.GearUnloadID != null && si.VesselUnloadID != null)
            {
                //var ls = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(si.SamplingDayID);
                //var gu = ls.GearUnloadViewModel.getGearUnload((int)si.GearUnloadID,loadVesselViewModel:true);
                //var vu = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(si.SamplingDayID)
                //    .GearUnloadViewModel.getGearUnload((int)si.GearUnloadID, loadVesselViewModel: true).
                //    VesselUnloadViewModel.getVesselUnload((int)si.VesselUnloadID);
                return NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(si.SamplingDayID)
                    .GearUnloadViewModel.GetGearUnload((int)si.GearUnloadID, loadVesselViewModel: true).
                    VesselUnloadViewModel.getVesselUnload((int)si.VesselUnloadID);
            }
            else
            {
                return null;
            }
        }
        public List<DateTime> GetMonthsSampledByEnumerator(NSAPEnumerator en)
        {
            List<DateTime> results = new List<DateTime>();
            foreach (var em_group in SummaryItemCollection.Where(t => t.EnumeratorID == en.ID)
                .OrderBy(t => t.SamplingDate)
                .GroupBy(t => ((DateTime)t.SamplingDate).ToString("MMM-yyyy")))
            {

                results.Add(new DateTime(((DateTime)em_group.First().SamplingDate).Year, ((DateTime)em_group.First().SamplingDate).Month, 1));
            }
            return results;
        }

        public List<ServerUploadsByMonth> ListServerUploadsByMonths()
        {
            var reg_fg_ls = SummaryItemCollection
                .Where(t => t.DateSubmitted != null)
            .GroupBy(t => new
            {
                Koboserver = NSAPEntities.KoboServerViewModel.GetKoboServer(t.XFormIdentifier),
                MonthOfSubmission = new DateTime(((DateTime)t.DateSubmitted).Year, ((DateTime)t.DateSubmitted).Month, 1)
            })
            .Select(submission => new
            {
                KoboServer = submission.Key.Koboserver,
                MonthOfSubmission = submission.Key.MonthOfSubmission,
                Count = submission.Count()
            }).ToList();

            List<ServerUploadsByMonth> list = new List<ServerUploadsByMonth>();
            foreach (var item in reg_fg_ls)
            {
                list.Add(
                    new ServerUploadsByMonth
                    {
                        CountUploads = item.Count,
                        MonthOfSubmission = item.MonthOfSubmission,
                        Koboserver = item.KoboServer,
                        CountEnumerators = NumberOfEnumeratorsByMonthForKoboServer(item.KoboServer, item.MonthOfSubmission)
                    }
                );
            }
            return list;
        }
        public List<GearUnload> GetGearUnloadsFromTree(TreeViewModelControl.AllSamplingEntitiesEventHandler treeData)
        {
            var landingSiteNameToUse = treeData.LandingSiteText;
            if (treeData.LandingSite != null)
            {
                landingSiteNameToUse = treeData.LandingSite.ToString();
            }
            List<GearUnload> unload_list = new List<GearUnload>();
            List<SummaryItem> items = new List<SummaryItem>();
            if (treeData.TreeViewEntity == "tv_LandingSiteViewModel")
            {
                items = SummaryItemCollection.Where(t => t.Region.Code == treeData.NSAPRegion.Code &&
                    t.FMA.FMAID == treeData.FMA.FMAID &&
                    t.FishingGround.Code == treeData.FishingGround.Code &&
                    t.LandingSite != null &&
                    t.LandingSite.ToString() == landingSiteNameToUse).ToList()

                    .GroupBy(gu => gu.GearUnloadID).Select(x => x.First()).ToList();
            }
            else if (treeData.TreeViewEntity == "tv_MonthViewModel")
            {
                items = SummaryItemCollection.Where(t => t.Region.Code == treeData.NSAPRegion.Code &&
                    t.FMA.FMAID == treeData.FMA.FMAID &&
                    t.FishingGround.Code == treeData.FishingGround.Code &&
                    t.LandingSite != null &&
                    t.LandingSite.ToString() == landingSiteNameToUse &&
                    t.SamplingDate > (DateTime)treeData.MonthSampled && t.SamplingDate < ((DateTime)treeData.MonthSampled).AddMonths(1)).ToList()

                    .GroupBy(gu => gu.GearUnloadID).Select(x => x.First()).ToList();
            }
            else if (treeData.TreeViewEntity == "tv_FishingGroundViewModel")
            {
                items = SummaryItemCollection.Where(t => t.Region.Code == treeData.NSAPRegion.Code &&
                    t.SamplingDate != null &&
                    t.FMA.FMAID == treeData.FMA.FMAID &&
                    t.FishingGround.Code == treeData.FishingGround.Code).ToList()

                    .GroupBy(gu => gu.GearUnloadID).Select(x => x.First()).ToList();
            }

            foreach (var item in items)
            {
                unload_list.Add(item.GearUnload);
            }

            return unload_list;
        }
        private int NumberOfEnumeratorsByMonthForKoboServer(Koboserver ks, DateTime monthSubmitted)
        {
            if (ks != null)
            {
                //var n = SummaryItemCollection.Where(t => t.MonthSubmitted == monthSubmitted && t.XFormIdentifier == ks.ServerID).GroupBy(t => t.EnumeratorNameToUse).Count();
                return SummaryItemCollection.Where(t => t.MonthSubmitted == monthSubmitted && t.XFormIdentifier == ks.ServerID).GroupBy(t => t.EnumeratorNameToUse).Count();
            }
            else
            {
                return 0;
            }
        }
        public List<GearUnload> GetGearUnloads(string gearUsedName, int offsetDays, string sectorCode = "")
        {
            if (offsetDays >= 0)
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
                                                    (t.LandingSiteName == lsName || t.LandingSiteID == _treeViewData.LandingSite.LandingSiteID) &&
                                                    t.GearUsedName == gearUsedName &&
                                                    ((DateTime)t.SamplingDate).Date == sDate.Date).ToList();

                if (!string.IsNullOrEmpty(sectorCode))
                {
                    summaryItems = summaryItems.Where(t => t.SectorCode == sectorCode).ToList();
                }

                HashSet<GearUnload> gear_unloads = new HashSet<GearUnload>(new GearUnloadComparer());
                //foreach (var item in summaryItems)
                //{
                //    if (item.GearUnload == null)
                //    {

                //    }
                //    gear_unloads.Add(item.GearUnload);
                //}

                //return gear_unloads.ToList();
                gear_unloads.Add(summaryItems.FirstOrDefault().GearUnload);
                return gear_unloads.ToList();
            }
            else
            {
                return null;
            }
        }

        public Task<List<SummaryResults>> GetRegionOverallSummaryAsync()
        {
            return Task.Run(() => GetRegionOverallSummary());
        }
        public List<SummaryResults> GetRegionOverallSummary()
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> ls = new List<SummaryResults>();
            var regionGroups = SummaryItemCollection
                .Where(t => t.SamplingDate != null && t.DateAdded != null)
                .OrderBy(t => t.RegionSequence).GroupBy(t => t.RegionID);
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
                    FirstSampledLandingDate = reg.Min(t => (DateTime)t.SamplingDate),
                    LastSampledLandingDate = reg.Max(t => (DateTime)t.SamplingDate),
                    FirstLandingFormattedDate = reg.Min(t => ((DateTime)t.SamplingDate)).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = reg.Max(t => ((DateTime)t.SamplingDate)).ToString("MMM-dd-yyyy HH:mm"),
                    DateLastDownload = reg.Max(t => (DateTime)t.DateAdded),
                    LatestDownloadFormattedDate = reg.Max(t => ((DateTime)t.DateAdded)).ToString("MMM-dd-yyyy HH:mm"),
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
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: ls.Count);
            return ls.OrderBy(t => t.DBSummary.NSAPRegion.Sequence).ToList();
        }

        public List<SummaryResults> GetEnumeratorSummaryLatestUpload(NSAPRegion reg, string enumerator, DateTime lastSampling)
        {
            List<SummaryResults> resuts = new List<SummaryResults>();
            var lastSamplings = SummaryItemCollection
                                .Where(t => t.Region.Code == reg.Code && t.EnumeratorNameToUse == enumerator && ((DateTime)t.SamplingDate).Date == lastSampling.Date)
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
                        LastSampledLandingDate = (DateTime)lss.Last().SamplingDate,
                        LastLandingFormattedDate = ((DateTime)lss.Last().SamplingDate).ToString("MMM-dd-yyyy"),
                        LatestDownloadFormattedDate = ((DateTime)lss.Last().DateAdded).ToString("MMM-dd-yyyy HH:mm"),
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

        public Task<List<SummaryResults>> GetEnumeratorSummaryLatestUploadAsync(NSAPRegion reg)
        {
            return Task.Run(() => GetEnumeratorSummaryLatestUpload(reg));
        }
        public List<SummaryResults> GetEnumeratorSummaryLatestUpload(NSAPRegion reg)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var enuData in SummaryItemCollection.Where(t => t.SamplingDate != null && t.RegionID == reg.Code)
                        .OrderByDescending(t => t.SamplingDate)
                        .GroupBy(t => t.EnumeratorNameToUse))
            {
                var lastSamplingDate = enuData.First().SamplingDate;
                resuts.AddRange(GetEnumeratorSummaryLatestUpload(reg, enuData.First().EnumeratorNameToUse, ((DateTime)lastSamplingDate).Date));
            }
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: resuts.Count);
            return resuts.OrderBy(t => t.DBSummary.EnumeratorName).OrderBy(t => t.DBSummary.EnumeratorName).ToList();
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

        public Task<List<SummaryResults>> GetEnumeratorSummaryAsync(NSAPRegion reg, string enumeratorName)
        {
            return Task.Run(() => GetEnumeratorSummary(reg, enumeratorName));
        }
        public List<SummaryResults> GetEnumeratorSummary(NSAPRegion reg, string enumeratorName)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> results = new List<SummaryResults>();
            int seq = 0;

            var enumeratorSummaryGroup =
                from e in SummaryItemCollection.Where(t => t.RegionID == reg.Code && t.EnumeratorNameToUse == enumeratorName)
                     .OrderBy(t => t.SamplingDate)
                group e by new
                {
                    e.FMA,
                    e.FishingGround,
                    e.LandingSite,
                    e.EnumeratorNameToUse,
                    e.GearName,
                };

            foreach (var item in enumeratorSummaryGroup)
            {
                DBSummary summ = new DBSummary
                {
                    FMA = item.First().FMA,
                    EnumeratorName = enumeratorName,
                    FishingGround = item.First().FishingGround,
                    LandingSiteName = item.First().LandingSiteNameText,
                    GearName = item.First().GearUsedName,
                    VesselUnloadCount = item.Count(),
                    CountLandingsWithCatchComposition = item.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = item.Count(t => t.IsTracked == true),
                    FirstLandingFormattedDate = item.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = item.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = item.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = item.Last().FormVersion,
                };

                results.Add(
                    new SummaryResults
                    {
                        Sequence = ++seq,
                        DBSummary = summ,
                        SummaryLevelType = SummaryLevelType.Enumerator
                    });
            }

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: results.Count);
            return results;
        }
        public List<SummaryResults> GetEnumeratorSummary1(NSAPRegion reg, string enumeratorName)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
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
                            FishingGround = enu_ls.First().FishingGround,
                            LandingSiteName = enu_ls.First().LandingSiteNameText,
                            GearName = enu_gear.First().GearUsedName,
                            VesselUnloadCount = enu_gear.Count(),
                            CountLandingsWithCatchComposition = enu_gear.Count(t => t.HasCatchComposition == true),
                            TrackedOperationsCount = enu_gear.Count(t => t.IsTracked == true),
                            FirstLandingFormattedDate = enu_gear.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                            LastLandingFormattedDate = enu_gear.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                            LatestDownloadFormattedDate = enu_gear.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
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
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: resuts.Count);
            return resuts.OrderBy(t => t.DBSummary.LandingSiteName).ToList();
        }

        public Task<List<SummaryResults>> GetEnumeratorSummaryAsync(NSAPRegion reg)
        {
            return Task.Run(() => GetEnumeratorSummary(reg));
        }
        public List<SummaryResults> GetEnumeratorSummary(NSAPRegion reg)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var enuData in SummaryItemCollection.Where(t => t.RegionID == reg.Code && t.SamplingDate != null && t.DateAdded != null)
                    .OrderBy(t => t.SamplingDate)
                    .GroupBy(t => t.EnumeratorNameToUse))
            {

                DBSummary summ = new DBSummary
                {
                    EnumeratorName = enuData.First().EnumeratorNameToUse,
                    VesselUnloadCount = enuData.Count(),
                    CountLandingsWithCatchComposition = enuData.Count(t => t.HasCatchComposition == true),
                    TrackedOperationsCount = enuData.Count(t => t.IsTracked == true),
                    FirstLandingFormattedDate = enuData.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = enuData.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = enuData.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
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

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: resuts.Count);
            return resuts.OrderBy(t => t.DBSummary.EnumeratorName).ToList();
        }

        public int SetFishingGroundOfSamplingDay(int samplingDayID, string fg_code)
        {
            int count = 0;
            foreach (var item in SummaryItemCollection.Where(t => t.SamplingDayID == samplingDayID).ToList())
            {
                item.FishingGroundID = fg_code;
                count++;
            }
            return count;
        }
        public Task<List<SummaryResults>> GetRegionFishingGroundSummaryAsync(NSAPRegion region, FishingGround fishingGround, FMA fma)
        {
            return Task.Run(() => GetRegionFishingGroundSummary(region, fishingGround, fma));
        }
        public List<SummaryResults> GetRegionFishingGroundSummary(NSAPRegion region, FishingGround fishingGround, FMA fma)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var fgData in SummaryItemCollection.Where(t =>
                t.SamplingDate != null &&
                t.DateAdded != null &&
                t.RegionID == region.Code && t.FMAId == fma.FMAID && t.FishingGroundID == fishingGround.Code)
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
                    FirstLandingFormattedDate = fgData.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = fgData.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = fgData.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = fgData.Last().FormVersion
                };

                //if(summ.VesselUnloadIDs==null)
                //{
                //    summ.VesselUnloadIDs = new List<int>();
                //}
                if (summ.SamplingDayIDs == null)
                {
                    summ.SamplingDayIDs = new List<int>();
                }

                foreach (var item in fgData)
                {
                    summ.SamplingDayIDs.Add(item.SamplingDayID);
                    //summ.VesselUnloadIDs.Add((int)item.VesselUnloadID);
                }

                SummaryResults sr = new SummaryResults
                {
                    Sequence = ++seq,
                    DBSummary = summ,
                    SummaryLevelType = SummaryLevelType.FishingGround
                };

                resuts.Add(sr);
            }
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: resuts.Count);
            return resuts.OrderBy(t => t.DBSummary.LandingSiteName).ToList();
        }

        public Task<List<SummaryResults>> GetFMASummaryAsync(NSAPRegion region, FMA fma)
        {
            return Task.Run(() => GetFMASummary(region,fma));
        }

        public List<SummaryResults> GetFMASummary(NSAPRegion region,FMA fma)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var fgData in SummaryItemCollection.Where(t => t.SamplingDate != null && t.DateAdded != null && t.RegionID == region.Code && t.FMAId==fma.FMAID)
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
                    FirstLandingFormattedDate = fgData.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = fgData.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = fgData.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
                    LatestEformVersion = fgData.Last().FormVersion
                };
                resuts.Add(

                    new SummaryResults
                    {
                        Sequence = ++seq,
                        DBSummary = summ,
                        SummaryLevelType = SummaryLevelType.FMA
                    }
                );



            }

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: resuts.Count);
            return resuts.OrderBy(t => t.DBSummary.FishingGround.Name).ToList();
        }
        public Task<List<SummaryResults>> GetRegionSummaryAsync(NSAPRegion region)
        {
            return Task.Run(() => GetRegionSummary(region));
        }
        public List<SummaryResults> GetRegionSummary(NSAPRegion region)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryResults> resuts = new List<SummaryResults>();
            int seq = 0;
            foreach (var fgData in SummaryItemCollection.Where(t => t.SamplingDate != null && t.DateAdded != null && t.RegionID == region.Code)
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
                    FirstLandingFormattedDate = fgData.Min(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LastLandingFormattedDate = fgData.Max(t => (DateTime)t.SamplingDate).ToString("MMM-dd-yyyy HH:mm"),
                    LatestDownloadFormattedDate = fgData.Max(t => (DateTime)t.DateAdded).ToString("MMM-dd-yyyy HH:mm"),
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

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: resuts.Count);
            return resuts.OrderBy(t => t.DBSummary.FMA.Name).ThenBy(t => t.DBSummary.FishingGround.Name).ToList();
        }
        public int GetEnumertorUnloadCount()
        {
            return SummaryItemCollection.GroupBy(t => t.EnumeratorID).Count();
        }
        public List<GearUnload> GetGearUnloads(string gearUsedText)
        {
            List<GearUnload> gearUnloads = new List<GearUnload>();
            foreach (var si in SummaryItemCollection.Where(t => t.GearText == gearUsedText && t.GearCode?.Length == 0).GroupBy(t => t.SamplingDayID))
            {
                var lss = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(si.Key);
                if (lss != null)
                {
                    if (lss.GearUnloadViewModel == null)
                    {
                        lss.GearUnloadViewModel = new GearUnloadViewModel(lss);
                    }
                    foreach (GearUnload gu in lss.GearUnloadViewModel.GetGearUnloads(gearUsedText))
                    {
                        gearUnloads.Add(gu);
                    }
                }
            }
            return gearUnloads;
        }

        public GearUnload GetGearUnload(int gearUnloadID)
        {
            return SummaryItemCollection.FirstOrDefault(t => t.GearUnloadID == gearUnloadID)?.GearUnload;
        }
        public GearUnload GetGearUnload(int landingSiteSamplingID, string gearName)
        {
            return SummaryItemCollection.FirstOrDefault(t => t.SamplingDayID == landingSiteSamplingID && t.GearUsedName == gearName)?.GearUnload;
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
                                               ((DateTime)t.SamplingDate).Date == sDate.Date).FirstOrDefault().GearUnload;

            gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
            return gu;
        }

        public void RefreshLastPrimaryLeys(bool delayedSave = false)
        {
            LastPrimaryKeys = SummaryItems.GetLastPrimaryKeys(delayedSave);
            //LastPrimaryKeys = SummaryItems.GetLastPrimaryKeys();
        }

        public List<string> GetXFormIDList()
        {
            List<string> list = new List<string>();
            foreach (var item in SummaryItemCollection.GroupBy(t => t.XFormIdentifier))
            {
                list.Add(item.Key);
            }
            return list;
        }

        public LastPrimaryKeys LastPrimaryKeys { get; set; }
        public SummaryItemViewModel()
        {
            SummaryItems = new SummaryItemRepository();
            SummaryItemCollection = new ObservableCollection<SummaryItem>(SummaryItems.SummaryItems);
            SummaryItemCollection.CollectionChanged += SummaryItemCollection_CollectionChanged;
            RefreshLastPrimaryLeys();
        }
        public List<VesselUnload> GetVesselUnloads(SummaryResults sr, string region, SummaryLevelType summaryLevelType, string sampledMonth = null)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryItem> enum_unloads = new List<SummaryItem>();
            switch (summaryLevelType)
            {
                case SummaryLevelType.EnumeratorRegion:
                    enum_unloads = SummaryItemCollection.Where(
                        t => t.EnumeratorNameToUse == sr.DBSummary.EnumeratorName &&
                        t.Region.ShortName == region &&
                        ((DateTime)t.SamplingDate).Date == sr.DBSummary.LastSampledLandingDate.Date &&
                        t.LandingSiteNameText == sr.DBSummary.LandingSiteName &&
                        t.GearUsedName == sr.DBSummary.GearName
                        ).ToList();
                    break;
                case SummaryLevelType.Enumerator:
                    if (sr.DBSummary.GearName == null)
                    {
                        enum_unloads = SummaryItemCollection.Where(
                            t => t.EnumeratorNameToUse == sr.DBSummary.EnumeratorName &&
                            t.Region.ShortName == region).ToList();
                    }
                    else
                    {
                        enum_unloads = SummaryItemCollection.Where(
                            t => t.EnumeratorNameToUse == sr.DBSummary.EnumeratorName &&
                            t.Region.ShortName == region &&
                            t.LandingSiteNameText == sr.DBSummary.LandingSiteName &&
                            t.GearUsedName == sr.DBSummary.GearName
                            ).ToList();
                    }
                    break;
                case SummaryLevelType.EnumeratedMonth:
                    DateTime start = DateTime.Parse(sr.DBSummary.FirstLandingFormattedDate);
                    DateTime end = DateTime.Parse(sr.DBSummary.LastLandingFormattedDate);


                    enum_unloads = SummaryItemCollection.Where(
                        t => t.EnumeratorNameToUse == sr.DBSummary.EnumeratorName &&
                        t.Region.ShortName == region &&
                        t.GearUsedName == sr.DBSummary.GearName &&
                        t.SamplingDate >= start &&
                        t.SamplingDate <= end &&
                        t.LandingSiteNameText == sr.DBSummary.LandingSiteName
                        ).ToList();
                    break;
            }

            List<VesselUnload> unloads = new List<VesselUnload>();

            foreach (var si in enum_unloads)
            {
                unloads.Add(NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(si.SamplingDayID)
                        .GearUnloadViewModel.GetGearUnload((int)si.GearUnloadID, true)
                        .VesselUnloadViewModel.getVesselUnload((int)si.VesselUnloadID));
            }
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: unloads.Count);
            return unloads;
        }

        public List<EnumeratorFormVersion> EnumeratorsAndLatestFormVersion()
        {
            List<EnumeratorFormVersion> en_fvs = new List<EnumeratorFormVersion>();
            foreach (var en_fv in SummaryItemCollection.OrderBy(t => t.EnumeratorNameToUse).ThenBy(t => t.SamplingDate).GroupBy(t => t.EnumeratorNameToUse))
            {
                var en = en_fv.First();
                if (en.EnumeratorID != null)
                {
                    en_fvs.Add(new EnumeratorFormVersion
                    {
                        NSAPEnumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)en.EnumeratorID),
                        FormVersion = en_fv.Last().FormVersion,
                        LastSamplingDate = (DateTime)en_fv.Last().SamplingDate
                    });
                }

            }
            return en_fvs;
        }
        public List<VesselUnload> GetVesselUnloads(string summaryRegion, string summaryFMA)
        {
            //         var vesselUnloadsWithOrphanedEnumerators = SummaryItemCollection
            //.Where(t => t.EnumeratorID == null && t.EnumeratorText.Length > 0)
            //.GroupBy(t => new { LandingSiteName = t.LandingSiteNameText, EnumeratorName = t.EnumeratorNameToUse })
            //.Select(enumerator => new
            //{
            //    LandingSiteName = enumerator.Key.LandingSiteName,
            //    EnumeratorName = enumerator.Key.EnumeratorName
            //}).ToList();

            List<VesselUnload> unloads = new List<VesselUnload>();

            var reg_fg_ls = SummaryItemCollection.Where(
                        t => t.Region.ShortName == summaryRegion &&
                        t.FMA.Name == summaryFMA)

                .GroupBy(t => new
                {
                    SamplingDayID = t.SamplingDayID,
                    GearUnloadID = t.GearUnloadID
                })
                .Select(sampling => new
                {
                    SamplingDay_id = sampling.Key.SamplingDayID,
                    GU_id = sampling.Key.GearUnloadID
                }).ToList();


            foreach (var si in reg_fg_ls)
            {
                unloads.AddRange(NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(si.SamplingDay_id)
                        .GearUnloadViewModel.GetGearUnload((int)si.GU_id, true)
                        .VesselUnloadViewModel.VesselUnloadCollection.ToList());
            }

            return unloads;
        }
        public List<VesselUnload> GetVesselUnloads(string summaryRegion, string summaryFishingGround, string summaryLandingSite)
        {
            //         var vesselUnloadsWithOrphanedEnumerators = SummaryItemCollection
            //.Where(t => t.EnumeratorID == null && t.EnumeratorText.Length > 0)
            //.GroupBy(t => new { LandingSiteName = t.LandingSiteNameText, EnumeratorName = t.EnumeratorNameToUse })
            //.Select(enumerator => new
            //{
            //    LandingSiteName = enumerator.Key.LandingSiteName,
            //    EnumeratorName = enumerator.Key.EnumeratorName
            //}).ToList();

            List<VesselUnload> unloads = new List<VesselUnload>();

            var reg_fg_ls = SummaryItemCollection.Where(
                        t => t.Region.ShortName == summaryRegion &&
                        t.FishingGround.Name == summaryFishingGround &&
                        t.LandingSiteNameText == summaryLandingSite)

                .GroupBy(t => new
                {
                    SamplingDayID = t.SamplingDayID,
                    GearUnloadID = t.GearUnloadID
                })
                .Select(sampling => new
                {
                    SamplingDay_id = sampling.Key.SamplingDayID,
                    GU_id = sampling.Key.GearUnloadID
                }).ToList();


            foreach (var si in reg_fg_ls)
            {
                unloads.AddRange(NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(si.SamplingDay_id)
                        .GearUnloadViewModel.GetGearUnload((int)si.GU_id, true)
                        .VesselUnloadViewModel.VesselUnloadCollection.ToList());
            }

            return unloads;
        }
        public int CountMissingLandingSiteInformation()
        {
            return SummaryItemCollection.Count(t => t.LandingSiteID == null && string.IsNullOrEmpty(t.LandingSiteText));
        }

        public int CountMissingEnumeratorInformation()
        {
            return SummaryItemCollection.Count(t => t.EnumeratorID == null && string.IsNullOrEmpty(t.EnumeratorText));
        }
        public int CountMissingFishingGearInformation()
        {
            return SummaryItemCollection.Count(t => string.IsNullOrEmpty(t.GearCode) && string.IsNullOrEmpty(t.GearText));
        }

        public int CountMissingFormIDs()
        {
            //return SummaryItemCollection.Count(t => t.XFormIdentifier.Length==0);
            return SummaryItemCollection.Count(t => string.IsNullOrEmpty(t.XFormIdentifier));
        }
        public VesselUnloadSummary GetVesselUnloadSummary()
        {
            VesselUnloadSummary vs = new VesselUnloadSummary();

            if (SummaryItemCollection.Count > 0)
            {
                vs.FirstSamplingDate = SummaryItemCollection.Where(t => t.SamplingDate != null).Min(t => (DateTime)t.SamplingDate);
                vs.LastSamplingDate = SummaryItemCollection.Where(t => t.SamplingDate != null).Max(t => (DateTime)t.SamplingDate);
                vs.LatestDownloadDate = SummaryItemCollection.Where(t => t.DateAdded != null).Max(t => (DateTime)t.DateAdded);
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
                        items = SummaryItemCollection.Where(t =>
                                                            t.SamplingDate != null &&
                                                            t.DateAdded != null &&
                                                            t.RegionID == _treeViewData.NSAPRegion.Code).ToList();

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
                                FirstLandingFormattedDate = ((DateTime)rg.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
                                LastLandingFormattedDate = ((DateTime)rg.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
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

                        items = SummaryItemCollection.Where(t => t.SamplingDate != null &&
                                                            t.DateAdded != null &&
                                                            t.RegionID == _treeViewData.NSAPRegion.Code &&
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
                                FirstLandingFormattedDate = ((DateTime)fm.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
                                LastLandingFormattedDate = ((DateTime)fm.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
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

                        items = SummaryItemCollection.Where(t =>
                                                            t.SamplingDate != null &&
                                                            t.DateAdded != null &&
                                                            t.RegionID == _treeViewData.NSAPRegion.Code &&
                                                            t.FMAId == _treeViewData.FMA.FMAID &&
                                                            t.FishingGroundID == _treeViewData.FishingGround.Code).ToList();


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
                                FirstLandingFormattedDate = ((DateTime)fg.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
                                LastLandingFormattedDate = ((DateTime)fg.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
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


                        _month_summaryItems = items.Where(t => t.MonthSampled != null).GroupBy(t => (DateTime)t.MonthSampled)
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
                                LastLandingFormattedDate = ((DateTime)month.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
                                FirstLandingFormattedDate = ((DateTime)month.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate).ToString("MMM-dd-yyyy"),
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
                }
            }
        }

        public int? UpdateLandingSiteInLanding(string samplingIdentifierUUID, string landingSiteText)
        {
            var landing = SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == samplingIdentifierUUID);
            if (landing != null && landing.LandingSiteText.Length == 0 && landing.LandingSiteID == null && landingSiteText != null)
            {
                SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == samplingIdentifierUUID).LandingSiteText = landingSiteText.Replace("»", ",");
                return landing.SamplingDayID;
            }
            return null;
        }

        public bool LandingHasLandingSite(string samplingIdentifierUUID)
        {
            return SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == samplingIdentifierUUID) != null;
        }
        internal bool XformIDExists(string formID)
        {
            var item = SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == formID);
            return item != null;

        }

        private void ProceessBuildOrphanedEntitiesEvent(BuildOrphanedEntityStatus status, int? totalRows = null, int? currentRow = null, int? totalRowsFetched = null, bool isIndeterminate = false)
        {
            switch (status)
            {
                case BuildOrphanedEntityStatus.StatusBuildStart:
                    if (!isIndeterminate)
                    {

                    }
                    else
                    {
                        BuildingOrphanedEntity?.Invoke(null, new BuildOrphanedEntityEventArg { BuildOrphanedEntityStatus = BuildOrphanedEntityStatus.StatusBuildStart, IsIndeterminate = isIndeterminate });
                    }
                    break;
                case BuildOrphanedEntityStatus.StatusBuildFirstRecordFound:
                    BuildingOrphanedEntity?.Invoke(null, new BuildOrphanedEntityEventArg { BuildOrphanedEntityStatus = BuildOrphanedEntityStatus.StatusBuildFirstRecordFound, TotalCount = (int)totalRows });
                    break;
                case BuildOrphanedEntityStatus.StatusBuildFetchedRow:
                    BuildingOrphanedEntity?.Invoke(null, new BuildOrphanedEntityEventArg { BuildOrphanedEntityStatus = BuildOrphanedEntityStatus.StatusBuildFetchedRow, CurrentCount = (int)currentRow });
                    break;
                case BuildOrphanedEntityStatus.StatusBuildEnd:
                    BuildingOrphanedEntity?.Invoke(null, new BuildOrphanedEntityEventArg { BuildOrphanedEntityStatus = BuildOrphanedEntityStatus.StatusBuildEnd, TotalCountFetched = (int)totalRowsFetched });
                    break;
            }
        }
        private void ProcessBuildEvent(BuildSummaryReportStatus status, int? totalRows = null, int? currentRow = null, int? totalRowsFetched = null, bool isIndeterminate = false)
        {
            switch (status)
            {
                case BuildSummaryReportStatus.StatusBuildStart:
                    if (!isIndeterminate)
                    {
                        BuildingSummaryTable?.Invoke(null, new BuildSummaryReportEventArg { BuildSummaryReportStatus = BuildSummaryReportStatus.StatusBuildStart, TotalRowCount = (int)totalRows, IsIndeterminate = isIndeterminate });
                    }
                    else
                    {
                        BuildingSummaryTable?.Invoke(null, new BuildSummaryReportEventArg { BuildSummaryReportStatus = BuildSummaryReportStatus.StatusBuildStart, IsIndeterminate = isIndeterminate });
                    }
                    break;
                case BuildSummaryReportStatus.StatusBuildFetchedRow:
                    BuildingSummaryTable?.Invoke(null, new BuildSummaryReportEventArg { BuildSummaryReportStatus = BuildSummaryReportStatus.StatusBuildFetchedRow, CurrentRow = (int)currentRow });
                    break;
                case BuildSummaryReportStatus.StatusBuildEnd:
                    BuildingSummaryTable?.Invoke(null, new BuildSummaryReportEventArg { BuildSummaryReportStatus = BuildSummaryReportStatus.StatusBuildEnd, TotalRowCount = (int)totalRowsFetched });
                    break;
            }
        }

        public List<int> GearUnloadPKs(DateTime date_downloaded)
        {
            List<int> keys = new List<int>();
            foreach (var item in SummaryItemCollection.Where(t => ((DateTime)t.DateAdded).Date == date_downloaded.Date).GroupBy(t => t.GearUnloadID))
            {
                keys.Add((int)item.Key);
            }
            return keys;
        }

        public Task<List<SummaryItem>> GetUnloadStatisticsByDateAsync(DateTime date_download)
        {
            return Task.Run(() => GetUnloadStatisticsByDate(date_download));
        }
        public List<SummaryItem> GetUnloadStatisticsByDate(DateTime date_download)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            var thisList = SummaryItemCollection
                .Where(t => t.DateAdded != null && t.SamplingDate != null && ((DateTime)t.DateAdded).Date == date_download)
                .OrderBy(t => t.EnumeratorNameToUse)
                .ThenBy(t => t.GearUsedName)
                .ToList();


            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: thisList.Count);
            return thisList;
        }
        public int CountLandingsWithCatchComposition()
        {
            return SummaryItemCollection.Count(t => t.HasCatchComposition == true);
        }




        public async Task<List<SummaryItem>> GetValidateLandedCatchWeightsAsync(DateTime date_download)
        {
            return await GetDownloadDetailsByDateAsync(date_download, forWeightValidation: true);
            //List<ValidateLandedCatchWeight> vlcws = new List<ValidateLandedCatchWeight>();
            //foreach (var item in items)
            //{
            //    ValidateLandedCatchWeight vlcw = new ValidateLandedCatchWeight
            //    {
            //        SummaryItem = item
            //    };
            //    vlcws.Add(vlcw);
            //}
            //return vlcws;
        }
        public Task<List<SummaryItem>> GetDownloadDetailsByDateAsync(DateTime date_download, bool? isTracked = null, bool forWeightValidation = false)
        {
            return Task.Run(() => GetDownloadDetailsByDate(date_download, isTracked));
        }


        public Task<List<SummaryItem>> GetDownloadDetailsByCalendarTreeSelectionTaskAsync(TreeViewModelControl.AllSamplingEntitiesEventHandler e, bool monthInTreeView = false)
        {
            return Task.Run(() => GetDownloadDetailsByCalendarTreeSelectionTask(e, monthInTreeView));
        }

        public List<SummaryItem> GetDownloadDetailsByCalendarTreeSelectionTask(TreeViewModelControl.AllSamplingEntitiesEventHandler e, bool monthInTreeView = false)
        {
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            List<SummaryItem> thisList = new List<SummaryItem>();
            if (e.TreeViewEntity == "tv_LandingSiteViewModel" || monthInTreeView)
            {
                thisList = SummaryItemCollection
                    .Where(
                        t => t.SamplingDate != null &&
                        t.SamplingDate >= (DateTime)e.MonthSampled &&
                        t.SamplingDate < ((DateTime)e.MonthSampled).AddMonths(1) &&
                        t.Region.Code == e.NSAPRegion.Code &&
                        t.FMAId == e.FMA.FMAID &&
                        t.FishingGroundID == e.FishingGround.Code &&
                        t.LandingSiteID == e.LandingSite.LandingSiteID
                        )
                    .ToList();
            }
            else
            {
                thisList = SummaryItemCollection
                    .Where(
                        t => t.SamplingDate != null &&
                        t.SamplingDate >= (DateTime)e.MonthSampled &&
                        t.SamplingDate < ((DateTime)e.MonthSampled).AddMonths(1) &&
                        t.Region.Code == e.NSAPRegion.Code &&
                        t.FMAId == e.FMA.FMAID &&
                        t.FishingGroundID == e.FishingGround.Code &&
                        t.LandingSiteID == e.LandingSite.LandingSiteID &&
                        t.GearName == e.GearUsed
                        )
                    .ToList();
            }
            //List<ValidateLandedCatchWeight> vlcws = new List<ValidateLandedCatchWeight>();
            //foreach (var item in thisList)
            //{
            //    ValidateLandedCatchWeight vlcw = new ValidateLandedCatchWeight
            //    {
            //        SummaryItem = item
            //    };
            //    vlcws.Add(vlcw);
            //}
            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: thisList.Count);
            return thisList;
            //return vlcws;
        }

        public List<SummaryItem> GetDownloadDetailsByDate(DateTime date_download, bool? isTracked = null, bool forWeightValidation = false)
        {
            if (!forWeightValidation)
            {
                ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, isIndeterminate: true);
            }
            List<SummaryItem> thisList = new List<SummaryItem>();
            if (isTracked == null)
            {
                thisList = SummaryItemCollection
                    .Where(t => t.SamplingDate != null && t.DateAdded != null && ((DateTime)t.DateAdded).Date == date_download)
                    .OrderBy(t => t.EnumeratorNameToUse)
                    .ThenBy(t => t.GearUsedName)
                    .ToList();
            }
            else
            {
                thisList = SummaryItemCollection
                    .Where(t => t.SamplingDate != null && t.DateAdded != null && ((DateTime)t.DateAdded).Date == date_download && t.IsTracked == isTracked)
                    .OrderBy(t => t.EnumeratorNameToUse)
                    .ThenBy(t => t.GearUsedName)
                    .ToList();
            }


            if (!forWeightValidation)
            {
                ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, totalRows: thisList.Count);
                ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: thisList.Count);
            }

            return thisList;
        }

        public Task<List<SummaryResults>> GetDownloadSummaryByDateAsync(DateTime date_download)
        {
            return Task.Run(() => GetDownloadSummaryByDate(date_download));
        }
        public List<SummaryResults> GetDownloadSummaryByDate(DateTime date_download)
        {
            var v = from e_d in SummaryItemCollection
                    .Where(t => t.DateAdded != null && t.SamplingDate != null && ((DateTime)t.DateAdded).Date == date_download)
                    .OrderBy(t => t.SamplingDate)

                    group e_d by new
                    {
                        e_d.EnumeratorNameToUse,
                        e_d.GearUsedName,
                    } into e_d2

                    select new DBSummary()
                    {
                        EnumeratorName = e_d2.First().EnumeratorNameToUse,
                        GearName = e_d2.First().GearUsedName,
                        FirstLandingFormattedDate = ((DateTime)e_d2.Min(t => t.SamplingDate)).ToString("MMM-dd-yyyy HH:mm"),
                        LastLandingFormattedDate = ((DateTime)e_d2.Max(t => t.SamplingDate)).ToString("MMM-dd-yyyy HH:mm"),
                        LatestDownloadFormattedDate = date_download.ToString("MMM-dd-yyyy"),
                        VesselUnloadCount = e_d2.Count(),
                        CountLandingsWithCatchComposition = e_d2.Count(t => t.HasCatchComposition == true),
                        TrackedOperationsCount = e_d2.Count(t => t.IsTracked == true),
                        LatestEformVersion = e_d2.Last().FormVersion
                    };


            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildStart, totalRows: v.Count());

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

                ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildFetchedRow, currentRow: count);
            }

            ProcessBuildEvent(status: BuildSummaryReportStatus.StatusBuildEnd, totalRowsFetched: count);
            return se;
        }

        public bool SetFishingGroundOfSummaryItem(int vesselUnloadId, FishingGround fg)
        {

            var si = SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == vesselUnloadId);
            if (si != null)
            {
                si.FishingGroundID = fg.Code;
                return true;
            }
            return false;
        }
        public void ResetResults()
        {
            SummaryResults = new List<SummaryResults>();
            LastPrimaryKeys.Reset();
        }
        public List<SummaryResults> SummaryResults { get; private set; }

        public void Clear()
        {
            SummaryItemCollection.Clear();
            LastPrimaryKeys.Reset();

        }
        public bool AddRecordToRepo(LandingSiteSampling ls)
        {
            SummaryItem si = new SummaryItem
            {
                ID = SummaryItemCollection.Count + 1,

                SamplingDayID = ls.PK,
                LandingSiteID = ls.LandingSiteID,
                LandingSiteText = ls.LandingSiteText,
                FMAId = ls.FMAID,
                RegionSequence = ls.NSAPRegion.Sequence,
                RegionID = ls.NSAPRegionID,
                FishingGroundID = ls.FishingGroundID,
                DateAdded = ls.DateAdded,
            };

            SummaryItemCollection.Add(si);
            return _editSuccess;

        }
        public bool AddRecordToRepo(GearUnload gu)
        {
            SummaryItem si = new SummaryItem
            {
                ID = SummaryItemCollection.Count + 1,

                SamplingDayID = gu.Parent.PK,
                LandingSiteID = gu.Parent.LandingSiteID,
                LandingSiteText = gu.Parent.LandingSiteText,
                FMAId = gu.Parent.FMAID,
                RegionSequence = gu.Parent.NSAPRegion.Sequence,
                RegionID = gu.Parent.NSAPRegionID,
                FishingGroundID = gu.Parent.FishingGroundID,
                DateAdded = gu.Parent.DateAdded,

                GearUnloadID = gu.PK,
                GearUnloadBoats = gu.Boats,
                GearUnloadCatch = gu.Catch,
                GearCode = gu.GearID,
                GearText = gu.GearUsedText,
            };

            SummaryItemCollection.Add(si);
            return _editSuccess;
        }
        public bool AddRecordToRepo(VesselUnload vu)
        {
            SummaryItem si = new SummaryItem
            {
                ID = SummaryItemCollection.Count + 1,

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
                RefNo = vu.RefNo,
                //GearName = vu.Parent.Gear.GearName,

                VesselUnloadID = vu.PK,
                XFormIdentifier = vu.XFormIdentifier,
                ODKRowID = vu.ODKRowID,
                GPSCode = vu.GPSCode,
                NumberOfFishers = vu.NumberOfFishers,
                FormVersion = vu.FormVersion,
                VesselName = vu.VesselName,
                VesselID = vu.VesselID,
                VesselText = vu.VesselText,
                HasCatchComposition = vu.HasCatchComposition,
                IsTracked = vu.OperationIsTracked,
                IsTripCompleted = vu.FishingTripIsCompleted,
                EnumeratorID = vu.NSAPEnumeratorID,
                EnumeratorText = vu.EnumeratorText,
                //EnumeratorName = vu.EnumeratorName,
                SamplingDate = vu.SamplingDate,
                IsSuccess = vu.OperationIsSuccessful,
                SectorCode = vu.SectorCode,
                DateAdded = (DateTime)vu.DateAddedToDatabase,
                DateSubmitted = (DateTime)vu.DateTimeSubmitted,
                FishingGridRows = vu.CountGrids,
                GearSoakRows = vu.CountGearSoak,
                VesselEffortRows = vu.CountEffortIndicators,
                CatchCompositionRows = vu.CountCatchCompositionItems,
                LenFreqRows = vu.CountLenFreqRows,
                LengthRows = vu.CountLengthRows,
                LenWtRows = vu.CountLenWtRows,
                CatchMaturityRows = vu.CountMaturityRows,
                LandingSiteHasOperation = vu.Parent.Parent.HasFishingOperation,
                WeightOfCatch = vu.WeightOfCatch,
                WeightOfCatchSample = vu.WeightOfCatchSample

            };
            if (vu.NSAPEnumeratorID != null)
            {
                si.EnumeratorName = vu.EnumeratorName;
            }
            if (!string.IsNullOrEmpty(vu.Parent.GearID))
            {
                si.GearName = vu.Parent.Gear.GearName;
            }



            SummaryItemCollection.Add(si);
            CurrentEntity = si;
            return _editSuccess;
        }
        public bool AddRecordToRepo(SummaryItem item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            SummaryItemCollection.Add(item);
            return _editSuccess;
        }

        public bool DeleteUsingVesselUnloadID(int vuID)
        {
            return DeleteRecordFromRepo(SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == vuID).ID);
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
        public bool DeleteOrphanedFishingGears(string gearName)
        {
            var orphanedItems = SummaryItemCollection.Where(t => string.IsNullOrEmpty(t.GearCode) && t.GearUsedName == gearName).ToList();
            int itemCount = orphanedItems.Count;
            int deletedCount = 0;
            foreach (var item in orphanedItems)
            {
                if (DeleteRecordFromRepo(item.ID))
                {
                    deletedCount++;
                }
            }
            if (deletedCount == itemCount)
            {
                _orphanedFishingGears = GetOrphanedFishingGears();
                return true;
            }
            else
            {
                return false;
            }
        }
        public Task<bool> DeleteOrphanedLandingSiteAsync(string landingSiteName)
        {
            return Task.Run(() => DeleteOrphanedLandingSite(landingSiteName));
        }
        public bool DeleteOrphanedLandingSite(string landingSiteName)
        {
            var orphanedItems = SummaryItemCollection.Where(t => t.LandingSiteID == null && t.LandingSiteNameText == landingSiteName).ToList();
            int itemCount = orphanedItems.Count;
            int deletedCount = 0;
            foreach (var item in orphanedItems)
            {
                if (DeleteRecordFromRepo(item.ID))
                {
                    deletedCount++;
                }
            }
            if (deletedCount == itemCount)
            {
                //_orphanedLandingSites = GetOrphanedLandingSites();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteOrphanedEnumeratorItems(string enumeratorName)
        {
            var orphanedItems = SummaryItemCollection.Where(t => t.EnumeratorID == null && t.EnumeratorNameToUse == enumeratorName).ToList();
            int itemCount = orphanedItems.Count;
            int deletedCount = 0;
            foreach (var item in orphanedItems)
            {
                if (DeleteRecordFromRepo(item.ID))
                {
                    deletedCount++;
                }
            }
            if (deletedCount == itemCount)
            {
                _orphanedEnumerators = GetOrphanedEnumerators();
                return true;
            }
            else
            {
                return false;
            }
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
