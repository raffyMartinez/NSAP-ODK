using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class FemaleMaturitySummary
    {
        public FemaleMaturitySummary(string regionCode,
            int fmaID, 
            string fishingGroundCode, 
            int landingSiteID, 
            string gearCode, 
            int year, 
            int month,
            string taxa,
            int speciesID,
            string maturityCode,
            int count
            )
        {
            Region = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(regionCode);
            FMA = NSAPEntities.FMAViewModel.GetFMA(fmaID);
            FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(fishingGroundCode);
            LandingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite(landingSiteID);
            Gear = NSAPEntities.GearViewModel.GetGear(gearCode);
            SampledMonth = new DateTime(year, month, 1);
            Taxa = NSAPEntities.TaxaViewModel.GetTaxa(taxa);
            if(taxa=="FIS")
            {
                SpeciesName = NSAPEntities.FishSpeciesViewModel.GetSpecies(speciesID).ToString();
            }
            else
            {
                SpeciesName = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(speciesID).ToString();
            }
            MaturityStage = CatchMaturity.MaturityStageFromCode(maturityCode);
            Stage = CatchMaturity.MaturityStageEnum;
            Count = count;
        }

        public string GearCode { get; set; }
        public MaturityStageEnum Stage { get; set; }
        public NSAPRegion Region { get; set; }
        public FMA FMA { get; set; }
        public FishingGround FishingGround { get; set; }
        public LandingSite LandingSite { get; set; }
        public Gear Gear { get; set; }
        public DateTime SampledMonth { get; set; }
        public Taxa Taxa { get; set; }
       public string SpeciesName { get; set; }
        public string MaturityStage { get; set; }
        public int Count { get; set; }
    }
}
