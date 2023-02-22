using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    public static class WeightValidationUpdater
    {
        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        public static List<SummaryItem> SummaryItems { get; set; }
        public static SummaryItem SummaryItem { get; set; }




        public static string CSV { get; private set; }
        public static Task<bool> UpdateDatabaseAsync()
        {
            return Task.Run(() => UpdateDatabase());
        }
        public static List<VesselCatch> VesselCatches { get; set; }

        public static bool Cancel { get; set; }

        public static bool UpdateDatabase()
        {
            Cancel = false;
            StringBuilder csv = new StringBuilder();
            int loopCount = 0;
            int csv_update_counter = 0;

            int csv_loop = 2000;
            var looper = Global.Settings.DownloadSizeForBatchMode;
            if (looper != null && looper > 0)
            {
                csv_loop = (int)looper;
            }

            List<SummaryItem> itemList = new List<SummaryItem>();
            List<VesselCatchWV> vcwvs = new List<VesselCatchWV>();

            if (SummaryItem != null)
            {
                itemList.Add(SummaryItem);
                CSV = "";
            }
            else
            {
                vcwvs = VesselCatchRepository.GetVesselCatchForWV();

                var maxID = VesselUnloadRepository.WeightValidationTableMaxID();
                maxID = null;

                if (maxID != null)
                {
                    itemList = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.OrderBy(t => t.VesselUnloadID).Where(t => t.VesselUnloadID > (int)maxID).ToList();
                }
                else
                {
                    itemList = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.OrderBy(t => t.VesselUnloadID).ToList();
                }
            }

            //itemList = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.Where(t => t.VesselUnloadID >= 792).OrderBy(t => t.VesselUnloadID).ToList();
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.StartOfUpdate, VesselUnloadToUpdateCount = itemList.Count });
            foreach (SummaryItem item in itemList)
            {
                double version = item.FormVersionNumeric;
                if (Cancel)
                {
                    break;
                }

                bool hasSpeciesWtOfZero = false;
                int countTotalEnum = 0;
                int countFromSample = 0;
                bool computeForRaisedValue = false;
                double sumOfCatchCompositionSampleWeight = 0;
                double sumOfCatchCompositionWeight = 0;
                double sumOfCatchCompositionWeight_earlyVersion = 0;


                if (SummaryItem == null)
                {
                    item.ListOfCatch = vcwvs.Where(t => t.VesselUnloadID == item.VesselUnloadID).ToList();
                }
                else
                {
                    item.ListOfCatch = new List<VesselCatchWV>();
                    foreach (var c in VesselCatches)
                    {
                        VesselCatchWV vesselCatchWV = new VesselCatchWV
                        {
                            PK = c.VesselUnloadID,
                            FromTotalCatch = c.FromTotalCatch,
                            Species_kg = c.Catch_kg,
                            Species_sample_kg = c.Sample_kg
                        };
                        item.ListOfCatch.Add(vesselCatchWV);
                    }
                }


                if (item.WeightOfCatch != null && item.WeightOfCatchSample != null && item.WeightOfCatch > 0 && item.WeightOfCatchSample > 0)
                {
                    computeForRaisedValue = true;
                    double from_total_sum = 0;

                    if (item.CatchCompositionRows != null && item.CatchCompositionRows > 0)
                    {
                        if (version >= 6.43)
                        {
                            foreach (VesselCatchWV vc in item.ListOfCatch)
                            {
                                if (!hasSpeciesWtOfZero && vc.Species_kg == 0)
                                {
                                    hasSpeciesWtOfZero = true;
                                }
                                if (vc.FromTotalCatch)
                                {
                                    from_total_sum += (double)vc.Species_kg;
                                    countTotalEnum++;
                                }
                                else if (vc.Species_sample_kg != null)
                                {
                                    sumOfCatchCompositionSampleWeight += (double)vc.Species_sample_kg;
                                    countFromSample++;
                                }

                            }
                            item.SumOfCatchCompositionSampleWeight = sumOfCatchCompositionSampleWeight;
                        }
                        else
                        {

                            foreach (VesselCatchWV vc in item.ListOfCatch)
                            {
                                if (vc.Species_kg != null)
                                {
                                    sumOfCatchCompositionWeight_earlyVersion += (double)vc.Species_kg;
                                }
                                if (!hasSpeciesWtOfZero && vc.Species_kg == 0)
                                {
                                    hasSpeciesWtOfZero = true;
                                }
                                if (vc.Species_sample_kg == null || vc.Species_sample_kg == 0)
                                {

                                    from_total_sum += (double)vc.Species_kg;
                                    countTotalEnum++;
                                }
                                else
                                {
                                    sumOfCatchCompositionSampleWeight += (double)vc.Species_sample_kg;
                                    countFromSample++;
                                }
                            }
                            item.SumOfCatchCompositionSampleWeight = sumOfCatchCompositionSampleWeight;
                        }

                        item.RaisingFactor = ((double)item.WeightOfCatch - from_total_sum) / (double)item.WeightOfCatchSample;
                    }
                }
                else
                {
                    if (item.ListOfCatch != null)
                    {
                        foreach (VesselCatchWV vc in item.ListOfCatch)
                        {
                            if (!hasSpeciesWtOfZero && vc.Species_kg == 0)
                            {
                                hasSpeciesWtOfZero = true;
                            }
                            sumOfCatchCompositionWeight += (double)vc.Species_kg;

                            if (version >= 6.43)
                            {
                                if (vc.FromTotalCatch)
                                {
                                    countTotalEnum++;
                                }
                                else if (vc.Species_sample_kg != null)
                                {
                                    countFromSample++;
                                }

                            }
                            else
                            {
                                if (vc.Species_sample_kg != null)
                                {
                                    countFromSample++;
                                }
                                else if (vc.Species_kg != null && vc.Species_sample_kg == null)
                                {
                                    countTotalEnum++;
                                }
                            }
                        }
                        item.SumOfCatchCompositionWeight = sumOfCatchCompositionWeight;
                    }
                }

                if (computeForRaisedValue && item.ListOfCatch != null)
                {
                    foreach (VesselCatchWV vc in item.ListOfCatch)
                    {
                        if (vc.FromTotalCatch || vc.Species_sample_kg == null || vc.Species_sample_kg == 0)
                        {
                            sumOfCatchCompositionWeight += (double)vc.Species_kg;
                        }
                        else
                        {
                            sumOfCatchCompositionWeight += (double)vc.Species_sample_kg * item.RaisingFactor;
                        }
                    }
                    item.SumOfCatchCompositionWeight = sumOfCatchCompositionWeight;
                }

                if (item.SumOfCatchCompositionWeight > 0)
                {
                    item.DifferenceCatchWtandSumCatchCompWeight = Math.Abs((double)item.WeightOfCatch - (double)item.SumOfCatchCompositionWeight) / (double)item.WeightOfCatch * 100;
                    if (hasSpeciesWtOfZero)
                    {
                        item.WeightValidationFlag = WeightValidationFlag.WeightValidationInValid;
                    }
                    else if (item.DifferenceCatchWtandSumCatchCompWeight <= (int)Utilities.Global.Settings.AcceptableWeightsDifferencePercent)
                    {
                        item.WeightValidationFlag = WeightValidationFlag.WeightValidationValid;
                    }
                    else
                    {
                        item.WeightValidationFlag = WeightValidationFlag.WeightValidationInValid;
                        if (version < 6.43 && Math.Abs(sumOfCatchCompositionWeight_earlyVersion - (double)item.WeightOfCatch) < .1)
                        {
                            item.WeightValidationFlag = WeightValidationFlag.WeightValidationValid;
                            item.SumOfCatchCompositionWeight = sumOfCatchCompositionWeight_earlyVersion;
                            item.DifferenceCatchWtandSumCatchCompWeight = Math.Abs((double)item.WeightOfCatch - (double)item.SumOfCatchCompositionWeight) / (double)item.WeightOfCatch * 100;
                        }
                    }
                }
                else if (hasSpeciesWtOfZero)
                {
                    item.WeightValidationFlag = WeightValidationFlag.WeightValidationInValid;
                }
                else if (item.SumOfCatchCompositionSampleWeight > item.WeightOfCatchSample)
                {
                    item.WeightValidationFlag = WeightValidationFlag.WeightValidationInValid;
                }

                if (version < 6.43)
                {
                    if (item.WeightOfCatch != null && item.WeightOfCatch > 0 && item.ListOfCatch != null && item.ListOfCatch.Count > 0)
                    {
                        if (item.WeightOfCatchSample == null)
                        {
                            item.SamplingTypeFlag = SamplingTypeFlag.SamplingTypeTotalEnumeration;
                        }
                        else if (item.WeightOfCatchSample > 0)
                        {
                            item.SamplingTypeFlag = SamplingTypeFlag.SamplingTypeSampled;
                        }
                    }
                }
                else if (countFromSample > 0 && countTotalEnum > 0)
                {
                    item.SamplingTypeFlag = SamplingTypeFlag.SamplingTypeMixed;
                }
                else if (countFromSample > 0)
                {
                    item.SamplingTypeFlag = SamplingTypeFlag.SamplingTypeSampled;
                }
                else if (countTotalEnum > 0)
                {
                    item.SamplingTypeFlag = SamplingTypeFlag.SamplingTypeTotalEnumeration;
                }
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.UpdateFound });

                if (SummaryItem == null)
                {
                    csv.AppendLine(MakeUnloadCSVLine(item));
                    double q = (double)loopCount / (double)csv_loop;
                    if (loopCount > 0 && q == (int)q)
                    {
                        csv_update_counter += csv_loop;
                        if (VesselUnloadRepository.BulkUpdateWeightValidationUsingCSV(csv))
                        {
                            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.BatchCSVUploaded, VesselUnloadWeightValidationUpdateCount = csv_update_counter });
                            csv.Clear();
                        }
                    }
                    else
                    {

                    }

                    loopCount++;
                    UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.SummaryItemProcessed, SummaryItemProcessedCount = loopCount });
                }
                else
                {
                    CSV = MakeUnloadCSVLine(item);
                    return CSV.Length > 0;
                }
            }
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.EndOfUpdate });

            if (csv.ToString().Length > 0)
            {
                VesselUnloadRepository.BulkUpdateWeightValidationUsingCSV(csv);
                csv.Clear();
            }

            return !Cancel;

        }
        private static string MakeUnloadCSVLine(SummaryItem item)
        {
            var diff = "";
            if (item.DifferenceCatchWtandSumCatchCompWeight != null)
            {
                if (item.DifferenceCatchWtandSumCatchCompWeight == 0 || item.DifferenceCatchWtandSumCatchCompWeight < 0.01)
                {
                    diff = "0";
                }
                else
                {
                    diff = ((double)item.DifferenceCatchWtandSumCatchCompWeight).ToString("N2");
                }
            }
            string rv = item.VesselUnloadID.ToString();
            //it turns out when a value has decimal point, there are cases where only the whole number part is saved
            //that is why the values (sum of catch comp wt, sum of catch comp sample wt) are in quotes
            rv += $",\"{item.SumOfCatchCompositionWeight}\"";
            rv += $",\"{item.SumOfCatchCompositionSampleWeight}\"";
            rv += $",{(int)item.WeightValidationFlag}";
            rv += $",{(int)item.SamplingTypeFlag}";
            rv += $",\"{diff}\"";
            rv += $",\"{item.FormVersionCleaned}\"";
            rv += $",\"{item.RaisingFactor}\"";
            return rv;
        }
    }
}
