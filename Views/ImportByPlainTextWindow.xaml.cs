using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ImportByPlainTextWindow.xaml
    /// </summary>
    public partial class ImportByPlainTextWindow : Window
    {
        private NSAPRegion _selectedRegion;
        private bool _errorMessageHandled;
        private DispatcherTimer _timer;
        private LandingSite _landingSite;
        public ImportByPlainTextWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        public ImportByPlainTextWindow(LandingSite ls, NSAPEntity entityType)
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            _landingSite = ls;
            NSAPEntityType = entityType;
        }

        private void FillRegionRadioButtons()
        {
            panelRegions.Children.Clear();
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection.OrderBy(t => t.Sequence))
            {
                var radioButton = new RadioButton
                {
                    Content = region.ShortName,
                    Tag = region,
                    Margin = new Thickness(10, 5, 0, 0)
                };
                radioButton.Checked += OnRadioButton_Checked;
                panelRegions.Children.Add(radioButton);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            Close();
        }
        private void OnRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _selectedRegion = (NSAPRegion)((RadioButton)sender).Tag;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            progressLabel.Content = "";
            progressBar.Value = 0;
            menuImportFile.IsEnabled = false;
            panelSector.Visibility = Visibility.Collapsed;
            switch (NSAPEntityType)
            {
                case NSAPEntity.GPS:
                    Title = "Import GPS in csv";
                    FillRegionRadioButtons();
                    labelEntityImport.Text = "Import GPS using CSV with headers in the first row";
                    menuImportFile.IsEnabled = true;
                    break;
                case NSAPEntity.Enumerator:
                    Title = "Import enumerator names";
                    FillRegionCheckBoxes();
                    break;
                case NSAPEntity.FishingGear:
                    Title = "Import fishing gear names";
                    break;
                case NSAPEntity.FishingVessel:
                    Title = "Import fishing vessels";
                    panelSector.Visibility = Visibility.Visible;
                    if (_landingSite == null)
                    {
                        FillRegionRadioButtons();
                    }
                    else
                    {
                        labelRegion.Content = "Landing site";
                        panelRegions.Children.Clear();
                        TextBlock tb = new TextBlock { Text = _landingSite.ToString(), TextWrapping = TextWrapping.Wrap };
                        panelRegions.Children.Add(new CheckBox { Content = tb, Margin = new Thickness(10, 5, 0, 0), IsChecked = true, IsEnabled = false });
                        //panelRegions.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
            labelTitle.Content = Title;


        }

        private void FillRegionCheckBoxes()
        {
            int counter = 0;
            foreach (var r in NSAPEntities.NSAPRegionViewModel.GetAllNSAPRegions())
            {
                if (counter == 0)
                {
                    ((CheckBox)panelRegions.Children[0]).Content = r.ShortName;
                    ((CheckBox)panelRegions.Children[0]).Tag = r.Code;
                }
                else
                {
                    panelRegions.Children.Add(new CheckBox { Tag = r.Code, Content = r.ShortName, Margin = new Thickness(10, 5, 0, 0) });
                }
                counter++;
            }
        }

        private int CheckedRegions()
        {
            int count = 0;
            if (panelRegions.Children.Count > 0)
            {
                if (panelRegions.Children[0].GetType().Name == "CheckBox")
                {
                    foreach (CheckBox c in panelRegions.Children)
                    {
                        if ((bool)c.IsChecked)
                        {
                            count++;
                        }
                    }
                }
                else if (panelRegions.Children[0].GetType().Name == "RadioButton")
                {
                    foreach (RadioButton rb in panelRegions.Children)
                    {
                        if ((bool)rb.IsChecked)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
        public NSAPEntity NSAPEntityType { get; set; }
        private async void onButtonClick(object sender, RoutedEventArgs e)
        {
            NSAPRegion region = null;
            List<EntityValidationMessage> entityMessages = new List<EntityValidationMessage>();
            int importCount = 0;
            switch (((Button)sender).Name)
            {
                case "buttonSelect":
                    break;
                case "buttonOk":
                    string msg = "";
                    if (textBox.Text.Length > 0 && CheckedRegions() > 0)
                    {
                        switch (NSAPEntityType)
                        {
                            case NSAPEntity.FishingVessel:
                                //bool autoPrefix = (bool)checkboxIncludePrefix.IsChecked;
                                FisheriesSector fs = FisheriesSector.Municipal;
                                if (!(bool)checkboxMunicipalSector.IsChecked)
                                {
                                    fs = FisheriesSector.Commercial;
                                }

                                if (_landingSite == null)
                                {
                                    foreach (RadioButton rb in panelRegions.Children)
                                    {
                                        if ((bool)rb.IsChecked)
                                        {
                                            region = (NSAPRegion)rb.Tag;
                                            break;
                                        }
                                    }
                                }

                                ProgressDialogWindow pdw = ProgressDialogWindow.GetInstance("import fishing vessels");
                                pdw.Sector = fs;
                                pdw.ListToImportFromTextBox = textBox.Text;
                                pdw.Region = region;
                                pdw.LandingSite = _landingSite;

                                pdw.Owner = Owner;
                                if (pdw.Visibility == Visibility.Visible)
                                {
                                    pdw.BringIntoView();
                                }
                                else
                                {
                                    pdw.Show();
                                }
                                _timer.Interval = TimeSpan.FromSeconds(3);
                                _timer.Start();
                                //NSAPEntities.FishingVesselViewModel.BulkImportFishingVessels += FishingVesselViewModel_BulkImportFishingVessels;
                                //importCount = await NSAPEntities.FishingVesselViewModel.ImportVesselsAsync(textBox.Text, region, fs);
                                //NSAPEntities.FishingVesselViewModel.BulkImportFishingVessels -= FishingVesselViewModel_BulkImportFishingVessels;

                                break;
                            case NSAPEntity.GPS:

                                NSAPEntities.GPSViewModel.ImportGPSCSV = textBox.Text;
                                msg = NSAPEntities.GPSViewModel.GPSImportErrorMessage;
                                if (NSAPEntities.GPSViewModel.GPSCSVImportSuccess)
                                {
                                    if (NSAPEntities.GPSViewModel.GPSImportErrorMessage?.Length > 0)
                                    {
                                        msg = NSAPEntities.GPSViewModel.GPSImportErrorMessage;
                                    }
                                }
                                importCount = NSAPEntities.GPSViewModel.ImportGPSCSVImportedCount;


                                break;
                            case NSAPEntity.Enumerator:
                                foreach (var item in textBox.Text.Split('\n').ToList())
                                {
                                    var nse = new NSAPEnumerator { Name = item.Trim(), ID = NSAPEntities.NSAPEnumeratorViewModel.NextRecordNumber };
                                    if (NSAPEntities.NSAPEnumeratorViewModel.EntityValidated(nse, out entityMessages, true))
                                    {
                                        //if (NSAPEntities.NSAPEnumeratorViewModel.AddRecordToRepo(new NSAPEnumerator { Name = item.Trim(), ID = NSAPEntities.NSAPEnumeratorViewModel.NextRecordNumber }))
                                        if (NSAPEntities.NSAPEnumeratorViewModel.AddRecordToRepo(nse))
                                        {
                                            foreach (CheckBox c in panelRegions.Children)
                                            {
                                                if ((bool)c.IsChecked)
                                                {
                                                    region = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(c.Tag.ToString());
                                                    var nre = NSAPRegionWithEntitiesRepository.CreateRegionEnumerator
                                                    (
                                                        enumerator: NSAPEntities.NSAPEnumeratorViewModel.CurrentEntity,
                                                        region: region,
                                                        added: DateTime.Now
                                                    );

                                                    var rvm = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(region);
                                                    rvm.AddEnumerator(nre);
                                                }
                                            }
                                            importCount++;
                                        }
                                    }
                                    else
                                    {
                                        foreach (var msg1 in entityMessages)
                                        {
                                            if (msg1.MessageType == MessageType.Error)
                                            {
                                                Logger.Log($"Batch import by text error in adding new enumerator: {msg1.Message} after adding {nse.Name}");
                                            }
                                        }
                                    }
                                }
                                break;
                        }

                        if (NSAPEntityType != NSAPEntity.FishingVessel)
                        {
                            ((MainWindow)Owner).RefreshEntityGrid();
                            if (importCount > 0)
                            {
                                MessageBox.Show($"Succesfully imported {importCount} items", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                                Close();
                            }
                            else
                            {
                                if (msg?.Length > 0)
                                {
                                    MessageBox.Show(msg, "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show($"Was not able to import an item", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Provide one or more items to import and a selected landing site", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    break;
                case "buttonCancel":
                    Close();
                    break;
            }

        }

        private void FishingVesselViewModel_BulkImportFishingVessels(object sender, Entities.Database.EntityBulkImportEventArg e)
        {
            switch (e.Intent)
            {
                case "start":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Maximum = e.RecordsToImport;

                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = "Getting items for importing";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "imported_entity":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.ImportedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              string entityType = "";
                              if (e.NSAPEntity == NSAPEntity.FishingVessel)
                              {
                                  entityType = "fishing vessel";
                              }
                              progressLabel.Content = $"Imported {entityType} {e.ImportedCount} of {progressBar.Maximum}: {e.ImportedEntityName}";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "import_done":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Value = 0;

                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Import done";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
            }
        }

        public string OpenCSVFile(string entityName)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = false;
            ofd.Title = $"Open csv file containg {entityName} data";
            ofd.Filter = "CSV files(*.csv)|*.CSV|All file types (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if ((bool)ofd.ShowDialog())
            {

                return ofd.FileName;
            }
            return null;
        }

        private void onMenuClick(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Name)
            {
                case "menuImportFile":
                    string csv = "";
                    string entityName = "";
                    switch (NSAPEntityType)
                    {
                        case NSAPEntity.GPS:
                            entityName = "GPS";
                            break;
                        default:
                            break;
                    }
                    csv = OpenCSVFile(entityName);
                    if (csv?.Length > 0)
                    {
                        textBox.Text = File.ReadAllText(csv);
                    }

                    break;
            }
        }
    }
}
