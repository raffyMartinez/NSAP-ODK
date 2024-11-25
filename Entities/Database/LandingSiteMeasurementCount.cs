using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class LandingSiteMeasurementCount
    {
        public string GearCode { get; set; }

        public Gear Gear
        {
            get
            {
                return NSAPEntities.GearViewModel.GetGear(GearCode);
            }
        }
        public DateTime SampledMonth { get; set; }
        public int SpeciesID { get; set; }
        public string SpeciesName { get; set; }
        public int CountMeasurement { get; set; }
        public string MeasurementType { get; set; }
        public string TaxaCode { get; set; }
    }

    public class LandingSiteMeasurements
    {
        public string TaxaName { get; set; }
        public string Species { get; set; }
        public DateTime MonthSampled { get; set; }
        public int CountLengthMeasurements { get; set; }
        public int CountLengthWeightMeasurements { get; set; }
        public int CountLengthFreqMeasurements { get; set; }
        public int CountMaturityMeasurements { get; set; }

        public int? CountLength
        {
            get
            {
                if(CountLengthMeasurements==0)
                {
                    return null;
                }
                else
                {
                    return CountLengthMeasurements;
                }
            }
        }

        public int? CountLenFreq
        {
            get
            {
                if (CountLengthFreqMeasurements == 0)
                {
                    return null;
                }
                else
                {
                    return CountLengthFreqMeasurements;
                }
            }
        }

        public int? CountLenWt
        {
            get
            {
                if (CountLengthWeightMeasurements == 0)
                {
                    return null;
                }
                else
                {
                    return CountLengthWeightMeasurements;
                }
            }
        }

        public int? CountMat
        {
            get
            {
                if (CountMaturityMeasurements == 0)
                {
                    return null;
                }
                else
                {
                    return CountMaturityMeasurements;
                }
            }
        }
    }
}
