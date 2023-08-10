using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NSAP_ODK.Entities.Database;
using System.Diagnostics;
using NSAP_ODK.Utilities;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class NSAPRegionViewModel
    {
        public event EventHandler VesselListEvent;
        private bool _editSuccess;
        private LandingSite _editedLandingSite;
        public DBSummary TotalOfSummary { get; set; }

        public Dictionary<DateTime, DBSummary> RegionMonthSampledSummaryDictionary { get; private set; }
        public Dictionary<string, DBSummary> RegionLandingSiteSummaryDictionary { get; private set; }
        public Dictionary<FishingGround, DBSummary> RegionFishingGroundSummaryDictionary { get; private set; }
        public Dictionary<NSAPRegion, DBSummary> RegionSummaryDictionary { get; private set; }

        public Dictionary<FishingGround, DBSummary> RegionFMASummaryDictionary { get; private set; }
        public ObservableCollection<NSAPRegion> NSAPRegionCollection { get; set; }
        private NSAPRegionRepository NSAPRegions { get; set; }

        public Dictionary<string, NSAPRegionWithEntitiesRepository> NSAPRegionsWithEntitiesRepositories { get; set; }


        private void ManageVesselListingEvent(string intent, int? listCount = 0)
        {
            EventHandler h = VesselListEvent;
            if (h != null)
            {
                var ev = new VesselListingEventArgs();
                ev.Intent = intent;
                switch (intent)
                {
                    case "start":
                        //placeholder
                        break;
                    case "end":
                        ev.ListCount = listCount;
                        break;
                }
                h(null, ev);
            }
        }
        public Task<List<string>> GetVesselNamesByRegionAsync(NSAPRegion region)
        {
            return Task.Run(() => GetVesselNamesByRegion(region));
        }
        public List<string> GetVesselNamesByRegion(NSAPRegion region)
        {
            ManageVesselListingEvent(intent: "start");
            List<string> vesselNames = new List<string>();
            foreach (var landing in NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Where(t => t.Parent.Parent.NSAPRegion == region && t.VesselName.Length > 0))
            {
                vesselNames.Add($"{landing.VesselName}\t{landing.SectorCode}");
            }

            var listNames = vesselNames.Distinct().ToList();
            ManageVesselListingEvent(intent: "end", listCount: listNames.Count());
            return listNames;
        }
        public List<FishingGround> GetFishingGrounds(NSAPRegion region)
        {
            List<FishingGround> listFG = new List<FishingGround>();
            foreach (var fma in NSAPRegionCollection.Where(t => t.Code == region.Code).FirstOrDefault().FMAs)
            {
                foreach (var fg in fma.FishingGrounds)
                {
                    listFG.Add(fg.FishingGround);
                }
            }
            return listFG;
        }
        public LandingSite GetLandingSiteInRegion(string regionCode, int regionFMA, int regionFMAFishingGround, int fishingGroundLandingSite)
        {
            //var nsr = NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault();
            //var rf = nsr.FMAs.Where(t => t.RowID == regionFMA).FirstOrDefault();
            //var fg = rf.FishingGrounds.Where(t => t.RowID == regionFMAFishingGround).FirstOrDefault();
            //var ls = fg.LandingSites.Where(t => t.RowID == fishingGroundLandingSite).FirstOrDefault()?.LandingSite;

            return NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault()?.
                FMAs.Where(t => t.RowID == regionFMA).FirstOrDefault()?.
                FishingGrounds.Where(t => t.RowID == regionFMAFishingGround).FirstOrDefault()?.
                LandingSites.Where(t => t.RowID == fishingGroundLandingSite).FirstOrDefault()?.LandingSite;
            //return ls;
        }
        public FishingGround GetFishingGroundInRegion(string regionCode, int regionFMA, int regionFMAFishingGround)
        {
            return NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault()
                .FMAs.Where(t => t.RowID == regionFMA).FirstOrDefault()
                .FishingGrounds.Where(t => t.RowID == regionFMAFishingGround).FirstOrDefault().FishingGround;
        }
        public List<LandingSite> GetLandingSites(NSAPRegion region, FishingGround fishingGround)
        {
            List<LandingSite> listLS = new List<LandingSite>();
            foreach (var fma in NSAPRegionCollection.Where(t => t.Code == region.Code).FirstOrDefault().FMAs)
            {
                foreach (var fg in fma.FishingGrounds.Where(t => t.FishingGround.Code == fishingGround.Code))
                {
                    foreach (var ls in fg.LandingSites)
                    {
                        listLS.Add(ls.LandingSite);
                    }
                }
            }
            return listLS;
        }

        public LandingSite EditedLandingSite
        {
            get { return _editedLandingSite; }
            set
            {
                _editedLandingSite = value;
                foreach(var item in NSAPRegionsWithEntitiesRepositories)
                {
                    foreach(var fma in item.Value.NSAPRegion.FMAs)
                    {
                        foreach(var fg in fma.FishingGrounds)
                        {
                            foreach(var ls in fg.LandingSites)
                            {
                                if(ls.LandingSite.LandingSiteID==_editedLandingSite.LandingSiteID)
                                {
                                    ls.LandingSite = _editedLandingSite;
                                    break;
                                }
                            }
                        }
                    }

                }
            }
        }

        public void SetUpSummaryForLandingSite(NSAPRegion region, FMA fma, FishingGround fishingGround, string landingSite)
        {
            RegionMonthSampledSummaryDictionary = new Dictionary<DateTime, DBSummary>();
            var monthSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .Where(t => t.NSAPRegionID == region.Code &&
                          t.FMAID == fma.FMAID &&
                          t.FishingGroundID == fishingGround.Code &&
                          t.LandingSiteName == landingSite).GroupBy(t => t.MonthSampled)
                          .OrderBy(t => t.Key)
                          .ToList();

            foreach (var month in monthSamplings)
            {
                DBSummary smmry = new DBSummary();
                smmry.MonthSampled = month.Key.ToString("MMM-dd-yyyy");

                //var landingSiteSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                //    .Where(t => t.FMAID == fma.FMAID &&
                //    t.LandingSiteName == landingSite &&
                //    t.MonthSampled == month.Key).ToList();

                //var gus = new List<GearUnload>();
                //foreach(var ls in landingSiteSamplings)
                //{
                //    gus.AddRange(ls.GearUnloadViewModel.GearUnloadCollection.ToList());
                //}

                //var landings = new List<VesselUnload>();

                //foreach(var gu in gus)
                //{
                //    landings.AddRange(gu.VesselUnloadViewModel.VesselUnloadCollection.ToList());
                //}

                //var landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                //    .Where(t => t.Parent.Parent.NSAPRegionID == region.Code &&
                //        t.Parent.Parent.FMAID == fma.FMAID &&
                //        t.Parent.Parent.LandingSiteName == landingSite &&
                //        t.MonthSampled == month.Key
                //    ).ToList();
                var gearUnloads = NSAPEntities.LandingSiteSamplingViewModel.GetGearUnloads(region.Code, fma.FMAID, fishingGround.Code, landingSite, month.Key);
                var landings = NSAPEntities.LandingSiteSamplingViewModel.GetVesselUnloads(gearUnloads);

                smmry.VesselUnloadCount = landings.Count;
                if (landings.Count > 0)
                {
                    smmry.LastLandingFormattedDate = landings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                    smmry.FirstLandingFormattedDate = landings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                    smmry.LatestDownloadFormattedDate = ((DateTime)landings.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                    smmry.TrackedOperationsCount = landings.Count(t => t.OperationIsTracked == true);
                    smmry.CountLandingsWithCatchComposition = landings.Count(t => t.HasCatchComposition == true);
                    smmry.GearUnloadCount = gearUnloads.Count;
                    smmry.CountCompleteGearUnload = gearUnloads.Count(t => t.Boats != null && t.Catch != null);
                    smmry.GearUnloadCount = gearUnloads.GroupBy(t => t.PK).Count();
                }

                RegionMonthSampledSummaryDictionary.Add(month.Key, smmry);
            }
        }

        public void SetupSummaryForFishingGround(NSAPRegion region, FishingGround fishingGround, bool onlyWithLandings = false)
        {
            RegionLandingSiteSummaryDictionary = new Dictionary<string, DBSummary>();
            foreach (var fma in NSAPRegionCollection.Where(t => t.Code == region.Code).FirstOrDefault().FMAs)
            {

                var lsSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .Where(t => t.NSAPRegionID == region.Code &&
                       t.FMAID == fma.FMAID &&
                       t.FishingGroundID == fishingGround.Code).GroupBy(t => t.LandingSiteName).ToList();

                //if (lsSamplings.Count > 0)
                //{
                foreach (var lsSampling in lsSamplings)
                {
                    DBSummary smmry = new DBSummary();
                    var landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                     .Where(t => t.Parent.Parent.LandingSiteName == lsSampling.Key).ToList();
                    smmry.LandingSiteName = lsSampling.Key;
                    smmry.VesselUnloadCount = landings.Count;
                    smmry.FMA = fma.FMA;
                    smmry.FishingGround = fishingGround;
                    if (landings.Count > 0)
                    {
                        smmry.LastLandingFormattedDate = landings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                        smmry.FirstLandingFormattedDate = landings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                        smmry.LatestDownloadFormattedDate = ((DateTime)landings.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                        smmry.TrackedOperationsCount = landings.Count(t => t.OperationIsTracked == true);


                        var gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Where(t => t.Parent.LandingSiteName == lsSampling.Key).ToList();
                        smmry.GearUnloadCount = gearUnloads.Count;
                        smmry.CountCompleteGearUnload = gearUnloads.Count(t => t.Boats != null && t.Catch != null);
                        smmry.CountLandingsWithCatchComposition = landings.Count(t => t.HasCatchComposition == true);
                        //RegionLandingSiteSummaryDictionary.Add(lsSampling.Key, smmry);
                    }
                    else if (!onlyWithLandings)
                    {
                        //RegionLandingSiteSummaryDictionary.Add(lsSampling.Key, smmry);
                    }
                    RegionLandingSiteSummaryDictionary.Add(lsSampling.Key, smmry);

                }
                //}
            }

        }
        public void SetupSummaryForFishingGroundAllSites(NSAPRegion region, FishingGround fishingGround, bool onlyWithLandings = false)
        {
            RegionLandingSiteSummaryDictionary = new Dictionary<string, DBSummary>();
            foreach (var fma in NSAPRegionCollection.Where(t => t.Code == region.Code).FirstOrDefault().FMAs)
            {
                foreach (var fg in fma.FishingGrounds.Where(t => t.FishingGround.Code == fishingGround.Code))
                {

                    foreach (var ls in fg.LandingSites)
                    {
                        DBSummary smmry = new DBSummary();
                        smmry.FMA = fma.FMA;
                        smmry.FishingGround = fg.FishingGround;
                        smmry.LandingSite = ls.LandingSite;
                        var landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                            .Where(t => t.Parent.Parent.LandingSiteID == ls.LandingSite.LandingSiteID &&
                                        t.Parent.Parent.FishingGroundID == fishingGround.Code &&
                                        t.Parent.Parent.FMAID == fma.FMAID &&
                                        t.Parent.Parent.NSAPRegionID == region.Code).ToList();
                        smmry.VesselUnloadCount = landings.Count;
                        if (landings.Count > 0)
                        {
                            smmry.LastLandingFormattedDate = landings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                            smmry.FirstLandingFormattedDate = landings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                            smmry.LatestDownloadFormattedDate = ((DateTime)landings.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                            smmry.TrackedOperationsCount = landings.Count(t => t.OperationIsTracked == true);
                            smmry.CountLandingsWithCatchComposition = landings.Count(t => t.HasCatchComposition == true);

                            var gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                                .Where(t => t.Parent.LandingSiteID == ls.LandingSite.LandingSiteID &&
                                          t.Parent.FishingGroundID == fishingGround.Code &&
                                          t.Parent.FMAID == fma.FMAID &&
                                          t.Parent.NSAPRegionID == region.Code).ToList();
                            smmry.GearUnloadCount = gearUnloads.Count;
                            smmry.CountCompleteGearUnload = gearUnloads.Count(t => t.Boats != null && t.Catch != null);

                            RegionLandingSiteSummaryDictionary.Add(ls.LandingSite.ToString(), smmry);

                        }
                        else if (!onlyWithLandings)
                        {
                            RegionLandingSiteSummaryDictionary.Add(ls.LandingSite.ToString(), smmry);
                        }
                    }

                    var lsSamplings = NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                        .Where(t => t.NSAPRegionID == region.Code &&
                                  t.LandingSite == null &&
                                  t.LandingSiteText.Length > 0 &&
                                  t.FMAID == fma.FMAID &&
                                  t.FishingGroundID == fg.FishingGround.Code).GroupBy(t => t.LandingSiteText).ToList();

                    if (lsSamplings.Count > 0)
                    {
                        foreach (var lsSampling in lsSamplings)
                        {
                            DBSummary smmry = new DBSummary();
                            var landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                             .Where(t => t.Parent.Parent.LandingSiteText == lsSampling.Key &&
                                    t.Parent.Parent.LandingSiteID == null).ToList();
                            smmry.VesselUnloadCount = landings.Count;
                            smmry.FMA = fma.FMA;
                            smmry.FishingGround = fg.FishingGround;
                            if (landings.Count > 0)
                            {
                                smmry.LastLandingFormattedDate = landings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                                smmry.FirstLandingFormattedDate = landings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                                smmry.LatestDownloadFormattedDate = ((DateTime)landings.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                                smmry.TrackedOperationsCount = landings.Count(t => t.OperationIsTracked == true);


                                var gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Where(t => t.Parent.LandingSiteText == lsSampling.Key).ToList();
                                smmry.GearUnloadCount = gearUnloads.Count;
                                smmry.CountCompleteGearUnload = gearUnloads.Count(t => t.Boats != null && t.Catch != null);
                                RegionLandingSiteSummaryDictionary.Add(lsSampling.Key, smmry);
                            }
                            else if (!onlyWithLandings)
                            {
                                RegionLandingSiteSummaryDictionary.Add(lsSampling.Key, smmry);
                            }

                        }
                    }

                }
            }
        }

        public void SetupSummaryForFMA(NSAPRegion region, FMA fma)
        {
            RegionFMASummaryDictionary = new Dictionary<FishingGround, DBSummary>();
            foreach (var regionFMA in NSAPRegionCollection.Where(t => t.Code == region.Code).FirstOrDefault().FMAs)
            {
                if (regionFMA.FMAID == fma.FMAID)
                {
                    foreach (var fg in regionFMA.FishingGrounds)
                    {
                        DBSummary smmry = new DBSummary();
                        smmry.FishingGround = fg.FishingGround;
                        smmry.FMA = fma;
                        var landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Where(t => t.Parent.Parent.FishingGroundID == fg.FishingGround.Code && t.Parent.Parent.FMAID == fma.FMAID && t.Parent.Parent.NSAPRegionID == region.Code).ToList();
                        smmry.VesselUnloadCount = landings.Count;
                        if (landings.Count > 0)
                        {
                            smmry.LastLandingFormattedDate = landings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                            smmry.FirstLandingFormattedDate = landings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                            smmry.LatestDownloadFormattedDate = ((DateTime)landings.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                            var gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Where(t => t.Parent.FMA.FMAID == fma.FMAID && t.Parent.FishingGroundID == fg.FishingGroundCode && t.Parent.NSAPRegionID == region.Code).ToList();
                            smmry.GearUnloadCount = gearUnloads.Count;
                            smmry.CountCompleteGearUnload = gearUnloads.Count(t => t.Boats != null && t.Catch != null);
                            smmry.TrackedOperationsCount = landings.Count(t => t.OperationIsTracked == true);
                            smmry.CountLandingsWithCatchComposition = landings.Count(t => t.HasCatchComposition == true);
                        }
                        RegionFMASummaryDictionary.Add(fg.FishingGround, smmry);
                    }
                }



            }


        }

        public void SetupSummaryForRegion(NSAPRegion region)
        {
            RegionFishingGroundSummaryDictionary = new Dictionary<FishingGround, DBSummary>();
            foreach (var fma in NSAPRegionCollection.Where(t => t.Code == region.Code).FirstOrDefault().FMAs)
            {
                foreach (var fg in fma.FishingGrounds)
                {
                    DBSummary smmry = new DBSummary();
                    smmry.FishingGround = fg.FishingGround;
                    smmry.FMA = fma.FMA;
                    var landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Where(t => t.Parent.Parent.FishingGroundID == fg.FishingGround.Code && t.Parent.Parent.FMAID == fma.FMAID && t.Parent.Parent.NSAPRegionID == region.Code).ToList();
                    smmry.VesselUnloadCount = landings.Count;
                    if (landings.Count > 0)
                    {
                        smmry.LastLandingFormattedDate = landings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                        smmry.FirstLandingFormattedDate = landings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                        smmry.LatestDownloadFormattedDate = ((DateTime)landings.OrderByDescending(t => t.DateAddedToDatabase).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                        var gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Where(t => t.Parent.FMA.FMAID == fma.FMAID && t.Parent.FishingGroundID == fg.FishingGroundCode && t.Parent.NSAPRegionID == region.Code).ToList();
                        smmry.GearUnloadCount = gearUnloads.Count;
                        smmry.CountCompleteGearUnload = gearUnloads.Count(t => t.Boats != null && t.Catch != null);
                        smmry.TrackedOperationsCount = landings.Count(t => t.OperationIsTracked == true);
                        smmry.CountLandingsWithCatchComposition = landings.Count(t => t.HasCatchComposition == true);
                    }
                    RegionFishingGroundSummaryDictionary.Add(fg.FishingGround, smmry);
                }
            }
        }
        public void SetupSummary()
        {
            DBSummary smmry;
            int total_landing_count = 0;
            int total_gear_unload_count = 0;
            int total_completed_gear_unload_count = 0;
            RegionSummaryDictionary = new Dictionary<NSAPRegion, DBSummary>();
            foreach (var rgn in NSAPRegionCollection)
            {
                smmry = new DBSummary();
                smmry.FMACount = rgn.FMAs.Count(t => t.NSAPRegion.Code == rgn.Code);
                foreach (var fma in rgn.FMAs.Where(t => t.NSAPRegion.Code == rgn.Code))
                {
                    smmry.FishingGroundCount += fma.FishingGroundCount;
                    foreach (var fg in fma.FishingGrounds)
                    {
                        smmry.LandingSiteCount += fg.LandingSiteCount;
                    }
                }
                smmry.FishingGearCount = rgn.Gears.Count(t => t.NSAPRegion.Code == rgn.Code);
                smmry.EnumeratorCount = rgn.NSAPEnumerators.Count(t => t.NSAPRegion.Code == rgn.Code);
                smmry.FishingVesselCount = rgn.FishingVessels.Count(t => t.NSAPRegion.Code == rgn.Code);
                int landings = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t => t.Parent.Parent.NSAPRegion.Code == rgn.Code);
                smmry.VesselUnloadCount = landings;
                if (landings > 0)
                {
                    smmry.LastLandingFormattedDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.OrderByDescending(t => t.SamplingDate).Where(t => t.Parent.Parent.NSAPRegion.Code == rgn.Code).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                    smmry.FirstLandingFormattedDate = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.OrderBy(t => t.SamplingDate).Where(t => t.Parent.Parent.NSAPRegion.Code == rgn.Code).FirstOrDefault().SamplingDate.ToString("MMM-dd-yyyy");
                    smmry.LatestDownloadFormattedDate = ((DateTime)NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.OrderByDescending(t => t.DateAddedToDatabase).Where(t => t.Parent.Parent.NSAPRegion.Code == rgn.Code).FirstOrDefault().DateAddedToDatabase).ToString("MMM-dd-yyyy");
                    smmry.GearUnloadCount = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count(t => t.Parent.NSAPRegion.Code == rgn.Code);
                    smmry.CountCompleteGearUnload = NSAPEntities.GearUnloadViewModel.GearUnloadCollection.Count(t => t.Parent.NSAPRegion.Code == rgn.Code && t.Boats != null && t.Catch != null);
                    smmry.TrackedOperationsCount = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t => t.Parent.Parent.NSAPRegion.Code == rgn.Code && t.OperationIsTracked == true);
                    smmry.CountLandingsWithCatchComposition = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Where(
                        t => t.HasCatchComposition == true &&
                        t.Parent.Parent.NSAPRegion.Code == rgn.Code).ToList().Count;
                }
                total_landing_count += landings;
                total_gear_unload_count += smmry.GearUnloadCount;
                total_completed_gear_unload_count += smmry.CountCompleteGearUnload;
                RegionSummaryDictionary.Add(rgn, smmry);
            }
            TotalOfSummary = new DBSummary();
            TotalOfSummary.IsTotal = true;
            TotalOfSummary.VesselUnloadCount = total_landing_count;
            TotalOfSummary.GearUnloadCount = total_gear_unload_count;
            TotalOfSummary.CountCompleteGearUnload = total_completed_gear_unload_count;

        }
        public NSAPRegionWithEntitiesRepository GetNSAPRegionWithEntitiesRepository(NSAPRegion nsapRegion)
        {
            if (nsapRegion == null)
            {
                return null;
            }
            else
            {
                return NSAPRegionsWithEntitiesRepositories[nsapRegion.Code];
            }
        }

        //public List<VesselUnload> GetFirstSamplingDateOfEnumerators()
        //{
        //    HashSet<VesselUnload> thisList = new HashSet<VesselUnload>();
        //    bool success = false;
        //    foreach (var region in NSAPRegionCollection)
        //    {
        //        var unloads = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Where(
        //            t => t.Parent.Parent.NSAPRegionID == region.Code);

        //        if (unloads.Count() > 0)
        //            foreach (var nre in region.NSAPEnumerators.Where(t => t.DateFirstSampling == null))
        //            {


        //                {
        //                    var enumerators_firstSampling = unloads
        //                        .Where(t => t.NSAPEnumeratorID == nre.EnumeratorID)
        //                        .OrderBy(t => t.SamplingDate).FirstOrDefault();

        //                    if(enumerators_firstSampling!=null)
        //                    {
        //                        thisList.Add(enumerators_firstSampling);
        //                    }
        //                }
        //            }
        //    }

        //    return thisList.ToList();
        //}
        public bool UpdateLandingSiteInFishingGround(string regionCode)
        {
            bool success = false;
            var nrwer = NSAPRegionsWithEntitiesRepositories[regionCode];

            return success;
        }
        public int SetNSAPRegionsWithEntitiesRepositories()
        {
            NSAPRegionsWithEntitiesRepositories = new Dictionary<string, NSAPRegionWithEntitiesRepository>();
            int counter = 0;
            foreach (NSAPRegion nsapRegion in NSAPRegionCollection)
            {
                NSAPRegionWithEntitiesRepository nswer = new NSAPRegionWithEntitiesRepository(nsapRegion);
                NSAPRegionsWithEntitiesRepositories.Add(nsapRegion.Code, nswer);
                counter++;
            }
            return counter;

            //add additional fmas in nsap region 6
            //var region6 = NSAPRegionCollection.FirstOrDefault(t => t.Code == "6");
            //if (region6 != null)
            //{
            //    var regionEntitiesRepo_6 = NSAPRegionsWithEntitiesRepositories[region6.Code];
            //    var fma = GetFMAInRegion(region6.Code, "FMA 6");
            //    if (fma == null)
            //    {
            //        var fma6 = NSAPEntities.FMAViewModel.GetFMA(6);
            //        NSAPRegionFMA region6FMA6 = new NSAPRegionFMA { NSAPRegion = region6, FMA = fma6 };
            //        if (regionEntitiesRepo_6.AddNSAPRegionFMA(region6FMA6))
            //        {

            //        }
            //    }

            //    fma = GetFMAInRegion(region6.Code, "FMA 4");
            //    if (fma == null)
            //    {
            //        var fma4 = NSAPEntities.FMAViewModel.GetFMA(4);
            //        NSAPRegionFMA region6FMA4 = new NSAPRegionFMA { NSAPRegion = region6, FMA = fma4 };
            //        if (regionEntitiesRepo_6.AddNSAPRegionFMA(region6FMA4))
            //        {

            //        }
            //    }
            //}



        }
        public NSAPRegionViewModel()
        {
            NSAPRegions = new NSAPRegionRepository();
            NSAPRegionCollection = new ObservableCollection<NSAPRegion>(NSAPRegions.NSAPRegions);
            NSAPRegionCollection.CollectionChanged += NSAPRegionCOllection_CollectionChanged;

            //NSAPRegionsWithEntitiesRepositories = new Dictionary<string, NSAPRegionWithEntitiesRepository>();
            //foreach (NSAPRegion nsapRegion in NSAPRegionCollection)
            //{
            //    NSAPRegionWithEntitiesRepository nswer = new NSAPRegionWithEntitiesRepository(nsapRegion);
            //    NSAPRegionsWithEntitiesRepositories.Add(nsapRegion.Code, nswer);
            //}
        }

        public FMA GetFMAInRegion(string regionCode, string fmaName)
        {
            FMA fma = null;
            try
            {
                var region = NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault();
                //fma = region.FMAs.Select(t=>t.)
            }
            catch
            {
                //ignore
            }

            return fma;
        }
        public FMA GetFMAInRegion(string regionCode, int regionFMA)
        {
            return NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault().FMAs.Where(t => t.RowID == regionFMA).FirstOrDefault().FMA;
        }

        public Dictionary<NSAPRegion, List<NSAPEnumerator>> GetEnumeratorsByRegionDictionary()
        {
            Dictionary<NSAPRegion, List<NSAPEnumerator>> dict = new Dictionary<NSAPRegion, List<NSAPEnumerator>>();
            foreach (var r in NSAPRegionCollection)
            {
                List<NSAPEnumerator> list = new List<NSAPEnumerator>();
                foreach (var e in r.NSAPEnumerators)
                {
                    list.Add(e.Enumerator);
                }
                dict.Add(r, list);
            }
            return dict;
        }

        public List<NSAPEnumerator> GetEnumeratorsInRegion(NSAPRegion region)
        {
            return GetEnumeratorsByRegionDictionary()[region];
        }
        public NSAPEnumerator GetEnumeratorInRegion(string regionCode, int nsapEnumerator)
        {
            var nsapregionEnumerator = NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault().NSAPEnumerators.Where(t => t.RowID == nsapEnumerator).FirstOrDefault();
            if (nsapregionEnumerator != null)
            {
                return nsapregionEnumerator.Enumerator;
            }
            else
            {
                Logger.Log($"Query for non-exisiting enumerator with ID of {nsapEnumerator}");
                return null;
            }
            //return NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault().NSAPEnumerators.Where(t => t.RowID == nsapEnumerator).FirstOrDefault().Enumerator;
        }

        public List<FishingGround> GetRegionFMAFishingGrounds(string regionCode, int fmaCode)
        {
            List<FishingGround> listFG = new List<FishingGround>();
            var region = NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault();
            var regionFMA = region.FMAs.Where(t => t.FMAID == fmaCode).FirstOrDefault();
            foreach (var item in regionFMA.FishingGrounds)
            {
                listFG.Add(item.FishingGround);
            }
            return listFG;
        }
        public NSAPRegionFMAFishingGround GetRegionFMAFishingGround(string regionCode, int fmaCode, string fishingGroundCode)
        {
            var region = NSAPRegionCollection.Where(t => t.Code == regionCode).FirstOrDefault();
            var regionFMA = region.FMAs.Where(t => t.FMAID == fmaCode).FirstOrDefault();
            return regionFMA.FishingGrounds.Where(t => t.FishingGroundCode == fishingGroundCode).FirstOrDefault();

        }
        public int Count

        {
            get { return NSAPRegionCollection.Count; }
        }

        public bool NSAPRegionNameExists(string regionName)
        {
            foreach (NSAPRegion nsr in NSAPRegionCollection)
            {
                if (nsr.Name == regionName)
                {
                    return true;
                }
            }
            return false;
        }

        public NSAPRegion GetNSAPRegion(string code)
        {
            CurrentEntity = NSAPRegionCollection.FirstOrDefault(n => n.Code == code);
            if (CurrentEntity != null)
            {
                var nrwe = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(CurrentEntity);
                CurrentEntity.FMAs = nrwe.NSAPRegion.FMAs;
                CurrentEntity.FishingVessels = nrwe.NSAPRegion.FishingVessels;
                CurrentEntity.Gears = nrwe.NSAPRegion.Gears;
                CurrentEntity.NSAPEnumerators = nrwe.NSAPRegion.NSAPEnumerators;
            }
            return CurrentEntity;
        }

        public NSAPRegion CurrentEntity { get; private set; }

        public List<NSAPRegion> GetAllNSAPRegions()
        {
            return NSAPRegionCollection.OrderBy(t => t.Sequence).ToList();
        }

        private void NSAPRegionCOllection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            NSAPRegion editedNSAPRegion = new NSAPRegion();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        editedNSAPRegion = NSAPRegionCollection[newIndex];
                        _editSuccess = NSAPRegions.Add(editedNSAPRegion);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<NSAPRegion> tempListOfRemovedItems = e.OldItems.OfType<NSAPRegion>().ToList();
                        editedNSAPRegion = tempListOfRemovedItems[0];
                        _editSuccess = NSAPRegions.Delete(editedNSAPRegion.Code);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<NSAPRegion> tempList = e.NewItems.OfType<NSAPRegion>().ToList();
                        editedNSAPRegion = tempList[0];
                        _editSuccess = NSAPRegions.Update(editedNSAPRegion);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
            //EntityChangedEventArgs args = new EntityChangedEventArgs(editedLandingSite.GetType().Name,editedLandingSite);
            //EntityChanged?.Invoke(this, args);
        }

        public bool AddRecordToRepo(NSAPRegion nsr)
        {
            if (nsr == null)
                throw new ArgumentNullException("Error: The argument is Null");
            NSAPRegionCollection.Add(nsr);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(NSAPRegion nsr)
        {
            if (nsr.Code == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < NSAPRegionCollection.Count)
            {
                if (NSAPRegionCollection[index].Code == nsr.Code)
                {
                    NSAPRegionCollection[index] = nsr;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(string code)
        {
            if (code == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < NSAPRegionCollection.Count)
            {
                if (NSAPRegionCollection[index].Code == code)
                {
                    NSAPRegionCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool EntityValidated(NSAPRegion nsapRegion, out List<string> messages, bool isNew = false)
        {
            messages = new List<string>();

            if (nsapRegion.Name.Length < 5)
                messages.Add("NSAP Region's name must be at least 5 characters long");

            if (nsapRegion.Code == null)
                messages.Add("NSAP Region codecannot be empty");
            else if (nsapRegion.Code.Length > 3)
                messages.Add("NSAP Region code must not exceed 3 letters in length");
            return messages.Count == 0;
        }
    }
}