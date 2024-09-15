using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace NSAP_ODK.Mapping
{
    public class MapLayout
    {
        private string _mapBitmapFile;
        private string _logoFile;
        private Image _logos;

        public MapLayout()
        {
            LogoScaleFactor = .5d;
            TitleDistanceFactor = 1d;
        }

        public void Cleanup()
        {

        }

        public double LogoScaleFactor { get; set; }
        public Bitmap MapBitmap { get; set; }

        public Bitmap LogosBitmap { get; set; }

        public int Spacing { get; set; }

        public float ScreenResolutionDPI { get; set; }

        public string MapBitmapFile
        {
            get { return _mapBitmapFile; }
            set
            {
                _mapBitmapFile = value;
                MapBitmap = new Bitmap(_mapBitmapFile);
                if (LogosBitmap == null)
                {
                    LogosBitmap = new Bitmap($@"{AppDomain.CurrentDomain.BaseDirectory}\usaid-bfar_300.png");
                    _logos = ScaleImage(LogosBitmap, (int)(LogosBitmap.Width * LogoScaleFactor), (int)(LogosBitmap.Height * LogoScaleFactor));
                }
            }
        }

        public Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        public string LogoFile
        {
            get { return _logoFile; }
            set
            {
                _logoFile = value;
                LogosBitmap = new Bitmap(_logoFile);
            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public double TitleDistanceFactor { get; set; }

        public string LayoutMapFile { get; private set; }
        public void LayoutMap()
        {

            if (MapBitmap == null || LogosBitmap == null)
            {
                return;
            }
            else
            {
                using (Bitmap layout = new Bitmap(MapBitmap.Width, MapBitmap.Height + LogosBitmap.Height))
                {

                    using (Graphics g = Graphics.FromImage(layout))
                    {
                        g.Clear(Color.White);

                        g.DrawImage(MapBitmap, new Rectangle(0, 0, MapBitmap.Width, MapBitmap.Height));

                        g.DrawImage(_logos, new Rectangle((MapBitmap.Width - _logos.Width) / 2, (int)(MapBitmap.Height * TitleDistanceFactor), _logos.Width, _logos.Height));



                        var fn = Path.GetFileNameWithoutExtension(_mapBitmapFile) + "_layout.jpg";
                        LayoutMapFile = Path.GetDirectoryName(_mapBitmapFile) + "\\" + fn;

                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);

                        var myEncoderParameter = new EncoderParameter(myEncoder, 75L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        layout.Save(LayoutMapFile, jpgEncoder, myEncoderParameters);

                    }
                }
                MapBitmap.Dispose();
                MapBitmap = null;
                LogosBitmap.Dispose();
                LogosBitmap = null;
            }
        }
    }
}
