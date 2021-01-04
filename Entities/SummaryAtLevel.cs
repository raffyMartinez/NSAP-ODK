using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities
{
    public class SummaryAtLevel
    {
        public SummaryAtLevel(string name, Database.SummaryLevelType summaryLevel)
        {
            LevelName = name;
            SummaryLevelType = summaryLevel;
        }
        public string LevelName { get; set; }
        public SummaryLevelType SummaryLevelType { get; set; }

        public DBSummary Summary { get; set; }
    }
}
