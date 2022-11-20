using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class Analysis
    {
        public bool IsSimilarData { get; set; }
        public int CSVRecordCount { get; set; }
        public int DatabaseRecordCount { get; set; }
        public List<string> CSVItemsNotInDatabase { get; set; }
        public List<string> DatabaseItemsNotInCSV { get; set; }
    }
}
