using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class GPS
    {
        public string Code { get; set; }
        public string AssignedName { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public override string ToString()
        {
            return AssignedName;
        }
    }
}
