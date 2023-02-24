using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities.Database
{
    public static class AnalyzeJsonForMismatch
    {
        private static string _resultStatus = "";
        private static bool _successAnalyze;
        private static JSONFile _jSONFile;
        public static void Reset()
        {
            _jSONFile = null;
            _resultStatus = "";
            OrphanedEnumerators = null;
            UnmatchedEnumeratorIDs = null;
            OrphanedlandingSites = null;
            UnmatchedlandingSiteIDs = null;
            VesselLandings = null;
            UnmatchedGearCodes = null;
            OrphanedGearNames = null;
            OrphanedSpeciesNamesTaxa = null;
        }

        public static HashSet<string> OrphanedSpeciesNamesTaxa { get; private set; }
        public static List<VesselLanding> VesselLandings { get; set; }
        public static string JSONFileName { get; set; }

        public static HashSet<string> OrphanedEnumerators { get; private set; }
        public static HashSet<int> UnmatchedEnumeratorIDs { get; private set; }

        public static HashSet<string> OrphanedlandingSites { get; private set; }
        public static HashSet<int> UnmatchedlandingSiteIDs { get; private set; }

        public static HashSet<string> OrphanedGearNames { get; private set; }
        public static HashSet<string> UnmatchedGearCodes { get; private set; }

        public static string ResultStatus { get { return _resultStatus; } }

        public static bool Save()
        {
            bool success = false;
            //if (_successAnalyze)
            //{
            UnmatchedFieldsFromJSONFile ufjf = new UnmatchedFieldsFromJSONFile
            {
                LandingSiteIDs = UnmatchedlandingSiteIDs.ToList(),
                LandingSiteNames = OrphanedlandingSites.ToList(),
                FishingGearCodes = UnmatchedGearCodes.ToList(),
                FishingGearNames = OrphanedGearNames.ToList(),
                EnumeratorIDs = UnmatchedEnumeratorIDs.ToList(),
                EnumeratorNames = OrphanedEnumerators.ToList(),
                SpeciesNamesTaxa = OrphanedSpeciesNamesTaxa.ToList(),
                JSONFileName = JSONFileName,
                NSAPRegion = NSAPEntities.NSAPRegionViewModel.CurrentEntity,
                DateStart = _jSONFile.Earliest,
                DateEnd = _jSONFile.Latest,
                DateOfParsing = _jSONFile.DateAdded,
                RowID = _jSONFile.RowID
            };

            success = NSAPEntities.UnmatchedFieldsFromJSONFileViewModel.AddRecordToRepo(ufjf);
            //}
            return success;
        }
        private static bool AnalyzeLandings(List<VesselLanding> vls = null)
        {
            bool hasLandingsAndJsonFile = vls != null && _jSONFile != null;
            if (hasLandingsAndJsonFile)
            {
                VesselLandings = vls;
                
            }
            else
            {
                VesselLandings = _jSONFile.VesselLandings;
            }
            JSONFileName = _jSONFile.FullFileName;
            OrphanedEnumerators = VesselLandings.Where(t => t.RegionEnumeratorID == null).Select(t => t.EnumeratorName).ToHashSet();
            UnmatchedEnumeratorIDs = VesselLandings.Where(t => t.EnumeratorName == "NOT RECOGNIZED").Select(t => (int)t.RegionEnumeratorID).ToHashSet();
            OrphanedlandingSites = VesselLandings.Where(t => t.LandingSiteID == null).Select(t => t.LandingSiteName).ToHashSet();
            UnmatchedlandingSiteIDs = VesselLandings.Where(t => t.LandingSiteName == "NOT RECOGNIZED").Select(t => (int)t.LandingSiteID).ToHashSet();
            OrphanedGearNames = VesselLandings.Where(t => t.GearCode == "_OT").Select(t => t.GearNameToUse).ToHashSet();
            UnmatchedGearCodes = VesselLandings.Where(t => t.GearNameToUse == "NOT RECOGNIZED").Select(t => t.GearCode).ToHashSet();

            HashSet<string> orpSpecies = new HashSet<string>();
            foreach (VesselLanding l in VesselLandings.Where(t => t.CatchComposition != null))
            {
                //var os = l.CatchComposition.Where(t => t.SpeciesID == null).Select(t => t.SpeciesName).ToHashSet();
                //var os = l.CatchComposition.Where(t => t.SpeciesID == null).Select(i => new { i.Taxa, i.SpeciesName });

                orpSpecies.UnionWith(l.CatchComposition.Where(t => t.SpeciesID == null).Select(t => t.TaxaSpecies).ToHashSet());
            }
            OrphanedSpeciesNamesTaxa = orpSpecies.OrderBy(t => t).ToHashSet();

            //if (!hasLandingsAndJsonFile)
            //{
            return OrphanedEnumerators.Count > 0 ||
                UnmatchedEnumeratorIDs.Count > 0 ||
                OrphanedGearNames.Count > 0 ||
                UnmatchedGearCodes.Count > 0 ||
                OrphanedlandingSites.Count > 0 ||
                UnmatchedlandingSiteIDs.Count > 0 ||
                OrphanedSpeciesNamesTaxa.Count > 0;
            //}
            //else
            //{
            //    return Save();
            //}
        }
        //public static bool Analyze(List<VesselLanding> vls = null, string jf = null)


        public static bool Analyze(List<VesselLanding> vls = null, JSONFile jsonFile = null)
        {

            if (jsonFile != null && jsonFile.VesselLandings != null)
            {
                Reset();
                _jSONFile = jsonFile;
                if (AnalyzeLandings())
                {
                    _successAnalyze = Save();
                }

            }
            else if (vls != null && jsonFile != null)
            {
                Reset();
                _jSONFile = jsonFile;
                if (AnalyzeLandings(vls))
                {
                    _successAnalyze = Save();
                }
            }
            else if (VesselLandings != null && VesselLandings.Count > 0)
            {
                _successAnalyze = AnalyzeLandings();
            }
            else
            {
                _resultStatus = "Vessel landings is null or empty";
                _successAnalyze = false;
            }
            return _successAnalyze;
        }
    }
}
