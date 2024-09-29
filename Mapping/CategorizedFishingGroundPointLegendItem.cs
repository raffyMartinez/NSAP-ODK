using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;
namespace NSAP_ODK.Mapping
{
    public class CategorizedFishingGroundPointLegendItem
    {
        public ShapefileCategory ShapefileCategory { get; set; }
        public string Range { get; set; }
        public System.Windows.Media.Imaging.BitmapImage ImageInLegend { get; set; }
    }
}
