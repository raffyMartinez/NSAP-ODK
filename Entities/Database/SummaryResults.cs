using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SummaryResults
    {

        public SummaryLevelType SummaryLevelType { get; set; }
        public int Sequence { get; set; }
        public DBSummary DBSummary { get; set; }
    }
}
