using System.Collections.Generic;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{

    public class FishingVessel
    {

        public FisheriesSector FisheriesSector { get; set; }

        public string SectorString
        {
            get
            {
                string sectorString = "";
                switch(FisheriesSector)
                {
                    case FisheriesSector.Aquaculture:
                        sectorString = "Aquaculture";
                        break;
                    case FisheriesSector.Commercial:
                        sectorString = "Commercial";
                        break;
                    case FisheriesSector.Municipal:
                        sectorString = "Municipal";
                        break;
                    case FisheriesSector.Unknown:
                        sectorString = "Unknown";
                        break;
                }
                return sectorString;
            }
        }
        public List<Engine> Engines { get; set; }
        public string RegistrationNumber { get; set; }
        public string Name { get; set; }
        public double? Length { get; set; }
        public double? Depth { get; set; }
        public double? Breadth { get; set; }

        public string NameOfOwner { get; set; }

        [ReadOnly(true)]
        public int ID { get; set; }

        public List<Gear> Gears { get; set; }

        public string NameToUse(bool addPrefix = true)
        {
            if (Name == null || Name.Length == 0)
            {
                return NameOfOwner;
            }
            else
            {
                if (addPrefix)
                {
                    return $"F/V {Name}";
                }
                else
                {
                    return Name;
                }
            }
        }

        public override string ToString()
        {
            if (Name == null || Name.Length == 0)
            {
                return NameOfOwner;
            }
            else
            {
                return $"F/V {Name}";
            }
        }
    }
}