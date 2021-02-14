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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for OrphanItemsManagerWindow.xaml
    /// </summary>
    public partial class OrphanItemsManagerWindow : Window
    {
        private LandingSite _replacementLandingSite;
        public OrphanItemsManagerWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        public void ReplaceChecked()
        {

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
            switch (NSAPEntity)
            {
                case NSAPEntity.LandingSite:
                    labelTitle.Content = "Manage orphaned landing sites";
                    RefreshItemsSource();
                    //dataGrid.DataContext = NSAPEntities.LandingSiteSamplingViewModel.OrphanedLandingSites();

                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site name", Binding = new Binding("LandingSiteName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Replace", Binding = new Binding("ForReplacement") });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Region", Binding = new Binding("Region.ShortName"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround"), IsReadOnly = true });
                    dataGrid.Columns.Add(new DataGridTextColumn { Header = "Number of landings", Binding = new Binding("NumberOfLandings"), IsReadOnly = true });
                    break;
                case NSAPEntity.Enumerator:
                    labelTitle.Content = "Manage orphaned enumerators";
                    break;
                case NSAPEntity.FishingGear:
                    labelTitle.Content = "Manage orphaned fishing gears";
                    break;
            }

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

        private void DoTheReplacement()
        {
            switch (NSAPEntity)
            {
                case NSAPEntity.LandingSite:
                    foreach (OrphanedLandingSite selectedOrphanedLandingSite in dataGrid.Items)
                    {
                        if (selectedOrphanedLandingSite.ForReplacement)
                        {
                            foreach (var samplingWithOrphanedLandingSite in selectedOrphanedLandingSite.LandingSiteSamplings)
                            //.Where(t => t.LandingSiteText == selectedOrphanedLandingSite.LandingSiteName &&
                            //t.LandingSiteID == null))
                            {
                                //search for duplictes that will happen if we update landing site
                                //duplication will happen in landing site, fishing ground and sampling date
                                var sampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(selectedOrphanedLandingSite, ReplacementLandingSite, samplingWithOrphanedLandingSite.SamplingDate);
                                if (sampling != null)
                                {
                                    foreach (GearUnload gu in NSAPEntities.GearUnloadViewModel.GetGearUnloads(samplingWithOrphanedLandingSite))
                                    {
                                        gu.Parent = sampling;
                                        gu.LandingSiteSamplingID = gu.Parent.PK;
                                        NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);

                                        if(gu.GearUsedName=="Stationary liftnet" && gu.Parent.SamplingDate.Date==new DateTime(2021,1,19).Date && gu.Parent.LandingSiteID==138)
                                        {

                                        }

                                        var otherGearUnload = NSAPEntities.GearUnloadViewModel.getGearUnload(gearUnload: gu, samplingDate: sampling.SamplingDate.Date, ls: sampling.LandingSite);
                                        if(otherGearUnload!=null)
                                        {
                                            otherGearUnload.Boats = gu.Boats;
                                            otherGearUnload.Catch = gu.Catch;
                                            gu.Boats = null;
                                            gu.Catch = null;
                                            if(NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(otherGearUnload))
                                            {
                                                NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);
                                            }
                                            //NSAPEntities.GearUnloadViewModel.DeleteRecordFromRepo(gu.PK);
                                        }
                                    }

                                }
                                else
                                {
                                    samplingWithOrphanedLandingSite.LandingSiteID = ReplacementLandingSite.LandingSiteID;
                                    NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(samplingWithOrphanedLandingSite);
                                }


                                //sampling.LandingSiteID = ReplacementLandingSite.LandingSiteID;
                                //NSAPEntities.LandingSiteSamplingViewModel.UpdateRecordInRepo(sampling);
                            }
                        }
                    }
                    break;
                case NSAPEntity.FishingGear:
                    break;
                case NSAPEntity.NSAPRegionEnumerator:
                    break;

            }
        }

        private void RefreshItemsSource()
        {
            switch (NSAPEntity)
            {
                case NSAPEntity.LandingSite:
                    dataGrid.DataContext = NSAPEntities.LandingSiteSamplingViewModel.OrphanedLandingSites();

                    break;
                case NSAPEntity.Enumerator:
                    break;
                case NSAPEntity.FishingGear:
                    break;
            }

            //if(dataGrid.Items.Count==0)
            //{
            //    MessageBox.Show("There are no orphaned items for now","GPX Manager",MessageBoxButton.OK,MessageBoxImage.Information);
            //    Close();
            //}
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            bool procced = false;
            switch (((Button)sender).Name)
            {
                case "buttonReplace":
                    DoTheReplacement();
                    //dataGrid.Items.Refresh();
                    RefreshItemsSource();
                    break;
                case "buttonSelectReplacement":
                    var replacementWindow = new SelectionToReplaceOrpanWIndow();
                    replacementWindow.Owner = this;
                    replacementWindow.NSAPEntity = NSAPEntity;

                    foreach (var item in dataGrid.Items)
                    {
                        switch (NSAPEntity)
                        {
                            case NSAPEntity.LandingSite:
                                if (((OrphanedLandingSite)item).ForReplacement)
                                {
                                    procced = true;
                                    replacementWindow.LandingSiteSampling = ((OrphanedLandingSite)item).LandingSiteSamplings[0];

                                }
                                break;
                            case NSAPEntity.FishingGear:
                                break;
                            case NSAPEntity.NSAPRegionEnumerator:
                                break;

                        }

                        if (procced)
                        {
                            break;
                        }
                    }

                    if (procced)
                    {
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
    }
}
