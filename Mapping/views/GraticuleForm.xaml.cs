using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for GraticuleForm.xaml
    /// </summary>
    public partial class GraticuleForm : Window
    {
        private static GraticuleForm _instance;
        private MapWindowForm _parent;
        public event EventHandler GraticuleRemoved;
        private string _maptitle;
        public GraticuleForm(MapWindowForm parent)
        {
            InitializeComponent();
            Loaded += OnFormLoad;
            Closing += OnFormClosing;
            _parent = parent;
        }
        public void SetGraticuleTitle(string title)
        {
            txtMapTitle.Text = title;
            ShowGraticule();
            SaveMapParameters.MapTitle = title;
        }
        public Graticule Graticule { get; internal set; }
        public string MapTitle
        {
            get { return _maptitle; }
            set
            {
                _maptitle = value;
                txtMapTitle.Text = _maptitle;
            }
        }
        public static GraticuleForm GetInstance(MapWindowForm parent)
        {
            if (_instance == null)
            {
                _instance = new GraticuleForm(parent);
            }
            return _instance;
        }
        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            //throw new NotImplementedException();
            _instance = null;
            _parent = null;
            //SaveMapParameters.MapTitle = null;
        }

        private void SetAllLabelsPositionToAboveParent()
        {
            foreach (var layer in _parent.MapLayersHandler)
            {
                if (layer.Labels != null)
                {
                    layer.Labels.VerticalPosition = MapWinGIS.tkVerticalPosition.vpAboveParentLayer;
                }
            }
        }
        private void ShowGraticule()
        {
            SetAllLabelsPositionToAboveParent();
            int widthGridlines = int.Parse(txtGridlineWidth.Text);
            int numberGridlines = int.Parse(txtNumberOfGridlines.Text);
            Graticule.Configure(
                name: txtName.Text,
                sizeLabelFont: int.Parse(txtLabelSize.Text),
                numberGridlines: numberGridlines,
                widthBorder: int.Parse(txtBordeWidth.Text),
                widthGridlines: widthGridlines,
                gridVisible: (bool)chkShowGrid.IsChecked,
                boldLabels: (bool)chkBold.IsChecked,
                leftHasLabel: (bool)chkLeft.IsChecked,
                rightHasLabel: (bool)chkRight.IsChecked,
                topHasLabel: (bool)chkTop.IsChecked,
                bottomHasLabel: (bool)chkBottom.IsChecked,
                coordFormat: ((KeyValuePair<CoordinateDisplayFormat, string>)cboCoordFormat.SelectedItem).Key
                );
            Graticule.MapTitle = txtMapTitle.Text;
            Graticule.ShowGraticule();

            Utilities.Global.Settings.NumberOfGridLines = numberGridlines;
            Utilities.Global.Settings.MapGridLinesWidth = widthGridlines;

            SaveMapParameters.MapTitle = Graticule.MapTitle;
            Utilities.Global.SaveGlobalSettings();
        }

        private void OnFormLoad(object sender, RoutedEventArgs e)
        {

            if (SaveMapParameters.MapTitle == null || SaveMapParameters.MapTitle.Length == 0)
            {
                _maptitle = "New untitled map";
            }
            else
            {
                _maptitle = SaveMapParameters.MapTitle;
            }

            txtMapTitle.Text = _maptitle;
            txtName.Text = "Graticule";
            txtLabelSize.Text = "8";
            txtBordeWidth.Text = "2";
            if (Utilities.Global.Settings.MapGridLinesWidth == null)
            {
                txtGridlineWidth.Text = "1";
            }
            else
            {
                txtGridlineWidth.Text = ((int)Utilities.Global.Settings.MapGridLinesWidth).ToString();
            }
            if (Utilities.Global.Settings.NumberOfGridLines == null)
            {
                txtNumberOfGridlines.Text = "5";
            }
            else
            {
                txtNumberOfGridlines.Text = ((int)Utilities.Global.Settings.NumberOfGridLines).ToString();
            }
            


            cboCoordFormat.Items.Add(new KeyValuePair<CoordinateDisplayFormat, string>(CoordinateDisplayFormat.DegreeDecimal, "Degrees"));
            cboCoordFormat.Items.Add(new KeyValuePair<CoordinateDisplayFormat, string>(CoordinateDisplayFormat.DegreeMinute, "Degree-minutes"));
            cboCoordFormat.Items.Add(new KeyValuePair<CoordinateDisplayFormat, string>(CoordinateDisplayFormat.DegreeMinuteSecond, "Degree-minute-seconds"));

            cboCoordFormat.DisplayMemberPath = "Value";
            cboCoordFormat.SelectedValuePath = "Key";

            cboCoordFormat.SelectedIndex = (int)Utilities.Global.Settings.CoordinateDisplayFormat;

            chkBottom.IsChecked = true;
            chkTop.IsChecked = true;
            chkLeft.IsChecked = true;
            chkRight.IsChecked = true;
            chkShowGrid.IsChecked = true;
            Graticule = _parent.Graticule;
            Graticule.MapTitle = _maptitle;
            Graticule.GraticuleExtentChanged += OnGraticuleExtentChanged;
            Graticule.MapRedrawNeeded += OnRedrawNeeded;
        }

        private void RefreshPreview()
        {
            //if (picPreview.Image != null)
            //{
            //    picPreview.Image.Dispose();
            //}

            //var tempFileName = global.MappingForm.SaveTempMapToImage();
            //if (tempFileName?.Length > 0)
            //{
            //    picPreview.ImageLocation = tempFileName;

            //    picPreview.Load();
            //    picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            //}
        }
        private void OnRedrawNeeded(object sender, EventArgs e)
        {
            RefreshPreview();
        }

        private void OnGraticuleExtentChanged(object sender, EventArgs e)
        {
            RefreshPreview();
        }

        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonCancel":
                    break;
                case "buttonOk":
                    ShowGraticule();
                    Close();
                    break;
                case "buttonApply":
                    ShowGraticule();
                    break;
                case "buttonRemoveGraticule":
                    GraticuleRemoved?.Invoke(this, EventArgs.Empty);
                    Close();
                    break;
            }

        }
    }
}
