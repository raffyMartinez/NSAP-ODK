using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthViewModel : IDisposable
    {


        public bool _editSuccess;
        public ObservableCollection<CatchLength> CatchLengthCollection { get; set; }
        private CatchLengthRepository CatchLengths { get; set; }

        private static StringBuilder _csv = new StringBuilder();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public static int CurrentIDNumber { get; set; }
        public bool DeleteAllInCollection()
        {
            int deleteCount = 0;
            int collectionCount = CatchLengthCollection.Count;
            foreach (var item in CatchLengthCollection.ToList())
            {
                if (DeleteRecordFromRepo(item.PK))
                {
                    deleteCount++;
                }
            }
            Dispose();
            return deleteCount == collectionCount;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                CatchLengthCollection.Clear();
                CatchLengthCollection = null;
                CatchLengths = null;

            }
            // free native resources if there are any.
        }
        public CatchLengthViewModel(VesselCatch vc)
        {
            CatchLengths = new CatchLengthRepository(vc);
            CatchLengthCollection = new ObservableCollection<CatchLength>(CatchLengths.CatchLengths);
            CatchLengthCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }
        public CatchLengthViewModel(bool isNew = false)
        {
            CatchLengths = new CatchLengthRepository(isNew);
            if (isNew)
            {
                CatchLengthCollection = new ObservableCollection<CatchLength>();
            }
            else
            {
                CatchLengthCollection = new ObservableCollection<CatchLength>(CatchLengths.CatchLengths);
            }
            CatchLengthCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }

        public List<CatchLength> GetAllCatchLengths()
        {
            return CatchLengthCollection.ToList();
        }

        public List<CatchLengthFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<CatchLengthFlattened> thisList = new List<CatchLengthFlattened>();
            if (tracked)
            {
                foreach (var item in CatchLengthCollection
                    .Where(t => t.Parent.Parent.OperationIsTracked == tracked))
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
            return CatchLengthRepository.ClearTable();
        }
        public static void ClearCSV()
        {

            _csv.Clear();
        }
        public CatchLength getCatchLength(int pk)
        {
            return CatchLengthCollection.FirstOrDefault(n => n.PK == pk);
        }

        private static bool SetCSV(CatchLength item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("catch_len_id", item.PK.ToString());
            myDict.Add("catch_id", item.Parent.PK.ToString());
            myDict.Add("length", item.Length.ToString());
            myDict.Add("sex", item.Sex);


            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_catch_len"));

            //_csv.AppendLine($"{item.PK},{item.Parent.PK},{item.Length},\"{item.Sex}\"");
            return true;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_catch_length")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_catch_len")}\r\n{_csv}";
                }
            }
        }
        private void CatchLenFreqCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        CatchLength newItem = CatchLengthCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = CatchLengths.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = CatchLengths.Add(CatchLengthCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchLength> tempListOfRemovedItems = e.OldItems.OfType<CatchLength>().ToList();
                        _editSuccess = CatchLengths.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchLength> tempList = e.NewItems.OfType<CatchLength>().ToList();
                        _editSuccess = CatchLengths.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
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
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(CatchLength item)
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
            return _editSuccess;
        }

        public int NextRecordNumber
        {

            get
            {
                //if (CatchLengthCollection.Count == 0)
                //{
                //    return 1;
                //}
                //else
                //{
                return CatchLengths.MaxRecordNumber() + 1;
                //}
            }
        }

        public bool DeleteRecordFromRepo(int id)
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
            return _editSuccess;
        }
    }
}
