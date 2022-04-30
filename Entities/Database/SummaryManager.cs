using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
namespace NSAP_ODK.Entities.Database
{

    public static class SummaryManager
    {
        public static TreeViewModelControl.AllSamplingEntitiesEventHandler TreeEntities { get; set; }
        public static NSAPRegion Region { get; set; }
        public static FishingGround FishingGround { get; set; }

        public static LandingSite LandingSite{ get; set; }
        public static bool OnlyWithLandings { get; set; }

        public static SummaryLevelType SummaryLevelType { get; set; }
        public static List<SummaryResults> MakeSummary()
        {
            List<SummaryResults> results = new List<SummaryResults>();
            switch(SummaryLevelType)
            {
                case SummaryLevelType.FishingGround:
                    results= MakeSummaryForFishingGround();
                    break;
            }
            return results;
        }

        private static List<SummaryResults>MakeSummaryForFishingGround()
        {
            return new List<SummaryResults>();
        }
    }
}
