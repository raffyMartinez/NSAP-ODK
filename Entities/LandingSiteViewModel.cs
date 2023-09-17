using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Data;

namespace NSAP_ODK.Entities

{
    public class LandingSiteViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<LandingSite> LandingSiteCollection { get; set; }
        private LandingSiteRepository LandingSites { get; set; }

        //public event EventHandler<EntityChangedEventArgs> EntityChanged;


        private string WhereLandingSiteIsUsed(List<NSAPRegionFMAFishingGroundLandingSite> listUsedLS, LandingSite ls)
        {
            string whereUsed = "";
            foreach (NSAPRegionFMAFishingGroundLandingSite usedLS in listUsedLS.Where(t => t.LandingSite.LandingSiteID == ls.LandingSiteID))
            {
                whereUsed += $"{usedLS.NSAPRegionFMAFishingGround.FishingGround}, ";
            }
            return whereUsed.Trim(new char[] { ' ', ',' });
        }
        public List<LandingSite> GetAllLandingSitesShowUsed()
        {
            List<LandingSite> allLandingSitesShowUsed = new List<LandingSite>();
            var listUsedLS = GetUsedLandingSites();
            foreach (LandingSite ls in LandingSiteCollection)
            {
                ls.IsUsed = listUsedLS.Where(t => t.LandingSite.LandingSiteID == ls.LandingSiteID).Count() > 0;
                ls.WhereUsed = WhereLandingSiteIsUsed(listUsedLS, ls);
                allLandingSitesShowUsed.Add(ls);
            }
            return allLandingSitesShowUsed;
        }
        public List<NSAPRegionFMAFishingGroundLandingSite> GetUsedLandingSites()
        {


            List<NSAPRegionFMAFishingGroundLandingSite> usedLandingSites = new List<NSAPRegionFMAFishingGroundLandingSite>();
            foreach (NSAPRegion nr in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection)
            {
                foreach (NSAPRegionFMA nrf in nr.FMAs)
                {
                    foreach (NSAPRegionFMAFishingGround nrfg in nrf.FishingGrounds)
                    {
                        foreach (NSAPRegionFMAFishingGroundLandingSite nrfgls in nrfg.LandingSites)
                        {
                            usedLandingSites.Add(nrfgls);
                        }
                    }
                }
            }
            return usedLandingSites;


        }
        public DataSet Dataset()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Landing sites");


            DataColumn dc = new DataColumn { ColumnName = "Landing site", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Id", DataType = typeof(int) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Province", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Municipality", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Brgy", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Longitude", DataType = typeof(double) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Latitude", DataType = typeof(double) };
            dt.Columns.Add(dc);

            foreach (var ls in LandingSiteCollection)
            {
                var row = dt.NewRow();
                row["Landing site"] = ls.LandingSiteName;
                row["Id"] = ls.LandingSiteID;
                row["Province"] = ls.Municipality.Province.ProvinceName;
                row["Municipality"] = ls.Municipality.MunicipalityName;
                row["Brgy"] = ls.Barangay;
                if (ls.Longitude == null)
                {
                    row["Longitude"] = DBNull.Value;

                }
                else
                {
                    row["Longitude"] = ls.Longitude;
                }
                if (ls.Latitude == null)
                {
                    row["Latitude"] = DBNull.Value;

                }
                else
                {
                    row["Latitude"] = ls.Latitude;
                }
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds;
        }
        public LandingSiteViewModel()
        {
            LandingSites = new LandingSiteRepository();
            LandingSiteCollection = new ObservableCollection<LandingSite>(LandingSites.landingSites);
            LandingSiteCollection.CollectionChanged += LandingSiteCollection_CollectionChanged;
        }

        public int Count

        {
            get { return LandingSiteCollection.Count; }
        }

        public bool LandingSiteNameExists(string landingSiteName)
        {
            foreach (LandingSite ls in LandingSiteCollection)
            {
                if (ls.LandingSiteName == landingSiteName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool LandingSiteNameExists(LandingSite landingSite)
        {
            foreach (LandingSite ls in LandingSiteCollection)
            {
                if (ls.LandingSiteName == landingSite.LandingSiteName && ls.Municipality == landingSite.Municipality)
                {
                    return true;
                }
            }
            return false;
        }

        public int NextRecordNumber
        {
            get
            {
                if (LandingSiteCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return LandingSiteCollection.Max(t => t.LandingSiteID) + 1;
                    return LandingSites.MaxRecordNumber() + 1;
                }
            }
        }

        public LandingSite CurrentEntity { get; set; }
        public LandingSite GetLandingSite(string landingSiteName)
        {
            CurrentEntity = LandingSiteCollection.FirstOrDefault(n => n.ToString() == landingSiteName);
            return CurrentEntity;
        }
        public LandingSite GetLandingSite(int landingSiteID)
        {
            CurrentEntity = LandingSiteCollection.FirstOrDefault(n => n.LandingSiteID == landingSiteID);
            return CurrentEntity;
        }

        public List<LandingSite> GetAllLandingSites()
        {
            return LandingSiteCollection.ToList();
        }

        private void LandingSiteCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            LandingSite editedLandingSite = new LandingSite();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        editedLandingSite = LandingSiteCollection[newIndex];
                        if (LandingSites.Add(editedLandingSite))
                        {
                            CurrentEntity = editedLandingSite;
                            _editSuccess = true;
                        }

                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<LandingSite> tempListOfRemovedItems = e.OldItems.OfType<LandingSite>().ToList();
                        editedLandingSite = tempListOfRemovedItems[0];
                        _editSuccess = LandingSites.Delete(editedLandingSite.LandingSiteID);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<LandingSite> tempListOfLandingSites = e.NewItems.OfType<LandingSite>().ToList();
                        editedLandingSite = tempListOfLandingSites[0];
                        _editSuccess = LandingSites.Update(editedLandingSite);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
            //EntityChangedEventArgs args = new EntityChangedEventArgs(editedLandingSite.GetType().Name,editedLandingSite);
            //EntityChanged?.Invoke(this, args);
        }

        public bool AddRecordToRepo(LandingSite ls)
        {
            if (ls == null)
                throw new ArgumentNullException("Error: The argument is Null");
            LandingSiteCollection.Add(ls);
            return _editSuccess;
        }
        public bool UpdateRecordInRepo(LandingSite ls)
        {
            if (ls.LandingSiteID == 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < LandingSiteCollection.Count)
            {
                if (LandingSiteCollection[index].LandingSiteID == ls.LandingSiteID)
                {
                    LandingSiteCollection[index] = ls;
                    break;
                }
                index++;
            }
            if (_editSuccess)//&& !EditedLandingSiteIDs.Contains(ls.LandingSiteID))
            {
                //EditedLandingSiteIDs.Add(ls.LandingSiteID);
                NSAPEntities.NSAPRegionViewModel.EditedLandingSite = ls;
            }
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < LandingSiteCollection.Count)
            {
                if (LandingSiteCollection[index].LandingSiteID == id)
                {
                    LandingSiteCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public EntityValidationResult EntityValidated(LandingSite landingSite, bool isNew)
        {
            EntityValidationResult evr = new EntityValidationResult();
            if (landingSite.LandingSiteName.Length < 3)
                evr.AddMessage("Landing site's name must be at least 5 characters long");

            if (landingSite.Municipality == null)
                evr.AddMessage("Municipality cannot be empty");

            if (isNew && LandingSiteNameExists(landingSite))
                evr.AddMessage("Landing site already exists");

            if (landingSite.Latitude != null && ((double)landingSite.Latitude > 21.1 || (double)landingSite.Latitude < 4.5))
            {
                evr.AddMessage("Latitude is out of bounds");
            }

            if (landingSite.Longitude != null && ((double)landingSite.Longitude < 112 || (double)landingSite.Longitude > 126.6))
            {
                evr.AddMessage("Longitude is out of bounds");
            }

            if (landingSite.Longitude != null || landingSite.Latitude != null)
            {
                if (landingSite.Longitude == null || landingSite.Latitude == null)
                    evr.AddMessage("Longitude and latitude must both be filled up or should be both empty");
            }
            return evr;
        }

    }
}