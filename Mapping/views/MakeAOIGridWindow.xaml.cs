﻿using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.IO;


namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for MakeAOIGridWindow.xaml
    /// </summary>
    public partial class MakeAOIGridWindow : Window
    {
        private static MakeAOIGridWindow _instance;
        public MakeAOIGridWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
            Closed += MakeAOIGridWindow_Closed;

        }

        private void MakeAOIGridWindow_Closed(object sender, EventArgs e)
        {
            _instance = null;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_instance = null;
            Owner.Focus();
        }

        public AOI AOI { get; set; }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (globalMapping.GridSize != null)
            {
                cboGridSize.Text = globalMapping.GridSize.ToString();
            }
            else
            {
                cboGridSize.Text = "200";
            }
            labelTitle.Content = $"Generate grid for {AOI.Name}";
        }

        public static MakeAOIGridWindow GetInstance()
        {
            if (_instance == null) _instance = new MakeAOIGridWindow();
            return _instance;
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    SaveFileDialog sfd = null;
                    bool proceed = true;
                    AOI.GridSizeMeters = int.Parse(cboGridSize.Text);
                    if ((bool)checkSaveGrid.IsChecked)
                    {
                        proceed = false;
                        sfd = new SaveFileDialog();
                        sfd.DefaultExt = "*.shp";
                        sfd.Filter = "Shapefiles (*.shp)|*.shp|All files (*.*)|*.*";
                        sfd.FileName = $"{AOI.GridLayerName}";
                        if ( !string.IsNullOrEmpty( globalMapping.SaveFolderForGrids))
                        {
                            sfd.InitialDirectory = globalMapping.SaveFolderForGrids;
                        }
                        else
                        {
                            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        }
                        if ((bool)sfd.ShowDialog() && sfd.FileName.Length > 0 && Directory.Exists(System.IO.Path.GetDirectoryName(sfd.FileName)))
                        {
                            proceed = true;

                        }
                    }

                    if (proceed)
                    {
                        Grid25.UTMZone = UTMZone.UTMZone51N;
                        if (MapWindowManager.Grid25MajorGrid == null ||
                            MapWindowManager.Grid25MajorGrid.GeoProjection.ProjectionName != "WGS 84 / UTM zone 51N")
                        {
                            Grid25.UTMZone = UTMZone.UTMZone51N;
                            MapWindowManager.Grid25MajorGrid = Grid25.CreateGrid25MajorGrid();

                        }

                        var grids = AOI.MajorGridIntersect();
                        if (grids == null)
                        {
                            int minZoneNumber = Grid25.ZonesFromConversion.Min(t => t.ZoneNumber);
                            if (minZoneNumber == 50)
                            {
                                Grid25.UTMZone = UTMZone.UTMZone50N;
                                MapWindowManager.Grid25MajorGrid = Grid25.CreateGrid25MajorGrid();
                            }
                            grids = AOI.MajorGridIntersect(minZoneNumber);
                            if (AOI.ErrorMsg.Length > 0)
                            {
                                MessageBox.Show(AOI.ErrorMsg, "GPX Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }

                        AOI.GenerateMinorGrids();
                        if (!AOI.GeneratedSubGrids(int.Parse(cboGridSize.Text)))
                        {
                            MessageBox.Show("Subgrid size does not fit grid", "GPX Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {

                            AOI.GridSizeMeters = int.Parse(cboGridSize.Text);

                            int h = MapWindowManager.MapLayersHandler.AddLayer(AOI.SubGrids, $"{AOI.GridLayerName}");
                            if (h >= 0)
                            {
                                AOI.GridHandle = h;
                                var sf = (MapWinGIS.Shapefile)MapWindowManager.MapLayersHandler[h].LayerObject;
                                sf.Key = $"subgrid_{AOI.Name}";
                                sf.DefaultDrawingOptions.FillVisible = false;
                                sf.DefaultDrawingOptions.LineColor = new MapWinGIS.Utils().ColorByName(MapWinGIS.tkMapColor.DarkGray);

                                if (sfd != null)
                                {
                                    if (!string.IsNullOrEmpty( globalMapping.SaveFolderForGrids) )
                                    {
                                        globalMapping.SaveFolderForGrids = System.IO.Path.GetDirectoryName(sfd.FileName);
                                        //Global.SaveGlobalSettings();
                                    }

                                    Callback cb = new Callback();
                                    if (sf.SaveAs(sfd.FileName, cb))
                                    {
                                        AOI.GridFileName = sfd.FileName;
                                        Entities.AOIViewModel.UpdateRecordInRepo(AOI);
                                    }
                                    else
                                    {
                                        MessageBox.Show(cb.ErrorMessage, "GPX Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }
                                ((AOIWindow)Owner).GridIsLoaded();
                                Owner.Focus();
                                Close();
                            }
                        }
                    }
                    break;
                case "buttonCancel":
                    Owner.Focus();
                    Close();
                    break;
                case "buttonGridSaveLocation":
                    break;
            }
        }
    }
}
