using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSamplingViewModel
    {
        public bool EditSuccess { get; set; }
        public ObservableCollection<LandingSiteSampling> LandingSiteSamplingCollection { get; set; }
        private LandingSiteSamplingRepository LandingSiteSamplings { get; set; }

        public DateTime LatestEformSubmissionDate
        {
            get
            {
                return LandingSiteSamplingCollection.Where(t => t.XFormIdentifier.Length > 0).Max(t => t.DateSubmitted).Value;
            }
        }
        public int CountEformSubmissions
        {
            get
            {
                return LandingSiteSamplingCollection.Count(t => t.XFormIdentifier.Length > 0);
            }
        }
        public List<OrphanedLandingSite> OrphanedLandingSites()
        {
            var items= LandingSiteSamplingCollection
                .Where(t => t.LandingSiteID == null)
                .OrderBy(t => t.LandingSiteName)
                .GroupBy(t => t.LandingSiteName).ToList();

            var list = new List<OrphanedLandingSite>();
            foreach (var item in items)
            {

                var orphan = new OrphanedLandingSite
                {
                    LandingSiteName = item.Key,
                    LandingSiteSamplings = LandingSiteSamplingCollection.Where(t => t.LandingSiteName == item.Key).ToList()
                };
                list.Add(orphan);
            }

            return list;

        }
        public LandingSiteSamplingViewModel()
        {
            LandingSiteSamplings = new LandingSiteSamplingRepository();
            LandingSiteSamplingCollection = new ObservableCollection<LandingSiteSampling>(LandingSiteSamplings.LandingSiteSamplings);
            LandingSiteSamplingCollection.CollectionChanged += LandingSiteSamplingCollection_CollectionChanged;
        }

        public List<LandingSiteSampling> GetAllLandingSiteSamplings()
        {
            return LandingSiteSamplingCollection.ToList();
        }

        public LandingSiteSamplingFlattened GetFlattenedItem(int id)
        {
            return new LandingSiteSamplingFlattened(LandingSiteSamplingCollection.Where(t => t.PK == id).FirstOrDefault());
        }
        public bool ClearRepository()
        {
            LandingSiteSamplingCollection.Clear();
            return LandingSiteSamplings.ClearTable();
        }

        public List<LandingSiteSamplingFlattened>GetAllFlattenedItems()
        {
            List<LandingSiteSamplingFlattened> thisList = new List<LandingSiteSamplingFlattened>();
            foreach (var item in LandingSiteSamplingCollection)
            {
                thisList.Add(new LandingSiteSamplingFlattened(item));
            }
            return thisList;
        }
        public List<LandingSiteSampling> getLandingSiteSamplings(LandingSite ls, FishingGround fg, DateTime samplingDate)
        {
            return LandingSiteSamplingCollection
                .Where(t => t.LandingSiteID == ls.LandingSiteID)
                .Where(t => t.FishingGroundID == fg.Code)
                .Where(t => t.SamplingDate == samplingDate).ToList();
        }

        public LandingSiteSampling getLandingSiteSampling(FromJson.VesselLanding landing)
        {
            if(landing.LandingSiteText!=null &&   landing.LandingSiteText.Length>0)
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.LandingSiteText == landing.LandingSiteText)
                    .Where(t => t.FishingGroundID ==  landing.FishingGround.Code)
                    .Where(t => t.SamplingDate.Date == landing.SamplingDate.Date).FirstOrDefault();
            }
            else
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.LandingSiteID == landing.LandingSite.LandingSiteID)
                    .Where(t => t.FishingGroundID == landing.FishingGround.Code)
                    .Where(t => t.SamplingDate.Date == landing.SamplingDate.Date).FirstOrDefault();
            }
        }

        public LandingSiteSampling getLandingSiteSampling(ExcelMainSheet sheet)
        {
            if (sheet.LandingSiteText != null)
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.LandingSiteText == sheet.LandingSiteText)
                    .Where(t => t.FishingGroundID == sheet.NSAPRegionFMAFishingGround.FishingGround.Code)
                    .Where(t => t.SamplingDate == sheet.SamplingDate).FirstOrDefault();
            }
            else
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.LandingSiteID == sheet.NSAPRegionFMAFishingGroundLandingSite.LandingSite.LandingSiteID)
                    .Where(t => t.FishingGroundID == sheet.NSAPRegionFMAFishingGround.FishingGround.Code)
                    .Where(t => t.SamplingDate.Date == sheet.SamplingDate.Date).FirstOrDefault();
            }
        }


        public LandingSiteSampling getLandingSiteSampling(int pk)
        {
            return LandingSiteSamplingCollection.FirstOrDefault(n => n.PK == pk);
        }


        private void LandingSiteSamplingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        EditSuccess= LandingSiteSamplings.Add(LandingSiteSamplingCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<LandingSiteSampling> tempListOfRemovedItems = e.OldItems.OfType<LandingSiteSampling>().ToList();
                        LandingSiteSamplings.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<LandingSiteSampling> tempList = e.NewItems.OfType<LandingSiteSampling>().ToList();
                        EditSuccess = LandingSiteSamplings.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return LandingSiteSamplingCollection.Count; }
        }

        public bool AddRecordToRepo(LandingSiteSampling item)
        {

            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            LandingSiteSamplingCollection.Add(item);
            return EditSuccess;
        }

        public bool UpdateRecordInRepo(LandingSiteSampling item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < LandingSiteSamplingCollection.Count)
            {
                if (LandingSiteSamplingCollection[index].PK == item.PK)
                {
                    LandingSiteSamplingCollection[index] = item;
                    break;
                }
                index++;
            }
            return EditSuccess;
        }

        public int NextRecordNumber
        {
            get
            {
                if (LandingSiteSamplingCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return LandingSiteSamplings.MaxRecordNumber() + 1;
                }
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < LandingSiteSamplingCollection.Count)
            {
                if (LandingSiteSamplingCollection[index].PK == id)
                {
                    LandingSiteSamplingCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
