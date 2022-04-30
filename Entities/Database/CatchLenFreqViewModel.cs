using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLenFreqViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<CatchLenFreq> CatchLenFreqCollection { get; set; }
        private CatchLenFreqRepository CatchLenFreqs { get; set; }

        public CatchLenFreqViewModel(VesselCatch vc)
        {
            CatchLenFreqs = new CatchLenFreqRepository(vc);
            CatchLenFreqCollection = new ObservableCollection<CatchLenFreq>(CatchLenFreqs.CatchLenFreqs);
            CatchLenFreqCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }
        public CatchLenFreqViewModel()
        {
            CatchLenFreqs = new CatchLenFreqRepository();
            CatchLenFreqCollection = new ObservableCollection<CatchLenFreq>(CatchLenFreqs.CatchLenFreqs);
            CatchLenFreqCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }

        public List<CatchLenFreq> GetAllCatchLenFreqs()
        {
            return CatchLenFreqCollection.ToList();
        }

        public List<CatchLenFreqFlattened> GetAllFlattenedItems(bool tracked=false)
        {
            List<CatchLenFreqFlattened> thisList = new List<CatchLenFreqFlattened>();
            if (tracked)
            {
                foreach (var item in CatchLenFreqCollection
                    .Where(t=>t.Parent.Parent.OperationIsTracked==tracked))
                {
                    thisList.Add(new CatchLenFreqFlattened(item));
                }
            }
            else
            {
                foreach (var item in CatchLenFreqCollection)
                {
                    thisList.Add(new CatchLenFreqFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            CatchLenFreqCollection.Clear();
            return CatchLenFreqs.ClearTable();
        }

        public CatchLenFreq getCatchLenFreq(int pk)
        {
            return CatchLenFreqCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void CatchLenFreqCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = CatchLenFreqs.Add(CatchLenFreqCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchLenFreq> tempListOfRemovedItems = e.OldItems.OfType<CatchLenFreq>().ToList();
                        _editSuccess= CatchLenFreqs.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchLenFreq> tempList = e.NewItems.OfType<CatchLenFreq>().ToList();
                        _editSuccess =  CatchLenFreqs.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return CatchLenFreqCollection.Count; }
        }

        public bool AddRecordToRepo(CatchLenFreq item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CatchLenFreqCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(CatchLenFreq item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CatchLenFreqCollection.Count)
            {
                if (CatchLenFreqCollection[index].PK == item.PK)
                {
                    CatchLenFreqCollection[index] = item;
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
                if (CatchLenFreqCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return CatchLenFreqs.MaxRecordNumber()+ 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < CatchLenFreqCollection.Count)
            {
                if (CatchLenFreqCollection[index].PK == id)
                {
                    CatchLenFreqCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
    }
}
