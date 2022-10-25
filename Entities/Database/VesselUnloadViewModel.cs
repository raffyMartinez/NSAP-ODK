using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class VesselUnloadViewModel
    {
        public event EventHandler DatabaseUpdatedEvent;
        public bool EditSucceeded;
        private bool _updateXFormID;
        private static StringBuilder _csv = new StringBuilder();
        private static StringBuilder _csv_1 = new StringBuilder();
        private static StringBuilder _unloadStats_csv = new StringBuilder();
        //private static int _deleted_vu_count;
        //public static event EventHandler<DeleteVesselUnloadFromOrphanEventArg> DeleteVesselUnloadFromOrphanedItem;
        public int CountLandingWithCatchComposition()
        {
            return VesselUnloadCollection.Count(t => t.HasCatchComposition == true);
        }
        public ObservableCollection<VesselUnload> VesselUnloadCollection { get; set; }
        private VesselUnloadRepository VesselUnloads { get; set; }


        //private Dictionary<NSAPRegion,int>CountByRegion
        //{
        //    get
        //    {
        //        NSAPEntities.NSAPRegionViewModel.GetEnumeratorInRegion
        //    }
        //}

        public string GetSector()
        {
            if (VesselUnloadCollection.Count == 0)
            {
                return "n/a";
            }
            else
            {
                HashSet<string> sectors = new HashSet<string>();
                foreach (var item in VesselUnloadCollection)
                {
                    sectors.Add(item.SectorCode);
                }
                if (sectors.Count == 1)
                {
                    return sectors.First();
                }
                else
                {
                    return "both";
                }
            }

        }
        public bool IgnoreCollectionChange { get; set; }
        public static int CurrentIDNumber { get; set; }
        public bool UpdateUnloadStats(VesselUnload vu)
        {
            if (vu.DelayedSave)
            {

                _unloadStats_csv.AppendLine($"{vu.PK},{vu.CountEffortIndicators},{vu.CountGrids},{vu.CountGearSoak},{vu.CountCatchCompositionItems},{vu.CountLengthRows},{vu.CountLenFreqRows},{vu.CountLenWtRows},{vu.CountMaturityRows}");

                return true;
            }
            else
            {
                return VesselUnloads.AddUnloadStats(vu);
            }
        }

        public Task<int> UpdateUnloadStatsAsync()
        {
            return Task.Run(() => UpdateUnloadStats());
        }

        public List<Download_summary> GetDownlodaSummary(List<SummaryItem> downloadedvus, DateTime downloadDate)
        {
            List<Download_summary> dws = new List<Download_summary>();
            var enumerators = downloadedvus.GroupBy(t => t.EnumeratorName).OrderBy(t => t.Key);
            int n = 0;


            foreach (var en in enumerators)
            {

                var enDownloads = en.ToList();

                var gears = enDownloads.ToList().GroupBy(t => t.GearUsedName).OrderBy(t => t.Key);
                foreach (var g in gears)
                {
                    var gl = g.ToList();
                    Download_summary ds = new Download_summary
                    {
                        Enumerator = enDownloads[0].EnumeratorName,
                        Gear = gl[0].GearUsedName,
                        NumberLandings = gl.Count,
                        NumberLandingsWithCatchComposition = gl.Count(t => t.HasCatchComposition == true),
                        EarliestSamplingDate = gl.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        LatestSamplingDate = gl.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DownloadDate = downloadDate,
                        NumberOfTrackedLandings = gl.Count(t => t.IsTracked == true),
                    };
                    dws.Add(ds);
                }



                n++;
            }

            var sorted = dws.OrderBy(t => t.Enumerator).ToList();

            Download_summary s = new Download_summary
            {
                Enumerator = "Grand total",
                NumberLandings = downloadedvus.Count,
                NumberLandingsWithCatchComposition = downloadedvus.Count(t => t.HasCatchComposition == true),
                NumberOfTrackedLandings = downloadedvus.Count(t => t.IsTracked == true)
            };
            sorted.Add(s);
            return sorted;
        }
        public List<Download_summary> GetDownlodaSummary1(List<VesselUnload> downloadedvus, DateTime downloadDate)
        {
            List<Download_summary> dws = new List<Download_summary>();
            var enumerators = downloadedvus.GroupBy(t => t.EnumeratorName).OrderBy(t => t.Key);
            int n = 0;


            foreach (var en in enumerators)
            {

                var enDownloads = en.ToList();

                var gears = enDownloads.ToList().GroupBy(t => t.Parent.GearUsedName).OrderBy(t => t.Key);
                foreach (var g in gears)
                {
                    var gl = g.ToList();
                    Download_summary ds = new Download_summary
                    {
                        Enumerator = enDownloads[0].EnumeratorName,
                        Gear = gl[0].Parent.GearUsedName,
                        NumberLandings = gl.Count,
                        NumberLandingsWithCatchComposition = gl.Count(t => t.HasCatchComposition == true),
                        EarliestSamplingDate = gl.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        LatestSamplingDate = gl.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate,
                        DownloadDate = downloadDate,
                        NumberOfTrackedLandings = gl.Count(t => t.OperationIsTracked == true),
                    };
                    dws.Add(ds);
                }



                n++;
            }

            var sorted = dws.OrderBy(t => t.Enumerator).ToList();

            Download_summary s = new Download_summary
            {
                Enumerator = "Grand total",
                NumberLandings = downloadedvus.Count,
                NumberLandingsWithCatchComposition = downloadedvus.Count(t => t.HasCatchComposition == true),
                NumberOfTrackedLandings = downloadedvus.Count(t => t.OperationIsTracked == true)
            };
            sorted.Add(s);
            return sorted;
        }

        public static void ClearCSV()
        {
            _csv.Clear();
            _csv_1.Clear();
            _unloadStats_csv.Clear();
        }

        //public static VesselUnloadSummary GetSummary()
        //{
        //    return VesselUnloadRepository.GetSummary();
        //}
        public static int CountVesselUnload(bool isTracked = false)
        {
            return VesselUnloadRepository.VesselUnloadCount(isTracked);
        }

        private int UpdateUnloadStats()
        {
            int result = 0;
            ManageUpdateEvent(intent: "start", rowsForUpdating: VesselUnloadCollection.Count);
            foreach (var vu in VesselUnloadCollection)
            {
                vu.CountGrids = NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count(t => t.Parent.PK == vu.PK);
                vu.CountGearSoak = NSAPEntities.GearSoakViewModel.GearSoakCollection.Count(t => t.Parent.PK == vu.PK);
                vu.CountEffortIndicators = NSAPEntities.VesselEffortViewModel.VesselEffortCollection.Count(t => t.Parent.PK == vu.PK);
                if (vu.HasCatchComposition)
                {
                    vu.CountCatchCompositionItems = NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Count(t => t.Parent.PK == vu.PK);
                    foreach (var c in NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Where(t => t.Parent.PK == vu.PK))
                    {
                        vu.CountLenFreqRows += c.ListCatchLenFreq.Count;
                        vu.CountLenWtRows += c.ListCatchLengthWeight.Count;
                        vu.CountLengthRows += c.ListCatchLength.Count;
                        vu.CountMaturityRows += c.ListCatchMaturity.Count;
                    }
                }
                if (result == 1)
                {
                    ManageUpdateEvent(intent: "start updating");
                }

                if (VesselUnloads.AddUnloadStats(vu))
                {
                    result++;
                    ManageUpdateEvent(intent: "row updated", runningCount: result);
                }
            }
            ManageUpdateEvent(intent: "finished");
            return result;
        }
        private void ManageUpdateEvent(string intent, int? round = null, int? runningCount = null, int? rowsForUpdating = null)
        {
            EventHandler h = DatabaseUpdatedEvent;
            if (h != null)
            {
                switch (intent)
                {
                    case "start":
                        var ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            RowsToUpdate = (int)rowsForUpdating
                        };
                        if (round != null)
                        {
                            ev.Round = (int)round;
                        }

                        h(null, ev);
                        break;
                    case "start updating":
                        ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent
                        };
                        h(null, ev);
                        break;
                    case "row updated":
                        ev = new UpdateDatabaseColumnEventArg
                        {
                            Intent = intent,
                            RunningCount = (int)runningCount
                        };
                        h(null, ev);
                        break;
                    case "finished":
                        h(null, new UpdateDatabaseColumnEventArg { Intent = intent });
                        break;
                }

            }
        }
        public Task<int> UpdateHasCatchCompositionColumnsAsync(List<UpdateHasCatchCompositionResultItem> updateItem, int round)
        {
            return Task.Run(() => UpdateHasCatchCompositionColumns(updateItem, round));
        }

        public Task<int> UpdateXFormIdentifierColumnAsync(List<UpdateXFormIdentifierItem> updateItem, int round)
        {
            return Task.Run(() => UpdateXFormIdentifierColumn(updateItem, round));
        }
        private int UpdateXFormIdentifierColumn(List<UpdateXFormIdentifierItem> updateItem, int round)
        {
            ManageUpdateEvent(intent: "start", round: round, rowsForUpdating: updateItem.Count);
            int results = 0;
            foreach (var vu in updateItem)
            {
                if (VesselUnloads.UpdateXFormIdentifierColumn(vu))
                {
                    results++;
                    ManageUpdateEvent(intent: "row updated", runningCount: results);
                }
            }
            ManageUpdateEvent(intent: "finished");
            return results;
        }
        private int UpdateHasCatchCompositionColumns(List<UpdateHasCatchCompositionResultItem> updateItem, int round)
        {
            ManageUpdateEvent(intent: "start", round: round, rowsForUpdating: updateItem.Count);
            int results = 0;
            foreach (var vu in updateItem)
            {
                VesselUnloads.UpdateHasCatchCompositionColumn(vu);
                results++;
                ManageUpdateEvent(intent: "row updated", runningCount: results);
            }
            ManageUpdateEvent(intent: "finished");
            return results;
        }

        public int UpdateHasCatchCompositionColumns()
        {
            int results = 0;
            foreach (var vl in VesselUnloadCollection)
            {
                VesselUnloads.UpdateHasCatchCompositionColumn(vl.ListVesselCatch.Count > 0, vl.PK);
                results++;
            }
            return results;
        }
        public bool HasBSCInCatchComposition(VesselUnload vu)
        {
            return vu.ListVesselCatch.Where(t => t.CatchName == "Portunus pelagicus").FirstOrDefault() != null;
        }
        public List<DateTime> MonthsSampledByEnumerator(NSAPEnumerator enumerator)
        {
            List<DateTime> list = new List<DateTime>();
            foreach (var vu in VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID != null && t.NSAPEnumerator.ID == enumerator.ID)
                .GroupBy(t => t.MonthSampled)
                .OrderBy(t => t.Key)
                .ToList())
            {
                list.Add(vu.Key);
            }
            return list;
        }
        public List<VesselUnload> GetAllVesselUnloadsWithDate(string region, string fishingGround = "", string landingSite = "",
             string enumerator = "", string gear = "", DateTime? dateUploaded = null)
        {
            List<VesselUnload> list = new List<VesselUnload>();

            if (landingSite.Length > 0 && gear.Length > 0 && dateUploaded != null && enumerator.Length > 0)
            {
                list = VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region &&
                    t.Parent.Parent.LandingSiteName == landingSite &&
                    t.Parent.GearUsedName == gear &&
                    t.EnumeratorName == enumerator &&
                    ((DateTime)t.DateAddedToDatabase).Date == ((DateTime)dateUploaded).Date).ToList();
            }
            return list;
        }
        public List<VesselUnload> GetAllVesselUnloads(string region, string fishingGround = "", string landingSite = "")
        {
            if (fishingGround.Length > 0 && landingSite.Length > 0)
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region &&
                    t.Parent.Parent.FishingGround.Name == fishingGround &&
                    t.Parent.Parent.LandingSiteName == landingSite).ToList();
            }
            else if (fishingGround.Length > 0)
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region &&
                    t.Parent.Parent.FishingGround.Name == fishingGround).ToList();
            }
            else
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegion.ShortName == region).ToList();
            }
        }
        public List<VesselUnload> GetAllVesselUnloads(NSAPRegion region, bool sorted = true)
        {
            if (sorted)
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegionID == region.Code)
                    .OrderBy(t => t.SamplingDate)
                    .ToList();
            }
            else
            {
                return VesselUnloadCollection
                    .Where(t => t.Parent.Parent.NSAPRegionID == region.Code)
                    .ToList();
            }
        }

        public List<VesselUnload> GetAllVesselUnloads(NSAPEnumerator enumerator)
        {
            return VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID == enumerator.ID)
                .OrderBy(t => t.SamplingDate)
                .ToList();
        }
        public List<VesselUnload> GetAllVesselUnloads(NSAPEnumerator enumerator, DateTime month)
        {
            return VesselUnloadCollection
                .Where(t => t.NSAPEnumeratorID == enumerator.ID && t.MonthSampled == month)
                .OrderBy(t => t.SamplingDate)
                .ToList();
        }
        public int CountEnumeratorsWithUnloadRecord
        {
            get
            {
                return VesselUnloadCollection.Where(t => t.NSAPEnumeratorID != null).GroupBy(t => t.NSAPEnumeratorID).ToList().Count;
            }
        }

        public static List<VesselUnload> GetVesselUnloads(List<GearUnload> vus)
        {
            List<VesselUnload> vessel_unloads = new List<VesselUnload>();
            foreach (GearUnload vu in vus)
            {
                vu.VesselUnloadViewModel = new VesselUnloadViewModel(vu, updatesubViewModels: true);
                vessel_unloads.AddRange(vu.VesselUnloadViewModel.VesselUnloadCollection.ToList());
            }
            return vessel_unloads;
        }
        public VesselUnloadViewModel(GearUnload parent, bool updatesubViewModels = false)
        {
            VesselUnloads = new VesselUnloadRepository(parent);
            VesselUnloadCollection = new ObservableCollection<VesselUnload>(VesselUnloads.VesselUnloads);
            if (updatesubViewModels)
            {
                foreach (VesselUnload vu in VesselUnloadCollection)
                {
                    if (vu.FishingGroundGridViewModel == null)
                    {
                        vu.FishingGroundGridViewModel = new FishingGroundGridViewModel(vu);
                    }

                    if (vu.VesselCatchViewModel == null)
                    {
                        vu.VesselCatchViewModel = new VesselCatchViewModel(vu);
                    }

                    if (vu.VesselEffortViewModel == null)
                    {
                        vu.VesselEffortViewModel = new VesselEffortViewModel(vu);
                    }

                    if (vu.GearSoakViewModel == null)
                    {
                        vu.GearSoakViewModel = new GearSoakViewModel(vu);
                    }
                }
            }
            VesselUnloadCollection.CollectionChanged += VesselUnloadCollection_CollectionChanged;
        }
        public VesselUnloadViewModel(bool isNew = false)
        {
            VesselUnloads = new VesselUnloadRepository(isNew);
            if (isNew)
            {
                VesselUnloadCollection = new ObservableCollection<VesselUnload>();
            }
            else
            {
                VesselUnloadCollection = new ObservableCollection<VesselUnload>(VesselUnloads.VesselUnloads);
            }
            VesselUnloadCollection.CollectionChanged += VesselUnloadCollection_CollectionChanged;
        }

        public List<VesselUnload> GetSampledLandingsOfVessel(string veselName, string sector, string landingSiteName)
        {
            return VesselUnloadCollection
                .Where(t => t.VesselID == null && t.VesselName == veselName && t.Sector == sector && t.Parent.Parent.LandingSiteName == landingSiteName)
                .ToList();
        }


        public List<VesselUnload> GetUnloadsPastDateUploadLocalDB(DateTime dateUpload)
        {
            return VesselUnloadCollection.Where(t => t.DateAddedToDatabase > dateUpload).ToList();
        }

        public Task<DeleteVesselUnloaResult> DeleteUnloadChildrenAsync(List<VesselUnload> listUnload)
        {
            return Task.Run(() => DeleteUnloadChildren(listUnload));
        }

        private int _countUnloadDeleted;
        public event EventHandler DeleteUnloadChildrenEvent;
        public DeleteVesselUnloaResult DeleteUnloadChildren(List<VesselUnload> listUnload)
        {
            int counter = 0;
            int countUnloadDeleted = 0;
            List<int> pks = new List<int>();

            //int counter = NSAPEntities.VesselCatchViewModel.DeleteCatchFromUnloads(listUnload);

            foreach (var vu in listUnload)
            {
                foreach (var soakvu in vu.ListGearSoak)
                {
                    if (vu.GearSoakViewModel.DeleteRecordFromRepo(soakvu.PK))
                    {
                        counter++;
                    }
                }


                foreach (var gridvu in vu.ListFishingGroundGrid)
                {
                    if (vu.FishingGroundGridViewModel.DeleteRecordFromRepo(gridvu.PK))
                    {
                        counter++;
                    }
                }

                foreach (var effortvu in vu.ListVesselEffort)
                {
                    if (vu.VesselEffortViewModel.DeleteRecordFromRepo(effortvu.PK))
                    {
                        counter++;
                    }
                }

                if (vu.VesselCatchViewModel == null)
                {
                    vu.VesselCatchViewModel = new VesselCatchViewModel(vu);
                }
                counter += vu.VesselCatchViewModel.DeleteCatchFromUnload(vu);

                if (DeleteRecordFromRepo(vu.PK))
                {
                    counter++;
                    countUnloadDeleted++;
                    DeleteUnloadChildrenEvent?.Invoke(this, null);
                    //_deleted_vu_count++;
                    //DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "unload_deleted", DeletedCount = _deleted_vu_count });
                }

            }

            return new DeleteVesselUnloaResult { CountDeleted = counter, VesselUnloadToDeleteCoount = countUnloadDeleted };

        }
        public List<VesselUnload> GetSampledLandings(string enumeratorName, string landingSiteName)
        {
            return VesselUnloadCollection.Where(t => t.NSAPEnumeratorID == null && t.EnumeratorName == enumeratorName && t.Parent.Parent.LandingSiteName == landingSiteName).ToList();
        }
        public List<VesselUnload> GetSampledLandings(string enumeratorName)
        {
            return VesselUnloadCollection.Where(t => t.NSAPEnumeratorID == null && t.EnumeratorName == enumeratorName).ToList();
        }
        public List<LandingSiteSampling> GetLandingSiteSamplings(string enumeratorName)
        {
            var list = new List<LandingSiteSampling>();
            foreach (var vu in VesselUnloadCollection.Where(t => t.EnumeratorName == enumeratorName && t.NSAPEnumeratorID == null))
            {
                if (!list.Contains(vu.Parent.Parent))
                {
                    list.Add(vu.Parent.Parent);
                }
            }

            return list;
        }
        public List<VesselUnload> GetAllVesselUnloads()
        {
            return VesselUnloadCollection.ToList();
        }


        public List<VesselUnload> GetAllVesselUnloads(GearUnload parent, bool includeSimilarDateAndGear = false)
        {
            if (includeSimilarDateAndGear)
            {
                return VesselUnloadCollection.Where(t => t.SamplingDate.Date == parent.Parent.SamplingDate.Date &&
                t.Parent.GearUsedName == parent.GearUsedName &&
                t.Parent.Parent.LandingSiteName == parent.Parent.LandingSiteName).ToList();
            }
            else
            {
                return VesselUnloadCollection.Where(t => t.Parent.PK == parent.PK).ToList();
            }
        }

        public DateTime DateLatestDownload
        {
            get { return (DateTime)VesselUnloadCollection.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase; }
        }
        public List<VesselUnloadTrackedFlattened> GetAllTrackedFlattenedItems()
        {
            return VesselUnloads.getTrackedFlattenedList();
        }

        public string GetSector(string sectorCode)
        {
            return sectorCode == "c" ? "Commercial" : "Municipal";
        }

        public List<VesselUnloadFlattened> GetAllFlattenedItems()
        {
            List<VesselUnloadFlattened> thisList = new List<VesselUnloadFlattened>();
            foreach (var vu in VesselUnloadCollection)
            {
                thisList.Add(new VesselUnloadFlattened(vu));
            }
            return thisList;
        }

        public bool ClearRepository()
        {
            VesselUnloadCollection.Clear();
            return VesselUnloadRepository.ClearTable();
        }

        public DateTime DateOfFirstSampledLanding
        {
            get { return VesselUnloadCollection.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate; }
        }

        public DateTime DateOfLastSampledLanding
        {
            get { return VesselUnloadCollection.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate; }
        }

        public int TrackedUnloadCount
        {
            get { return VesselUnloadCollection.Where(t => t.OperationIsTracked == true).ToList().Count; }
        }

        public VesselUnload getVesselUnload(int pk)
        {
            var vu = VesselUnloadCollection.FirstOrDefault(n => n.PK == pk);
            if (vu != null)
            {
                vu.ContainerViewModel = this;
            }
            return vu;
        }

        private static bool SetCSV(VesselUnload vu)
        {
            string date_submitted = vu.DateTimeSubmitted.ToString();
            string date_added = vu.DateAddedToDatabase.ToString();
            string date_sampled = vu.SamplingDate.ToString();
            string boat_id = string.Empty;
            if (vu.VesselID != null)
            {
                boat_id = ((int)vu.VesselID).ToString();
            }

            string catch_wt = string.Empty;
            if (vu.WeightOfCatch != null)
            {
                catch_wt = ((double)vu.WeightOfCatch).ToString();
            }

            string sample_wt = string.Empty;
            if (vu.WeightOfCatchSample != null)
            {
                sample_wt = ((double)vu.WeightOfCatchSample).ToString();
            }

            string boxes_total = string.Empty;
            if (vu.Boxes != null)
            {
                boxes_total = ((int)vu.Boxes).ToString();
            }

            string boxes_sampled = string.Empty;
            if (vu.BoxesSampled != null)
            {
                boxes_sampled = ((int)vu.BoxesSampled).ToString();
            }

            string raising_factor = string.Empty;
            if (vu.RaisingFactor != null)
            {
                raising_factor = ((double)vu.RaisingFactor).ToString();
            }

            string departure = string.Empty;
            if (vu.DepartureFromLandingSite != null)
            {
                departure = ((DateTime)vu.DepartureFromLandingSite).ToString();
            }

            string arrival = string.Empty;
            if (vu.ArrivalAtLandingSite != null)
            {
                arrival = ((DateTime)vu.ArrivalAtLandingSite).ToString();
            }

            string xFormDate = string.Empty;
            if (vu.XFormDate != null)
            {
                xFormDate = ((DateTime)vu.XFormDate).ToString();
            }

            string enum_id = string.Empty;
            if (vu.NSAPEnumeratorID != null)
            {
                enum_id = ((int)vu.NSAPEnumeratorID).ToString();
            }

            string enum_text = string.Empty;
            if (vu.NSAPEnumeratorID == null)
            {
                enum_text = vu.EnumeratorText;
            }

            string no_fishers = string.Empty;
            if (vu.NumberOfFishers != null)
            {
                no_fishers = ((int)vu.NumberOfFishers).ToString();
            }

            if (Utilities.Global.Settings.UsemySQL)
            {
                if (vu.VesselID == null)
                {
                    boat_id = @"\N";
                }

                if (vu.WeightOfCatch == null)
                {
                    catch_wt = @"\N";
                }

                if (vu.WeightOfCatchSample == null)
                {
                    sample_wt = @"\N";
                }

                if (vu.Boxes == null)
                {
                    boxes_total = @"\N";
                }

                if (vu.BoxesSampled == null)
                {
                    boxes_sampled = @"\N";
                }


                if (vu.RaisingFactor == null)
                {
                    raising_factor = @"\N";
                }



                if (vu.DepartureFromLandingSite == null)
                {
                    departure = @"\N";
                }
                else
                {
                    departure = ((DateTime)vu.DepartureFromLandingSite).ToString("yyyy-MM-dd HH:mm:ss");
                }


                if (vu.ArrivalAtLandingSite == null)
                {
                    arrival = @"\N";
                }
                else
                {
                    arrival = ((DateTime)vu.ArrivalAtLandingSite).ToString("yyyy-MM-dd HH:mm:ss");
                }



                if (vu.XFormDate == null)
                {
                    xFormDate = @"\N";
                }
                else
                {
                    xFormDate = ((DateTime)vu.XFormDate).ToString("yyyy-MM-dd HH:mm:ss");
                }

                if (vu.NSAPEnumeratorID == null)
                {
                    enum_id = @"\N";
                }

                if (vu.NumberOfFishers == null)
                {
                    no_fishers = @"\N";
                }

                date_submitted = vu.DateTimeSubmitted.ToString("yyyy-MM-dd HH:mm:ss");
                date_sampled = vu.SamplingDate.ToString("yyyy-MM-dd HH:mm:ss");
                if (vu.DateAddedToDatabase == null)
                {
                    date_added = @"\N";
                }
                else
                {
                    date_added = ((DateTime)vu.DateAddedToDatabase).ToString("yyyy-MM-dd HH:mm:ss");
                }
            }


            _csv.AppendLine($"{vu.Parent.PK},{vu.PK},{boat_id},{catch_wt},{sample_wt},{boxes_total},{boxes_sampled},\"{vu.VesselText}\",{Convert.ToInt32(vu.IsBoatUsed)},{raising_factor}");
            _csv_1.AppendLine($"{vu.PK},{Convert.ToInt32(vu.OperationIsSuccessful)},{Convert.ToInt32(vu.OperationIsTracked)},{departure},{arrival},\"{vu.SectorCode}\",\"{vu.ODKRowID}\",\"{vu.XFormIdentifier}\",{xFormDate},\"{vu.UserName}\",\"{vu.DeviceID}\",{date_submitted},\"{vu.FormVersion}\",\"{vu.GPSCode}\",{date_sampled},\"{vu.Notes}\",{enum_id},\"{enum_text}\",{date_added},{Convert.ToInt32(vu.FromExcelDownload)},{Convert.ToInt32(vu.FishingTripIsCompleted)},{Convert.ToInt32(vu.HasCatchComposition)},{no_fishers}");



            return true;
        }

        public static string UnloadStatsCSV
        {
            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_vessel_unload_stats")}\r\n{_unloadStats_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_unload_stats")}\r\n{_unloadStats_csv}";
                }
            }
        }
        public static string CSV_1
        {

            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_vessel_unload_1")}\r\n{_csv_1}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_unload_1")}\r\n{_csv_1}";
                }
            }
        }
        public static string CSV
        {

            get
            {
                if (Utilities.Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_vessel_unload")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_vessel_unload")}\r\n{_csv.ToString()}";
                }
            }
        }
        public VesselUnload getVesselUnload(string odkROWID)
        {
            var vu = VesselUnloadCollection.FirstOrDefault(n => n.ODKRowID == odkROWID);
            if (vu != null)
            {
                vu.ContainerViewModel = this;
            }
            return vu;
        }


        private void VesselUnloadCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IgnoreCollectionChange) return;

            EditSucceeded = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        VesselUnload newvu = VesselUnloadCollection[e.NewStartingIndex];
                        if (newvu.DelayedSave)
                        {
                            EditSucceeded = SetCSV(newvu);
                        }
                        else
                        {
                            EditSucceeded = VesselUnloads.Add(newvu);
                        }
                        //int newIndex = e.NewStartingIndex;
                        //EditSucceeded = VesselUnloads.Add(VesselUnloadCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<VesselUnload> tempListOfRemovedvus = e.OldItems.OfType<VesselUnload>().ToList();
                        EditSucceeded = VesselUnloads.Delete(tempListOfRemovedvus[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<VesselUnload> tempList = e.NewItems.OfType<VesselUnload>().ToList();
                        if (_updateXFormID)
                        {
                            EditSucceeded = VesselUnloads.UpdateEx(tempList[0]);
                        }
                        else
                        {
                            EditSucceeded = VesselUnloads.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                        }
                    }
                    break;
            }
        }

        public int Count
        {
            get { return VesselUnloadCollection.Count; }
        }

        public FishingGroundGrid FirstGridLocation(VesselUnload unload)
        {
            FishingGroundGrid grid = null;
            if (unload.ListFishingGroundGrid.Count > 0)
            {
                grid = unload.ListFishingGroundGrid[0];
            }
            return grid;
        }


        public bool AddRecordToRepo(VesselUnload vu)
        {
            if (vu == null)
                throw new ArgumentNullException("Error: The argument is Null");
            VesselUnloadCollection.Add(vu);
            return EditSucceeded;
        }

        public bool UpdateRecordInRepo(VesselUnload vu, bool updateXFormID = false)
        {
            _updateXFormID = updateXFormID;
            int index = 0;
            if (_updateXFormID)
            {
                while (index < VesselUnloadCollection.Count)
                {
                    if (VesselUnloadCollection[index].ODKRowID == vu.ODKRowID)
                    {
                        VesselUnloadCollection[index] = vu;
                        break;
                    }
                    index++;
                }
            }
            else
            {
                if (vu.PK == 0)
                    throw new Exception("Error: ID cannot be zero");


                while (index < VesselUnloadCollection.Count)
                {
                    if (VesselUnloadCollection[index].PK == vu.PK)
                    {
                        VesselUnloadCollection[index] = vu;
                        break;
                    }
                    index++;
                }
            }

            return EditSucceeded;
        }

        public int NextRecordNumber
        {
            get
            {
                if (VesselUnloadCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return VesselUnloads.MaxRecordNumber() + 1;
                }
            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < VesselUnloadCollection.Count)
            {
                if (VesselUnloadCollection[index].PK == id)
                {
                    VesselUnloadCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            if (EditSucceeded)
            {

            }
            return EditSucceeded;
        }
    }
}
