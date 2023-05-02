using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CatchWithZeroWeight
    {
        public int? SpeciesID { get; set; }
        public string SpeciesName { get; set; }
        public string FishingGround
        {
            get
            {
                return Parent.Parent.Parent.FishingGround.Name;
            }
        }


        public string FMA
        {
            get
            {
                return Parent.Parent.Parent.FMA.Name;
            }
        }
        public string SpeciesNameToUse
        {
            get
            {
                if(SpeciesID==null)
                {
                    return SpeciesName;
                }
                else if(Taxa.Code=="FIS")
                {
                    if (FishSpecies != null)
                    {
                        return FishSpecies.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return NotFishSpecies.ToString();
                }
            }
        }

        public string FormVersion
        {
            get
            {
                return Parent.FormVersion;
            }
        }
        public string Enumerator
        {
            get
            {
                return Parent.EnumeratorName;
            }
        }

        public string FishingVessel
        {
            get
            {
                return Parent.VesselName;
            }
        }
        public string GearName
        {
            get
            {
                return Parent.Parent.GearUsedName;
            }
        }
        public string SamplingDateFormatted
        {
            get
            {
                return Parent.SamplingDate.ToString("MMM-dd-yyyy");
            }
        }
        public string LandingSiteName
        {
            get
            {
                return Parent.Parent.Parent.LandingSiteName;
            }
        }
        public FishSpecies FishSpecies { get; set; }
        public NotFishSpecies NotFishSpecies { get; set; }
        public Taxa Taxa { get; set; }

        public int VesselUnloadID { get; set; }
        public VesselUnload Parent { get; set; }
        public double? CorrectedWeight { get; set; }
        public bool JSONFileFound { get; set; }
    }
}
