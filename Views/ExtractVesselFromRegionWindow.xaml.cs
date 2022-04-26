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
using Microsoft.Win32;
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using System.IO;
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ExtractVesselFromRegionWindow.xaml
    /// </summary>
    public partial class ExtractVesselFromRegionWindow : Window
    {
        NSAPRegion _selectedRegion;
        private List<string> _vesselList;
        public ExtractVesselFromRegionWindow()
        {
            InitializeComponent();
            Loaded += ExtractVesselFromRegionWindow_Loaded;
            Closing += OnExtractVesselFromRegionWindow_Closing;
        }

        private void OnExtractVesselFromRegionWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void ExtractVesselFromRegionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            progressLabel.Visibility=Visibility.Collapsed;
            progressBar.Visibility = Visibility.Collapsed;
            foreach (var item in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection.OrderBy(t => t.Sequence))
            {
                RadioButton rb = new RadioButton
                {
                    Content = item.ShortName,
                    Tag = item,
                    Margin = new Thickness(3, 3, 3, 3)
                };
                rb.Checked += OnRb_Checked;
                stackPanelButtons.Children.Add(rb);
            }
        }

        private void OnRb_Checked(object sender, RoutedEventArgs e)
        {
            _selectedRegion = (NSAPRegion)((RadioButton)sender).Tag;
        }


        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonExtract":
                    if (_selectedRegion != null)
                    {
                        NSAPEntities.NSAPRegionViewModel.VesselListEvent += NSAPRegionViewModel_VesselListEvent;
                        progressBar.Visibility = Visibility.Visible;
                        progressLabel.Visibility = Visibility.Visible;
                        progressLabel.Content = "";
                        _vesselList = await NSAPEntities.NSAPRegionViewModel.GetVesselNamesByRegionAsync(_selectedRegion);
                        NSAPEntities.NSAPRegionViewModel.VesselListEvent -= NSAPRegionViewModel_VesselListEvent;
                        if(_vesselList.Count>0)
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Title = "Save list of vessel names into a text file";
                            sfd.Filter = "Text file (*.txt)|*.txt";
                            sfd.DefaultExt = "*.txt";
                            if ((bool)sfd.ShowDialog())
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach(var v in _vesselList)
                                {
                                    sb.Append(v + "\r\n");
                                }
                                File.WriteAllText(sfd.FileName, sb.ToString());
                                if(File.Exists(sfd.FileName))
                                {
                                    MessageBox.Show("Vessel list saved successfully","NSAP-ODK Database",MessageBoxButton.OK,MessageBoxImage.Information);
                                    Close();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("There are no names of vessels to export", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void NSAPRegionViewModel_VesselListEvent(object sender, EventArgs e)
        {
            var ev = (VesselListingEventArgs)e;
            switch(ev.Intent)
            {
                case "start":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Creating list...";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "end":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = (int)ev.ListCount;
                              progressBar.Maximum = (int)ev.ListCount;
                              progressBar.IsIndeterminate = false;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"List finished with {(int)ev.ListCount} items";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }
    }
}
