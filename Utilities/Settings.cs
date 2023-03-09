using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NSAP_ODK.Utilities
{
    public class Settings
    {

        public static int DefaultCutoffUndesizedCW = 11;
        public static int DefaultDownloadSizeForBatchMode = 2000;
        public static int DefaultWeigthDiffPercent = 10;

        private static int? _acceptableWeightsDifferencePercent;
        public string MDBPath { get; set; }
        public string JSONFolder { get; set; }

        public string FisheriesLandingSurveyNumericID { get; set; }
        public string TBL_TWSPKoboserverServerNumericID { get; set; }
        public string NSAPFishCatchMonitoringKoboserverServerNumericID { get; set; }
        public bool UsemySQL { get; set; }
        public string FileNameFBSpeciesUpdate { get; set; }
        public int? AcceptableWeightsDifferencePercent
        {
            get
            {
                if (_acceptableWeightsDifferencePercent == null)
                {
                    _acceptableWeightsDifferencePercent = DefaultWeigthDiffPercent;
                }
                return _acceptableWeightsDifferencePercent;
            }
            set { _acceptableWeightsDifferencePercent = value; }
        }

        public string MySQLBackupFolder { get; set; }

        public int? CutOFFUndersizedCW { get; set; }


        public int? DownloadSizeForBatchMode { get; set; }


        //public List<string> Setting2 { get; set; }

        public void Save(string filename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(sw, this);
            }
        }
        public static Settings Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                return xmls.Deserialize(sw) as Settings;
            }
        }
    }
}
