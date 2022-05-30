using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit;

namespace NSAP_ODK.Entities.Database
{
    public enum SummaryLevelType
    {
        Overall,
        AllRegions,
        FMA,
        Region,
        RegionOverall,
        FishingGround,
        LandingSite,
        Enumerator,
        EnumeratedMonth,
        EnumeratorRegion,
        Enumerators,
        SummaryOfEnumerators,
        SummaryOfDownloadDate

    }

    [CategoryOrder("Database", 1)]
    [CategoryOrder("Lookup choices", 2)]
    [CategoryOrder("Submitted fish landing data", 3)]
    [CategoryOrder("Saved JSON files", 4)]
    public class DBSummary
    {
        public void Refresh()
        {
            DBPath = Global.MDBPath;
            NSAPRegionCount = NSAPEntities.NSAPRegionViewModel.Count;
            FMACount = NSAPEntities.FMAViewModel.Count;
            FishSpeciesCount = NSAPEntities.FishSpeciesViewModel.Count;
            NonFishSpeciesCount = NSAPEntities.NotFishSpeciesViewModel.Count;
            FishingGroundCount = NSAPEntities.FishingGroundViewModel.Count;
            FishingGearCount = NSAPEntities.GearViewModel.Count;
            LandingSiteCount = NSAPEntities.LandingSiteViewModel.Count;
            GearSpecificationCount = NSAPEntities.EffortSpecificationViewModel.Count;
            VesselUnloadCount = NSAPEntities.SummaryItemViewModel.Count;
            GearUnloadCount = GearUnloadViewModel.GearUnloadCount();
            EnumeratorCount = NSAPEntities.NSAPEnumeratorViewModel.Count;
            FishingVesselCount = NSAPEntities.FishingVesselViewModel.Count;
            GPSCount = NSAPEntities.GPSViewModel.Count;
            TrackedOperationsCount = VesselUnloadViewModel.CountVesselUnload(isTracked:true);
            if (VesselUnloadCount > 0)
            {
                //VesselUnloadSummary vs = VesselUnloadViewModel.GetSummary();
                VesselUnloadSummary vs = NSAPEntities.SummaryItemViewModel.GetVesselUnloadSummary();
                //FirstSampledLandingDate = NSAPEntities.VesselUnloadViewModel.DateOfFirstSampledLanding;
                FirstSampledLandingDate = vs.FirstSamplingDate;
                //LastSampledLandingDate = NSAPEntities.VesselUnloadViewModel.DateOfLastSampledLanding;
                LastSampledLandingDate = vs.LastSamplingDate;
                //CountCompleteGearUnload = NSAPEntities.GearUnloadViewModel.CountCompletedGearUnload;
                CountCompleteGearUnload = GearUnloadViewModel.GearUnloadCount(isCompleted:true);
                //DateLastDownload = NSAPEntities.VesselUnloadViewModel.DateLatestDownload;
                DateLastDownload = vs.LatestDownloadDate;
                SavedJSONFolder = Global.Settings.JSONFolder;
                SavedFishingEffortJSONCount = NSAPEntities.JSONFileViewModel.CountSavedEffortJsonFile();
                SavedVesselCountsJSONCount = NSAPEntities.JSONFileViewModel.CountSavedVesselCountsJsonFile();
                //CountLandingsWithCatchComposition = NSAPEntities.VesselUnloadViewModel.CountLandingWithCatchComposition();
                CountLandingsWithCatchComposition = vs.CountUnloadsWithCatchComposition;
            }
        }
        public DBSummary()
        {

        }
        public string EnumeratorName { get; set; }
        [ReadOnly(true)]
        public int CountLandingsWithCatchComposition { get; set; }
        public bool IsTotal { get; set; }
        public FMA FMA { get; set; }

        public NSAPRegion NSAPRegion
        {
            get
            {
                return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAPRegionCode);
            }
        }

        public NSAPEnumerator NSAPEnumerator { get; set; }
        public string NSAPRegionCode { get; set; }
        [ReadOnly(true)]
        public string DBPath { get; set; }

        [ReadOnly(true)]
        public int FMACount { get; set; }


        public string DateDownloadFormatted { get; set; }
        [ReadOnly(true)]
        public int NSAPRegionCount { get; set; }

        [ReadOnly(true)]
        public int FishSpeciesCount { get; set; }

        [ReadOnly(true)]

        public int NonFishSpeciesCount { get; set; }

        [ReadOnly(true)]
        public int FishingGroundCount { get; set; }

        [ReadOnly(true)]
        public int LandingSiteCount { get; set; }

        [ReadOnly(true)]
        public int FishingGearCount { get; set; }

        [ReadOnly(true)]

        public int GearSpecificationCount { get; set; }

        [ReadOnly(true)]
        public int VesselUnloadCount { get; set; }

        [ReadOnly(true)]
        public int GearUnloadCount { get; set; }

        [ReadOnly(true)]
        public int EnumeratorCount { get; set; }

        [ReadOnly(true)]
        public int FishingVesselCount { get; set; }

        public string LatestEformVersion { get; set; }

        [ReadOnly(true)]
        public int GPSCount { get; set; }

        [ReadOnly(true)]
        public int TrackedOperationsCount { get; set; }
        [ReadOnly(true)]
        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime FirstSampledLandingDate { get; set; }
        [ReadOnly(true)]
        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime LastSampledLandingDate { get; set; }
        [ReadOnly(true)]

        public int CountCompleteGearUnload { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        [ReadOnly(true)]
        public DateTime DateLastDownload { get; set; }

        public string FirstLandingFormattedDate { get; set; }
        public string LastLandingFormattedDate { get; set; }
        public string LatestDownloadFormattedDate { get; set; }

        public FishingGround FishingGround { get; set; }

        public LandingSite LandingSite { get; set; }

        public string LandingSiteName { get; set; }

        public string GearName { get; set; }

        public string MonthSampled { get; set; }

        public DateTime SampledMonth { get; set; }
        public string SavedJSONFolder { get; set; }

        public int SavedFishingEffortJSONCount { get; set; }
        public int SavedVesselCountsJSONCount { get; set; }
    }
}
