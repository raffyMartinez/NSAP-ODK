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
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for UnmatchedJSONAnalysisResultWindow.xaml
    /// </summary>
    public partial class UnmatchedJSONAnalysisResultWindow : Window
    {
        private static UnmatchedJSONAnalysisResultWindow _instance;
        public UnmatchedJSONAnalysisResultWindow()
        {
            InitializeComponent();
            Loaded += UnmatchedJSONAnalysisResultWindow_Loaded;
            Closing += UnmatchedJSONAnalysisResultWindow_Closing;

        }

        public void ShowAnalysis()
        {
            foreach (RadioButton rb in panelButtons.Children)
            {
                rb.IsChecked = false;
            }
            if (UnmatchedFieldsFromJSONFile != null)
            {
                foreach (RadioButton rb in panelButtons.Children)
                {
                    if (rb.Content.ToString() == "Enumerator names")
                    {
                        rb.IsChecked = true;
                        break;
                    }
                }
            }
            if (UnmatchedFieldsFromJSONFile != null)
            {
                labelTitle.Content = System.IO.Path.GetFileName(UnmatchedFieldsFromJSONFile.FileName);
            }
            else
            {
                labelTitle.Content = "The selected JSON file do not have mismatched items";
                labelExplain.Text = "";
            }
        }
        private void UnmatchedJSONAnalysisResultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Analysis of content of JSON file";
            ShowAnalysis();
        }

        private void UnmatchedJSONAnalysisResultWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        public static UnmatchedJSONAnalysisResultWindow Instance
        {
            get
            {
                return _instance;
            }
        }
        public static UnmatchedJSONAnalysisResultWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UnmatchedJSONAnalysisResultWindow();
            }
            return _instance;
        }
        public UnmatchedFieldsFromJSONFile UnmatchedFieldsFromJSONFile { get; set; }
        private void OnRBChecked(object sender, RoutedEventArgs e)
        {
            if (UnmatchedFieldsFromJSONFile != null)
            {
                listBox.ItemsSource = null;
                listBox.Items.Clear();
                switch (((RadioButton)sender).Content)
                {
                    case "Enumerator names":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.EnumeratorNames;
                        labelExplain.Text = "If there are names of enumerators listed, then it means that these were inputted by typing instead of selecting from a list.";
                        //foreach(var item in UnmatchedFieldsFromJSONFile.EnumeratorNames)
                        //{
                        //    listBox.Items.Add(item);
                        //}
                        break;
                    case "Enumerator IDs":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.EnumeratorIDs;
                        labelExplain.Text = "If there are IDs listed, then it means that the select list of enumerators were created from another backend that is different from the backend in your computer";
                        break;
                    case "Fishing gear names":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.FishingGearNames;
                        labelExplain.Text = "If there are names of fishing gears listed, then it means that these were inputted by typing instead of selecting from a list";
                        break;
                    case "Fishing gear codes":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.FishingGearCodes;
                        labelExplain.Text = "If there are fishing gear codes listed, then it means that the select list of fishing gears were created from another backend that is different from the backend in your computer";
                        break;
                    case "Landing site names":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.LandingSiteNames;
                        labelExplain.Text = "If there are names of landing sites listed, then it means that these were inputted by typing instead of selecting from a list";
                        break;
                    case "Landing site IDs":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.LandingSiteIDs;
                        labelExplain.Text = "If there are IDs listed, then it means that the select list of landing sites were created from another backend that is different from the backend in your computer";
                        break;
                    case "Species":
                        listBox.ItemsSource = UnmatchedFieldsFromJSONFile.SpeciesNamesTaxa;
                        labelExplain.Text = "If there are species names listed, then it means that these were inputted by typing instead of selecting from a list";
                        break;
                }
            }
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
