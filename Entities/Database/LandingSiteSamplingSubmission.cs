using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteSamplingSubmission
    {
        public string SubmissionID { get; set; }
        public DateTime DateAdded { get; set; }
        public string JSONFile { get; set; }
        public LandingSiteSampling LandingSiteSampling { get; set; }

        public string XFormIdentifier { get; set; }

        public bool DelayedSave { get; set; }
    }
}
