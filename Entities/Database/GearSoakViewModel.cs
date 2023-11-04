using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class GearSoakViewModel : IDisposable
    {

        private bool _editSuccess;
        public ObservableCollection<GearSoak> GearSoakCollection { get; set; }
        private GearSoakRepository GearSoaks { get; set; }
        private static StringBuilder _csv = new StringBuilder();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DateTime GetFirstSet()
        {
            return GearSoakCollection.OrderBy(t => t.TimeAtSet).First().TimeAtSet;
        }

        public DateTime GetLastHaul()
        {
            return GearSoakCollection.OrderByDescending(t => t.TimeAtHaul).First().TimeAtHaul;
        }
        public static int CurrentIDNumber { get; set; }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                GearSoakCollection.Clear();
                GearSoakCollection = null;
                GearSoaks = null;

            }
            // free native resources if there are any.
        }
        public GearSoakViewModel(VesselUnload vu)
        {
            GearSoaks = new GearSoakRepository(vu);
            GearSoakCollection = new ObservableCollection<GearSoak>(GearSoaks.GearSoaks);
            GearSoakCollection.CollectionChanged += GearSoaks_CollectionChanged;
        }
        public GearSoakViewModel(bool isNew = false)
        {
            GearSoaks = new GearSoakRepository(isNew);
            if (isNew)
            {
                GearSoakCollection = new ObservableCollection<GearSoak>();
            }
            else
            {
                GearSoakCollection = new ObservableCollection<GearSoak>(GearSoaks.GearSoaks);
            }
            GearSoakCollection.CollectionChanged += GearSoaks_CollectionChanged;
        }

        public List<GearSoak> GetAllGearSoaks()
        {
            return GearSoakCollection.ToList();
        }

        public List<GearSoakFlattened> GetAllFlattenedItems(bool tracked = false)
        {
            List<GearSoakFlattened> thisList = new List<GearSoakFlattened>();
            if (tracked)
            {
                foreach (var item in GearSoakCollection
                    .Where(t => t.Parent.OperationIsTracked == tracked))
                {
                    thisList.Add(new GearSoakFlattened(item));
                }
            }
            else
            {
                foreach (var item in GearSoakCollection)
                {
                    thisList.Add(new GearSoakFlattened(item));
                }
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            GearSoakCollection.Clear();
            return GearSoakRepository.ClearTable();
        }

        public GearSoak getGearSoak(int pk)
        {
            return GearSoakCollection.FirstOrDefault(n => n.PK == pk);
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_gear_soak")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_gear_soak")}\r\n{_csv}";
                }
            }
        }
        private static bool SetCSV(GearSoak item)
        {
            string time_set = item.TimeAtSet.ToString();
            string time_haul = item.TimeAtHaul.ToString();

            if (Utilities.Global.Settings.UsemySQL)
            {
                time_set = item.TimeAtSet.ToString("yyyy-MM-dd HH:mm:ss");
                time_haul = item.TimeAtHaul.ToString("yyyy-MM-dd HH:mm:ss");
            }
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("gear_soak_id", item.PK.ToString());
            myDict.Add("v_unload_id", item.Parent.PK.ToString());
            myDict.Add("time_set", time_set);
            myDict.Add("time_hauled", time_haul);
            myDict.Add("wpt_set", item.WaypointAtSet);
            myDict.Add("wpt_haul", item.WaypointAtHaul);

            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_gear_soak"));
            //_csv.AppendLine($"{item.PK},{item.Parent.PK},{time_set},{time_haul},\"{item.WaypointAtSet}\",\"{item.WaypointAtHaul}\"");
            return true;
        }
        private void GearSoaks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        GearSoak newItem = GearSoakCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = GearSoaks.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = GearSoaks.Add(GearSoakCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GearSoak> tempListOfRemovedItems = e.OldItems.OfType<GearSoak>().ToList();
                        _editSuccess = GearSoaks.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GearSoak> tempList = e.NewItems.OfType<GearSoak>().ToList();
                        _editSuccess = GearSoaks.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
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
            get
            {
                if (GearSoakCollection == null)
                {
                    return 0;
                }
                else
                {
                    return GearSoakCollection.Count;
                }
            }
        }

        public bool AddRecordToRepo(GearSoak item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GearSoakCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(GearSoak item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < GearSoakCollection.Count)
            {
                if (GearSoakCollection[index].PK == item.PK)
                {
                    GearSoakCollection[index] = item;
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
                if (GearSoakCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return GearSoaks.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearSoakCollection.Count)
            {
                if (GearSoakCollection[index].PK == id)
                {
                    GearSoakCollection.RemoveAt(index);
                    break;
                }
                index++;
            }

            return _editSuccess;
        }
    }
}
