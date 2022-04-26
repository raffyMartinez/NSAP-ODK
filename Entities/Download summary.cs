using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class Download_summary
    {
        public string Enumerator { get; set; }
        public string Gear { get; set; }
        public DateTime? DownloadDate { get; set; }

        public DateTime? EarliestSamplingDate { get; set; }
        public DateTime? LatestSamplingDate { get; set; }
        public int? NumberLandings { get; set; }
        public int? NumberLandingsWithCatchComposition { get; set; }
        public int? NumberOfTrackedLandings { get; set; }

        public string EarliestSamplingDateString
        {
            get {
                if (EarliestSamplingDate == null)
                {
                    return "";
                }
                else
                {
                    return ((DateTime)EarliestSamplingDate).ToString("MMM-dd-yyyy HH:mm");
                }
            }
        }
        public string LatestSamplingDateString
        {
            get {
                if (LatestSamplingDate == null)
                {
                    return "";
                }
                else
                {
                    return ((DateTime)LatestSamplingDate).ToString("MMM-dd-yyyy HH:mm");
                }
            }
        }

        public string DownloadDateString
        {
            get
            {
                if (DownloadDate == null)
                {
                    return "";
                }
                else
                {
                    return ((DateTime)DownloadDate).ToString("MMM-dd-yyyy HH:mm");
                }
            }
        }
    }
}
