using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class EntityBulkImportEventArg : EventArgs
    {
        public NSAPEntity NSAPEntity { get; set; }
        public int RecordsToImport { get; set; }
        public int ImportedCount { get; set; }
        public string Intent { get; set; }

        public string ImportedEntityName { get; set; }
    }
}
