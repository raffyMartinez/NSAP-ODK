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
        private static List<Entities.Database.VesselLanding> _listLandingsToRepair = new List<Entities.Database.VesselLanding>();
        public static void VesselLandingToRepair(Entities.Database.VesselLanding vesselLanding)
        {
            _listLandingsToRepair.Add(vesselLanding);
        }
        public static int Count
        {
            get { return _listLandingsToRepair.Count; }
        }
        public static bool RepairVesselLanding(string userName, string password)
        {
            if (_listLandingsToRepair.Count > 0)
            {

            }
            return true;
        }

        //search and replace paths in updated xform to match paths in original xform
        // might replace with Regex.Replace() if warranted by performance concerns
        // https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.replace?redirectedfrom=MSDN&view=net-6.0#overloads
        public static string JsonNewToOldVersion(string json)
        {

            string json1 = json.Replace(
                "catch_comp_group/catch_composition_repeat/speciesname_group/",
                "catch_comp_group/catch_composition_repeat/");

            json1 = json1.Replace(
                "catch_comp_group/catch_composition_repeat/length_list_repeat/length_list_group/",
                "catch_comp_group/catch_composition_repeat/length_list_repeat/");

            json1 = json1.Replace(
                "catch_comp_group/catch_composition_repeat/species_data_group/",
                "catch_comp_group/catch_composition_repeat/speciesname_group/");

            json1 = json1.Replace("vessel_sampling/sampling_group_1", "vessel_sampling");

            json1 = json1.Replace("vessel_sampling/sampling_group_2", "vessel_sampling");


            return json1;
        }
    }
}
