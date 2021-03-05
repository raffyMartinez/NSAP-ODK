using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;


namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }
        private async Task LoadEntitiesAsync()
        {
            await Task.Run(() => LoadEntities());
            LabelLoading.Content = "Finished reading database";
            //CSVFIleManager.ReadCSVXML();
            Close();
            //MessageBox.Show("Finished loading data");
        }
        private void EntityRead(string entity)
        {

            //if (LabelEntityRead.InvokeRequired)
            //    LabelEntityRead.Invoke(new Action(() => LabelEntityRead.Content = entity);
        }
        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            labelVersion.Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            ProgressBarRead.IsIndeterminate = true;
            await LoadEntitiesAsync();
            ProgressBarRead.IsIndeterminate = false; // Maybe hide it, too

           

        }
        private void LoadEntities()
        {
            NSAPEntities.GPSViewModel = new GPSViewModel();

            NSAPEntities.FMAViewModel = new FMAViewModel();
            NSAPEntities.EngineViewModel = new EngineViewModel();
            NSAPEntities.FishingVesselViewModel = new FishingVesselViewModel();
            NSAPEntities.FishingGroundViewModel = new FishingGroundViewModel();
            NSAPEntities.EffortSpecificationViewModel = new EffortSpecificationViewModel();
            NSAPEntities.GearViewModel = new GearViewModel();
            NSAPEntities.NSAPEnumeratorViewModel = new NSAPEnumeratorViewModel();
            NSAPEntities.NSAPRegionViewModel = new NSAPRegionViewModel();
            NSAPEntities.ProvinceViewModel = new ProvinceViewModel();
            NSAPEntities.LandingSiteViewModel = new LandingSiteViewModel();

            NSAPEntities.NSAPRegionViewModel.SetNSAPRegionsWithEntitiesRepositories();

            NSAPEntities.SizeTypeViewModel = new SizeTypeViewModel();
            NSAPEntities.TaxaViewModel = new TaxaViewModel();
            NSAPEntities.FishSpeciesViewModel = new FishSpeciesViewModel();
            NSAPEntities.NotFishSpeciesViewModel = new NotFishSpeciesViewModel();
            NSAPEntities.LandingSiteSamplingViewModel = new LandingSiteSamplingViewModel();
            NSAPEntities.GearUnloadViewModel = new GearUnloadViewModel();
            NSAPEntities.VesselUnloadViewModel = new VesselUnloadViewModel();
            NSAPEntities.VesselEffortViewModel = new VesselEffortViewModel();
            NSAPEntities.VesselCatchViewModel = new VesselCatchViewModel();
            NSAPEntities.GearSoakViewModel = new GearSoakViewModel();
            NSAPEntities.FishingGroundGridViewModel = new FishingGroundGridViewModel();
            NSAPEntities.CatchLenFreqViewModel = new CatchLenFreqViewModel();
            NSAPEntities.CatchLengthWeightViewModel = new CatchLengthWeightViewModel();
            NSAPEntities.CatchLengthViewModel = new CatchLengthViewModel();
            NSAPEntities.CatchMaturityViewModel = new CatchMaturityViewModel();
            NSAPEntities.DBSummary = new DBSummary();
            NSAPEntities.DatabaseEnumeratorSummary = new DatabaseEnumeratorSummary();
            NSAPEntities.JSONFileViewModel = new JSONFileViewModel();
        }
    }
}
