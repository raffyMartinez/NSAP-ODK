using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class FishingGroundGridViewModel
    {
        public bool EditSucceeded;
        public ObservableCollection<FishingGroundGrid> FishingGroundGridCollection { get; set; }
        private FishingGroundGridRepository FishingGroundGrids { get; set; }

        public FishingGroundGridViewModel()
        {
            FishingGroundGrids = new FishingGroundGridRepository();
            FishingGroundGridCollection = new ObservableCollection<FishingGroundGrid>(FishingGroundGrids.FishingGroundGrids);
            FishingGroundGridCollection.CollectionChanged += FishingGroundGridCollection_CollectionChanged;
        }

        public List<FishingGroundGrid> GetAllFishingGroundGrids()
        {
            return FishingGroundGridCollection.ToList();
        }

        public List<FishingGroundGridFlattened> GetAllFlattenedItems(bool tracked=false)
        {
            List<FishingGroundGridFlattened> thisList = new List<FishingGroundGridFlattened>();
            if (tracked)
            {
                foreach (var item in FishingGroundGridCollection
                    .Where(t=>t.Parent.OperationIsTracked==tracked))
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
            return FishingGroundGrids.ClearTable();
        }

        public FishingGroundGrid getFishingGroundGrid(int pk)
        {
            return FishingGroundGridCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void FishingGroundGridCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        EditSucceeded = FishingGroundGrids.Add(FishingGroundGridCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FishingGroundGrid> tempListOfRemovedItems = e.OldItems.OfType<FishingGroundGrid>().ToList();
                        EditSucceeded=FishingGroundGrids.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FishingGroundGrid> tempList = e.NewItems.OfType<FishingGroundGrid>().ToList();
                        EditSucceeded=FishingGroundGrids.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return FishingGroundGridCollection.Count; }
        }

        public bool AddRecordToRepo(FishingGroundGrid item)
        {
            EditSucceeded = false;
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FishingGroundGridCollection.Add(item);
            return EditSucceeded;
        }

        public bool UpdateRecordInRepo(FishingGroundGrid item)
        {
            EditSucceeded = false;
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
            return EditSucceeded;
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
            EditSucceeded = false;
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
            return EditSucceeded;
        }
    }
}
