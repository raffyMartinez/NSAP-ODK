using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Mapping
{
    public class MapCursorLocationChangedEventArgs:EventArgs
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
