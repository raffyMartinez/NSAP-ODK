using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public  class DownloadHistoryEntity
    {
        public static DateTime DownloadDate { get; set; }
        public static List<VesselUnload> VesselUnloads { get; set; }
        public static List<LandingSiteSampling> LandingSiteGearUnload { get; set; }
    }
   public static class DownloadToDatabaseHistory
    {
        public static Dictionary<DateTime, List<VesselUnload>> DownloadToDatabaseHistoryDictionary 
        {
            get 
            {
                
                return NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                    .OrderByDescending(t=>t.DateAddedToDatabase)
                    .GroupBy(t => ((DateTime)t.DateAddedToDatabase).Date)
                    .ToDictionary(t => t.Key, t => t.ToList());
            }
            
        }

        public static Dictionary<DateTime,DownloadHistoryEntity> DownloadToDatabaseHistoryDictionary1
        {
            get
            {
                return null;
            }
        }

    }
}
