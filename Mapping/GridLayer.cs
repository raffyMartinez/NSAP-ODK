using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;
using AxMapWinGIS;
using System.Windows.Threading;

namespace NSAP_ODK.Mapping
{

    public class GridLayer : IDisposable
    {
        public static event EventHandler<CreateProxyImageEventArgs> CreatingProxyImageEvent;

        public event EventHandler<MapCursorLocationChangedEventArgs> MapCursorLocationChangedEvent;
        private double _gridMean;
        private double _gridMin;
        private double _gridMax;


        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 1000),
            IsEnabled = false
        };

        public static Image ProxyImage { get; set; }
        public static Task<Image> CreateProxyImageAsync(Grid grid)
        {
            return Task.Run(() => CreateProxyImage(grid));
        }


        public static Image CreateProxyImage(Grid grid)
        {
            CreatingProxyImageEvent?.Invoke(null, new CreateProxyImageEventArgs { Intent = "proxy image creating" });
            Image proxyImage = new Image();
            Utils utils = new Utils();

            GridColorScheme gcs = new GridColorScheme();
            GridColorBreak gcb = new GridColorBreak();
            gcb.ColoringType = ColoringType.Gradient;


            gcb.HighColor = utils.ColorByName(tkMapColor.White);
            gcb.HighValue = 5000;
            gcb.LowColor = utils.ColorByName(tkMapColor.White);
            gcb.LowValue = 10;
            gcs.InsertBreak(gcb);


            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.White);
            gcb.HighValue = 10;
            gcb.LowColor = utils.ColorByName(tkMapColor.Beige);
            gcb.LowValue = 0;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Beige);
            gcb.HighValue = 0;
            gcb.LowColor = utils.ColorByName(tkMapColor.Bisque);
            gcb.LowValue = -10;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Bisque);
            gcb.HighValue = -10;
            gcb.LowColor = utils.ColorByName(tkMapColor.PaleGreen);
            gcb.LowValue = -20;
            gcs.InsertBreak(gcb);


            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.PaleGreen);
            gcb.HighValue = -20;
            gcb.LowColor = utils.ColorByName(tkMapColor.DarkSeaGreen);
            gcb.LowValue = -50;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.DarkSeaGreen);
            gcb.HighValue = -50;
            gcb.LowColor = utils.ColorByName(tkMapColor.LightSkyBlue);
            gcb.LowValue = -75;
            gcs.InsertBreak(gcb);


            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.LightSkyBlue);
            gcb.HighValue = -75;
            gcb.LowColor = utils.ColorByName(tkMapColor.DodgerBlue);
            gcb.LowValue = -100;
            gcs.InsertBreak(gcb);


            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.DodgerBlue);
            gcb.HighValue = -100;
            gcb.LowColor = utils.ColorByName(tkMapColor.Blue);
            gcb.LowValue = -250;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Blue);
            gcb.HighValue = -250;
            gcb.LowColor = utils.ColorByName(tkMapColor.MediumPurple);
            gcb.LowValue = -500;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.MediumPurple);
            gcb.HighValue = -500;
            gcb.LowColor = utils.ColorByName(tkMapColor.BlueViolet);
            gcb.LowValue = -1000;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.BlueViolet);
            gcb.HighValue = -1000;
            gcb.LowColor = utils.ColorByName(tkMapColor.DarkViolet);
            gcb.LowValue = -2000;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.DarkViolet);
            gcb.HighValue = -2000;
            gcb.LowColor = utils.ColorByName(tkMapColor.Orange);
            gcb.LowValue = -3000;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Orange);
            gcb.HighValue = -3000;
            gcb.LowColor = utils.ColorByName(tkMapColor.Peru);
            gcb.LowValue = -4000;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Peru);
            gcb.HighValue = -4000;
            gcb.LowColor = utils.ColorByName(tkMapColor.Chocolate);
            gcb.LowValue = -6000;
            gcs.InsertBreak(gcb);


            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Chocolate);
            gcb.HighValue = -6000;
            gcb.LowColor = utils.ColorByName(tkMapColor.Red);
            gcb.LowValue = -8000;
            gcs.InsertBreak(gcb);

            gcb = new GridColorBreak();
            gcb.HighColor = utils.ColorByName(tkMapColor.Red);
            gcb.HighValue = -8000;
            gcb.LowColor = utils.ColorByName(tkMapColor.DarkRed);
            gcb.LowValue = -10000;
            gcs.InsertBreak(gcb);



            gcs.ApplyColoringType(ColoringType.Gradient);
            proxyImage = grid.CreateImageProxy(gcs);
            proxyImage.AllowHillshade = false;

            CreatingProxyImageEvent?.Invoke(null, new CreateProxyImageEventArgs { Intent = "proxy image created" });
            ProxyImage = proxyImage;
            return proxyImage;
        }

        public MapInterActionHandler MapInterActionHandler { get; set; }
        public GridLayer(Grid grid, AxMap map, MapLayersHandler mapLayers, int h, MapInterActionHandler interactionHandler)
        {


            LayerHandle = h;
            MapLayers = mapLayers;
            Grid = grid;
            MapControl = map;
            MapInterActionHandler = interactionHandler;
            MapInterActionHandler.MapControl.MouseMoveEvent += MapControl_MouseMoveEvent;
            
            //GridColorScheme gcs = grid.GenerateColorScheme(tkGridSchemeGeneration.gsgGradient, PredefinedColorScheme.FallLeaves);
            //gcs.ApplyColoringType(ColoringType.Gradient);
            ////scheme.SetColors4(PredefinedColorScheme.FallLeaves);
            ////gcs.ApplyColors(tkColorSchemeType.ctSchemeGraduated, scheme, true);
            //ProxyImage = Grid.CreateImageProxy(gcs);
            //ProxyImage.AllowHillshade = false;


            timer.Tick += timer_Tick;
            MapControl.ExtentsChanged += OnMapExtentsChanged;
            GetGridStatisticsInMapExtents();
        }

        private void MapControl_MouseMoveEvent(object sender, _DMapEvents_MouseMoveEvent e)
        {

            MapCursorLocationChangedEvent?.Invoke(null, new MapCursorLocationChangedEventArgs { Longitude = MapInterActionHandler.Longitude, Latitude = MapInterActionHandler.Latitude });
        }

        public double GetGridValue()
        {
            int col = 0;
            int row = 0;
            Grid.ProjToCell(MapInterActionHandler.Longitude, MapInterActionHandler.Latitude, out col, out row);
            return (double)Grid.Value[col, row];
        }
        public double GridMean { get { return _gridMean; } }
        public double GridMin { get { return _gridMin; } }
        public double GridMax { get { return _gridMax; } }
        private void GetGridStatisticsInMapExtents()
        {

            _gridMean = 0;
            _gridMin = 0;
            _gridMax = 0;

            new MapWinGIS.Utils().GridStatisticsForPolygon(Grid, Grid.Header, Grid.Extents, MapControl.Extents.ToShape(), Grid.Header.NodataValue, ref _gridMean, ref _gridMin, ref _gridMax);

            Console.WriteLine($"mean:{_gridMean}  minimum:{_gridMin} maximum:{_gridMax}");
        }
        void timer_Tick(object sender, EventArgs e)
        {
            timer.IsEnabled = false;
            if (MapLayers.LayerIsVisible(LayerHandle) && GetMinMaxValuesWithinMapExtent)
            {
                GetGridStatisticsInMapExtents();
            }
        }

        public int LayerHandle { get; set; }

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool GetMinMaxValuesWithinMapExtent { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                MapLayers = null;
                Grid = null;
                ProxyImage = null;

                MapControl.ExtentsChanged -= OnMapExtentsChanged;
                MapControl = null;

                timer.Tick -= timer_Tick;
                timer = null;
                _disposed = true;

            }
        }
        private void OnMapExtentsChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Start();

        }

        public MapLayersHandler MapLayers { get; set; }

        public Grid Grid { get; set; }
        public AxMap MapControl { get; set; }
    }
}
