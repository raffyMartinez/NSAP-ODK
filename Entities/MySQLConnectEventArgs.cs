using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class MySQLConnectEventArgs:EventArgs
    {
        public bool RequireRoot { get; set; }
        public bool TestIfDBExist { get; set; }
    }
}
