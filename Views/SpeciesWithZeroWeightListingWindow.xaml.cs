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
    /// Interaction logic for SpeciesWithZeroWeightListingWindow.xaml
    /// </summary>
    /// 
    public partial class SpeciesWithZeroWeightListingWindow : Window
    {
        private static SpeciesWithZeroWeightListingWindow _instance;
        private List<CatchWithZeroWeight> _catchWithZeroWeights;
        public static SpeciesWithZeroWeightListingWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SpeciesWithZeroWeightListingWindow();
            }
            return _instance;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        public List<string> JSONFolders { get; set; }
        public SpeciesWithZeroWeightListingWindow()
        {
            InitializeComponent();
            Title = "Species in catch composition with weight of zero";
            labelTitle.Content = "List of species in catch composition with weight of zero";
            Closing += SpeciesWithZeroWeightListingWindow_Closing;
            gridItems.IsReadOnly = true;
            gridItems.AutoGenerateColumns = false;
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("FormVersion") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Vessel unload ID", Binding = new Binding("VesselUnloadID") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Species ID", Binding = new Binding("SpeciesID") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Species name", Binding = new Binding("SpeciesNameToUse") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Sampling date", Binding = new Binding("SamplingDateFormatted") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearName") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Enumerator") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("FishingVessel") });
            gridItems.Columns.Add(new DataGridCheckBoxColumn { Header = "JSON file found", Binding = new Binding("JSONFileFound") });
            gridItems.Columns.Add(new DataGridTextColumn { Header = "Corrected weight", Binding = new Binding("CorrectedWeight") });
            //gridItems.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.Parent.Parent.") });
        }

        private void SpeciesWithZeroWeightListingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            switch(((MenuItem)sender).Name)
            {
                case "menuLocateJSONFolder":
                    ListFoldersWindow lfw = new ListFoldersWindow();
                    var dr = lfw.ShowDialog();
                    if((bool)dr)
                    {

                    }
                    break;
                case "menuCorrectWeights":
                    if(JSONFolders!=null && JSONFolders.Count>0)
                    {

                    }
                    else
                    {
                        MessageBox.Show("Please provide folders where JSON files are saved",
                            Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }
                    break;
                case "menuClose":
                    Close();
                    break;
            }

        }
        public List<CatchWithZeroWeight> CatchesWithZeroWeight
        {
            get { return _catchWithZeroWeights; }
            set
            {
                _catchWithZeroWeights = value;
                gridItems.DataContext = _catchWithZeroWeights;
            }
        }
        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
