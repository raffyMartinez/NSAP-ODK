using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselUnload_Gear_Spec_ViewModel : IDisposable
    {
        private static StringBuilder _csv = new StringBuilder();
        private bool _editSucceeded;
        private bool _isTemporary = false;
        public ObservableCollection<VesselUnload_Gear_Spec> VesselUnload_Gear_SpecCollection { get; set; }
        private VesselUnload_Gear_Spec_Repository VesselUnload_Gear_Specs { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static int CurrentIDNumber { get; set; }
        public bool ClearRepository()
        {
            VesselUnload_Gear_SpecCollection.Clear();
            return VesselUnload_Gear_Spec_Repository.ClearTable();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                VesselUnload_Gear_SpecCollection.Clear();
                VesselUnload_Gear_SpecCollection = null;
                VesselUnload_Gear_Specs = null;

            }
            // free native resources if there are any.
        }
        public VesselUnload_Gear_Spec_ViewModel(VesselUnload_FishingGear parent = null)
        {
            VesselUnload_Gear_Specs = new VesselUnload_Gear_Spec_Repository(parent);
            if (parent != null)
            {
                VesselUnload_Gear_SpecCollection = new ObservableCollection<VesselUnload_Gear_Spec>(VesselUnload_Gear_Specs.VesselUnload_Gear_Specses);
            }
            else
            {
                VesselUnload_Gear_SpecCollection = new ObservableCollection<VesselUnload_Gear_Spec>();
            }
            VesselUnload_Gear_SpecCollection.CollectionChanged += VesselUnload_Gear_SpecCollection_CollectionChanged;
        }
        private static bool SetCSV(VesselUnload_Gear_Spec item)
        {
            string effort_numeric = "";
            if (item.EffortValueNumeric != null)
            {
                effort_numeric = ((double)item.EffortValueNumeric).ToString();
            }
            else if (Utilities.Global.Settings.UsemySQL && item.EffortValueNumeric == null)
            {
                effort_numeric = @"\N";
            }

            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("effort_row_id", item.RowID.ToString());
            myDict.Add("v_unload_id", "");
            myDict.Add("effort_spec_id", item.EffortSpecID.ToString());
            myDict.Add("effort_value_numeric", effort_numeric);
            myDict.Add("effort_value_text", item.EffortValueText);
            myDict.Add("vessel_unload_fishing_gear_id", item.Parent.RowID.ToString());

            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_vessel_effort"));
            //_csv.AppendLine($"{item.RowID},{item.EffortSpecID},{effort_numeric},\"{item.EffortValueText}\",{item.Parent.RowID}");
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
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_effort")}\r\n{_csv.ToString()}";
                }
            }
        }

        public static void ClearCSV()
        {
            _csv.Clear();
        }
        private void VesselUnload_Gear_SpecCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSucceeded = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!_isTemporary)
                        {
                            VesselUnload_Gear_Spec newItem = VesselUnload_Gear_SpecCollection[e.NewStartingIndex];

                            if (newItem.DelayedSave)
                            {
                                _editSucceeded = SetCSV(newItem);
                            }
                            else
                            {
                                _editSucceeded = VesselUnload_Gear_Specs.Add(newItem);
                            }
                        }
                        //int newIndex = e.NewStartingIndex;
                        //EditSucceeded = VesselEfforts.Add(VesselEffortCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselUnload_Gear_Spec> tempListOfRemovedItems = e.OldItems.OfType<VesselUnload_Gear_Spec>().ToList();
                        _editSucceeded = VesselUnload_Gear_Specs.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselUnload_Gear_Spec> tempList = e.NewItems.OfType<VesselUnload_Gear_Spec>().ToList();
                        _editSucceeded = VesselUnload_Gear_Specs.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public bool AddRecordToRepo(VesselUnload_Gear_Spec item, bool isTemporary = false)
        {
            _editSucceeded = false;
            _isTemporary = isTemporary;
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselUnload_Gear_SpecCollection.Add(item);
            return _editSucceeded;
        }

        public bool UpdateRecordInRepo(VesselUnload_Gear_Spec item)
        {
            _editSucceeded = false;
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < VesselUnload_Gear_SpecCollection.Count)
            {
                if (VesselUnload_Gear_SpecCollection[index].RowID == item.RowID)
                {
                    VesselUnload_Gear_SpecCollection[index] = item;
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
                return NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselEffortsPK+1;
                //if (VesselUnload_Gear_SpecCollection.Count == 0)
                //{
                //    return 1;
                //}
                //else
                //{
                //    return VesselUnload_Gear_Specs.MaxRecordNumber() + 1;
                //}
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            _editSucceeded = false;
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselUnload_Gear_SpecCollection.Count)
            {
                if (VesselUnload_Gear_SpecCollection[index].RowID == id)
                {
                    VesselUnload_Gear_SpecCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSucceeded;
        }
    }
}
