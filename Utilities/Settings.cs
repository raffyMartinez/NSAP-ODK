﻿using System;
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
        public string MDBPath { get; set; }
        public string JSONFolder { get; set; }

        public bool UsemySQL { get; set; }

        public string MySQLBackupFolder { get; set; }

        public int? CutOFFUndersizedCW { get; set; }


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
