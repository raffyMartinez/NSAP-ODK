using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Media_csv
{
    public class CSVMediaAnalysis
    {
        public string EntityName { get; set; }
        public bool IsSimilarData { get; set; }
        public int CSVRecordCount { get; set; }
        public int DatabaseRecordCount { get; set; }
        //public List<int> CSVItemsNotInDatabase { get; set; }
        //public List<int> DatabaseItemsNotInCSV { get; set; }
        public List<string> CSVItemsNotInDatabase { get; set; }
        public List<string> DatabaseItemsNotInCSV { get; set; }
    }
}
