using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
   public class CrossTabLengthWeight
    {
        public CrossTabCommon CrossTabCommon { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
    }
}
