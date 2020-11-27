using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
namespace NSAP_ODK.TreeViewModelControl
{
    public class AllSamplingEntitiesEventHandler : EventArgs
    {
        public NSAPRegion NSAPRegion { get; set; }

        public LandingSite LandingSite { get; set; }

        public string LandingSiteText { get; set; }

        public Gear Gear { get; set; }
        public string GearUsed { get; set; }

        public FMA FMA { get; set; }

        public DateTime? MonthSampled { get; set; }
        public FishingGround FishingGround { get; set; }

        public string TreeViewEntity { get; set; }

        public string ContextMenuTopic { get; set; }
    }
}
