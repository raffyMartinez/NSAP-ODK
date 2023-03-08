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
    public class NotFishSpeciesEdit
    {

        public NotFishSpeciesEdit()
        {

        }
        public NotFishSpeciesEdit(NotFishSpecies nfs)
        {
            NotFishSpecies = nfs;
            Genus = nfs.Genus;
            Species = nfs.Species;
            MaxSize = nfs.MaxSize;
            TaxaCode = nfs.Taxa.Code;
            
            if(nfs.SizeType!=null)
              SizeTypeCode = nfs.SizeType.Code;
            
            SpeciesID = nfs.SpeciesID;
        }
        public NotFishSpecies NotFishSpecies { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public double? MaxSize { get; set; }

        [ItemsSource(typeof(TaxaItemSourceNotFish))]
        public string TaxaCode { get; set; }

        [ItemsSource(typeof(SizeTypeItemsSource))]
        public string SizeTypeCode { get; set; }

        [ReadOnly(true)]
        public int SpeciesID { get; set; }
    }

    public class NotFishSpecies
    {
        public int SpeciesID { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public Taxa Taxa { get; set; }
        public double? MaxSize { get; set; }
        public SizeType SizeType { get; set; }

        public override string ToString()
        {
            return $"{Genus} {Species}";
        }

    }
}
