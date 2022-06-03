using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public enum UploadToDBIntent
    {
        StartOfUpload,
        Uploading,
        EndOfUpload,
        SearchingUpdates,
        UpdateFound,
        StartOfUpdate,
        Updating,
        EndOfUpdate,
        Cancelled,
        UnloadFound,
        

    }
    public class UploadToDbEventArg:EventArgs
    {
        public string EntitySaved { get; set; }
        public int VesselUnloadSavedCount { get; set; }

        public int VesselUnloadUpdatedCount { get; set; }
        public int VesselUnloadToSaveCount { get; set; }
        public int VesselUnloadToUpdateCount { get; set; }

        public int VesselUnloadTotalSavedCount { get; set; }

        public int LandingSiteBoatLandingsSavedCount { get; set; }
        public int LandingSiteBoatLandingsToSaveCount { get; set; }

        public int LandingSiteBoatLandingsTotalSavedCount { get; set; }

        public UploadToDBIntent Intent { get; set; }

        public int VesselUnloadFoundCount { get; set; }
    }
}
