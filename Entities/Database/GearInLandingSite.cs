using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class GearInLandingSite
    {
        public override string ToString()
        {
            string comm_count;
            string mun_count;
            string toString = $"{GearUsedName}-Unknown";
            switch (Sector)
            {
                case FisheriesSector.Commercial:
                    comm_count = ((int)CountLandingsCommercial).ToString();
                    toString = $"{GearUsedName}-Commercial:{comm_count} Wt{WeightCatchCommercial}";
                    break;
                case FisheriesSector.Municipal:
                    mun_count = ((int)CountLandingsMunicipal).ToString();
                    toString = $"{GearUsedName}-Municipal:{mun_count} Wt{WeightCatchMunicipal}";
                    break;
                case FisheriesSector.Aquaculture:
                    toString = $"{GearUsedName}-Aquculture";
                    break;
            }

            return toString;
        }
        public string GearAndSector
        {
            get
            {
                if (string.IsNullOrEmpty(GearCode))
                {
                    return $"{GearUsedName}_{SectorCode}";
                }
                else
                {
                    return $"{Gear.GearName}_{SectorCode}";
                }
            }
        }
        public string GearCode { get; set; }
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

        public string GearUsedName
        {
            get
            {
                if (string.IsNullOrEmpty(GearCode))
                {
                    return GearText;
                }
                else
                {
                    return Gear.GearName;
                }
            }
        }
        public string GearText { get; set; }
        public LandingSiteSampling Parent { get; set; }

        public string SectorName
        {
            get
            {
                string s = "";
                switch (Sector)
                {
                    case FisheriesSector.Aquaculture:
                        s = "Aquaculture";
                        break;
                    case FisheriesSector.Municipal:
                        s = "Municipal";
                        break;
                    case FisheriesSector.Commercial:
                        s = "Commercial";
                        break;
                    default:
                        s = "Unknown";
                        break;
                }
                return s;
            }
        }
        public string SectorCode
        {
            get
            {
                string s = "";
                switch (Sector)
                {
                    case FisheriesSector.Aquaculture:
                        s = "a";
                        break;
                    case FisheriesSector.Municipal:
                        s = "m";
                        break;
                    case FisheriesSector.Commercial:
                        s = "c";
                        break;
                    default:
                        s = "u";
                        break;
                }
                return s;
            }
        }

        public int? CountGearLandings
        {
            get
            {
                int? landings = null;
                switch(Sector)
                {
                    case FisheriesSector.Commercial:
                        landings = CountLandingsCommercial;
                        break;
                    case FisheriesSector.Municipal:
                        landings = CountLandingsMunicipal;
                        break;
                }
                return landings;
            }
        }

        public double? WeightGearLandings
        {
            get
            {
                double? weight = null;
                switch (Sector)
                {
                    case FisheriesSector.Commercial:
                        weight = WeightCatchCommercial;
                        break;
                    case FisheriesSector.Municipal:
                        weight = WeightCatchMunicipal;
                        break;
                }
                return weight;
            }
        }
        public FisheriesSector Sector { get; set; }
        public int? CountLandingsCommercial { get; set; }
        public int? CountLandingsMunicipal { get; set; }
        public double? WeightCatchCommercial { get; set; }
        public double? WeightCatchMunicipal { get; set; }
    }
}
