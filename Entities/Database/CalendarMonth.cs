using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.TreeViewModelControl;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarMonth
    {
        LandingSite LandingSite { get; set; }
        public DateTime Month { get; set; }
        public List<CalendarGearSector> GearSectors { get; set; }

        public AllSamplingEntitiesEventHandler SampledMonthEntities { get; set; }
        
    }
}
