using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselEffortViewModel : IDisposable
    {
        public bool EditSucceeded { get; set; }
        public ObservableCollection<VesselEffort> VesselEffortCollection { get; set; }
        private VesselEffortRepository VesselEfforts { get; set; }
        private static StringBuilder _csv = new StringBuilder();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public static int CurrentIDNumber { get; set; }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                VesselEffortCollection.Clear();
                VesselEffortCollection = null;
                VesselEfforts = null;

            }
            // free native resources if there are any.
        }
        public VesselEffortViewModel(VesselUnload vu)
        {
            VesselEfforts = new VesselEffortRepository(vu);
            VesselEffortCollection = new ObservableCollection<VesselEffort>(VesselEfforts.VesselEfforts);
            VesselEffortCollection.CollectionChanged += LandingSiteSamplingCollection_CollectionChanged;
        }
        public VesselEffortViewModel(bool isNew = false)
        {
            VesselEfforts = new VesselEffortRepository(isNew);
            if (isNew)
            {
                VesselEffortCollection = new ObservableCollection<VesselEffort>();
            }
            else
            {
                VesselEffortCollection = new ObservableCollection<VesselEffort>(VesselEfforts.VesselEfforts);
            }
            VesselEffortCollection.CollectionChanged += LandingSiteSamplingCollection_CollectionChanged;
        }
        public bool ClearRepository()
        {
            VesselEffortCollection.Clear();
            return VesselEffortRepository.ClearTable();
        }

        public List<VesselEffortFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<VesselEffortFlattened> thisList = new List<VesselEffortFlattened>();
            if (tracked)
            {
                foreach (var item in VesselEffortCollection
                    .Where(t => t.Parent.OperationIsTracked == tracked))
                {
                    thisList.Add(new VesselEffortFlattened(item));
                }
            }
            else
            {
                foreach (var item in VesselEffortCollection)
                {
                    thisList.Add(new VesselEffortFlattened(item));
                }
            }
            return thisList;
        }
        public List<VesselEffort> GetAllVesselEfforts()
        {
            return VesselEffortCollection.ToList();
        }

        //public List<VesselEffort> getLandingSiteSamplings(LandingSite ls, FishingGround fg, DateTime samplingDate)
        //{
        //    return VesselEffortCollection
        //        .Where(t => t.LandingSiteID == ls.LandingSiteID)
        //        .Where(t => t.FishingGroundID == fg.Code)
        //        .Where(t => t.SamplingDate == samplingDate).ToList();
        //}

        public VesselEffort getVesselEffort(ExcelEffortRepeat sheet)
        {
            return VesselEffortCollection
                .Where(t => t.EffortSpecID == sheet.EffortTypeID)
                .Where(t => t.Parent.FishingVessel.ID == sheet.Parent.FishingVessel.ID).FirstOrDefault();
        }


        public VesselEffort getVesselEffort(int pk)
        {
            return VesselEffortCollection.FirstOrDefault(n => n.PK == pk);
        }

        private static bool SetCSV(VesselEffort item)
        {
            _csv.AppendLine($"{item.PK},{item.Parent.PK},{item.EffortSpecID},{item.EffortValueNumeric},\"{item.EffortValueText}\"");
            return true;
        }

        public static string CSV
        {
            get { return $"{AccessHelper.GetColumnNamesCSV("dbo_vessel_effort")}\r\n{_csv.ToString()}"; }
        }

        private void LandingSiteSamplingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSucceeded = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        VesselEffort newItem = VesselEffortCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            EditSucceeded = SetCSV(newItem);
                        }
                        else
                        {
                            EditSucceeded = VesselEfforts.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //EditSucceeded = VesselEfforts.Add(VesselEffortCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselEffort> tempListOfRemovedItems = e.OldItems.OfType<VesselEffort>().ToList();
                        EditSucceeded = VesselEfforts.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselEffort> tempList = e.NewItems.OfType<VesselEffort>().ToList();
                        EditSucceeded = VesselEfforts.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return VesselEffortCollection.Count; }
        }

        public bool AddRecordToRepo(VesselEffort item)
        {
            EditSucceeded = false;
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselEffortCollection.Add(item);
            return EditSucceeded;
        }

        public bool UpdateRecordInRepo(VesselEffort item)
        {
            EditSucceeded = false;
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < VesselEffortCollection.Count)
            {
                if (VesselEffortCollection[index].PK == item.PK)
                {
                    VesselEffortCollection[index] = item;
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
                if (VesselEffortCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return VesselEfforts.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            EditSucceeded = false;
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselEffortCollection.Count)
            {
                if (VesselEffortCollection[index].PK == id)
                {
                    VesselEffortCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return EditSucceeded;
        }
    }
}
