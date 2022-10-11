using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CatchLenFreqFlattened
    {
        public  CatchLenFreqFlattened(CatchLenFreq clf)
        {
            ID = clf.PK;
            ParentID = clf.Parent.PK;
            CatchName = clf.Parent.CatchName;
            Taxa = clf.Parent.Taxa.ToString();
            Gear = clf.Parent.Parent.Parent.Gear.ToString();
            Length = clf.LengthClass;
            Frequency = clf.Frequency;
        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string CatchName { get; set; }
        public string Taxa { get; set; }
        public string Gear { get; set; }
        public int Frequency { get; set; }
        public double Length { get; set; }
    }
    public class CatchLenFreq
    {
        private VesselCatch _parent;

        public bool DelayedSave { get; set; }
        public int PK { get; set; }
        public int VesselCatchID { get; set; }

        public double LengthClass { get; set; }
        public int Frequency { get; set; }

        public string Sex { get; set; }
        public VesselCatch Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAPEntities.VesselCatchViewModel.getVesselCatch(VesselCatchID);
                }
                return _parent;
            }
        }
    }
}
