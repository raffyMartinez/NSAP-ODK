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
using NSAP_ODK.Mapping;
using System.Windows.Threading;
using NSAP_ODK.Utilities;
using NSAP_ODK.Mapping.views;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using MapWinGIS;
using System.IO;
using Ookii.Dialogs.Wpf;

namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for MapLayersWindow.xaml
    /// </summary>
    public partial class MapLayersWindow : Window
    {
        private double? _gridRowHeight;
        private MapLayer _currentLayer;
        private bool _isDragDropDone;
        private bool _gridIsClicked;
        private string _selectedFishingGroundLocationCategory;
        private string _pointShapefileKey;
        public MapLayersHandler MapLayersHandler { get; set; }

        private static MapLayersWindow _instance;
        public MapLayersWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            Closed += OnWindowClosed;
            dataGridLayers.ContextMenuOpening += DataGridLayers_ContextMenuOpening;
        }

        private void DataGridLayers_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_currentLayer.LayerObject != null)
            {
                if (_currentLayer.LayerType == "ShapefileClass" && ((Shapefile)_currentLayer.LayerObject).ShapefileType == ShpfileType.SHP_POINT)
                {
                    Shapefile pointShapefile = (Shapefile)_currentLayer.LayerObject;
                    menuConvexHull.Visibility = Visibility.Visible;
                    menuCategorize.Visibility = Visibility.Visible;
                    _pointShapefileKey = pointShapefile.Key;
                }

                bool sameLayers = true;
                if (dataGridLayers.SelectedItems.Count > 1)
                {
                    string firstItemKey = ((MapLayer)dataGridLayers.SelectedItems[0]).LayerKey.ToString();
                    for (int x = 1; x <= dataGridLayers.SelectedItems.Count; x++)
                    {
                        if (firstItemKey != ((MapLayer)dataGridLayers.SelectedItems[x - 1]).LayerKey.ToString())
                        {
                            sameLayers = false;
                            break;
                        }
                    }
                    if (sameLayers)
                    {
                        menuMerge.Visibility = Visibility.Visible;
                    }
                }
                if (_currentLayer.LayerKey == "bathymetry")
                {
                    menuGetDepth.Visibility = Visibility.Visible;
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        public void RefreshCurrentLayer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }


        public MapWindowForm ParentForm { get; set; }

        private void Cleanup()
        {
            if (MapWindowManager.MapLayersHandler != null)
            {
                MapWindowManager.MapLayersViewModel.LayerRead -= MapLayersViewModel_LayerRead;
                MapWindowManager.MapLayersViewModel.LayerRemoved -= MapLayersViewModel_LayerRemoved;
                MapWindowManager.MapLayersViewModel.CurrentLayer -= MapLayersViewModel_CurrentLayer;
            }

            dataGridLayers.SelectionChanged -= DataGridLayers_SelectionChanged;
            dataGridLayers.DataContextChanged -= DataGridLayers_DataContextChanged;
            dataGridLayers.PreviewDrop -= DataGridLayers_PreviewDrop;
            dataGridLayers.PreviewMouseDown -= DataGridLayers_PreviewMouseDown;
            dataGridLayers.LayoutUpdated -= DataGridLayers_LayoutUpdated;

            ParentForm.Closing -= ParentForm_Closing;

            MapLayersHandler.OnLayerVisibilityChanged -= MapLayersHandler_OnLayerVisibilityChanged;

            MapWindowManager.MapLayersWindow = null;
            this.SavePlacement();


        }
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cleanup();
        }

        public static MapLayersWindow GetInstance()
        {
            if (_instance == null) _instance = new MapLayersWindow();
            return _instance;
        }


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            MapWindowManager.MapLayersViewModel.LayerRead += MapLayersViewModel_LayerRead;
            MapWindowManager.MapLayersViewModel.LayerRemoved += MapLayersViewModel_LayerRemoved;
            MapWindowManager.MapLayersViewModel.CurrentLayer += MapLayersViewModel_CurrentLayer;

            dataGridLayers.SelectionChanged += DataGridLayers_SelectionChanged;
            dataGridLayers.DataContextChanged += DataGridLayers_DataContextChanged;
            dataGridLayers.PreviewDrop += DataGridLayers_PreviewDrop;
            dataGridLayers.PreviewMouseDown += DataGridLayers_PreviewMouseDown;
            dataGridLayers.MouseUp += DataGridLayers_MouseUp;
            dataGridLayers.LayoutUpdated += DataGridLayers_LayoutUpdated;
            ParentForm.Closing += ParentForm_Closing;


            MapLayersHandler.OnLayerVisibilityChanged += MapLayersHandler_OnLayerVisibilityChanged;

            MapWindowManager.MapLayersWindow = this;

            ConfigureDataGrid();

            List<MapLayer> dummyList = new List<MapLayer>();
            dummyList.Add(new MapLayer { Visible = true, Name = "dummy" });

            dataGridLayers.DataContext = dummyList;
            dataGridLayers.Items.Refresh();

            //RefreshLayerGrid();
            _gridIsClicked = false;

            //SelectCurrentLayerInGrid();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            _instance = null;
            ParentForm.Focus();
        }

        private void DataGridLayers_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("DataGridLayers_MouseUp");
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                return;
            }
            else if (dep is DataGridColumnHeader)
            {
                DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
                string boundPropertyName = FindBoundProperty(columnHeader.Column);

                int columnIndex = columnHeader.Column.DisplayIndex;

                //Title = string.Format(
                //    "Header clicked [{0}] = {1}",
                //    columnIndex, boundPropertyName);
            }
            else if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;

                // navigate further up the tree
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                {
                    return;
                }

                DataGridRow row = dep as DataGridRow;

                object value = ExtractBoundValue(row, cell);

                int columnIndex = cell.Column.DisplayIndex;
                int rowIndex = FindRowIndex(row);

                //we clicked on visibility checkbox if column is zero
                if (columnIndex == 0 && value != null)
                {
                    //Console.WriteLine("visibility check cliked");
                    MapLayersHandler.EditLayer(CurrentLayer.Handle, CurrentLayer.Name, !(bool)value);


                    RefreshCurrentLayer();
                    dataGridLayers.Focus();
                }
            }

        }

        void timer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            SelectCurrentLayerInGrid();

        }
        private int FindRowIndex(DataGridRow row)
        {
            DataGrid dataGrid = ItemsControl.ItemsControlFromItemContainer(row) as DataGrid;

            int index = dataGrid.ItemContainerGenerator.IndexFromContainer(row);

            return index;
        }
        private object ExtractBoundValue(DataGridRow row, DataGridCell cell)
        {
            // find the property that this cell's column is bound to
            string boundPropertyName = FindBoundProperty(cell.Column);
            if (boundPropertyName != null)
            {
                // find the object that is realted to this row
                object data = row.Item;

                // extract the property value
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(data);
                PropertyDescriptor property = properties[boundPropertyName];
                object value = property.GetValue(data);

                return value;
            }
            return null;
        }

        private string FindBoundProperty(DataGridColumn col)
        {
            DataGridBoundColumn boundColumn = col as DataGridBoundColumn;

            if (boundColumn != null)
            {
                // find the property that this column is bound to
                Binding binding = boundColumn.Binding as Binding;
                string boundPropertyName = binding.Path.Path;


                return boundPropertyName;
            }
            return null;


        }

        private void ConfigureDataGrid()
        {


            dataGridLayers.Columns.Add(new DataGridCheckBoxColumn { Header = "Visible", Binding = new Binding("Visible") });


            string xaml = "<Style xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" TargetType=\"TextBlock\"><Setter Property=\"TextWrapping\" Value=\"Wrap\"/></Style>";
            Style style = System.Windows.Markup.XamlReader.Parse(xaml) as Style;
            dataGridLayers.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name"), ElementStyle = style, Width = 400 });





            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(System.Windows.Controls.Image));
            //Binding bind = new Binding("image");//please keep "image" name as you have set in your class data member name
            Binding bind = new Binding("LayerImageInLegend");//please keep "image" name as you have set in your class data member name
            factory.SetValue(System.Windows.Controls.Image.SourceProperty, bind);
            DataTemplate cellTemplate = new DataTemplate() { VisualTree = factory };
            DataGridTemplateColumn imgCol = new DataGridTemplateColumn()
            {
                Header = "Legend", //this is upto you whatever you want to keep, this will be shown on column to represent the data for helping the user...
                CellTemplate = cellTemplate
            };
            imgCol.Width = new DataGridLength(50);
            dataGridLayers.Columns.Add(imgCol);

            dataGridLayers.AutoGenerateColumns = false;
        }
        private void MapLayersViewModel_CurrentLayer(MapLayersViewModel s, LayerEventArg e)
        {
            CurrentLayer = MapLayersHandler.CurrentMapLayer;
            //SelectCurrentLayerInGrid();
        }

        private void MapLayersViewModel_LayerRemoved(MapLayersViewModel s, LayerEventArg e)
        {
            RefreshLayerGrid(s);
        }

        private void MapLayersViewModel_LayerRead(MapLayersViewModel s, LayerEventArg e)
        {
            RefreshLayerGrid(s);

        }

        private void DataGridLayers_LayoutUpdated(object sender, EventArgs e)
        {
            if (_isDragDropDone)
            {
                List<MapLayerSequence> layersSequence = new List<MapLayerSequence>();
                List<MapLayer> layers = new List<MapLayer>();
                int sequence = dataGridLayers.Items.Count - 1;
                foreach (MapLayer ly in dataGridLayers.Items)
                {
                    layersSequence.Add(new MapLayerSequence { MapLayer = ly, Sequence = sequence });
                    layers.Add(ly);
                    sequence--;
                }
                MapLayersHandler.LayersSequence(layersSequence);
                dataGridLayers.DataContext = layers;
                _isDragDropDone = false;



            }
            if (_gridRowHeight == null && dataGridLayers.Items.Count >= 1)
            {

                var r = dataGridLayers.ItemContainerGenerator.ContainerFromItem(dataGridLayers.Items[0]) as DataGridRow;

                _gridRowHeight = r.ActualHeight;
                MapLayersHandler.LegendSymbolHeight = globalMapping.PointsToPixels(r.ActualHeight, LengthDirection.Vertical);
                //MapLayersHandler.LegendSymbolWidth = globalMapping.PointsToPixels(r.ActualHeight*8, LengthDirection.Horizontal);
                MapLayersHandler.LegendSymbolWidth = globalMapping.PointsToPixels(dataGridLayers.Columns[2].Width.Value, LengthDirection.Horizontal);

                if (((MapLayer)dataGridLayers.Items[0]).Name == "dummy")
                {
                    RefreshLayerGrid();
                    //Title = $"row height:{((double)_gridRowHeight).ToString()} ";
                }
            }

        }

        private void DataGridLayers_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _gridIsClicked = true;


        }

        private void DataGridLayers_PreviewDrop(object sender, DragEventArgs e)
        {
            _isDragDropDone = true;

        }

        private void ParentForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cleanup();
        }

        public void UpdateGridLayout()
        {
            dataGridLayers.UpdateLayout();
            SelectCurrentLayerInGrid();
        }
        private void DataGridLayers_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => SelectCurrentLayerInGrid()));

        }

        public MapLayer CurrentLayer
        {
            get { return _currentLayer; }
            private set
            {
                _currentLayer = value;
            }
        }

        public void DisableGrid(bool disabled = true)
        {
            if (disabled)
            {
                dataGridLayers.Visibility = Visibility.Hidden;
            }
            else
            {
                dataGridLayers.Visibility = Visibility.Visible;
            }
        }

        public void SelectCurrentLayerInGrid(bool inMouseUp = false)
        {
            if (dataGridLayers.Items.Count > 0)
            {
                _gridIsClicked = false;
                if (CurrentLayer == null)
                {
                    CurrentLayer = MapLayersHandler.CurrentMapLayer;
                }

                foreach (var item in dataGridLayers.Items)
                {
                    if (CurrentLayer != null && ((MapLayer)item).Handle == CurrentLayer.Handle)
                    {
                        if (!inMouseUp)
                        {
                            dataGridLayers.SelectedItem = item;
                        }
                        DataGridRow r = (DataGridRow)dataGridLayers.ItemContainerGenerator.ContainerFromIndex(dataGridLayers.SelectedIndex);
                        if (r != null)
                        {
                            r.FontWeight = FontWeights.Bold;
                        }
                        break;
                    }
                }
            }

        }


        private void SetRowsNormalFont()
        {
            for (int n = 0; n < dataGridLayers.Items.Count; n++)
            {
                DataGridRow r = (DataGridRow)dataGridLayers.ItemContainerGenerator.ContainerFromIndex(n);
                if (r != null)
                {
                    r.FontWeight = FontWeights.Normal;
                }
            }
        }
        private void DataGridLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _pointShapefileKey = "";
            //Console.WriteLine("DataGridLayers_SelectionChanged");
            menuConvexHull.Visibility = Visibility.Collapsed;
            menuCategorize.Visibility = Visibility.Collapsed;
            menuMerge.Visibility = Visibility.Collapsed;
            menuGetDepth.Visibility = Visibility.Collapsed;
            if (_gridIsClicked && dataGridLayers.SelectedItems.Count > 0)
            {
                MapLayer ml = (MapLayer)dataGridLayers.SelectedItem;
                //if(ml.LayerType=="ShapefileClass" && ((Shapefile)ml.LayerObject).ShapefileType==ShpfileType.SHP_POINT)
                //{
                //    menuConvexHull.Visibility = Visibility.Visible;
                //}
                MapLayersHandler.set_MapLayer(ml.Handle);
                SetRowsNormalFont();
                SelectCurrentLayerInGrid();

                if (((MapWindowForm)Owner).IsGetDepthMode)
                {
                    ((MapWindowForm)Owner).DepthModeEnabled(isEnabled: ml.LayerKey == "bathymetry");
                }
            }

        }


        private void MapLayersHandler_OnLayerVisibilityChanged(MapLayersHandler s, LayerEventArg e)
        {
            RefreshLayerGrid();
        }

        private void RefreshLayerGrid(MapLayersViewModel mlvm)
        {
            foreach (var ly in mlvm.MapLayerCollection.Where(t => t.LayerImageInLegend == null))
            {
                ly.LayerImageInLegend = MapLayersHandler.LayerSymbol(ly);
            }

            dataGridLayers.DataContext = mlvm.MapLayerCollection;
            try
            {
                dataGridLayers.Items.Refresh();
            }
            catch (Exception)
            {
                //ignore;
            }

        }
        private void RefreshLayerGrid()
        {
            //dataGridLayers.DataContext = MapWindowManager.MapLayersViewModel.MapLayerCollection.Where(T => T.VisibleInLayersUI == true).ToList(); ;
            //dataGridLayers.DataContext = MapWindowManager.MapLayersViewModel.MapLayerCollection;
            dataGridLayers.DataContext = MapWindowManager.MapLayersViewModel.GetLayerUIVisibleLayers().ToList();
            dataGridLayers.Items.Refresh();
            //dataGridLayers.ItemsSource = MapWindowManager.MapLayersViewModel.GetLayerUIVisibleLayers().ToList();

        }


        private void OnToolbarButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonAttributes":
                    if (MapLayersHandler.CurrentMapLayer != null)
                    {
                        ShapeFileAttributesWindow sfw = ShapeFileAttributesWindow.GetInstance(MapWindowManager.MapInterActionHandler);
                        if (sfw.Visibility == Visibility.Visible && sfw.Focusable)
                        {
                            sfw.BringIntoView();
                            sfw.Focus();
                        }
                        else
                        {
                            sfw.Show();
                            sfw.Owner = this;
                            sfw.ShapeFile = MapLayersHandler.CurrentMapLayer.LayerObject as Shapefile;
                            sfw.ShowShapeFileAttribute();
                        }
                        MapWindowManager.ShapeFileAttributesWindow = sfw;
                    }
                    else
                    {
                        MessageBox.Show("Select a layer",
                            Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    break;
                case "buttonRemove":
                    if (MapLayersHandler.CurrentMapLayer != null)
                    {
                        MapLayersHandler.RemoveLayer(MapLayersHandler.CurrentMapLayer.Handle);
                    }
                    break;

                case "buttonAdd":
                    break;
            }
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            var mnu = (MenuItem)sender;
            switch (mnu.Header)
            {
                case "Categorize":
                    CategorizeFishingGroundLayerWindow cfglw = new CategorizeFishingGroundLayerWindow();
                    cfglw.Owner = this;
                    var layerKeys = _pointShapefileKey.Split('|');
                    cfglw.ShowCategoryLegendOnly = layerKeys[0] == "classified fishing ground points";
                    if (layerKeys.Length > 1 && int.TryParse(layerKeys[1], out int point_size))
                    {
                        cfglw.MaxPointSizeOfCategorizedFishingGroundPoints = point_size;
                    }
                    cfglw.Shapefile = (Shapefile)CurrentLayer.LayerObject;
                    _selectedFishingGroundLocationCategory = null;



                    if (!cfglw.ShowCategoryLegendOnly)
                    {
                        if ((bool)cfglw.ShowDialog())
                        {
                            _selectedFishingGroundLocationCategory = cfglw.SelectedCategory;
                            MapLayersHandler.AddLayer(cfglw.CategorizedShapefile.FishingGroundPointShapefile, "Categorized");
                        }
                    }
                    else
                    {
                        cfglw.Owner = this;
                        cfglw.Show();
                    }

                    break;
                case "Set visibility...":
                    ShapeFileVisibilityExpressionWindow vew = new ShapeFileVisibilityExpressionWindow();
                    vew.Owner = this;
                    vew.ExpressionTarget = VisibilityExpressionTarget.ExpressionTargetShape;
                    vew.Shapefile = (Shapefile)MapWindowManager.MapLayersHandler.CurrentMapLayer.LayerObject;
                    vew.Show();
                    break;
                case "Export":
                    VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
                    vfbd.Description = "Select folder to save exported shapefiles";
                    vfbd.UseDescriptionForTitle = true;
                    vfbd.ShowDialog();
                    if (vfbd.SelectedPath.Length > 0 && Directory.Exists(vfbd.SelectedPath))
                    {
                        string fileName = $@"{vfbd.SelectedPath}\{MapLayersHandler.CurrentMapLayer.Name}.shp";
                        if (((Shapefile)MapLayersHandler.CurrentMapLayer.LayerObject).SaveAs(fileName))
                        {
                            MessageBox.Show($"{System.IO.Path.GetFileName(fileName)} was successfully exported", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case "Convex hull":
                    if (_currentLayer.LayerType == "ShapefileClass" && ((Shapefile)_currentLayer.LayerObject).ShapefileType == ShpfileType.SHP_POINT)
                    {
                        switch (((Shapefile)_currentLayer.LayerObject).Key)
                        {
                            case "points of fishing ground grid":
                                FishingGroundPointsFromCalendarMappingManager.CreateConvexHullFromFishingGroundPoints(_currentLayer);
                                break;
                        }
                    }

                    break;
                case "menuMerge":
                    break;
                case "Properties":

                    break;
                case "Zoom to layer":
                    MapWindowManager.MapWindowForm.MapControl.ZoomToLayer(MapLayersHandler.CurrentMapLayer.Handle);
                    break;

            }
        }

        private void OnMenuCheckChange(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Name)
            {
                case "menuGetDepth":
                    ((MapWindowForm)Owner).GetDepthMode(isEnabled: ((MenuItem)sender).IsChecked);
                    break;
            }
        }
    }
}