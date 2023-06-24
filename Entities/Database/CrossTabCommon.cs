using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{

    public class CrossTabCommonProperties
    {
        private VesselUnload _vesselUnload;
        private DateTime _samplingDate;
        private LandingSiteSampling _landingSiteSampling;
        private GearUnload _gearUnload;
        private VesselUnload_FishingGear _vesselUnload_FishingGear;


        public VesselUnload_FishingGear VesselUnload_FishingGear
        {
            get { return _vesselUnload_FishingGear; }
            set
            {
                _vesselUnload_FishingGear = value;
                VesselUnload = _vesselUnload_FishingGear.Parent;
            }
        }

        public VesselUnload VesselUnload
        {
            get { return _vesselUnload; }
            set
            {
                _vesselUnload = value;
                //VesselUnloadViewModel.SetUpFishingGearSubModel(_vesselUnload);
                _samplingDate = _vesselUnload.SamplingDate;
                _landingSiteSampling = _vesselUnload.Parent.Parent;
                _gearUnload = _vesselUnload.Parent;
                if (_landingSiteSampling.LandingSite != null)
                {
                    Province = _landingSiteSampling.LandingSite.Municipality.Province.ProvinceName;
                    Municipality = _landingSiteSampling.LandingSite.Municipality.MunicipalityName;
                }
                else
                {
                    Province = "";
                    Municipality = "";
                }
                if (_vesselUnload.FishingGroundGridViewModel == null)
                {
                    _vesselUnload.FishingGroundGridViewModel = new FishingGroundGridViewModel(_vesselUnload);
                }
                //var grid = NSAPEntities.VesselUnloadViewModel.FirstGridLocation(_vesselUnload);
                var grid = _vesselUnload.FishingGroundGridViewModel.FishingGroundGridCollection?.FirstOrDefault();
                if (grid != null)
                {
                    FishingGroundGrid = grid.ToString();
                    xCoordinate = grid.GridCell.Coordinate.Longitude;
                    yCoordinate = grid.GridCell.Coordinate.Latitude;
                }

            }
        }


        public int DataID { get { return _vesselUnload.PK; } }

        public string RefNo { get { return _vesselUnload.RefNo; } }
        public string Enumerator { get { return _vesselUnload.EnumeratorName; } }
        public DateTime MonthSampled { get { return new DateTime(_samplingDate.Year, _samplingDate.Month, 1); } }

        public int Year { get { return _samplingDate.Year; } }
        public int? NumberOfFishers { get { return _vesselUnload.NumberOfFishers; } }
        public string Month { get { return _samplingDate.ToString("MMMM"); } }
        public DateTime SamplingDate { get { return _samplingDate; } }
        public string Notes { get { return _vesselUnload.Notes; } }
        public bool SamplingDay { get { return _landingSiteSampling.IsSamplingDay; } set { SamplingDay = value; } }

        public string Region { get { return _landingSiteSampling.NSAPRegion.ShortName; } }
        public string FMA { get { return _landingSiteSampling.FMA.Name; } }

        public string FishingGround { get { return _landingSiteSampling.FishingGround.Name; } }

        public string LandingSite { get { return _landingSiteSampling.LandingSiteName; } }

        //public string Gear { get { return _gearUnload.GearUsedName; } }

        public string Gear { get { return _vesselUnload_FishingGear.GearUsedName; } }
        //public string Gears { get { return _vesselUnload.Gears; } }

        public string Province { get; private set; }


        public string Municipality { get; private set; }

        public string Sector { get { return _vesselUnload.Sector; } }

        public string FishingGroundGrid { get; private set; }

        public double? xCoordinate { get; private set; }
        public double? yCoordinate { get; private set; }
        public string FBName { get { return _vesselUnload.VesselName; } }
        public int? FBL { get { return _gearUnload.Boats; } }
        public int? FBM { get { return _gearUnload.ListVesselUnload.Count; } }

        public double? TotalWeight { get { return _vesselUnload.WeightOfCatch; } }

        public bool IsCatchSold { get { return _vesselUnload.IsCatchSold; } }

        public bool IsBoatUsed { get { return _vesselUnload.IsBoatUsed; } }

    }
    public class CrossTabCommon
    {
        private DateTime _samplingDate;
        private LandingSiteSampling _landingSiteSampling;
        private GearUnload _gearUnload;
        private VesselUnload _vesselUnload;
        private VesselCatch _vesselCatch;
        private VesselUnload_FishingGear _vesselUnload_FishingGear;
        private string _family;
        private string _sn;

        public override string ToString()
        {
            return $"{CommonProperties.VesselUnload.PK} {CommonProperties.VesselUnload.VesselName} {CommonProperties.VesselUnload.SamplingDate.ToString("MMM-dd-yyyy")}";
        }
        public CrossTabCommon(CatchLenFreq clf, VesselUnload_FishingGear vufg)
        {
            _samplingDate = clf.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = clf.Parent.Parent.Parent.Parent;
            _gearUnload = clf.Parent.Parent.Parent;
            _vesselUnload = clf.Parent.Parent;
            _vesselCatch = clf.Parent;
            _vesselUnload_FishingGear = vufg;
            //CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload.PK];
            CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[vufg.Guid.ToString()];
        }

        public CrossTabCommon(CatchLengthWeight clw, VesselUnload_FishingGear vufg)
        {
            _samplingDate = clw.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = clw.Parent.Parent.Parent.Parent;
            _gearUnload = clw.Parent.Parent.Parent;
            _vesselUnload = clw.Parent.Parent;
            _vesselUnload_FishingGear = vufg;
            _vesselCatch = clw.Parent;
            //CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload.PK];
            CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[vufg.Guid.ToString()];
        }
        public CrossTabCommon(CatchLength cl, VesselUnload_FishingGear vufg)
        {
            _samplingDate = cl.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = cl.Parent.Parent.Parent.Parent;
            _gearUnload = cl.Parent.Parent.Parent;
            _vesselUnload = cl.Parent.Parent;
            _vesselUnload_FishingGear = vufg;
            _vesselCatch = cl.Parent;
            //CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload.PK];
            CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[vufg.Guid.ToString()];
        }

        public CrossTabCommon(CatchMaturity cm, VesselUnload_FishingGear vufg)
        {
            _samplingDate = cm.Parent.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = cm.Parent.Parent.Parent.Parent;
            _gearUnload = cm.Parent.Parent.Parent;
            _vesselUnload = cm.Parent.Parent;
            _vesselCatch = cm.Parent;
            _vesselUnload_FishingGear = vufg;
            //CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload.PK];
            CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[vufg.Guid.ToString()];
        }
        public CrossTabCommon(VesselCatch vc)
        {
            _samplingDate = vc.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = vc.Parent.Parent.Parent;
            _gearUnload = vc.Parent.Parent;
            _vesselUnload = vc.Parent;
            _vesselCatch = vc;
            //CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload.PK];
            CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload_FishingGear.Guid.ToString()];
        }
        public CrossTabCommon(VesselCatch vc, VesselUnload_FishingGear vufg)
        {
            _samplingDate = vc.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = vc.Parent.Parent.Parent;
            _gearUnload = vc.Parent.Parent;
            _vesselUnload = vc.Parent;
            _vesselCatch = vc;
            _vesselUnload_FishingGear = vufg;
            //CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[_vesselUnload.PK];
            CommonProperties = CrossTabManager.UnloadCrossTabCommonPropertyDictionary[vufg.Guid.ToString()];
        }
        public CrossTabCommon(VesselUnload_FishingGear vufg)
        {
            _samplingDate = vufg.Parent.Parent.Parent.SamplingDate;
            _landingSiteSampling = vufg.Parent.Parent.Parent;
            _gearUnload = vufg.Parent.Parent;
            _vesselUnload = vufg.Parent;
            _vesselUnload_FishingGear = vufg;
            CrossTabCommonProperties ccp = new CrossTabCommonProperties { VesselUnload_FishingGear = vufg };
            CommonProperties = ccp;
        }
        public CrossTabCommon(VesselUnload vl)
        {
            _samplingDate = vl.Parent.Parent.SamplingDate;
            _landingSiteSampling = vl.Parent.Parent;
            _gearUnload = vl.Parent;
            _vesselUnload = vl;
            CrossTabCommonProperties ccp = new CrossTabCommonProperties { VesselUnload = vl };
            CommonProperties = ccp;
        }

        public CrossTabCommonProperties CommonProperties { get; private set; }

        public string Family
        {
            get
            {
                if(_vesselCatch==null)
                {
                    _family = "UNKNOWN";
                }
                else if (_vesselCatch.SpeciesID == null)
                {
                    _family = _vesselCatch.Taxa.Name;
                }
                else
                {
                    switch (_vesselCatch.Taxa.Code)
                    {
                        case "FIS":
                            _family = _vesselCatch.FishSpecies?.Family;
                            //_family = _vesselCatch.FishSpecies.Family;
                            break;
                        default:
                            _family = _vesselCatch.Taxa.Name;
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
                if(_vesselCatch==null)
                {
                    _sn = "UNKNOWN";
                }
                else if (_vesselCatch.SpeciesID == null)
                {
                    _sn = _vesselCatch.CatchName;
                }
                else
                {
                    if (_vesselCatch.FishSpecies != null || _vesselCatch.NotFishSpecies != null)
                    {
                        switch (_vesselCatch.Taxa.Code)
                        {
                            case "FIS":
                                _sn = $"{_vesselCatch.FishSpecies?.GenericName} {_vesselCatch.FishSpecies?.SpecificName}";
                                break;
                            default:
                                _sn = $"{_vesselCatch.NotFishSpecies.Genus} {_vesselCatch.NotFishSpecies.Species}";
                                break;
                        }
                    }
                    else
                    {
                        _sn = "UNKNOWN SPECIES";
                    }
                }
                return _sn;
            }
            set
            {
                _sn = value;
            }
        }
        public double? TWS
        {
            get
            {
                return _vesselCatch.Sample_kg;
            }
        }
        public double? SpeciesWeight
        {
            get
            {
                return _vesselCatch.Catch_kg;
            }
        }
        public double? Price
        {
            get { return _vesselCatch.PriceOfSpecies; }
        }

        public string Unit
        {
            get { return _vesselCatch.PriceUnit; }
        }
        public string WeightUnit
        {
            get
            {
                return _vesselCatch.WeighingUnit;
            }
        }

    }
}


