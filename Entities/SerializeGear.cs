using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
namespace NSAP_ODK.Entities
{
    public class SerializeGear
    {
        public List<GearFlattened> Gears { get; set; }
        public void Save(string fileName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(SerializeGear));
                xmls.Serialize(sw, this);
            }
        }
    }
}
