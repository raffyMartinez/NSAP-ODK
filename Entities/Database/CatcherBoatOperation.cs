using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CatcherBoatOperation
    {
        public int RowID { get; set; }
        public string CatcherBoatName { get; set; }
        public DateTime StartOfOperation { get; set; }
        public double? WeightOfCatch { get; set; }
        public string GearCode { get; set; }

        public CarrierLanding Parent{ get; set; }
        //public CarrierLanding CarrierLanding
        //{
        //    get
        //    {
        //        return NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(LandingSiteSamplingID);
        //    }
        //}

        public Gear Gear
        {
            get
            {
                if (string.IsNullOrEmpty(GearCode))
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(GearCode);
                }
            }
        }
    }
}
