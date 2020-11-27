using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelCatchComposition
    {
        private FishSpecies _fishSpecies;
        private NotFishSpecies _notFishSpecies;
        private Taxa _taxa;
        private ExcelMainSheet _parent;

        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/taxa")]
        public string TaxonomicCode { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/species")]
        public int? FishSpeciesCode { get; set; }


        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/spName_other")]
        public string CatchCompositionNameText { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/species_notfish")]
        public int? NonFishSpeciesCode { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/species_wt")]
        public double SpeciesWeight { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/species_sample_wt")]
        public double? SpeciesSampleWeight { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/speciesname_group/sum_species_weight_note")]
        public string Notes { get; set; }

        [Column("_index")]
        public int RowIndex { get; set; }

        [Column("_parent_index")]
        public int ParentIndex { get; set; }

        public FishSpecies FishSpecies
        {
            set { _fishSpecies = value; }
            get
            {
                if(_fishSpecies==null && FishSpeciesCode != null)
                {
                    _fishSpecies = NSAPEntities.FishSpeciesViewModel.SpeciesCollection.FirstOrDefault(t => t.SpeciesCode == FishSpeciesCode);
                }
                return _fishSpecies;
            }
        }

        public NotFishSpecies NotFishSpecies
        {
            set { _notFishSpecies = value; }
            get
            {
                if (_notFishSpecies == null && NonFishSpeciesCode != null)
                {
                    _notFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)NonFishSpeciesCode);
                }
                return _notFishSpecies;
            }
        }

        public Taxa Taxa
        {
            get
            {
                if(_taxa==null)
                {
                    _taxa = NSAPEntities.TaxaViewModel.GetTaxa(TaxonomicCode);
                }
                return _taxa;
            }
            set { _taxa = value; }
        }

        public ExcelMainSheet Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelMainSheets.FirstOrDefault(t => t.RowIndex == ParentIndex);
                }
                return _parent;
            }
        }

        public string SpeciesName
        {
            get
            {
                string name;
                if (CatchCompositionNameText != null && CatchCompositionNameText.Length > 0)
                {
                    name = CatchCompositionNameText;
                }
                else if (FishSpeciesCode != null)
                {
                    name = FishSpecies.ToString();
                }
                else
                {
                    name = NotFishSpecies.ToString();
                }
                return name;
            }
        }
       public int? SpeciesCode(string taxa)
        {
            if (CatchCompositionNameText == null)
            {
                if (taxa == "FIS")
                {
                    return FishSpeciesCode;
                }
                else
                {
                    return NonFishSpeciesCode;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
