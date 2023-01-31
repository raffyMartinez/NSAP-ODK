using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SetFishingGroundOfUnloadEventArg : EventArgs
    {
        public int TotalVesselUnloads { get; set; }
        public string Intent { get; set; }

        public int CountFishingGroundChanged { get; set; }

    }
}
