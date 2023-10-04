using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class Koboserver
    {
        bool _dummy = false;
        public int ServerNumericID { get; set; }
        public string FormName { get; set; }
        public string ServerID { get; set; }

        public string Owner { get; set; }

        public string FormVersion { get; set; }
        public string eFormVersion { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime? DateLastSubmission { get; set; }
        public int SubmissionCount { get; set; }
        public int UserCount { get; set; }

        public DateTime DateLastAccessed { get; set; }

        public bool IsFishLandingSurveyForm
        {
            get
            {
                return FormName.Contains("NSAP Fish Catch Monitoring e-Form") || FormName.Contains("Fisheries landing survey");
            }
            set { _dummy = value; }
        }

        public bool IsFishLandingMultiGearSurveyForm
        {
            get
            {
                return  FormName.Contains("NSAP Fish Catch Monitoring e-Form") && FormName.Contains("MultiGear");
            }
            set
            {
                //
            }
        }

        public bool IsFishLandingMultiVesselSurveyForm
        {
            get
            {
                return FormName.Contains("NSAP Fish Catch Monitoring e-Form") && FormName.Contains("Multi-Vessel");
            }
            set
            {
                //
            }
        }

        public int SavedInDBCount { get; set; }

        public string LastUploadedJSON { get; set; }
        public string LastCreatedJSON { get; set; }

        public override string ToString()
        {
            return FormName;
        }
    }
}
