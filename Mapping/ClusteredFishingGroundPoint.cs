using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISO_Classes;

namespace NSAP_ODK.Mapping
{
    public class ClusteredFishingGroundPoint
    {
        public string GridName { get; set; }
        public Coordinate Coordinate { get; set; }
        public int FrequencyOfFishing { get; set; }
        public double WeightOfCatch { get; set; }

        //public System.Windows.Media.Imaging.BitmapImage CategoryImageInLegend { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
