using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.TreeViewModelControl;

namespace NSAP_ODK.Entities.Database
{
    public class SpeciesCalendarDay
    {
        private Gear _gear;
        public DateTime SamplingDate { get; set; }
        public LandingSite LandingSite { get; set; }

        public GearUnload GearUnload { get; set; }
        public int? CountGearTypes { get; set; }

        public int CountVesselUnloads { get; set; }
        public AllSamplingEntitiesEventHandler MonthViewModel { get; set; }
        public Gear Gear
        {
            get
            {
                if (_gear == null)
                {
                    _gear = NSAPEntities.GearViewModel.GetGear(GearCode);

                }
                return _gear;
            }
            set
            {
                _gear = value;
            }
        }
        public string SectorCode { get; set; }
        public double? TotalWeightOfCatch { get; set; }
        public string Sector
        {
            get
            {
                string sector = "";
                switch (SectorCode)
                {
                    case "m":
                        sector = "Municipal";
                        break;
                    case "c":
                        sector = "Commercial";
                        break;
                }
                return sector;
            }
        }
        public int? TotalNumberOfLandings { get; set; }
        public int? TotalNumberOfLandingsSampled { get; set; }

        public int? CountMunicipalLandings { get; set; }
        public int? CountCommercialLandings { get; set; }
        public double? TotalWeightMunicipalLandings { get; set; }
        public double? TotalWeightCommercialLandings { get; set; }
        public string Remarks { get; set; }
        public List<VesselUnload> VesselUnloads { get; set; }
        public bool IsSamplingDay { get; set; }
        public bool HasFishingOperation { get; set; }
        public override string ToString()
        {

            return $"SamplingDate:{SamplingDate.ToString("MMM-dd-yyyy")} - Taxa:{Taxa} -  Species:{SpeciesName} - Gear:{GearName}  - Sector:{Sector}";
        }
        public string GearCode { get; set; }
        public int SpeciesID { get; set; }

        public string GearName
        {
            get
            {
                if (Gear == null)
                {
                    Gear = NSAPEntities.GearViewModel.GetGear(GearCode);
                }
                return Gear.ToString();
            }
        }
        public string SpeciesName
        {
            get
            {
                if (TaxaCode == "FIS")
                {
                    return NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesID).ToString();
                }
                else
                {
                    return NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesID).ToString();
                }
            }
        }
        public string MaturityStage
        {
            get
            {
                string maturity = "";
                switch (MaturityStageCode)
                {
                    case "pr":
                        maturity = "Premature";
                        break;
                    case "im":
                        maturity = "Immature";
                        break;
                    case "de":
                        maturity = "Developing";
                        break;
                    case "ma":
                        maturity = "Maturing";
                        break;

                    case "mt":
                        maturity = "Mature";
                        break;
                    case "ri":
                        maturity = "Ripening";
                        break;
                    case "gr":
                        maturity = "Gravid";
                        break; ;
                    case "spw":
                        maturity = "Spawning";
                        break;
                    case "sp":
                        maturity = "Spent";
                        break;
                    default:
                        maturity = "";
                        break;

                }
                return maturity;
            }
        }
        public string MaturityStageCode { get; set; }
        public string Species { get; set; }
        public string TaxaCode { get; set; }

        public string Taxa
        {
            get
            {
                return NSAPEntities.TaxaViewModel.GetTaxa(TaxaCode).ToString();
            }
        }
        public int NumberOfLandingsOfSpecies { get; set; }
        public double WeightOfSpeciesLanded { get; set; }
        public int CountLenFreqMeas { get; set; }
        public int CountLenWtMeas { get; set; }
        public int CountLenMeas { get; set; }
        public int CountMaturityMeas { get; set; }
    }
}
