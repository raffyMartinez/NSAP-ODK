using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSAP_ODK.Entities.Database
{
    public enum JsonFileType
    {
        jsonFileTypeCatchAndEffort,
        jsonFileTypeLandingsCount,
        jsonFileTypeCatchEffortLandingsCount
    }
    public class JSONFile : IDisposable
    {
        private string _jsonText;
        private List<VesselLanding> _vesselLandings;
        private string _fullFileName;

        public void CleanUp()
        {
            _jsonText = null;
            _vesselLandings.Clear();
            _vesselLandings = null;
        }

        public bool IsMultivessel { get; set; }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _jsonText = null;
                if (_vesselLandings != null)
                {
                    _vesselLandings.Clear();
                    _vesselLandings = null;
                }
            }
            // free native resources if there are any.
        }
        public JsonFileType JsonFileType { get; set; }
        public string JSONText
        {
            get
            {
                if (_jsonText == null || _jsonText.Length == 0)
                {
                    _jsonText = File.ReadAllText(FullFileName);
                    MD5 = Utilities.MD5.CreateMD5(_jsonText);
                }
                return _jsonText;
            }
            set
            {
                _jsonText = value;
                MD5 = Utilities.MD5.CreateMD5(_jsonText);
            }
        }
        public string VersionString { get; set; }

        public int? CountVesselLandings { get; set; }
        public List<VesselLanding> VesselLandings
        {
            get
            {
                if (_vesselLandings == null)
                {
                    _vesselLandings = JsonConvert.DeserializeObject<List<VesselLanding>>(JSONText);
                    //if (_vesselLandings != null)
                    //{
                    //    Utilities.Logger.Log($"vessel landings of {Path.GetFileName(FullFileName)} ({_vesselLandings.Count} items) created");
                    //}
                }
                return _vesselLandings;
            }
            set { _vesselLandings = value; }
        }


        public string FileName
        {
            get { return Path.GetFileName(FullFileName); }
        }
        public List<string> LandingIdentifiers { get; set; }
        public string FullFileName
        {
            get { return _fullFileName; }
            set
            {
                _fullFileName = value;
            }
        }

        public int Count { get; set; }

        public string FormID { get; set; }

        public DateTime Earliest { get; set; }

        public DateTime Latest { get; set; }

        public string MD5 { get; set; }

        public string Description { get; set; }

        public int RowID { get; set; }

        public DateTime DateAdded { get; set; }

        public NSAPRegion NSAPRegion { get; set; }

    }
}
