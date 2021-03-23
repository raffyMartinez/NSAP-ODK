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
        FishingGround,
        LandingSite,
        Enumerator,
        EnumeratedMonth,
        EnumeratorRegion,
        Enumerators

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
            VesselUnloadCount = NSAPEntities.VesselUnloadViewModel.Count;
            GearUnloadCount = NSAPEntities.GearUnloadViewModel.Count;
            EnumeratorCount = NSAPEntities.NSAPEnumeratorViewModel.Count;
            FishingVesselCount = NSAPEntities.FishingVesselViewModel.Count;
            GPSCount = NSAPEntities.GPSViewModel.Count;
            TrackedOperationsCount = NSAPEntities.VesselUnloadViewModel.TrackedUnloadCount;
            if (VesselUnloadCount > 0)
            {
                FirstSampledLandingDate = NSAPEntities.VesselUnloadViewModel.DateOfFirstSampledLanding;
                LastSampledLandingDate = NSAPEntities.VesselUnloadViewModel.DateOfLastSampledLanding;
                CountCompleteGearUnload = NSAPEntities.GearUnloadViewModel.CountCompletedGearUnload;
                DateLastDownload = NSAPEntities.VesselUnloadViewModel.DateLatestDownload;
                SavedJSONFolder = Global.Settings.JSONFolder;
                SavedFishingEffortJSONCount = NSAPEntities.JSONFileViewModel.CountSavedEffortJsonFile();
                SavedVesselCountsJSONCount = NSAPEntities.JSONFileViewModel.CountSavedVesselCountsJsonFile();
            }
        }
        public DBSummary()
        {

        }
        public bool IsTotal { get; set; }
        public FMA FMA { get; set; }
        [ReadOnly(true)]
        public string DBPath { get; set; }

        [ReadOnly(true)]
        public int FMACount { get; set; }

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

        public string SavedJSONFolder { get; set; }

        public int SavedFishingEffortJSONCount { get; set; }
        public int SavedVesselCountsJSONCount { get; set; }
    }
}
