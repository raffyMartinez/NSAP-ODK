using System;
namespace NSAP_ODK.Entities.Database
{
    public class DeleteLandingSiteSamplingFromOrphanEventArg : EventArgs
    {
        public int LandinggSiteSamplingToDeleteTotalCount { get; set; }
        public string Intent { get; set; }
        public int CountDeleted { get; set; }

        public string SamplingDeleted { get; set; }
    }
}