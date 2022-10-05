using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class DeleteVesselUnloadFromOrphanEventArg : EventArgs
    {
        public int VesselUnloadTotalCount { get; set; }
        public int DeletedCount { get; set; }
        public string Intent { get; set; }

        public NSAPEntity NSAPEntity { get; set; }

    }
}
