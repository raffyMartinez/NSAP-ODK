using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MapWinGIS;
namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for MapColorsViewerWindow.xaml
    /// </summary>
    public partial class MapColorsViewerWindow : Window
    {
        public MapColorsViewerWindow()
        {
            InitializeComponent();
            Loaded += MapColorsViewerWindow_Loaded;
        }


        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        private void MapColorsViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var utils = new MapWinGIS.Utils();
            foreach (var c in Enum.GetValues(typeof(tkMapColor)))
            {
                WrapPanel wp = new WrapPanel();

                System.Windows.Controls.Label lb = new System.Windows.Controls.Label();
                lb.Content = c.ToString();
                lb.Margin = new Thickness(2);
                lb.Width = 200;
                wp.Children.Add(lb);


                System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                im.Height = 20;
                im.Width = 20;

                Bitmap newImage = new Bitmap((int)im.Height, (int)im.Width);
                using (Graphics g = Graphics.FromImage(newImage))
                {

                    g.Clear(Colors.UintToColor(utils.ColorByName((tkMapColor)c)));
                }

                im.Source = BitmapToImageSource(newImage);
                im.Margin = new Thickness(2);
                wp.Children.Add(im);

                sp.Children.Add(wp);
            }




        }
    }
}
