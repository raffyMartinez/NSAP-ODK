using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class UnloadChildrenSummary
    {
        private VesselUnload _vesselUnload;
        private string _noCounts = "-";
        public UnloadChildrenSummary(VesselUnload unload)
        {
            _vesselUnload = unload;
        }

        public VesselUnload VesselUnload { get { return _vesselUnload; } }
        public string DateSampling { get { return _vesselUnload.SamplingDate.ToString("MMM-dd-yyyy HH:mm"); } }

        public string Enumerator { get { return _vesselUnload.EnumeratorName; } }
        public string LandingSite { get { return _vesselUnload.Parent.Parent.LandingSiteName; } }

        public string FishingGround { get { return _vesselUnload.Parent.Parent.FishingGround.Name; } }
        public string FMA { get { return _vesselUnload.Parent.Parent.FMA.Name; } }
        public string Region { get { return _vesselUnload.Parent.Parent.NSAPRegion.ShortName; } }

        public string Gear { get { return _vesselUnload.Parent.GearUsedName; } }
        public int PK { get { return _vesselUnload.PK; } }
        public int CountGridLocations { get { return _vesselUnload.ListFishingGroundGrid.Count; } }
        public int CountEffortIndicators { get { return _vesselUnload.ListVesselEffort.Count; } }
        public int CountSoakTimes { get { return _vesselUnload.ListGearSoak.Count; } }

        public int CountCatchComposition { get { return _vesselUnload.ListVesselCatch.Count; } }


        public string CountCatchMaturities
        {
            get
            {
                int speciesWithMeasurements = 0;
                if (CountCatchComposition > 0)
                {
                    int count = 0;
                    foreach (var item in _vesselUnload.ListVesselCatch)
                    {
                        if (item.ListCatchMaturity.Count > 0)
                        {
                            count += item.ListCatchMaturity.Count;
                            speciesWithMeasurements++;
                        }

                    }
                    if (speciesWithMeasurements > 0)
                    {
                        return $"{count}/{speciesWithMeasurements}";
                    }
                    else
                    {
                        return _noCounts;
                    }
                }
                else
                {
                    return _noCounts;
                }
            }
        }

        public string CountCatchLengthFreqs
        {
            get
            {
                int speciesWithMeasurements = 0;
                if (CountCatchComposition > 0)
                {
                    int count = 0;
                    foreach (var item in _vesselUnload.ListVesselCatch)
                    {
                        if (item.ListCatchLenFreq.Count > 0)
                        {
                            count += item.ListCatchLenFreq.Count;
                            speciesWithMeasurements++;
                        }
                    }
                    if (speciesWithMeasurements > 0)
                    {
                        return $"{count}/{speciesWithMeasurements}";
                    }
                    else
                    {
                        return _noCounts;
                    }
                }
                else
                {
                    return _noCounts;
                }
            }
        }
        public string CountCatchLengthWeights
        {
            get
            {
                int speciesWithMeasurements = 0;
                if (CountCatchComposition > 0)
                {
                    int count = 0;
                    foreach (var item in _vesselUnload.ListVesselCatch)
                    {
                        if (item.ListCatchLengthWeight.Count > 0)
                        {
                            count += item.ListCatchLengthWeight.Count;
                            speciesWithMeasurements++;
                        }
                    }
                    if (speciesWithMeasurements > 0)
                    {
                        return $"{count}/{speciesWithMeasurements}";
                    }
                    else
                    {
                        return _noCounts;
                    }
                }
                else
                {
                    return _noCounts;
                }
            }
        }
        public string CountCatchLengths
        {
            get
            {
                int speciesWithMeasurements = 0;
                if (CountCatchComposition > 0)
                {
                    int count = 0;
                    foreach (var item in _vesselUnload.ListVesselCatch)
                    {
                        if (item.ListCatchLength.Count > 0)
                        {
                            count += item.ListCatchLength.Count;
                            speciesWithMeasurements++;
                        }
                    }
                    if (speciesWithMeasurements > 0)
                    {
                        return $"{count}/{speciesWithMeasurements}";
                    }
                    else
                    {
                        return _noCounts;
                    }
                }
                else
                {
                    return _noCounts;
                }
            }
        }
    }
}
