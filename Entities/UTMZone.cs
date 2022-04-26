using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class UTMZone
    {
        public ushort MajorGridSizeMeters { get; set; }
        public ushort MinorGridCellSizeMeters { get; set; }
        public int MajorGridXOrigin { get; private set; }
        public int MajorGridYOrigin { get; private set; }

        public int ColumnWidth { get; private set; }

        public byte MajorGridColumns { get; private set; }

        public byte MajorGridNumberingStart { get; private set; }
        public ushort MaxGridNumber { get; private set; }
        public byte ZoneNumber { get; private set; }
        public char ZoneNS { get; set; }


        private void ProcessZoneProperties()
        {
            string utmZone = $"{ZoneNumber}{ZoneNS}";
            IsValid = true;
            switch (utmZone)
            {
                case "50N":
                    MajorGridNumberingStart = 1;
                    //MaxGridNumber = 315;
                    MaxGridNumber = 600;
                    MajorGridXOrigin = 300000;
                    MajorGridYOrigin = 400000;
                    MajorGridColumns = 15;
                    ColumnWidth = 15;
                    break;
                case "51N":
                    MajorGridNumberingStart = 11;
                    ColumnWidth = 20;
                    MaxGridNumber = 1230;
                    MajorGridXOrigin = -500000;
                    MajorGridYOrigin = 350000;
                    MajorGridColumns = 30;
                    break;
                default:
                    IsValid = false;
                    throw new Exception("Expected zone numbers can only be 50 or 51 and zone letter can only be N");

            }
            if(IsValid)
            {
                MajorGridSizeMeters = 50000;
                MinorGridCellSizeMeters = 2000;
            }
        }

        public UTMZone(byte zoneNumber, char zoneNS)
        {
            ZoneNumber = zoneNumber;
            ZoneNS = zoneNS;
            ProcessZoneProperties();

        }



        public bool IsValid { get; private set; }
        public string SerializeToString()
        {
            return $"{ZoneNumber}{ZoneNS}";
        }

        public override string ToString()
        {
            return $"{ZoneNumber}{ZoneNS}";
        }

        public UTMZone(string utmZone, bool processProperties = true)
        {
            ZoneNumber = byte.Parse(utmZone.Substring(0, utmZone.Length - 1));
            ZoneNS = utmZone.ToCharArray()[utmZone.Length-1];

            if(processProperties)
              ProcessZoneProperties();
        }
    }
}
