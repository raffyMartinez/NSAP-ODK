﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class VesselCatchFlattened
    {
        public VesselCatchFlattened(VesselCatch vesselCatch)
        {
            ID = vesselCatch.PK;
            ParentID = vesselCatch.Parent.PK;
            CatchName = vesselCatch.CatchName;
            Taxa = vesselCatch.Taxa.ToString();
            Catch_kg = vesselCatch.Catch_kg;
            Sample_kg = vesselCatch.Sample_kg;
        }

        public int ID { get; private set; }
        public int ParentID { get; private set; }

        public string CatchName { get; private set; }

        public string Taxa { get; private set; }
        public double? Catch_kg { get; private set; }
        public double? Sample_kg { get; private set; }
    }

    public class VesselCatchEdited
    {
        public VesselCatchEdited()
        {

        }
        public VesselCatchEdited(VesselCatch vc)
        {
            PK = vc.PK;
            VesselUnloadID = vc.VesselUnloadID;
            TaxaCode = vc.TaxaCode;
            TaxaName = vc.Taxa.Name;
            Sample_kg = vc.Sample_kg;
            Catch_kg = vc.Catch_kg;
            

            if ((vc.SpeciesText == null || vc.SpeciesText.Length == 0) && vc.SpeciesID != null)
            {
                if (TaxaCode == "FIS")
                {
                    Genus = vc.FishSpecies.GenericName;
                    Species = vc.FishSpecies.SpecificName;


                }
                else
                {
                    Genus = vc.NotFishSpecies.Genus;
                    Species = vc.NotFishSpecies.Species;
                }
                SpeciesID = vc.SpeciesID;
            }
            else
            {
                OtherName = vc.SpeciesText;
            }
        }
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }

        public string TaxaName { get; set; }
        public string TaxaCode { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public string OtherName { get; set; }

        public int? SpeciesID { get; set; }

        public double? Catch_kg { get; set; }
        public double? Sample_kg { get; set; }
        public double? Boxes_count { get; set; }
        public int? Boxes_sampled { get; set; }
        public double? RaisingFactor { get; set; }
    }
    public class VesselCatch
    {
        private VesselUnload _parent;
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }

        public int? SpeciesID { get; set; }
        public string TaxaCode { get; set; }
        public FishSpecies FishSpecies
        {
            get
            {
                if (TaxaCode == "FIS" && SpeciesID != null)
                {
                    return NSAPEntities.FishSpeciesViewModel.GetSpecies((int)SpeciesID);
                }
                else
                {
                    return null;
                }
            }
        }

        public NotFishSpecies NotFishSpecies
        {
            get
            {
                if (TaxaCode != "FIS" && SpeciesID != null)
                {
                    return NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)SpeciesID);
                }
                else
                {
                    return null;
                }
            }
        }

        public Taxa Taxa
        {
            get { return NSAPEntities.TaxaViewModel.GetTaxa(TaxaCode); }
        }
        public string SpeciesText { get; set; }

        public double? Catch_kg { get; set; }
        public double? Sample_kg { get; set; }

        public string CatchName
        {
            get
            {
                if (SpeciesText != null && SpeciesText.Length > 0)
                {
                    return SpeciesText;
                }
                else if (SpeciesID != null && TaxaCode == "FIS")
                {
                    return FishSpecies.ToString();
                }
                else
                {
                    return NotFishSpecies.ToString();
                }

            }
        }

        public List<CatchLenFreq> ListCatchLenFreq
        {
            get
            {
                return NSAPEntities.CatchLenFreqViewModel.CatchLenFreqCollection
                    .Where(t => t.Parent.PK == PK).ToList();

            }
        }

        public List<CatchLengthWeight> ListCatchLengthWeight
        {
            get
            {
                return NSAPEntities.CatchLengthWeightViewModel.CatchLengthWeightCollection
                    .Where(t => t.Parent.PK == PK).ToList();

            }
        }

        public List<CatchLength> ListCatchLength
        {
            get
            {
                return NSAPEntities.CatchLengthViewModel.CatchLengthCollection
                    .Where(t => t.Parent.PK == PK).ToList();

            }
        }

        public List<CatchMaturity> ListCatchMaturity
        {
            get
            {
                return NSAPEntities.CatchMaturityViewModel.CatchMaturityCollection
                    .Where(t => t.Parent.PK == PK).ToList();
            }
        }
        public VesselUnload Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.VesselUnloadViewModel.getVesselUnload(VesselUnloadID);
                }
                return _parent;
            }
        }
    }
}
