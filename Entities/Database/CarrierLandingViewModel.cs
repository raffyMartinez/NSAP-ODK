using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Org.BouncyCastle.Asn1.Mozilla;
namespace NSAP_ODK.Entities.Database
{
    public class CarrierLandingViewModel:IDisposable
    {
        private static StringBuilder _csv = new StringBuilder();
        private bool _editSuccess;
        private bool _delayedSave;

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
                CarrierLandingCollection.Clear();
                CarrierLandingCollection = null;
                CarrierLandings = null;

            }
            // free native resources if there are any.
        }
        public static int CurrentIDNumber { get; set; }
        public ObservableCollection<CarrierLanding> CarrierLandingCollection { get; set; }
        private CarrierLandingRepository CarrierLandings { get; set; }


        public CarrierLandingViewModel(LandingSiteSampling parent)
        {
            _delayedSave = parent.DelayedSave;
            CarrierLandings = new CarrierLandingRepository(parent);
            CarrierLandingCollection = new ObservableCollection<CarrierLanding>(CarrierLandings.CarrierLandings);
            CarrierLandingCollection.CollectionChanged += CarrierLandingCollection_CollectionChanged;
        }

        public bool AddRecordToRepo(CarrierLanding cl)
        {
            if (cl == null)
                throw new ArgumentNullException("Error: The argument is Null");
            CarrierLandingCollection.Add(cl);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(CarrierLanding cl)
        {
            if (cl.RowID == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < CarrierLandingCollection.Count)
            {
                if (CarrierLandingCollection[index].RowID == cl.RowID)
                {
                    CarrierLandingCollection[index] = cl;
                    break;
                }
                index++;
            }

            return _editSuccess;
        }

        public bool DeleteRecordInRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < CarrierLandingCollection.Count)
            {
                if (CarrierLandingCollection[index].RowID == id)
                {
                    CarrierLandingCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public static void ClearCSV()
        {

            _csv.Clear();
        }

        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_carrier_landing")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_carrier_landing")}\r\n{_csv}";
                }
            }
        }
        public static bool SetCSV(CarrierLanding item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("row_id", item.RowID.ToString());
            myDict.Add("carrier_name", item.CarrierName);
            myDict.Add("landingsitesampling_id", item.Parent.PK.ToString());
            myDict.Add("sampling_date", item.SamplingDate.ToString());
            myDict.Add("count_catchers", item.CountCatchers == null ? "" : item.CountCatchers.ToString());
            myDict.Add("count_species_catch_composition", item.CountSpeciesComposition.ToString());

            string wt_catch = "";
            if (item.WeightOfCatch != null)
            {
                wt_catch = item.WeightOfCatch.ToString();
            }
            myDict.Add("weight_catch", wt_catch);
            myDict.Add("ref_no", item.RefNo);
            _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_carrier_landing"));


            return true;
        }
        private void CarrierLandingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        CarrierLanding newItem = CarrierLandingCollection[e.NewStartingIndex];
                        if (_delayedSave)
                        {
                            _editSuccess = SetCSV(newItem);
                        }
                        else
                        {
                            _editSuccess = CarrierLandings.Add(newItem);
                        }
                        //int newIndex = e.NewStartingIndex;

                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<CarrierLanding> tempListOfRemovedItems = e.OldItems.OfType<CarrierLanding>().ToList();
                        _editSuccess = CarrierLandings.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<CarrierLanding> tempList = e.NewItems.OfType<CarrierLanding>().ToList();
                        _editSuccess = CarrierLandings.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
    }
}
