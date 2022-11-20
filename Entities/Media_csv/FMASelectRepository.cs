using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Media_csv
{
    public class FMASelectRepository
    {
        public List<FMASelect> FMASelects{ get; set; }

        private string _pathToCSV;
        public FMASelectRepository(string pathToCSV)
        {
            _pathToCSV = pathToCSV;
            FMASelects = getFMASelects();

        }

        private List<FMASelect> getFMASelects()
        {
            List<FMASelect> values = File.ReadAllLines(_pathToCSV)
                                           .Where(t => t.Length > 0)
                                           .Skip(1)
                                           .Select(v => ParseCSVLine(v))
                                           .ToList();
            return values;
        }

        private FMASelect ParseCSVLine(string csvLine)
        {
            var arr = csvLine.Split(',');
            var regionCode = arr[3];
            var fs = new FMASelect
            {
                RowID = Convert.ToInt32(arr[1]),
                Name = arr[2].Replace("\"", ""),
                NsapRegionCode = regionCode,
                NSAPRegion = GetNSAPRegion(regionCode)
            };
            return fs;
        }

        private NSAPRegion GetNSAPRegion(string regionCode)
        {
            return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(regionCode);
        }
    }
}
