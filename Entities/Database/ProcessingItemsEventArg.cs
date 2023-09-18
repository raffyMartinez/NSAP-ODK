using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class ProcessingItemsEventArg : EventArgs
    {
        
        public bool DoNotShowRunningTotal { get; set; }
        public string Intent { get; set; }
        public int TotalCountToProcess { get; set; }
        public int CountProcessed { get; set; }

        public string ProcessedItemName { get; set; }
    }
}
