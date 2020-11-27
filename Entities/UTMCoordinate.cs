using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class UTMCoordinate
    {
        public UTMZone UTMZone { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public override string ToString()
        {
            return $"{UTMZone.ToString()} {X} {Y}";
        }

        public UTMCoordinate(UTMZone zone, int x, int y)
        {
            UTMZone = zone;
            X = x;
            Y = y;
        }

        public ISO_Classes.Coordinate ToDegreeCoordinate()
        {
            return Grid25GridCell.UTMCoordinateToLongLatCoordinate(this);
        }
    }
}
