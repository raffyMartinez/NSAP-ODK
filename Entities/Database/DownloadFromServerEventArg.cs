using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public enum DownloadFromServerIntent
    {
        ContactingServer,
        DownloadingData,
        GotJSONString,
        ConvertDataToExcel,
        ConvertDataToEntities,
        FinishedDownload,
        FinishedDownloadAndSavedJSONFile,
        StoppedDueToError
    }
    public class DownloadFromServerEventArg:EventArgs
    {
        public DownloadFromServerIntent Intent { get; set; }
        public string JSONString { get; set; }
    }
}
