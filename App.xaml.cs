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

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, Utilities.Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}