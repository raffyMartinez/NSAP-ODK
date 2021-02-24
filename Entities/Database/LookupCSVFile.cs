using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LookupCSVFile
    {
        public string FileName { get; set; }
        public string Desccription { get; set; }

        public bool IsSelectList { get; set; }
    }
}
