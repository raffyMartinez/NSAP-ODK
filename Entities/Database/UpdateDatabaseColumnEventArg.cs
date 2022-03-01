using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    class UpdateDatabaseColumnEventArg:EventArgs
    {
        public string Intent { get; set; }
        public int Round { get; set; }
        public int RunningCount { get; set; }
        public int RowsToUpdate { get; set; }
    }
}
