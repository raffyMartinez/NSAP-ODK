using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.NSAPMysql
{
    public class TableStats
    {
        public string TableName{ get; set; }
        public int Rows { get; set; }

        public string Comment { get; set; }

        public DateTime Created { get; set; }
        public override string ToString()
        {
            return $"{TableName} - {Rows}";
        }
    }
}
