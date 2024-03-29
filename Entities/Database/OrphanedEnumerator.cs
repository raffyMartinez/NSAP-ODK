﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedEnumerator
    {
        public List<SummaryItem> SummaryItems { get; set; }
        public GearUnload GearUnload { get; set; }
        public NSAPRegion Region { get; set; }

        public LandingSiteSampling LandingSiteSampling { get; set; }
        //public NSAPRegion Region
        //{
        //    get
        //    {
        //        if (SampledLandings.Count == 0)
        //        {
        //            return LandingSiteSamplings[0].NSAPRegion;
        //        }
        //        else
        //        {
        //            return SampledLandings[0].Parent.Parent.NSAPRegion;
        //        }
        //    }
        //}

        public FMA FMA { get; set; }
        //public FMA FMA
        //{
        //    get
        //    {
        //        if (SampledLandings.Count == 0)
        //        {
        //            return LandingSiteSamplings[0].FMA;
        //        }
        //        else
        //        {
        //            return SampledLandings[0].Parent.Parent.FMA;
        //        }
        //    }
        //}

        public FishingGround FishingGround { get; set; }

        //public FishingGround FishingGround
        //{
        //    get
        //    {
        //        if (SampledLandings.Count == 0)
        //        {
        //            return LandingSiteSamplings[0].FishingGround;
        //        }
        //        else
        //        {
        //            return SampledLandings[0].Parent.Parent.FishingGround;
        //        }
        //    }
        //}

        public string Name { get; set; }

        public List<VesselUnload> SampledLandings { get; set; }

        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

        public bool ForReplacement { get; set; }

        public int NumberOfVesselCountings
        {
            get
            {
                if (LandingSiteSamplings == null)
                {
                    return 0;
                }
                else
                {
                    return LandingSiteSamplings.Count;
                }
            }
        }
        public int NumberOfLandings { get { return SampledLandings.Count; } }

        public string LandingSiteName { get; set; }
        //public string LandingSiteName
        //{
        //    get
        //    {
        //        if (SampledLandings.Count == 0)
        //        {
        //            return LandingSiteSamplings[0].LandingSiteName;
        //        }
        //        else
        //        {
        //            return SampledLandings[0].Parent.Parent.LandingSiteName;
        //        }
        //    }
        //}


    }

}
