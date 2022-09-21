using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;
namespace NSAP_ODK.Entities
{
    public class FishSpeciesEdit
    {
        public FishSpeciesEdit()
        {

        }

        public FishSpeciesEdit(FishSpecies fishSpecies)
        {
            FishSpecies = fishSpecies;
            GenericName = fishSpecies.GenericName;
            SpecificName = fishSpecies.SpecificName;
            Family = fishSpecies.Family;
            MaxLength = fishSpecies.LengthMax;
            CommonLength = fishSpecies.LengthCommon;

            if (fishSpecies.LengthType != null)
                LengthType = fishSpecies.LengthType.Code;

            SpeciesCode = fishSpecies.SpeciesCode;
            RowNumber = fishSpecies.RowNumber;
            Importance = fishSpecies.Importance;
            MainCatchingMethod = fishSpecies.MainCatchingMethod;
        }

        public FishSpecies FishSpecies { get; set; }
        public string GenericName { get; set; }
        public string SpecificName { get; set; }

        [ItemsSource(typeof(FishFamilyItemsSource))]
        public string Family { get; set; }

        public double? MaxLength { get; set; }

        public double? CommonLength { get; set; }

        [ItemsSource(typeof(SizeTypeItemsSource))]
        public string LengthType { get; set; }

        [ReadOnly(true)]
        public int RowNumber { get; set; }

        public int? SpeciesCode { get; set; }

        [ItemsSource(typeof(ImportanceItemsSource))]
        public string Importance { get; set; }

        [ItemsSource(typeof(MainCatchingMethodItemsSource))]
        public string MainCatchingMethod { get; set; }
    }

    public class SelectedFishSpeciesData
    {
        public SelectedFishSpeciesData(FishSpecies species)
        {
            FishSpecies = species;
        }
        public FishSpecies FishSpecies { get; set; }
        public bool AlreadyInLocalSpeciesList { get; set; }
    }
    public class FishSpecies
    {
        public int RowNumber { get; set; }
        public string GenericName { get; set; }
        public string SpecificName { get; set; }

        public double? LengthCommon { get; set; }

        public SizeType LengthType { get; set; }
        public double? LengthMax { get; set; }
        public string Importance { get; set; }

        public string MainCatchingMethod { get; set; }
        public string Family { get; set; }
        public int? SpeciesCode { get; set; }

        public string NameInOldFishbase { get; set; }

        public string Synonym
        {
            get
            {
                if (NameInOldFishbase != null && NameInOldFishbase.Length > 0 && NameInOldFishbase != ToString())
                {
                    return NameInOldFishbase;
                }
                else
                {
                    return "";
                }
            }
        }


        public override string ToString()
        {
            return $"{GenericName} {SpecificName}";
            //if (SpeciesCode != null)
            //{
            //    return $"{GenericName} {SpecificName}";
            //}
            //else
            //{
            //    return "";
            //}
        }
        public FishSpecies(int rowNumber, string genus, string species)
        {
            RowNumber = rowNumber;
            GenericName = genus;
            SpecificName = species;
        }

        public FishSpecies()
        {

        }

    }
}
