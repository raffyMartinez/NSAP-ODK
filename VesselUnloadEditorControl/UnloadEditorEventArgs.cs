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
        public VesselUnload_Gear_Spec_Edited VesselUnload_Gear_Spec_Edited { get; set; }
        public VesselCatchEdited VesselCatchEdited { get; set; }
        public string TreeItem { get; set; }
        public string ButtonPressed { get; set; }
        public bool Proceed { get; set; }

        public string UnloadView { get; set; }

        public CatchLengthWeightEdited CatchLengthWeightEdited { get; set; }
        public CatchLenFreqEdited CatchLenFreqEdited { get; set; }
        public CatchLengthEdited CatchLengthEdited { get; set; }
        public CatchMaturityEdited CatchMaturityEdited { get; set; }
        public VesselUnload VesselUnload { get; set; }
        public GearSoakEdited GearSoakEdited { get; set; }

        public FishingGroundGridEdited FishingGroundGridEdited { get; set; }

        public VesselEffort VesselEffort { get; set; }

        public VesselUnload_FishingGear_Edited VesselUnload_FishingGear_Edited { get; set; }

        public VesselCatch VesselCatch { get; set; }
        public object DataGridSelectedItem { get; set; }
    }
}
