using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Utilities
{
    public class MDBColumnInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string TypeName { get; set; }
        public int Sequence { get; set; }
        public override string ToString()
        {
            return $"{TableName} - {ColumnName} - {TypeName} - {Sequence}";
        }
    }
}
