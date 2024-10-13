using System.Reflection;
using System.Windows;
using OSVersionExtension;



namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWIndowLoaded(object sender, RoutedEventArgs e)
        {
            labelVersion.Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            labelNetVersion.Content = $".NET version: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}";

            string windows_os = OSVersion.GetOperatingSystem().ToString();
            if (OSVersion.GetOSVersion().Version.Major >= 10)
            {
                labelWindowsVersion.Content = $"Windows version: {windows_os} {OSVersion.MajorVersion10Properties().DisplayVersion}";
            }
            else
            {
                labelWindowsVersion.Content = $"Windows version: {windows_os}";
            }
        }
    }
}
