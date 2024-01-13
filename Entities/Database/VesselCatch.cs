using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;

namespace NSAP_ODK.Entities.Database
{
    public class VesselCatchFlattened
    {
        public VesselCatchFlattened(VesselCatch vesselCatch)
        {
            ID = vesselCatch.PK;
            ParentID = vesselCatch.Parent.Parent.PK;
            CatchName = vesselCatch.CatchName;
            Taxa = vesselCatch.Taxa.ToString();
            Catch_kg = vesselCatch.Catch_kg;
            Sample_kg = vesselCatch.Sample_kg;
            //TWS = vesselCatch.TWS;
        }

        public int ID { get; private set; }
        public int ParentID { get; private set; }

        public string CatchName { get; private set; }

        public string Taxa { get; private set; }
        public double? Catch_kg { get; private set; }
        public double? Sample_kg { get; private set; }

        public double? TWS { get; private set; }
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
            GearCode = vc.GearCode;
            GearText = vc.GearText;
            FromTotalCatch = vc.FromTotalCatch;
            PriceOfSpecies = vc.PriceOfSpecies;
            PriceUnit = vc.PriceUnit;
            OtherPriceUnit = vc.OtherPriceUnit;
            IsCatchSold = vc.IsCatchSold;
            VesselCatch = vc;
            if (!string.IsNullOrEmpty(vc.GearText))
            {
                GearUsed = vc.GearText;
            }
            else
            {
                GearUsed = vc.Gear.GearName;
            }
            //TWS = vc.TWS;


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
        [ReadOnly(true)]
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }
        public VesselCatch VesselCatch { get; set; }
        public string TaxaName { get; set; }

        [ItemsSource(typeof(TaxaItemsSource))]
        public string TaxaCode { get; set; }
        [ItemsSource(typeof(GenusFromTaxaItemsSource))]
        public string Genus { get; set; }
        [ItemsSource(typeof(SpeciesFromGenusItemsSource))]
        public string Species { get; set; }
        public string OtherName { get; set; }

        public int? SpeciesID { get; set; }

        public double? Catch_kg { get; set; }
        public double? Sample_kg { get; set; }

        public double? TWS { get; set; }
        //public double? Boxes_count { get; set; }
        //public int? Boxes_sampled { get; set; }
        //public double? RaisingFactor { get; set; }

        public string GearCode { get; set; }
        public string GearText { get; set; }
        [ItemsSource(typeof(GearItemsSource))]
        public string GearUsed { get; set; }
        public bool FromTotalCatch { get; set; }
        public double? PriceOfSpecies { get; set; }

        public string PriceUnit { get; set; }
        public string OtherPriceUnit { get; set; }
        public bool IsCatchSold { get; set; }
    }
    public class VesselCatch
    {
        public bool IsCatchSold { get; set; }
        private VesselUnload _parent;
        private VesselUnload_FishingGear _parentFishingGear;
        private CarrierLanding _parentCarrierLanding;
        private bool _fromTotalCatch;

        public double? PriceOfSpecies { get; set; }

        public string OtherPriceUnit { get; set; }
        public string PriceUnit { get; set; }
        public bool DelayedSave { get; set; }
        public CatchLenFreqViewModel CatchLenFreqViewModel { get; set; }
        public CatchLengthViewModel CatchLengthViewModel { get; set; }
        public CatchLengthWeightViewModel CatchLengthWeightViewModel { get; set; }
        public CatchMaturityViewModel CatchMaturityViewModel { get; set; }
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }
        public string WeighingUnit { get; set; }
        public int? SpeciesID { get; set; }
        public string TaxaCode { get; set; }
        public string GearCode { get; set; }
        public string GearText { get; set; }

        public string GearNameUsedEx
        {
            get
            {

                if (string.IsNullOrEmpty(GearCode) && string.IsNullOrEmpty(GearText))
                {
                    return "";
                }
                else if (string.IsNullOrEmpty(GearCode))
                {
                    return GearText;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(GearCode).GearName;
                }

            }
        }
        public string GearNameUsed
        {
            get
            {
                if (Parent is null)
                {
                    return "";
                }
                else if (Parent.IsMultiGear)
                {
                    if (string.IsNullOrEmpty(GearCode))
                    {
                        return GearText;
                    }
                    else
                    {
                        return NSAPEntities.GearViewModel.GetGear(GearCode).GearName;
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        public Gear Gear
        {
            get
            {
                if (string.IsNullOrEmpty(GearCode))
                {
                    return null;
                }
                else
                {
                    return NSAPEntities.GearViewModel.GetGear(GearCode);
                }
            }
        }
        public bool FromTotalCatch
        {
            get
            {
                if (ParentFishingGear == null || ParentFishingGear.WeightOfSample == null)
                {
                    return true;
                }
                else
                {
                    return _fromTotalCatch;
                }
            }
            set { _fromTotalCatch = value; }
        }


        private string FishSpeciesName(int speciesID)
        {
            var sp = NSAPEntities.FishSpeciesViewModel.GetSpecies((int)SpeciesID);
            if (sp == null)
            {
                return "";
            }
            else
            {
                return sp.ToString();
            }
        }

        private string NotFishSpeciesName(int speciesID)
        {
            return NSAPEntities.NotFishSpeciesViewModel.GetSpecies((int)SpeciesID).ToString();
        }
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

        public void SetTaxa(Taxa taxa)
        {
            TaxaCode = taxa.Code;
        }

        public Taxa Taxa
        {
            get { return NSAPEntities.TaxaViewModel.GetTaxa(TaxaCode); }
        }
        public string SpeciesText { get; set; }

        public double? Catch_kg { get; set; }
        public double? Sample_kg { get; set; }

        //public double? TWS { get; set; }

        public string CatchNameEx
        {
            get
            {
                string rv = "";
                if (SpeciesID != null)
                {
                    if ((int)SpeciesID < 500000)
                    {
                        rv = FishSpeciesName((int)SpeciesID);
                    }
                    else
                    {
                        rv = NotFishSpeciesName((int)SpeciesID);
                    }
                }
                else if (SpeciesText != null && SpeciesText.Length > 0)
                {
                    rv = SpeciesText;
                }

                return rv;
            }
        }

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
                    return FishSpecies?.ToString();
                }
                else
                {
                    if (NotFishSpecies == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return NotFishSpecies.ToString();
                    }
                }

            }
        }

        public List<CatchLenFreq> ListCatchLenFreq
        {
            get
            {
                if (CatchLenFreqViewModel == null)
                {
                    CatchLengthViewModel = new CatchLengthViewModel(this);
                }

                return CatchLenFreqViewModel.CatchLenFreqCollection.ToList();
                //.Where(t => t.Parent.PK == PK).ToList();

            }
        }

        public List<CatchLengthWeight> ListCatchLengthWeight
        {
            get
            {
                if (CatchLengthViewModel == null)
                {
                    CatchLengthViewModel = new CatchLengthViewModel(this);
                }
                return CatchLengthWeightViewModel.CatchLengthWeightCollection.ToList();
                //Where(t => t.Parent.PK == PK).ToList();

            }
        }

        public List<CatchLength> ListCatchLength
        {
            get
            {
                if (CatchLengthViewModel == null)
                {
                    CatchLengthViewModel = new CatchLengthViewModel(this);
                }

                return CatchLengthViewModel.CatchLengthCollection.ToList();
                //.Where(t => t.Parent.PK == PK).ToList();

            }
        }

        public List<CatchMaturity> ListCatchMaturity
        {
            get
            {
                if (CatchMaturityViewModel == null)
                {
                    CatchMaturityViewModel = new CatchMaturityViewModel(this);
                }

                return CatchMaturityViewModel.CatchMaturityCollection.ToList();
                //.Where(t => t.Parent.PK == PK).ToList();
            }
        }

        public CarrierLanding ParentCarrierLanding
        {
            set { _parentCarrierLanding = value; }
            get
            {
                if (_parentCarrierLanding == null)
                {
                    // _parentEx = NSAPEntities
                    return null;
                }
                return _parentCarrierLanding;
            }
        }
        public VesselUnload_FishingGear ParentFishingGear
        {
            set { _parentFishingGear = value; }
            get
            {
                if (_parentFishingGear == null)
                {
                    // _parentEx = NSAPEntities
                    return null;
                }
                return _parentFishingGear;
            }
        }
        public VesselUnload Parent
        {
            set { _parent = value; }
            get
            {
                //if (ParentFishingGear == null)
                //{
                //    return null;
                //}
                return _parent;
                //else if (_parent == null)
                //{
                //    //_parent = NSAPEntities.VesselUnloadViewModel.getVesselUnload(VesselUnloadID);
                //    //_parent = ParentFishingGear.Parent;
                //    return _parent;
                //}
                //return _parent;
            }
        }
    }
}
