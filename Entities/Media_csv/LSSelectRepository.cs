using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class LSSelectRepository
    {
        public List<LSSelect> LSSelects { get; set; }

        private string _pathToCSV;
        public LSSelectRepository(string pathToCSV)
        {
            _pathToCSV = pathToCSV;
            LSSelects = getLSSelects();

        }

        private List<LSSelect> getLSSelects()
        {
            List<LSSelect> values = File.ReadAllLines(_pathToCSV)
                                           .Where(t=>t.Length>0)
                                           .Skip(1)
                                           .Select(v => ParseCSVLine(v))
                                           .ToList();
            return values;
        }

        private LSSelect ParseCSVLine(string csvLine)
        {
            //var arr = csvLine.Split(',');

            var arr = Utilities.Global.SplitQualified(csvLine, ',', '"', true);
            var nsapRegionFMAFishingGround_ID = Convert.ToInt32(arr[3]);
            var ls = new LSSelect
            {
                RowID = Convert.ToInt32(arr[1]),
                Name = arr[2].Replace("»", ","),
                NSAPRegionFMAFishingGround_ID = nsapRegionFMAFishingGround_ID,
                NSAPRegionFMAFishingGround = GetRegionFMAFishingGround(nsapRegionFMAFishingGround_ID)
            };
            return ls;
        }

        private NSAPRegionFMAFishingGround GetRegionFMAFishingGround(int id)
        {
            foreach(var fma in NSAPEntities.NSAPRegionViewModel.CurrentEntity.FMAs)
            {
                foreach(var fg in fma.FishingGrounds)
                {
                    if(fg.RowID==id)
                    {
                        return fg;
                    }
                }
            }
            return null;
        }
    }
}
