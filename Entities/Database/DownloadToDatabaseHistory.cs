using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{

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

    }
}
