using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class DeleteRegionEntityFail
    {
        public NSAPEntity NSAPEntity { get; set; }
        public int EntityID { get; set; }

        public int RegionEntityID { get; set; }
        public string ErrorMessage { get; set; }
        public string EntityTable { get; set; }
        public string RegionEntityTable { get; set; }
        public string AffectedTable { get; set; }
    }
}
