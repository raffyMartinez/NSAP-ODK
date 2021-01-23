using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CrossTabCommon
    {
        private DateTime _samplingDate;
        private LandingSiteSampling _landingSiteSamplinng;
        private GearUnload _gearUnload;
        private VesselUnload _vesselUnload;
        private VesselCatch _vesselCatch;
        public CrossTabCommon(CatchLenFreq clf)
        {
            _samplingDate = clf.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSamplinng = clf.Parent.Parent.Parent.Parent;
            _gearUnload = clf.Parent.Parent.Parent;
            _vesselUnload = clf.Parent.Parent;
            _vesselCatch = clf.Parent;
            SetCommonProperties();
        }

        public CrossTabCommon(CatchLength cl)
        {
            _samplingDate = cl.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSamplinng = cl.Parent.Parent.Parent.Parent;
            _gearUnload = cl.Parent.Parent.Parent;
            _vesselUnload = cl.Parent.Parent;
            _vesselCatch = cl.Parent;
            SetCommonProperties();
        }

        public CrossTabCommon(CatchMaturity cm)
        {
            _samplingDate = cm.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSamplinng = cm.Parent.Parent.Parent.Parent;
            _gearUnload = cm.Parent.Parent.Parent;
            _vesselUnload = cm.Parent.Parent;
            _vesselCatch = cm.Parent;
            SetCommonProperties();
        }
        public CrossTabCommon(VesselCatch vc)
        {
            _samplingDate = vc.Parent.Parent.Parent.SamplingDate;
            _landingSiteSamplinng = vc.Parent.Parent.Parent;
            _gearUnload = vc.Parent.Parent;
            _vesselUnload = vc.Parent;
            _vesselCatch = vc;
            SetCommonProperties();
        }
        private void SetCommonProperties()
        {
            DataID = _vesselUnload.PK;
            FishingGround = _landingSiteSamplinng.FishingGround;
            LandingSite = _landingSiteSamplinng.LandingSite;
            LandingSiteName = _landingSiteSamplinng.LandingSiteName;
            Province = LandingSite.Municipality.Province;
            Municipality = LandingSite.Municipality;
            Sector = _vesselUnload.Sector;
            FishingGroundGrid = NSAPEntities.VesselUnloadViewModel.FirstGridLocation(_vesselUnload);
            Gear = _gearUnload.Gear;
            GearName = _gearUnload.GearUsedName;
            FBName = _vesselUnload.VesselName;
            FBM = _gearUnload.Boats;
            SamplingDay = _landingSiteSamplinng.IsSamplingDay;
            Weight = _vesselCatch.Catch_kg;
            MonthSampled = new DateTime(_samplingDate.Year, _samplingDate.Month, 1);
            SamplingDate = _samplingDate;
        }
        public int DataID { get; private set; }
        public FishingGround FishingGround { get; private set; }
        public DateTime MonthSampled { get; private set; }

        public DateTime SamplingDate { get; private set; }

        public LandingSite LandingSite { get; private set; }

        public string LandingSiteName { get; private set; }
        public Province Province { get; set; }

        public Municipality Municipality { get; set; }
        public string Sector { get; private set; }

        public FishingGroundGrid FishingGroundGrid { get; private set; }
        public Gear Gear { get; private set; }
        public string GearName { get; private set; }
        public string FBName { get; private set; }
        public int FBL { get; set; }
        public int? FBM { get; private set; }

        public bool SamplingDay { get; private set; }

        public VesselCatch Catch { get { return _vesselCatch; } }

        public string Family
        {
            get
            {
                if (Catch.CatchName.Length > 0)
                {
                    return Catch.Taxa.Name;
                }
                else
                {
                    switch (Catch.Taxa.Code)
                    {
                        case "FIS":
                            return Catch.FishSpecies.Family;
                        default:
                            return Catch.NotFishSpecies.Taxa.Name;
                    }
                }
            }
        }
        public string SN
        {
            get
            {
                if (Catch.CatchName.Length > 0)
                {
                    return Catch.CatchName;
                }
                else
                {
                    switch (Catch.Taxa.Code)
                    {
                        case "FIS":
                            return $"{Catch.FishSpecies.GenericName} {Catch.FishSpecies.SpecificName})";
                        default:
                            return $"{Catch.NotFishSpecies.Genus} {Catch.NotFishSpecies.Species}";
                    }
                }
            }
        }
        public double? Weight { get; private set; }
    }
}


