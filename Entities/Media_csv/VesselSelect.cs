using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class VesselSelect
    {
        public int RowID { get; set; }

        public FisheriesSector FisheriesSector { get; set; }

        public string Name { get; set; }

        public string NSAPRegionID { get; set; }

        public NSAPRegion NSAPRegion { get; set; }
    }
}
