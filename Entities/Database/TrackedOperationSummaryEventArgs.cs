using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class TrackedOperationSummaryEventArgs
    {
        public int CountOfSummaryRead { get; set; }
        public int TotalCountSummary { get; set; }

        public string Intent { get; set; }
    }
}
