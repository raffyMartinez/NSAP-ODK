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
    public class CatchLengthFlattened
    {
        public CatchLengthFlattened(CatchLength cl)
        {
            ID = cl.PK;
            ParentID = cl.Parent.PK;
            CatchName = cl.Parent.CatchName;
            Taxa = cl.Parent.Taxa.ToString();
            Gear = cl.Parent.Parent.Parent.Gear.ToString();
            Length = cl.Length;

        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string CatchName { get; set; }
        public string Taxa { get; set; }
        public string Gear { get; set; }
        public double Length { get; set; }
    }

    public class CatchLengthEdited
    {
        public CatchLengthEdited()
        {
            //
        }

        public CatchLengthEdited(CatchLength cl)
        {
            PK = cl.PK;
            SexCode = cl.Sex;
            CatchLength = cl;
            Length = cl.Length;
        }
        [ItemsSource(typeof(SexItemsSource))]
        public string SexCode { get; set; }
        [ReadOnly(true)]
        public int PK { get; set; }

        public CatchLength CatchLength { get; set; }

        public double? Length { get; set; }

    }
    public class CatchLengthCrossTab
    {
        public int RowID { get; set; }
        public int ParentCatchID { get; set; }
        public int V_unload_id { get; set; }
        public VesselUnload VesselUnload { get; set; }
        public double WeightOfCatch { get; set; }
        public string GearCode { get; set; }
        public string GearName { get; set; }
        public double WeightGearCatch { get; set; }
        public double? SampleWeightGearCatch { get; set; }
        public string SpeciesName { get; set; }
        public string Taxa { get; set; }
        public double WeightSpecies { get; set; }
        public double Length { get; set; }
        public string Sex { get; set; }
        public VesselCatch Parent { get; set; }

    }
    public class CatchLength
    {
        private VesselCatch _parent;

        public string Sex { get; set; }
        public int PK { get; set; }

        public bool DelayedSave { get; set; }
        public int VesselCatchID { get; set; }

        public double Length { get; set; }
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
