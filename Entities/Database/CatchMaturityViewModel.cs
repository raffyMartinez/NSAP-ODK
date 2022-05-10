using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchMaturityViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<CatchMaturity> CatchMaturityCollection { get; set; }
        private CatchMaturityRepository CatchMaturities { get; set; }

        public CatchMaturityViewModel(VesselCatch vc)
        {
            CatchMaturities = new CatchMaturityRepository(vc);
            CatchMaturityCollection = new ObservableCollection<CatchMaturity>(CatchMaturities.CatchMaturities);
            CatchMaturityCollection.CollectionChanged += CatchMaturityCollection_CollectionChanged;
        }
        public CatchMaturityViewModel(bool isNew=false)
        {
            CatchMaturities = new CatchMaturityRepository(isNew);
            if (isNew)
            {
                CatchMaturityCollection = new ObservableCollection<CatchMaturity>();
            }
            else
            {
                CatchMaturityCollection = new ObservableCollection<CatchMaturity>(CatchMaturities.CatchMaturities);
            }
            CatchMaturityCollection.CollectionChanged += CatchMaturityCollection_CollectionChanged;
        }

        public List<CatchMaturity> GetAllCatchMaturities()
        {
            return CatchMaturityCollection.ToList();
        }

        public List<CatchMaturityFlattened> GetAllFlattenedItems(bool tracked=false)
        {
            List<CatchMaturityFlattened> thisList = new List<CatchMaturityFlattened>();
            if(tracked)
            {
                foreach (var item in CatchMaturityCollection
                    .Where(t=>t.Parent.Parent.OperationIsTracked==tracked))
                {
                    thisList.Add(new CatchMaturityFlattened(item));
                }
            }
            foreach (var item in CatchMaturityCollection)
            {
                thisList.Add(new CatchMaturityFlattened(item));
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            CatchMaturityCollection.Clear();
            return CatchMaturityRepository.ClearTable();
        }

        public CatchMaturity getCatchMaturity(int pk)
        {
            return CatchMaturityCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void CatchMaturityCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = CatchMaturities.Add(CatchMaturityCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchMaturity> tempListOfRemovedItems = e.OldItems.OfType<CatchMaturity>().ToList();
                        _editSuccess= CatchMaturities.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchMaturity> tempList = e.NewItems.OfType<CatchMaturity>().ToList();
                        _editSuccess= CatchMaturities.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return CatchMaturityCollection.Count; }
        }

        public bool AddRecordToRepo(CatchMaturity item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CatchMaturityCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(CatchMaturity item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CatchMaturityCollection.Count)
            {
                if (CatchMaturityCollection[index].PK == item.PK)
                {
                    CatchMaturityCollection[index] = item;
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
                if (CatchMaturityCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return CatchMaturities.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < CatchMaturityCollection.Count)
            {
                if (CatchMaturityCollection[index].PK == id)
                {
                    CatchMaturityCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
    }
}
