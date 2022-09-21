using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class FBSpecies
    {
        public string Genus { get; set; }
        public string Species { get; set; }
        public string Family { get; set; }
        public int SpCode { get; set; }
        public string Importance { get; set; }
        public string MainCatchingMethod { get; set; }

        public string DemersalPelagic { get; set; }
        public double? LengthMax { get; set; }
        public double? LengthCommon { get; set; }
        public string LengthType { get; set; }
        public string LengthMaxLengthType { get; set; }

        public override string ToString()
        {
            return $"{Genus} {Species} SPCode:{SpCode}"; ;
        }
    }
}
