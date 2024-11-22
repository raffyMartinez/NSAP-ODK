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
using NSAP_ODK.Utilities;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.VesselUnloadEditorControl;
using System.Windows.Interop;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EditCatchWindow.xaml
    /// </summary>
    public partial class EditCatchWindow : Window
    {
        private static EditCatchWindow _instance;
        private bool _isNew;
        private VesselCatchEdited _vesselCatchEdited;
        private VesselUnloadEdit _vesselUnloadEdit;
        private string _selectedTaxaCode;
        private string _vesselUnloadGearCode;
        private string _gearCode;
        private Xceed.Wpf.Toolkit.PropertyGrid.Attributes.ItemCollection _fishGenera;
        private string _genus;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        public VesselUnloadEdit VesselUnloadEdit
        {
            get { return _vesselUnloadEdit; }
            set
            {
                _vesselUnloadEdit = value;
                ConfigureUI();
            }
        }
        public VesselCatchEdited VesselCatchEdited
        {
            get { return _vesselCatchEdited; }
            set
            {
                _vesselCatchEdited = value;
                ConfigureUI();

            }
        }

        private void ConfigureUI()
        {
            panelOtherSaleUnit.Visibility = Visibility.Collapsed;
            panelPrice.Visibility = Visibility.Collapsed;
            panelPriceUnit.Visibility = Visibility.Collapsed;
            panelIsCatchSold.Visibility = Visibility.Collapsed;
            panelSearchFishGenus.Visibility = Visibility.Collapsed;


            TaxaItemsSource tis = new TaxaItemsSource();
            tis.GetValues();
            int fishIndex = 0;
            int loop = 0;
            cboTaxa.Items.Clear();
            foreach (var item in tis.TaxaItems)
            {
                cboTaxa.Items.Add(item);
                if (item.DisplayName == "Fish")
                {
                    fishIndex = loop;
                }
                loop++;
            }



            GearItemsSource.UnloadGears = VesselUnloadEdit.VesselUnload.ListUnloadFishingGears;
            GearItemsSource.AllowAddBlankGearName = true;
            GearItemsSource gis = new GearItemsSource();
            gis.GetValues();

            int gearIndex = 0;
            loop = 0;
            foreach (var item in gis.Gears)
            {
                cboGear.Items.Add(item);
                if ((string)item.Value == VesselUnloadEdit.GearCode)
                {
                    gearIndex = loop;
                    //_gearCode = VesselUnloadEdit.GearCode;
                }
                loop++;
            }

            //WeightUnitItemSource wuis = new WeightUnitItemSource();
            //wuis.GetValues();
            //loop = 0;
            //foreach(var item in wuis.Units)
            //{
            //    cboWtUnit.Items.Add(item);
            //    if((string)item.Value=="kg")
            //    {
            //        kiloIndex = loop;
            //    }
            //    loop++;
            //}

            PricingUnitItemsSource puis = new PricingUnitItemsSource();
            puis.GetValues();
            foreach (var item in puis.PricingUnitItems)
            {
                cboSaleUnit.Items.Add(item);
            }

            if (_vesselUnloadEdit.IsCatchSold)
            {
                panelIsCatchSold.Visibility = Visibility.Visible;
            }

            if (_isNew)
            {

                cboTaxa.SelectedIndex = fishIndex;
                cboGear.SelectedIndex = gearIndex;
                if (VesselUnloadEdit.WeightOfCatchSample == null)
                {
                    chkFromTotal.IsEnabled = false;
                    chkFromTotal.IsChecked = true;
                }
            }
            else
            {
                foreach (Item item in cboTaxa.Items)
                {
                    if ((string)item.Value == _vesselCatchEdited.TaxaCode)
                    {
                        cboTaxa.SelectedItem = item;
                        break;
                    }
                }

                foreach (Item item in cboGenus.Items)
                {
                    if ((string)item.Value == _vesselCatchEdited.Genus)
                    {
                        cboGenus.SelectedItem = item;
                        break;
                    }
                }
                foreach (Item item in cboSpecies.Items)
                {
                    if ((string)item.Value == _vesselCatchEdited.Species)
                    {
                        cboSpecies.SelectedItem = item;
                        break;
                    }
                }
                foreach (Item item in cboGear.Items)
                {
                    if ((string)item.Value == _vesselCatchEdited.GearCode)
                    {
                        cboGear.SelectedItem = item;
                        break;
                    }
                }
                if (VesselUnloadEdit.WeightOfCatchSample == null)
                {
                    chkFromTotal.IsEnabled = false;
                    chkFromTotal.IsChecked = true;
                }
                else
                {
                    chkFromTotal.IsChecked = _vesselCatchEdited.FromTotalCatch;
                }
                txtOtherGear.Text = _vesselCatchEdited.GearText;
                txtOtherName.Text = _vesselCatchEdited.OtherName;
                txtWeightKg.Text = _vesselCatchEdited.Catch_kg.ToString();
                //foreach(Item item in cboWtUnit.Items)
                //{
                //    if((string)item.Value==_vesselCatchEdited.WeighingUnit)
                //    {
                //        cboWtUnit.SelectedItem = item;
                //    }
                //}
                txtWeightSample.Text = _vesselCatchEdited.Sample_kg.ToString();
            }

        }
        public EditCatchWindow(bool isNew, VesselUnloadEdit vuEdit = null)
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
            _isNew = isNew;
            _vesselUnloadEdit = vuEdit;
            if (FishSpeciesViewModel.FishGenusList == null || FishSpeciesViewModel.FishGenusList.Count == 0)
            {
                NSAPEntities.FishSpeciesViewModel.GetAllGenus();
            }
            if (_isNew)
            {
                VesselCatchEdited = new VesselCatchEdited();
            }
            //txtSearchFishGenus.PreviewTextInput += TextBox_PreviewTextInput;
            //txtSearchFishGenus.LostFocus += txtBoxLostFocus;
            txtSearchFishGenus.TextChanged += TxtSearchFishGenus_TextChanged;
            //txtWeightKg.PreviewTextInput += TextBox_PreviewTextInput;
        }

        private void TxtSearchFishGenus_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchFishGenus.Text.Length > 2)
            {
                GenusFromTaxaItemsSource.SearchFishGenus = txtSearchFishGenus.Text;
                _fishGenera.Clear();
                FillGenusComboBox();
            }
        }

        private void txtBoxLostFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string msg = "";
            switch (((TextBox)sender).Name)
            {
                case "txtSearchFishGenus":
                    msg = "";
                    if (txtSearchFishGenus.Text.Length > 2)
                    {
                        GenusFromTaxaItemsSource.SearchFishGenus = txtSearchFishGenus.Text;
                        _fishGenera.Clear();
                        FillGenusComboBox();
                    }
                    //Console.WriteLine(msg);
                    break;

                case "txtWeightKg":
                    bool proceed = false;
                    if (int.TryParse(e.Text, out int v))
                    {
                        proceed = true;
                    }
                    else
                    {
                        if (e.Text != ".")
                        {
                            e.Handled = true;
                            msg = "Only numeric values allowed";
                        }
                        else
                        {
                            proceed = true;
                        }
                    }
                    if (proceed)
                    {
                        double catch_kg = double.Parse($"{txtWeightKg.Text}{e.Text}");
                        double? weight_catch_composition = null;
                        if (_isNew)
                        {
                            weight_catch_composition = catch_kg + VesselUnloadEdit.VesselUnload.VesselCatchViewModel.WeightOfCatchComposition();
                        }
                        else
                        {
                            weight_catch_composition = catch_kg + VesselUnloadEdit.VesselUnload.VesselCatchViewModel.WeightOfCatchComposition(VesselCatchEdited.PK);
                        }
                        if (weight_catch_composition > VesselUnloadEdit.WeightOfCatch)
                        {
                            msg = "Total weight of catch composition is greater than weight of catch";
                        }
                    }
                    break;
            }
            if (msg.Length > 0)
            {
                MessageBox.Show(msg, Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }



        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null;
            this.SavePlacement();
            ((VesselUnloadEditWindow)Owner).Focus();
        }

        public static EditCatchWindow GetInstance(bool isNew, VesselUnloadEdit vuEdit = null)
        {
            if (_instance == null)
            {
                _instance = new EditCatchWindow(isNew, vuEdit);
            }
            return _instance;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //panelOtherSaleUnit.Visibility = Visibility.Collapsed;
            //panelPrice.Visibility = Visibility.Collapsed;
            //panelPriceUnit.Visibility = Visibility.Collapsed;
            //panelIsCatchSold.Visibility = Visibility.Collapsed;
            cboTaxa.DisplayMemberPath = "DisplayName";
            cboSpecies.DisplayMemberPath = "DisplayName";
            cboGenus.DisplayMemberPath = "DisplayName";
            cboSaleUnit.DisplayMemberPath = "DisplayName";
            cboGear.DisplayMemberPath = "DisplayName";
            //cboWtUnit.DisplayMemberPath = "DisplayName";

        }
        private bool Validate()
        {
            //bool isValidated = false;
            string msg = "";
            bool proceed = false;
            if (double.TryParse(txtWeightKg.Text, out double v) && v > 0)
            {
                proceed = false;
                VesselUnload_FishingGear vufg = VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection.FirstOrDefault(t => t.GearCode == _gearCode);
                double sum_wt;
                if (_isNew)
                {
                    sum_wt = vufg.VesselCatchViewModel.VesselCatchCollection.Sum(t => (double)t.Catch_kg) + double.Parse(txtWeightKg.Text);


                }
                else
                {
                    double? sp_wt = vufg.VesselCatchViewModel.VesselCatchCollection.FirstOrDefault(t => t.PK == VesselCatchEdited.PK).Catch_kg;
                    sum_wt = vufg.VesselCatchViewModel.VesselCatchCollection.Sum(t => (double)t.Catch_kg) - (double)sp_wt;
                    sum_wt += double.Parse(txtWeightKg.Text);
                }
                if (sum_wt <= vufg.WeightOfCatch)
                {
                    proceed = true;
                }
                else
                {
                    msg = "Total weight of species composition of gear catch must not be greater than weight of catch of gear";
                }

                if (proceed)
                {
                    proceed = false;
                    if (txtWeightSample.Text.Length > 0)
                    {
                        if (double.TryParse(txtWeightSample.Text, out v) && v > 0)
                        {
                            proceed = true;
                        }
                        else
                        {
                            msg = "Sample weight must have a numeric value that is greater than zero";
                        }
                    }
                    else
                    {
                        proceed = true;
                    }
                }
                if (proceed)
                {
                    proceed = false;
                    if (VesselUnloadEdit.IsCatchSold)
                    {
                        if ((bool)chkIsCatchSold.IsChecked)
                        {
                            if (!double.TryParse(txtPrice.Text, out v) && v > 0)
                            {
                                msg = "Price of catch must have a numeric value that is greater than zero";
                            }
                            else if (cboSaleUnit.SelectedItem == null && txtOtherUnitSale.Text.Length == 0)
                            {
                                msg = "Unit of sale must be provided";
                            }
                            else if (((Item)cboSaleUnit.SelectedItem).DisplayName == "Other" && txtOtherUnitSale.Text.Length == 0)
                            {
                                msg = "Other unit of selling catch must have a value";
                            }
                            else
                            {
                                proceed = true;
                            }
                        }
                        else
                        {
                            proceed = true;
                        }
                    }
                    else
                    {
                        proceed = true;
                    }
                }
                //if (proceed)
                //{
                //    proceed = false;
                //    if ((bool)chkIsCatchSold.IsChecked)
                //    {
                //        if (cboSaleUnit.SelectedItem == null)
                //        {
                //            msg = "Unit of selling catch must have a value";
                //        }
                //        else if (!double.TryParse(txtOtherUnitSale.Text, out v))
                //        {
                //            msg = "Price of sold catch must have a numeric value";
                //        }

                //        else
                //        {
                //            proceed = true;
                //        }
                //    }
                //    else
                //    {
                //        proceed = true;
                //    }
                //}
                if (proceed)
                {
                    proceed = false;
                    if (txtOtherName.Text.Length == 0)
                    {
                        if (cboGenus.SelectedItem == null || cboSpecies.SelectedItem == null)
                        {
                            msg = "Genus and species must have a value. If species name is not known then other name of catch must have a value";
                        }
                        else
                        {
                            proceed = cboGenus.SelectedItem != null && cboSpecies.SelectedItem != null;
                        }
                    }
                    else
                    {
                        proceed = true;
                    }
                }
                if (proceed)
                {
                    proceed = false;
                    if (((Item)cboGear.SelectedItem).DisplayName == "" && txtOtherGear.Text.Length == 0)
                    {
                        msg = "If gear is not known then other gear used must have a value";
                    }
                    else
                    {
                        proceed = true;
                    }
                }
                //isValidated = proceed;
            }
            else
            {
                msg = "Catch weight must have a numeric value that is greater than zero";
            }
            if (msg.Length > 0)
            {
                MessageBox.Show(
                    msg,
                    Global.MessageBoxCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            //return isValidated;
            return proceed;
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    bool proceed = true;
                    string msg = "";
                    double catch_kg;
                    if (Validate())
                    {
                        if (((VesselUnloadEditWindow)Owner).UnloadEditor.VesselUnloadHasChangedProperties)
                        {

                            proceed = ((VesselUnloadEditWindow)Owner).UnloadEditor.SaveChangesToVesselUnload();
                        }
                        if (proceed)
                        {
                            catch_kg = double.Parse(txtWeightKg.Text);
                            double weight_catch_composition = 0;
                            VesselUnload parent = null;
                            int? vesselunload_id = null;
                            if (!VesselUnloadEdit.IsMultigear)
                            {
                                parent = VesselUnloadEdit.VesselUnload;
                                vesselunload_id = VesselUnloadEdit.VesselUnload.PK;
                            }
                            VesselCatch vc = new VesselCatch
                            {
                                SpeciesID = SpeciesCode,
                                SpeciesText = txtOtherName.Text,
                                Catch_kg = catch_kg,
                                TaxaCode = _selectedTaxaCode,
                                Parent = parent,
                                VesselUnloadID = vesselunload_id,
                                GearCode = _gearCode,
                                GearText = txtOtherGear.Text,
                                Sample_kg = null,
                                FromTotalCatch = (bool)chkFromTotal.IsChecked,
                                IsCatchSold = false,
                                PriceOfSpecies = null,
                                PriceUnit = null,
                                OtherPriceUnit = null,
                            };
                            if (_isNew)
                            {
                                if (VesselUnloadEdit != null)
                                {
                                    vc.ParentFishingGear = VesselUnloadEdit.VesselUnload.ListUnloadFishingGears.FirstOrDefault(t => t.GearCode == _gearCode);
                                    vc.ParentCarrierLanding = null;
                                }
                                else
                                {

                                }

                            }
                            else
                            {
                                vc.ParentFishingGear = VesselCatchEdited.ParentFishingGear;
                                vc.ParentCarrierLanding = VesselCatchEdited.ParentCarrierLanding;
                            }

                            if (txtWeightSample.Text.Length > 0)
                            {
                                if (double.TryParse(txtWeightSample.Text, out double v))
                                {
                                    vc.Sample_kg = v;
                                }
                            }


                            if ((bool)chkIsCatchSold.IsChecked)
                            {
                                vc.IsCatchSold = true;
                                vc.PriceOfSpecies = double.Parse(txtPrice.Text);
                                vc.PriceUnit = (string)((Item)cboSaleUnit.SelectedItem).Value;
                                if (vc.PriceUnit == "other")
                                {
                                    vc.OtherPriceUnit = txtOtherUnitSale.Text;
                                }
                            }

                            proceed = false;
                            if (_isNew)
                            {
                                vc.PK = VesselUnloadEdit.VesselUnload.VesselCatchViewModel.NextRecordNumber;
                                if (VesselUnloadEdit.VesselUnload.IsMultiGear)
                                {
                                    proceed = VesselUnloadEdit.VesselUnload.VesselUnload_FishingGearsViewModel
                                        .VesselUnload_FishingGearsCollection.FirstOrDefault(t => t.GearCode == _gearCode)
                                        .VesselCatchViewModel.AddRecordToRepo(vc);
                                }
                                else
                                {
                                    proceed = VesselUnloadEdit.VesselUnload.VesselCatchViewModel.AddRecordToRepo(vc);
                                }
                                if (proceed)
                                {
                                    vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(vc);
                                    vc.CatchLengthViewModel = new CatchLengthViewModel(vc);
                                    vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(vc);
                                    vc.CatchMaturityViewModel = new CatchMaturityViewModel(vc);
                                    ((VesselUnloadEditWindow)Owner).UnloadEditor.RefreshView();
                                    ((VesselUnloadEditWindow)Owner).Focus();
                                    Close();
                                }


                            }
                            else
                            {
                                vc.PK = VesselCatchEdited.PK;
                                VesselCatchViewModel vcm;
                                if (!VesselUnloadEdit.VesselUnload.IsMultiGear)
                                {
                                    vcm = VesselUnloadEdit.VesselUnload.VesselCatchViewModel;
                                }
                                else
                                {
                                    vcm = VesselCatchEdited.ParentFishingGear.VesselCatchViewModel;
                                }

                                if (vcm.UpdateRecordInRepo(vc))
                                {
                                    vc.CatchLenFreqViewModel = VesselCatchEdited.VesselCatch.CatchLenFreqViewModel;
                                    vc.CatchLengthViewModel = VesselCatchEdited.VesselCatch.CatchLengthViewModel;
                                    vc.CatchLengthWeightViewModel = VesselCatchEdited.VesselCatch.CatchLengthWeightViewModel;
                                    vc.CatchMaturityViewModel = VesselCatchEdited.VesselCatch.CatchMaturityViewModel;
                                    ((VesselUnloadEditWindow)Owner).UnloadEditor.RefreshView();
                                    Close();
                                }

                            }
                        }
                    }

                    if (msg.Length > 0)
                    {
                        MessageBox.Show(msg,
                            Global.MessageBoxCaption,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void OnCheckboxChange(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            switch (chk.Name)
            {
                case "chkIsCatchSold":
                    if ((bool)chk.IsChecked)
                    {
                        //panelOtherSaleUnit.Visibility = Visibility.Visible;
                        panelPrice.Visibility = Visibility.Visible;
                        panelPriceUnit.Visibility = Visibility.Visible;
                        if (cboSaleUnit.SelectedItem != null && ((Item)cboSaleUnit.SelectedItem).DisplayName == "Other")
                        {
                            panelOtherSaleUnit.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        panelOtherSaleUnit.Visibility = Visibility.Collapsed;
                        panelPrice.Visibility = Visibility.Collapsed;
                        panelPriceUnit.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }
        public string SpeciesName { get; set; }
        public int SpeciesCode { get; set; }

        private void FillGenusComboBox()
        {
            _selectedTaxaCode = (string)((Item)cboTaxa.SelectedItem).Value;
            cboGenus.Items.Clear();
            cboSpecies.Items.Clear();
            GenusFromTaxaItemsSource.TaxaCode = _selectedTaxaCode;
            GenusFromTaxaItemsSource gft = new GenusFromTaxaItemsSource();

            if (_selectedTaxaCode == "FIS")
            {
                if (_fishGenera == null || _fishGenera.Count == 0)
                {
                    gft.GetValues();
                    _fishGenera = gft.GeneraItemCollection;
                }
                foreach (var item in _fishGenera)
                {
                    cboGenus.Items.Add(item);
                }
                panelSearchFishGenus.Visibility = Visibility.Visible;
            }
            else
            {
                panelSearchFishGenus.Visibility = Visibility.Collapsed;
                gft.GetValues();
                foreach (var item in gft.GeneraItemCollection)
                {
                    cboGenus.Items.Add(item);
                }
            }

        }
        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBox)sender).Name)
            {
                case "cboTaxa":
                    FillGenusComboBox();
                    //_selectedTaxaCode = (string)((Item)cboTaxa.SelectedItem).Value;
                    //cboGenus.Items.Clear();
                    //cboSpecies.Items.Clear();
                    //GenusFromTaxaItemsSource.TaxaCode = _selectedTaxaCode;
                    //GenusFromTaxaItemsSource gft = new GenusFromTaxaItemsSource();

                    //if (_selectedTaxaCode == "FIS")
                    //{
                    //    if (_fishGenera == null || _fishGenera.Count == 0)
                    //    {
                    //        gft.GetValues();
                    //        _fishGenera = gft.GeneraItemCollection;
                    //    }
                    //    foreach (var item in _fishGenera)
                    //    {
                    //        cboGenus.Items.Add(item);
                    //    }
                    //    panelSearchFishGenus.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    panelSearchFishGenus.Visibility = Visibility.Collapsed;
                    //    gft.GetValues();
                    //    foreach (var item in gft.GeneraItemCollection)
                    //    {
                    //        cboGenus.Items.Add(item);
                    //    }
                    //}
                    break;
                case "cboGenus":
                    if (cboGenus.Items.Count > 0)
                    {
                        _genus = (string)((Item)cboGenus.SelectedItem).Value;
                        SpeciesFromGenusItemsSource.Genus = _genus;
                        SpeciesFromGenusItemsSource sfgis = new SpeciesFromGenusItemsSource();
                        sfgis.GetValues();
                        cboSpecies.Items.Clear();
                        foreach (var item in sfgis.SpeciesItemCollection)
                        {
                            cboSpecies.Items.Add(item);
                        }
                    }
                    break;
                case "cboSpecies":
                    if (cboSpecies.SelectedItem != null)
                    {
                        SpeciesName = $"{_genus} {((Item)cboSpecies.SelectedItem).DisplayName}";
                        if (_selectedTaxaCode == "FIS")
                        {
                            SpeciesCode = (int)NSAPEntities.FishSpeciesViewModel.GetSpecies(SpeciesName).SpeciesCode;
                        }
                        else
                        {
                            SpeciesCode = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(SpeciesName).SpeciesID;
                        }
                    }
                    break;
                case "cboGear":
                    if (cboGear.SelectedItem != null)
                    {
                        _gearCode = (string)((Item)cboGear.SelectedItem).Value;
                    }
                    break;
                case "cboSaleUnit":
                    string saleUnit = ((Item)cboSaleUnit.SelectedItem).DisplayName;
                    if (saleUnit == "Other")
                    {
                        panelOtherSaleUnit.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        panelOtherSaleUnit.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }
    }
}
