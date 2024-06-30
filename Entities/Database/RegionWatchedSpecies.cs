using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;
namespace NSAP_ODK.Entities.Database
{
    public class RegionWatchedSpeciesForAdding
    {
        public RegionWatchedSpeciesForAdding(RegionWatchedSpecies rws)
        {
            if (rws != null)
            {
                RegionWatchedSpecies = rws;
            }
            else
            {
                RegionWatchedSpecies = new RegionWatchedSpecies();
                RegionWatchedSpecies.NSAPRegion = NSAPEntities.NSAPRegionViewModel.CurrentEntity;
            }
        }
        public RegionWatchedSpecies RegionWatchedSpecies { get; private set; }
        [ItemsSource(typeof(TaxaItemsSource))]
        public string Taxa { get; set; }
        [ItemsSource(typeof(GenusFromTaxaItemsSource))]
        public string Genus { get; set; }
        [ItemsSource(typeof(SpeciesFromGenusItemsSource))]
        public string Species { get; set; }
        public bool AddToWatchList { get; set; }
    }
    public class RegionWatchedSpecies
    {

        public int PK { get; set; }
        public FishSpecies FishSpecies
        {
            get; set;
            //    get
            //    {
            //        if (TaxaCode == "FIS" && _fishSpecies == null)
            //        {
            //            _fishSpecies = NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesID);
            //            return _fishSpecies;
            //        }
            //        else
            //        {
            //            return null;
            //        }
            //    }
        }
        public NotFishSpecies NotFishSpecies
        {
            get; set;
            //get
            //{
            //    if (TaxaCode != "FIS" && _notFishSpecies == null)
            //    {
            //        _notFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesID);
            //        return _notFishSpecies;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
        }

        public string Family
        {
            get
            {
                if (TaxaCode == "FIS")
                {
                    return FishSpecies.Family;
                }
                else
                {
                    return NotFishSpecies.Taxa.ToString();
                }
            }
        }
        public NSAPRegion NSAPRegion { get; set; }
        public int SpeciesID { get; set; }
        public string TaxaCode { get; set; }

        public string SpeciesName
        {
            get
            {
                string sp_name = "";
                if (TaxaCode == "FIS")
                {
                    if (FishSpecies == null)
                    {
                        FishSpecies = NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesID);
                    }
                    sp_name = FishSpecies.ToString();
                }
                else
                {
                    if (NotFishSpecies == null)
                    {
                        NotFishSpecies = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesID);
                    }
                    sp_name = NotFishSpecies.ToString();
                }
                return sp_name;
            }
        }
    }
}
