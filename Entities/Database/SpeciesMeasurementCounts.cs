using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SpeciesMeasurementCounts
    {
        string _speciesName;
        string _taxaCode;
        public string TaxaCode
        {
            get { return _taxaCode; }

            set
            {
                _taxaCode = value;
                Taxa = NSAPEntities.TaxaViewModel.GetTaxa(_taxaCode);
            }
        }
        public Taxa Taxa { get; private set; }
        public string SpeciesName
        {
            get
            {
                if(_taxaCode=="FIS")
                {
                    return NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesID).ToString();
                }
                else
                {
                    return NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesID).ToString();
                }
            }
        }
        public int SpeciesID { get; set; }
        public int? CountLFMeasurements { get; set; }
        public int? CountLWMeasurements { get; set; }
        public int? CountLenMeasurements { get; set; }
        public int? CountMatMeasurements { get; set; }
    }
}
