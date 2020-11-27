using Npoi.Mapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class ExcelMainSheet
    {
        private NSAPRegionFMA _nsapRegionFMA;
        private NSAPRegion _nsapRegion;
        private NSAPRegionEnumerator _nsapRegionEnumerator;
        private NSAPRegionFMAFishingGround _nsapRegionFMAFishingGround;
        private NSAPRegionFMAFishingGroundLandingSite _nsapRegionFMAFishingGroundLandingSite;
        private NSAPRegionGear _nsapRegionGear;
        private FishingVessel _fishingVessel;
        private GPS _gps;
        private NSAPEnumerator _nsapEnumerator;
        private int? _nsapEnumeratorID;

        public bool IsSaved
        {
            get
            {
                return (NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.FirstOrDefault(t => t.ODKRowID == RowUUID) != null);
            }
            set
            {
                //
            }
        }

        [Column("_uuid")]
        public string RowUUID { get; set; }
        public string XFormIdentifier { get; set; }

        public DateTime XFormDate { get; set; }

        [Column("start")]
        public DateTime Start { get; set; }

        [Column("today")]
        public DateTime Today { get; set; }

        [Column("device_id")]
        public string DeviceId { get; set; }


        [Column("user_name")]
        public string UserName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("intronote")]
        public string FormVersion { get; set; }

        [Column("vessel_sampling/sampling_date")]
        public DateTime SamplingDate { get; set; }

        [Column("vessel_sampling/nsap_region")]
        public string NSAPRegionID { get; set; }

        public NSAPRegion NSAPRegion
        {
            set { NSAPRegion = value; }
            get
            {
                if (_nsapRegion == null)
                {
                    _nsapRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAPRegionID);
                }
                return _nsapRegion;
            }
        }

        [Column("vessel_sampling/region_enumerator")]
        public int? NSAPERegionEnumeratorID { get; set; }

        public int? EnumeratorID 
        {
            set { EnumeratorID = value; }
            get
            {
                if( _nsapEnumeratorID==null && NSAPERegionEnumeratorID != null)
                {
                    if (NSAPRegionEnumerator == null)
                    {
                        return null;
                    }
                    else
                    {
                        _nsapEnumeratorID = NSAPRegionEnumerator.Enumerator.ID;
                    }
                }
                return _nsapEnumeratorID;
            }
        }

        public NSAPEnumerator NSAPEnumerator 
        {
            get
            {
                if(_nsapEnumerator ==null)
                {
                    _nsapEnumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)EnumeratorID);
                }
                return _nsapEnumerator;
            }
            set { _nsapEnumerator = value; }
        }

        public NSAPRegionEnumerator NSAPRegionEnumerator
        {
            set { _nsapRegionEnumerator = value; }
            get
            {
                if (_nsapRegionEnumerator == null)
                {
                    _nsapRegionEnumerator = NSAPRegion.NSAPEnumerators.FirstOrDefault(t => t.RowID == NSAPERegionEnumeratorID);
                    
                }
                return _nsapRegionEnumerator;
            }
        }

        public string Sector
        {
            get
            {
                return SectorCode == "c" ? "Commercial" :
                       SectorCode == "m" ? "Municipal" : "";
            }
        }

        [Column("fishing_vessel_group/fish_sector")]
        public string SectorCode { get; set; }

        [Column("vessel_sampling/region_enumerator_text")]
        public string RegionEnumeratorText { get; set; }

        public string EnumeratorName
        {
            get
            {
                string name;
                if(NSAPERegionEnumeratorID==null)
                {
                    name = RegionEnumeratorText;
                }
                else
                {
                    
                    name = NSAPRegionEnumerator==null?"": NSAPRegionEnumerator.Enumerator.Name;
                }
                return name;
            }
        }

        [Column("vessel_sampling/fma_in_region")]
        public int FMAInRegionID { get; set; }
        public NSAPRegionFMA NSAPRegionFMA
        {
            get
            {
                if (_nsapRegionFMA == null)
                {
                    _nsapRegionFMA = NSAPRegion.FMAs.FirstOrDefault(t => t.RowID == FMAInRegionID);
                }
                return _nsapRegionFMA;
            }
        }

        [Column("vessel_sampling/fishing_ground")]
        public int FishingGroundFMARegionID { get; set; }

        public NSAPRegionFMAFishingGround NSAPRegionFMAFishingGround
        {
            set { _nsapRegionFMAFishingGround = value; }
            get
            {
                if (_nsapRegionFMAFishingGround == null)
                {
                    _nsapRegionFMAFishingGround = NSAPRegionFMA.FishingGrounds.FirstOrDefault(t => t.RowID == FishingGroundFMARegionID);
                }
                return _nsapRegionFMAFishingGround;
            }
        }

        [Column("vessel_sampling/landing_site")]
        public int? NSAPRegionFishingGroundLandingSiteID { get; set; }

        public NSAPRegionFMAFishingGroundLandingSite NSAPRegionFMAFishingGroundLandingSite
        {
            set { _nsapRegionFMAFishingGroundLandingSite = value; }

            get
            {
                if (_nsapRegionFMAFishingGroundLandingSite == null)
                {
                    _nsapRegionFMAFishingGroundLandingSite = NSAPRegionFMAFishingGround.LandingSites.FirstOrDefault(t => t.RowID == NSAPRegionFishingGroundLandingSiteID);
                }
                return _nsapRegionFMAFishingGroundLandingSite;

            }
        }

        [Column("vessel_sampling/landing_site_text")]
        public string LandingSiteText { get; set; }

        public string LandingSiteName
        {
            get
            {
                string name;
                if(NSAPRegionFishingGroundLandingSiteID==null)
                {
                    name = LandingSiteText;
                }
                else
                {
                    name = NSAPRegionFMAFishingGroundLandingSite.LandingSite.ToString();
                }
                return name;
            }
    
        }

        [Column("vessel_sampling/gear_used")]
        public int? GearUsedID { get; set; }

        public NSAPRegionGear NSAPRegionGear
        {
            set { _nsapRegionGear = value; }
            get
            {
                if (_nsapRegionGear == null)
                {
                    _nsapRegionGear = NSAPRegion.Gears.FirstOrDefault(t => t.RowID == GearUsedID);
                }
                return _nsapRegionGear;
            }

        }

        [Column("vessel_sampling/gear_used_text")]
        public string GearUsedText { get; set; }

        public string GearName
        {
            get
            {
                string name;
                if (GearUsedID == null)
                {
                    name = GearUsedText;
                }
                else
                {
                    name = NSAPRegionGear.Gear.GearName;
                }
                return name;
            }
        }

        [Column("fishing_vessel_group/boat_used")]
        public int? VesselUsedID { get; set; }

        [Column("fishing_vessel_group/is_boat_used")]
        public string IsBoatUsedText { get; set; }

        public bool IsBoatUsed
        {
            get
            {
                if(IsBoatUsedText=="yes")
                {
                    return true;
                }
                else if(IsBoatUsedText=="no")
                {
                    return false;
                }
                else
                {
                    return VesselName.Length > 0;
                }
            }
            set
            {
                IsBoatUsed = value;
                if (IsBoatUsed)
                {
                    IsBoatUsedText = "yes";
                }
                else
                {
                    IsBoatUsedText = "no";
                }
            }
        }
        public FishingVessel FishingVessel
        {
            get
            {
                if (_fishingVessel == null)
                {
                    _fishingVessel = NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)VesselUsedID);
                }
                return _fishingVessel;
            }
            set { _fishingVessel = value; }
        }

        [Column("fishing_vessel_group/boat_used_text")]
        public string FishingVesselText { get; set; }

        public string VesselName
        {
            get
            {
                string name;
                if(VesselUsedID==null)
                {
                    name = FishingVesselText;
                }
                else
                {
                    name = FishingVessel.ToString();
                }
                return name;
            }
        }

        [Column("vessel_catch/trip_isSuccess")]
        public string TripIsSuccessText { get; set; }

        public bool TripIsSuccess
        {
            get
            {
                return TripIsSuccessText == "yes";
            }
            set
            {
                TripIsSuccess = value;
                if(TripIsSuccess)
                {
                    TripIsSuccessText = "yes";
                }
                else
                {
                    TripIsSuccessText = "no";
                }
            }
        }

        [Column("vessel_catch/catch_total")]
        public double? CatchWeightTotal { get; set; }

        [Column("vessel_catch/boxes_total")]
        public int? BoxesTotal { get; set; }

        [Column("vessel_catch/catch_sampled")]
        public double? CatchWeightSampled { get; set; }

        [Column("vessel_catch/boxes_sampled")]
        public int? BoxesSampled { get; set; }

        [Column("vessel_catch/raising_factor")]
        public double? RaisingFactor { get; set; }


        [Column("vessel_catch/remarks")]
        public string Remarks { get; set; }


        [Column("soak_time_group/include_soak_time")]
        public string IncludeSoakTime { get; set; }

        [Column("soak_time_group/include_tracking")]
        public string IncludeTracking { get; set; }

        public bool TripIsTracked
        {
            get
            {
                return IncludeTracking == "yes";
            }
            set
            {
                TripIsTracked = value;
                if (TripIsTracked)
                {
                    IncludeTracking = "yes";
                }
                else
                {
                    IncludeTracking = "no";
                }
            }
        }

        [Column("grid_coord_group/utmZone")]
        public string UTMZone { get; set; }

        [Column("soak_time_group/soaktime_tracking_group/gps")]
        public string GPSCode { get; set; }

        public GPS GPS
        {
            set { _gps = value; }
            get
            {
                if (_gps == null)
                {
                    _gps = NSAPEntities.GPSViewModel.GetGPS(GPSCode);
                }
                return _gps;
            }
        }

        [Column("soak_time_group/soaktime_tracking_group/time_depart_landingsite")]
        public DateTime? DateTimeDepartLandingSite { get; set; }

        [Column("soak_time_group/soaktime_tracking_group/time_arrive_landingsite")]
        public DateTime? DateTimeArriveLandingSite { get; set; }

        [Column("_index")]
        public int RowIndex { get; set; }


        [Column("_submission_time")]
        public DateTime DateTimeSubmitted { get; set; }



    }
}
