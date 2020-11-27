using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthViewModel
    {
        public bool AddSucceeded;
        public ObservableCollection<CatchLength> CatchLengthCollection { get; set; }
        private CatchLengthRepository CatchLengths { get; set; }

        public CatchLengthViewModel()
        {
            CatchLengths = new CatchLengthRepository();
            CatchLengthCollection = new ObservableCollection<CatchLength>(CatchLengths.CatchLengths);
            CatchLengthCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }

        public List<CatchLength> GetAllCatchLengths()
        {
            return CatchLengthCollection.ToList();
        }

        public List<CatchLengthFlattened> GetAllFlattenedItems(bool tracked=false)
        {
            List<CatchLengthFlattened> thisList = new List<CatchLengthFlattened>();
            if (tracked)
            {
                foreach (var item in CatchLengthCollection
                    .Where(t=>t.Parent.Parent.OperationIsTracked == tracked))
                {
                    thisList.Add(new CatchLengthFlattened(item));
                }
            }
            else
            {
                foreach (var item in CatchLengthCollection)
                {
                    thisList.Add(new CatchLengthFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            CatchLengthCollection.Clear();
            return CatchLengths.ClearTable();
        }

        public CatchLength getCatchLength(int pk)
        {
            return CatchLengthCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void CatchLenFreqCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        AddSucceeded = CatchLengths.Add(CatchLengthCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchLength> tempListOfRemovedItems = e.OldItems.OfType<CatchLength>().ToList();
                        CatchLengths.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchLength> tempList = e.NewItems.OfType<CatchLength>().ToList();
                        CatchLengths.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return CatchLengthCollection.Count; }
        }

        public bool AddRecordToRepo(CatchLength item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CatchLengthCollection.Add(item);
            return AddSucceeded;
        }

        public void UpdateRecordInRepo(CatchLength item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CatchLengthCollection.Count)
            {
                if (CatchLengthCollection[index].PK == item.PK)
                {
                    CatchLengthCollection[index] = item;
                    break;
                }
                index++;
            }
        }

        public int NextRecordNumber
        {
            get
            {
                if (CatchLengthCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return CatchLengths.MaxRecordNumber() + 1;
                }
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < CatchLengthCollection.Count)
            {
                if (CatchLengthCollection[index].PK == id)
                {
                    CatchLengthCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
