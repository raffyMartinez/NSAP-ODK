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
        public event EventHandler ColumnUpdatedEvent;
        public bool EditSucceeded;
        public ObservableCollection<VesselUnload> VesselUnloadCollection { get; set; }
        private VesselUnloadRepository VesselUnloads { get; set; }


        //private Dictionary<NSAPRegion,int>CountByRegion
        //{
        //    get
        //    {
        //        NSAPEntities.NSAPRegionViewModel.GetEnumeratorInRegion
        //    }
        //}
        private void ManageUpdateColumnEvent(string intent, int? round = null, int? runningCount = null, int? rowsForUpdating = null)
        {
            EventHandler h = ColumnUpdatedEvent;
            if (h != null)
            {
                switch (intent)
                {
                    case "start":
                        var ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            Round = (int)round,
                            RowsToUpdate = (int)rowsForUpdating
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
            return Task.Run(() => UpdateHasCatchCompositionColumns(updateItems,round));
        }
        public int UpdateHasCatchCompositionColumns(List<UpdateHasCatchCompositionResultItem> updateItems, int round)
        {
            ManageUpdateColumnEvent(intent: "start", round: round, rowsForUpdating: updateItems.Count);
            int results = 0;
            foreach (var item in updateItems)
            {
                VesselUnloads.UpdateHasCatchCompositionColumn(item);
                results++;
                ManageUpdateColumnEvent(intent: "row updated", runningCount: results);
            }
            ManageUpdateColumnEvent(intent: "finished");
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
        public List<VesselUnload> GetAllVesselUnloads(NSAPRegion region)
        {
            return VesselUnloadCollection
                .Where(t => t.Parent.Parent.NSAPRegionID == region.Code)
                .OrderBy(t => t.SamplingDate)
                .ToList();
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
        public VesselUnloadViewModel()
        {
            VesselUnloads = new VesselUnloadRepository();
            VesselUnloadCollection = new ObservableCollection<VesselUnload>(VesselUnloads.VesselUnloads);
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
            return VesselUnloads.ClearTable();
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
            return VesselUnloadCollection.FirstOrDefault(n => n.PK == pk);
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
                        EditSucceeded = VesselUnloads.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
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

        public bool UpdateRecordInRepo(VesselUnload item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < VesselUnloadCollection.Count)
            {
                if (VesselUnloadCollection[index].PK == item.PK)
                {
                    VesselUnloadCollection[index] = item;
                    break;
                }
                index++;
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
