using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xceed.Wpf.Toolkit;
namespace NSAP_ODK.Entities.Database
{
    public class GearSoakFlattened
    {
        public GearSoakFlattened(GearSoak gearSoak)
        {
            ID = gearSoak.PK;
            ParentID = gearSoak.Parent.PK;
            TimeAtSet = gearSoak.TimeAtSet;
            TimeAtHaul = gearSoak.TimeAtHaul;
            WaypointAtSet = gearSoak.WaypointAtSet;
            WaypointAtHaul = gearSoak.WaypointAtHaul;
        }
        public int ID { get; set; }
        public int ParentID { get; set; }

        public DateTime TimeAtSet { get; set; }
        public DateTime TimeAtHaul { get; set; }
        public string WaypointAtSet { get; set; }
        public string WaypointAtHaul { get; set; }
    }

    public class GearSoakEdited
    {
        public GearSoakEdited()
        {

        }
        public GearSoakEdited(GearSoak gs)
        {
            if (gs != null)
            {
                PK = gs.PK;
                GearSoak = gs;
                TimeAtSet = gs.TimeAtSet;
                TimeAtHaul = gs.TimeAtHaul;
                WaypointAtSet = gs.WaypointAtSet;
                WaypointAtHaul = gs.WaypointAtHaul;
            }
        }
        [ReadOnly(true)]
        public int PK { get; set; }
        public GearSoak GearSoak { get; set; }
        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime TimeAtSet { get; set; }
        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime TimeAtHaul { get; set; }
        public string WaypointAtSet { get; set; }
        public string WaypointAtHaul { get; set; }

    }
    public class GearSoak
    {
        private VesselUnload _parent;
        public bool DelayedSave { get; set; }
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }

        public DateTime TimeAtSet { get; set; }
        public DateTime TimeAtHaul { get; set; }
        public string WaypointAtSet { get; set; }
        public string WaypointAtHaul { get; set; }

        public VesselUnload Parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.VesselUnloadViewModel.getVesselUnload(VesselUnloadID);
                }
                return _parent;
            }
            set { _parent = value; }
        }
    }
}
