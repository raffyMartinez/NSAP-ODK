using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NSAP_ODK.Entities;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for ImportByPlainTextWindow.xaml
    /// </summary>
    public partial class ImportByPlainTextWindow : Window
    {
        private NSAPRegion _selectedRegion;
        public ImportByPlainTextWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
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

        private void OnRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _selectedRegion = (NSAPRegion)((RadioButton)sender).Tag;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            panelSector.Visibility = Visibility.Collapsed;
            switch (NSAPEntityType)
            {
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
                    FillRegionRadioButtons();
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
            foreach (CheckBox c in panelRegions.Children)
            {
                if ((bool)c.IsChecked)
                {
                    count++;
                }
            }
            return count;
        }
        public NSAPEntity NSAPEntityType { get; set; }
        private void onButtonClick(object sender, RoutedEventArgs e)
        {
            List<EntityValidationMessage> entityMessages = new List<EntityValidationMessage>();
            int importCount = 0;
            switch (((Button)sender).Name)
            {
                case "buttonOk":

                    if (textBox.Text.Length > 0 && CheckedRegions() > 0)
                    {
                        switch (NSAPEntityType)
                        {
                            case NSAPEntity.Enumerator:
                                foreach (var item in textBox.Text.Split('\n').ToList())
                                {
                                    var nse = new NSAPEnumerator { Name = item.Trim(), ID = NSAPEntities.NSAPEnumeratorViewModel.NextRecordNumber };
                                    if (NSAPEntities.NSAPEnumeratorViewModel.EntityValidated(nse, out entityMessages, true))
                                    {
                                        if (NSAPEntities.NSAPEnumeratorViewModel.AddRecordToRepo(new NSAPEnumerator { Name = item.Trim(), ID = NSAPEntities.NSAPEnumeratorViewModel.NextRecordNumber }))
                                        {
                                            foreach (CheckBox c in panelRegions.Children)
                                            {
                                                if ((bool)c.IsChecked)
                                                {
                                                    var region = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(c.Tag.ToString());
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
                                        foreach (var msg in entityMessages)
                                        {
                                            if (msg.MessageType == MessageType.Error)
                                            {
                                                Logger.Log($"Batch import by text error in adding new enumerator: {msg.Message} after adding {nse.Name}");
                                            }
                                        }
                                    }
                                }
                                break;
                        }


                    ((MainWindow)Owner).RefreshEntityGrid();
                        if (importCount > 0)
                        {
                            MessageBox.Show($"Succesfully imported {importCount} items", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                        else
                        {
                            MessageBox.Show($"Was not able to import an item", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Provide an item to import and a selected region1", "NSAP-ODK Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    break;
                case "buttonCancel":
                    Close();
                    break;
            }

        }
    }
}
