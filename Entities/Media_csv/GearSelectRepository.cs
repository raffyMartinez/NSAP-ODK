using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Media_csv
{
    class GearSelectRepository
    {

        public List<GearSelect> GearSelects { get; set; }

        private string _pathToCSV;
        public GearSelectRepository(string pathToCSV)
        {
            _pathToCSV = pathToCSV;
            GearSelects = getGearSelects();

        }

        private List<GearSelect> getGearSelects()
        {
            List<GearSelect> values = File.ReadAllLines(_pathToCSV)
                                           .Where(t => t.Length > 0)
                                           .Skip(1)
                                           .Select(v => ParseCSVLine(v))
                                           .ToList();
            return values;
        }

        private GearSelect ParseCSVLine(string csvLine)
        {
            var arr = csvLine.Split(',');
            var regionCode = arr[3];
            var gs = new GearSelect
            {
                RowID = Convert.ToInt32(arr[1]),
                Name = arr[2].Replace("\"", ""),
                RegionCode = regionCode,
                NSAPRegion = GetNSAPRegion(regionCode)
            };
            return gs;
        }

        private NSAPRegion GetNSAPRegion(string regionCode)
        {
            return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(regionCode);
        }
    }
}
