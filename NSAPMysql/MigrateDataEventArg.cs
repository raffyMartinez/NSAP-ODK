using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.NSAPMysql
{
    class MigrateDataEventArg:EventArgs
    {
        public string Description { get; set; }
        public string TableName { get; set; }
        public int OriginalRowCount { get; set; }

        public int RowsCopies { get; set; }

        public bool RowCopied { get; set; }
        public string Intent { get; set; }
    }
}
