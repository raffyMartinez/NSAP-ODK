using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedEnumerator
    {

        public NSAPRegion NSAPRegion { get; set; }

        public NSAPRegion Region
        {
            get
            {
                if (SampledLandings.Count == 0)
                {
                    return LandingSiteSamplings[0].NSAPRegion;
                }
                else
                {
                    return SampledLandings[0].Parent.Parent.NSAPRegion;
                }
            }
        }

        public FMA FMA
        {
            get
            {
                if (SampledLandings.Count == 0)
                {
                    return LandingSiteSamplings[0].FMA;
                }
                else
                {
                    return SampledLandings[0].Parent.Parent.FMA;
                }
            }
        }

        public FishingGround FishingGround
        {
            get
            {
                if (SampledLandings.Count == 0)
                {
                    return LandingSiteSamplings[0].FishingGround;
                }
                else
                {
                    return SampledLandings[0].Parent.Parent.FishingGround;
                }
            }
        }

        public string Name { get; set; }

        public List<VesselUnload> SampledLandings { get; set; }

        public List<LandingSiteSampling> LandingSiteSamplings { get; set; }

        public bool ForReplacement { get; set; }

        public int NumberOfVesselCountings { get { return LandingSiteSamplings.Count; } }
        public int NumberOfLandings { get { return SampledLandings.Count; } }

        public string LandingSiteName
        {
            get
            {
                if (SampledLandings.Count == 0)
                {
                    return LandingSiteSamplings[0].LandingSiteName;
                }
                else
                {
                    return SampledLandings[0].Parent.Parent.LandingSiteName;
                }
            }
        }


    }

}
