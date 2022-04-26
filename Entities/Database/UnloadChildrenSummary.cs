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

        public int? NumberOfFishers { get { return _vesselUnload.NumberOfFishers; } }
        public int PK { get { return _vesselUnload.PK; } }
        public int CountGridLocations { get { return _vesselUnload.CountGrids; } }
        public int CountEffortIndicators { get { return _vesselUnload.CountEffortIndicators; } }
        public int CountSoakTimes { get { return _vesselUnload.CountGearSoak; } }

        public int CountCatchComposition { get { return _vesselUnload.CountCatchCompositionItems; } }


        public int CountCatchMaturities
        {
            get
            {
                return _vesselUnload.CountMaturityRows;
            }
        }

        public int CountCatchLengthFreqs
        {
            get
            {
                return _vesselUnload.CountLenFreqRows;
            }
        }
        public int CountCatchLengthWeights
        {
            get
            {
                return _vesselUnload.CountLenWtRows;
            }
        }
        public int CountCatchLengths
        {
            get
            {
                return _vesselUnload.CountLengthRows;
            }
        }
    }
}
