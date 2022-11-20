using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class FMASelect
    {
        public int RowID { get; set; }
        public string Name{ get; set; }
        public string NsapRegionCode { get; set; }

        public NSAPRegion NSAPRegion { get; set; }
    }
}
