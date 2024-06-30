using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Mapping
{

    public class globalMapping
    {
        private static string _appPath = "";
        public static fad3MappingMode MappingMode { get; set; }

        static globalMapping()
        {
            _appPath = System.Windows.Forms.Application.StartupPath;
            GeoProjection = new GeoProjection();
            GeoProjection.SetWgs84();

        }
        public static string ApplicationPath
        {
            get { return _appPath; }
        }
        public static string BingAPIKey { get; set; }
        public static GeoProjection GeoProjection { get; set; }
        public static double HoursOffsetGMT { get; set; }
        public static string SaveFolderForGrids { get; set; }

        public static int? GridSize { get; set; }
        public static string CoastlineIDFieldName{get;set;}
        public static int CoastlineIDFieldIndex { get; set; }
    }

}
