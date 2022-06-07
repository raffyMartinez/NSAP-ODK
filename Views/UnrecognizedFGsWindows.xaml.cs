using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using wf = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using NSAP_ODK.Entities.Database;
using System.IO;
using NSAP_ODK.Entities;
using System.Windows.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for UnrecognizedFGsWindows.xaml
    /// </summary>
    public partial class UnrecognizedFGsWindows : Window
    {
        private CollectedUnrecognizedFG _unrecognizedFishingGrounds;
        private NSAPRegion _selectedRegion;
        private NSAPRegionFMA _selectedRegionFMA;
        private NSAPRegionFMAFishingGround _selectedRegionFMAFG;
        private FishingGround _newFG;
        private FishingGround _selectedFG;
        private bool _selectAll = false;
        private NSAPRegionFMAFishingGround _nrfg;
        private int _savedCount;
        public UnrecognizedFGsWindows()
        {
            InitializeComponent();
            Loaded += UnrecognizedFGsWindows_Loaded;
            Closing += UnrecognizedFGsWindows_Closing;
        }

        private void UnrecognizedFGsWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cboRegion.SelectionChanged -= CboRegion_SelectionChanged;
            cboFMA.SelectionChanged -= CboRegion_SelectionChanged;
            cboFishingGround.SelectionChanged -= CboRegion_SelectionChanged;
        }

        private void UnrecognizedFGsWindows_Loaded(object sender, RoutedEventArgs e)
        {
            cboRegion.SelectedValuePath = "Key";
            cboRegion.DisplayMemberPath = "Value";

            cboFMA.SelectedValuePath = "Key";
            cboFMA.DisplayMemberPath = "Value";

            cboFishingGround.SelectedValuePath = "Key";
            cboFishingGround.DisplayMemberPath = "Value";

            labelURFG.Content = $"List of samplings with unrecognized fishing grounds generated on {UnrecognizedFishingGrounds.DateCreated.ToString("MMM-dd-yyyy HH:mm")}";
            if (_unrecognizedFishingGrounds != null && _unrecognizedFishingGrounds.UnrecognizedFishingGrounds.Count > 0)
            {
                dataGrid.IsReadOnly = false;
                dataGrid.CanUserAddRows = false;
                dataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
                dataGrid.DataContext = _unrecognizedFishingGrounds.UnrecognizedFishingGrounds.OrderBy(t => t.SamplingDate).ToList();

                dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Select", Binding = new Binding("Selected"), IsReadOnly = false });
                var col = new DataGridTextColumn()
                {
                    Binding = new Binding("SamplingDate"),
                    Header = "Date and time sampled"
                };
                col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                dataGrid.Columns.Add(col);
                //dataGrid.Columns.Add(new DataGridTextColumn { Header = "RowID", Binding = new Binding("RowID") });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Enumerator"), IsReadOnly = true });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("FishingVessel"), IsReadOnly = true });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("FishingGear"), IsReadOnly = true });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region"), IsReadOnly = true });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA"), IsReadOnly = true });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGroundName"), IsReadOnly = true });
                dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSite"), IsReadOnly = true });
            }

            cboRegion.SelectionChanged += CboRegion_SelectionChanged;
            cboFMA.SelectionChanged += CboRegion_SelectionChanged;
            cboFishingGround.SelectionChanged += CboRegion_SelectionChanged;
            cboFishingGround.MouseDoubleClick += CboFishingGround_MouseDoubleClick;
        }

        public FishingGround NewFishingGround
        {
            get { return _newFG; }
            set
            {
                _newFG = value;
                if (_selectedRegionFMA != null)
                {


                    _nrfg = new NSAPRegionFMAFishingGround
                    {
                        RowID = NSAPRegionWithEntitiesRepository.MaxRecordNumber_FishingGround() + 1,
                        RegionFMA = _selectedRegionFMA,
                        FishingGround = _newFG

                    };
                    NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(_selectedRegion).AddFMAFishingGround(_nrfg);

                    AddFishingGroundToCombo();
                    foreach (KeyValuePair<NSAPRegionFMAFishingGround, string> item in cboFishingGround.Items)
                    {
                        if (item.Key.FishingGround.Code == _newFG.Code)
                        {
                            cboFishingGround.SelectedItem = item;
                        }
                    }
                    _selectedFG = _newFG;
                }
            }
        }
        private void CboFishingGround_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NewFishingGroundWindow nfgw = new NewFishingGroundWindow();
            nfgw.Owner = this;
            nfgw.ShowDialog();
        }

        private void CboRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBox)sender).Name)
            {
                case "cboRegion":
                    _selectedRegion = ((KeyValuePair<NSAPRegion, string>)cboRegion.SelectedItem).Key;

                    cboFMA.Items.Clear();
                    cboFishingGround.Items.Clear();
                    foreach (var item in _selectedRegion.FMAs)
                    {
                        cboFMA.Items.Add(new KeyValuePair<NSAPRegionFMA, string>(item, item.FMA.Name));
                    }
                    break;
                case "cboFMA":
                    if (cboFMA.SelectedItem != null)
                    {
                        _selectedRegionFMA = ((KeyValuePair<NSAPRegionFMA, string>)cboFMA.SelectedItem).Key;

                        AddFishingGroundToCombo();
                    }
                    break;
                case "cboFishingGround":
                    if (cboFishingGround.SelectedItem != null)
                    {
                        _selectedRegionFMAFG = ((KeyValuePair<NSAPRegionFMAFishingGround, string>)cboFishingGround.SelectedItem).Key;
                        _selectedFG = _selectedRegionFMAFG.FishingGround;
                    }
                    break;
            }
        }
        private void AddFishingGroundToCombo()
        {
            cboFishingGround.Items.Clear();
            foreach (var fg in _selectedRegionFMA.FishingGrounds)
            {
                cboFishingGround.Items.Add(new KeyValuePair<NSAPRegionFMAFishingGround, string>(fg, fg.FishingGround.Name));
            }
        }
        public DateTime UnrecognizedFGDateCreated { get; set; }
        public CollectedUnrecognizedFG UnrecognizedFishingGrounds
        {
            get { return _unrecognizedFishingGrounds; }
            set
            {
                _unrecognizedFishingGrounds = value;
            }
        }

        private void Save()
        {
            var xml = new XElement("DateCreated", _unrecognizedFishingGrounds.DateCreated.ToString("MMM-dd-yyyy HH:mm"));
            xml.Add(new XElement("UnrecognizedFishingGrounds", _unrecognizedFishingGrounds.UnrecognizedFishingGrounds.Select(x => new XElement("UnrecognizedFishingGround",
                                              new XAttribute("Row ID", x.RowID),
                                              new XAttribute("Region", x.Region),
                                              new XAttribute("FMA", x.FMA),
                                              new XAttribute("FishingGround", x.FishingGroundName),
                                              new XAttribute("LandingSite", x.LandingSite),
                                              new XAttribute("FishingVessel", x.FishingVessel),
                                              new XAttribute("FishingGear", x.FishingGear),
                                              new XAttribute("Enumerator", x.Enumerator),
                                              new XAttribute("SamplingDate", x.SamplingDate.ToString("MMM-dd-yyyy HH:mm"))
                                              ))));


            wf.SaveFileDialog sfd = new wf.SaveFileDialog();
            sfd.Title = "Provide filename to save unrecognized fishing grounds data";
            sfd.Filter = "XML file (*.xml)|*.xml|All files ( *.*)|*.*";
            sfd.FilterIndex = 1;

            if (sfd.ShowDialog() == wf.DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, xml.ToString());
            }
        }

        private void FillRegions()
        {

            foreach (NSAPRegion r in NSAPEntities.NSAPRegionViewModel.GetAllNSAPRegions())
            {
                KeyValuePair<NSAPRegion, string> kv = new KeyValuePair<NSAPRegion, string>(r, r.ShortName);
                cboRegion.Items.Add(kv);
            }
        }
        private async void onButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonUpload":
                    panelUploadChoices.Visibility = Visibility.Visible;
                    FillRegions();
                    break;
                case "buttonUploadNow":
                    List<Entities.Database.FromJson.VesselLanding> vesselLandings = new List<Entities.Database.FromJson.VesselLanding>();
                    foreach (UnrecognizedFishingGround item in dataGrid.Items)
                    {
                        if (item.Selected)
                        {
                            item.FishingGroundCode = _selectedFG.Code;
                            item.VesselLanding.RegionFishingGroundID = _nrfg.RowID;
                            vesselLandings.Add(item.VesselLanding);
                        }

                    }
                    Entities.Database.FromJson.VesselUnloadServerRepository.UploadSubmissionToDB += VesselUnloadServerRepository_UploadSubmissionToDB;
                    await ((ODKResultsWindow)Owner).SetResolvedFishingGroundLandings(vesselLandings);
                    Entities.Database.FromJson.VesselUnloadServerRepository.UploadSubmissionToDB -= VesselUnloadServerRepository_UploadSubmissionToDB;
                    dataGrid.Items.Refresh();
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void VesselUnloadServerRepository_UploadSubmissionToDB(object sender, UploadToDbEventArg e)
        {
            switch (e.Intent)
            {
                case UploadToDBIntent.UnloadFound:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.Value = e.VesselUnloadFoundCount;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = "Please wait...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.SearchingUpdates:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = "Please wait...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case UploadToDBIntent.Cancelled:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.VesselUnloadTotalSavedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressLabel.Content = $"Uploading was cancelled with {e.VesselUnloadTotalSavedCount} submissions";
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _savedCount = e.VesselUnloadTotalSavedCount;
                    break;
                case UploadToDBIntent.EndOfUpload:
                case UploadToDBIntent.EndOfUpdate:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              if (e.Intent == UploadToDBIntent.EndOfUpload)
                              {
                                  progressLabel.Content = $"Finished uploading {e.VesselUnloadTotalSavedCount} submissions";
                              }
                              else
                              {
                                  progressLabel.Content = $"Finished updating {e.VesselUnloadUpdatedCount} submissions";
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _savedCount = e.VesselUnloadTotalSavedCount;


                    
                    break;

                case UploadToDBIntent.StartOfUpload:
                case UploadToDBIntent.StartOfUpdate:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              if (e.Intent == UploadToDBIntent.StartOfUpload)
                              {
                                  progressBar.Maximum = e.VesselUnloadToSaveCount;
                              }
                              else
                              {
                                  progressBar.Maximum = e.VesselUnloadToUpdateCount;
                              }
                              //do what you need to do on UI Thread
                              return null;
                          }), null);
                    break;
                case UploadToDBIntent.UpdateFound:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {
                                progressBar.IsIndeterminate = false;
                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);
                    break;
                case UploadToDBIntent.Updating:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {

                                progressBar.Value = e.VesselUnloadFoundCount;

                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = $"Updating {(int)progressBar.Value} of {(int)progressBar.Maximum} submissions";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;
                case UploadToDBIntent.Uploading:
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {

                                progressBar.Value = e.VesselUnloadSavedCount;

                                //do what you need to do on UI Thread
                                return null;
                            }
                         ), null);

                    progressLabel.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {

                              progressLabel.Content = $"Uploading {(int)progressBar.Value} of {(int)progressBar.Maximum} submissions";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    break;

            }
        }

        private void OnDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void onCheckChanged(object sender, RoutedEventArgs e)
        {
            _selectAll = (bool)((CheckBox)sender).IsChecked;
            foreach (var item in dataGrid.Items)
            {
                ((UnrecognizedFishingGround)item).Selected = _selectAll;
            }
            dataGrid.Items.Refresh();
        }
    }
}
