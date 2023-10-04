using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    internal class UpdateTableRowsEventArg:EventArgs
    {
        public string Intent { get; set; }
        public int TotalRowsToUpdate { get; set; }
        public string TableName { get; set; }
        public int RowsUpdatedCount { get; set; }
    }
}
