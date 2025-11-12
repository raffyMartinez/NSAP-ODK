using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Database
{
    public class ETPviewModel
    {
        private bool _editSuccess;

        public static int? CurrentIDNumber { get; set; }
        public ETPviewModel(VesselUnload vu, List<string> list_etps)
        {

            if (CurrentIDNumber == null)
            {
                CurrentIDNumber = NextRecordNumber;
            }

            ETPs = new ETPRepository();
            //ETP_Collection = new ObservableCollection<ETP>(ETPs.ETPs);
            ETP_Collection = new ObservableCollection<ETP>();
            ETP_Collection.CollectionChanged += ETP_Collection_CollectionChanged;
            foreach (var item in list_etps)
            {
                ETP etp = new ETP
                {
                    VesselUnloadID = vu.PK,
                    RowID = (int)++CurrentIDNumber,
                    ETP_Name = item,
                    DelayedSave = true,
                    Parent = vu
                };
                AddRecordToRepo(etp);
            }
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_unload_etp_interaction")}\r\n{_csv}";
                }
            }
        }
        public ETPviewModel(VesselUnload vu)
        {
            ETPs = new ETPRepository(vu);
            ETP_Collection = new ObservableCollection<ETP>(ETPs.ETPs);
            ETP_Collection.CollectionChanged += ETP_Collection_CollectionChanged;
        }
        public static void ClearCSV()
        {
            _csv.Clear();
        }
        private static bool SetCSV(ETP item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("row_id", item.RowID.ToString());
            myDict.Add("v_unload_id", item.Parent.PK.ToString());
            myDict.Add("etp_name", item.ETP_Name.ToString());
            


            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_vessel_unload_etp_interaction"));
            //_csv.AppendLine($"{item.PK},{item.Parent.PK},{item.LengthClass},{item.Frequency},\"{item.Sex}\"");
            return true;
        }
        private void ETP_Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        ETP newItem = ETP_Collection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = ETPs.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = ETPs.Add(ETP_Collection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<ETP> tempListOfRemovedItems = e.OldItems.OfType<ETP>().ToList();
                        _editSuccess = ETPs.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<ETP> tempList = e.NewItems.OfType<ETP>().ToList();
                        _editSuccess = ETPs.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public ObservableCollection<ETP> ETP_Collection { get; set; }
        private ETPRepository ETPs { get; set; }

        private static StringBuilder _csv = new StringBuilder();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                ETP_Collection.Clear();
                ETP_Collection = null;
                ETPs = null;

            }
            // free native resources if there are any.
        }
        public int Count
        {
            get { return ETP_Collection.Count; }
        }

        public bool AddRecordToRepo(ETP item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            ETP_Collection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(ETP item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < ETP_Collection.Count)
            {
                if (ETP_Collection[index].RowID == item.RowID)
                {
                    ETP_Collection[index] = item;
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
                if (ETP_Collection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return ETPRepository.MaxRecordNumber() + 1;

                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < ETP_Collection.Count)
            {
                if (ETP_Collection[index].RowID == id)
                {
                    ETP_Collection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
    }
}
