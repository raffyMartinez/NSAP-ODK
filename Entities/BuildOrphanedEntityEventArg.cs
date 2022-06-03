using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public enum BuildOrphanedEntityStatus
    {
        StatusBuildStart,
        StatusBuildFirstRecordFound,
        StatusBuildFetchedRow,
        StatusBuildEnd
    }
    public class BuildOrphanedEntityEventArg : EventArgs
    {
        public string Intent { get; set; }

        public int TotalCountFetched { get; set; }
        public int CurrentCount { get; set; }
        public int TotalCount { get; set; }

        public BuildOrphanedEntityStatus BuildOrphanedEntityStatus { get; set; }
        public bool IsIndeterminate { get; set; }
    }
}
