using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedLandingSite
    {
        public bool CanBeDeletedNow { get; set; }
        public string LandingSiteName { get; set; }
        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }
        public NSAPRegion Region { get; set; }
        //public NSAPRegion Region
        //{
        //    get
        //    {
        //        if (LandingSiteSamplings.Count > 0)
        //        {
        //            return LandingSiteSamplings[0].NSAPRegion;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public FMA FMA { get; set; }
        //public FMA FMA
        //{
        //    get
        //    {
        //        if (LandingSiteSamplings.Count > 0)
        //        {
        //            return LandingSiteSamplings[0].FMA;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
        public FishingGround FishingGround { get; set; }
        //public FishingGround FishingGround
        //{
        //    get
        //    {
        //        if (LandingSiteSamplings.Count > 0)
        //        {
        //            return LandingSiteSamplings[0].FishingGround;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}


        public string EnumeratorName { get; set; }


        public int NumberOfLandings { get { return LandingSiteSamplings.Count; } }

        public bool ForReplacement { get; set; }

        public int NumberOfVesselLandings { get; set; }

    }
}
