using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;

namespace NSAP_ODK.Views
{
    public class EnumeratorLandingSiteSummary
    {
        public LandingSite LandingSite { get; set; }

        public string GearsListed
        {
            get
            {
                int counter = 0;
                var s = "";
                foreach (var g in GearsUsed)
                {
                    s += $"{g}, ";
                    counter++;
                    if (counter % 3 == 0)
                    {
                        s += "\r\n";
                    }
                }
                return s.Trim(new char[] { ',', ' ' });
            }
        }

        public List<string> GearsUsed { get; set; }
        public int CountSampledLandings { get; set; }

        public DateTime FirstSampling { get; set; }

        public DateTime LastSampling { get; set; }
        public string FirstSamplingText { get { return FirstSampling.ToString("MMM-dd-yyyy"); } }
        public string LastSamplingText { get { return LastSampling.ToString("MMM-dd-yyyy"); } }

        public string LandingSiteName { get; set; }
    }
}
