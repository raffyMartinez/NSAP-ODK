using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxMapWinGIS;
using MapWinGIS;

namespace NSAP_ODK.Mapping
{
    public class Graticule : IDisposable
    {
        private MapLayersHandler _mapLayersHandler;
        private AxMap _axMap;
        private Shapefile _sfGraticule;
        private int _ifldPart;
        private int _ifldLabel;
        private int _ifldSide;
        private GeoProjection _geoProjection;
        private Extents _graticuleExtents;
        private Extents _boundaryExtents;
        private Extents _mapExtents;
        private double _xOrigin;
        private double _yOrigin;
        private Double _gridHeight;
        private double _gridWidth;
        private double _intervalSize;
        private const double _EXTENTFACTOR = 0.93;
        public MapTextGraticuleHelper GraticuleTextHelper { get; internal set; }

        public string MapTitle { get; set; }
        public bool BottomHasLabel { get; set; }
        public bool TopHasLabel { get; set; }
        public bool RightHasLabel { get; set; }
        public bool LeftHasLabel { get; set; }
        public string Name { get; set; }
        public int LabelFontSize { get; set; }
        public int NumberOfGridlines { get; set; }
        public int BorderWidth { get; set; }
        public int GridlinesWidth { get; set; }
        public bool BoldLabels { get; set; }
        public bool GridVisible { get; set; }
        public double TopLeft { get; internal set; }
        public double TopRight { get; internal set; }
        public double BottomLeft { get; internal set; }
        public double BottomRight { get; internal set; }
        public uint BorderColor { get; set; }

        public event EventHandler GraticuleExtentChanged;
        public event EventHandler MapRedrawNeeded;
        public static EventHandler GraticuleLoaded;
        public static EventHandler GraticuleUnloaded;
        //public EventHandler GraticuleBeforeLoading;

        public Extents GraticuleExtents
        {
            get { return _graticuleExtents; }
        }

        public GeoProjection GeoProjection
        {
            get { return _geoProjection; }
        }

        private bool _disposed;

        public string CoordFormat { get; set; }
        public CoordinateDisplayFormat CoordinateDisplayFormat { get; set; }
        public Shapefile GraticuleShapeFile
        {
            get { return _sfGraticule; }
        }

        /// <summary>
        /// Receives the input parameters needed to build the graticule
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sizeLabelFont"></param>
        /// <param name="numberGridlines"></param>
        /// <param name="widthBorder"></param>
        /// <param name="widthGridlines"></param>
        /// <param name="gridVisible"></param>
        /// <param name="boldLabels"></param>
        /// <param name="leftHasLabel"></param>
        /// <param name="rightHasLabel"></param>
        /// <param name="topHasLabel"></param>
        /// <param name="bottomHasLabel"></param>
        public void Configure(string name, int sizeLabelFont, int numberGridlines, int widthBorder, int widthGridlines,
                              bool gridVisible, bool boldLabels, bool leftHasLabel, bool rightHasLabel,
                              bool topHasLabel, bool bottomHasLabel, CoordinateDisplayFormat coordFormat)
        {
            Name = name;
            LabelFontSize = sizeLabelFont;
            NumberOfGridlines = numberGridlines;
            BorderWidth = widthBorder;
            GridlinesWidth = widthGridlines;
            BoldLabels = boldLabels;
            GridVisible = gridVisible;
            BottomHasLabel = bottomHasLabel;
            TopHasLabel = topHasLabel;
            LeftHasLabel = leftHasLabel;
            RightHasLabel = rightHasLabel;
            CoordinateDisplayFormat = coordFormat;

            Utilities.Global.Settings.CoordinateDisplayFormat = CoordinateDisplayFormat;
            globalMapping.CoordinateDisplay = CoordinateDisplayFormat;
            SetCoordinateDisplayFormat();
            Utilities.Global.SaveGlobalSettings();
        }

        public void Dispose()
        {
            GraticuleUnloaded?.Invoke(this, EventArgs.Empty);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
                _axMap.ExtentsChanged -= OnMapExtentsChanged;
                _axMap.LayerRemoved -= OnLayerRemoved;
                _axMap.LayerAdded -= OnLayerAdded;
                _axMap = null;

                if (_sfGraticule != null)
                {
                    _sfGraticule.EditClear();
                }
                _sfGraticule = null;

                _geoProjection = null;
                _graticuleExtents = null;
                _boundaryExtents = null;
                _mapExtents = null;
                _mapLayersHandler.OnLayerVisibilityChanged -= OnLayerVisibleChange;
                _mapLayersHandler.MapRedrawNeeded -= OnRedrawNeeded;
                _mapLayersHandler.RemoveLayer("Mask");
                _mapLayersHandler.RemoveLayer(Name);
                _mapLayersHandler = null;
            }
        }

        /// <summary>
        /// Sets up the polyline shapefile that makes the graticule
        /// </summary>
        private void SetupShapefile()
        {
            _sfGraticule = new Shapefile();
            if (_sfGraticule.CreateNew("", ShpfileType.SHP_POLYLINE))
            {
                _ifldPart = _sfGraticule.EditAddField("Part", FieldType.STRING_FIELD, 1, 25);
                _ifldLabel = _sfGraticule.EditAddField("Label", FieldType.STRING_FIELD, 1, 25);
                _ifldSide = _sfGraticule.EditAddField("Side", FieldType.STRING_FIELD, 1, 1);
                _sfGraticule.GeoProjection = _geoProjection;
                GraticuleLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Creates a shapefile mask that hides features outside the extent of the graticule
        /// </summary>
        /// <returns></returns>
        public Shapefile Mask()
        {
            var sf = new Shapefile();
            if (sf.CreateNew("", ShpfileType.SHP_POLYGON))
            {
                var shp = new Shape();
                if (shp.Create(ShpfileType.SHP_POLYGON))
                {
                    //ef is expansion factor
                    var ef = (_mapExtents.xMax - _mapExtents.xMin) * 0.01;
                    //var ef = (_mapExtents.xMax - _mapExtents.xMin) * 0.5;

                    _mapExtents.SetBounds(_mapExtents.xMin - ef, _mapExtents.yMin - ef, 0, _mapExtents.xMax + ef, _mapExtents.yMax + (ef), 0);

                    shp = _mapExtents.ToShape().Clip(_boundaryExtents.ToShape(), tkClipOperation.clDifference);
                    var iShp = sf.EditAddShape(shp);
                    sf.DefaultDrawingOptions.LineVisible = false;
                    sf.DefaultDrawingOptions.FillColor = new Utils().ColorByName(tkMapColor.White);
                    return sf;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="layersHandler"></param>
        public Graticule(AxMap mapControl, MapLayersHandler layersHandler)
        {
            Name = "Graticule";
            LabelFontSize = 8;
            NumberOfGridlines = 5;
            _axMap = mapControl;
            _axMap.ExtentsChanged += OnMapExtentsChanged;
            _axMap.LayerRemoved += OnLayerRemoved;
            _axMap.LayerAdded += OnLayerAdded;
            _geoProjection = _axMap.GeoProjection;
            _mapLayersHandler = layersHandler;
            _mapLayersHandler.OnLayerVisibilityChanged += OnLayerVisibleChange;
            _mapLayersHandler.MapRedrawNeeded += OnRedrawNeeded;
            GraticuleTextHelper = new MapTextGraticuleHelper(_geoProjection, this);
            BorderColor = new Utils().ColorByName(tkMapColor.Black);
            SetCoordinateDisplayFormat();
        }


        private void SetCoordinateDisplayFormat()
        {
            switch (globalMapping.CoordinateDisplay)
            {
                case CoordinateDisplayFormat.DegreeDecimal:
                    CoordFormat = "D";
                    break;

                case CoordinateDisplayFormat.DegreeMinute:
                    CoordFormat = "DM";
                    break;

                case CoordinateDisplayFormat.DegreeMinuteSecond:
                    CoordFormat = "DMS";
                    break;

                default:
                    throw new Exception("Graticule: Invalid coordinate format");
            }
        }

        private void OnRedrawNeeded(object sender, EventArgs e)
        {
            if (_mapLayersHandler.Exists(Name))
            {
                MapRedrawNeeded?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnLayerVisibleChange(MapLayersHandler s, LayerEventArg e)
        {
            if (_mapLayersHandler.Exists(Name))
            {
                MapRedrawNeeded?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnLayerAdded(object sender, _DMapEvents_LayerAddedEvent e)
        {
            if (_mapLayersHandler.Exists(Name))
            {
                MapRedrawNeeded?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnLayerRemoved(object sender, _DMapEvents_LayerRemovedEvent e)
        {
            if (_mapLayersHandler.Exists(Name))
            {
                MapRedrawNeeded?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnMapExtentsChanged(object sender, EventArgs e)
        {
            if (_mapLayersHandler.Exists(Name))
            {
                Refresh();
                GraticuleExtentChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Rebuilds the graticule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Refresh()
        {
            _sfGraticule?.EditClear();
            _sfGraticule?.Labels.Clear();
            ShowGraticule();
        }

        /// <summary>
        /// Calls ComputeGraticule to builds the graticule, adds a mask and then add the shapefile to the layer handler class
        /// </summary>
        public void ShowGraticule()
        {
            //GraticuleBeforeLoading?.Invoke(this, EventArgs.Empty);
            _mapLayersHandler.RemoveLayer(Name);
            _mapLayersHandler.RemoveLayer("Mask");
            SetupShapefile();
            ComputeGraticule(_axMap.Extents.xMax, _axMap.Extents.yMax, _axMap.Extents.xMin, _axMap.Extents.yMin);
            _sfGraticule.Labels.VerticalPosition = tkVerticalPosition.vpAboveAllLayers;
            var maskHandle = _mapLayersHandler.AddLayer(Mask(), "Mask", true, false);
            _mapLayersHandler[maskHandle].IsMaskLayer = true;
            _mapLayersHandler[maskHandle].IsGraticule = true;
            var graticuleHandle = _mapLayersHandler.AddLayer(_sfGraticule, Name, true, true);
            _mapLayersHandler[graticuleHandle].IsGraticule = true;
            _mapLayersHandler.RefreshLayers();
        }

        /// <summary>
        /// Functionality to build the graticule.Input parameters are actually from the  extent of a map control
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <param name="xMin"></param>
        /// <param name="yMin"></param>
        private void ComputeGraticule(double xMax, double yMax, double xMin, double yMin)
        {
            double titleX = 0;
            double titleY = 0;
            var coordinateString = "";
            var coord = new ISO_Classes.Coordinate();
            var tempW = 0D;
            var tempH = 0D;

            _mapExtents = new Extents();
            _mapExtents.SetBounds(xMin, yMin, 0, xMax, yMax, 0);

            _graticuleExtents = new Extents();

            //0.93 means that the graticule is 93% of the map extent
            //tempW = (xMax - xMin) * 0.93;
            //tempH = (yMax - yMin) * 0.93;

            tempW = (xMax - xMin) * _EXTENTFACTOR;
            tempH = (yMax - yMin) * _EXTENTFACTOR;

            //compute the origin of the graticule
            _xOrigin = (xMin + ((xMax - xMin) / 2)) - (tempW / 2);
            _yOrigin = (yMin + ((yMax - yMin) / 2)) - (tempH / 2);

            tempH = (yMax - yMin) * 0.87;

            var ticLength = Math.Abs(xMin - _xOrigin) / 4;

            _graticuleExtents.SetBounds(_xOrigin, _yOrigin, 0, _xOrigin + tempW, _yOrigin + tempH, 0);

            //boundary extent is used in calculating the mask. The mask hides parts of the map control between the edge of the map control and the boundary
            _boundaryExtents = new Extents();
            _boundaryExtents.SetBounds(_xOrigin, _yOrigin, 0, _xOrigin + tempW, _yOrigin + tempH, 0);

            var graticuleShape = _graticuleExtents.ToShape();

            //build the Border of the graticule
            Point pt1, pt2;
            Shape shp;
            for (int n = 0; n < 4; n++)
            {
                pt1 = new Point();
                pt2 = new Point();
                pt1.x = graticuleShape.Point[n].x;
                pt1.y = graticuleShape.Point[n].y;
                pt2.x = graticuleShape.Point[n + 1].x;
                pt2.y = graticuleShape.Point[n + 1].y;
                shp = new Shape();
                if (shp.Create(ShpfileType.SHP_POLYLINE))
                {
                    shp.AddPoint(pt1.x, pt1.y);
                    shp.AddPoint(pt2.x, pt2.y);
                    var iShp = _sfGraticule.EditAddShape(shp);
                    _sfGraticule.EditCellValue(_ifldPart, iShp, "Border");
                    if (n == 1)
                    {
                        //_sfGraticule.EditCellValue(_ifldLabel, iShp, MapTitle);
                        titleX = pt1.x;
                        titleY = pt1.y;
                    }
                }
            }

            xMax = graticuleShape.Extents.xMax;
            yMax = graticuleShape.Extents.yMax;
            xMin = graticuleShape.Extents.xMin;
            yMin = graticuleShape.Extents.yMin;
            _gridHeight = yMax - yMin;
            _gridWidth = xMax - xMin;

            var sideLength = _gridHeight;
            if (_gridWidth > _gridHeight) sideLength = _gridWidth;

            var factor = 60;
            _intervalSize = (sideLength / (NumberOfGridlines - 1)) * factor;
            var roundTo = (int)(_intervalSize / 5) * 5;

            if (roundTo == 0)
            {
                factor = 60 * 60;
                _intervalSize = (sideLength / (NumberOfGridlines - 1)) * factor;
                roundTo = (int)(_intervalSize / 5) * 5;
            }

            roundTo = SetInterval((int)roundTo);

            //setup label categories
            _sfGraticule.Labels.AddCategory("Top");
            _sfGraticule.Labels.AddCategory("Bottom");
            _sfGraticule.Labels.AddCategory("Right");
            _sfGraticule.Labels.AddCategory("Left");
            _sfGraticule.Labels.AddCategory("Title");

            //setup display options of labels
            for (int n = 0; n < _sfGraticule.Labels.NumCategories; n++)
            {
                _sfGraticule.Labels.Category[n].FontBold = BoldLabels;
                _sfGraticule.Labels.Category[n].FontSize = LabelFontSize;
                _sfGraticule.Labels.Category[n].FrameVisible = false;
                _sfGraticule.Labels.Category[n].LineOrientation = tkLineLabelOrientation.lorPerpindicular;
                _sfGraticule.Labels.AvoidCollisions = false;
                switch (_sfGraticule.Labels.Category[n].Name)
                {
                    case "Top":
                        _sfGraticule.Labels.Category[n].Alignment = tkLabelAlignment.laTopCenter;
                        break;

                    case "Bottom":
                        _sfGraticule.Labels.Category[n].Alignment = tkLabelAlignment.laBottomCenter;
                        break;

                    case "Left":
                    case "Right":
                        _sfGraticule.Labels.Category[n].Alignment = tkLabelAlignment.laCenter;
                        break;

                    case "Title":
                        _sfGraticule.Labels.AddLabel(MapTitle, titleX, titleY + ((_axMap.Extents.yMax - titleY) / 2), Category: n);
                        _sfGraticule.Labels.Category[n].Alignment = tkLabelAlignment.laTopRight;
                        _sfGraticule.Labels.Category[n].FontSize = 16;
                        _sfGraticule.Labels.Category[n].FontBold = true;
                        break;
                }
            }

            //set interval size which determines spacing of gridlines
            _intervalSize = (int)(_intervalSize / roundTo) * roundTo;

            if (_intervalSize >= 1)
            {
                _intervalSize /= factor;
                double baseX = ((int)(_xOrigin - (int)_xOrigin) * factor);
                if (baseX / roundTo != (baseX / 10))
                {
                    baseX = ((baseX / roundTo) + 1) * roundTo;
                }
                baseX = (int)_xOrigin + (baseX / factor);

                _sfGraticule.Labels.AddCategory("Top");
                while (baseX < xMax)
                {
                    if (baseX > xMin)
                    {
                        shp = new Shape();
                        if (shp.Create(ShpfileType.SHP_POLYLINE))
                        {
                            //Build the vertical gridlines
                            shp.AddPoint(baseX, yMin);
                            shp.AddPoint(baseX, yMax);
                            var iShp = _sfGraticule.EditAddShape(shp);
                            _sfGraticule.EditCellValue(_ifldPart, iShp, "Gridline");

                            //set tics on the bottom side
                            if (BottomHasLabel)
                            {
                                shp = new Shape();
                                if (shp.Create(ShpfileType.SHP_POLYLINE))
                                {
                                    shp.AddPoint(baseX, yMin);
                                    shp.AddPoint(baseX, yMin - ticLength);
                                    iShp = _sfGraticule.EditAddShape(shp);
                                    _sfGraticule.EditCellValue(_ifldPart, iShp, "tic");

                                    //we input the tic's location to the coordinate class
                                    coord.SetD((float)shp.Center.y, (float)shp.Center.x);

                                    //coordinate string is the formatted coordinate of the tic label
                                    coordinateString = coord.ToString(false, CoordFormat);

                                    _sfGraticule.EditCellValue(_ifldLabel, iShp, coordinateString);
                                    _sfGraticule.EditCellValue(_ifldSide, iShp, "B");
                                    _sfGraticule.Labels.AddLabel(coordinateString, baseX, yMin - ticLength, 0, 1);
                                }
                            }

                            //set tics on the top side
                            if (TopHasLabel)
                            {
                                shp = new Shape();
                                if (shp.Create(ShpfileType.SHP_POLYLINE))
                                {
                                    shp.AddPoint(baseX, yMax);
                                    shp.AddPoint(baseX, yMax + ticLength);
                                    iShp = _sfGraticule.EditAddShape(shp);
                                    _sfGraticule.EditCellValue(_ifldPart, iShp, "tic");
                                    coord.SetD((float)shp.Center.y, (float)shp.Center.x);
                                    coordinateString = coord.ToString(false, CoordFormat);
                                    _sfGraticule.EditCellValue(_ifldLabel, iShp, coordinateString);
                                    _sfGraticule.EditCellValue(_ifldSide, iShp, "T");
                                    _sfGraticule.Labels.AddLabel(coordinateString, baseX, yMax + ticLength, 0, 0);
                                }
                            }
                        }
                    }
                    baseX += _intervalSize;
                }

                double baseY = (int)((_yOrigin - (int)_yOrigin) * factor);
                if (baseY / roundTo != (int)(baseY / roundTo))
                {
                    baseY = ((int)(baseY / roundTo) + 1) * roundTo;
                }
                baseY = (int)_yOrigin + (baseY / factor);

                while (baseY < yMax)
                {
                    if (baseY > yMin)
                    {
                        shp = new Shape();
                        if (shp.Create(ShpfileType.SHP_POLYLINE))
                        {
                            //build the horizontal gridlines
                            shp.AddPoint(xMin, baseY);
                            shp.AddPoint(xMax, baseY);
                            var iShp = _sfGraticule.EditAddShape(shp);
                            _sfGraticule.EditCellValue(_ifldPart, iShp, "GridLine");
                        }

                        //set tics on the left side
                        if (LeftHasLabel)
                        {
                            shp = new Shape();
                            if (shp.Create(ShpfileType.SHP_POLYLINE))
                            {
                                shp.AddPoint(xMin, baseY);
                                shp.AddPoint(xMin - ticLength, baseY);
                                var iShp = _sfGraticule.EditAddShape(shp);
                                _sfGraticule.EditCellValue(_ifldPart, iShp, "tic");
                                coord.SetD((float)shp.Center.y, (float)shp.Center.x);
                                coordinateString = coord.ToString(isYcoord: true, format: CoordFormat);
                                _sfGraticule.EditCellValue(_ifldLabel, iShp, coordinateString);
                                _sfGraticule.EditCellValue(_ifldSide, iShp, "L");

                                //we add tic label and rotate the label 270 degrees
                                _sfGraticule.Labels.AddLabel(coordinateString, xMin - (ticLength * 2), baseY, 270, 3);
                            }
                        }

                        //set tics on the right side
                        if (RightHasLabel)
                        {
                            shp = new Shape();
                            if (shp.Create(ShpfileType.SHP_POLYLINE))
                            {
                                shp.AddPoint(xMax, baseY);
                                shp.AddPoint(xMax + ticLength, baseY);
                                var iShp = _sfGraticule.EditAddShape(shp);
                                _sfGraticule.EditCellValue(_ifldPart, iShp, "tic");
                                coord.SetD((float)shp.Center.y, (float)shp.Center.x);
                                coordinateString = coord.ToString(isYcoord: true, format: CoordFormat);
                                _sfGraticule.EditCellValue(_ifldLabel, iShp, coordinateString);
                                _sfGraticule.EditCellValue(_ifldSide, iShp, "R");

                                //we add tic label and rotate the label 90 degrees
                                _sfGraticule.Labels.AddLabel(coordinateString, xMax + (ticLength * 2), baseY, 90, 2);
                            }
                        }
                    }

                    baseY += _intervalSize;
                }

                //sets up appearance of the various lines that make up the graticule
                if (_sfGraticule.Categories.Generate(_ifldPart, tkClassificationType.ctUniqueValues, 1))
                {
                    for (int n = 0; n < _sfGraticule.Categories.Count; n++)
                    {
                        var category = _sfGraticule.Categories.Item[n];
                        switch (category.Expression)
                        {
                            case @"[Part] = ""Border""":
                            case @"[Part] = ""tic""":
                                category.DrawingOptions.LineWidth = BorderWidth;
                                category.DrawingOptions.LineColor = BorderColor;
                                break;

                            default:
                                category.DrawingOptions.VerticesColor = new Utils().ColorByName(tkMapColor.Gray);
                                category.DrawingOptions.LineWidth = GridlinesWidth;
                                category.DrawingOptions.LineVisible = GridVisible;
                                break;
                        }
                    }

                    _sfGraticule.Categories.ApplyExpressions();
                }
            }
            else
            {
                throw new Exception("Graticule: Interval size is invalid");
            }
        }

        /// <summary>
        /// Sets up the interval size of tics and returns interval size in minutes
        /// </summary>
        /// <param name="inValue"></param>
        /// <returns></returns>
        private int SetInterval(int inValue)
        {
            if (inValue < 2)
            {
                return 1;
            }
            else if (inValue < 5)
            {
                return 2;
            }
            else if (inValue < 10)
            {
                return 5;
            }
            else if (inValue < 15)
            {
                return 10;
            }
            else if (inValue < 20)
            {
                return 15;
            }
            else if (inValue < 30)
            {
                return 20;
            }
            else if (inValue < 60)
            {
                return 30;
            }
            else if (inValue < 90)
            {
                return 60;
            }
            else if (inValue < 120)
            {
                return 90;
            }
            else if (inValue < 150)
            {
                return 120;
            }
            else if (inValue < 180)
            {
                return 150;
            }
            else if (inValue < 240)
            {
                return 180;
            }
            else if (inValue >= 240)
            {
                return 240;
            }

            return 0;
        }
    }
}
