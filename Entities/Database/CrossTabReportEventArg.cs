using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CrossTabReportEventArg:EventArgs
    {
        public int RowsToPrepare { get; set; }

        public int RowsPrepared { get; set; }

        public bool IsDone { get; set; }

        public string Context { get; set; }
    }
}
