using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class EnumeratorFormVersion
    {
        public NSAPEnumerator NSAPEnumerator { get; set; }
        public string FormVersion { get; set; }

        public DateTime LastSamplingDate { get; set; }
    }
}
