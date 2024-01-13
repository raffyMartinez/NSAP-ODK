using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.Record.Chart;

namespace NSAP_ODK.Entities.Database
{
    public class CarrierBoatLanding_FishingGround_ViewModel : IDisposable
    {
        private static StringBuilder _csv = new StringBuilder();
        private bool _editSuccess;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public string FishingGroundsAsString
        {
            get
            {
                string fg = "";
                foreach (var item in CarrierBoatLanding_FishingGroundCollection)
                {
                    fg += $"{item.FishingGround.Name}, ";
                }
                return fg.Trim(new char[] { ',', ' ' });
            }
        }
        public int Count
        {
            get { return CarrierBoatLanding_FishingGroundCollection.Count; }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                CarrierBoatLanding_FishingGroundCollection.Clear();
                CarrierBoatLanding_FishingGroundCollection = null;
                CarrierBoatLanding_FishingGrounds = null;

            }
            // free native resources if there are any.
        }
        public ObservableCollection<CarrierBoatLanding_FishingGround> CarrierBoatLanding_FishingGroundCollection { get; set; }
        private CarrierBoatLanding_FishingGroundRepository CarrierBoatLanding_FishingGrounds { get; set; }
        public CarrierBoatLanding_FishingGround_ViewModel()
        {
            CarrierBoatLanding_FishingGrounds = new CarrierBoatLanding_FishingGroundRepository();
            CarrierBoatLanding_FishingGroundCollection = new ObservableCollection<CarrierBoatLanding_FishingGround>(CarrierBoatLanding_FishingGrounds.CarrierBoatLanding_FishingGrounds);
            CarrierBoatLanding_FishingGroundCollection.CollectionChanged += CarrierBoatLanding_FishingGroundCollection_CollectionChanged;
        }

        public static void ClearCSV()
        {

            _csv.Clear();
        }
        public static bool SetCSV(CarrierBoatLanding_FishingGround item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>
            {
                { "row_id", item.RowID.ToString() },
                { "fishing_ground", item.FishingGroundCode },
                { "carrierlanding_id", item.Parent.RowID.ToString() }
            };
            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_carrier_landing_fishing_ground"));
            return true;
        }
        public CarrierBoatLanding_FishingGround_ViewModel(CarrierLanding parent)
        {
            CarrierBoatLanding_FishingGrounds = new CarrierBoatLanding_FishingGroundRepository(parent);
            CarrierBoatLanding_FishingGroundCollection = new ObservableCollection<CarrierBoatLanding_FishingGround>(CarrierBoatLanding_FishingGrounds.CarrierBoatLanding_FishingGrounds);
            CarrierBoatLanding_FishingGroundCollection.CollectionChanged += CarrierBoatLanding_FishingGroundCollection_CollectionChanged;
        }

        public static int CurrentIDNumber { get; set; }

        public bool AddToRepo(CarrierBoatLanding_FishingGround item)
        {
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CarrierBoatLanding_FishingGroundCollection.Add(item);
            return _editSuccess;
        }

        public bool UpdateRecord(CarrierBoatLanding_FishingGround item)
        {
            if (item.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CarrierBoatLanding_FishingGroundCollection.Count)
            {
                if (CarrierBoatLanding_FishingGroundCollection[index].RowID == item.RowID)
                {
                    CarrierBoatLanding_FishingGroundCollection[index] = item;
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
            while (index < CarrierBoatLanding_FishingGroundCollection.Count)
            {
                if (CarrierBoatLanding_FishingGroundCollection[index].RowID == row_id)
                {
                    CarrierBoatLanding_FishingGroundCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_carrier_landing_fishing_ground")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_carrier_landing_fishing_ground")}\r\n{_csv}";
                }
            }
        }
        private void CarrierBoatLanding_FishingGroundCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        CarrierBoatLanding_FishingGround newItem = CarrierBoatLanding_FishingGroundCollection[e.NewStartingIndex];
                        if (newItem.Parent.Parent.DelayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = CarrierBoatLanding_FishingGrounds.Add(newItem);
                        }
                        //if (newItem.DelayedSave)
                        //{
                        //    _editSuccess = SetCSV(newItem);
                        //}
                        //else
                        //{

                        //}
                        //int newIndex = e.NewStartingIndex;
                        //_editSuccess = CatchLenFreqs.Add(CatchLenFreqCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CarrierBoatLanding_FishingGround> tempListOfRemovedItems = e.OldItems.OfType<CarrierBoatLanding_FishingGround>().ToList();
                        _editSuccess = CarrierBoatLanding_FishingGrounds.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CarrierBoatLanding_FishingGround> tempList = e.NewItems.OfType<CarrierBoatLanding_FishingGround>().ToList();
                        _editSuccess = CarrierBoatLanding_FishingGrounds.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
    }
}
