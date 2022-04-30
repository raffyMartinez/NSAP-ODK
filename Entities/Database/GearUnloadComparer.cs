using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class GearUnloadComparer : IEqualityComparer<GearUnload>
    {
        public bool Equals(GearUnload x, GearUnload y)
        {
            return x.PK.Equals(y.PK);
        }

        public int GetHashCode(GearUnload obj)
        {
            return obj.PK.GetHashCode();
        }
    }
}
