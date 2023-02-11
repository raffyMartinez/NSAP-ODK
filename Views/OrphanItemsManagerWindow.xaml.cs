using NSAP_ODK.Entities;
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
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for OrphanItemsManagerWindow.xaml
    /// </summary>
    public partial class OrphanItemsManagerWindow : Window
    {
        private DispatcherTimer _timer;
        private LandingSite _replacementLandingSite;
        private NSAPEnumerator _replacementEnumerator;
        private Gear _replacementGear;
        private int _countReplaced;
        private int _countForReplacement;
        private FishSpecies _replacementFishSpecies;
        private NotFishSpecies _replacementNotFishSpecies;
        private bool _isMultiline;
        private FishingVessel _fishingVessel;
        private bool _closingDeleteDone;
        private bool _hasReplacedCellChecked;
        private List<OrphanedLandingSite> _olsForDeletion = new List<OrphanedLandingSite>();
        private OrphanedLandingSite _orphanedLandingSite;
        private string _itemToReplace;
        private List<string> _itemsToReplace = new List<string>();
        private int _checkCount = 0;
        private EntityContext _entityContext = new EntityContext();

        public OrphanItemsManagerWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
            dataGrid.LoadingRow += Grid_LoadingRow;
        }

        public void ReplaceChecked()
        {

        }

        public NotFishSpecies ReplacementNotFishSpecies
        {
            get { return _replacementNotFishSpecies; }
            set
            {
                _replacementNotFishSpecies = value;

                buttonReplace.IsEnabled = true;

            }
        }
        public FishSpecies ReplacementFishSpecies
        {
            get { return _replacementFishSpecies; }
            set
            {
                _replacementFishSpecies = value;

                buttonReplace.IsEnabled = true;

            }
        }


        public Gear ReplacementGear
        {
            get { return _replacementGear; }
            set
            {
                _replacementGear = value;
                buttonReplace.IsEnabled = true;
            }
        }

        public FishingVessel FishingVessel
        {
            get { return _fishingVessel; }
            set
            {
                _fishingVessel = value;
                buttonReplace.IsEnabled = true;
            }
        }
        public NSAPEnumerator ReplacementEnumerator
        {
            get { return _replacementEnumerator; }
            set
            {
                _replacementEnumerator = value;
                buttonReplace.IsEnabled = true;
            }
        }
        public LandingSite ReplacementLandingSite
        {
            get { return _replacementLandingSite; }
            set
            {
                _replacementLandingSite = value;
                buttonReplace.IsEnabled = true;
            }
        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            buttonDelete.IsEnabled = false;
            checkMultipleSp.Visibility = Visibility.Collapsed;
            labelStatus.Content = "";
            RefreshItemsSource();
            switch (NSAPEntity)
            {
                case NSAPEntity.SpeciesName:
                    labelTitle.Content = "Manage orphaned species names";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species name", Binding = new Binding("Name"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Taxa", Binding = new Binding("Taxa"), IsReadOnly = true });
                    checkMultipleSp.Visibility = Visibility.Visible;
                    buttonFix.Visibility = Visibility.Visible;
                    checkCheckAll.Visibility = Visibility.Visible;
                    break;
                case NSAPEntity.FishSpecies:
                    labelTitle.Content = "Manage orphaned fish species names";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species name", Binding = new Binding("Name"), IsReadOnly = true });
                    checkMultipleSp.Visibility = Visibility.Visible;
                    buttonFix.Visibility = Visibility.Visible;

                    break;
                case NSAPEntity.NonFishSpecies:
                    labelTitle.Content = "Manage orphaned non-fish species names";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Species name", Binding = new Binding("LandingSiteName"), IsReadOnly = true });
                    break;
                case NSAPEntity.LandingSite:
                    buttonDelete.IsEnabled = true;
                    labelTitle.Content = "Manage orphaned landing sites";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site name", Binding = new Binding("LandingSiteName"), IsReadOnly = true });

                    break;
                case NSAPEntity.Enumerator:
                    buttonDelete.IsEnabled = true;
                    labelTitle.Content = "Manage orphaned enumerators";

                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Name"), IsReadOnly = true });


                    break;
                case NSAPEntity.FishingGear:
                    buttonDelete.IsEnabled = true;
                    labelTitle.Content = "Manage orphaned fishing gears";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear name", Binding = new Binding("Name"), IsReadOnly = true });
                    break;

                case NSAPEntity.FishingVessel:
                    labelTitle.Content = "Manage orphaned fishing vessels";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Vessel name", Binding = new Binding("Name"), IsReadOnly = true });
                    break;
            }


            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Replace", Binding = new Binding("ForReplacement"), });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.ShortName"), IsReadOnly = true });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA"), IsReadOnly = true });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround"), IsReadOnly = true });


            switch (NSAPEntity)
            {
                case NSAPEntity.FishingVessel:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfUnload"), IsReadOnly = true });
                    break;

                case NSAPEntity.Enumerator:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSiteName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfLandings"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel countings", Binding = new Binding("NumberOfVesselCountings"), IsReadOnly = true });
                    break;

                case NSAPEntity.LandingSite:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfVesselLandings"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Can be deleted", Binding = new Binding("CanBeDeletedNow"), IsReadOnly = true });
                    break;
                case NSAPEntity.SpeciesName:
                    ((DataGridTextColumn)dataGrid.Columns.FirstOrDefault(t => t.Header.ToString() == "Region")).Binding = new Binding("Region");
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfLandings"), IsReadOnly = true });
                    break;
                case NSAPEntity.FishingGear:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerators", Binding = new Binding("EnumeratorNameList"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("NumberOfUnload"), IsReadOnly = true });
                    
                    break;
            }


            Title = labelTitle.Content.ToString();

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            NSAPEntities.LandingSiteSamplingViewModel.DeleteOrphanedLandingSiteFromOrphanedItem += LandingSiteSamplingViewModel_DeleteOrphanedLandingSiteFromOrphanedItem;
            NSAPEntities.LandingSiteSamplingViewModel.DeleteVesselUnloadFromOrphanedItem += LandingSiteSamplingViewModel_DeleteVesselUnloadFromOrphanedItem;
            GearUnloadViewModel.DeleteVesselUnloadFromOrphanedItem += GearUnloadViewModel_DeleteVesselUnloadFromOrphanedItem;
        }

        private void LandingSiteSamplingViewModel_DeleteOrphanedLandingSiteFromOrphanedItem(object sender, DeleteLandingSiteSamplingFromOrphanEventArg e)
        {
            switch (e.Intent)
            {
                case "start":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Maximum = e.LandinggSiteSamplingToDeleteTotalCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Preparing to delete {e.LandinggSiteSamplingToDeleteTotalCount} items";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "deleted orphaned landingSiteSampling":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.CountDeleted;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Deleted record {e.CountDeleted} of {progressBar.Maximum} - {e.SamplingDeleted}";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "finished deleting":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = 0;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Finished deleting landing site samplings in orphaned landing sites ";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();

                    break;

            }
        }

        private void GearUnloadViewModel_DeleteVesselUnloadFromOrphanedItem(object sender, DeleteVesselUnloadFromOrphanEventArg e)
        {
            switch (e.Intent)
            {
                case "searching":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.IsIndeterminate = true;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Searching for matching records...";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "start":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.IsIndeterminate = false;
                              progressBar.Maximum = e.VesselUnloadTotalCount;
                              //switch(e.NSAPEntity)
                              //{
                              //    case NSAPEntity.FishingGear:
                              //        break;
                              //    case NSAPEntity.Enumerator:
                              //        break;
                              //}
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Starting to delete {e.VesselUnloadTotalCount} sampled fishing vessel landings";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "unload_deleted":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.DeletedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Deleted fishing vessel landing {e.DeletedCount} of {progressBar.Maximum}";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "done":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.DeletedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Finished deleting {progressBar.Maximum} landings";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();
                    break;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            labelStatus.Content = "";
            _timer.Stop();


        }

        private async void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (NSAPEntity == NSAPEntity.LandingSite && !_closingDeleteDone && _olsForDeletion.Count > 0)
            {
                var result = MessageBox.Show("Delete landing site samplings in orphaned landing sites?", "NSAP-ODK Database", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    await DeleteOrphanedLandingSitesBeforeCLosing(verbose: false);
                    _closingDeleteDone = true;
                    try
                    {
                        Close();
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
            if (!e.Cancel)
            {

                this.SavePlacement();
                Closing -= OnWindowClosing;
            }
            NSAPEntities.LandingSiteSamplingViewModel.DeleteOrphanedLandingSiteFromOrphanedItem -= LandingSiteSamplingViewModel_DeleteOrphanedLandingSiteFromOrphanedItem;
            NSAPEntities.LandingSiteSamplingViewModel.DeleteVesselUnloadFromOrphanedItem -= LandingSiteSamplingViewModel_DeleteVesselUnloadFromOrphanedItem;
            GearUnloadViewModel.DeleteVesselUnloadFromOrphanedItem -= GearUnloadViewModel_DeleteVesselUnloadFromOrphanedItem;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private Task<int> FixMultiLineSpeciesAsync()
        {
            return Task.Run(() => FixMulitlineSpecies());
        }
        private int FixMulitlineSpecies()
        {
            _countReplaced = 0;
            foreach (var item in dataGrid.Items)
            {

                var orphanedSpecies = (OrphanedSpeciesName)item;
                if (orphanedSpecies.ForReplacement)
                {
                    int countReplaced = 0;
                    var list = SpeciesName_Weight.ParseMultiSpeciesRow(orphanedSpecies.Name);
                    if (list.Count > 1)
                    {
                        foreach (VesselUnload vu in orphanedSpecies.SampledLandings)
                        {
                            if (vu.VesselCatchViewModel == null)
                            {
                                vu.VesselCatchViewModel = new VesselCatchViewModel(vu);
                            }
                            if (vu.VesselCatchViewModel.ConvertToIindividualCatches(orphanedSpecies.Name, list) > 0)
                            {
                                countReplaced += orphanedSpecies.SampledLandings.Count;
                            }
                        }
                    }
                    if (countReplaced > 0)
                    {
                        _countReplaced++;
                        ShowProgressWhileReplacing(_countReplaced, $"Fixed multi-species row {_countReplaced} of {_countForReplacement}");
                    }
                }


            }
            return _countReplaced;
        }
        private Task<int> ReplaceOrphanedAsync()
        {
            return Task.Run(() => DoTheReplacement());
        }
        private void ShowProgressWhileSearching()
        {
            progressBar.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      progressBar.IsIndeterminate = true;
                      //do what you need to do on UI Thread
                      return null;
                  }
                 ), null);

            labelStatus.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      labelStatus.Content = "Searching...";

                      //do what you need to do on UI Thread
                      return null;
                  }
                 ), null);
        }

        private void ShowProgressWhileReplacing(int counter, string message, bool isDone = false)
        {
            if (isDone)
            {
                message = "Finished replacing";
                counter = 0;
            }

            progressBar.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      progressBar.IsIndeterminate = false;
                      progressBar.Value = counter;
                      //do what you need to do on UI Thread
                      return null;
                  }
                 ), null);


            labelStatus.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
                      labelStatus.Content = message;

                      //do what you need to do on UI Thread
                      return null;
                  }
                 ), null);
        }



        private int DoTheReplacement()
        {

            switch (NSAPEntity)
            {
                case NSAPEntity.SpeciesName:
                case NSAPEntity.FishSpecies:
                case NSAPEntity.NonFishSpecies:
                    //var G = SpeciesTextToSpeciesConvert.FillDictionary();
                    if (!_isMultiline)
                    {
                        //if not multiline species
                        foreach (OrphanedSpeciesName sp in dataGrid.Items)
                        {
                            ShowProgressWhileSearching();
                            if (sp.ForReplacement)
                            {
                                foreach (var unload in sp.SampledLandings)
                                {
                                    if (unload.VesselCatchViewModel == null)
                                    {
                                        unload.VesselCatchViewModel = new VesselCatchViewModel(unload);
                                    }
                                    foreach (VesselCatch vc in unload.ListVesselCatch)
                                    {
                                        if (vc.SpeciesID == null)
                                        {

                                        }
                                        if (vc.SpeciesID == null && vc.SpeciesText != null && vc.SpeciesText.Trim(new char[] { '\n', ' ' }) == sp.Name)
                                        {
                                            if (_replacementFishSpecies != null)
                                            {
                                                vc.SpeciesID = _replacementFishSpecies.SpeciesCode;
                                                vc.SetTaxa(NSAPEntities.TaxaViewModel.FishTaxa);
                                            }
                                            else if (_replacementNotFishSpecies != null)
                                            {
                                                vc.SpeciesID = _replacementNotFishSpecies.SpeciesID;
                                                vc.SetTaxa(ReplacementNotFishSpecies.Taxa);
                                            }

                                            if (unload.VesselCatchViewModel.UpdateRecordInRepo(vc))
                                            {
                                                //Console.WriteLine(vc.SpeciesID);
                                                _countReplaced++;
                                                ShowProgressWhileReplacing(_countReplaced, $"Updated species names {_countReplaced} of {_countForReplacement}");
                                            }
                                            else
                                            {
                                                vc.SpeciesID = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        _replacementFishSpecies = null;
                        _replacementNotFishSpecies = null;

                    }
                    else
                    {
                        //if multiline then we have to separate into each own line and add to vessel catch

                    }
                    break;
                case NSAPEntity.FishingGear:
                    foreach (OrphanedFishingGear gear in dataGrid.Items)
                    {
                        ShowProgressWhileSearching();
                        if (gear.ForReplacement)
                        {
                            foreach (var unload in NSAPEntities.SummaryItemViewModel.GetGearUnloads(gear.Name))
                            {
                                if (unload.GearID.Length == 0)
                                {
                                    unload.GearID = ReplacementGear.Code;
                                    unload.Gear = ReplacementGear;
                                    if (unload.Parent.GearUnloadViewModel.UpdateRecordInRepo(unload))
                                    {
                                        _countReplaced++;
                                        ShowProgressWhileReplacing(_countReplaced, $"Updated fishing gear {_countReplaced} of {_countForReplacement}");
                                        NSAPEntities.SummaryItemViewModel.UpdateRecordsInRepo(unload);
                                    }
                                }

                            }
                        }
                    }
                    break;
                case NSAPEntity.Enumerator:
                    foreach (OrphanedEnumerator orpahn in dataGrid.Items)
                    {
                        ShowProgressWhileSearching();
                        if (orpahn.ForReplacement)
                        {
                            foreach (var unload in orpahn.SampledLandings)
                            {
                                unload.NSAPEnumerator = ReplacementEnumerator;
                                unload.NSAPEnumeratorID = ReplacementEnumerator.ID;
                                //if (unload.ContainerViewModel.UpdateRecordInRepo(unload) && NSAPEntities.SummaryItemViewModel.UpdateRecordInRepo(unload))
                                //{

                                //    _countReplaced++;
                                //    ShowProgressWhileReplacing(_countReplaced, $"Updated enumerator {_countReplaced} of {_countForReplacement}");

                                //}
                                if (VesselUnloadRepository.UpdateEnumerator(unload) && NSAPEntities.SummaryItemViewModel.UpdateRecordInRepo(unload, updateEnumerators: true))
                                {

                                    _countReplaced++;
                                    ShowProgressWhileReplacing(_countReplaced, $"Updated enumerator {_countReplaced} of {_countForReplacement}");

                                }
                            }

                            if (orpahn.LandingSiteSamplings != null)
                            {
                                foreach (var vesselCounting in orpahn.LandingSiteSamplings)
                                {
                                    vesselCounting.EnumeratorID = ReplacementEnumerator.ID;
                                    if (NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(vesselCounting))
                                    {
                                        _countReplaced++;
                                        ShowProgressWhileReplacing(_countReplaced, $"Updated enumerator {_countReplaced} of {_countForReplacement}");
                                    }
                                }
                            }
                        }
                    }

                    //MessageBox.Show($"{_countReplaced} vessel unload updated to enumerator {ReplacementEnumerator} with ID {ReplacementEnumerator.ID}");
                    break;

                case NSAPEntity.LandingSite:
                    int counter = 0;
                    foreach (OrphanedLandingSite selectedOrphanedLandingSite in dataGrid.Items)
                    {
                        ShowProgressWhileSearching();
                        if (selectedOrphanedLandingSite.ForReplacement)
                        {
                            foreach (var samplingWithOrphanedLandingSite in selectedOrphanedLandingSite.LandingSiteSamplings)
                            {
                                counter++;

                                //search for duplictes that will happen if we update landing site
                                //duplication will happen in landing site, fishing ground and sampling date
                                var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(selectedOrphanedLandingSite, ReplacementLandingSite, samplingWithOrphanedLandingSite.SamplingDate);

                                if (landingSiteSampling != null)
                                {
                                    landingSiteSampling.UserName = samplingWithOrphanedLandingSite.UserName;
                                    landingSiteSampling.DeviceID = samplingWithOrphanedLandingSite.DeviceID;
                                    landingSiteSampling.DateSubmitted = samplingWithOrphanedLandingSite.DateSubmitted;
                                    landingSiteSampling.XFormIdentifier = samplingWithOrphanedLandingSite.XFormIdentifier;
                                    landingSiteSampling.DateAdded = samplingWithOrphanedLandingSite.DateAdded;
                                    landingSiteSampling.FormVersion = samplingWithOrphanedLandingSite.FormVersion;
                                    landingSiteSampling.FromExcelDownload = samplingWithOrphanedLandingSite.FromExcelDownload;
                                    landingSiteSampling.RowID = samplingWithOrphanedLandingSite.RowID;
                                    landingSiteSampling.EnumeratorText = samplingWithOrphanedLandingSite.EnumeratorText;
                                    landingSiteSampling.EnumeratorID = samplingWithOrphanedLandingSite.EnumeratorID;
                                    if (NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(landingSiteSampling))
                                    {
                                        samplingWithOrphanedLandingSite.UserName = "";
                                        samplingWithOrphanedLandingSite.DeviceID = "";
                                        samplingWithOrphanedLandingSite.DateSubmitted = null;
                                        samplingWithOrphanedLandingSite.XFormIdentifier = "";
                                        samplingWithOrphanedLandingSite.DateAdded = null;
                                        samplingWithOrphanedLandingSite.FormVersion = "";
                                        samplingWithOrphanedLandingSite.FromExcelDownload = false;
                                        samplingWithOrphanedLandingSite.RowID = "";
                                        samplingWithOrphanedLandingSite.EnumeratorText = "";
                                        samplingWithOrphanedLandingSite.EnumeratorID = null;
                                        samplingWithOrphanedLandingSite.Remarks = "orphaned landing site, could be removed";
                                        if (NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(samplingWithOrphanedLandingSite))
                                        {

                                        }
                                    }


                                    foreach (GearUnload gu in samplingWithOrphanedLandingSite.GearUnloadViewModel.GetGearUnloads())
                                    {
                                        gu.Parent = landingSiteSampling;
                                        gu.LandingSiteSamplingID = landingSiteSampling.PK;
                                        if (samplingWithOrphanedLandingSite.GearUnloadViewModel.UpdateRecordInRepo(gu))
                                        {
                                            NSAPEntities.SummaryItemViewModel.UpdateRecordsInRepo(gu);
                                            var otherGearUnload = samplingWithOrphanedLandingSite.GearUnloadViewModel.getOtherGearUnload(gearUnload: gu);
                                            if (otherGearUnload != null)
                                            {
                                                otherGearUnload.Boats = gu.Boats;
                                                otherGearUnload.Catch = gu.Catch;
                                                gu.Boats = null;
                                                gu.Catch = null;
                                                //if (NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(otherGearUnload))
                                                if (landingSiteSampling.GearUnloadViewModel.UpdateRecordInRepo(otherGearUnload))
                                                {
                                                    landingSiteSampling.GearUnloadViewModel.UpdateRecordInRepo(gu);
                                                    //NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);
                                                }
                                            }
                                            ShowProgressWhileReplacing(_countReplaced, $"Updated landing site {_countReplaced} of {_countForReplacement}");
                                        }


                                    }
                                    _countReplaced++;

                                }
                                else
                                {
                                    samplingWithOrphanedLandingSite.LandingSiteID = ReplacementLandingSite.LandingSiteID;
                                    if (NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(samplingWithOrphanedLandingSite))
                                    {
                                        _countReplaced++;
                                        ShowProgressWhileReplacing(_countReplaced, $"Updated landing site {_countReplaced} of {_countForReplacement}");
                                    }
                                }

                                NSAPEntities.SummaryItemViewModel.UpdateRecordsInRepo(selectedOrphanedLandingSite.LandingSiteName, ReplacementLandingSite.LandingSiteID);
                            }
                            _olsForDeletion.Add(selectedOrphanedLandingSite);
                        }
                    }
                    break;
            }

            ShowProgressWhileReplacing(0, "done", true);
            return _countReplaced;
        }


        private void RefreshItemsSource()
        {
            switch (NSAPEntity)
            {
                //case NSAPEntity.FishSpecies:
                case NSAPEntity.SpeciesName:
                    //dataGrid.DataContext = NSAPEntities.VesselCatchViewModel.OrphanedSpeciesNames();
                    dataGrid.DataContext = VesselCatchViewModel.OrphanedSpeciesNamesStatic();
                    break;
                case NSAPEntity.NonFishSpecies:
                    break;
                case NSAPEntity.LandingSite:
                    //dataGrid.DataContext = NSAPEntities.LandingSiteSamplingViewModel.OrphanedLandingSites().OrderBy(t => t.LandingSiteName.Trim()).ToList();
                    //dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.GetOrphanedLandingSites().OrderBy(t => t.LandingSiteName.Trim()).ToList();
                    dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedLandingSites.OrderBy(t => t.LandingSiteName).ToList();
                    break;
                case NSAPEntity.Enumerator:
                    //dataGrid.DataContext = NSAPEntities.NSAPEnumeratorViewModel.OrphanedEnumerators().OrderBy(t=>t.Name);

                    //dataGrid.DataContext = NSAPEntities.NSAPEnumeratorViewModel.OrphanedEnumerators();
                    //dataGrid.DataContext = await NSAPEntities.SummaryItemViewModel.GetOrphanedEnumeratorsAync();
                    dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedEnumerators.OrderBy(t => t.Name).ToList();
                    break;
                case NSAPEntity.FishingGear:
                    //dataGrid.DataContext = NSAPEntities.GearUnloadViewModel.OrphanedFishingGears();
                    dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedFishingGears.OrderBy(t => t.Name).ToList();
                    break;
                case NSAPEntity.FishingVessel:
                    dataGrid.DataContext = NSAPEntities.FishingVesselViewModel.OrphanedFishingVesseks();
                    break;
            }

        }

        private bool GetEntityContext(SelectionToReplaceOrpanWIndow replacementWindow = null)
        {
            //EntityContext entityContext = new EntityContext();
            bool proceed = false;
            foreach (var item in dataGrid.Items)
            {
                switch (NSAPEntity)
                {

                    case NSAPEntity.FishingVessel:
                        if (((OrphanedFishingVessel)item).ForReplacement)
                        {
                            if (!proceed)
                            {
                                proceed = true;
                                if (replacementWindow != null && ((OrphanedFishingVessel)item).VesselUnloads.Count > 0)
                                {
                                    replacementWindow.VesselUnload = ((OrphanedFishingVessel)item).VesselUnloads[0];
                                }
                            }
                            _countForReplacement += ((OrphanedFishingVessel)item).VesselUnloads.Count;
                        }
                        break;
                    case NSAPEntity.SpeciesName:
                        if (((OrphanedSpeciesName)item).ForReplacement)
                        {

                            _itemToReplace = ((OrphanedSpeciesName)item).Name;
                            _itemsToReplace.Add(_itemToReplace);
                            _checkCount++;
                            proceed = true;

                            _countForReplacement += ((OrphanedSpeciesName)item).SampledLandings.Count;
                        }
                        break;
                    case NSAPEntity.LandingSite:
                        if (((OrphanedLandingSite)item).ForReplacement)
                        {
                            if (!proceed)
                            {
                                proceed = true;
                                if (replacementWindow != null)
                                {
                                    replacementWindow.LandingSiteSampling = ((OrphanedLandingSite)item).LandingSiteSamplings[0];
                                }

                                _entityContext.Region = ((OrphanedLandingSite)item).Region;
                                _entityContext.FMA = ((OrphanedLandingSite)item).FMA;
                                _entityContext.FishingGround = ((OrphanedLandingSite)item).FishingGround;
                                _entityContext.NSAPEntity = NSAPEntity.NSAPRegionFMAFishingGround;
                            }
                            _countForReplacement += ((OrphanedLandingSite)item).LandingSiteSamplings.Count;

                            //_entityContext.Region = ((OrphanedLandingSite)item).Region;
                            //_entityContext.FMA = ((OrphanedLandingSite)item).FMA;
                            //_entityContext.FishingGround = ((OrphanedLandingSite)item).FishingGround;
                            //_entityContext.NSAPEntity = NSAPEntity.NSAPRegionFMAFishingGround;
                        }
                        break;
                    case NSAPEntity.FishingGear:

                        if (((OrphanedFishingGear)item).ForReplacement)
                        {
                            if (!proceed)
                            {
                                proceed = true;
                                if (replacementWindow != null)
                                {
                                    replacementWindow.GearUnload = ((OrphanedFishingGear)item).GearUnloads[0];
                                }
                            }
                            _countForReplacement += ((OrphanedFishingGear)item).GearUnloads.Count;

                            _entityContext.Region = ((OrphanedFishingGear)item).Region;
                            _entityContext.FMA = ((OrphanedFishingGear)item).FMA;
                            _entityContext.FishingGround = ((OrphanedFishingGear)item).FishingGround;
                            _entityContext.NSAPEntity = NSAPEntity.NSAPRegion;
                        }
                        break;
                    case NSAPEntity.Enumerator:
                        if (((OrphanedEnumerator)item).ForReplacement)
                        {
                            if (!proceed)
                            {
                                proceed = true;
                                if (replacementWindow != null && ((OrphanedEnumerator)item).SampledLandings.Count > 0)
                                {
                                    replacementWindow.NSAPRegion = ((OrphanedEnumerator)item).Region;
                                }
                            }
                            _countForReplacement += ((OrphanedEnumerator)item).SampledLandings.Count;
                            if (((OrphanedEnumerator)item).LandingSiteSamplings != null)
                            {
                                _countForReplacement += ((OrphanedEnumerator)item).LandingSiteSamplings.Count;
                            }

                            _entityContext.Region = ((OrphanedEnumerator)item).Region;
                            _entityContext.FMA = ((OrphanedEnumerator)item).FMA;
                            _entityContext.FishingGround = ((OrphanedEnumerator)item).FishingGround;
                            _entityContext.NSAPEntity = NSAPEntity.NSAPRegion;

                        }
                        break;

                }
            }
            return proceed;

        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _itemsToReplace = new List<string>();
            labelStatus.Content = "";
            //progressBar.Value = 0;
            bool proceed = false;
            string btnName = ((Button)sender).Name;
            switch (btnName)
            {
                //case "buttonDelete":
                //    if(_countForReplacement>0)
                //    {

                //    }
                //    else
                //    {
                //        MessageBox.Show("Check at least one item in the table", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                //    }
                //    break;
                case "buttonReplace":
                    _countReplaced = 0;
                    await ReplaceOrphanedAsync();

                    buttonReplace.IsEnabled = false;
                    //DoTheReplacement();
                    //dataGrid.Items.Refresh();

                    await NSAPEntities.SummaryItemViewModel.SetOrphanedEntityAsync(NSAPEntity);
                    RefreshItemsSource();



                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();

                    break;
                case "buttonDelete":
                    int deletedCount = 0;
                    int countLandingSiteSamplingsInOrphanedLandingSite = 0;
                    if (NSAPEntity == NSAPEntity.LandingSite && !GridHasSeletedCheckedItems() && OrphanLandingSiteGridHasCanDeleteItems())
                    {

                        var result = MessageBox.Show("Delete orphaned landing sites marked \"Can be deleted\"?", "NSAP-ODK Database", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            List<OrphanedLandingSite> orphanedLandingSites = new List<OrphanedLandingSite>();
                            foreach (var item in dataGrid.Items)
                            {
                                if (((OrphanedLandingSite)item).CanBeDeletedNow)
                                {
                                    orphanedLandingSites.Add((OrphanedLandingSite)item);
                                    countLandingSiteSamplingsInOrphanedLandingSite += ((OrphanedLandingSite)item).LandingSiteSamplings.Count;
                                }
                            }

                            NSAPEntities.LandingSiteSamplingViewModel.CountLandingSiteSamplingsInOrphanedLandingSiteForDelete = countLandingSiteSamplingsInOrphanedLandingSite;
                            foreach (OrphanedLandingSite item in orphanedLandingSites)
                            {
                                foreach (var sampling in item.LandingSiteSamplings)
                                {
                                    if (await NSAPEntities.LandingSiteSamplingViewModel.DeleteRecordFromRepoAsync(sampling))
                                    {
                                        await NSAPEntities.SummaryItemViewModel.DeleteOrphanedLandingSiteAsync(sampling.LandingSiteName);
                                        deletedCount++;
                                    }
                                }
                            }
                            await NSAPEntities.SummaryItemViewModel.SetOrphanedEntityAsync(NSAPEntity.LandingSite);
                            dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedLandingSites.OrderBy(t => t.LandingSiteName).ToList();
                        }
                    }
                    else
                    {
                        if (GetEntityContext())
                        {
                            switch (NSAPEntity)
                            {
                                case NSAPEntity.LandingSite:

                                    List<OrphanedLandingSite> orphanedLandingSites = new List<OrphanedLandingSite>();
                                    foreach (var item in dataGrid.Items)
                                    {
                                        if (((OrphanedLandingSite)item).ForReplacement)
                                        {
                                            orphanedLandingSites.Add((OrphanedLandingSite)item);
                                        }
                                    }
                                    if (orphanedLandingSites.Count > 0 &&
                                        MessageBox.Show("Delete selected orphaned landing sites?", "NSAP-ODK Database", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes &&
                                        await NSAPEntities.LandingSiteSamplingViewModel.DeleteOrphanedLandingSites(orphanedLandingSites))
                                    {

                                        dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedLandingSites.OrderBy(t => t.LandingSiteName).ToList();

                                    }

                                    break;
                                case NSAPEntity.FishingGear:
                                    List<OrphanedFishingGear> orphanedFishingGears = new List<OrphanedFishingGear>();
                                    foreach (var item in dataGrid.Items)
                                    {

                                        if (((OrphanedFishingGear)item).ForReplacement)
                                        {
                                            orphanedFishingGears.Add((OrphanedFishingGear)item);
                                        }
                                    }

                                    if (orphanedFishingGears.Count > 0 &&
                                        MessageBox.Show("Delete selected orphaned fishing gears?", "NSAP-ODK Database", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes &&
                                        await GearUnloadViewModel.DeleteVesselUnloads(orphanedFishingGears))
                                    {
                                        dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedFishingGears.OrderBy(t => t.Name).ToList();
                                    }
                                    break;
                                case NSAPEntity.FishingGround:
                                    break;
                                case NSAPEntity.Enumerator:
                                    List<OrphanedEnumerator> orphanedEnumerators = new List<OrphanedEnumerator>();
                                    foreach (var item in dataGrid.Items)
                                    {
                                        if (((OrphanedEnumerator)item).ForReplacement)
                                        {
                                            orphanedEnumerators.Add((OrphanedEnumerator)item);
                                        }
                                    }
                                    //NSAPEntities.LandingSiteSamplingViewModel.DeleteOrphanedEnumerators(orphanedEnumerators);

                                    if (orphanedEnumerators.Count > 0 &&
                                        MessageBox.Show("Delete selected orphaned enumerators?", "NSAP-ODK Database", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes &&
                                        await GearUnloadViewModel.DeleteVesselUnloads(orphanedEnumerators))
                                    {
                                        dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedEnumerators.OrderBy(t => t.Name).ToList();
                                    }
                                    break;
                            }

                        }
                        else
                        {
                            MessageBox.Show("Check at least one item in the table", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case "buttonSelectReplacement":
                    //case "buttonDelete":
                    int checkCount = 0;
                    //string itemToReplace = "";
                    //List<string> itemsToReplace = new List<string>();
                    _countForReplacement = 0;

                    var replacementWindow = new SelectionToReplaceOrpanWIndow();
                    replacementWindow.Owner = this;
                    replacementWindow.NSAPEntity = NSAPEntity;

                    //EntityContext entityContext = new EntityContext();

                    proceed = GetEntityContext(replacementWindow);
                    //foreach (var item in dataGrid.Items)
                    //{

                    //    switch (NSAPEntity)
                    //    {

                    //        case NSAPEntity.FishingVessel:
                    //            if (((OrphanedFishingVessel)item).ForReplacement)
                    //            {
                    //                if (!procced)
                    //                {
                    //                    procced = true;
                    //                    if (((OrphanedFishingVessel)item).VesselUnloads.Count > 0)
                    //                    {
                    //                        replacementWindow.VesselUnload = ((OrphanedFishingVessel)item).VesselUnloads[0];
                    //                    }
                    //                }
                    //                _countForReplacement += ((OrphanedFishingVessel)item).VesselUnloads.Count;
                    //            }
                    //            break;
                    //        case NSAPEntity.SpeciesName:
                    //            if (((OrphanedSpeciesName)item).ForReplacement)
                    //            {

                    //                itemToReplace = ((OrphanedSpeciesName)item).Name;
                    //                itemsToReplace.Add(itemToReplace);
                    //                checkCount++;
                    //                procced = true;

                    //                _countForReplacement += ((OrphanedSpeciesName)item).SampledLandings.Count;
                    //            }
                    //            break;
                    //        case NSAPEntity.LandingSite:
                    //            if (((OrphanedLandingSite)item).ForReplacement)
                    //            {
                    //                if (!procced)
                    //                {
                    //                    procced = true;
                    //                    replacementWindow.LandingSiteSampling = ((OrphanedLandingSite)item).LandingSiteSamplings[0];
                    //                }
                    //                _countForReplacement += ((OrphanedLandingSite)item).LandingSiteSamplings.Count;

                    //                entityContext.Region = ((OrphanedLandingSite)item).Region;
                    //                entityContext.FMA = ((OrphanedLandingSite)item).FMA;
                    //                entityContext.FishingGround = ((OrphanedLandingSite)item).FishingGround;
                    //                entityContext.NSAPEntity = NSAPEntity.NSAPRegionFMAFishingGround;
                    //            }
                    //            break;
                    //        case NSAPEntity.FishingGear:

                    //            if (((OrphanedFishingGear)item).ForReplacement)
                    //            {
                    //                if (!procced)
                    //                {
                    //                    procced = true;
                    //                    replacementWindow.GearUnload = ((OrphanedFishingGear)item).GearUnloads[0];
                    //                }
                    //                _countForReplacement += ((OrphanedFishingGear)item).GearUnloads.Count;

                    //                entityContext.Region = ((OrphanedFishingGear)item).Region;
                    //                entityContext.FMA = ((OrphanedFishingGear)item).FMA;
                    //                entityContext.FishingGround = ((OrphanedFishingGear)item).FishingGround;
                    //                entityContext.NSAPEntity = NSAPEntity.NSAPRegion;
                    //            }
                    //            break;
                    //        case NSAPEntity.Enumerator:
                    //            if (((OrphanedEnumerator)item).ForReplacement)
                    //            {
                    //                if (!procced)
                    //                {
                    //                    procced = true;
                    //                    if (((OrphanedEnumerator)item).SampledLandings.Count > 0)
                    //                    {
                    //                        replacementWindow.NSAPRegion = ((OrphanedEnumerator)item).Region;
                    //                    }
                    //                }
                    //                _countForReplacement += ((OrphanedEnumerator)item).SampledLandings.Count;
                    //                if (((OrphanedEnumerator)item).LandingSiteSamplings != null)
                    //                {
                    //                    _countForReplacement += ((OrphanedEnumerator)item).LandingSiteSamplings.Count;
                    //                }

                    //                entityContext.Region = ((OrphanedEnumerator)item).Region;
                    //                entityContext.FMA = ((OrphanedEnumerator)item).FMA;
                    //                entityContext.FishingGround = ((OrphanedEnumerator)item).FishingGround;
                    //                entityContext.NSAPEntity = NSAPEntity.NSAPRegion;

                    //            }
                    //            break;

                    //    }
                    //}

                    if (proceed)
                    {
                        //if (btnName == "buttonDelete")
                        //{

                        //    switch (NSAPEntity)
                        //    {
                        //        case NSAPEntity.LandingSite:

                        //            List<OrphanedLandingSite> orphanedLandingSites = new List<OrphanedLandingSite>();
                        //            foreach (var item in dataGrid.Items)
                        //            {

                        //                if (((OrphanedLandingSite)item).ForReplacement)
                        //                {
                        //                    orphanedLandingSites.Add((OrphanedLandingSite)item);
                        //                }
                        //            }
                        //            if (await NSAPEntities.LandingSiteSamplingViewModel.DeleteOrphanedLandingSites(orphanedLandingSites))
                        //            {
                        //                dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedLandingSites.OrderBy(t => t.LandingSiteName).ToList();
                        //            }

                        //            break;
                        //        case NSAPEntity.FishingGear:
                        //            List<OrphanedFishingGear> orphanedFishingGears = new List<OrphanedFishingGear>();
                        //            foreach (var item in dataGrid.Items)
                        //            {

                        //                if (((OrphanedFishingGear)item).ForReplacement)
                        //                {
                        //                    orphanedFishingGears.Add((OrphanedFishingGear)item);
                        //                }
                        //            }

                        //            if (await GearUnloadViewModel.DeleteVesselUnloads(orphanedFishingGears))
                        //            {
                        //                dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedFishingGears.OrderBy(t => t.Name).ToList();
                        //            }
                        //            break;
                        //        case NSAPEntity.FishingGround:
                        //            break;
                        //        case NSAPEntity.Enumerator:
                        //            List<OrphanedEnumerator> orphanedEnumerators = new List<OrphanedEnumerator>();
                        //            foreach (var item in dataGrid.Items)
                        //            {

                        //                if (((OrphanedEnumerator)item).ForReplacement)
                        //                {
                        //                    orphanedEnumerators.Add((OrphanedEnumerator)item);
                        //                }
                        //            }
                        //            //NSAPEntities.LandingSiteSamplingViewModel.DeleteOrphanedEnumerators(orphanedEnumerators);
                        //            if (await GearUnloadViewModel.DeleteVesselUnloads(orphanedEnumerators))
                        //            {
                        //                dataGrid.DataContext = NSAPEntities.SummaryItemViewModel.OrphanedEnumerators.OrderBy(t => t.Name).ToList();
                        //            }
                        //            break;
                        //    }

                        //}
                        //else
                        //{
                        if (checkCount == 1)
                        {
                            switch (NSAPEntity)
                            {
                                case NSAPEntity.SpeciesName:
                                    replacementWindow.ItemToReplace = _itemToReplace;
                                    break;
                            }
                        }
                        else if (checkCount > 1)
                        {
                            switch (NSAPEntity)
                            {
                                case NSAPEntity.SpeciesName:
                                case NSAPEntity.FishSpecies:
                                    replacementWindow.ItemsToReplace = _itemsToReplace;
                                    break;
                            }
                        }

                        progressBar.Maximum = _countForReplacement;

                        replacementWindow.EntityContext = _entityContext;
                        replacementWindow.FillSelection();
                        if (!(bool)replacementWindow.ShowDialog())
                        {
                            foreach (var item in dataGrid.Items)
                            {
                                switch (NSAPEntity)
                                {
                                    case NSAPEntity.FishingVessel:
                                        ((OrphanedFishingVessel)item).ForReplacement = false;
                                        break;
                                    case NSAPEntity.SpeciesName:
                                        ((OrphanedSpeciesName)item).ForReplacement = false;
                                        break;
                                    case NSAPEntity.LandingSite:
                                        ((OrphanedLandingSite)item).ForReplacement = false;
                                        break;
                                    case NSAPEntity.FishingGear:
                                        ((OrphanedFishingGear)item).ForReplacement = false;
                                        break;
                                    case NSAPEntity.Enumerator:
                                        ((OrphanedEnumerator)item).ForReplacement = false;
                                        break;
                                }
                            }
                        }
                        dataGrid.Items.Refresh();
                    }
                    //}

                    if (!proceed)
                    {
                        MessageBox.Show("Check at least one item in the table", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "buttonFix":
                    if (NSAPEntity == NSAPEntity.SpeciesName)
                    {

                        _countForReplacement = 0;
                        foreach (var item in dataGrid.Items)
                        {
                            if (((OrphanedSpeciesName)item).ForReplacement)
                            {
                                _countForReplacement += ((OrphanedSpeciesName)item).SampledLandings.Count;
                            }
                        }

                        progressBar.Maximum = _countForReplacement;
                        await FixMultiLineSpeciesAsync();


                        _timer.Interval = TimeSpan.FromSeconds(3);
                        _timer.Start();
                        //dataGrid.DataContext = NSAPEntities.VesselCatchViewModel.OrphanedSpeciesNames(getMultiLine: (bool)checkMultipleSp.IsChecked);
                        dataGrid.DataContext = VesselCatchViewModel.OrphanedSpeciesNamesStatic(getMultiLine: (bool)checkMultipleSp.IsChecked);
                    }
                    break;
                case "buttonCancel":
                    await DeleteOrphanedLandingSitesBeforeCLosing();
                    Close();
                    break;
            }
        }
        private async Task DeleteOrphanedLandingSitesBeforeCLosing(bool verbose = true)
        {
            bool proceed = false;
            int deletedCount = 0;
            int forDeletionCount = 0;
            if (!_closingDeleteDone && _countReplaced > 0 && NSAPEntity == NSAPEntity.LandingSite && _olsForDeletion.Count > 0)
            {
                if (verbose && MessageBox.Show("Remaining orphaned landing sites can be safely deleted from the database\r\n\r\n" +
                                   "Select Yes to safely delete items", "NSAP-ODK Database",
                                   MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    proceed = true;
                }
                else
                {
                    proceed = !verbose;
                }
            }

            if (proceed)
            {
                forDeletionCount = _olsForDeletion.Sum(t => t.LandingSiteSamplings.Count);
                NSAPEntities.LandingSiteSamplingViewModel.CountLandingSiteSamplingsInOrphanedLandingSiteForDelete = forDeletionCount;
                foreach (OrphanedLandingSite item in _olsForDeletion)
                {
                    foreach (var sampling in item.LandingSiteSamplings)
                    {
                        if (sampling.GearUnloadViewModel.GetGearUnloads(sampling).Count == 0)
                        {
                            if (await NSAPEntities.LandingSiteSamplingViewModel.DeleteRecordFromRepoAsync(sampling))
                            {
                                deletedCount++;
                            }
                        }
                    }
                }

                RefreshItemsSource();
                _closingDeleteDone = true;
                if (deletedCount > 0 && verbose)
                {
                    MessageBox.Show($"Succesfully deleteted {deletedCount} sampling days with orphaned landing sites", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }


        }

        private bool OrphanLandingSiteGridHasCanDeleteItems()
        {
            bool hasCanDeleteOrphan = false;
            foreach (var item in dataGrid.Items)
            {
                switch (NSAPEntity)
                {
                    case NSAPEntity.LandingSite:
                        if (((OrphanedLandingSite)item).CanBeDeletedNow)
                        {
                            hasCanDeleteOrphan = true;
                            break;
                        }
                        break;
                }
            }
            return hasCanDeleteOrphan;
        }
        private bool GridHasSeletedCheckedItems()
        {
            bool hasSelectedItem = false;
            foreach (var item in dataGrid.Items)
            {
                switch (NSAPEntity)
                {
                    case NSAPEntity.LandingSite:
                        if (((OrphanedLandingSite)item).ForReplacement)
                        {
                            hasSelectedItem = true;
                            break;
                        }
                        break;
                }
            }
            return hasSelectedItem;
        }
        private void LandingSiteSamplingViewModel_DeleteVesselUnloadFromOrphanedItem(object sender, DeleteVesselUnloadFromOrphanEventArg e)
        {
            switch (e.Intent)
            {
                case "start":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Maximum = e.VesselUnloadTotalCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Starting to delete {e.VesselUnloadTotalCount} sampled fishing vessel landings";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "unload_deleted":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.DeletedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Deleted fishing vessel landing {e.DeletedCount} of {progressBar.Maximum}";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    break;
                case "done":
                    progressBar.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              progressBar.Value = e.DeletedCount;
                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);

                    labelStatus.Dispatcher.BeginInvoke
                        (
                          DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                          {
                              labelStatus.Content = $"Finished deleting {progressBar.Maximum} fishing vessel landings";

                              //do what you need to do on UI Thread
                              return null;
                          }
                         ), null);
                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();
                    break;
            }
        }

        public NSAPEntity NSAPEntity { get; set; }
        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void OnCheckChanged(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            switch (chk.Name)
            {
                case "checkMultipleSp":
                    _isMultiline = (bool)chk.IsChecked;
                    dataGrid.DataContext = VesselCatchViewModel.OrphanedSpeciesNamesStatic(getMultiLine: (bool)chk.IsChecked);
                    buttonFix.IsEnabled = (bool)chk.IsChecked;
                    checkCheckAll.IsEnabled = buttonFix.IsEnabled;
                    buttonSelectReplacement.IsEnabled = !(bool)chk.IsChecked;
                    break;

                case "checkCheckAll":
                    if (NSAPEntity == NSAPEntity.SpeciesName && (bool)checkCheckAll.IsEnabled)
                    {
                        foreach (OrphanedSpeciesName item in dataGrid.Items)
                        {
                            item.ForReplacement = (bool)checkCheckAll.IsChecked;
                        }
                    }
                    dataGrid.Items.Refresh();
                    break;
            }
        }

        private void OnDataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var dg = (DataGrid)sender;
            menuCheckState.IsEnabled = dg.SelectedCells.Count > 1;
            if (dg.SelectedCells.Count == 1 && NSAPEntity == NSAPEntity.LandingSite)
            {
                _orphanedLandingSite = (OrphanedLandingSite)dg.SelectedCells[0].Item;
                if (dg.ContextMenu.Items.Count == 1)
                {

                    var menuItem = new MenuItem { Name = "menuOrphanedLandingSiteEnumerator", Header = "Show landing site of enumerator", Tag = "enumerator_landing_site" };
                    menuItem.Click += onContextMenuClicked;
                    dg.ContextMenu.Items.Add(menuItem);
                }
            }

            else if (dg.SelectedCells.Count > 1)
            {
                var sc = dg.SelectedCells[0];
                switch (NSAPEntity)
                {
                    case NSAPEntity.SpeciesName:
                        if (((OrphanedSpeciesName)sc.Item).ForReplacement)
                        {
                            menuCheckState.Header = "Uncheck selected";
                        }
                        else
                        {
                            menuCheckState.Header = "Check selected";
                        }
                        break;
                    case NSAPEntity.Enumerator:
                        if (((OrphanedEnumerator)sc.Item).ForReplacement)
                        {
                            menuCheckState.Header = "Uncheck selected";
                        }
                        else
                        {
                            menuCheckState.Header = "Check selected";
                        }
                        break;
                    case NSAPEntity.LandingSite:
                        if (((OrphanedLandingSite)sc.Item).ForReplacement)
                        {
                            menuCheckState.Header = "Uncheck selected";
                        }
                        else
                        {
                            menuCheckState.Header = "Check selected";
                        }
                        break;
                    case NSAPEntity.FishingGear:
                        if (((OrphanedFishingGear)sc.Item).ForReplacement)
                        {
                            menuCheckState.Header = "Uncheck selected";
                        }
                        else
                        {
                            menuCheckState.Header = "Check selected";
                        }
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void onContextMenuClicked(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Tag.ToString())
            {
                case "enumerator_landing_site":
                    EnumeratorsAndLandingSitesWindow elsw = EnumeratorsAndLandingSitesWindow.GetInstance();
                    elsw.OrphanedLandingSite = _orphanedLandingSite;
                    elsw.Owner = this;
                    if (elsw.Visibility == Visibility.Visible)
                    {
                        elsw.BringIntoView();
                    }
                    else
                    {
                        elsw.ShowDialog();
                    }

                    break;
                case "check_state":
                    if (menuCheckState.Header.ToString() == "Check selected")
                    {
                        switch (NSAPEntity)
                        {
                            case NSAPEntity.SpeciesName:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedSpeciesName)sc.Item).ForReplacement = true;
                                }
                                break;
                            case NSAPEntity.Enumerator:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedEnumerator)sc.Item).ForReplacement = true;
                                }
                                break;
                            case NSAPEntity.LandingSite:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedLandingSite)sc.Item).ForReplacement = true;
                                }
                                break;
                            case NSAPEntity.FishingGear:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedFishingGear)sc.Item).ForReplacement = true;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (NSAPEntity)
                        {
                            case NSAPEntity.SpeciesName:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedSpeciesName)sc.Item).ForReplacement = false;
                                }
                                break;
                            case NSAPEntity.Enumerator:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedEnumerator)sc.Item).ForReplacement = false;
                                }
                                break;
                            case NSAPEntity.LandingSite:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedLandingSite)sc.Item).ForReplacement = false;
                                }
                                break;
                            case NSAPEntity.FishingGear:
                                for (int x = 0; x < dataGrid.SelectedCells.Count; x++)
                                {
                                    var sc = dataGrid.SelectedCells[x];
                                    ((OrphanedFishingGear)sc.Item).ForReplacement = false;
                                }
                                break;
                        }
                    }
                    break;
            }
            dataGrid.Items.Refresh();
        }
    }
}
