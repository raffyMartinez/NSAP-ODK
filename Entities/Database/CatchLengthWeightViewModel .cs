using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthWeightViewModel : IDisposable
    {
        private bool _editSuccess;
        public ObservableCollection<CatchLengthWeight> CatchLengthWeightCollection { get; set; }
        private CatchLenWeightRepository CatchLengthWeights { get; set; }
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
                CatchLengthWeightCollection.Clear();
                CatchLengthWeightCollection = null;
                CatchLengthWeights = null;

            }
            // free native resources if there are any.
        }
        public CatchLengthWeightViewModel(VesselCatch vc)
        {
            CatchLengthWeights = new CatchLenWeightRepository(vc);
            CatchLengthWeightCollection = new ObservableCollection<CatchLengthWeight>(CatchLengthWeights.CatchLengthWeights);
            CatchLengthWeightCollection.CollectionChanged += CatchLengthWeightCollection_CollectionChanged;
        }
        public CatchLengthWeightViewModel(bool isNew = false)
        {
            CatchLengthWeights = new CatchLenWeightRepository(isNew);
            if (isNew)
            {
                CatchLengthWeightCollection = new ObservableCollection<CatchLengthWeight>();
            }
            else
            {
                CatchLengthWeightCollection = new ObservableCollection<CatchLengthWeight>(CatchLengthWeights.CatchLengthWeights);
            }
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
            return CatchLenWeightRepository.ClearTable();
        }

        public CatchLengthWeight getCatchLenghtWeight(int pk)
        {
            return CatchLengthWeightCollection.FirstOrDefault(n => n.PK == pk);
        }

        private static bool SetCSV(CatchLengthWeight item)
        {
            _csv.AppendLine($"{item.PK},{item.Parent.PK},{item.Length},{item.Weight},\"{item.Sex}\"");
            return true;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_catch_len_wt")}\r\n{ _csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_catch_len_wt")}\r\n{ _csv}";
                }
            }
        }
        private void CatchLengthWeightCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        CatchLengthWeight newItem = CatchLengthWeightCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = CatchLengthWeights.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = CatchLengthWeights.Add(CatchLengthWeightCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchLengthWeight> tempListOfRemovedItems = e.OldItems.OfType<CatchLengthWeight>().ToList();
                        _editSuccess = CatchLengthWeights.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchLengthWeight> tempList = e.NewItems.OfType<CatchLengthWeight>().ToList();
                        _editSuccess = CatchLengthWeights.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
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
            get { return CatchLengthWeightCollection.Count; }
        }

        public bool AddRecordToRepo(CatchLengthWeight item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CatchLengthWeightCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(CatchLengthWeight item)
        {
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
            return _editSuccess;
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
            return _editSuccess;
        }
    }
}
