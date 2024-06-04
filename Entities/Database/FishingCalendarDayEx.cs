using DocumentFormat.OpenXml.Presentation;
using NSAP_ODK.TreeViewModelControl;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class FishingGearAndSector
    {
        public FishingGearAndSector(Gear g , string sector_code)
        {
            Gear = g;
            SectorCode = sector_code; 
        }
        public Gear Gear { get; set; }
        public string SectorCode { get; set; }

        public override int GetHashCode()
        {
            int hc = (Gear.Code, SectorCode).GetHashCode();
            return hc;
        }
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
                    default:
                        sector = "Other";
                        break;
                }
                return sector;
            }
        }
        public override string ToString()
        {
            return $"{Gear} - {Sector}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is FishingGearAndSector)
            {
                FishingGearAndSector fgs = obj as FishingGearAndSector;
                return fgs.Gear.Code == Gear.Code && fgs.SectorCode == SectorCode;
            }
            else
            {
                return false;
            }
        }
    }
    public class FishingCalendarDayEx
    {
        public DateTime SamplingDate { get; set; }
        public LandingSite LandingSite { get; set; }

        public GearUnload GearUnload { get; set; }
        public int? CountGearTypes { get; set; }

        public int CountVesselUnloads { get; set; }
        public AllSamplingEntitiesEventHandler MonthViewModel { get; set; }
        public Gear Gear { get; set; }
        public string SectorCode { get; set; }
        public double? TotalWeightOfCatch { get; set; }
        public string Sector
        {
            get
            {
                string sector = "";
                switch(SectorCode)
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
            string gear = Gear == null ? "NO_GEAR" : Gear.GearName;
            return $"SamplingDate:{SamplingDate.ToString("MMM-dd-yyyy")} - IsSamplingDay:{IsSamplingDay} - HasFishingOperation: {HasFishingOperation} - Gear:{gear} - VesselUnloads:{CountVesselUnloads}";
        }
    }
}
