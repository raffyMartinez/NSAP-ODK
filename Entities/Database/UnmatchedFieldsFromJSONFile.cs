using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Database
{
    public class UnmatchedFieldsFromJSONFile
    {
        public int RowID { get; set; }
        public string FileName
        {
            get { return Path.GetFileName(JSONFileName); }
        }
        private string StringifyNumericList(List<int> numbers)
        {
            string result = "";
            foreach (var number in numbers)
            {
                result += $"{number},";
            }
            return result.Trim(',');
        }

        private string StringifyStringList(List<string> strings)
        {
            string result = "";
            foreach (var s in strings)
            {
                result += $"\"{s}\",";
            }
            return result.Trim(',');
        }
        private string ProcessList(string whatList)
        {
            string result = "";
            switch (whatList)
            {
                case "landing_site_ids":
                    result = StringifyNumericList(LandingSiteIDs);
                    break;
                case "landing_site_names":
                    result = StringifyStringList(LandingSiteNames);
                    break;
                case "enumerator_ids":
                    result = StringifyNumericList(EnumeratorIDs);
                    break;
                case "enumerator_names":
                    result = StringifyStringList(EnumeratorNames);
                    break;
                case "gear_ids":
                    result = StringifyStringList(FishingGearCodes);
                    break;
                case "gear_names":
                    result = StringifyStringList(FishingGearNames);
                    break;
                case "species_ids":
                    result = StringifyNumericList(SpeciesIDs);
                    break;
                case "species_names":
                    result = StringifyStringList(SpeciesNames);
                    break;
                case "species_taxa":
                    result = StringifyStringList(SpeciesNamesTaxa);
                    break;
            }
            return result;
        }

        public string AllSpeciesIDs
        {
            get
            {
                return ProcessList("species_ids");
            }
        }

        public string AllSpeciesNames
        {
            get
            {
                return ProcessList("species_names");
            }
        }
        public string AllLandingSiteIDs
        {
            get
            {
                return ProcessList("landing_site_ids");
            }
        }

        public string AllEnumeratorIDs
        {
            get
            {
                return ProcessList("enumerator_ids");
            }
        }
        public string AllGearIDs
        {
            get
            {
                return ProcessList("gear_ids");
            }
        }

        public string AllGearNames
        {
            get
            {
                return ProcessList("gear_names");
            }
        }

        public string AllEnumeratorNames
        {
            get
            {
                return ProcessList("enumerator_names");
            }
        }

        public string AllLandingSiteNames
        {
            get
            {
                return ProcessList("landing_site_names");
            }
        }

        public string AllSpeciesTaxa
        {
            get
            {
                return ProcessList("species_taxa");
            }
        }
        public List<int> LandingSiteIDs { get; set; }
        public List<string> LandingSiteNames { get; set; }
        public List<int> EnumeratorIDs { get; set; }
        public List<string> EnumeratorNames { get; set; }
        public string JSONFileName { get; set; }

        public int JsonFileID { get; set; }
        public NSAPRegion NSAPRegion { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public DateTime DateOfParsing { get; set; }

        public List<string> FishingGearCodes { get; set; }
        public List<string> FishingGearNames { get; set; }

        public List<int> SpeciesIDs { get; set; }
        public List<string> SpeciesNames { get; set; }
        public List<string> SpeciesNamesTaxa { get; set; }

    }
}
