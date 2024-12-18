﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    public class GearUnloadViewModel
    {
        private static int _deleted_vu_count = 0;
        public static event EventHandler<ProcessingItemsEventArg> ProcessingItemsEvent;
        public static event EventHandler<DeleteVesselUnloadFromOrphanEventArg> DeleteVesselUnloadFromOrphanedItem;
        public bool EditSuccess;
        public ObservableCollection<GearUnload> GearUnloadCollection { get; set; }
        private GearUnloadRepository GearUnloads { get; set; }
        private static StringBuilder _csv = new StringBuilder();
        private static StringBuilder _temp_csv = new StringBuilder();
        private bool _addToCollectionOnly;

        public GearUnloadViewModel()
        {
            GearUnloads = new GearUnloadRepository(fetch: false);
            GearUnloadCollection = new ObservableCollection<GearUnload>(GearUnloads.GearUnloads);
            GearUnloadCollection.CollectionChanged += GearUnloadCollection_CollectionChanged;
        }
        public GearUnloadViewModel(LandingSiteSampling parent)
        {
            GearUnloads = new GearUnloadRepository(parent);
            GearUnloadCollection = new ObservableCollection<GearUnload>(GearUnloads.GearUnloads);
            GearUnloadCollection.CollectionChanged += GearUnloadCollection_CollectionChanged;
        }
        public static void MarkVesselUnloadsWithWatchedSpecies(string watchedSpeciesName, List<GearUnload> gearUnloads)
        {

        }

        public static async Task<List<VesselUnload>> GetVesselUnloadsFromGearUnloadsAsync(List<GearUnload> gear_unloads)
        {
            return await Task.Run(() => GetVesselUnloadsFromGearUnloads(gear_unloads));
        }
        public static List<VesselUnload> GetVesselUnloadsFromGearUnloads(List<GearUnload> gear_unloads)
        {
            List<VesselUnload> vus = new List<VesselUnload>();
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "start" });
            foreach (GearUnload gu in gear_unloads)
            {
                if (gu.VesselUnloadViewModel == null)
                {
                    gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
                }
                vus.AddRange(gu.VesselUnloadViewModel.VesselUnloadCollection.ToList());
            }
            ProcessingItemsEvent?.Invoke(null, new ProcessingItemsEventArg { Intent = "end" });
            return vus;
        }
        public static int CurrentIDNumber { get; set; }
        public static async Task<bool> DeleteVesselUnloads(List<OrphanedFishingGear> ofg)
        {
            _deleted_vu_count = 0;
            int landingsCount = 0;
            //DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "searching" });
            foreach (var item in ofg)
            {
                landingsCount += item.GearUnloads.Sum(t => t.ListVesselUnload.Count);
                //DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "searching" });
            }

            DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "start", VesselUnloadTotalCount = landingsCount, NSAPEntity = NSAPEntity.FishingGear });

            int countDeleted = 0;
            foreach (var item in ofg)
            {
                int deletedCount = 0;
                foreach (var gu in item.GearUnloads)
                {
                    if (gu.VesselUnloadViewModel == null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, updatesubViewModels: true);
                    }
                    foreach (var vu in gu.VesselUnloadViewModel.VesselUnloadCollection.ToList())
                    {
                        List<VesselUnload> lvu = new List<VesselUnload>();
                        lvu.Add(vu);
                        var result = await gu.VesselUnloadViewModel.DeleteUnloadChildrenAsync(lvu);
                        if (result.CountDeleted > 0)
                        {
                            var getvu = gu.VesselUnloadViewModel.getVesselUnload(vu.PK);
                            if (getvu == null || (getvu != null && gu.VesselUnloadViewModel.DeleteRecordFromRepo(vu.PK)))
                            {
                                if (getvu == null)
                                {
                                    deletedCount++;
                                }
                                else if (gu.Parent.GearUnloadViewModel.DeleteRecordFromRepo(gu.PK))
                                {
                                    deletedCount++;
                                }
                            }
                            DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "unload_deleted", DeletedCount = ++_deleted_vu_count });
                        }

                    }

                    if (deletedCount > 0 && NSAPEntities.SummaryItemViewModel.DeleteOrphanedFishingGears(item.Name))
                    {
                        countDeleted++;
                    }

                    if (gu.VesselUnloadViewModel.Count == 0)
                    {
                        if (gu.Parent.GearUnloadViewModel.DeleteRecordFromRepo(gu.PK))
                        {
                            deletedCount++;
                        }
                    }

                    if (gu.Parent.GearUnloadViewModel.Count == 0)
                    {

                    }
                }
            }
            DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "done" });
            return countDeleted > 0;
        }
        public static async Task<bool> DeleteVesselUnloads(List<OrphanedEnumerator> ols)
        {
            _deleted_vu_count = 0;
            int countDeleted = 0;
            int landingsCount = 0;

            foreach (var item in ols)
            {
                landingsCount += item.SampledLandings.Count;
            }
            DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "start", VesselUnloadTotalCount = landingsCount, NSAPEntity = NSAPEntity.Enumerator });

            //List<VesselUnload> vesselUnloads = new List<VesselUnload>();
            foreach (var item in ols)
            {
                int deletedCount = 0;
                //vesselUnloads.AddRange(item.SampledLandings);

                foreach (var vu in item.SampledLandings)
                {
                    if (vu.Parent == null)
                    {
                        vu.Parent = NSAPEntities.SummaryItemViewModel.GetGearUnload(vu.GearUnloadID);
                    }
                    GearUnload gu = vu.Parent;
                    if (gu.VesselUnloadViewModel == null)
                    {
                        gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu, updatesubViewModels: true);
                    }
                    List<VesselUnload> lvu = new List<VesselUnload>();
                    lvu.Add(vu);
                    var result = await gu.VesselUnloadViewModel.DeleteUnloadChildrenAsync(lvu);
                    if (result.CountDeleted > 0)
                    {

                        //if ((gu.VesselUnloadViewModel.getVesselUnload(vu.PK) != null && gu.VesselUnloadViewModel.DeleteRecordFromRepo(vu.PK)) && gu.VesselUnloadViewModel.Count == 0)
                        var getvu = gu.VesselUnloadViewModel.getVesselUnload(vu.PK);
                        if (getvu == null || (getvu != null && gu.VesselUnloadViewModel.DeleteRecordFromRepo(vu.PK)))
                        {
                            if (getvu == null)
                            {
                                deletedCount++;
                            }
                            else if (gu.Parent.GearUnloadViewModel.DeleteRecordFromRepo(gu.PK))
                            {
                                deletedCount++;
                            }
                        }

                        DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "unload_deleted", DeletedCount = ++_deleted_vu_count });
                    }
                }

                if (deletedCount > 0 && NSAPEntities.SummaryItemViewModel.DeleteOrphanedEnumeratorItems(item.Name))
                {
                    countDeleted++;
                }



            }

            DeleteVesselUnloadFromOrphanedItem?.Invoke(null, new DeleteVesselUnloadFromOrphanEventArg { Intent = "done" });
            return countDeleted > 0;
        }
        public static GearUnload GearUnloadFromID(int unloadID)
        {
            foreach (LandingSiteSampling lss in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection)
            {
                foreach (GearUnload gu in lss.GearUnloadViewModel.GearUnloadCollection)
                {
                    if (gu.PK == unloadID)
                    {
                        return gu;
                    }
                }
            }
            return null;
        }
        public static int GearUnloadCount(bool isCompleted = false)
        {
            return GearUnloadRepository.GearUnloadCount(isCompleted);
        }
        //public GearUnloadViewModel()
        //{
        //    GearUnloads = new GearUnloadRepository();
        //    GearUnloadCollection = new ObservableCollection<GearUnload>(GearUnloads.GearUnloads);
        //    GearUnloadCollection.CollectionChanged += GearUnloadCollection_CollectionChanged;
        //}
        private static bool SetCSV(GearUnload item)
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            string boat_ct = "";
            string catch_wt = "";
            string municipal_catch_wt = "";
            string commercial_catch_wt = "";
            string gr_id = "";
            string gr_text = item.GearUsedText;
            try
            {
                if (!string.IsNullOrEmpty(item.GearID))
                {
                    gr_id = "\"{item.GearID}\"";
                    gr_text = "";
                }
                if (item.WeightOfCommercialLandings != null)
                {
                    commercial_catch_wt = ((double)item.WeightOfCommercialLandings).ToString();
                }
                if (item.WeightOfMunicipalLandings != null)
                {
                    municipal_catch_wt = ((double)item.WeightOfMunicipalLandings).ToString();
                }
                if (item.Boats != null)
                {
                    boat_ct = ((int)item.Boats).ToString();
                }

                if (item.Catch != null)
                {
                    catch_wt = ((double)item.Catch).ToString();
                }

                if (Global.Settings.UsemySQL)
                {
                    if (item.Boats == null)
                    {
                        boat_ct = @"\N";
                    }

                    if (item.Catch == null)
                    {
                        catch_wt = @"\N";
                    }
                }

                string gear_sequence = "";
                if (Global.Settings.UsemySQL)
                {
                    gear_sequence = @"\N";
                }

                if (item.Parent.IsMultiVessel && item.Sequence != null)
                {
                    gear_sequence = ((int)item.Sequence).ToString();
                }

                myDict.Add("unload_gr_id", item.PK.ToString());
                myDict.Add("unload_day_id", item.Parent.PK.ToString());
                myDict.Add("gr_id", item.GearID);
                myDict.Add("boats", boat_ct);
                myDict.Add("catch", catch_wt);
                myDict.Add("gr_text", gr_text);
                myDict.Add("remarks", item.Remarks);
                myDict.Add("sp_twsp_count", item.SpeciesWithTWSpCount.ToString());
                myDict.Add("sector", item.SectorCode);
                myDict.Add("gear_sequence", gear_sequence);
                myDict.Add("gear_count_municipal", item.NumberOfMunicipalLandings.ToString());
                myDict.Add("gear_count_commercial", item.NumberOfCommercialLandings.ToString());
                myDict.Add("gear_catch_municipal", municipal_catch_wt);
                myDict.Add("gear_catch_commercial", commercial_catch_wt);

                _csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "dbo_gear_unload"));


                myDict.Clear();
                if (item.Parent.IsMultiVessel)
                {
                    if (item.NumberOfMunicipalLandings > 0)
                    {
                        myDict.Add("unload_gr_id", item.PK.ToString());
                        myDict.Add("unload_day_id", item.Parent.PK.ToString());
                        myDict.Add("sector", "m");
                        myDict.Add("boats", item.NumberOfMunicipalLandings.ToString());
                        myDict.Add("catch", municipal_catch_wt);
                        myDict.Add("gr_id", item.GearID);
                        myDict.Add("gr_text", gr_text);
                        myDict.Add("remarks", item.Remarks);

                        _temp_csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "temp_GearUnload"));
                    }

                    myDict.Clear();
                    if (item.NumberOfCommercialLandings > 0)
                    {
                        myDict.Add("unload_gr_id", item.PK.ToString());
                        myDict.Add("unload_day_id", item.Parent.PK.ToString());
                        myDict.Add("sector", "c");
                        myDict.Add("boats", item.NumberOfCommercialLandings.ToString());
                        myDict.Add("catch", commercial_catch_wt);
                        myDict.Add("gr_id", item.GearID);
                        myDict.Add("gr_text", gr_text);
                        myDict.Add("remarks", item.Remarks);

                        _temp_csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "temp_GearUnload"));
                    }
                }
                else
                {

                    myDict.Add("unload_gr_id", item.PK.ToString());
                    myDict.Add("unload_day_id", item.Parent.PK.ToString());
                    myDict.Add("sector", item.SectorCode == "m" ? "m" : item.SectorCode == "c" ? "c" : "");
                    myDict.Add("boats", (item.Boats ?? 0).ToString());
                    myDict.Add("catch", (item.Catch ?? 0).ToString());
                    myDict.Add("gr_id", item.GearID);
                    myDict.Add("gr_text", gr_text);
                    myDict.Add("remarks", item.Remarks);

                    _temp_csv.AppendLine(CreateTablesInAccess.CSVFromObjectDataDictionary(myDict, "temp_GearUnload"));

                }
                //_csv.AppendLine($"{item.PK},{item.Parent.PK},{gr_id},{boat_ct},{catch_wt},\"{gr_text}\",\"{item.Remarks}\",{item.SpeciesWithTWSpCount},\"{item.SectorCode}\"");


            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return true;
        }

        public static async Task<List<GearUnload>> GetGearUnloadsForCalendar(LandingSite ls, DateTime month_year)
        {
            return await GearUnloadRepository.GearUnloadsForCalendarTask(ls, month_year);
        }
        public static async Task<List<GearUnload>> GetTotalNumberLandingsPerDayCalendar(LandingSite ls, DateTime month_year)
        {
            return await GearUnloadRepository.NumberOfDailyLandingsForCalendarTask(ls, month_year);
        }
        public static string TempCSV
        {
            get
            {
                if (Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_gear_unload")}\r\n{_csv}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("temp_GearUnload")}\r\n{_temp_csv}";
                }
            }
        }
        public static string CSV
        {
            get
            {
                if (Global.Settings.UsemySQL)
                {
                    return $"{NSAPMysql.MySQLConnect.GetColumnNamesCSV("dbo_gear_unload")}\r\n{_csv.ToString()}";
                }
                else
                {
                    return $"{CreateTablesInAccess.GetColumnNamesCSV("dbo_gear_unload")}\r\n{_csv.ToString()}";
                }
            }

        }
        public List<OrphanedFishingGear> OrphanedFishingGears()
        {
            //var items = GearUnloadCollection
            //    .Where(t => t.GearID!=null && t.GearID.Length == 0 && t.GearUsedText!=null && t.GearUsedText.Length>0)
            //    .OrderBy(t => t.GearUsedText)
            //    .GroupBy(t => t.GearUsedText).ToList();

            var items = GearUnloadCollection
                .Where(t => (t.GearID == null || t.GearID.Length == 0) && t.GearUsedText != null && t.GearUsedText.Length > 0)
                .OrderBy(t => t.GearUsedText)
                .GroupBy(t => t.GearUsedText).ToList();

            var list = new List<OrphanedFishingGear>();
            foreach (var item in items)
            {

                var orphan = new OrphanedFishingGear
                {
                    Name = item.Key,
                    GearUnloads = GearUnloadCollection.Where(t => t.GearUsedText == item.Key).ToList()
                };
                list.Add(orphan);
            }

            return list;

        }

        public List<GearUnload> GearUnloadsWithNoBoatCountAndChildVesselUnoad()
        {
            List<GearUnload> list = new List<GearUnload>();
            foreach (var gu in GearUnloadCollection.Where(t => t.Boats == null && t.Catch == null))
            {
                if (NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(gu) == null)
                {
                    list.Add(gu);
                }
            }
            return list;
        }

        public static void ClearCSV()
        {

            _csv.Clear();
        }

        public int CountCompletedGearUnload
        {
            //get { return GearUnloadCollection.Where(t => t.Boats != null).Where(t => t.Catch != null).Count(); }
            get { return GearUnloadCollection.Count(t => (t.Catch != null && t.Boats != null)); }
        }
        public List<GearUnload> GetAllGearUnloads()
        {
            return GearUnloadCollection.ToList();
        }

        public int FixGearUnload()
        {
            int count = 0;
            foreach (var gu in GearUnloadCollection
                .Where(t => t.Parent.LandingSite != null && t.Parent.XFormIdentifier != null &&
                t.Parent.XFormIdentifier.Length > 0 && (t.Boats != null || t.Catch != null))
                .OrderByDescending(t => t.Parent.DateAdded))
            {

                var otherUnload = getOtherGearUnload(gu);
                if (otherUnload != null)
                {
                    otherUnload.Boats = gu.Boats;
                    otherUnload.Catch = gu.Catch;
                    if (UpdateRecordInRepo(otherUnload))
                    {
                        gu.Boats = null;
                        gu.Catch = null;
                        if (UpdateRecordInRepo(gu))
                        {
                            count++;
                        }
                    }
                }
            }
            return count;

        }

        /// <summary>
        /// Returns a list of gear unload whose parent is the input landing site sampling
        /// </summary>
        /// <param name="parentSampling"></param>
        /// <returns></returns>
        public List<GearUnload> GetGearUnloads(LandingSiteSampling parentSampling)
        {
            return GearUnloadCollection.Where(t => t.Parent.PK == parentSampling.PK).ToList();
        }

        public List<GearUnload> GetGearUnloads()
        {
            return GearUnloadCollection.ToList();
        }


        /// <summary>
        /// Returns  gear unload with similar gear, landing site and data of sampling of input gear unload
        /// </summary>
        /// <param name="gearUnload"></param>
        /// <param name="samplingDate"></param>
        /// <param name="ls"></param>
        /// <returns></returns>
        public GearUnload getOtherGearUnload(GearUnload gearUnload, DateTime samplingDate, LandingSite ls)
        {
            return GearUnloadCollection.Where(t => t.PK != gearUnload.PK &&
                                t.GearUsedName == gearUnload.GearUsedName &&
                                t.Parent.SamplingDate.Date == samplingDate.Date &&
                                t.Parent.LandingSiteID == ls.LandingSiteID).FirstOrDefault();
        }


        /// <summary>
        /// returns gear unload having the same gear, sampling date and landing site of input gear unload
        /// </summary>
        /// <param name="gearUnload"></param>
        /// <param name="sampling"></param>
        /// <returns></returns>
        public GearUnload getOtherGearUnload(GearUnload gearUnload)
        {
            return GearUnloadCollection.Where(t => t.PK != gearUnload.PK &&
                                t.GearUsedName == gearUnload.GearUsedName &&
                                t.Parent.SamplingDate.Date == gearUnload.Parent.SamplingDate.Date &&
                                t.Parent.LandingSiteID == gearUnload.Parent.LandingSite.LandingSiteID).FirstOrDefault();
        }
        public void UndoChangesToGearUnloadBoatCatch(List<GearUnload> gearUnloadList)
        {
            if (gearUnloadList != null && gearUnloadList.Count > 0)
            {
                foreach (var item in gearUnloadList)
                {
                    var originalItem = CopyOfGearUnloadList.FirstOrDefault(t => t.PK == item.PK);
                    item.Boats = originalItem?.Boats;
                    item.Catch = originalItem?.Catch;
                }
            }
        }


        public void SaveChangesToBoatAndCatch(List<GearUnload> listToSave)
        {
            foreach (var item in listToSave)
            {
                try
                {
                    var original = CopyOfGearUnloadList.FirstOrDefault(t => t.PK == item.PK);

                    if (item.Boats != original.Boats || item.Catch != original.Catch)
                    {
                        UpdateRecordInRepo(item);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            CopyOfGearUnloadList = null;
        }
        public List<GearUnload> CopyOfGearUnloadList { get; internal set; }

        public List<GearUnload> GetAllGearUnloads(TreeViewModelControl.AllSamplingEntitiesEventHandler parameters, bool createCopyOfList = true)
        {
            CopyOfGearUnloadList = new List<GearUnload>();
            var list = new List<GearUnload>();
            string ls = "";
            switch (parameters.ContextMenuTopic)
            {
                case "contextMenuGearUnloadMonth":
                    ls = parameters.LandingSite == null ? parameters.LandingSiteText : parameters.LandingSite.LandingSiteName;
                    list = GearUnloadCollection
                        .Where(t => t.Parent.NSAPRegionID == parameters.NSAPRegion.Code)
                        .Where(t => t.Parent.FMAID == parameters.FMA.FMAID)
                        .Where(t => t.Parent.FishingGroundID == parameters.FishingGround.Code)
                        .Where(t => t.Parent.LandingSiteName == ls)
                        .Where(t => t.Parent.SamplingDate.Year == ((DateTime)parameters.MonthSampled).Year)
                        .Where(t => t.Parent.SamplingDate.Month == ((DateTime)parameters.MonthSampled).Month)
                        .OrderBy(t => t.Parent.LandingSiteName)
                        .ThenBy(t => t.Parent.SamplingDate).ToList();
                    break;
                case "contextMenuGearUnloadLandingSite":
                    ls = parameters.LandingSite == null ? parameters.LandingSiteText : parameters.LandingSite.LandingSiteName;
                    list = GearUnloadCollection
                        .Where(t => t.Parent.NSAPRegionID == parameters.NSAPRegion.Code)
                        .Where(t => t.Parent.FMAID == parameters.FMA.FMAID)
                        .Where(t => t.Parent.FishingGroundID == parameters.FishingGround.Code)
                        .Where(t => t.Parent.LandingSiteName == ls)
                        .OrderBy(t => t.Parent.LandingSiteName)
                        .ThenBy(t => t.Parent.SamplingDate).ToList();
                    break;
                case "contextMenuGearUnloadFishingGround":

                    list = GearUnloadCollection
                        .Where(t => t.Parent.NSAPRegionID == parameters.NSAPRegion.Code)
                        .Where(t => t.Parent.FMAID == parameters.FMA.FMAID)
                        .Where(t => t.Parent.FishingGroundID == parameters.FishingGround.Code)
                        .OrderBy(t => t.Parent.LandingSiteName)
                        .ThenBy(t => t.Parent.SamplingDate).ToList();
                    break;
            }

            if (createCopyOfList)
            {
                var newList = new List<GearUnload>();
                foreach (var item in list)
                {
                    var gu = new GearUnload
                    {
                        PK = item.PK,
                        Boats = item.Boats,
                        Catch = item.Catch
                    };
                    newList.Add(gu);
                }
                CopyOfGearUnloadList = newList;
            }

            return list;
        }
        public List<GearUnload> GetAllGearUnloads(NSAPRegion region, FMA fma, FishingGround fishingGround, bool createCopyOfList = true)
        {
            CopyOfGearUnloadList = new List<GearUnload>();

            var list = GearUnloadCollection
                .Where(t => t.Parent.NSAPRegionID == region.Code)
                .Where(t => t.Parent.FMAID == fma.FMAID)
                .Where(t => t.Parent.FishingGroundID == fishingGround.Code)
                .OrderBy(t => t.Parent.LandingSiteName)
                .ThenBy(t => t.Parent.SamplingDate).ToList();


            if (createCopyOfList)
            {
                var newList = new List<GearUnload>();
                foreach (var item in list)
                {
                    var gu = new GearUnload
                    {
                        PK = item.PK,
                        Boats = item.Boats,
                        Catch = item.Catch
                    };
                    newList.Add(gu);
                }
                CopyOfGearUnloadList = newList;
            }

            return list;
        }

        public List<GearUnload> GetAllGearUnloads(DateTime dateAddedToDatabase, bool createCopyOfList = true)
        {
            CopyOfGearUnloadList = new List<GearUnload>();

            var list = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                    .Where(t => t.DateAddedToDatabase.Value.Date == dateAddedToDatabase.Date)
                    .Select(t => t.Parent)
                    .GroupBy(t => t.PK)
                    .Select(t => t.First())
                    .OrderBy(t => t.Parent.NSAPRegion.Name)
                    .ThenBy(t => t.Parent.FMA.Name)
                    .ThenBy(t => t.Parent.FishingGround.Name)
                    .ThenBy(t => t.Parent.LandingSiteName)
                    .ThenBy(t => t.Parent.SamplingDate)
                    .ThenBy(t => t.GearUsedName)
                    .ToList();


            if (createCopyOfList)
            {
                var newList = new List<GearUnload>();
                foreach (var item in list)
                {
                    var gu = new GearUnload
                    {
                        PK = item.PK,
                        Boats = item.Boats,
                        Catch = item.Catch
                    };
                    newList.Add(gu);
                }
                CopyOfGearUnloadList = newList;
            }

            return list;


        }
        public List<GearUnloadFlattened> GetAllFlattenedItems()
        {
            List<GearUnloadFlattened> thisList = new List<GearUnloadFlattened>();
            foreach (var item in GearUnloadCollection)
            {
                thisList.Add(new GearUnloadFlattened(item));
            }
            return thisList;
        }
        public bool ClearRepository()
        {
            GearUnloadCollection.Clear();
            return GearUnloadRepository.ClearTable();
        }

        public List<GearUnload> GetGearUnloads(string gearText)
        {
            return GearUnloadCollection.Where(t => t.GearUsedText == gearText).ToList();
        }

        public GearUnload GetGearUnloadEx(string gearUsedName)
        {
            GearUnload gu = GearUnloadCollection.FirstOrDefault(t => t.GearUsedName == gearUsedName);
            //return GearUnloadCollection.FirstOrDefault(t=>t.GearUsedName==gearUsedName);
            return gu;

        }
        public GearUnload GetGearUnload(int pk, bool loadVesselViewModel = false)
        {
            var gu = GearUnloadCollection.FirstOrDefault(n => n.PK == pk);
            if (gu != null && loadVesselViewModel)
            {
                if (gu.VesselUnloadViewModel == null)
                {
                    gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
                }
            }
            return gu;

        }

        public GearUnload GetGearUnload(ExcelMainSheet ex)
        {
            return GearUnloadCollection
                .Where(t => t.Parent.LandingSiteName == ex.LandingSiteName)
                .Where(t => t.Parent.FishingGroundID == ex.NSAPRegionFMAFishingGround.FishingGround.Code)
                .Where(t => t.Parent.SamplingDate.Date == ex.SamplingDate.Date)
                .Where(t => t.GearUsedName == ex.GearName).FirstOrDefault();
        }

        public GearUnload GetGearUnload(VesselLanding landing)
        {
            if (landing.LandingSite == null && landing.LandingSiteText == null)
            {
                return null;
            }
            else
            {
                return GearUnloadCollection
                    .Where(t => t.Parent.LandingSiteName == landing.LandingSiteName)
                    .Where(t => t.Parent.FishingGroundID == landing.FishingGround.Code)
                    .Where(t => t.Parent.SamplingDate.Date == landing.SamplingDate.Date)
                    .Where(t => t.GearUsedName == landing.GearName).FirstOrDefault();
            }
        }


        private void GearUnloadCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            EditSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        GearUnload newItem = GearUnloadCollection[e.NewStartingIndex];
                        if (newItem.DelayedSave)
                        {
                            EditSuccess = SetCSV(newItem);
                        }
                        else if (!_addToCollectionOnly)
                        {
                            EditSuccess = GearUnloads.Add(newItem);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GearUnload> tempListOfRemovedItems = e.OldItems.OfType<GearUnload>().ToList();
                        EditSuccess = GearUnloads.Delete(tempListOfRemovedItems[0].PK);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GearUnload> tempList = e.NewItems.OfType<GearUnload>().ToList();
                        EditSuccess = GearUnloads.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GearUnloadCollection.Count; }
        }

        public bool AddRecordToRepo(GearUnload item, bool addToCollectionOnly = false)
        {
            _addToCollectionOnly = addToCollectionOnly;
            if (item == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GearUnloadCollection.Add(item);
            return EditSuccess;
        }

        public bool UpdateRecordInRepo(GearUnload item)
        {
            if (item.PK == 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < GearUnloadCollection.Count)
            {
                if (GearUnloadCollection[index].PK == item.PK)
                {
                    GearUnloadCollection[index] = item;
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

                return GearUnloads.MaxRecordNumber() + 1;

            }
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GearUnloadCollection.Count)
            {
                if (GearUnloadCollection[index].PK == id)
                {
                    GearUnloadCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return EditSuccess;
        }
    }
}
