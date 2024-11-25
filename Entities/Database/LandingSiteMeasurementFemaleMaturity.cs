using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteMeasurementFemaleMaturity
    {
        public DateTime MonthSampled { get; set; }
        public string TaxaCode { get; set; }

        public string TaxaName
        {
            get
            {
                return NSAPEntities.TaxaViewModel.GetTaxa(TaxaCode).ToString();
            }
        }
        public int SpeciesCode { get; set; }
        public int? CountStagePremature { get; set; }
        public int? CountStageImmature { get; set; }
        public int? CountStageDeveloping { get; set; }
        public int? CountStageRipenening { get; set; }
        public int? CountStageSpawning { get; set; }
        public int? CountStageSpent { get; set; }

        public string SpeciesName
        {
            get
            {
                if (TaxaCode == "FIS")
                {
                    return NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesCode).ToString();
                }
                else
                {
                    return NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesCode).ToString();
                }
            }
        }




    }
}
