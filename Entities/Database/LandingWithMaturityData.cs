using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingWithMaturityData
    {
        private bool _hasBSCMaturityData;
        private bool _hasUndersizedCrabs;
        private bool _withGPS;
        const string BSC = "Portunus pelagicus";
        private bool _hasBerriedCrabs;
        private List<CatchMaturity> _BSCMaturityList = new List<CatchMaturity>();

        public LandingWithMaturityData(VesselUnload vesselUnload, int undersizedCrabCutoffLen)
        {
            VesselUnload = vesselUnload;

            _BSCMaturityList = vesselUnload.ListVesselCatch.Where(t => t.CatchName == BSC).FirstOrDefault()?.ListCatchMaturity;
            _hasBSCMaturityData = _BSCMaturityList?.Count > 0;
            _withGPS = vesselUnload.GPS != null;

            if(_hasBSCMaturityData)
            {
                _hasBerriedCrabs = _BSCMaturityList.Count(t => t.MaturityCode == "spw") > 0;
                _hasUndersizedCrabs = _BSCMaturityList.Count(t => t.Length <= undersizedCrabCutoffLen) > 0;
            }
        }

        public VesselUnload VesselUnload { get; set; }

        public bool HasBSCMaturityData { get { return _hasBSCMaturityData; } }


        public bool HasGPS { get { return _withGPS; } }

        public bool HasBerriedCrabs { get { return _hasBerriedCrabs; } }
        public bool HasUndersizedCrabs { get { return _hasUndersizedCrabs; } }



    }
}
