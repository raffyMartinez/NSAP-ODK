using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapWinGIS;

namespace NSAP_ODK.Mapping
{
    public enum fadMapTextType
    {
        mapTextTypeNone,
        mapTextTypeTitle,
        mapTextTypeNote
    }

    public class MapTextGraticuleHelper
    {
        private Shapefile _textShapefile;
        private int _ifldTextType;
        private int _ifldText;

        private LabelCategory _categoryTitle;
        private LabelCategory _categoryNote;

        private int _iShpTitle = -1;
        private int _ishpNote = -1;

        private Graticule _graticule;

        public MapTextGraticuleHelper(GeoProjection projection, Graticule graticule)
        {
            _graticule = graticule;
            TitleSize = 13;
            NoteSize = 10;
            TextBold = false;
            TitleText = "Map title";
            NoteText = "Map note";
            TitleVisible = false;
            NoteVisible = false;
            TitleAlignment = tkLabelAlignment.laCenter;
            NoteAlignment = tkLabelAlignment.laCenterLeft;
            MapTextType = fadMapTextType.mapTextTypeNone;
            _textShapefile = new Shapefile();
            if (_textShapefile.CreateNewWithShapeID("", ShpfileType.SHP_POINT))
            {
                _textShapefile.GeoProjection = projection;
                _ifldTextType = _textShapefile.EditAddField("TextType", FieldType.STRING_FIELD, 1, 5);
                _ifldText = _textShapefile.EditAddField("Text", FieldType.STRING_FIELD, 1, 100);
                _categoryTitle = _textShapefile.Labels.AddCategory("Text");
                _categoryNote = _textShapefile.Labels.AddCategory("Note");
            }
        }

        public fadMapTextType MapTextType { get; set; }
        public tkLabelAlignment TextAlignment { get; set; }
        public bool TitleVisible { get; internal set; }
        public bool NoteVisible { get; internal set; }
        public bool TextBold { get; set; }
        public int TextSize { get; set; }
        public int TitleSize { get; internal set; }
        public int NoteSize { get; internal set; }
        public tkLabelAlignment TitleAlignment { get; set; }
        public tkLabelAlignment NoteAlignment { get; set; }

        public string TitleText { get; internal set; }
        public string NoteText { get; internal set; }

        public Shapefile TextShapefile
        {
            get { return _textShapefile; }
        }

        public void SetText(string text)
        {
            switch (MapTextType)
            {
                case fadMapTextType.mapTextTypeTitle:
                    TitleText = text;
                    TitleVisible = TitleText.Length > 0;
                    _categoryTitle.FontSize = TextSize;
                    _categoryTitle.FontBold = TextBold;
                    _categoryTitle.Alignment = TextAlignment;

                    if (_textShapefile.NumShapes == 0)
                    {
                        var shape = new Shape();
                        var x = 0D;
                        var y = 0D;
                        if (shape.Create(ShpfileType.SHP_POINT))
                        {
                            switch (_categoryTitle.Alignment)
                            {
                                case tkLabelAlignment.laCenterLeft:
                                    x = _graticule.GraticuleExtents.xMin;
                                    break;

                                case tkLabelAlignment.laCenter:
                                    x = ((_graticule.GraticuleExtents.xMax - _graticule.GraticuleExtents.xMin) / 2) + _graticule.GraticuleExtents.xMin;
                                    break;

                                case tkLabelAlignment.laCenterRight:
                                    x = _graticule.GraticuleExtents.xMax;
                                    break;
                            }

                            y = _graticule.GraticuleExtents.yMax;

                            shape.AddPoint(x, y);

                            _iShpTitle = _textShapefile.EditAddShape(shape);
                            _textShapefile.EditCellValue(_ifldText, _iShpTitle, TitleText);
                        }
                    }
                    else
                    {
                        _textShapefile.EditCellValue(_ifldText, _iShpTitle, TitleText);
                    }
                    break;

                case fadMapTextType.mapTextTypeNote:
                    NoteText = text;
                    NoteVisible = NoteText.Length > 0;
                    _categoryNote.FontSize = TextSize;
                    _categoryNote.Alignment = TextAlignment;
                    _categoryNote.FontBold = TextBold;
                    if (_textShapefile.NumShapes == 1)
                    {
                        var shape = new Shape();
                        if (shape.Create(ShpfileType.SHP_POINT))
                        {
                            _iShpTitle = _textShapefile.EditAddShape(shape);
                            _textShapefile.EditCellValue(_ifldText, _ishpNote, NoteText);
                        }
                    }
                    else
                    {
                        _textShapefile.EditCellValue(_ifldText, _ishpNote, NoteText);
                    }

                    break;
            }

            ShowText();
        }

        private void ShowText()
        {
            if (TitleVisible || NoteVisible)
            {
                //if (MapTextType == fadMapTextType.mapTextTypeTitle)
                //{
                //}
                //else if (MapTextType == fadMapTextType.mapTextTypeNote)
                //{
                //}
                _graticule.Refresh();
            }
        }
    }
}
