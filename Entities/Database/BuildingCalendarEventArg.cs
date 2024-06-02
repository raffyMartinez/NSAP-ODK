using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class BuildingCalendarEventArg:EventArgs
    {
        public int ItemsToBuild { get; set; }
        public int ItemsBuilt { get; set; }
    }
}
