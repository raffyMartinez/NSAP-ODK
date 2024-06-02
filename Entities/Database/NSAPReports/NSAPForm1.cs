using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;

namespace NSAP_ODK.Entities.Database.NSAPReports
{
    public class NSAPForm1
    {
        public string LandingSiteText { get; set; }
        public LandingSite LandingSite { get; set; }
        public FishingGround FishingGround { get; set; }
        public FMA FMA { get; set; }
        public NSAPRegion Region { get; set; }
        public DateTime MonthSampled { get; set; }

        public void GetData()
        {
            var dateGearBoatCatches = NSAPEntities.SummaryItemViewModel.GetDateGearBoatsCatches(monthSampled: MonthSampled, fma: FMA, landingSite: LandingSite, fishingGround: FishingGround);
        }


    }
}
