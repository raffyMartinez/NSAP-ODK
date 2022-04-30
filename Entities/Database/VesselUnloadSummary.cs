using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database    
{
    public class VesselUnloadSummary
    {
        public DateTime FirstSamplingDate { get; set; }

        public DateTime LastSamplingDate { get; set; }

        public DateTime LatestDownloadDate { get; set; }

        public int CountUnloadsWithCatchComposition { get; set; }
    }
}
