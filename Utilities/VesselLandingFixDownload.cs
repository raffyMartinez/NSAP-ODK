using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
namespace NSAP_ODK.Utilities
{
    public static class VesselLandingFixDownload
    {
        private static List<Entities.Database.FromJson.VesselLanding> _listLandingsToRepair = new List<Entities.Database.FromJson.VesselLanding>();
        public  static void VesselLandingToRepair(Entities.Database.FromJson.VesselLanding vesselLanding)
        {
            _listLandingsToRepair.Add(vesselLanding);
        }
        public static int Count
        {
            get { return _listLandingsToRepair.Count; }
        }
        public static bool RepairVesselLanding(string userName, string password)
        {
            if(_listLandingsToRepair.Count>0)
            {

            }
            return true;
        }
    }
}
