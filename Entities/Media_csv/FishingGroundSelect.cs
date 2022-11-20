using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class FishingGroundSelect
    {
        public int RowID { get; set; }
        public string Name { get; set; }
        public int NSAPRegionFMAID { get; set; }
        public NSAPRegionFMA NSAPRegionFMA { get; set; }
    }
}
