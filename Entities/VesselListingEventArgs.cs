using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class VesselListingEventArgs:EventArgs
    {
        public string Intent { get; set; }

        public int? ListCount { get; set; }
    }
}
