using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class OrphanedEnumeratorFromCarrierLandings:IOrphanedItem
    {
        public string Enumerator { get; set; }
        public bool ForReplacement { get; set; }
        public NSAPRegion Region { get; set; }
        public FMA FMA { get; set; }
        public string LandingSite { get; set; }
        public int Number_of_landings { get; set; }
    }
}
