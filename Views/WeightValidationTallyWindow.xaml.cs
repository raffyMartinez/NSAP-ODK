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
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for WeightValidationTallyWindow.xaml
    /// </summary>
    public partial class WeightValidationTallyWindow : Window
    {
        private List<SummaryItem> _summaryItems;
        private static WeightValidationTallyWindow _instance;

        public static WeightValidationTallyWindow GetInstance(List<SummaryItem> summaryItems)
        {
            if (_instance == null)
            {
                _instance = new WeightValidationTallyWindow(summaryItems);
            }
            return _instance;
        }
        public WeightValidationTallyWindow(List<SummaryItem> summaryItems)
        {
            InitializeComponent();
            _summaryItems = summaryItems;
            Loaded += OnWeightValidationTallyWindow_Loaded;
            Closed += OnWeightValidationTallyWindow_Closed;
        }

        public DataGrid DataGrid { get; set; }
        private void OnWeightValidationTallyWindow_Closed(object sender, EventArgs e)
        {
            _instance = null;
        }

        private void OnWeightValidationTallyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Control c in gridMain.Children)
            {
                if (c.GetType().Name == "Label" && c.Tag != null && c.Tag.ToString() == "count label")
                {
                    ((Label)c).Content = "";
                }
            }

            int countSamplingTypeMixed = 0;
            int countSamplingTypeNone = 0;
            int countSamplingTypeSampled = 0;
            int countSamplingTypeTotalEnumeration = 0;

            int countValidationValid = 0;
            int countValidationNotValid = 0;
            int countValidationNotApplicable = 0;
            int countValidationNotValidated = 0;



            foreach (var item in _summaryItems)
            {
                switch (item.SamplingTypeFlag)
                {
                    case SamplingTypeFlag.SamplingTypeMixed:
                        countSamplingTypeMixed++;
                        break;
                    case SamplingTypeFlag.SamplingTypeNone:
                        countSamplingTypeNone++;
                        break;
                    case SamplingTypeFlag.SamplingTypeSampled:
                        countSamplingTypeSampled++;
                        break;
                    case SamplingTypeFlag.SamplingTypeTotalEnumeration:
                        countSamplingTypeTotalEnumeration++;
                        break;
                }

                switch (item.WeightValidationFlag)
                {
                    case WeightValidationFlag.WeightValidationInValid:
                        countValidationNotValid++;
                        break;
                    case WeightValidationFlag.WeightValidationNotApplicable:
                        countValidationNotApplicable++;
                        break;
                    case WeightValidationFlag.WeightValidationNotValidated:
                        countValidationNotValidated++;
                        break;
                    case WeightValidationFlag.WeightValidationValid:
                        countValidationValid++;
                        break;
                }
            }

            labelValidationValid.Content = countValidationValid.ToString();
            labelValidationNotValid.Content = countValidationNotValid.ToString();
            //int countNotApplicableNoCatch= _summaryItems.Count(t => t.VesselUnload.WeightValidationFlag == WeightValidationFlag.WeightValidationNotApplicable);
            labelValidationNotValidated.Content = countValidationNotValidated.ToString();
            labelValidationNotApplicable.Content = countValidationNotApplicable.ToString();

            labelSamplingMixed.Content = countSamplingTypeMixed.ToString();
            labelSamplingSampling.Content = countSamplingTypeSampled.ToString();
            labelSamplingTotalEnumeration.Content = countSamplingTypeTotalEnumeration.ToString();
            labelSamplingNotSampled.Content = countSamplingTypeNone.ToString();
        }


        private void onButtonClicked(object sender, RoutedEventArgs e)
        {
            List<SummaryItem> filteredList = null;
            bool applyFilter = true;
            switch (((Button)sender).Name)
            {
                case "buttonFilterValidationValid":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t=>t.WeightValidationFlag==WeightValidationFlag.WeightValidationValid).ToList());
                    filteredList = _summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationValid).ToList();
                    break;
                case "buttonFilterValidationNotValid":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationInValid).ToList());
                    filteredList = _summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationInValid).ToList();
                    break;
                case "buttonFilterValidationNotApplicable":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationNotApplicable).ToList());
                    filteredList = _summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationNotApplicable).ToList();
                    break;
                case "buttonFilterValidationNotValidated":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationNotValidated).ToList());
                    filteredList = _summaryItems.Where(t => t.WeightValidationFlag == WeightValidationFlag.WeightValidationNotValidated).ToList();
                    break;

                case "buttonSamplingTotalEnumeration":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeTotalEnumeration).ToList());
                    filteredList = _summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeTotalEnumeration).ToList();
                    break;
                case "buttonSamplingMixed":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeMixed).ToList());
                    filteredList = _summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeMixed).ToList();
                    break;
                case "buttonSamplingSampling":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeSampled).ToList());
                    filteredList = _summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeSampled).ToList();
                    break;
                case "buttonSamplingNotSampled":
                    //((GearUnloadWindow)Owner).FilterWeightGrid(_summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeNone).ToList());
                    filteredList = _summaryItems.Where(t => t.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeNone).ToList();
                    break;
                case "buttonResetFilter":
                    //((GearUnloadWindow)Owner).ResetFilter();
                    filteredList = _summaryItems;
                    break;
                case "buttonClose":
                    Close();
                    applyFilter = false;
                    break;
            }

            if (applyFilter) DataGrid.DataContext = filteredList;

        }
    }
}
