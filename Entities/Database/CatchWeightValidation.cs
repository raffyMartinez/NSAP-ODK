using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CatchWeightValidation
    {
        public VesselUnload VesselUnload { get; set; }
        public VesselUnload_FishingGear VesselUnload_FishingGear { get; set; }
        public double? TotalWeigthCatchComposition { get; set; }
        public double? TotalWeightOfSampleFromCatch { get; set; }

        public WeightValidationFlag WeightValidationFlag { get; set; }
        public SamplingTypeFlag SamplingTypeFlag { get; set; }
        public double WeightDifference { get; set; }
        public string FormVersion { get; set; }
        public double? RaisingFactor { get; set; }
        public double? DifferenceCatchWtandSumCatchCompWeight { get; set; }
    }
}
