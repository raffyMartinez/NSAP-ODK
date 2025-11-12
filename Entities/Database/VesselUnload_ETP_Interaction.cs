using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class VesselUnload_ETP_Interaction
    {
        public int VesselUnloadID { get; set; }
        public VesselUnload VesselUnload { get; set; }

        public bool HasETPAndGearInteraction {get;set;}
        public bool HasMarineMammal { get; set; }
        public bool HasSeaTurtle { get; set; }
        public bool HasShark { get; set; }
        public bool HasRay { get; set; }
        public bool HasETPEscapeFromGear { get; set; }
        public bool HasETPReleaseFromGear { get; set; }
        public bool HasETPInjuryAndReleaseFromGear { get; set; }
        public bool HasETPMortality { get; set; }
        public bool HasETPOtherInteraction { get; set; }
        public string ETPOtherInteraction { get; set; }



    }
}
