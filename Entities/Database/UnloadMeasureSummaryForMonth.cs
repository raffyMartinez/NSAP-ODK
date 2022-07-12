using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class UnloadMeasureSummaryForMonth
    {
        public DateTime Month { get; set; }
        public string LandingSiteName { get; set; }

        public int? CountLW { get; set; }
        public int? CountLF { get; set; }
        public int? CountL { get; set; }
        public int? CountGMS { get; set; }
    }
}
