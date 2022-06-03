using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class FileInfoJSONMetadata
    {
        public FileInfo JSONFile { get; set; }

        public DownloadedJsonMetadata DownloadedJsonMetadata { get; set; }

        public int ItemNumber { get; set; }
        public Koboserver Koboserver { get; set; }

    }
}
