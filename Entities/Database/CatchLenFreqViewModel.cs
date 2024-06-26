﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLenFreqViewModel : IDisposable
    {
        private bool _editSuccess;
        public ObservableCollection<CatchLenFreq> CatchLenFreqCollection { get; set; }
        private CatchLenFreqRepository CatchLenFreqs { get; set; }

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
                CatchLenFreqCollection.Clear();
                CatchLenFreqCollection = null;
                CatchLenFreqs = null;

            }
            // free native resources if there are any.
        }
        public bool DeleteAllInCollection()
        {
            int deleteCount = 0;
            int collectionCount = CatchLenFreqCollection.Count;
            foreach (var item in CatchLenFreqCollection.ToList())
            {
                if (DeleteRecordFromRepo(item.PK))
                {
                    deleteCount++;
                }
            }
            Dispose();
            return deleteCount == collectionCount;
        }
        public CatchLenFreqViewModel(VesselCatch vc)
        {
            CatchLenFreqs = new CatchLenFreqRepository(vc);
            CatchLenFreqCollection = new ObservableCollection<CatchLenFreq>(CatchLenFreqs.CatchLenFreqs);
            CatchLenFreqCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }
        public CatchLenFreqViewModel(bool isNew = false)
        {
            CatchLenFreqs = new CatchLenFreqRepository(isNew);
            if (isNew)
            {
                CatchLenFreqCollection = new ObservableCollection<CatchLenFreq>();
            }
            else
            {
                CatchLenFreqCollection = new ObservableCollection<CatchLenFreq>(CatchLenFreqs.CatchLenFreqs);
            }
            CatchLenFreqCollection.CollectionChanged += CatchLenFreqCollection_CollectionChanged;
        }

        public List<CatchLenFreq> GetAllCatchLenFreqs()
        {
            return CatchLenFreqCollection.ToList();
        }

        public List<CatchLenFreqFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<CatchLenFreqFlattened> thisList = new List<CatchLenFreqFlattened>();
            if (tracked)
            {
                foreach (var item in CatchLenFreqCollection
                    .Where(t => t.Parent.Parent.OperationIsTracked == tracked))
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
        public static void ClearCSV()
        {
            _csv.Clear();
        }
        public bool ClearRepository()
        {
            CatchLenFreqCollection.Clear();
            return CatchLenFreqRepository.ClearTable();
        }

        public CatchLenFreq getCatchLenFreq(int pk)
        {
            return CatchLenFreqCollection.FirstOrDefault(n => n.PK == pk);
        }

        private static bool SetCSV(CatchLenFreq item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("catch_len_freq_id", item.PK.ToString());
            myDict.Add("catch_id", item.Parent.PK.ToString());
            myDict.Add("len_class", item.LengthClass.ToString());
            myDict.Add("freq", item.Frequency.ToString());
            myDict.Add("sex", item.Sex);


            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_catch_len_freq"));
            //_csv.AppendLine($"{item.PK},{item.Parent.PK},{item.LengthClass},{item.Frequency},\"{item.Sex}\"");
            return true;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_catch_len_freq")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_catch_len_freq")}\r\n{_csv}";
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
                        CatchLenFreq newItem = CatchLenFreqCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = CatchLenFreqs.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = CatchLenFreqs.Add(CatchLenFreqCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CatchLenFreq> tempListOfRemovedItems = e.OldItems.OfType<CatchLenFreq>().ToList();
                        _editSuccess = CatchLenFreqs.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CatchLenFreq> tempList = e.NewItems.OfType<CatchLenFreq>().ToList();
                        _editSuccess = CatchLenFreqs.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
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
                //if (CatchLenFreqCollection.Count == 0)
                //{
                //    return 1;
                //}
                //else
                //{
                    return CatchLenFreqs.MaxRecordNumber() + 1;
                //}
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
