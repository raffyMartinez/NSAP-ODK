using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class TrackedLandingCentroid
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string DeviceName { get; set; }
        public DateTime End { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime Start { get; set; }
        public string SourceType { get; set; }
        public int SourceID { get; set; }
        public string Gear { get; set; }
        public string LGU { get; set; }
        public int MWShapeID { get; set; }
    }
}
