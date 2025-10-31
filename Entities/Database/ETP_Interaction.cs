using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class ETP_Interaction
    {
        public int VesselUnloadID { get; set; }
        public int RowID { get; set; }

        public string Interaction { get; set; }
        public string OtherInteraction { get; set; }
        public bool DelayedSave { get; set; }
    }
}
