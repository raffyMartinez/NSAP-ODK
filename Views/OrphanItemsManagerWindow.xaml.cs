﻿using NSAP_ODK.Entities;
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
        public OrphanItemsManagerWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        public void ReplaceChecked()
        {

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
            checkMultipleSp.Visibility = Visibility.Collapsed;
            labelStatus.Content = "";
            RefreshItemsSource();
            switch (NSAPEntity)
            {
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
                    labelTitle.Content = "Manage orphaned landing sites";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site name", Binding = new Binding("LandingSiteName"), IsReadOnly = true });

                    break;
                case NSAPEntity.Enumerator:
                    labelTitle.Content = "Manage orphaned enumerators";

                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Name"), IsReadOnly = true });


                    break;
                case NSAPEntity.FishingGear:
                    labelTitle.Content = "Manage orphaned fishing gears";
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear name", Binding = new Binding("Name"), IsReadOnly = true });
                    break;
            }

            dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Replace", Binding = new Binding("ForReplacement") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.ShortName"), IsReadOnly = true });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA"), IsReadOnly = true });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround"), IsReadOnly = true });
            

            switch(NSAPEntity)
            {

                case NSAPEntity.Enumerator:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfLandings"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of vessel countings", Binding = new Binding("NumberOfVesselCountings"), IsReadOnly = true });
                    break;
                case NSAPEntity.FishSpecies:
                case NSAPEntity.LandingSite:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of landings", Binding = new Binding("NumberOfLandings"), IsReadOnly = true });
                    break;
                case NSAPEntity.FishingGear:
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "# of gear unload", Binding = new Binding("NumberOfUnload"), IsReadOnly = true });
                    break;
            }


            Title = labelTitle.Content.ToString();

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            labelStatus.Content = "";
            _timer.Stop();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            Closing -= OnWindowClosing;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private Task<int> ReplaceOrphanedAsync()
        {
            return Task.Run(() => DoTheReplacement());
        }

        private void ShowProgressWhileReplacing(int counter, string message)
        {

            progressBar.Dispatcher.BeginInvoke
                (
                  DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                  {
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
                case NSAPEntity.FishSpecies:
                    break;
                case NSAPEntity.NonFishSpecies:
                    break;
                case NSAPEntity.FishingGear:
                    foreach(OrphanedFishingGear gear in dataGrid.Items)
                    {
                        if(gear.ForReplacement)
                        {
                            foreach(var unload in gear.GearUnloads)
                            {
                                unload.GearID = ReplacementGear.Code;
                                unload.Gear = ReplacementGear;
                                if(NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(unload))
                                {
                                    _countReplaced++;
                                    ShowProgressWhileReplacing(_countReplaced,$"Updated fishing gear {_countReplaced} of {_countForReplacement}");
                                }

                            }
                        }
                    }
                    break;
                case NSAPEntity.Enumerator:
                    foreach (OrphanedEnumerator orpahn in dataGrid.Items)
                    {
                        if (orpahn.ForReplacement)
                        {
                            foreach (var unload in orpahn.SampledLandings)
                            {
                                unload.NSAPEnumerator = ReplacementEnumerator;
                                unload.NSAPEnumeratorID = ReplacementEnumerator.ID;
                                if (NSAPEntities.VesselUnloadViewModel.UpdateRecordInRepo(unload))
                                {
                                    _countReplaced++;
                                    ShowProgressWhileReplacing(_countReplaced, $"Updated enumerator {_countReplaced} of {_countForReplacement}");

                                }
                            }

                            foreach(var vesselCounting in orpahn.LandingSiteSamplings)
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

                    //MessageBox.Show($"{_countReplaced} vessel unload updated to enumerator {ReplacementEnumerator} with ID {ReplacementEnumerator.ID}");
                    break;

                case NSAPEntity.LandingSite:
                    int counter = 0;
                    foreach (OrphanedLandingSite selectedOrphanedLandingSite in dataGrid.Items)
                    {
                        if (selectedOrphanedLandingSite.ForReplacement)
                        {
                            foreach (var samplingWithOrphanedLandingSite in selectedOrphanedLandingSite.LandingSiteSamplings)
                            {
                                counter++;
                                //search for duplictes that will happen if we update landing site
                                //duplication will happen in landing site, fishing ground and sampling date
                                var sampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(selectedOrphanedLandingSite, ReplacementLandingSite, samplingWithOrphanedLandingSite.SamplingDate);
                                if (sampling != null)
                                {
                                    //if(sampling.UserName!=null && sampling.UserName.Length==0)
                                    //{
                                        sampling.UserName = samplingWithOrphanedLandingSite.UserName;
                                        sampling.DeviceID = samplingWithOrphanedLandingSite.DeviceID;
                                        sampling.DateSubmitted = samplingWithOrphanedLandingSite.DateSubmitted;
                                        sampling.XFormIdentifier = samplingWithOrphanedLandingSite.XFormIdentifier;
                                        sampling.DateAdded = samplingWithOrphanedLandingSite.DateAdded;
                                        sampling.FormVersion = samplingWithOrphanedLandingSite.FormVersion;
                                        sampling.FromExcelDownload = samplingWithOrphanedLandingSite.FromExcelDownload;
                                        sampling.RowID = samplingWithOrphanedLandingSite.RowID;
                                        sampling.EnumeratorText = samplingWithOrphanedLandingSite.EnumeratorText;
                                        sampling.EnumeratorID = samplingWithOrphanedLandingSite.EnumeratorID;
                                        if (NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(sampling))
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
                                           if( NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(samplingWithOrphanedLandingSite))
                                            {

                                            }
                                        }

                                    //}

                                    foreach (GearUnload gu in NSAPEntities.GearUnloadViewModel.GetGearUnloads(samplingWithOrphanedLandingSite))
                                    {
                                        gu.Parent = sampling;
                                        gu.LandingSiteSamplingID = gu.Parent.PK;
                                        if (NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu))
                                        {
                                            var otherGearUnload = NSAPEntities.GearUnloadViewModel.getOtherGearUnload(gearUnload: gu);
                                            if (otherGearUnload != null)
                                            {
                                                otherGearUnload.Boats = gu.Boats;
                                                otherGearUnload.Catch = gu.Catch;
                                                gu.Boats = null;
                                                gu.Catch = null;
                                                if (NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(otherGearUnload))
                                                {
                                                    NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);
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
                            }

                        }
                    }
                    break;
            }


            return _countReplaced;
        }

        private void RefreshItemsSource()
        {
            switch (NSAPEntity)
            {
                case NSAPEntity.FishSpecies:
                    dataGrid.DataContext = NSAPEntities.VesselCatchViewModel.OrphanedFishSpeciesNames();
                    break;
                case NSAPEntity.NonFishSpecies:
                    break;
                case NSAPEntity.LandingSite:
                    dataGrid.DataContext = NSAPEntities.LandingSiteSamplingViewModel.OrphanedLandingSites();

                    break;
                case NSAPEntity.Enumerator:
                    //dataGrid.DataContext = NSAPEntities.NSAPEnumeratorViewModel.OrphanedEnumerators().OrderBy(t=>t.Name);
                    dataGrid.DataContext = NSAPEntities.NSAPEnumeratorViewModel.OrphanedEnumerators();
                    break;
                case NSAPEntity.FishingGear:
                    dataGrid.DataContext = NSAPEntities.GearUnloadViewModel.OrphanedFishingGears();
                    break;
            }

            //if(dataGrid.Items.Count==0)
            //{
            //    MessageBox.Show("There are no orphaned items for now","GPX Manager",MessageBoxButton.OK,MessageBoxImage.Information);
            //    Close();
            //}
        }
        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            labelStatus.Content = "";
            //progressBar.Value = 0;
            bool procced = false;

            switch (((Button)sender).Name)
            {
                case "buttonReplace":
                    _countReplaced = 0;
                    await ReplaceOrphanedAsync();
                    
                    buttonReplace.IsEnabled = false;
                    //DoTheReplacement();
                    //dataGrid.Items.Refresh();
                    RefreshItemsSource();

                    if (dataGrid.Items.Count > 0 && NSAPEntity==NSAPEntity.LandingSite)
                    {
                        if (MessageBox.Show("Remaining orphaned landing sites can be safely deleted from the database\r\n\r\n" +
                                           "Select Yes to safely delete items", "NSAP-ODK Database",
                                           MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            foreach (OrphanedLandingSite item in dataGrid.Items)
                            {
                                foreach (var sampling in item.LandingSiteSamplings)
                                {
                                    if (NSAPEntities.GearUnloadViewModel.GetGearUnloads(sampling).Count == 0)
                                    {
                                        if (NSAPEntities.LandingSiteSamplingViewModel.DeleteRecordFromRepo(sampling))
                                        {

                                        }
                                    }
                                }
                            }

                            RefreshItemsSource();
                        }
                    }

                    _timer.Interval = TimeSpan.FromSeconds(3);
                    _timer.Start();

                    break;
                case "buttonSelectReplacement":
                    int checkCount = 0;
                    string itemToReplace = "";
                    List<string> itemsToReplace = new List<string>();
                    _countForReplacement = 0;

                    var replacementWindow = new SelectionToReplaceOrpanWIndow();
                    replacementWindow.Owner = this;
                    replacementWindow.NSAPEntity = NSAPEntity;



                    foreach (var item in dataGrid.Items)
                    {

                        switch (NSAPEntity)
                        {
                            case NSAPEntity.FishSpecies:
                            
                                if (((OrphanedFishSpeciesName)item).ForReplacement)
                                {
                                    itemToReplace = ((OrphanedFishSpeciesName)item).Name;
                                    itemsToReplace.Add(itemToReplace);
                                    checkCount++;
                                    procced = true;
                                }
                                
                                break;
                            case NSAPEntity.LandingSite:
                                if (((OrphanedLandingSite)item).ForReplacement)
                                {
                                    if (!procced)
                                    {
                                        procced = true;
                                        replacementWindow.LandingSiteSampling = ((OrphanedLandingSite)item).LandingSiteSamplings[0];
                                    }
                                    _countForReplacement += ((OrphanedLandingSite)item).LandingSiteSamplings.Count;

                                }
                                break;
                            case NSAPEntity.FishingGear:

                                if(((OrphanedFishingGear)item).ForReplacement)
                                {
                                    if(!procced)
                                    {
                                        procced = true;
                                        replacementWindow.GearUnload = ((OrphanedFishingGear)item).GearUnloads[0];
                                    }
                                    _countForReplacement += ((OrphanedFishingGear)item).GearUnloads.Count;
                                }
                                break;
                            case NSAPEntity.Enumerator:
                                if (((OrphanedEnumerator)item).ForReplacement)
                                {
                                    if (!procced)
                                    {
                                        procced = true;
                                        if (((OrphanedEnumerator)item).SampledLandings.Count > 0)
                                        {
                                            replacementWindow.LandingSiteSampling = ((OrphanedEnumerator)item).SampledLandings[0].Parent.Parent;
                                        }
                                    }
                                    _countForReplacement += ((OrphanedEnumerator)item).SampledLandings.Count;
                                    _countForReplacement += ((OrphanedEnumerator)item).LandingSiteSamplings.Count;

                                }
                                break;

                        }


                    }

                    if (procced)
                    {
                        if(checkCount==1 )
                        {
                            switch(NSAPEntity)
                            {
                                case NSAPEntity.FishSpecies:
                                    replacementWindow.ItemToReplace = itemToReplace;
                                    break;
                            }
                        }
                        else if(checkCount>1)
                        {
                            switch(NSAPEntity)
                            {
                                case NSAPEntity.FishSpecies:
                                    replacementWindow.ItemsToReplace = itemsToReplace;
                                    break;
                            }
                        }

                        progressBar.Maximum = _countForReplacement;
                        replacementWindow.FillSelection();
                        replacementWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Check at least one item in the table", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "buttonCancel":
                    Close();
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
            switch(chk.Name)
            {
                case "checkMultipleSp":
                    dataGrid.DataContext = NSAPEntities.VesselCatchViewModel.OrphanedFishSpeciesNames(getMultiLine:(bool)chk.IsChecked);
                    buttonFix.IsEnabled = (bool)chk.IsChecked;
                    buttonSelectReplacement.IsEnabled = !(bool)chk.IsChecked;
                    break;
            }
        }
    }
}
