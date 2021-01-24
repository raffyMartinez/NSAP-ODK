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
        private string _family;
        private string _sn;
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
            FishingGround = _landingSiteSamplinng.FishingGround.ToString();
            //LandingSite = _landingSiteSamplinng.LandingSite;
            //LandingSiteName = _landingSiteSamplinng.LandingSiteName;
            LandingSite = _landingSiteSamplinng.LandingSiteName;
            if (_landingSiteSamplinng.LandingSite != null)
            {
                Province = _landingSiteSamplinng.LandingSite.Municipality.Province.ProvinceName;
                Municipality = _landingSiteSamplinng.LandingSite.Municipality.MunicipalityName;
            }
            Sector = _vesselUnload.Sector;
            FishingGroundGrid = NSAPEntities.VesselUnloadViewModel.FirstGridLocation(_vesselUnload)?.ToString();
            //Gear = _gearUnload.Gear;
            //GearName = _gearUnload.GearUsedName;
            Gear = _gearUnload.GearUsedName;
            FBName = _vesselUnload.VesselName;
            FBL = _gearUnload.Boats;
            FBM = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t=>t.Parent.PK==_gearUnload.PK);
            SamplingDay = _landingSiteSamplinng.IsSamplingDay;
            TotalWeight = _vesselCatch.Catch_kg;
            MonthSampled = new DateTime(_samplingDate.Year, _samplingDate.Month, 1);
            SamplingDate = _samplingDate;
        }
        public int DataID { get; private set; }
        public string FishingGround { get; private set; }
        public DateTime MonthSampled { get; private set; }

        public int Year { get { return MonthSampled.Year; } }

        public string Month { get { return MonthSampled.ToString("MMMM"); } }
        public DateTime SamplingDate { get; private set; }

        public string LandingSite { get; private set; }

        //public string LandingSiteName { get; private set; }
        public string Province { get; set; }

        //public string ProvinceName { get { return Province == null ? "" : Province.ProvinceName; } }

        public string Municipality { get; set; }

        //public string MunicipalityName { get { return Municipality== null ? "" : Municipality.MunicipalityName; } }
        public string Sector { get; private set; }

        public string FishingGroundGrid { get; private set; }
        public string Gear { get; private set; }
        //public string GearName { get; private set; }
        public string FBName { get; private set; }
        public int? FBL { get; set; }
        public int? FBM { get; private set; }

        public bool SamplingDay { get; set; }

        //public string Catch { get { return _vesselCatch.; } }

        public string Family
        {
            get
            {
                if (_vesselCatch.SpeciesID==null)
                {
                    _family= _vesselCatch.Taxa.Name;
                }
                else
                {
                    switch (_vesselCatch.Taxa.Code)
                    {
                        case "FIS":
                            _family= _vesselCatch.FishSpecies.Family;
                            break;
                        default:
                            _family= _vesselCatch.NotFishSpecies.Taxa.Name;
                            break;
                    }
                }
                return _family;
            }
            set
            {
                _family = value;
            }
        }
        public string SN
        {
            get
            {
                if (_vesselCatch.SpeciesID==null)
                {
                    _sn= _vesselCatch.CatchName;
                }
                else
                {
                    switch (_vesselCatch.Taxa.Code)
                    {
                        case "FIS":
                            _sn=$"{_vesselCatch.FishSpecies.GenericName} {_vesselCatch.FishSpecies.SpecificName}";
                            break;
                        default:
                            _sn = $"{_vesselCatch.NotFishSpecies.Genus} {_vesselCatch.NotFishSpecies.Species}";
                            break;
                    }
                }
                return _sn;
            }
            set
            {
                _sn = value;
            }
        }
        public double? TotalWeight { get; set; }
    }
}


