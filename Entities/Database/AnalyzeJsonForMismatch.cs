using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database.FromJson;

namespace NSAP_ODK.Entities.Database
{
    public static class AnalyzeJsonForMismatch
    {
        private static string _resultStatus;
        public static void Reset()
        {
            VesselLandings = null;
        }
        public static string ResultsStatus
        {
            get { return _resultStatus; }
        }
        public static List<VesselLanding> VesselLandings { get; set; }

        private static bool AnalyzeLandings()
        {
            var orphanedEnumerators =  VesselLandings.Where(t => t.RegionEnumeratorID == null).Select(t => t.EnumeratorName).ToHashSet();
            var unmatchedEnumeratorID =  VesselLandings.Where(t => t.EnumeratorName == "NOT RECOGNIZED").Select(t => t.RegionEnumeratorID).ToHashSet();
            var orphanedLandingSites = VesselLandings.Where(t => t.LandingSiteID == null).Select(t => t.LandingSiteName).ToHashSet();
            var unmatchaedLandingSiteIDs = VesselLandings.Where(t => t.LandingSiteName == "NOT RECOGNIZED").Select(t => t.LandingSiteID).ToHashSet();
            var orphanedGearNames = VesselLandings.Where(t => t.GearCode == "_OT").Select(t => t.GearNameToUse).ToHashSet();
            var unmatchedGearCodes = VesselLandings.Where(t => t.GearNameToUse == "NOT RECOGNIZED").Select(t => t.GearCode).ToHashSet();
            
            return false;
        }
        public static void Analyze()
        {
            if(VesselLandings!=null && VesselLandings.Count>0)
            {
                if (AnalyzeLandings())
                {
                    Reset();
                }
            }
            else
            {
                _resultStatus = "Vessel landings is null or empty";
            }
        }
    }
}
