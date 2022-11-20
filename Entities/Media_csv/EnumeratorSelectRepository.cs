using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Media_csv
{
    class EnumeratorSelectRepository
    {
        public List<EnumeratorSelect> EnumeratorSelects { get; set; }

        private string _pathToCSV;
        public EnumeratorSelectRepository(string pathToCSV)
        {
            _pathToCSV = pathToCSV;
            EnumeratorSelects = getEnumeratorSelects();

        }

        private List<EnumeratorSelect> getEnumeratorSelects()
        {
            List<EnumeratorSelect> values = File.ReadAllLines(_pathToCSV)
                                           .Where(t => t.Length > 0)
                                           .Skip(1)
                                           .Select(v => ParseCSVLine(v))
                                           .ToList();
            return values;
        }

        private EnumeratorSelect ParseCSVLine(string csvLine)
        {
            //var arr = csvLine.Split(',');
            var arr = Utilities.Global.SplitQualified(csvLine, ',', '"', true);
            string nsapRegion = arr[3];
            var es = new EnumeratorSelect
            {
                RowID = Convert.ToInt32(arr[1]),
                Name = arr[2].Replace("\"", ""),
                NSAPRegionCode = nsapRegion,
                NSAPRegion = GetNSAPRegion(nsapRegion)
            };
            return es;
        }

        private NSAPRegion GetNSAPRegion(string regionCode)
        {
            return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(regionCode);
        }
    }
}
