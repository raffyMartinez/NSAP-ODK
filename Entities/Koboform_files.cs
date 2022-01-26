using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{

    public class FileMetadata
    {

        public string hash { get; set; }

        public string filename { get; set; }

        public string mimetype { get; set; }
    }

    public class Koboform_file
    {
        public string uid { get; set; }
        public string url { get; set; }
        public string asset { get; set; }
        public string user { get; set; }
        public string user__username { get; set; }
        public string file_type { get; set; }
        public string description { get; set; }
        public DateTime date_created { get; set; }
        public string content { get; set; }

        public FileMetadata metadata { get; set; }
    }

    public class Koboform_files
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public string GetFileUID(string fileName)
        {
            return results.FirstOrDefault(t => t.metadata.filename == fileName).uid;
        }
        public List<Koboform_file> results { get; set; }
    }
}
