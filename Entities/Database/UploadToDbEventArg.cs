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
        EndOfUpload

    }
    public class UploadToDbEventArg:EventArgs
    {
        public string EntitySaved { get; set; }
        public int VesselUnloadSavedCount { get; set; }
        public int VesselUnloadToSaveCount { get; set; }

        public int VesselUnloadTotalSavedCount { get; set; }

        public UploadToDBIntent Intent { get; set; }
    }
}
