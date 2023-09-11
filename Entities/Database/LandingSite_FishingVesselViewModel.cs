using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSite_FishingVesselViewModel
    {
        private int? _nextRecordNumber;
        public event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        private bool _editSuccess = false;

        public LandingSite LandingSite { get; set; }
        public ObservableCollection<LandingSite_FishingVessel> LandingSite_FishingVessel_Collection { get; set; }
        private LandingSite_FishingVessel_Repository landingSite_FishingVessels { get; set; }
        public LandingSite_FishingVesselViewModel(LandingSite ls)
        {
            LandingSite = ls;
            landingSite_FishingVessels = new LandingSite_FishingVessel_Repository(ls);
            LandingSite_FishingVessel_Collection = new ObservableCollection<LandingSite_FishingVessel>(landingSite_FishingVessels.LandingSite_FishingVessels);
            LandingSite_FishingVessel_Collection.CollectionChanged += LandingSite_FishingVessel_Collection_CollectionChanged;
        }

        public Task<bool> ImportVesselsAsync(string vesselNames,  FisheriesSector fs)
        {
            return Task.Run(() => ImportVessels(vesselNames, fs));
        }
        public bool Cancel { get; set; }
        public List<EntityValidationMessage> EntityValidationMessages { get; set; }

        public int? NextRecordNumber
        {
            get
            {
                if (_nextRecordNumber == null)
                {
                    _nextRecordNumber = LandingSite_FishingVessel_Repository.GetMaxRecordNumber();
                }
                return _nextRecordNumber + 1;

            }
            set { _nextRecordNumber = value; }
        }
        private bool ImportVessels(string vesselNames, FisheriesSector fs)
        {
            EntityValidationMessages = new List<EntityValidationMessage>();
            int importCount = 0;
            List<EntityValidationMessage> entityMessages = new List<EntityValidationMessage>();
            List<string> vesselsToImport = vesselNames.Split('\n').ToList();
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start", TotalCountToProcess = vesselsToImport.Count });
            foreach (var item in vesselsToImport.Where(t => t.Length > 0))
            {
                string to_import = string.Join(" ", item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim('\r');
                if (!Cancel && to_import.Length > 0)
                {
                    var fv = new FishingVessel { Name = item.Trim(), FisheriesSector = fs, ID = NSAPEntities.FishingVesselViewModel.NextRecordNumber };
                    if (NSAPEntities.FishingVesselViewModel.EntityValidated(fv, out entityMessages, true))
                    {
                        if (NSAPEntities.FishingVesselViewModel.AddRecordToRepo(fv))
                        {
                            var ls_fv = new LandingSite_FishingVessel
                            {
                                FishingVessel = fv,
                                LandingSite = LandingSite,
                                DateAdded = DateTime.Now,
                                PK = (int)NextRecordNumber
                            };


                            if (LandingSite.LandingSite_FishingVesselViewModel.Add(ls_fv))
                            {
                                _nextRecordNumber++;
                                importCount++;
                                ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "imported_entity", CountProcessed = importCount, ProcessedItemName = ls_fv.FishingVessel.Name });
                            }
                        }
                    }
                    else
                    {
                        EntityValidationMessages.AddRange(entityMessages);
                    }
                }
                else
                {
                    ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "cancel" });
                    return importCount > 0;
                }
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "import_done" });
            return importCount > 0;
        }
        public bool Add(LandingSite_FishingVessel lf)
        {
            if (lf == null)
                throw new ArgumentNullException("Error: The argument is Null");
            LandingSite_FishingVessel_Collection.Add(lf);
            return _editSuccess;
        }
        public bool Update(LandingSite_FishingVessel lf)
        {
            if (lf.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < LandingSite_FishingVessel_Collection.Count)
            {
                if (LandingSite_FishingVessel_Collection[index].PK == lf.PK)
                {
                    LandingSite_FishingVessel_Collection[index] = lf;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public int Count
        {
            get { return LandingSite_FishingVessel_Collection.Count; }
        }

        public List<FishingVessel> GetFishingVessels(LandingSite ls)
        {
            List<FishingVessel> fvs = new List<FishingVessel>();
            foreach (var lf in
            LandingSite_FishingVessel_Collection.Where(t => t.LandingSite.LandingSiteID == ls.LandingSiteID).ToList())
            {
                fvs.Add(lf.FishingVessel);
            }
            return fvs;

        }
        public bool Delete(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < LandingSite_FishingVessel_Collection.Count)
            {
                if (LandingSite_FishingVessel_Collection[index].PK == id)
                {
                    LandingSite_FishingVessel_Collection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        private void LandingSite_FishingVessel_Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        LandingSite_FishingVessel newItem = LandingSite_FishingVessel_Collection[e.NewStartingIndex];
                        _editSuccess = landingSite_FishingVessels.Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<LandingSite_FishingVessel> tempListOfRemovedItems = e.OldItems.OfType<LandingSite_FishingVessel>().ToList();
                        _editSuccess = landingSite_FishingVessels.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<LandingSite_FishingVessel> tempList = e.NewItems.OfType<LandingSite_FishingVessel>().ToList();
                        _editSuccess = landingSite_FishingVessels.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
    }
}
