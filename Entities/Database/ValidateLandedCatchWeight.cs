using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{

    public class ValidateLandedCatchWeight
    {
        private SummaryItem _summaryItem;

        public ValidateLandedCatchWeight(){}
        public ValidateLandedCatchWeight(VesselUnload vu)
        {
            if(vu.VesselCatchViewModel==null)
            {
                vu.VesselCatchViewModel = new VesselCatchViewModel(vu);
            }
            VesselUnload = vu;
        }
        public SummaryItem SummaryItem
        {
            get { return _summaryItem; }
            set
            {
                _summaryItem = value;
                VesselUnload = _summaryItem.VesselUnload;
            }
        }

        public VesselUnload VesselUnload { get; set; }
        public double? RaisingFactor
        {
            get
            {
                if (VesselUnload.HasCatchComposition && VesselUnload.WeightOfCatch != null && VesselUnload.WeightOfCatchSample != null)
                {
                    return VesselUnload.WeightOfCatch / SummaryItem.VesselUnload.WeightOfCatchSample;
                }
                else
                {
                    return null;
                }
            }
        }

        public string ValidationResult
        {
            get
            {
                switch (LandedCatchValidationResult)
                {
                    case LandedCatchValidationResult.ValidationResultNotApplicable:
                        return "Not applicable";
                    case LandedCatchValidationResult.ValidationResultCatchWeightIsValid:
                        return "Valid";
                    default:
                        return "Else case";

                }
            }
        }

        public int NumberOfSpeciesInCatchComposition
        {
            get
            {
                return VesselUnload.CountCatchCompositionItems;
            }
          
        }
        public string PrecisionOfCatchCompostionWeightText
        {
            get
            {
                if(PrecisionOfCatchCompositionWeight==null)
                {
                    return "";
                }
                else
                {
                    return ((double)PrecisionOfCatchCompositionWeight).ToString("N3");
                }
            }
        }
        public double? PrecisionOfCatchCompositionWeight
        {
            get
            {
                var totalSampleWt = VesselUnload.VesselCatchViewModel != null ? VesselUnload.VesselCatchViewModel.VesselCatchCollection.Sum(t => t.Sample_kg) : null;
                
                if (totalSampleWt == null)
                {
                    return null;
                }
                else
                {
                    if (VesselUnload.WeightOfCatchSample == null)
                    {
                        return null;
                    }
                    else
                    {
                        var diff = Math.Abs((double)totalSampleWt - (double)VesselUnload.WeightOfCatchSample);
                        return (diff / (double)totalSampleWt);
                    }
                }
            }
        }
        public string TypeOfSamplingOfCatchComposition
        {
            get
            {
                string samplingType = "";
                if (VesselUnload.HasCatchComposition)
                {
                    if(VesselUnload.VesselCatchViewModel==null)
                    {
                        VesselUnload.VesselCatchViewModel = new VesselCatchViewModel(VesselUnload);
                    }
                    if(VesselUnload.WeightOfCatchSample!=null)
                    {
                        var totalSampleWt = VesselUnload.VesselCatchViewModel != null ? VesselUnload.VesselCatchViewModel.VesselCatchCollection.Sum(t => t.Sample_kg) : null;
                        var diff = Math.Abs((double)totalSampleWt - (double)VesselUnload.WeightOfCatchSample);
                        if(VesselUnload.ListVesselCatch.Count(t=>t.FromTotalCatch)>0)
                        {
                            samplingType = "Mixed";
                        }
                        else if(diff/totalSampleWt<=0.01)
                        //if(diff/totalSampleWt <= (Utilities.Global.Settings.AcceptableWeightsDifferencePercent/100))
                        {
                            samplingType = "Sample from catch";
                        }
                        else
                        {
                            samplingType = "Mixed";
                        }
                    }
                    else
                    {
                        samplingType = "Total enumeration";
                    }
                }
                else
                {
                    samplingType = "No catch composition";
                }
                return samplingType;
            }
        }
        //public double? FinalTotalCatchCompWeight
        //{
        //    get
        //    {
        //        if(VesselUnload.CountCatchCompositionItems>0)
        //        {
        //            foreach(var item in VesselUnload.VesselCatchViewModel.VesselCatchCollection)
        //            {

        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
        public LandedCatchValidationResult LandedCatchValidationResult
        {
            get
            {
                if (VesselUnload == null)
                {
                    return LandedCatchValidationResult.ValidationResultNotApplicable;
                }
                else
                {
                    if (!VesselUnload.HasCatchComposition)
                    {
                        return LandedCatchValidationResult.ValidationResultNoCatchComposition;
                    }
                    else if (VesselUnload.WeightOfCatchSample == null)
                    {
                        //double totalCatchCompWeight = 0;
                        var totalCatchCompWeight = VesselUnload.VesselCatchViewModel != null ? VesselUnload.VesselCatchViewModel.VesselCatchCollection.Sum(t => t.Catch_kg) : null;
                        if (totalCatchCompWeight == VesselUnload.WeightOfCatch)
                        {
                            return LandedCatchValidationResult.ValidationResultCatchWeightIsValid;
                        }
                        else
                        {
                            return LandedCatchValidationResult.ValidationResultCatchWeightIsInvalid;
                        }
                    }
                    else
                    {
                        return LandedCatchValidationResult.ValidationResultCatchWeightIsValid;
                    }
                }
            }
        }
    }
}
