using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Mapping
{
    public static class SaveMapParameters
    {
        public static int? DPI { get; set; }
        public static string FileName { get; set; }

        public static double? LogoScaleFactoer { get; set; }

        public static bool LayoutMap { get; set; }


        public static string MapTitle { get; set; }


        public static void SetParameters(int dpi, string filename)
        {
            DPI = dpi;
            FileName = filename;

            Utilities.Global.Settings.SuggestedDPI = dpi;
            Utilities.Global.Settings.FolderToSaveMapImages = System.IO.Path.GetDirectoryName(filename);
            Utilities.Global.SaveGlobalSettings();

        }
        public static void SetAllParameters(int dpi, string filename, double scaleFactor, bool layoutMap)
        {
            DPI = dpi;
            FileName = filename;
            LogoScaleFactoer = scaleFactor;
            LayoutMap = layoutMap;
        }
    }
}
