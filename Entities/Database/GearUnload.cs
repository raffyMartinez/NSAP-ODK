using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class GearUnloadFlattened
    {
        private LandingSiteSamplingFlattened _parent;
        public GearUnloadFlattened(GearUnload gu)
        {
            _parent = NSAPEntities.LandingSiteSamplingViewModel.GetFlattenedItem(gu.Parent.PK);
            Boats = gu.Boats;
            Catch = gu.Catch;
            NSAPRegion = _parent.NSAPRegion;
            FMA = _parent.FMA;
            FishingGround = _parent.FishingGround;
            LandingSite = _parent.LandingSite;
            SamplingDate = _parent.SamplingDate;
            Gear = gu.GearUsedName;
            ID = gu.PK;
            ParentID = _parent.ID;
        }
        public int ID { get; private set; }

        public int ParentID { get; private set; }
        public string NSAPRegion { get; private set; }
        public string FMA { get; private set; }
        public string FishingGround { get; private set; }

        public string LandingSite { get; private set; }
        public DateTime SamplingDate { get; private set; }

        public string Gear { get; private set; }
        public int? Boats { get; private set; }
        public double? Catch { get; private set; }

    }
    public class GearUnload
    {
        private Gear _gear;
        private LandingSiteSampling _parent;

        public string Remarks { get; set; }
        public int PK{ get; set; }
        public int LandingSiteSamplingID { get; set; }
        public string GearID { get; set; }
        public int? Boats { get; set; }
        public double? Catch { get; set; }

        public List<VesselUnload> ListVesselUnload
        {
            get
            {
                return NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                    .Where(t => t.Parent.PK == PK).ToList();
            }
        }

        public string GearUsedText { get; set; }
        public Gear Gear
        {
            set { _gear = value; }
            get
            {
                if(_gear==null)
                {
                    _gear = NSAPEntities.GearViewModel.GetGear(GearID);
                }
                return _gear;
            }
        }


        public string GearUsedName
        {
            get
            {
                if(string.IsNullOrEmpty(GearID))
                {
                    return GearUsedText;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(GearID).GearName;
                }
            }
        }
        public LandingSiteSampling Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAPEntities.LandingSiteSamplingViewModel.getLandingSiteSampling(LandingSiteSamplingID);
                }
                return _parent;
            }
        }

        public override string ToString()
        {
            if (Parent.LandingSite == null)
            {
                return "";
            }
            else
            {
                return $"{Parent.LandingSite.LandingSiteName} {GearUsedName} {Parent.SamplingDate.ToString("MMM-dd-yyyy")} - ({ListVesselUnload.Count})";
            }
        }

    }
}
