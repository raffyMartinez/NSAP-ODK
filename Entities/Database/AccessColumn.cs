 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class AccessColumn
    {
        public string MySQLType { get; set; }
        public string AccessColumnName { get; set; }
        public string MySQLColumnName { get; set; }

        public override string ToString()
        {
            return AccessColumnName;
        }
    }
}
