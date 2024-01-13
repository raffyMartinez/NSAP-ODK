using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CarrierBoatLanding_FishingGround
    {

        public int RowID { get; set; }

        public string FishingGroundCode { get; set; }

        public CarrierLanding Parent { get; set; }
        public FishingGround FishingGround
        {
            get
            {
                return NSAPEntities.FishingGroundViewModel.GetFishingGround(FishingGroundCode);
            }
        }


    }
}
