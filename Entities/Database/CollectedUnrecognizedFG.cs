using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CollectedUnrecognizedFG
    {
        public DateTime DateCreated { get; set; }
        public List<UnrecognizedFishingGround> UnrecognizedFishingGrounds { get; set; }
    }
}
