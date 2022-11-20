using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace NSAP_ODK.Entities.Database
{
    public class DownloadMediaFromServerEventArgs:EventArgs
    {
        public int MediaFileCount { get; set; }
        public int MediaFileDownloadedCount { get; set; }
        public string MediaFileName { get; set; }

        public string Intent { get; set; }
        public bool IsCSV
        {
            get
            {

                return !string.IsNullOrEmpty(MediaFileName) && MediaFileName.ToLower().Contains("csv");
                
            }
        }
    }
}
