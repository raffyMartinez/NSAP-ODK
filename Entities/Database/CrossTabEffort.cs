using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CrossTabEffort
    {
        public CrossTabCommon CrossTabCommon { get; set; }

        public VesselUnload VesselUnload { get; set; }
    }

    public class CrossTabEffort_VesselUnloadGear
    {
        public CrossTabCommon CrossTabCommon { get; set; }

        public VesselUnload_FishingGear VesselUnload_FishingGear{ get; set; }
    }
}
