using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace NSAP_ODK.Mapping
{
    public enum CoordinateDisplayFormat
    {
        DegreeDecimal,
        DegreeMinute,
        DegreeMinuteSecond,
        UTM
    }
    public enum LengthDirection
    {
        Vertical, // |
        Horizontal // ——
    }
    public static class globalMapping
    {
        private static CoordinateDisplayFormat _coordDisplayFormat = CoordinateDisplayFormat.DegreeDecimal;
        private static List<string> _tempFiles = new List<string>();
        private static string _appPath = "";
        public static fad3MappingMode MappingMode { get; set; }

        public static CoordinateDisplayFormat CoordinateDisplay
        {
            get { return _coordDisplayFormat; }
            set
            {
                _coordDisplayFormat = value;
                SaveCoordinateDisplayFormat();
            }
        }

        private static void SaveCoordinateDisplayFormat()
        {
            Utilities.Global.Settings.CoordinateDisplayFormat = _coordDisplayFormat;
            Utilities.Global.SaveGlobalSettings();
        }
        public static void Cleanup()
        {
            foreach (var item in _tempFiles)
            {
                if (File.Exists(item))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
        }

        public static float SuggestedDPI { get; set; }
        public static void ListTemporaryFile(string fileName)
        {
            if (!_tempFiles.Contains(fileName))
            {
                _tempFiles.Add(fileName);
            }
        }
        static globalMapping()
        {
            SuggestedDPI = 0;
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
        public static string CoastlineIDFieldName { get; set; }
        public static int CoastlineIDFieldIndex { get; set; }

        public static double PointsToPixels(double wpfPoints, LengthDirection direction)
        {
            if (direction == LengthDirection.Horizontal)
            {
                return wpfPoints * Screen.PrimaryScreen.WorkingArea.Width / SystemParameters.WorkArea.Width;
            }
            else
            {
                return wpfPoints * Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height;
            }
        }

        public static double PixelsToPoints(int pixels, LengthDirection direction)
        {
            if (direction == LengthDirection.Horizontal)
            {
                return pixels * SystemParameters.WorkArea.Width / Screen.PrimaryScreen.WorkingArea.Width;
            }
            else
            {
                return pixels * SystemParameters.WorkArea.Height / Screen.PrimaryScreen.WorkingArea.Height;
            }
        }
        public static System.Drawing.Image ConvertPicture(stdole.IPictureDisp image)
        {
            int type = image.Type;
            if (type == 1)
            {
                IntPtr hPal = (IntPtr)image.hPal;
                return System.Drawing.Image.FromHbitmap((IntPtr)image.Handle, hPal);
            }
            return null;
        }

        public static uint GetRandomColorUint()
        {
            return ColorToUInt(GetRandomColor());
        }
        public static Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromArgb(255, Color.FromArgb(Convert.ToInt32(random.Next(0x1000000))));
        }
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) |
                          (color.G << 8) | (color.B << 0));
        }

    }

}
