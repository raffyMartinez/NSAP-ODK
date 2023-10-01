using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class VesselCatchWV
    {
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }
        public double? Species_kg { get; set; }
        public double? Species_sample_kg { get; set; }
        public bool FromTotalCatch { get; set; }

        public int? VesselUnload_GearID { get; set; }
    }
}
