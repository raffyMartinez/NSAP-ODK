using System.Reflection;
using System.Windows;




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
        }
    }
}
