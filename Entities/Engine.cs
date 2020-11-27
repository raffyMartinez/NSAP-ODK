using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class Engine
    {
        public int EngineID { get; set; }
        public double HorsePower { get; set; }

        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
    }
}
