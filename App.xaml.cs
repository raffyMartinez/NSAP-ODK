using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using System.Windows;

namespace NSAP_ODK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //NSAPEntities.GPSViewModel = new GPSViewModel();
            //NSAPEntities.FMAViewModel = new FMAViewModel();
            //NSAPEntities.EngineViewModel = new EngineViewModel();
            //NSAPEntities.FishingVesselViewModel = new FishingVesselViewModel();
            //NSAPEntities.FishingGroundViewModel = new FishingGroundViewModel();
            //NSAPEntities.ProvinceViewModel = new ProvinceViewModel();
            //NSAPEntities.LandingSiteViewModel = new LandingSiteViewModel();
            //NSAPEntities.EffortSpecificationViewModel = new EffortSpecificationViewModel();
            //NSAPEntities.GearViewModel = new GearViewModel();
            //NSAPEntities.NSAPEnumeratorViewModel = new NSAPEnumeratorViewModel();
            //NSAPEntities.NSAPRegionViewModel = new NSAPRegionViewModel();
            //NSAPEntities.SizeTypeViewModel = new SizeTypeViewModel();
            //NSAPEntities.TaxaViewModel = new TaxaViewModel();
            //NSAPEntities.FishSpeciesViewModel = new FishSpeciesViewModel();
            //NSAPEntities.NotFishSpeciesViewModel = new NotFishSpeciesViewModel();
            //NSAPEntities.LandingSiteSamplingViewModel = new LandingSiteSamplingViewModel();
            //NSAPEntities.GearUnloadViewModel = new GearUnloadViewModel();
            //NSAPEntities.VesselUnloadViewModel = new VesselUnloadViewModel();
            //NSAPEntities.VesselEffortViewModel = new VesselEffortViewModel();
            //NSAPEntities.VesselCatchViewModel = new VesselCatchViewModel();
            //NSAPEntities.GearSoakViewModel = new GearSoakViewModel();
            //NSAPEntities.FishingGroundGridViewModel = new FishingGroundGridViewModel();
            //NSAPEntities.CatchLenFreqViewModel = new CatchLenFreqViewModel();
            //NSAPEntities.CatchLengthWeightViewModel = new CatchLengthWeightViewModel();
            //NSAPEntities.CatchLengthViewModel = new CatchLengthViewModel();
            //NSAPEntities.CatchMaturityViewModel = new CatchMaturityViewModel();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}