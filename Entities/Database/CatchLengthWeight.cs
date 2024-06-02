using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthWeightFlattened
    {
        public CatchLengthWeightFlattened(CatchLengthWeight clw)
        {
            ID = clw.PK;
            ParentID = clw.Parent.PK;
            CatchName = clw.Parent.CatchName;
            Taxa = clw.Parent.Taxa.ToString();
            Gear = clw.Parent.Parent.Parent.GearUsedName;
            Length = clw.Length;
            Weight = clw.Weight;
        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string CatchName { get; set; }
        public string Taxa { get; set; }
        public string Gear { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
    }

    public class CatchLengthWeightEdited
    {
        public CatchLengthWeightEdited(CatchLengthWeight clw)
        {
            SexCode = clw.Sex;
            Length = clw.Length;
            Weight = clw.Weight;
            CatchLengthWeight = clw;
            PK = clw.PK;
        }
        public CatchLengthWeightEdited()
        {
            
        }

        [ItemsSource(typeof(SexItemsSource))]
        public string SexCode { get; set; }
        [ReadOnly(true)]
        public int PK { get; set; }

        public CatchLengthWeight CatchLengthWeight { get; set; }

        public double? Length { get; set; }
        public double? Weight { get; set; }
    }
    public class CatchLengthWeight
    {
        private VesselCatch _parent;

        public string Sex { get; set; }
        public int PK { get; set; }

        public bool DelayedSave { get; set; }
        public int VesselCatchID { get; set; }

        public double Length { get; set; }
        public double Weight { get; set; }

        public VesselCatch Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.VesselCatchViewModel.getVesselCatch(VesselCatchID);
                }
                return _parent;
            }
        }
    }
}
