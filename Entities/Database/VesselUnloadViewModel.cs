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
        public bool AddSucceeded;
        public ObservableCollection<VesselUnload> VesselUnloadCollection { get; set; }
        private VesselUnloadRepository VesselUnloads { get; set; }

        
        public VesselUnloadViewModel()
        {
            VesselUnloads = new VesselUnloadRepository();
            VesselUnloadCollection = new ObservableCollection<VesselUnload>(VesselUnloads.VesselUnloads);
            VesselUnloadCollection.CollectionChanged += VesselUnloadCollection_CollectionChanged;
        }

        public List<VesselUnload> GetAllVesselUnloads()
        {
            return VesselUnloadCollection.ToList();
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
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        AddSucceeded = VesselUnloads.Add(VesselUnloadCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselUnload> tempListOfRemovedItems = e.OldItems.OfType<VesselUnload>().ToList();
                        VesselUnloads.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselUnload> tempList = e.NewItems.OfType<VesselUnload>().ToList();
                        VesselUnloads.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
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
            return AddSucceeded;
        }

        public void UpdateRecordInRepo(VesselUnload item)
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
                    return VesselUnloads.MaxRecordNumber()  + 1;
                }
            }
        }

        public void DeleteRecordFromRepo(int id)
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
        }
    }
}
