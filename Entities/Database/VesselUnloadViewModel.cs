using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselUnloadViewModel
    {
        public event EventHandler DatabaseUpdatedEvent;
        public bool EditSucceeded;
        private bool _updateXFormID;

        public int CountLandingWithCatchComposition()
        {
            return VesselUnloadCollection.Count(t => t.HasCatchComposition == true);
        }
        public ObservableCollection<VesselUnload> VesselUnloadCollection { get; set; }
        private VesselUnloadRepository VesselUnloads { get; set; }


        //private Dictionary<NSAPRegion,int>CountByRegion
        //{
        //    get
        //    {
        //        NSAPEntities.NSAPRegionViewModel.GetEnumeratorInRegion
        //    }
        //}

        public bool UpdateUnloadStats(VesselUnload vu)
        {
            return VesselUnloads.AddUnloadStats(vu);
        }

        public Task<int> UpdateUnloadStatsAsync()
        {
            return Task.Run(() => UpdateUnloadStats());
        }

        public List<Download_summary> GetDownlodaSummary(List<SummaryItem> downloadedItems, DateTime downloadDate)
        {
            List<Download_summary> dws = new List<Download_summary>();
            var enumerators = downloadedItems.GroupBy(t => t.EnumeratorName).OrderBy(t => t.Key);
            int n = 0;


            foreach (var en in enumerators)
            {

                var enDownloads = en.ToList();

                var gears = enDownloads.ToList().GroupBy(t => t.GearUsedName).OrderBy(t => t.Key);
                foreach (var g in gears)
                {
                    var gl = g.ToList();
                    Download_summary ds = new Download_summary
                    {
                        Enumerator = enDownloads[0].EnumeratorName,
                        Gear = gl[0].GearUsedName,
                        NumberLandings = gl.Count,
                        NumberLandingsWithCatchComposition = gl.Count(t => t.HasCatchComposition == true),
                        EarliestSamplingDate = gl.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        LatestSamplingDate = gl.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DownloadDate = downloadDate,
                        NumberOfTrackedLandings = gl.Count(t => t.IsTracked == true),
                    };
                    dws.Add(ds);
                }



                n++;
            }

            var sorted = dws.OrderBy(t => t.Enumerator).ToList();

            Download_summary s = new Download_summary
            {
                Enumerator = "Grand total",
                NumberLandings = downloadedItems.Count,
                NumberLandingsWithCatchComposition = downloadedItems.Count(t => t.HasCatchComposition == true),
                NumberOfTrackedLandings = downloadedItems.Count(t => t.IsTracked == true)
            };
            sorted.Add(s);
            return sorted;
        }
        public List<Download_summary> GetDownlodaSummary1(List<VesselUnload> downloadedItems, DateTime downloadDate)
        {
            List<Download_summary> dws = new List<Download_summary>();
            var enumerators = downloadedItems.GroupBy(t => t.EnumeratorName).OrderBy(t => t.Key);
            int n = 0;


            foreach (var en in enumerators)
            {

                var enDownloads = en.ToList();

                var gears = enDownloads.ToList().GroupBy(t => t.Parent.GearUsedName).OrderBy(t => t.Key);
                foreach (var g in gears)
                {
                    var gl = g.ToList();
                    Download_summary ds = new Download_summary
                    {
                        Enumerator = enDownloads[0].EnumeratorName,
                        Gear = gl[0].Parent.GearUsedName,
                        NumberLandings = gl.Count,
                        NumberLandingsWithCatchComposition = gl.Count(t => t.HasCatchComposition == true),
                        EarliestSamplingDate = gl.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        LatestSamplingDate = gl.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DownloadDate = downloadDate,
                        NumberOfTrackedLandings = gl.Count(t => t.OperationIsTracked == true),
                    };
                    dws.Add(ds);
                }



                n++;
            }

            var sorted = dws.OrderBy(t => t.Enumerator).ToList();

            Download_summary s = new Download_summary
            {
                Enumerator = "Grand total",
                NumberLandings = downloadedItems.Count,
                NumberLandingsWithCatchComposition = downloadedItems.Count(t => t.HasCatchComposition == true),
                NumberOfTrackedLandings = downloadedItems.Count(t => t.OperationIsTracked == true)
            };
            sorted.Add(s);
            return sorted;
        }

        //public static VesselUnloadSummary GetSummary()
        //{
        //    return VesselUnloadRepository.GetSummary();
        //}
        public static int CountVesselUnload(bool isTracked = false)
        {
            return VesselUnloadRepository.VesselUnloadCount(isTracked);
        }

        private int UpdateUnloadStats()
        {
            int result = 0;
            ManageUpdateEvent(intent: "start", rowsForUpdating: VesselUnloadCollection.Count);
            foreach (var item in VesselUnloadCollection)
            {
                item.CountGrids = NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count(t => t.Parent.PK == item.PK);
                item.CountGearSoak = NSAPEntities.GearSoakViewModel.GearSoakCollection.Count(t => t.Parent.PK == item.PK);
                item.CountEffortIndicators = NSAPEntities.VesselEffortViewModel.VesselEffortCollection.Count(t => t.Parent.PK == item.PK);
                if (item.HasCatchComposition)
                {
                    item.CountCatchCompositionItems = NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Count(t => t.Parent.PK == item.PK);
                    foreach (var c in NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Where(t => t.Parent.PK == item.PK))
                    {
                        item.CountLenFreqRows += c.ListCatchLenFreq.Count;
                        item.CountLenWtRows += c.ListCatchLengthWeight.Count;
                        item.CountLengthRows += c.ListCatchLength.Count;
                        item.CountMaturityRows += c.ListCatchMaturity.Count;
                    }
                }
                if (result == 1)
                {
                    ManageUpdateEvent(intent: "start updating");
                }

                if (VesselUnloads.AddUnloadStats(item))
                {
                    result++;
                    ManageUpdateEvent(intent: "row updated", runningCount: result);
                }
            }
            ManageUpdateEvent(intent: "finished");
            return result;
        }
        private void ManageUpdateEvent(string intent, int? round = null, int? runningCount = null, int? rowsForUpdating = null)
        {
            EventHandler h = DatabaseUpdatedEvent;
            if (h != null)
            {
                switch (intent)
                {
                    case "start":
                        var ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            RowsToUpdate = (int)rowsForUpdating
                        };
                        if (round != null)
                        {
                            ev.Round = (int)round;
                        }

                        h(null, ev);
                        break;
                    case "start updating":
                        ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent
                        };
                        h(null, ev);
                        break;
                    case "row updated":
                        ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            RunningCount = (int)runningCount
                        };
                        h(null, ev);
                        break;
                    case "finished":
                        h(null, new UpdateDatabaseColumnEventArg { Intent = intent });
                        break;
                }

            }
        }
        public Task<int> UpdateHasCatchCompositionColumnsAsync(List<UpdateHasCatchCompositionResultItem> updateItems, int round)
        {
            return Task.Run(() => UpdateHasCatchCompositionColumns(updateItems, round));
        }

        public Task<int> UpdateXFormIdentifierColumnAsync(List<UpdateXFormIdentifierItem> updateItems, int round)
        {
            return Task.Run(() => UpdateXFormIdentifierColumn(updateItems, round));
        }
        private int UpdateXFormIdentifierColumn(List<UpdateXFormIdentifierItem> updateItems, int round)
        {
            ManageUpdateEvent(intent: "start", round: round, rowsForUpdating: updateItems.Count);
            int results = 0;
            foreach (var item in updateItems)
            {
                if (VesselUnloads.UpdateXFormIdentifierColumn(item))
                {
                    results++;
                    ManageUpdateEvent(intent: "row updated", runningCount: results);
                }
            }
            ManageUpdateEvent(intent: "finished");
            return results;
        }
        private int UpdateHasCatchCompositionColumns(List<UpdateHasCatchCompositionResultItem> updateItems, int round)
        {
            ManageUpdateEvent(intent: "start", round: round, rowsForUpdating: updateItems.Count);
            int results = 0;
            foreach (var item in updateItems)
            {
                VesselUnloads.UpdateHasCatchCompositionColumn(item);
                results++;
                ManageUpdateEvent(intent: "row updated", runningCount: results);
            }
            ManageUpdateEvent(intent: "finished");
            return results;
        }

        public int UpdateHasCatchCompositionColumns()
        {
            int results = 0;
            foreach (var vl in VesselUnloadCollection)
            {
                VesselUnloads.UpdateHasCatchCompositionColumn(vl.ListVesselCatch.Count > 0, vl.PK);
                results++;
            }
            return results;
        }
        public bool HasBSCInCatchComposition(VesselUnload vu)
        {
            return vu.ListVesselCatch.Where(t => t.CatchName == "Portunus pelagicus").FirstOrDefault() != null;
        }
        public List<DateTime> MonthsSampledByEnumerator(NSAPEnumerator enumerator)
        {
            List<DateTime> list = new List<DateTime>();
            foreach (var item in VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID != null && t.NSAPEnumerator.ID == enumerator.ID)
                .GroupBy(t => t.MonthSampled)
                .OrderBy(t => t.Key)
                .ToList())
            {
                list.Add(item.Key);
            }
            return list;
        }
        public List<VesselUnload> GetAllVesselUnloadsWithDate(string region, string fishingGround = "", string landingSite = "",
             string enumerator = "", string gear = "", DateTime? dateUploaded = null)
        {
            List<VesselUnload> list = new List<VesselUnload>();

            if (landingSite.Length > 0 && gear.Length > 0 && dateUploaded != null && enumerator.Length > 0)
            {
                list = VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region &&
                    t.Parent.Parent.LandingSiteName == landingSite &&
                    t.Parent.GearUsedName == gear &&
                    t.EnumeratorName == enumerator &&
                    ((DateTime)t.DateAddedToDatabase).Date == ((DateTime)dateUploaded).Date).ToList();
            }
            return list;
        }
        public List<VesselUnload> GetAllVesselUnloads(string region, string fishingGround = "", string landingSite = "")
        {
            if (fishingGround.Length > 0 && landingSite.Length > 0)
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region &&
                    t.Parent.Parent.FishingGround.Name == fishingGround &&
                    t.Parent.Parent.LandingSiteName == landingSite).ToList();
            }
            else if (fishingGround.Length > 0)
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region &&
                    t.Parent.Parent.FishingGround.Name == fishingGround).ToList();
            }
            else
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region).ToList();
            }
        }
        public List<VesselUnload> GetAllVesselUnloads(NSAPRegion region, bool sorted = true)
        {
            if (sorted)
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegionID == region.Code)
                    .OrderBy(t => t.SamplingDate)
                    .ToList();
            }
            else
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegionID == region.Code)
                    .ToList();
            }
        }

        public List<VesselUnload> GetAllVesselUnloads(NSAPEnumerator enumerator)
        {
            return VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID == enumerator.ID)
                .OrderBy(t => t.SamplingDate)
                .ToList();
        }
        public List<VesselUnload> GetAllVesselUnloads(NSAPEnumerator enumerator, DateTime month)
        {
            return VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID == enumerator.ID && t.MonthSampled == month)
                .OrderBy(t => t.SamplingDate)
                .ToList();
        }
        public int CountEnumeratorsWithUnloadRecord
        {
            get
            {
                return VesselUnloadCollection.Where(t => t.NSAPEnumeratorID != null).GroupBy(t => t.NSAPEnumeratorID).ToList().Count;
            }
        }

        public static List<VesselUnload> GetVesselUnloads(List<GearUnload> items)
        {
            List<VesselUnload> vessel_unloads = new List<VesselUnload>();
            foreach (GearUnload item in items)
            {
                item.VesselUnloadViewModel = new VesselUnloadViewModel(item, updatesubViewModels: true);
                vessel_unloads.AddRange(item.VesselUnloadViewModel.VesselUnloadCollection.ToList());
            }
            return vessel_unloads;
        }
        public VesselUnloadViewModel(GearUnload parent, bool updatesubViewModels = false)
        {
            VesselUnloads = new VesselUnloadRepository(parent);
            VesselUnloadCollection = new ObservableCollection<VesselUnload>(VesselUnloads.VesselUnloads);
            if (updatesubViewModels)
            {
                foreach (VesselUnload vu in VesselUnloadCollection)
                {
                    if (vu.FishingGroundGridViewModel == null)
                    {
                        vu.FishingGroundGridViewModel = new FishingGroundGridViewModel(vu);
                    }

                    if (vu.VesselCatchViewModel == null)
                    {
                        vu.VesselCatchViewModel = new VesselCatchViewModel(vu);
                    }

                    if (vu.VesselEffortViewModel == null)
                    {
                        vu.VesselEffortViewModel = new VesselEffortViewModel(vu);
                    }

                    if (vu.GearSoakViewModel == null)
                    {
                        vu.GearSoakViewModel = new GearSoakViewModel(vu);
                    }
                }
            }
            VesselUnloadCollection.CollectionChanged += VesselUnloadCollection_CollectionChanged;
        }
        public VesselUnloadViewModel(bool isNew = false)
        {
            VesselUnloads = new VesselUnloadRepository(isNew);
            if (isNew)
            {
                VesselUnloadCollection = new ObservableCollection<VesselUnload>();
            }
            else
            {
                VesselUnloadCollection = new ObservableCollection<VesselUnload>(VesselUnloads.VesselUnloads);
            }
            VesselUnloadCollection.CollectionChanged += VesselUnloadCollection_CollectionChanged;
        }

        public List<VesselUnload> GetSampledLandingsOfVessel(string veselName, string sector, string landingSiteName)
        {
            return VesselUnloadCollection
                .Where(t => t.VesselID == null && t.VesselName == veselName && t.Sector == sector && t.Parent.Parent.LandingSiteName == landingSiteName)
                .ToList();
        }


        public List<VesselUnload> GetUnloadsPastDateUploadLocalDB(DateTime dateUpload)
        {
            return VesselUnloadCollection.Where(t => t.DateAddedToDatabase > dateUpload).ToList();
        }

        public Task<DeleteVesselUnloaResult> DeleteUnloadChildrenAsync(List<VesselUnload> listUnload)
        {
            return Task.Run(() => DeleteUnloadChildren(listUnload));
        }

        private int _countUnloadDeleted;
        public event EventHandler DeleteUnloadChildrenEvent;
        public DeleteVesselUnloaResult DeleteUnloadChildren(List<VesselUnload> listUnload)
        {
            int counter = 0;
            int countUnloadDeleted = 0;
            List<int> pks = new List<int>();

            //int counter = NSAPEntities.VesselCatchViewModel.DeleteCatchFromUnloads(listUnload);

            foreach (var item in listUnload)
            {
                foreach (var soakItem in item.ListGearSoak)
                {
                    if (NSAPEntities.GearSoakViewModel.DeleteRecordFromRepo(soakItem.PK))
                    {
                        counter++;
                    }
                }


                foreach (var gridItem in item.ListFishingGroundGrid)
                {
                    if (NSAPEntities.FishingGroundGridViewModel.DeleteRecordFromRepo(gridItem.PK))
                    {
                        counter++;
                    }
                }

                foreach (var effortItem in item.ListVesselEffort)
                {
                    if (NSAPEntities.VesselEffortViewModel.DeleteRecordFromRepo(effortItem.PK))
                    {
                        counter++;
                    }
                }

                NSAPEntities.VesselCatchViewModel.DeleteCatchFromUnload(item);

                if (DeleteRecordFromRepo(item.PK))
                {
                    countUnloadDeleted++;
                    DeleteUnloadChildrenEvent?.Invoke(this, null);
                }

            }

            return new DeleteVesselUnloaResult { CountDeleted = counter, VesselUnloadToDeleteCoount = countUnloadDeleted };

        }
        public List<VesselUnload> GetSampledLandings(string enumeratorName, string landingSiteName)
        {
            return VesselUnloadCollection.Where(t => t.NSAPEnumeratorID == null && t.EnumeratorName == enumeratorName && t.Parent.Parent.LandingSiteName == landingSiteName).ToList();
        }
        public List<VesselUnload> GetSampledLandings(string enumeratorName)
        {
            return VesselUnloadCollection.Where(t => t.NSAPEnumeratorID == null && t.EnumeratorName == enumeratorName).ToList();
        }
        public List<LandingSiteSampling> GetLandingSiteSamplings(string enumeratorName)
        {
            var list = new List<LandingSiteSampling>();
            foreach (var item in VesselUnloadCollection.Where(t => t.EnumeratorName == enumeratorName && t.NSAPEnumeratorID == null))
            {
                if (!list.Contains(item.Parent.Parent))
                {
                    list.Add(item.Parent.Parent);
                }
            }

            return list;
        }
        public List<VesselUnload> GetAllVesselUnloads()
        {
            return VesselUnloadCollection.ToList();
        }


        public List<VesselUnload> GetAllVesselUnloads(GearUnload parent, bool includeSimilarDateAndGear = false)
        {
            if (includeSimilarDateAndGear)
            {
                return VesselUnloadCollection.Where(t => t.SamplingDate.Date == parent.Parent.SamplingDate.Date &&
                t.Parent.GearUsedName == parent.GearUsedName &&
                t.Parent.Parent.LandingSiteName == parent.Parent.LandingSiteName).ToList();
            }
            else
            {
                return VesselUnloadCollection.Where(t => t.Parent.PK == parent.PK).ToList();
            }
        }

        public DateTime DateLatestDownload
        {
            get { return (DateTime)VesselUnloadCollection.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase; }
        }
        public List<VesselUnloadTrackedFlattened> GetAllTrackedFlattenedItems()
        {
            return VesselUnloads.getTrackedFlattenedList();
        }

        public string GetSector(string sectorCode)
        {
            return sectorCode == "c" ? "Commercial" : "Municipal";
        }

        public List<VesselUnloadFlattened> GetAllFlattenedItems()
        {
            List<VesselUnloadFlattened> thisList = new List<VesselUnloadFlattened>();
            foreach (var item in VesselUnloadCollection)
            {
                thisList.Add(new VesselUnloadFlattened(item));
            }
            return thisList;
        }

        public bool ClearRepository()
        {
            VesselUnloadCollection.Clear();
            return VesselUnloadRepository.ClearTable();
        }

        public DateTime DateOfFirstSampledLanding
        {
            get { return VesselUnloadCollection.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate; }
        }

        public DateTime DateOfLastSampledLanding
        {
            get { return VesselUnloadCollection.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate; }
        }

        public int TrackedUnloadCount
        {
            get { return VesselUnloadCollection.Where(t => t.OperationIsTracked == true).ToList().Count; }
        }

        public VesselUnload getVesselUnload(int pk)
        {
            var vu = VesselUnloadCollection.FirstOrDefault(n => n.PK == pk);
            vu.ContainerViewModel = this;
            return vu;
        }

        public VesselUnload getVesselUnload(string odkROWID)
        {
            var vu = VesselUnloadCollection.FirstOrDefault(n => n.ODKRowID == odkROWID);
            if (vu != null)
            {
                vu.ContainerViewModel = this;
            }
            return vu;
        }
        private void VesselUnloadCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSucceeded = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        EditSucceeded = VesselUnloads.Add(VesselUnloadCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselUnload> tempListOfRemovedItems = e.OldItems.OfType<VesselUnload>().ToList();
                        EditSucceeded = VesselUnloads.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselUnload> tempList = e.NewItems.OfType<VesselUnload>().ToList();
                        if (_updateXFormID)
                        {
                            EditSucceeded = VesselUnloads.UpdateEx(tempList[0]);
                        }
                        else
                        {
                            EditSucceeded = VesselUnloads.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                        }
                    }
                    break;
            }
        }

        public int Count
        {
            get { return VesselUnloadCollection.Count; }
        }

        public FishingGroundGrid FirstGridLocation(VesselUnload unload)
        {
            FishingGroundGrid grid = null;
            if (unload.ListFishingGroundGrid.Count > 0)
            {
                grid = unload.ListFishingGroundGrid[0];
            }
            return grid;
        }


        public bool AddRecordToRepo(VesselUnload item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselUnloadCollection.Add(item);
            return EditSucceeded;
        }

        public bool UpdateRecordInRepo(VesselUnload item, bool updateXFormID = false)
        {
            _updateXFormID = updateXFormID;
            int index = 0;
            if (_updateXFormID)
            {
                while (index < VesselUnloadCollection.Count)
                {
                    if (VesselUnloadCollection[index].ODKRowID == item.ODKRowID)
                    {
                        VesselUnloadCollection[index] = item;
                        break;
                    }
                    index++;
                }
            }
            else
            {
                if (item.PK == 0)
                    throw new Exception("Error: ID cannot be zero");


                while (index < VesselUnloadCollection.Count)
                {
                    if (VesselUnloadCollection[index].PK == item.PK)
                    {
                        VesselUnloadCollection[index] = item;
                        break;
                    }
                    index++;
                }
            }

            return EditSucceeded;
        }

        public int NextRecordNumber
        {
            get
            {
                if (VesselUnloadCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return VesselUnloads.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselUnloadCollection.Count)
            {
                if (VesselUnloadCollection[index].PK == id)
                {
                    VesselUnloadCollection.RemoveAt(index);
                    break;
                }
                index++;
            }

            return EditSucceeded;
        }
    }
}
