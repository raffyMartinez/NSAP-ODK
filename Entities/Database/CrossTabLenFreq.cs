using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CrossTabLenFreq
    {
                public CrossTabCarrierLandingVesselCatchCommon CrossTabCarrierLandingVesselCatchCommon { get; set; }
        public CrossTabCommon CrossTabCommon { get; set; }
        public double Length { get; set; }
        public int Freq { get; set; }

        public string Sex { get; set; }
    }
}
