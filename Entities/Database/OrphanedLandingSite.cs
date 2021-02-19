using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedLandingSite
    {

        public string LandingSiteName { get; set; }
        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

        public NSAPRegion Region { get { return LandingSiteSamplings[0].NSAPRegion; } }

        public FMA FMA { get { return LandingSiteSamplings[0].FMA; } }

        public FishingGround FishingGround { get { return LandingSiteSamplings[0].FishingGround; } }
        public int NumberOfLandings
        {
            get
            {
                //return eturn NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                //    .Count(t =>t => t.Parent.Parent.NSAPRegionID == Region.Code &&
                //            t.Parent.Parent.FMAID == FMA.FMAID &&
                //            t.Parent.Parent.FishingGroundID == FishingGround.Code &&
                //            t.Parent.Parent.LandingSiteID == null &&
                //            t.Parent.Parent.LandingSiteText == LandingSiteName);

                //int count = 0;
                //foreach (var sampling in LandingSiteSamplings)
                //{
                //    //count += NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                //    //    .Count(t => t.Parent.Parent.PK == sampling.PK);
                //    count += NSAPEntities.GearUnloadViewModel.GetGearUnloads(sampling).Count;

                //}
                //return count;

                return LandingSiteSamplings.Count;
            }

        }

        public bool ForReplacement { get; set; }

    }
}
