using NSAP_ODK.Entities.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class SamplingDaySubmission
    {
        public int SamplingDayID { get; set; }
        public int? LandingSiteID { get; set; }
        public string LandingSiteText { get; set; }
        public string FishingGroundID { get; set; }
        public DateTime SamplingDate { get; set; }
        public LandingSiteSampling LandingSiteSampling { get; set; }
    }
}
