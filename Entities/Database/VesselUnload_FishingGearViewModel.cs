using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselUnload_FishingGearViewModel : IDisposable
    {
        private bool _editSucceeded;
        private static StringBuilder _csv = new StringBuilder();
        private bool _isTemporary = false;
        private bool _set_csv_on_update=false;
        public ObservableCollection<VesselUnload_FishingGear> VesselUnload_FishingGearsCollection { get; set; }
        private VesselUnload_FishingGearRepository VesselUnload_FishingGears { get; set; }
        public int Count()
        {
            return VesselUnload_FishingGearsCollection.Count;
        }
        public void RefreshCollection()
        {
            VesselUnload_FishingGearsCollection = new ObservableCollection<VesselUnload_FishingGear>(VesselUnload_FishingGears.VesselUnload_FishingGears);
        }
        public static int CurrentIDNumber { get; set; }

        public VesselUnload_FishingGearViewModel(VesselUnload parent = null)
        {

            VesselUnload_FishingGears = new VesselUnload_FishingGearRepository(parent);

            if (parent != null)
            {
                VesselUnload_FishingGearsCollection = new ObservableCollection<VesselUnload_FishingGear>(VesselUnload_FishingGears.VesselUnload_FishingGears);

                if (parent.Parent.Parent.IsMultiVessel)
                {
                    foreach(VesselUnload_FishingGear vufg in VesselUnload_FishingGearsCollection)
                    {

                    }

                }
            }
            else
            {
                VesselUnload_FishingGearsCollection = new ObservableCollection<VesselUnload_FishingGear>();
            }
            VesselUnload_FishingGearsCollection.CollectionChanged += VesselUnload_FishingGearsCollection_CollectionChanged;

        }
        public VesselUnload_FishingGearViewModel(bool isNew = false)
        {
            VesselUnload_FishingGears = new VesselUnload_FishingGearRepository(isNew);
            if (isNew)
            {
                VesselUnload_FishingGearsCollection = new ObservableCollection<VesselUnload_FishingGear>();
            }
            else
            {
                VesselUnload_FishingGearsCollection = new ObservableCollection<VesselUnload_FishingGear>(VesselUnload_FishingGears.VesselUnload_FishingGears);
            }
            VesselUnload_FishingGearsCollection.CollectionChanged += VesselUnload_FishingGearsCollection_CollectionChanged;
        }
        public bool ClearRepository()
        {
            VesselUnload_FishingGearsCollection.Clear();
            return VesselUnload_FishingGearRepository.ClearTable();
        }
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
                VesselUnload_FishingGearsCollection.Clear();
                VesselUnload_FishingGearsCollection = null;
                VesselUnload_FishingGears = null;

            }
            // free native resources if there are any.
        }

        private static bool SetCSV(VesselUnload_FishingGear item)
        {
            //string gear_code = item.GearCode;
            //if (!string.IsNullOrEmpty(item.GearText))
            //{
            //    gear_code = "";
            //}
            string catch_wt = "";
            if(item.WeightOfCatch!=null)
            {
                catch_wt = ((double)item.WeightOfCatch).ToString();
            }

            string sp_count = "";
            if(item.CountItemsInCatchComposition!=null)
            {
                sp_count = ((int)item.CountItemsInCatchComposition).ToString();
            }
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("row_id", item.RowID.ToString());
            myDict.Add("vessel_unload_id", item.Parent.PK.ToString());
            myDict.Add("gear_code", string.IsNullOrEmpty(item.GearText) ? item.GearCode : "");
            myDict.Add("gear_text", item.GearText);
            myDict.Add("catch_weight", catch_wt);
            myDict.Add("species_comp_count", sp_count);


            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_vesselunload_fishinggear"));
            //_csv.AppendLine($"{item.RowID},{item.Parent.PK},\"{item.GearCode}\",\"{item.GearText}\"");
            return true;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return "";
                    //return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_vessel_effort")}\r\n{_csv.ToString()}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vesselunload_fishinggear")}\r\n{_csv.ToString()}";
                }
            }
        }

        public static void ClearCSV()
        {
            _csv.Clear();
        }
        private void VesselUnload_FishingGearsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {


            _editSucceeded = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!_isTemporary)
                        {
                            VesselUnload_FishingGear newItem = VesselUnload_FishingGearsCollection[e.NewStartingIndex];
                            if (newItem.DelayedSave)
                            {
                                //_editSucceeded = SetCSV(newItem);
                                _editSucceeded = true;
                            }
                            else
                            {
                                _editSucceeded = VesselUnload_FishingGears.Add(newItem);
                            }
                        }
                        //int newIndex = e.NewStartingIndex;
                        //EditSucceeded = VesselEfforts.Add(VesselEffortCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselUnload_FishingGear> tempListOfRemovedItems = e.OldItems.OfType<VesselUnload_FishingGear>().ToList();
                        _editSucceeded = VesselUnload_FishingGears.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselUnload_FishingGear> tempList = e.NewItems.OfType<VesselUnload_FishingGear>().ToList();
                        var vufg = tempList[0];
                        if (vufg.DelayedSave && _set_csv_on_update)
                        {
                            _editSucceeded = SetCSV(vufg);
                        }
                        else
                        {
                            _editSucceeded = VesselUnload_FishingGears.Update(vufg);      // As the IDs are unique, only one row will be effected hence first index only
                        }
                    }
                    break;
            }
        }

        public bool AddRecordToRepo(VesselUnload_FishingGear vu_fg, bool isTemporary = false)
        {
            _editSucceeded = false;
            _isTemporary = isTemporary;
            if (vu_fg == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselUnload_FishingGearsCollection.Add(vu_fg);
            return _editSucceeded;
        }

        public bool UpdateRecordInRepo(VesselUnload_FishingGear vu_fg, bool update_wt_ct=false)
        {
            _set_csv_on_update = update_wt_ct;
            _editSucceeded = false;
            if (vu_fg.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < VesselUnload_FishingGearsCollection.Count)
            {
                if (VesselUnload_FishingGearsCollection[index].RowID == vu_fg.RowID)
                {
                    VesselUnload_FishingGearsCollection[index] = vu_fg;
                    break;
                }
                index++;
            }
            return _editSucceeded;
        }

        public int NextRecordNumber
        {
            get
            {
                if (VesselUnload_FishingGearsCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return VesselUnload_FishingGears.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            _editSucceeded = false;
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselUnload_FishingGearsCollection.Count)
            {
                if (VesselUnload_FishingGearsCollection[index].RowID == id)
                {
                    VesselUnload_FishingGearsCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSucceeded;
        }
    }
}
