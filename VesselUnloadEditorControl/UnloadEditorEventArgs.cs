using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.VesselUnloadEditorControl
{
    public class UnloadEditorEventArgs : EventArgs
    {
        public string TreeItem { get; set; }
        public string ButtonPressed { get; set; }
        public bool Proceed { get; set; }

        public VesselUnload Vessel { get; set; }
        public GearSoak GearSoak { get; set; }
        public FishingGroundGrid FishingGroundGrid { get; set; }

        public VesselEffort VesselEffort { get; set; }

        public VesselCatch VesselCacth { get; set; }
    }
}
