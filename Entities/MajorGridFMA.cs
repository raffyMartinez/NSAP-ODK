using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class MajorGridFMA
    {
        public int RowID { get; set; }
        public int MajorGridNumber { get; set; }
        public FMA FMA { get; set; }

        public UTMZone UTMZone { get; set; }

        public override string ToString()
        {
            if (FMA == null)
            {
                return $"{MajorGridNumber}-unknown FMA-{UTMZone}";
            }
            else
            {
                return $"{MajorGridNumber}-{FMA.Name}-{UTMZone}";
            }
        }

    }
}
