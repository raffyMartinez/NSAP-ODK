using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SummaryItem
    {
        public override string ToString()
        {
            string ls = LandingSiteName == null ? LandingSiteText : LandingSite.ToString();
            string gr = Gear != null ? Gear.GearName : GearText;
            return $"{ID}-{Region.ShortName}-{FMA.Name}-{FishingGround.Name}-{ls}-{gr}-{SamplingDate.ToString("MMM-dd-yyyy")}";
        }

        public DateTime MonthSampled
        {
            get
            {
                return new DateTime(SamplingDate.Year, SamplingDate.Month, 1);
            }
        }
        public string LandingSiteNameText
        {
            get
            {
                if(LandingSiteID==null)
                {
                    return LandingSiteText;
                }
                else
                {
                    return LandingSite.ToString();
                }
            }
        }
        public int ID { get; set; }
        public FMA FMA
        {
            get
            {
                return NSAPEntities.FMAViewModel.GetFMA(FMAId);
            }
        }
        public int? GearUnloadBoats { get; set; }
        public double? GearUnloadCatch { get; set; }
        public Gear Gear
        {
            get
            {
                if (GearCode.Length > 0)
                {
                    return NSAPEntities.GearViewModel.GetGear(GearCode);
                }
                else
                {
                    return null;
                }
            }
        }
        public NSAPRegion Region
        {
            get
            {
                return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(RegionID);
            }
        }
        public int SamplingDayID { get; set; }
        public LandingSite LandingSite
        {
            get
            {
                if (LandingSiteID != null)
                {
                    return NSAPEntities.LandingSiteViewModel.GetLandingSite((int)LandingSiteID);
                }
                else
                {
                    return null;
                }
            }
        }

        public GearUnload GearUnload
        {
            get
            {
                LandingSiteSampling lss = new LandingSiteSampling
                {
                    NSAPRegion = Region,
                    FMA = FMA,
                    LandingSite = LandingSite,
                    LandingSiteText = LandingSiteText,
                    FishingGround = FishingGround,
                    SamplingDate = SamplingDate.Date
                };

                return new GearUnload
                {
                    Parent = lss,
                    GearID = GearCode,
                    Boats = GearUnloadBoats,
                    Catch = GearUnloadCatch,
                    GearUsedText = GearText,
                    Gear = Gear,
                    PK = GearUnloadID
                };
            }
        }
        public int GearUnloadID { get; set; }
        public int VesselUnloadID { get; set; }
        public string RegionID { get; set; }
        public int FMAId { get; set; }
        public string FishingGroundID { get; set; }

        public FishingGround FishingGround
        {
            get
            {
                return NSAPEntities.FishingGroundViewModel.GetFishingGround(FishingGroundID);
            }
        }
        public int? LandingSiteID { get; set; }

        public string LandingSiteName { get; set; }
        public string LandingSiteText { get; set; }
        public string GearCode { get; set; }
        public string GearName { get; set; }

        public string GearUsedName
        {
            get
            {
                if(GearCode.Length==0)
                {
                    return GearText;
                }
                else
                {
                    return GearName;
                }

            }
        }
        public string GearText { get; set; }
        public int? EnumeratorID { get; set; }
        public string EnumeratorName { get; set; }

        public string EnumeratorText { get; set; }
        public DateTime SamplingDate { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsTracked { get; set; }
        public string SectorCode { get; set; }
        public bool HasCatchComposition { get; set; }

        public bool IsTripCompleted { get; set; }

        public int CatchCompositionRows { get; set; }
    }
}
