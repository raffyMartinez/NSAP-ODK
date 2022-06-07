using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class UnrecognizedFishingGroundComparer : IEqualityComparer<UnrecognizedFishingGround>
    {
        public bool Equals(UnrecognizedFishingGround x, UnrecognizedFishingGround y)
        {
            return x.RowID.Equals(y.RowID);
        }

        public int GetHashCode(UnrecognizedFishingGround obj)
        {
            return obj.RowID.GetHashCode();
        }
    }
}
