using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities.Database
{
    public static class CSVFIleManager
    {
        public static Dictionary<string, LookupCSVFile> CSVFiles { get; private set; } = new Dictionary<string, LookupCSVFile>();
        public static string XMLError { get; private set; }
        private static string Directory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool ReadCSVXML()
        {
            CSVFiles.Clear();
            if (File.Exists(Directory + "\\" + "csv_files.xml"))
            {

                var xml = File.ReadAllText(Directory + "\\" + "csv_files.xml");
                if (Global.IsValidXML(xml, out string message))
                {

                    using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
                    {
                        LookupCSVFile csv = null;
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                if (reader.Name == "FileName")
                                {
                                    csv = new LookupCSVFile { FileName = reader.ReadInnerXml() };
                                }
                                else if (reader.Name == "Description")
                                {
                                    csv.Desccription = reader.ReadInnerXml();
                                }
                                else if (reader.Name == "Displayed")
                                {
                                    csv.IsSelectList = reader.ReadInnerXml() == "1";
                                    CSVFiles.Add(csv.FileName, csv);
                                }

                            }
                        }
                    }
                    return CSVFiles.Count>0;
                }
                else
                {
                    XMLError = message;
                }
            }
            else
            {
                XMLError = "CSV XML file not found";
                return false;
            }

            return CSVFiles.Count>0;
        }
    }
}
