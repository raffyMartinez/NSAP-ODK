using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;
using System.Windows.Media.Imaging;
using System.Drawing;
namespace NSAP_ODK.Mapping
{
    public class CategorizedFishingPointShapefile
    {
        private Shapefile _categorizedSF;
        public int NumberOfBreaks { get; set; }
        private List<ClusteredFishingGroundPoint> _clusteredFishingGroundPoints = new List<ClusteredFishingGroundPoint>();

        private void PopulateLocationDictionary(Shapefile inputFishingGridPoints)
        {
            int gridNameColumnIndex = inputFishingGridPoints.Table.FieldIndexByName["GridPt"];
            int weightColumnIndex = inputFishingGridPoints.FieldIndexByName["WtCatch"];
            for (int x = 0; x < inputFishingGridPoints.NumShapes; x++)
            {
                string grid_name = ((string)inputFishingGridPoints.CellValue[gridNameColumnIndex, x]).ToLower();
                double weight_catch = (double)inputFishingGridPoints.CellValue[weightColumnIndex, x];
                ClusteredFishingGroundPoint cfgp = new ClusteredFishingGroundPoint();
                //cfgp = _clusteredFishingGroundPoints.Find(t => t.GridName == grid_name);
                try
                {
                    cfgp = _clusteredFishingGroundPoints.Find(t => t.GridName.ToLower() == grid_name);
                }
                catch
                {
                    //ignore
                }
                if (cfgp == null)
                {
                    var pt = inputFishingGridPoints.Shape[x].Point[0];
                    ClusteredFishingGroundPoint fgp = new ClusteredFishingGroundPoint
                    {
                        GridName = grid_name,
                        Coordinate = new ISO_Classes.Coordinate((float)pt.y, (float)pt.x),
                        FrequencyOfFishing = 1,
                        WeightOfCatch = weight_catch
                    };

                    _clusteredFishingGroundPoints.Add(fgp);
                }
                else
                {
                    cfgp.FrequencyOfFishing++;
                    cfgp.WeightOfCatch += weight_catch;
                }
            }
        }

        public List<CategorizedFishingGroundPointLegendItem> CategorizedFishingGroundPointLegendItems { get; private set; }
        public int PointSizeOfMaxCategory { get; set; }

        public BitmapImage CategorySymbol(ShapefileCategory cat)
        {
            //int w, h = (int)cat.DrawingOptions.PointSize;
            int w, h = PointSizeOfMaxCategory;
            w = h;

            Bitmap bmp = new Bitmap(1, 1);//, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.SetPixel(0, 0, System.Drawing.Color.White);
            bmp = new Bitmap(bmp, w, h);
            Graphics g = Graphics.FromImage(bmp);
            IntPtr ptr = g.GetHdc();

            cat.DrawingOptions.DrawPoint(hDC: (int)ptr,
                                        x: 0,
                                        y: 0,
                                        clipWidth: 0,
                                        clipHeight: 0);

            g.ReleaseHdc(ptr);
            return globalMapping.BitmapToBitmapImage(bmp);
        }

        public void GetCategoriesFromClusteredFishingGroundPoints()
        {
            CategorizedFishingGroundPointLegendItem legendItem;
            CategorizedFishingGroundPointLegendItems = new List<CategorizedFishingGroundPointLegendItem>();
            for (int x = 0; x < ClusteredFishingPointShapefile.Categories.Count-1; x++)
            {
                var category = ClusteredFishingPointShapefile.Categories.Item[x];
                legendItem = new CategorizedFishingGroundPointLegendItem
                {
                    ShapefileCategory=category,
                    ImageInLegend = CategorySymbol(category),
                    Range=$"{category.MinValue} - {category.MaxValue}"
                };
                CategorizedFishingGroundPointLegendItems.Add(legendItem);
            }
        }
        public void CategorizeNumericPointLayer(Shapefile sf, int classificationField = 1,
                                               tkClassificationType Method = tkClassificationType.ctNaturalBreaks)
        {
            CategorizedFishingGroundPointLegendItem legendItem;
            CategorizedFishingGroundPointLegendItems = new List<CategorizedFishingGroundPointLegendItem>();
            float ptSize = 0;
            if (sf.Categories.Generate(classificationField, Method, NumberOfBreaks))
            {
                for (int n = 0; n < sf.Categories.Count; n++)
                {
                    var category = sf.Categories.Item[n];
                    ptSize = PointSizeOfMaxCategory * ((float)(n + 1) / sf.Categories.Count);
                    category.DrawingOptions.PointSize = ptSize;
                    category.DrawingOptions.PointShape = tkPointShapeType.ptShapeCircle;
                    category.DrawingOptions.LineColor = new Utils().ColorByName(tkMapColor.White);

                    legendItem = new CategorizedFishingGroundPointLegendItem
                    {
                        ShapefileCategory = category,
                        ImageInLegend = CategorySymbol(category),
                        Range = $"{category.MinValue} - {category.MaxValue}"
                    };
                    CategorizedFishingGroundPointLegendItems.Add(legendItem);
                }
            }
            var cat0 = sf.Categories.Add("zero");
            Field f = sf.Field[classificationField];
            cat0.Expression = $"[{f.Name}]=0";
            cat0.DrawingOptions.PointShape = tkPointShapeType.ptShapeCircle;
            cat0.DrawingOptions.PointSize = 0;
            cat0.DrawingOptions.FillVisible = false;
            cat0.DrawingOptions.LineVisible = false;

            sf.Key = $"{sf.Key}|{PointSizeOfMaxCategory}";
            sf.Categories.ApplyExpression(sf.Categories.CategoryIndex[cat0]);
        }

        public Shapefile FishingGroundPointShapefile { get; private set; }
        public List<Double> Breaks { get; private set; }

        public List<ShapefileCategory> CategorizeField(string categorizeOption, tkClassificationType classificationType = tkClassificationType.ctNaturalBreaks)
        {
            return null;
        }
        public void CategorizeField(string categorizeOption)
        {
            List<double> source = new List<double>();
            switch (categorizeOption)
            {
                case "By frequency of fishing operations":
                    foreach (var item in _clusteredFishingGroundPoints)
                    {
                        source.Add(item.FrequencyOfFishing);
                    }
                    break;
                case "By weight of catch":
                    foreach (var item in _clusteredFishingGroundPoints)
                    {
                        source.Add(item.WeightOfCatch);
                    }
                    break;
            }
            Breaks = JenksFisher.CreateJenksFisherBreaksArray(source, NumberOfBreaks);
        }
        public Shapefile ClusteredFishingPointShapefile { get; set; }
        public CategorizedFishingPointShapefile()
        {

        }
        public CategorizedFishingPointShapefile(Shapefile inputFishingPointsFromGrid)
        {
            _categorizedSF = new Shapefile();
            PopulateLocationDictionary(inputFishingPointsFromGrid);

            if (_categorizedSF.CreateNewWithShapeID("", ShpfileType.SHP_POINT))
            {
                _categorizedSF.Key = "classified fishing ground points";
                _categorizedSF.GeoProjection = globalMapping.GeoProjection;

                int fldIndex = _categorizedSF.EditAddField("long", FieldType.DOUBLE_FIELD, 9, 12);
                _categorizedSF.Field[fldIndex].Alias = "Longitude";

                fldIndex = _categorizedSF.EditAddField("lat", FieldType.DOUBLE_FIELD, 9, 12);
                _categorizedSF.Field[fldIndex].Alias = "Latitude";

                fldIndex = _categorizedSF.EditAddField("grid", FieldType.STRING_FIELD, 1, 1);
                _categorizedSF.Field[fldIndex].Alias = "Grid location";

                fldIndex = _categorizedSF.EditAddField("freq", FieldType.INTEGER_FIELD, 1, 1);
                _categorizedSF.Field[fldIndex].Alias = "Frequency";

                fldIndex = _categorizedSF.EditAddField("weight", FieldType.DOUBLE_FIELD, 2, 12);
                _categorizedSF.Field[fldIndex].Alias = "Weight of catch";


                foreach (var item in _clusteredFishingGroundPoints)
                {
                    var shp = new Shape();
                    if (shp.Create(ShpfileType.SHP_POINT))
                    {
                        if (shp.AddPoint((double)item.Coordinate.Longitude, (double)item.Coordinate.Latitude) >= 0)
                        {
                            var shpIndex = _categorizedSF.EditAddShape(shp);
                            if (shpIndex >= 0)
                            {
                                _categorizedSF.EditCellValue(_categorizedSF.FieldIndexByName["long"], shpIndex, item.Coordinate.Longitude);
                                _categorizedSF.EditCellValue(_categorizedSF.FieldIndexByName["lat"], shpIndex, item.Coordinate.Latitude);
                                _categorizedSF.EditCellValue(_categorizedSF.FieldIndexByName["grid"], shpIndex, item.GridName);
                                _categorizedSF.EditCellValue(_categorizedSF.FieldIndexByName["freq"], shpIndex, item.FrequencyOfFishing);
                                _categorizedSF.EditCellValue(_categorizedSF.FieldIndexByName["weight"], shpIndex, item.WeightOfCatch);
                            }
                        }
                    }
                }
            }
            _categorizedSF.DefaultDrawingOptions.PointShape = tkPointShapeType.ptShapeCircle;
            _categorizedSF.DefaultDrawingOptions.PointSize = 14;
            _categorizedSF.CollisionMode = tkCollisionMode.AllowCollisions;
            FishingGroundPointShapefile = _categorizedSF;
        }
    }
}
