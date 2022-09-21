using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class ServerUploadsByMonth
    {
        public Koboserver Koboserver { get; set; }
        public DateTime MonthOfSubmission { get; set; }
        public int CountUploads { get; set; }

        public int CountEnumerators { get; set; }

        public override string ToString()
        {
            return $"{Koboserver}-{MonthOfSubmission.ToString("MMM-yyyy")}-{CountUploads}-{CountEnumerators}";
        }
    }
}
