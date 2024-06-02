using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    public class SamplingDaySubmissionViewModel
    {
        bool _editSuccess = false;
        public ObservableCollection<SamplingDaySubmission> SamplingDaySubmissionCollection { get; set; }
        private SamplingDaySubmissionRepository SamplingDaySubmissions { get; set; }
        public SamplingDaySubmissionViewModel()
        {
            SamplingDaySubmissions = new SamplingDaySubmissionRepository();
            SamplingDaySubmissionCollection = new ObservableCollection<SamplingDaySubmission>(SamplingDaySubmissions.SamplingDaySubmissions);
            SamplingDaySubmissionCollection.CollectionChanged += SamplingDaySubmissionCollection_CollectionChanged;
        }

        private void SamplingDaySubmissionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = SamplingDaySubmissions.Add(SamplingDaySubmissionCollection[newIndex]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        List<SamplingDaySubmission> tempListOfRemovedItems = e.OldItems.OfType<SamplingDaySubmission>().ToList();
                        _editSuccess = SamplingDaySubmissions.Delete(tempListOfRemovedItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        List<SamplingDaySubmission> tempList = e.NewItems.OfType<SamplingDaySubmission>().ToList();
                        _editSuccess = SamplingDaySubmissions.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public SamplingDaySubmission GetSamplingDaySubmission(string landingSiteText, string fishingGroundID, DateTime samplingDate)
        {
            try
            {
                SamplingDaySubmission sds = SamplingDaySubmissionCollection.FirstOrDefault(t =>
                                t.SamplingDate.Date == samplingDate.Date &&
                                t.FishingGroundID == fishingGroundID &&
                                t.LandingSiteText == landingSiteText);

                return sds;
            }
            catch { return null; }
        }

        public SamplingDaySubmission GetSamplingDaySubmissionEx(int landingSiteID, string typeOfSampling, DateTime samplingDate)
        {
            try
            {
                SamplingDaySubmission sds = SamplingDaySubmissionCollection.Where(t =>
                                t.SamplingDate.Date == samplingDate.Date &&
                                t.TypeOfSampling == typeOfSampling &&
                                t.LandingSiteID == landingSiteID).FirstOrDefault();

                return sds;
            }
            catch
            {
                return null;
            }
        }
        public SamplingDaySubmission GetSamplingDaySubmission(int landingSiteID, string fishingGroundID, DateTime samplingDate)
        {
            try
            {
                SamplingDaySubmission sds = SamplingDaySubmissionCollection.Where(t =>
                                t.SamplingDate.Date == samplingDate.Date &&
                                t.FishingGroundID == fishingGroundID &&
                                t.LandingSiteID == landingSiteID).FirstOrDefault();

                return sds;
            }
            catch 
            {
                return null; 
            }
        }
        public bool Exists(int landingSiteID, string fishingGroundID, DateTime samplingDate)
        {
            return SamplingDaySubmissionCollection.FirstOrDefault(t =>
                t.SamplingDate == samplingDate &&
                t.FishingGroundID == fishingGroundID &&
                (int)t.LandingSiteID == landingSiteID) != null;
        }

        public bool Add(LandingSiteSampling lss)
        {
            SamplingDaySubmission sds = new SamplingDaySubmission
            {
                SamplingDate = lss.SamplingDate,
                //LandingSiteID = lss?.LandingSite.LandingSiteID,
                //FishingGroundID = lss.FishingGround.Code,
                SamplingDayID = lss.PK,
                LandingSiteSampling = lss,
                TypeOfSampling = lss.LandingSiteTypeOfSampling
            };
            if(lss.FishingGround==null)
            {

            }
            else
            {
                sds.FishingGroundID = lss.FishingGround.Code;
            }
            if(lss.LandingSite!=null)
            {
                sds.LandingSiteID = lss.LandingSite.LandingSiteID;
            }
            else
            {
                sds.LandingSiteText=lss.LandingSiteText;
            }

            SamplingDaySubmissionCollection.Add(sds);
            return _editSuccess;
        }
        internal int Count()
        {
            return SamplingDaySubmissionCollection.Count;
        }
    }
}
