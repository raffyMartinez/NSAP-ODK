using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class ProvinceEventHandler:EventArgs
    {
        public Province Province { get; set; }
    }
}
