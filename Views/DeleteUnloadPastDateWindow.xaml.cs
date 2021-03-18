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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for DeleteUnloadPastDateWindow.xaml
    /// </summary>
    public partial class DeleteUnloadPastDateWindow : Window
    {
        public DeleteUnloadPastDateWindow()
        {
            InitializeComponent();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonCancel":
                    Close();
                    break;
                case "buttonOk":
                    if (textDate.Text.Length > 0)
                    {
                        if (DateTime.TryParse(textDate.Text, out DateTime d))
                        {
                            var unloads = Entities.NSAPEntities.VesselUnloadViewModel.GetUnloadsPastDateUploadLocalDB(d);
                            Entities.NSAPEntities.VesselUnloadViewModel.DeleteUnloadChildren(unloads, out int deletedVesselUnloadCount);
                            MessageBox.Show($"Deleted {deletedVesselUnloadCount} vessel unload out of {unloads.Count}", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                    }
                    break;
            }
        }
    }
}
