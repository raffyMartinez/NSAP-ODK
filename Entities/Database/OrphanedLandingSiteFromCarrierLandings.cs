using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedLandingSiteFromCarrierLandings:IOrphanedItem
    {
        public bool ForReplacement { get; set; }
        public NSAPRegion Region{ get; set; }
        public FMA FMA{ get; set; }
        public string EnumeratorName { get; set; }
        public string LandingSiteName { get; set; }
        public int NumberOfSampledLandings { get; set; }
    }
}
