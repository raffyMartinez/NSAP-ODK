using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;
using AxMapWinGIS;

namespace NSAP_ODK.Mapping
{
    public enum LegendPosition
    {
        lpOtherPosition,
        lpTopRight,
        lpBottomRight,
        lpBottomLeft,
        lpTopLeft,

    }
    public class MapLegendManager:IDisposable
    {
        private MapLayersHandler _mapLayers;
        private AxMap _mapControl;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _mapControl = null;
                _mapLayers = null;
                DrawingRectangle = null;
            }
            // free native resources if there are any.
        }


        public DrawingRectangle DrawingRectangle { get; internal set; }


        public MapLayersHandler MapLayers
        {
            get { return _mapLayers; }
            set
            {
                _mapLayers = value;
                _mapLayers.OnLayerVisibilityChanged += _mapLayers_OnLayerVisibilityChanged;
                _mapLayers.MapRedrawNeeded += _mapLayers_MapRedrawNeeded;
                _mapLayers.LayerRead += _mapLayers_LayerRead;
                _mapLayers.LayerRemoved += _mapLayers_LayerRemoved;
                _mapControl = _mapLayers.MapControl;
            }
        }

        private void _mapLayers_LayerRemoved(MapLayersHandler s, LayerEventArg e)
        {
            
        }

        private void _mapLayers_LayerRead(MapLayersHandler s, LayerEventArg e)
        {
            
        }

        private void _mapLayers_MapRedrawNeeded(object sender, EventArgs e)
        {
            
        }

        private void _mapLayers_OnLayerVisibilityChanged(MapLayersHandler s, LayerEventArg e)
        {
            
        }

        public double LegendWidth { get; set; }
        public double LegendHeight { get; set; }

        public void IsVisible(bool visibility)
        {
            DrawingRectangle.Visible = visibility;
        }

        public void DrawLegend(LegendPosition position, int? otherPosition_x, int? otherPosition_y)
        {
            DrawingRectangle = new DrawingRectangle();
            switch (position)
            {
                case LegendPosition.lpTopLeft:
                    break;
                case LegendPosition.lpTopRight:
                    break;
                case LegendPosition.lpBottomRight:
                    break;
                case LegendPosition.lpBottomLeft:
                    break;
                case LegendPosition.lpOtherPosition:
                    break;
            }

            DrawingRectangle.Visible = true;
            DrawingRectangle.ReferenceType = tkDrawReferenceList.dlScreenReferencedList;
            DrawingRectangle.LineWidth = 2;
            Utils utils = new Utils();
            DrawingRectangle.Color = new Utils().ColorByName(tkMapColor.White);
        }
    }
}
