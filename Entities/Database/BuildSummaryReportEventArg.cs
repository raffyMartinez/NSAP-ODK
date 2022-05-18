using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public enum BuildSummaryReportStatus
    {
        StatusBuildStart,
        StatusBuildFetchedRow,
        StatusBuildEnd
    }
    public class BuildSummaryReportEventArg:EventArgs
    {
        public int TotalRowCount { get; set; }
        public int CurrentRow { get; set; }
        public string Intent { get; set; }

        public BuildSummaryReportStatus BuildSummaryReportStatus { get; set; }

        public bool IsIndeterminate { get; set; }
        public int RowsFetchedAtEnd { get; set; }
    }
}
