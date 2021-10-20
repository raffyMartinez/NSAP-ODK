using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class TrackedOperationSummary
    {
        private int _samplingID;
        private List<CatchMaturity> _listCrabCatchMaturity = new List<CatchMaturity>();
        private double? _numberOfHoursFishing;
        private int? _numberOfFishers;
        private int? _countMale;
        private int? _countFemale;
        private string _vesselName;
        private double? _weightOfCrabs;
        private int _undersizedCutoffLen;
        const string BSC = "Portunus pelagicus";

        public TrackedOperationSummary(VesselUnload vu, int undersizedCutoffLen, List<TrackedLandingCentroid>landingCentroids =null)
        {
            _undersizedCutoffLen = undersizedCutoffLen;
            _samplingID = vu.PK;
            VesselUnload = vu;
            SetListCrabCatchMaturity();
            _numberOfHoursFishing = vu.ListVesselEffort.Where(t => t.EffortSpecification.Name == "Number of hours fishing").FirstOrDefault()?.EffortValueNumeric;
            _numberOfFishers = (int?)vu.ListVesselEffort.Where(t => t.EffortSpecification.Name == "Number of fishers").FirstOrDefault()?.EffortValueNumeric;
            _countFemale = _listCrabCatchMaturity.Count(t => t.SexCode == "f");
            _countMale = _listCrabCatchMaturity.Count(t => t.SexCode == "m");
            _vesselName = vu.VesselName;
            //_weightOfCrabs = vu.ListVesselCatch.Where(t => t.CatchName == "Portunus pelagicus").FirstOrDefault()?.Catch_kg;
            _weightOfCrabs = (double)vu.ListVesselCatch.Where(t => t.CatchName == BSC).Sum(t => t.Catch_kg);

            if(landingCentroids?.Count>0)
            {
                var items = landingCentroids.Where(t => t.DeviceName!=null &&
                t.DeviceName.ToLower() == vu.GPSText.ToLower() &&
                t.End< vu.SamplingDate)
                .OrderByDescending(t=>t.End).ToList();
                if(items.Count>0)
                {
                    EndHaulToSamplingDate = vu.SamplingDate - items[0].End;
                    X = items[0].X;
                    Y = items[0].Y;
                }
            }
        }
        public string VesselName { get { return _vesselName; } }
        public int? CountMale { get { return _countMale; } }
        public int? CountFemale { get { return _countFemale; } }
        public TrackedOperationSummary(int samplingID)
        {
            _samplingID = samplingID;
            VesselUnload = NSAPEntities.VesselUnloadViewModel.getVesselUnload(_samplingID);
            SetListCrabCatchMaturity();
        }
        public int SamplingID
        {
            get { return _samplingID; }
            set
            {
                _samplingID = value;
                VesselUnload = NSAPEntities.VesselUnloadViewModel.getVesselUnload(_samplingID);
                SetListCrabCatchMaturity();
            }
        }

        public int? CutoffLengfthForUndersize { get { return _undersizedCutoffLen; } }
        public int BerriedCrabCount
        {
            get { return _listCrabCatchMaturity.Count(t => t.MaturityCode == "spw"); }
        }

        public double? BerriedCrabTotalWeight
        {
            get { return _listCrabCatchMaturity.Where(t=>t.MaturityCode=="spw").Sum(t => t.Weight); }
        }
        public int CountMaturityMeasurements
        {
            get { return _listCrabCatchMaturity.Count; }
        }
        private void SetListCrabCatchMaturity()
        {

            _listCrabCatchMaturity.Clear();
            if (VesselUnload.ListVesselCatch.Count > 0)
            {
                var crabCatch = VesselUnload.ListVesselCatch.Where(t => t.CatchName == BSC).ToList();
                if (crabCatch.Count > 0)
                {
                    foreach (VesselCatch c in crabCatch)
                    {
                        foreach (CatchMaturity cm in c.ListCatchMaturity)
                        {
                            _listCrabCatchMaturity.Add(cm);
                        }
                    }
                }
            }
        }
        public TimeSpan EndHaulToSamplingDate { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public DateTime SamplingDate { get { return VesselUnload.SamplingDate; } }
        public VesselUnload VesselUnload { get; private set; }
        public LandingSite LandingSite
        {
            get { return VesselUnload.Parent.Parent.LandingSite; }
        }

        public double? WeightOfCrabs { get { return _weightOfCrabs; } }

        public Gear Gear { get { return VesselUnload.Parent.Gear; } }

        public GPS GPS { get { return VesselUnload.GPS; } }
        public int? NumberOfFishers { get { return _numberOfFishers; } }

        public double? NumberOHoursFishing { get { return _numberOfHoursFishing; } }

        public double TotalWeightOfCatch { get { return (double)VesselUnload.WeightOfCatch; } }

        public double UndersizedCrabPercentByWeight
        {
            get
            {
                double totalWt = 0;
                foreach (CatchMaturity c in _listCrabCatchMaturity.Where(t => t.Length <= _undersizedCutoffLen))
                {
                    totalWt += (double)c.Weight;
                }
                return totalWt / CrabMaturityTotalWeight * 100;
            }
        }

        public int? UndersizedCrabCount
        {
            get { return _listCrabCatchMaturity.Count(t => t.Length <= _undersizedCutoffLen); }
        }

        public double? UndersizedCrabWeight
        {
            get { return _listCrabCatchMaturity.Where(t => t.Length <= _undersizedCutoffLen).Sum(t => t.Weight); }
        }

        public double? UndersizedFemaleCrabWeight
        {
            get { return _listCrabCatchMaturity.Where(t => t.Length <= _undersizedCutoffLen && t.SexCode=="f").Sum(t => t.Weight); }
        }
        public int? UndersizedFemaleCrabCount
        {
            get { return _listCrabCatchMaturity.Count(t => t.Length <= _undersizedCutoffLen && t.SexCode == "f"); }
        }
        public double UndersizedFemaleCrabPercentByWeight
        {
            get
            {

                double totalWt = 0;
                foreach (CatchMaturity c in _listCrabCatchMaturity.Where(t => t.Length <= _undersizedCutoffLen && t.SexCode == "f"))
                {
                    totalWt += (double)c.Weight;
                }
                return totalWt / CrabMaurityFemaleTotalWeight * 100;
            }
        }

        public double CrabMaurityFemaleTotalWeight
        {
            get
            {
                double totalwt = 0;
                foreach (CatchMaturity c in _listCrabCatchMaturity.Where(t => t.SexCode == "f"))
                {
                    totalwt += (double)c.Weight;
                }
                return totalwt;
            }
        }

        public double CrabMaturityTotalWeight
        {
            get
            {
                //double totalwt = 0;
                //foreach (CatchMaturity c in _listCrabCatchMaturity)
                //{
                //    totalwt += (double)c.Weight;
                //}
                //return totalwt;
                return (double)_listCrabCatchMaturity.Sum(t => t.Weight);
            }
        }

        public double BerriedCrabPercentByWeight
        {
            get
            {
                double totalwt = 0;
                foreach (CatchMaturity cm in _listCrabCatchMaturity.Where(t => t.MaturityCode == "spw"))
                {
                    totalwt += (double)cm.Weight;
                }
                return totalwt / CrabMaturityTotalWeight * 100;

            }
        }
    }
}
