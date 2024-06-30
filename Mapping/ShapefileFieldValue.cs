using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;

namespace NSAP_ODK.Mapping
{
    public class ShapefileFieldTypeValue
    {
        public FieldType FieldType { get; set; }
        public int? Width { get; set; }
        public int? Precision { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
