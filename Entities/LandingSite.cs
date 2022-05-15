using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class LandingSite
    {
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
