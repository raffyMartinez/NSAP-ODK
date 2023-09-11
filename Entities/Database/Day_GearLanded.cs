using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class Day_GearLanded
    {
        public int Boats
        {
            get
            {
                int? count = null;
                switch(GearInLandingSite.Sector)
                {
                    case FisheriesSector.Commercial:
                        count = GearInLandingSite.CountLandingsCommercial;
                        break;
                    case FisheriesSector.Municipal:
                        count = GearInLandingSite.CountLandingsMunicipal;
                        break;
                }
                return (int)count;
            }
        }

        public double CatchWt
        {
            get
            {
                double? weight = null;
                switch (GearInLandingSite.Sector)
                {
                    case FisheriesSector.Commercial:
                        weight = GearInLandingSite.WeightCatchCommercial;
                        break;
                    case FisheriesSector.Municipal:
                        weight = GearInLandingSite.WeightCatchMunicipal;
                        break;
                }
                return (double)weight;
            }
        }
        public string GearAndSector
        {
            get
            {
                if(GearInLandingSite!=null)
                {
                    return GearInLandingSite.GearAndSector;
                }
                else
                {
                    return "NO LANDING";
                }
            }
        }
        public override string ToString()
        {
            if (GearInLandingSite != null)
            {
                return $"{LandingSiteSampling} -{GearInLandingSite}";
            }
            else
            {
                return $"{LandingSiteSampling} - NO GEAR";
            }
        }
        public LandingSiteSampling LandingSiteSampling { get; set; }
        public GearInLandingSite GearInLandingSite {get;set;}
    }
}
