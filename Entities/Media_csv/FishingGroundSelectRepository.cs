using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Media_csv
{
    public class FishingGroundSelectRepository
    {
        public List<FishingGroundSelect> FishingGroundSelects { get; set; }

        private string _pathToCSV;
        public FishingGroundSelectRepository(string pathToCSV)
        {
            _pathToCSV = pathToCSV;
            FishingGroundSelects = getFishingGroundSelects();

        }

        private List<FishingGroundSelect> getFishingGroundSelects()
        {
            List<FishingGroundSelect> values = File.ReadAllLines(_pathToCSV)
                                           .Where(t => t.Length > 0)
                                           .Skip(1)
                                           .Select(v => ParseCSVLine(v))
                                           .ToList();
            return values;
        }

        private FishingGroundSelect ParseCSVLine(string csvLine)
        {
            var arr = csvLine.Split(',');
            var regionFMAID = Convert.ToInt32(arr[3]);
            var es = new FishingGroundSelect
            {
                RowID = Convert.ToInt32(arr[1]),
                Name = arr[2].Replace("\"", ""),
                NSAPRegionFMAID = regionFMAID,
                NSAPRegionFMA = GetRegionFMA(regionFMAID)
            };
            return es;
        }

        private NSAPRegionFMA GetRegionFMA(int regionFMAID)
        {
            return NSAPEntities.NSAPRegionViewModel.CurrentEntity.FMAs.FirstOrDefault(t => t.RowID == regionFMAID);
        }
    }
}
