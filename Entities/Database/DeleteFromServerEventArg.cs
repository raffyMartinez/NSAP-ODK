using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class DeleteFromServerEventArg:EventArgs
    {
        public string Intent { get; set; }
        public string TableName { get; set; }
        public int CountToProcess { get; set; }
        public int CountProcessed { get; set; }
    }
}
