using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CreateTablesInAccessEventArgs : EventArgs
    {
        public int TotalTableCount { get; set; }

        public int CurrentTableCount { get; set; }
        public string CurrentTableName { get; set; }

        public int CurrentRowCount { get; set; }
        public string Intent { get; set; }


    }
}
