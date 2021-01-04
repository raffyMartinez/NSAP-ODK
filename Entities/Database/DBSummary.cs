using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;
using System.ComponentModel;


namespace NSAP_ODK.Entities.Database
{
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
            FirstSampledLandingDate = NSAPEntities.VesselUnloadViewModel.DateOfFirstSampledLanding;
            LastSampledLandingDate = NSAPEntities.VesselUnloadViewModel.DateOfLastSampledLanding;
            CountCompleteGearUnload = NSAPEntities.GearUnloadViewModel.CountCompletedGearUnload;
        }
        public DBSummary()
        {

        }

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
        public DateTime FirstSampledLandingDate { get; set; }
        [ReadOnly(true)]
        public DateTime LastSampledLandingDate { get; set; }
        [ReadOnly(true)]
        public int CountCompleteGearUnload { get; set; }


    }
}
