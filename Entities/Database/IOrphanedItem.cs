using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public interface IOrphanedItem
    {
        NSAPRegion Region { get; set; }
        FMA FMA { get; set; }

        bool ForReplacement { get; set; }

    }
}
