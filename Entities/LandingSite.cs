using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class LandingSite
    {
        public List<NSAPRegionFMAFishingGround> FishingGrounds { get; set; } = new List<NSAPRegionFMAFishingGround>();

        public string FishingGroundList
        {
            get
            {
                var ls_list = "";
                foreach (var fg in FishingGrounds)
                {
                    ls_list += $"{fg.RegionFMA.FMA} - {fg.FishingGround}, ";
                }
                return ls_list.Trim(new char[] { ' ', ',' });
            }
        }
        public LandingSite() { }

        public LandingSite(int id, string landingSiteName, Municipality municipality)
        {
            LandingSiteID = id;
            LandingSiteName = landingSiteName;
            Municipality = municipality;
        }

        public int LandingSiteID { get; set; }
        public string LandingSiteName { get; set; }

        public string Barangay { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool IsUsed { get; set; }

        public string WhereUsed { get; set; }
        public override bool Equals(object obj)
        {
            var otherLS = obj as LandingSite;
            if (otherLS == null)
            {
                return false;
            }
            else
            {
                return LandingSiteID == otherLS.LandingSiteID;
            }
        }

        public override int GetHashCode()
        {

            return LandingSiteID.GetHashCode();

        }
        public override string ToString()
        {
            if (Municipality != null)
            {
                if (Barangay != null && Barangay.Length >= 1)
                {
                    return $"{LandingSiteName}, Brgy. {Barangay}, {Municipality.ToString()}";
                }
                else
                {
                    return $"{LandingSiteName}, {Municipality.ToString()}";
                }
            }
            else
            {
                return $"{LandingSiteName}";
            }
        }

        public Municipality Municipality { get; set; }
    }
}
