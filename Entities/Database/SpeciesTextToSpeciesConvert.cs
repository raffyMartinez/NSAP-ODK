using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SpeciesTextToSpeciesConvert
    {

        public static int FillDictionary()
        {
            var catchList = NSAPEntities.VesselCatchViewModel.GetAllVesselCatches().Where(t => t.SpeciesText != null && t.SpeciesText.Length > 0 && t.SpeciesID != null).OrderBy(t => t.SpeciesText);
            var list = catchList.GroupBy(x => new { x.SpeciesText, x.SpeciesID, x.Taxa, x.FishSpecies, x.NotFishSpecies }, (key, group) => new
            {
                spText = key.SpeciesText,
                spID = key.SpeciesID,
                spTaxa = key.Taxa,
                spFishSpecies = key.FishSpecies,
                spNotFish = key.NotFishSpecies,
                Result = group.ToList()
            });

            var nameCounts = list.GroupBy(t => t.spText)
                                 .Select(n => new
                                 { spName = n.Key, spCount = n.Count() }
                                 )
                                 .OrderBy(t => t.spName);

            foreach (var item in nameCounts)
            {
                List<SpeciesTextToSpeciesConvert> thislist = new List<SpeciesTextToSpeciesConvert>();
                if (item.spCount == 1)
                {
                    var itemInList = list.Where(t => t.spText == item.spName).FirstOrDefault();
                    thislist.Add(new SpeciesTextToSpeciesConvert { SpeciesText = itemInList.spText, SpeciesID = (int)itemInList.spID, TaxaCode = itemInList.spTaxa.Code });
                }
                else
                {
                    foreach (var itemInList in list.Where(t => t.spText == item.spName))
                    {
                        thislist.Add(new SpeciesTextToSpeciesConvert { SpeciesText = itemInList.spText, SpeciesID = (int)itemInList.spID, TaxaCode = itemInList.spTaxa.Code });
                    }
                }
                SpeciesConveterdDict.Add(item.spName, thislist);

            }

            return SpeciesConveterdDict.Count();
        }
        public static Dictionary<string, List<SpeciesTextToSpeciesConvert>> SpeciesConveterdDict { get; set; } = new Dictionary<string, List<SpeciesTextToSpeciesConvert>>();
        public string SpeciesText { get; set; }
        public int SpeciesID { get; set; }

        public string TaxaCode { get; set; }

        public bool IsFishTaxa { get { return TaxaCode == NSAPEntities.TaxaViewModel.FishTaxa.Code; } }

        public NotFishSpecies NotFishSpecies
        {
            get
            {
                if (IsFishTaxa)
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesID);
                }
            }
        }
        public FishSpecies FishSpecies
        {
            get
            {
                if (IsFishTaxa)
                {
                    return NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesID);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
