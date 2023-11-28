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
        SetNumberOfLoops,
        DownloadingData,
        DownloadingSubmissionPairs,
        GotXLSFormVersion,
        GotJSONString,
        ConvertDataToExcel,
        ConvertDataToEntities,
        FinishedDownload,
        FinishedDownloadAndSavedJSONFile,
        FinishedDownloadSubmissionPairs,
        StoppedDueToError,
        SavingToJSONTextFile,
        PrepareItemDeleteFromServer,
        ItemDeletedFromServer,
        ItemDeletedFromServerDone,

    }
    public class DownloadFromServerEventArg:EventArgs
    {
        public DownloadFromServerIntent Intent { get; set; }
        public string JSONString { get; set; }

        public string FormName { get; set; }

        public string FileName { get; set; }

        public int? Loop { get; set; }

        public int? Loops { get; set; }
    }
}
