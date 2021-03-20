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
using System.Windows.Threading;

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

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonCancel":
                    Close();
                    break;
                case "buttonOk":
                    if (textDate.Text.Length > 0 && DateTime.TryParse(textDate.Text, out DateTime d))
                    {
                        var unloads = Entities.NSAPEntities.VesselUnloadViewModel.GetUnloadsPastDateUploadLocalDB(d);
                        if (unloads.Count > 0)
                        {
                            statusBar.Maximum = unloads.Count;
                            panelStatus.Visibility = Visibility.Visible;
                            Entities.NSAPEntities.VesselUnloadViewModel.DeleteUnloadChildrenEvent += OnDeleteVesselUnloadChildren;
                            var result = await Entities.NSAPEntities.VesselUnloadViewModel.DeleteUnloadChildrenAsync(unloads);
                            MessageBox.Show($"Deleted {result.VesselUnloadToDeleteCoount} vessel unload out of {unloads.Count}",
                                "NSAP-ODK Database",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("There are no vessel unload with upload date that match the date provided",
                                "NSAP-ODK Database",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Please provide a proper date",
                            "NSAP-ODK Database",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                    }
                    break;
            }
        }

        private void OnDeleteVesselUnloadChildren(object sender, EventArgs e)
        {
            statusBar.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      statusBar.Value += 1;
                      //do what you need to do on UI Thread
                      return null;
                  }
                 ), null);

            statusLabel.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      statusLabel.Content = $"Deleted {(int)statusBar.Value} unloads from {(int)statusBar.Maximum}";
                      //do what you need to do on UI Thread
                      return null;
                  }
                 ), null);
        }
    }
}
