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

        public List<LandingSiteSampling> GetSampledLandings(string enumeratorText, string landingSiteName)
        {
            return LandingSiteSamplingCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == enumeratorText && t.LandingSiteName == landingSiteName).ToList();
        }
        public List<LandingSiteSampling> GetSampledLandings(string enumeratorText)
        {
            return LandingSiteSamplingCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == enumeratorText).ToList();
        }



        public List<VesselUnload> GetVesselUnloads(List<GearUnload> gearUnloads)
        {
            var landings = new List<VesselUnload>();

            foreach (var gu in gearUnloads)
            {
                if (gu.VesselUnloadViewModel == null)
                {
                    gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
                }
                landings.AddRange(gu.VesselUnloadViewModel.VesselUnloadCollection.ToList());
            }

            return landings;
        }
        public List<GearUnload> GetGearUnloads(TreeViewModelControl.AllSamplingEntitiesEventHandler e)
        {
            return GetGearUnloads(e.NSAPRegion.Code, e.FMA.FMAID, e.FishingGround.Code, e.LandingSiteText, (DateTime)e.MonthSampled, e.LandingSite.LandingSiteID);
        }
        public List<GearUnload> GetGearUnloads(string regionCode, int fmaID, string fishingGroundCode, string landingSiteName, DateTime monthSampled, int? landingSiteID = null)
        {
            List<GearUnload> gearUnloads = new List<GearUnload>();
            List<LandingSiteSampling> landingSiteSamplings = null;
            if (landingSiteID != null)
            {
                landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                    .Where(t => t.NSAPRegionID == regionCode &&
                    t.FMAID == fmaID &&
                    t.FishingGroundID == fishingGroundCode &&
                    t.LandingSiteID == (int)landingSiteID &&
                    t.MonthSampled == monthSampled).ToList();
            }
            else
            {
                landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                    .Where(t => t.NSAPRegionID == regionCode &&
                    t.FMAID == fmaID &&
                    t.FishingGroundID == fishingGroundCode &&
                    t.LandingSiteName == landingSiteName &&
                    t.MonthSampled == monthSampled).ToList();
            }
            foreach (var ls in landingSiteSamplings)
            {
                gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.ToList());
            }

            return gearUnloads;
        }

        public VesselUnload GetVesselUnload(SummaryItem si)
        {
            List<LandingSiteSampling> lss = LandingSiteSamplingCollection.Where(t => t.SamplingDate.Date == si.SamplingDate.Date).ToList();
            foreach (LandingSiteSampling ls in lss)
            {
                List<GearUnload> gus = ls.GearUnloadViewModel.GearUnloadCollection.Where(t=>t.GearUsedName==si.GearUsedName) .ToList();
                foreach (GearUnload gu in gus)
                {
                    if(gu.VesselUnloadViewModel==null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
                    }
                    VesselUnload vu = gu.VesselUnloadViewModel.VesselUnloadCollection.ToList().FirstOrDefault(t => t.PK == si.VesselUnloadID);
                    
                    if (vu != null)
                    {
                        vu.SetSubModels();
                        return vu;
                    }
                }
            }
            return null;
        }
        //public List<GearUnload> GetGearUnloads(DateTime date_downloaded)
        //{
        //    List<GearUnload> gus = new List<GearUnload>();
        //    var gu_keys = NSAPEntities.SummaryItemViewModel.GearUnloadPKs(date_downloaded).OrderByDescending(t => t);
        //    foreach (var lss in LandingSiteSamplingCollection
        //        .Where(t => t.SamplingDate.Date <= date_downloaded.Date)
        //        .OrderByDescending(t => t.SamplingDate)
        //        .ToList())
        //    {
        //        gus.AddRange(lss.GearUnloadViewModel.GearUnloadCollection.ToList());
        //    }
        //    return gus;
        //}
        public List<GearUnload> GetGearUnloads(string regionCode, int fmaID, string fishingGroundCode, string landingSiteName, DateTime monthSampled)
        {

            var landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .Where(t => t.NSAPRegionID == regionCode &&
                t.FMAID == fmaID &&
                t.FishingGroundID == fishingGroundCode &&
                t.LandingSiteName == landingSiteName &&
                t.MonthSampled == monthSampled).ToList();

            var gearUnloads = new List<GearUnload>();
            foreach (var ls in landingSiteSamplings)
            {
                gearUnloads.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.ToList());
            }

            return gearUnloads;
        }
        //public List<VesselUnload> GetVesselUnloads(int fmaID, string landingSiteName, DateTime monthSampled)
        //{
        //    var landingSiteSamplings = LandingSiteSamplingCollection
        //            .Where(t => t.FMAID == fmaID &&
        //            t.LandingSiteName == landingSiteName &&
        //            t.MonthSampled == monthSampled).ToList();

        //    var gus = new List<GearUnload>();
        //    foreach (var ls in landingSiteSamplings)
        //    {
        //        gus.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.ToList());
        //    }

        //    var landings = new List<VesselUnload>();

        //    foreach (var gu in gus)
        //    {
        //        if (gu.VesselUnloadViewModel == null)
        //        {
        //            gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
        //        }
        //        landings.AddRange(gu.VesselUnloadViewModel.VesselUnloadCollection.ToList());
        //    }

        //    return landings;
        //}
        public DateTime? LatestEformSubmissionDate
        {
            get
            {
                var list = LandingSiteSamplingCollection.Where(t => t.XFormIdentifier != null && t.XFormIdentifier.Length > 0);
                if (list.Count() > 0)
                {
                    return list.Max(t => t.DateSubmitted).Value;
                }
                else
                {
                    return null;
                }

            }
        }
        public int CountEformSubmissions
        {
            get
            {
                return LandingSiteSamplingCollection.Count(t => t.XFormIdentifier != null && t.XFormIdentifier.Length > 0);
            }
        }


        public List<OrphanedLandingSite> OrphanedLandingSites()
        {
            var items = LandingSiteSamplingCollection
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

        public LandingSiteSampling GetLandingSiteSampling(OrphanedLandingSite ols, LandingSite replacement, DateTime samplingDate)
        {
            List<LandingSiteSampling> samplings = new List<LandingSiteSampling>();

            //if (LandingSiteSamplingCollection.FirstOrDefault(t => t.LandingSiteID == replacement.LandingSiteID) != null)
            //{

            samplings = LandingSiteSamplingCollection.Where(t => t.LandingSiteID != null &&
                                                                 t.FMAID == ols.FMA.FMAID &&
                                                                 t.FishingGround.Code == ols.FishingGround.Code &&
                                                                 t.LandingSite.LandingSiteID == replacement.LandingSiteID &&
                                                                 t.SamplingDate.Date == samplingDate.Date).ToList();
            if (samplings.Count > 0)
            {
                //return samplings.FirstOrDefault();
                return samplings[0];
            }
            else
            {
                return null;
            }
            //}
            //else
            //{
            //    return null;
            //}



        }
        public LandingSiteSamplingViewModel()
        {
            LandingSiteSamplings = new LandingSiteSamplingRepository();
            LandingSiteSamplingCollection = new ObservableCollection<LandingSiteSampling>(LandingSiteSamplings.LandingSiteSamplings);
            LandingSiteSamplingCollection.CollectionChanged += LandingSiteSamplingCollection_CollectionChanged;
        }

        //public List<LandingSiteSampling> GetAllLandingSiteSamplings()
        //{
        //    return LandingSiteSamplingCollection.ToList();
        //}

        //public List<VesselUnload> VesselUnloadsFromDummyGearUnload(GearUnload dummy)
        //{
        //    List<LandingSiteSampling> lss = new List<LandingSiteSampling>();
        //    if (dummy.Parent.LandingSite == null)
        //    {
        //        lss = LandingSiteSamplingCollection.Where(t => t.FishingGroundID == dummy.Parent.FishingGround.Code &&
        //                                                 t.NSAPRegionID == dummy.Parent.NSAPRegion.Code &&
        //                                                 t.LandingSiteText == dummy.Parent.LandingSiteText &&
        //                                                 t.FMAID == dummy.Parent.FMA.FMAID &&
        //                                                 t.SamplingDate == dummy.Parent.SamplingDate).ToList();
        //    }
        //    else
        //    {
        //        lss = LandingSiteSamplingCollection.Where(t => t.FishingGroundID == dummy.Parent.FishingGround.Code &&
        //                                                     t.NSAPRegionID == dummy.Parent.NSAPRegion.Code &&
        //                                                     t.LandingSiteID == dummy.Parent.LandingSite.LandingSiteID &&
        //                                                     t.FMAID == dummy.Parent.FMA.FMAID &&
        //                                                     t.SamplingDate == dummy.Parent.SamplingDate).ToList();
        //    }

        //    GearUnload gu = new GearUnload();
        //    if (lss.Count == 1)
        //    {
        //        foreach (var item in lss[0].GearUnloadViewModel.GearUnloadCollection)
        //        {
        //            if (item.GearUsedName == dummy.GearUsedName)
        //            {
        //                gu = item;
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var ls in lss)
        //        {
        //            foreach (var g in ls.GearUnloadViewModel.GearUnloadCollection)
        //            {
        //                if (g.GearUsedName == dummy.GearUsedName)
        //                {
        //                    gu = g;
        //                    break;
        //                }
        //                if (gu != null)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    if (gu.VesselUnloadViewModel == null)
        //    {
        //        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
        //    }
        //    var vus = gu.VesselUnloadViewModel.VesselUnloadCollection.ToList();
        //    return gu.VesselUnloadViewModel.VesselUnloadCollection.ToList();
        //}

        public LandingSiteSamplingFlattened GetFlattenedItem(int id)
        {
            return new LandingSiteSamplingFlattened(LandingSiteSamplingCollection.Where(t => t.PK == id).FirstOrDefault());
        }

        public void Clear()
        {
            LandingSiteSamplingCollection.Clear();
        }
        public bool ClearRepository()
        {
            LandingSiteSamplingCollection.Clear();
            return LandingSiteSamplingRepository.ClearTable();
        }

        public List<LandingSiteSamplingFlattened> GetAllFlattenedItems()
        {
            List<LandingSiteSamplingFlattened> thisList = new List<LandingSiteSamplingFlattened>();
            foreach (var item in LandingSiteSamplingCollection)
            {
                thisList.Add(new LandingSiteSamplingFlattened(item));
            }
            return thisList;
        }
        //public List<LandingSiteSampling> getLandingSiteSamplings(LandingSite ls, FishingGround fg, DateTime samplingDate)
        //{
        //    return LandingSiteSamplingCollection
        //        .Where(t => t.LandingSiteID == ls.LandingSiteID)
        //        .Where(t => t.FishingGroundID == fg.Code)
        //        .Where(t => t.SamplingDate == samplingDate).ToList();
        //}

        public LandingSiteSampling getLandingSiteSampling(FromJson.VesselLanding landing)
        {
            if (landing.LandingSiteText != null && landing.LandingSiteText.Length > 0)
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.NSAPRegionID==landing.NSAPRegionCode &&
                    t.FMAID==landing.fma_number &&
                    t.LandingSiteName == landing.LandingSiteName &&
                    t.FishingGroundID == landing.FishingGround.Code && 
                    t.SamplingDate.Date == landing.SamplingDate.Date).FirstOrDefault();
            }
            else if (landing.LandingSite == null)
            {
                return null;
            }
            else
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.NSAPRegionID==landing.NSAPRegionCode &&
                    t.FishingGroundID == landing.FishingGround.Code &&
                    t.LandingSiteID == landing.LandingSite.LandingSiteID &&
                    t.SamplingDate.Date == landing.SamplingDate.Date).FirstOrDefault();
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

                    int newIndex = e.NewStartingIndex;
                    EditSuccess = LandingSiteSamplings.Add(LandingSiteSamplingCollection[newIndex]);

                    break;

                case NotifyCollectionChangedAction.Remove:

                    List<LandingSiteSampling> tempListOfRemovedItems = e.OldItems.OfType<LandingSiteSampling>().ToList();
                    if (LandingSiteSamplings.Delete(tempListOfRemovedItems[0].PK))
                    {
                        EditSuccess = true;
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:

                    List<LandingSiteSampling> tempList = e.NewItems.OfType<LandingSiteSampling>().ToList();
                    EditSuccess = LandingSiteSamplings.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only

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

        public bool DeleteRecordFromRepo(LandingSiteSampling s)
        {
            if (s == null)
                throw new Exception("Sampling cannot be null");

            int index = 0;
            while (index < LandingSiteSamplingCollection.Count)
            {
                if (LandingSiteSamplingCollection[index].PK == s.PK)
                {
                    LandingSiteSamplingCollection.RemoveAt(index);
                    break;
                }
                index++;
            }

            return EditSuccess;
        }
    }
}
