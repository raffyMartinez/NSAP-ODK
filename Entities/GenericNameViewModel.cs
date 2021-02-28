using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public static class GenericNameViewModel
    {
        static GenericNameViewModel()
        {
            Taxa = NSAPEntities.TaxaViewModel.FishTaxa;
        }
        public static Taxa Taxa { get; set; }
        public static List<string> GenusList
        {
            get
            {
                switch(Taxa.Code)
                {
                    case "FIS":
                        return NSAPEntities.FishSpeciesViewModel.GetAllGenus();
                    default:
                        return NSAPEntities.NotFishSpeciesViewModel.GetAllGenus(Taxa);
                }
            }
        }
    }
}
