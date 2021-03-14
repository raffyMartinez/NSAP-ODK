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
        private LandingSiteSampling _landingSiteSampling;
        private GearUnload _gearUnload;
        private VesselUnload _vesselUnload;
        private VesselCatch _vesselCatch;
        private string _family;
        private string _sn;
        public CrossTabCommon(CatchLenFreq clf)
        {
            _samplingDate = clf.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = clf.Parent.Parent.Parent.Parent;
            _gearUnload = clf.Parent.Parent.Parent;
            _vesselUnload = clf.Parent.Parent;
            _vesselCatch = clf.Parent;
            SetCommonProperties();
        }

        public CrossTabCommon(CatchLength cl)
        {
            _samplingDate = cl.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = cl.Parent.Parent.Parent.Parent;
            _gearUnload = cl.Parent.Parent.Parent;
            _vesselUnload = cl.Parent.Parent;
            _vesselCatch = cl.Parent;
            SetCommonProperties();
        }

        public CrossTabCommon(CatchMaturity cm)
        {
            _samplingDate = cm.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = cm.Parent.Parent.Parent.Parent;
            _gearUnload = cm.Parent.Parent.Parent;
            _vesselUnload = cm.Parent.Parent;
            _vesselCatch = cm.Parent;
            SetCommonProperties();
        }
        public CrossTabCommon(VesselCatch vc)
        {
            _samplingDate = vc.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = vc.Parent.Parent.Parent;
            _gearUnload = vc.Parent.Parent;
            _vesselUnload = vc.Parent;
            _vesselCatch = vc;
            SetCommonProperties();
        }

        public CrossTabCommon(VesselUnload vl)
        {
            _samplingDate = vl.Parent.Parent.SamplingDate;
            _landingSiteSampling = vl.Parent.Parent;
            _gearUnload = vl.Parent;
            _vesselUnload = vl;
            SetCommonProperties();
        }
        private void SetCommonProperties()
        {
            DataID = _vesselUnload.PK;
            Enumerator = _vesselUnload.EnumeratorName;
            FishingGround = _landingSiteSampling.FishingGround.ToString();
            Region = _landingSiteSampling.NSAPRegion.ShortName;
            FMA = _landingSiteSampling.FMA.Name;
            LandingSite = _landingSiteSampling.LandingSiteName;
            if (_landingSiteSampling.LandingSite != null)
            {
                Province = _landingSiteSampling.LandingSite.Municipality.Province.ProvinceName;
                Municipality = _landingSiteSampling.LandingSite.Municipality.MunicipalityName;
            }
            Sector = _vesselUnload.Sector;
            var grid = NSAPEntities.VesselUnloadViewModel.FirstGridLocation(_vesselUnload);
            FishingGroundGrid = grid?.ToString();
            if(grid!=null)
            {
                xCoordinate = grid.GridCell.Coordinate.Longitude;
                yCoordinate = grid.GridCell.Coordinate.Latitude;
            }
            Gear = _gearUnload.GearUsedName;
            FBName = _vesselUnload.VesselName;
            FBL = _gearUnload.Boats;
            FBM = NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t=>t.Parent.PK==_gearUnload.PK);
            SamplingDay = _landingSiteSampling.IsSamplingDay;
            TotalWeight = _vesselUnload.WeightOfCatch;
            MonthSampled = new DateTime(_samplingDate.Year, _samplingDate.Month, 1);
            SamplingDate = _samplingDate;
        }
        public int DataID { get; private set; }
       
        public string Enumerator { get; private set; }
        public DateTime MonthSampled { get; private set; }

        public int Year { get { return MonthSampled.Year; } }

        public string Month { get { return MonthSampled.ToString("MMMM"); } }
        public DateTime SamplingDate { get; private set; }

        public bool SamplingDay { get; set; }

        public string Region { get; private set; }
        public string FMA { get; private set; }

        public string FishingGround { get; private set; }

        public string LandingSite { get; private set; }

        public string Gear { get; private set; }

        //public string LandingSiteName { get; private set; }
        public string Province { get; set; }

        //public string ProvinceName { get { return Province == null ? "" : Province.ProvinceName; } }

        public string Municipality { get; set; }

        //public string MunicipalityName { get { return Municipality== null ? "" : Municipality.MunicipalityName; } }
        public string Sector { get; private set; }

        public string FishingGroundGrid { get; private set; }

        public double? xCoordinate { get; private set; }
        public double? yCoordinate { get; private set; }

        //public string GearName { get; private set; }
        public string FBName { get; private set; }
        public int? FBL { get; set; }
        public int? FBM { get; private set; }



        //public string Catch { get { return _vesselCatch.; } }

         public double? TotalWeight { get; set; }

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

        public double? SpeciesWeight
        {
            get
            {
                return _vesselCatch.Catch_kg;
            }
        }

    }
}


