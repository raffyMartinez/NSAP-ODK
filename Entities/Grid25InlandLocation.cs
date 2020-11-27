using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class Grid25InlandLocation
    {
        public Grid25GridCell Grid25GridCell { get; set; }
        public int RowID { get; set; }

        public override string ToString()
        {
            return $"{Grid25GridCell.UTMZone}-{Grid25GridCell}";
        }
    }
}
