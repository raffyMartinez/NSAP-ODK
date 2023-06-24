using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    class CrossTabEffortAll
    {
        public CrossTabCommon CrossTabCommon { get; set; }
        public VesselUnload VesselUnload { get; set; }
    }

    class CrossTabEffortAll_VesselUnloadGear
    {
        public CrossTabCommon CrossTabCommon { get; set; }
        public VesselUnload_FishingGear VesselUnload_FishingGear { get; set; }
    }
}
