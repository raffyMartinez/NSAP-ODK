using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class ParentIDs
    {
        public int VesselUnloadID { get; set; }
        public int LandingSiteSamplingID { get; set; }
        public int GearUnloadID { get; set; }

        public VesselUnload VesselUnload
        {
            get
            {
                LandingSiteSampling lss = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSamling(LandingSiteSamplingID);
                GearUnload gu= lss.GearUnloadViewModel.GetGearUnload(GearUnloadID);
                if(gu.VesselUnloadViewModel==null)
                {
                    gu.VesselUnloadViewModel = new VesselUnloadViewModel(gu);
                }
                return gu.VesselUnloadViewModel.getVesselUnload(VesselUnloadID);
            }
        }
    }
}
