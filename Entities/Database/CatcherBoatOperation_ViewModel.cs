using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Database
{
    public class CatcherBoatOperation_ViewModel : IDisposable
    {
        private static StringBuilder _csv = new StringBuilder();
        private bool _editSuccess;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public static void ClearCSV()
        {

            _csv.Clear();
        }

        public string CatcherNamesAsString
        {
            get
            {
                string names = "";
                foreach (CatcherBoatOperation cb in CatcherBoatOperationCollection)
                {
                    names += $"{cb.CatcherBoatName}, ";
                }
                return names.Trim(new char[] { ',', ' ' });
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                CatcherBoatOperationCollection.Clear();
                CatcherBoatOperationCollection = null;
                CatcherBoatOperations = null;

            }
            // free native resources if there are any.
        }
        public ObservableCollection<CatcherBoatOperation> CatcherBoatOperationCollection { get; set; }
        private CatcherBoatOperationRepository CatcherBoatOperations { get; set; }
        public static int CurrentIDNumber { get; set; }
        public CatcherBoatOperation_ViewModel(CarrierLanding parent)
        {
            CatcherBoatOperations = new CatcherBoatOperationRepository(parent);
            CatcherBoatOperationCollection = new ObservableCollection<CatcherBoatOperation>(CatcherBoatOperations.CatcherBoatOperations);
            CatcherBoatOperationCollection.CollectionChanged += CatcherBoatOperationCollection_CollectionChanged;
        }
        public CatcherBoatOperation_ViewModel()
        {
            CatcherBoatOperations = new CatcherBoatOperationRepository();
            CatcherBoatOperationCollection = new ObservableCollection<CatcherBoatOperation>(CatcherBoatOperations.CatcherBoatOperations);
            CatcherBoatOperationCollection.CollectionChanged += CatcherBoatOperationCollection_CollectionChanged;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_catcher_boat_operations")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_catcher_boat_operations")}\r\n{_csv}";
                }
            }
        }
        public static bool SetCSV(CatcherBoatOperation item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>
            {
                { "row_id", item.RowID.ToString() },
                { "boat_name", item.CatcherBoatName },
                { "date_start_operation", item.StartOfOperation.Date.ToString() },
                { "catch_weight", item.WeightOfCatch==null ? "" : item.WeightOfCatch.ToString()},
                { "gear_code", item.GearCode },
                { "carrierlanding_id", item.Parent.RowID.ToString()}
            };
            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_catcher_boat_operations"));
            return true;
        }
        public int Count
        {
            get { return CatcherBoatOperationCollection.Count; }
        }
        public bool AddToRepo(CatcherBoatOperation item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CatcherBoatOperationCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecord(CatcherBoatOperation item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CatcherBoatOperationCollection.Count)
            {
                if (CatcherBoatOperationCollection[index].RowID == item.RowID)
                {
                    CatcherBoatOperationCollection[index] = item;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool DeleteRecord(int row_id)
        {
            if (row_id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < CatcherBoatOperationCollection.Count)
            {
                if (CatcherBoatOperationCollection[index].RowID == row_id)
                {
                    CatcherBoatOperationCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        private void CatcherBoatOperationCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        CatcherBoatOperation newItem = CatcherBoatOperationCollection[e.NewStartingIndex];
                        if (newItem.Parent.Parent.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = CatcherBoatOperations.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = CatchLenFreqs.Add(CatchLenFreqCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatcherBoatOperation> tempListOfRemovedItems = e.OldItems.OfType<CatcherBoatOperation>().ToList();
                        _editSuccess = CatcherBoatOperations.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatcherBoatOperation> tempList = e.NewItems.OfType<CatcherBoatOperation>().ToList();
                        _editSuccess = CatcherBoatOperations.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
    }
}
