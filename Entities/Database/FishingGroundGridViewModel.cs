using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class FishingGroundGridViewModel : IDisposable
    {
        public bool _editSuccess;
        public ObservableCollection<FishingGroundGrid> FishingGroundGridCollection { get; set; }
        private FishingGroundGridRepository FishingGroundGrids { get; set; }

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
                FishingGroundGridCollection.Clear();
                FishingGroundGridCollection = null;
                FishingGroundGrids = null;

            }
            // free native resources if there are any.
        }
        public FishingGroundGridViewModel(VesselUnload parent)
        {
            FishingGroundGrids = new FishingGroundGridRepository(parent);
            FishingGroundGridCollection = new ObservableCollection<FishingGroundGrid>(FishingGroundGrids.FishingGroundGrids);
            FishingGroundGridCollection.CollectionChanged += FishingGroundGridCollection_CollectionChanged;
        }
        public FishingGroundGridViewModel(bool isNew = false)
        {
            FishingGroundGrids = new FishingGroundGridRepository(isNew);
            if (isNew)
            {
                FishingGroundGridCollection = new ObservableCollection<FishingGroundGrid>();
            }
            else
            {
                FishingGroundGridCollection = new ObservableCollection<FishingGroundGrid>(FishingGroundGrids.FishingGroundGrids);
            }
            FishingGroundGridCollection.CollectionChanged += FishingGroundGridCollection_CollectionChanged;
        }

        public List<FishingGroundGrid> GetAllFishingGroundGrids()
        {
            return FishingGroundGridCollection.ToList();
        }

        public List<FishingGroundGridFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<FishingGroundGridFlattened> thisList = new List<FishingGroundGridFlattened>();
            if (tracked)
            {
                foreach (var item in FishingGroundGridCollection
                    .Where(t => t.Parent.OperationIsTracked == tracked))
                {
                    thisList.Add(new FishingGroundGridFlattened(item));
                }
            }
            else
            {
                foreach (var item in FishingGroundGridCollection)
                {
                    thisList.Add(new FishingGroundGridFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            FishingGroundGridCollection.Clear();
            return FishingGroundGridRepository.ClearTable();
        }

        public FishingGroundGrid getFishingGroundGrid(int pk)
        {
            return FishingGroundGridCollection.FirstOrDefault(n => n.PK == pk);
        }

        private bool SetCSV(FishingGroundGrid item)
        {
            _csv.AppendLine($"{item.PK},{item.Parent.PK},\"{item.UTMZone}\",\"{item.Grid}\"");
            return true;
        }
        public static string CSV
        {
            get { return $"{AccessHelper.GetColumnNamesCSV("dbo_fg_grid")}\r\n{_csv}"; }
        }

        private void FishingGroundGridCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        FishingGroundGrid newItem = FishingGroundGridCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = FishingGroundGrids.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = FishingGroundGrids.Add(FishingGroundGridCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FishingGroundGrid> tempListOfRemovedItems = e.OldItems.OfType<FishingGroundGrid>().ToList();
                        _editSuccess = FishingGroundGrids.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FishingGroundGrid> tempList = e.NewItems.OfType<FishingGroundGrid>().ToList();
                        _editSuccess = FishingGroundGrids.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public static void ClearCSV()
        {
            _csv.Clear();
        }

        public int Count
        {
            get { return FishingGroundGridCollection.Count; }
        }

        public bool AddRecordToRepo(FishingGroundGrid item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FishingGroundGridCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(FishingGroundGrid item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < FishingGroundGridCollection.Count)
            {
                if (FishingGroundGridCollection[index].PK == item.PK)
                {
                    FishingGroundGridCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public int NextRecordNumber
        {
            get
            {
                if (FishingGroundGridCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return FishingGroundGrids.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < FishingGroundGridCollection.Count)
            {
                if (FishingGroundGridCollection[index].PK == id)
                {
                    FishingGroundGridCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
    }
}
