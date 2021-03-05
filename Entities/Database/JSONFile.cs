using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public  class JSONFile
    {
        private  string _jsonText;

        public  string JSONText { get { return _jsonText; } 
            set
            {
                _jsonText = value;
                MD5 = Utilities.MD5.CreateMD5(_jsonText);
            } 
        }

        public string FileName { get; set; }

        public  int Count { get; set; }

        public  string FormID { get; set; }

        public  DateTime Earliest { get; set; }

        public  DateTime Latest { get; set; }

        public  string MD5 { get; set; }

        public  string Description { get; set; }

        public int RowID { get; set; }

        public DateTime DateAdded { get; set; }

    }
}
