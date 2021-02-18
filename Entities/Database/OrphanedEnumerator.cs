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

        public NSAPRegion Region { get { return SampledLandings[0].Parent.Parent.NSAPRegion; } }

        public FMA FMA { get { return SampledLandings[0].Parent.Parent.FMA; } }

        public FishingGround FishingGround { get { return SampledLandings[0].Parent.Parent.FishingGround; } }

        public string Name { get; set; }

        public List<VesselUnload> SampledLandings{ get; set; }

         public bool ForReplacement { get; set; }

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


                return SampledLandings.Count;
            }

        }


    }

}
