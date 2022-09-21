using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class TotalWtSp
    {
        public int RowID { get; set; }
        public GearUnload Parent { get; set; }
        public Taxa Taxa { get; set; }
        public int? SpeciesID { get; set; }
        public string SpeciesText { get; set; }
        public double TWSP { get; set; }

        public string SpeciesNameUsed
        {
            get
            {
                if(SpeciesText.Length>0)
                {
                    return SpeciesText;
                }    
                else
                {
                    if (FishSpecies != null)
                    {
                        return FishSpecies.ToString();
                    }
                    else
                    {
                        return NotFishSpecies.ToString();
                    }
                }
            }
        }
        public FishSpecies FishSpecies
        {
            get
            {
                if (SpeciesID == null || Taxa.Code != "FIS")
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.FishSpeciesViewModel.GetSpecies((int)SpeciesID);
                }

            }
        }
        public NotFishSpecies NotFishSpecies
        {
            get
            {
                if (SpeciesID == null || Taxa.Code == "FIS")
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)SpeciesID);
                }

            }
        }
    }
}
