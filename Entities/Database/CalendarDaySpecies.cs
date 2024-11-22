using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarDaySpecies
    {
        public CalendarDay Parent { get; private set; }
        public CalendarDaySpecies(CalendarDay day)
        {
            Parent = day;
        }
        public string Taxa_code { get; set; }
        public FishSpecies FishSpecies { get; set; }
        public NotFishSpecies NotFishSpecies { get; set; }
        public int CountLandings { get; set; }
        public double WeightLanded { get; set; }

        public string SpeciesName
        {
            get
            {
                if(FishSpecies!=null)
                {
                    return FishSpecies.ToString();
                }
                else
                {
                    return NotFishSpecies.ToString();
                }
            }
        }
    }

    public class CalendarDaySpeciesMeasured
    {
        public CalendarDay Parent { get; private set; }
        public CalendarDaySpeciesMeasured(CalendarDay day)
        {
            Parent = day;
        }
        public string Taxa_code { get; set; }
        public FishSpecies FishSpecies { get; set; }
        public NotFishSpecies NotFishSpecies { get; set; }
        public int CountMeasured { get; set; }
        public string MeasurementType { get; set; }

        public string MaturityStage { get; set; }

        public bool IsFemaleMaturity { get; set; }

        public override string ToString()
        {
            if (IsFemaleMaturity)
            {
                   return $"{Parent}-{SpeciesName} stage:{MaturityStage} count measured:{CountMeasured}";
            }
            else
            {
                return $"{Parent}-{SpeciesName}-count measured:{CountMeasured}";
            }
        }

        public string SpeciesName
        {
            get
            {
                if (FishSpecies != null)
                {
                    return FishSpecies.ToString();
                }
                else
                {
                    return NotFishSpecies.ToString();
                }
            }
        }
    }


}
