using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class LSSelect
    {
        public int RowID { get; set; }
        public string Name { get; set; }

        //public NSAPRegionFMAFishingGround NSAPRegionFMAFishingGround { get { return NSAPRegionWithEntitiesRepository.} }

        public int NSAPRegionFMAFishingGround_ID { get; set; }
        public NSAPRegionFMAFishingGround NSAPRegionFMAFishingGround { get; set; }

        public override string ToString()
        {
            return $"{Name} - RowID:{RowID} - FMAFGID:{NSAPRegionFMAFishingGround_ID}"; ;
        }
    }
}
