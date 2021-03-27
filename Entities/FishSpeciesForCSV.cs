using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class FishSpeciesForCSV
    {
        public int SpeciesCode { get; set; }

        public string SortName { get; set; }
        public string Name { get; set; }
        public SizeType LengthType { get; set; }

        public double? MaxLength { get; set; }
    }
}
