using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthFreqCrossTab
    {
        public int RowID { get; set; }

        public int V_unload_id { get; set; }
        public VesselUnload VesselUnload { get; set; }
        public double WeightOfCatch { get; set; }
        public string GearName { get; set; }
        public double WeightGearCatch { get; set; }
        public double? SampleWeightGearCatch { get; set; }
        public string SpeciesName { get; set; }
        public string Taxa { get; set; }
        public double WeightSpecies { get; set; }
        public double Length { get; set; }
        public int Frequency { get; set; }
        public string Sex { get; set; }

    }
    public class CatchLenFreqFlattened
    {
        public CatchLenFreqFlattened(CatchLenFreq clf)
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

    public class CatchLenFreqEdited
    {
        public CatchLenFreqEdited()
        {

        }

        public CatchLenFreqEdited(CatchLenFreq clf)
        {
            PK = clf.PK;
            SexCode = clf.Sex;
            LengthClass = clf.LengthClass;
            Frequency = clf.Frequency;
        }

        [ReadOnly(true)]
        public int PK { get; set; }
        public CatchLenFreq CatchLenFreq { get; set; }

        public double? LengthClass { get; set; }
        public int? Frequency { get; set; }
        [ItemsSource(typeof(SexItemsSource))]
        public string SexCode { get; set; }
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
                if (_parent == null)
                {
                    _parent = NSAPEntities.VesselCatchViewModel.getVesselCatch(VesselCatchID);
                }
                return _parent;
            }
        }
    }
}
