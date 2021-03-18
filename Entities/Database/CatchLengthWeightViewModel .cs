using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthWeightViewModel
    {
        public bool EditSucceeded;
        public ObservableCollection<CatchLengthWeight> CatchLengthWeightCollection { get; set; }
        private CatchLenWeightRepository CatchLengthWeights { get; set; }

        public CatchLengthWeightViewModel()
        {
            CatchLengthWeights = new CatchLenWeightRepository();
            CatchLengthWeightCollection = new ObservableCollection<CatchLengthWeight>(CatchLengthWeights.CatchLengthWeights);
            CatchLengthWeightCollection.CollectionChanged += CatchLengthWeightCollection_CollectionChanged;
        }

        public List<CatchLengthWeight> GetAllCatchLengthWeights()
        {
            return CatchLengthWeightCollection.ToList();
        }

        public List<CatchLengthWeightFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<CatchLengthWeightFlattened> thisList = new List<CatchLengthWeightFlattened>();
            if (tracked)
            {
                foreach (var item in CatchLengthWeightCollection.
                    Where(t => t.Parent.Parent.OperationIsTracked == tracked))
                {
                    thisList.Add(new CatchLengthWeightFlattened(item));
                }
            }
            else
            {
                foreach (var item in CatchLengthWeightCollection)
                {
                    thisList.Add(new CatchLengthWeightFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            CatchLengthWeightCollection.Clear();
            return CatchLengthWeights.ClearTable();
        }

        public CatchLengthWeight getCatchLenghtWeight(int pk)
        {
            return CatchLengthWeightCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void CatchLengthWeightCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        EditSucceeded = CatchLengthWeights.Add(CatchLengthWeightCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchLengthWeight> tempListOfRemovedItems = e.OldItems.OfType<CatchLengthWeight>().ToList();
                        EditSucceeded = CatchLengthWeights.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchLengthWeight> tempList = e.NewItems.OfType<CatchLengthWeight>().ToList();
                        EditSucceeded = CatchLengthWeights.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return CatchLengthWeightCollection.Count; }
        }

        public bool AddRecordToRepo(CatchLengthWeight item)
        {
            EditSucceeded = false;
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CatchLengthWeightCollection.Add(item);
            return EditSucceeded;
        }

        public bool UpdateRecordInRepo(CatchLengthWeight item)
        {
            EditSucceeded = false;
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CatchLengthWeightCollection.Count)
            {
                if (CatchLengthWeightCollection[index].PK == item.PK)
                {
                    CatchLengthWeightCollection[index] = item;
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
                if (CatchLengthWeightCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return CatchLengthWeights.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            EditSucceeded = false;
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < CatchLengthWeightCollection.Count)
            {
                if (CatchLengthWeightCollection[index].PK == id)
                {
                    CatchLengthWeightCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return EditSucceeded;
        }
    }
}
