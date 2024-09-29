using System;
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
using MapWinGIS;
namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for CategorizeFishingGroundLayerWindow.xaml
    /// </summary>
    public partial class CategorizeFishingGroundLayerWindow : Window
    {
        private RadioButton _selectedButton;
        private Shapefile _inputShapeFile;
        private CategorizedPointShapefile _categorizedShapefile;
        private List<double> _breaks;
        private List<ShapefileCategory> _sfCategories;

        public CategorizeFishingGroundLayerWindow()
        {
            InitializeComponent();
            Loaded += CategorizeFishingGroundLayerWindow_Loaded;
        }


        private void CategorizeFishingGroundLayerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cboCategoryCount.SelectedIndex = 2;
            //_categorizedShapefile.NumberOfBreaks = int.Parse(((ComboBoxItem)cboCategoryCount.SelectedItem).Content.ToString()) - 1;
            _categorizedShapefile.NumberOfBreaks = int.Parse(((ComboBoxItem)cboCategoryCount.SelectedItem).Content.ToString());
            _categorizedShapefile.PointSizeOfMaxCategory = int.Parse(txtSizeOfLargestCategory.Text);
            rbFishingFreq.IsChecked = true;

            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.IsReadOnly = true;
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Range", Binding = new Binding("Range") });

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(System.Windows.Controls.Image));
            //Binding bind = new Binding("image");//please keep "image" name as you have set in your class data member name
            Binding bind = new Binding("ImageInLegend");//please keep "image" name as you have set in your class data member name
            factory.SetValue(System.Windows.Controls.Image.SourceProperty, bind);
            DataTemplate cellTemplate = new DataTemplate() { VisualTree = factory };
            DataGridTemplateColumn imgCol = new DataGridTemplateColumn()
            {
                Header = "", //this is upto you whatever you want to keep, this will be shown on column to represent the data for helping the user...
                CellTemplate = cellTemplate,
                CellStyle = AlignMiddleStyle
            };
            imgCol.Width = new DataGridLength(CategorizedShapefile.PointSizeOfMaxCategory);
            dataGrid.Columns.Add(imgCol);

        }
        private Style AlignMiddleStyle
        {
            get
            {
                Style alignMiddleStyle = new Style(typeof(DataGridCell));

                // Create a Setter object to set (get it? Setter) horizontal alignment.
                Setter setAlign = new
                    Setter(HorizontalAlignmentProperty,
                    HorizontalAlignment.Left);

                // Bind the Setter object above to the Style object
                alignMiddleStyle.Setters.Add(setAlign);
                return alignMiddleStyle;
            }
        }

        public CategorizedPointShapefile CategorizedShapefile { get { return _categorizedShapefile; } }
        public Shapefile Shapefile
        {
            get { return _inputShapeFile; }
            set
            {
                _inputShapeFile = value;
                _categorizedShapefile = new CategorizedPointShapefile(_inputShapeFile);

            }
        }

        public string SelectedCategory { get; private set; }
        private RadioButton ScanForSelectedButton()
        {
            foreach (var item in panelButtonContainer.Children)
            {
                if (item.GetType().Name == "RadioButton" && (bool)((RadioButton)item).IsChecked)
                {
                    return (RadioButton)item;
                }
            }
            return null;
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            string msg = "";
            switch (((Button)sender).Content)
            {
                case "Refresh":
                    if (!string.IsNullOrEmpty(txtSizeOfLargestCategory.Text))
                    {
                        if (int.TryParse(txtSizeOfLargestCategory.Text, out int v))
                        {
                            if (v > 12)
                            {
                                //_categorizedShapefile.NumberOfBreaks = int.Parse(((ComboBoxItem)cboCategoryCount.SelectedItem).Content.ToString()) - 1;
                                _categorizedShapefile.NumberOfBreaks = int.Parse(((ComboBoxItem)cboCategoryCount.SelectedItem).Content.ToString());
                                _categorizedShapefile.PointSizeOfMaxCategory = int.Parse(txtSizeOfLargestCategory.Text);
                                _categorizedShapefile.CategorizeNumericPointLayer(_categorizedShapefile.FishingGroundPointShapefile, int.Parse(_selectedButton.Tag.ToString()));

                                dataGrid.ItemsSource = _categorizedShapefile.CategorizedFishingGroundPointLegendItems;
                                //_sfCategories = _categorizedShapefile.CategorizeField(SelectedCategory, classificationType: tkClassificationType.ctNaturalBreaks);


                            }
                            else
                            {
                                msg = "Size of largest category must be a numeric value equal or greater than 12";
                            }
                        }
                        else
                        {
                            msg = "Size of largest category must be a numeric value";
                        }
                    }
                    else
                    {
                        msg = "Size of largest category cannot be blank";
                    }
                    if (msg.Length > 0)
                    {
                        MessageBox.Show(msg, Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "Ok":
                    _selectedButton = ScanForSelectedButton();
                    if (_selectedButton != null)
                    {
                        SelectedCategory = _selectedButton.Content.ToString();
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("PLease select a category", Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "Cancel":
                    DialogResult = false;
                    break;
            }
        }

        private void OnRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            _selectedButton = (RadioButton)sender;
            SelectedCategory = _selectedButton.Content.ToString();
            _categorizedShapefile.CategorizeNumericPointLayer(_categorizedShapefile.FishingGroundPointShapefile, int.Parse(_selectedButton.Tag.ToString()));

            dataGrid.ItemsSource = _categorizedShapefile.CategorizedFishingGroundPointLegendItems;
        }

    }
}
