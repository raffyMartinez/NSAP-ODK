using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace NSAP_ODK.Entities.Database
{
    public class CarrierLanding
    {
        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime SamplingDate { get; set; }
        public string CarrierName { get; set; }
        public CatcherBoatOperation_ViewModel CatcherBoatOperation_ViewModel { get; set; }

        public VesselCatchViewModel VesselCatchViewModel { get; set; }

        public CarrierBoatLanding_FishingGround_ViewModel CarrierBoatLanding_FishingGround_ViewModel { get; set; }


        public string FishingGroundNames
        {
            get
            {
                if (NumberOfFishingGrounds > 0)
                {
                    return CarrierBoatLanding_FishingGround_ViewModel.FishingGroundsAsString;
                }
                else
                {
                    return "";
                }

            }
        }

        public int NumberOfFishingGrounds
        {
            get
            {
                if (CarrierBoatLanding_FishingGround_ViewModel == null)
                {
                    CarrierBoatLanding_FishingGround_ViewModel = new CarrierBoatLanding_FishingGround_ViewModel(this);
                }
                return CarrierBoatLanding_FishingGround_ViewModel.Count;
            }
        }
        public string CatcherBoatNames
        {
            get
            {
                if (CountCatchers == null || CountCatchers == 0)
                {
                    return "";
                }
                else
                {
                    if (CatcherBoatOperation_ViewModel == null)
                    {
                        CatcherBoatOperation_ViewModel = new CatcherBoatOperation_ViewModel(this);
                    }
                    CatcherBoatOperation_ViewModel = new CatcherBoatOperation_ViewModel(this);
                }
                return CatcherBoatOperation_ViewModel.CatcherNamesAsString;
            }
        }

        public double? WeightOfCatch { get; set; }

        public int CountSpeciesComposition { get; set; }

        public int RowID { get; set; }

        public LandingSiteSampling Parent { get; set; }

        public int? CountCatchers { get; set; }

        public string RefNo { get; set; }
    }
}
