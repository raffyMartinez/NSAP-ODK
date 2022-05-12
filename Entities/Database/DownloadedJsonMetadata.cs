using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class DownloadedJsonMetadata
    {
        public int? BatchSize { get; set; }
        public int DownloadSize { get; set; }
        public int NumberOfFiles { get; set; }
        public string DBOwner { get; set; }
        public string FormName { get; set; }
        public DateTime DateDownloaded { get; set; }
        public string FileName { get; set; }

        public string DownloadType { get; set; }
    }
}
