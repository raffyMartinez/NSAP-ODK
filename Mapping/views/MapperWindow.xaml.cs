using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MapWinGIS;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using NSAP_ODK.Mapping;

namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for MapperWindow.xaml
    /// </summary>
    public partial class MapperWindow : Window
    {
        private static  MapperWindow _instance;

        public MapLayersHandler MapLayersHandler { get; set; }
        public static MapperWindow GetInstance()
        {
            if(_instance==null)
            {
                _instance = new MapperWindow();
            }
            return _instance;
        }
        public MainWindow ParentWindow { get; set; }
        public MapperWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += MapperWindow_Closing;
        }

        private void MapperWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
        }

        public AxMapWinGIS.AxMap MapControl { get; set; }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();

            MapControl = new AxMapWinGIS.AxMap();
            host.Child = MapControl;
            gridMap.Children.Add(host);
        }
    }
}
