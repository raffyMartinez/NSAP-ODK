﻿using System;
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
        GotXLSFormVersion,
        GotJSONString,
        ConvertDataToExcel,
        ConvertDataToEntities,
        FinishedDownload,
        FinishedDownloadAndSavedJSONFile,
        StoppedDueToError,
        SavingToJSONTextFile

    }
    public class DownloadFromServerEventArg:EventArgs
    {
        public DownloadFromServerIntent Intent { get; set; }
        public string JSONString { get; set; }

        public string FormName { get; set; }

        public string FileName { get; set; }
    }
}
