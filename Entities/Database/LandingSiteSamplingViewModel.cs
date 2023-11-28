using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSamplingViewModel
    {
        public event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        public event EventHandler<DeleteLandingSiteSamplingFromOrphanEventArg> DeleteOrphanedLandingSiteFromOrphanedItem;
        public event EventHandler<DeleteVesselUnloadFromOrphanEventArg> DeleteVesselUnloadFromOrphanedItem;

        private int _countLandingSiteSamplingsInOrphanedLandingSiteForDelete;
        private int _landingSiteSamplingsInOrphanedLandingSiteForDeleteCounter;
        private List<LandingSiteSampling> _landingSiteSamplings;
        private List<GearUnload> _gearUnloads;
        private static StringBuilder _csv = new StringBuilder();
        private static StringBuilder _csv_1 = new StringBuilder();
        private int _deleted_vu_count = 0;
        private bool _addToCollectionOnly = false;
        public bool EditSuccess { get; set; }
        public ObservableCollection<LandingSiteSampling> LandingSiteSamplingCollection { get; set; }
        private LandingSiteSamplingRepository LandingSiteSamplings { get; set; }

        public bool SetSamplingExistsServer(List<SubmissionIDPair> submissionPairs, out int pairedCount)
        {
            pairedCount = 0;
            if (submissionPairs.Count > 0)
            {
                foreach (var pair in submissionPairs)
                {
                    var item = LandingSiteSamplingCollection.FirstOrDefault(t => t.Submission_id == pair._id);
                    //var item = LandingSiteSamplingCollection.FirstOrDefault(t => t.RowID == pair._uuid);
                    //LandingSiteSamplingCollection.FirstOrDefault(t => t.Submission_id == pair._id).FoundInServer = true;
                    try
                    {
                        item.FoundInServer = true;
                        pairedCount++;
                    }
                    catch
                    {
                        //ignore
                    }

                    
                }
            }
            return pairedCount > 0;
        }
        public int CountLandingSiteSamplingsInOrphanedLandingSiteForDelete
        {
            get { return _countLandingSiteSamplingsInOrphanedLandingSiteForDelete; }
            set
            {
                _landingSiteSamplingsInOrphanedLandingSiteForDeleteCounter = 0;
                _countLandingSiteSamplingsInOrphanedLandingSiteForDelete = value;
                DeleteOrphanedLandingSiteFromOrphanedItem?.Invoke(
                    null,
                    new DeleteLandingSiteSamplingFromOrphanEventArg
                    {
                        LandinggSiteSamplingToDeleteTotalCount = _countLandingSiteSamplingsInOrphanedLandingSiteForDelete,
                        Intent = "start"
                    });
            }
        }

        public Task<List<Day_GearLanded>> GetGearLandingsForDayTask(LandingSite ls, DateTime monthSampled)
        {
            return Task.Run(() => GetGearLandingsForDay(ls, monthSampled));
        }
        private List<Day_GearLanded> GetGearLandingsForDay(LandingSite ls, DateTime monthSampled)
        {
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start build calendar" });
            List<Day_GearLanded> a_list = new List<Day_GearLanded>();
            foreach (LandingSiteSampling lss in LandingSiteSamplingCollection
                .OrderBy(t => t.SamplingDate)
                .Where(t => t.LandingSiteID == ls.LandingSiteID && t.SamplingDate.Date >= monthSampled && t.SamplingDate.Date < monthSampled.AddMonths(1)))
            {
                if (lss.GearsInLandingSite.Count == 0)
                {
                    a_list.Add(new Day_GearLanded { LandingSiteSampling = lss, GearInLandingSite = null });
                }
                else
                {
                    foreach (GearInLandingSite gls in lss.GearsInLandingSite)
                    {
                        a_list.Add(new Day_GearLanded { LandingSiteSampling = lss, GearInLandingSite = gls });
                    }
                }
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "end build calendar" });
            return a_list;
        }
        public List<GearInLandingSite> GetGearsInLandingSiteSampling(LandingSiteSampling lss)
        {
            return LandingSiteSamplings.GetGearsInLandingSite(lss);
        }
        public List<LandingSiteSampling> GetSampledLandings(string enumeratorText, string landingSiteName)
        {
            return LandingSiteSamplingCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == enumeratorText && t.LandingSiteName == landingSiteName).ToList();
        }
        public List<LandingSiteSampling> GetSampledLandings(string enumeratorText)
        {
            return LandingSiteSamplingCollection.Where(t => t.EnumeratorID == null && t.EnumeratorText == enumeratorText).ToList();
        }
        public bool DeleteGearUnloads(List<GearUnload> gearUnloads, LandingSiteSampling parent = null)
        {

            foreach (var gu in gearUnloads)
            {
                if (gu.VesselUnloadViewModel == null)
                {
                    gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, updatesubViewModels: true);
                }
                foreach (var vu in gu.VesselUnloadViewModel.VesselUnloadCollection.ToList())
                {

                    foreach (var gs in vu.GearSoakViewModel.GearSoakCollection.ToList())
                    {
                        vu.GearSoakViewModel.DeleteRecordFromRepo(gs.PK);
                    }



                    foreach (var ve in vu.VesselEffortViewModel.VesselEffortCollection.ToList())
                    {
                        vu.VesselEffortViewModel.DeleteRecordFromRepo(ve.PK);
                    }



                    foreach (var fg in vu.FishingGroundGridViewModel.FishingGroundGridCollection.ToList())
                    {
                        vu.FishingGroundGridViewModel.DeleteRecordFromRepo(fg.PK);
                    }



                    foreach (var fc in vu.VesselCatchViewModel.VesselCatchCollection.ToList())
                    {
                        if (fc.CatchLenFreqViewModel == null)
                        {
                            fc.CatchLenFreqViewModel = new CatchLenFreqViewModel(fc);
                        }
                        foreach (var lf in fc.CatchLenFreqViewModel.CatchLenFreqCollection.ToList())
                        {
                            fc.CatchLenFreqViewModel.DeleteRecordFromRepo(lf.PK);
                        }

                        if (fc.CatchLengthWeightViewModel == null)
                        {
                            fc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(fc);
                        }
                        foreach (var lw in fc.CatchLengthWeightViewModel.CatchLengthWeightCollection.ToList())
                        {
                            fc.CatchLengthWeightViewModel.DeleteRecordFromRepo(lw.PK);
                        }

                        if (fc.CatchLengthViewModel == null)
                        {
                            fc.CatchLengthViewModel = new CatchLengthViewModel(fc);
                        }
                        foreach (var ll in fc.CatchLengthViewModel.CatchLengthCollection.ToList())
                        {
                            fc.CatchLengthViewModel.DeleteRecordFromRepo(ll.PK);
                        }

                        if (fc.CatchMaturityViewModel == null)
                        {
                            fc.CatchMaturityViewModel = new CatchMaturityViewModel(fc);
                        }
                        foreach (var cm in fc.CatchMaturityViewModel.CatchMaturityCollection.ToList())
                        {
                            fc.CatchMaturityViewModel.DeleteRecordFromRepo(cm.PK);
                        }

                        vu.VesselCatchViewModel.DeleteRecordFromRepo(fc.PK);

                    }
                    if (gu.VesselUnloadViewModel.DeleteRecordFromRepo(vu.PK))
                    {
                        _deleted_vu_count++;
                        DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "unload_deleted", DeletedCount = _deleted_vu_count });
                    }
                }
                parent.GearUnloadViewModel.DeleteRecordFromRepo(gu.PK);
                if (parent.GearUnloadViewModel.Count == 0)
                {
                    NSAPEntities.LandingSiteSamplingViewModel.DeleteRecordFromRepo(parent);
                }
            }
            return true;
        }
        private bool DeleteLandingSiteSamplings(List<LandingSiteSampling> landingSiteSamplings)
        {
            foreach (var lss in landingSiteSamplings)
            {
                DeleteGearUnloads(lss.GearUnloadViewModel.GearUnloadCollection.ToList(), lss);
            }
            return true;
        }

        public Task<bool> DeleteLandingSiteSamplingsAsync(List<LandingSiteSampling> landingSiteSamplings)
        {

            return Task.Run(() => DeleteLandingSiteSamplings(landingSiteSamplings));
        }
        public async Task<bool> DeleteOrphanedLandingSites(List<OrphanedLandingSite> orphanedLandingSites)
        {
            _deleted_vu_count = 0;
            DeleteVesselUnloadFromOrphanedItem?.Invoke(
                null,
                new DeleteVesselUnloadFromOrphanEventArg { Intent = "start", VesselUnloadTotalCount = orphanedLandingSites.Sum(t => t.NumberOfLandings) }
            );

            int countDeleted = 0;
            _landingSiteSamplings = new List<LandingSiteSampling>();
            foreach (var ols in orphanedLandingSites)
            {
                _landingSiteSamplings.AddRange(ols.LandingSiteSamplings);
                if (await DeleteLandingSiteSamplingsAsync(_landingSiteSamplings))
                {
                    if (NSAPEntities.SummaryItemViewModel.DeleteOrphanedLandingSite(ols.LandingSiteName))
                    {
                        countDeleted++;
                    }
                }
                _landingSiteSamplings.Clear();
            }

            if (countDeleted > 0)
            {
                DeleteVesselUnloadFromOrphanedItem?.Invoke(
                    null,
                    new DeleteVesselUnloadFromOrphanEventArg { Intent = "done" }
                );
                return true;
            }
            else
            {
                return false;
            }

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
            List<LandingSiteSampling> lss = LandingSiteSamplingCollection.Where(t => t.SamplingDate.Date == ((DateTime)si.SamplingDate).Date).ToList();
            foreach (LandingSiteSampling ls in lss)
            {
                List<GearUnload> gus = ls.GearUnloadViewModel.GearUnloadCollection.Where(t => t.GearUsedName == si.GearUsedName).ToList();
                foreach (GearUnload gu in gus)
                {
                    if (gu.VesselUnloadViewModel == null)
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
            return NSAPEntities.SummaryItemViewModel.GetOrphanedLandingSites();
        }
        public List<OrphanedLandingSite> OrphanedLandingSitesi()
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
        public List<LandingSiteSampling> GetLandingSiteSamplings(DateTime monthSampled, string form_uid)
        {
            List<LandingSiteSampling> list = null;
            list = LandingSiteSamplingCollection.
                Where(
                    t => t.XFormIdentifier == form_uid &&
                    t.SamplingDate >= monthSampled &&
                    t.SamplingDate < monthSampled.AddMonths(1))
                .OrderBy(t => t.SamplingDate)
                .ToList();
            return list;
        }
        public List<LandingSiteSampling> GetLandingSiteSamplings(FMA fma, FishingGround fg, LandingSite ls, DateTime monthSampled)
        {
            return LandingSiteSamplingCollection
                .Where(t => t.FMAID == fma.FMAID &&
                    t.FishingGroundID == fg.Code &&
                    t.LandingSiteID == ls.LandingSiteID &&
                    t.SamplingDate >= monthSampled.Date &&
                    t.SamplingDate < monthSampled.AddMonths(1).Date)
                .ToList();
        }

        public List<LandingSiteSampling> GetLandingSiteSampling(FMA fma, FishingGround fg, LandingSite ls, DateTime samplingDate)
        {
            return LandingSiteSamplingCollection
                .Where(t => t.FMAID == fma.FMAID &&
                    t.FishingGroundID == fg.Code &&
                    t.LandingSiteID == ls.LandingSiteID &&
                    t.SamplingDate == samplingDate)
                .ToList();
        }
        public LandingSiteSampling GetLandingSiteSampling(string samplingUUID)
        {
            return LandingSiteSamplingCollection.FirstOrDefault(t => t.RowID == samplingUUID);
        }
        public LandingSiteSampling GetLandingSiteSampling(OrphanedLandingSite ols, LandingSite replacement, DateTime samplingDate)
        {
            List<LandingSiteSampling> samplings = new List<LandingSiteSampling>();

            samplings = LandingSiteSamplingCollection.Where(t => t.LandingSiteID != null &&
                                                                 t.FMAID == ols.FMA.FMAID &&
                                                                 t.FishingGround.Code == ols.FishingGround.Code &&
                                                                 t.LandingSite.LandingSiteID == replacement.LandingSiteID &&
                                                                 t.SamplingDate.Date == samplingDate.Date).ToList();
            if (samplings.Count > 0)
            {
                return samplings[0];
            }
            else
            {
                return null;
            }
        }
        public LandingSiteSamplingViewModel()
        {
            LandingSiteSamplings = new LandingSiteSamplingRepository();
            LandingSiteSamplingCollection = new ObservableCollection<LandingSiteSampling>(LandingSiteSamplings.LandingSiteSamplings);
            LandingSiteSamplingCollection.CollectionChanged += LandingSiteSamplingCollection_CollectionChanged;
        }

        public LandingSiteSampling CreateInstance(int lss_id)
        {

            var lss = LandingSiteSamplings.Create(lss_id);
            _addToCollectionOnly = true;
            LandingSiteSamplingCollection.Add(lss);
            _addToCollectionOnly = false;
            return lss;
        }

        public LandingSiteSamplingFlattened GetFlattenedItem(int id)
        {
            return new LandingSiteSamplingFlattened(LandingSiteSamplingCollection.Where(t => t.PK == id).FirstOrDefault());
        }

        public void Clear()
        {
            LandingSiteSamplingCollection.Clear();
        }
        public static void ClearCSV()
        {

            _csv.Clear();
            _csv_1.Clear();
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

        public LandingSiteSampling GetLandingSiteSampling(VesselLanding landing)
        {
            if (landing.FishingGround == null)
            {
                return null;
            }
            else if (landing.LandingSiteText != null && landing.LandingSiteText.Length > 0)
            {
                return LandingSiteSamplingCollection
                    .Where(t => t.NSAPRegionID == landing.NSAPRegionCode &&
                    t.FMAID == landing.fma_number &&
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
                    .Where(t => t.NSAPRegionID == landing.NSAPRegionCode &&
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


        public LandingSiteSampling GetLandingSiteSampling(int pk)
        {
            //var lss = LandingSiteSamplingCollection.ToList().FirstOrDefault(n => n.PK == pk);
            try
            {

                var lss = LandingSiteSamplingCollection.ToList().FirstOrDefault(n => n.PK == pk);
                if (lss == null)
                {

                }
                return lss;
            }
            catch { return null; }
        }



        private static bool SetCSV(LandingSiteSampling item)
        {
            string ls_id = "";
            string ls_text = "";
            string sampling_date = item.SamplingDate.Date.ToString();
            string date_submitted = item.DateSubmitted.ToString();
            string date_added = item.DateAdded.ToString();
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(item.LandingSiteText) && item.LandingSiteID == null)
            {
                ls_text = item.LandingSiteText.Replace("»", ",");
            }

            if (item.LandingSiteID != null)
            {
                ls_id = ((int)item.LandingSiteID).ToString();
            }

            if (Utilities.Global.Settings.UsemySQL)
            {
                if (item.LandingSiteID == null)
                {
                    ls_id = @"\N";
                }

                sampling_date = item.SamplingDate.Date.ToString("yyyy-MM-dd HH:mm:ss");
                if (item.DateSubmitted == null)
                {
                    date_submitted = @"\N";
                }
                else
                {
                    date_submitted = ((DateTime)item.DateSubmitted).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (item.DateAdded == null)
                {
                    date_added = @"\N";
                }
                else
                {
                    date_added = ((DateTime)item.DateAdded).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }


            string enum_id = "";
            if (item.EnumeratorID != null)
            {
                enum_id = ((int)item.EnumeratorID).ToString();
            }
            else if (Utilities.Global.Settings.UsemySQL && item.EnumeratorID == null)
            {
                enum_id = @"\N";
            }

            string number_landings = "";
            string number_landings_sampled = "";
            string number_gear_types = "";
            string submission_id = "";
            if (Utilities.Global.Settings.UsemySQL)
            {
                number_landings = @"\N";
                number_landings_sampled = @"\N";
                number_gear_types = @"\N";
                submission_id = @"\N";
            }

            if (item.IsMultiVessel)
            {
                if (item.NumberOfLandings != null)
                {
                    number_landings = ((int)item.NumberOfLandings).ToString();
                }
                if (item.NumberOfLandingsSampled != null)
                {
                    number_landings_sampled = ((int)item.NumberOfLandingsSampled).ToString();
                }
                if (item.NumberOfGearTypesInLandingSite != null)
                {
                    number_gear_types = ((int)item.NumberOfGearTypesInLandingSite).ToString();
                }
                if (item.Submission_id != null)
                {
                    submission_id = ((int)item.Submission_id).ToString();
                }
            }


            if (Utilities.Global.Settings.UsemySQL)
            {
                _csv.AppendLine($"{item.PK},\"{item.NSAPRegionID}\",{item.FMAID},{sampling_date},{ls_id},\"{item.FishingGroundID}\",\"{item.Remarks}\",{Convert.ToInt32(item.IsSamplingDay)},\"{ls_text}\"");

            }
            else
            {

                myDict.Add("unload_day_id", item.PK.ToString());
                myDict.Add("region_id", item.NSAPRegionID);
                myDict.Add("sdate", sampling_date.ToString());
                myDict.Add("land_ctr_id", ls_id.ToString());
                myDict.Add("ground_id", item.FishingGroundID);
                myDict.Add("remarks", item.Remarks);
                myDict.Add("sampleday", item.IsSamplingDay.ToString());
                myDict.Add("land_ctr_text", ls_text);
                myDict.Add("fma", item.FMAID.ToString());
                myDict.Add("has_fishing_operation", item.HasFishingOperation.ToString());

                _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_LC_FG_sample_day"));

            }

            myDict.Clear();
            myDict.Add("unload_day_id", item.PK.ToString());
            myDict.Add("datetime_submitted", date_submitted);
            myDict.Add("user_name", item.UserName);
            myDict.Add("device_id", item.DeviceID);
            myDict.Add("XFormIdentifier", item.XFormIdentifier);
            myDict.Add("DateAdded", date_added);
            myDict.Add("FromExcelDownload", item.FromExcelDownload.ToString());
            myDict.Add("form_version", item.FormVersion);
            myDict.Add("RowID", item.RowID);
            myDict.Add("EnumeratorID", enum_id);
            myDict.Add("EnumeratorText", item.EnumeratorText);

            myDict.Add("is_multivessel", item.IsMultiVessel.ToString());

            myDict.Add("number_landings_sampled", number_landings_sampled);
            myDict.Add("number_landings", number_landings);
            myDict.Add("number_gear_types_in_landingsite", number_gear_types);
            myDict.Add("json_filename", item.JSONFileName);
            myDict.Add("can_sample_from_catch_composition", item.SamplingFromCatchCompositionIsAllowed.ToString());
            myDict.Add("submission_id", submission_id);


            _csv_1.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_LC_FG_sample_day_1"));

            return true;
        }

        public static string CSV_1
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_LC_FG_sample_day_1")}\r\n{_csv_1.ToString()}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_LC_FG_sample_day_1")}\r\n{_csv_1.ToString()}";
                }
            }

        }
        public static string CSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_LC_FG_sample_day")}\r\n{_csv.ToString()}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_LC_FG_sample_day")}\r\n{_csv.ToString()}";
                }
            }

        }

        private void LandingSiteSamplingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (!_addToCollectionOnly)
                    {
                        LandingSiteSampling newItem = LandingSiteSamplingCollection[e.NewStartingIndex];

                        if (!newItem.DelayedSave)
                        {
                            EditSuccess = LandingSiteSamplings.Add(newItem);
                        }
                        else
                        {
                            EditSuccess = SetCSV(newItem);
                        }
                    }
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
            if(EditSuccess)
            {
                item.FoundInServer = false;
            }
            return EditSuccess;
        }
        public static int CurrentIDNumber { get; set; }
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

                    //var m = LandingSiteSamplingCollection.Max(t => t.PK);
                    //return m + 1;
                    //return LandingSiteSamplings.MaxRecordNumber() + 1;
                    return LandingSiteSamplingCollection.Max(t => t.PK) + 1;
                }
            }
        }
        public Task<bool> DeleteRecordFromRepoAsync(LandingSiteSampling s)
        {
            return Task.Run(() => DeleteRecordFromRepo(s));
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
            if (EditSuccess)
            {
                DeleteOrphanedLandingSiteFromOrphanedItem?.Invoke(
                        null,
                        new DeleteLandingSiteSamplingFromOrphanEventArg
                        {
                            CountDeleted = ++_landingSiteSamplingsInOrphanedLandingSiteForDeleteCounter,
                            SamplingDeleted = s.ToString(),
                            Intent = "deleted orphaned landingSiteSampling"
                        });


                if (_countLandingSiteSamplingsInOrphanedLandingSiteForDelete == _landingSiteSamplingsInOrphanedLandingSiteForDeleteCounter)
                {
                    DeleteOrphanedLandingSiteFromOrphanedItem?.Invoke(
                            null,
                            new DeleteLandingSiteSamplingFromOrphanEventArg
                            {
                                Intent = "finished deleting"
                            });
                }
            }
            return EditSuccess;
        }
    }
}
