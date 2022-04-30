using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class EntityLoadedEventArg : EventArgs
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public int EntityCount { get; set; }

        public bool IsStarting { get; set; }
        public bool IsEnding { get; set; }
    }
}
