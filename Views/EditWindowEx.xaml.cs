﻿using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EditWindowEx.xaml
    /// </summary>
    public partial class EditWindowEx : Window
    {
        private static Dictionary<NSAPEntity, EditWindowEx> _editWindowsDict = new Dictionary<NSAPEntity, EditWindowEx>();
        private string _oldGenus;
        private string _oldSpecies;
        private string _oldName;
        private string _oldIdentifier;
        private GridLength _rowDataGridHeight;
        private GridLength _rowBottomLabelHeight;
        private NSAPEntity _nsapEntity;
        private object _nsapObject;
        private bool _isNew;
        private string _entityID;
        private bool _saveButtonClicked;
        private string _selectedProperty;
        private bool _oldIsForAllTypesFishing;


        public bool CloseCommandFromMainWindow { get; set; }
        public NSAPEntity NSAPEntity { get { return _nsapEntity; } }
        public bool Cancelled { get; set; } = false;

        public EditWindowEx(NSAPEntity nsapENtity, string entityID = "", object nsapObject = null)
        {
            InitializeComponent();
            Activated += OnWindowActivated;
            Loaded += OnFormLoaded;
            this.PropertyGrid.MouseDoubleClick += OnPropertyGridDoubleClick;
            sfDataGrid.SelectionChanged += OnsfDataGridSelectionChanged;
            _nsapEntity = nsapENtity;
            _entityID = entityID;
            _isNew = _entityID.Length == 0;
            _nsapObject = nsapObject;
        }

        private void OnWindowActivated(object sender, EventArgs e)
        {
            //Title = $"nsapentity is {_nsapEntity.ToString()}";

            if (
                (_nsapEntity == NSAPEntity.NSAPRegionFMA
                && NSAPEntities.EntityToRefresh == NSAPEntity.NSAPRegionFMAFishingGround)

                ||

                (_nsapEntity == NSAPEntity.NSAPRegionFMAFishingGround
                && NSAPEntities.EntityToRefresh == NSAPEntity.NSAPRegionFMAFishingGroundLandingSite)

                ||

                (_nsapEntity == NSAPEntity.NSAPRegion
                && NSAPEntities.EntityToRefresh == NSAPEntity.NSAPRegionGear)

                ||

                (_nsapEntity == NSAPEntity.NSAPRegion
                && NSAPEntities.EntityToRefresh == NSAPEntity.NSAPRegionFishingVessel)

                ||

                (_nsapEntity == NSAPEntity.NSAPRegion
                && NSAPEntities.EntityToRefresh == NSAPEntity.NSAPRegionEnumerator)

                ||

                (_nsapEntity == NSAPEntity.NSAPRegion
                && NSAPEntities.EntityToRefresh == NSAPEntity.NSAPRegionFMA)
                )
            {
                PropertyGrid.Update();
                NSAPEntities.EntityToRefresh = NSAPEntity.Nothing;
                SetUpSubFormSource();
            }



        }

        private void OnPropertyGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ComboBox cbo = null;
            EditWindowEx ew = null;
            switch (_selectedProperty)
            {
                case "GearCode":
                    ew = new EditWindowEx(NSAPEntity.FishingGear);
                    break;

                case "FishingGroundCode":
                    ew = new EditWindowEx(NSAPEntity.FishingGround);
                    break;

                case "FishingVesselID":
                    ew = new EditWindowEx(NSAPEntity.FishingVessel);
                    break;

                case "EnumeratorID":
                    ew = new EditWindowEx(NSAPEntity.Enumerator);
                    break;

                case "LandingSiteID":
                    ew = new EditWindowEx(NSAPEntity.LandingSite);
                    break;
            }

            if (_selectedProperty == "GearCode"
                || _selectedProperty == "FishingGroundCode"
                || _selectedProperty == "FishingVesselID"
                || _selectedProperty == "EnumeratorID"
                || _selectedProperty == "LandingSiteID")
            {
                this.Visibility = Visibility.Hidden;
                _editWindowsDict.Add(_nsapEntity, this);
            }

            if (ew != null && ew.ShowDialog() == true)
            {
                Dictionary<string, PropertyItem> dictProperties = new Dictionary<string, PropertyItem>();
                switch (_selectedProperty)
                {
                    case "EnumeratorID":
                        if (NSAPEntities.NSAPEnumeratorViewModel.CurrentEntity != null)
                        {
                            NSAPEnumerator nse = NSAPEntities.NSAPEnumeratorViewModel.CurrentEntity;
                            ComboBox cboEnum = new ComboBox();
                            KeyValuePair<int, string> enumItem = new KeyValuePair<int, string>(nse.ID, nse.Name);
                            foreach (PropertyItem prp in PropertyGrid.Properties)
                            {
                                dictProperties.Add(prp.PropertyName, prp);
                            }
                            foreach (Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item item in ((ComboBox)dictProperties["EnumeratorID"].Editor).Items)
                            {
                                cboEnum.Items.Add(new KeyValuePair<int, string>((int)item.Value, item.DisplayName));
                            }
                            cboEnum.DisplayMemberPath = "Value";
                            cboEnum.Tag = "new enumerator";
                            cboEnum.Items.Add(enumItem);
                            cboEnum.SelectedValue = enumItem;
                            dictProperties["EnumeratorID"].Editor = cboEnum;
                            dictProperties["EnumeratorID"].Value = enumItem.Key;
                            dictProperties["DateStart"].Value = DateTime.Parse("Jan-1-2010");
                            return;
                        }
                        foreach (PropertyItem prp in PropertyGrid.Properties)
                        {
                            if (prp.PropertyName == _selectedProperty)
                            {
                                cbo = new ComboBox();
                                cbo.Tag = "Enumerator";
                                foreach (var item in NSAPEntities.NSAPEnumeratorViewModel.NSAPEnumeratorCollection.OrderBy(t => t.ToString()))
                                {
                                    cbo.Items.Add(new KeyValuePair<int, string>(item.ID, item.Name));
                                }
                                prp.Editor = cbo;
                                break;
                            }
                        }
                        break;

                    case "FishingVesselID":
                        if (NSAPEntities.FishingVesselViewModel.CurrentEntity != null)
                        {
                            FishingVessel fv = NSAPEntities.FishingVesselViewModel.CurrentEntity;
                            ComboBox cboVessel = new ComboBox();
                            KeyValuePair<int, string> vesselItem = new KeyValuePair<int, string>(fv.ID, fv.ToString());
                            foreach (PropertyItem prp in PropertyGrid.Properties)
                            {
                                dictProperties.Add(prp.PropertyName, prp);
                            }
                            foreach (Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item item in ((ComboBox)dictProperties["FishingVesselID"].Editor).Items)
                            {
                                cboVessel.Items.Add(new KeyValuePair<int, string>((int)item.Value, item.DisplayName));
                            }
                            cboVessel.DisplayMemberPath = "Value";
                            cboVessel.Tag = "new vessel";
                            cboVessel.Items.Add(vesselItem);
                            cboVessel.SelectedValue = vesselItem;
                            dictProperties["FishingVesselID"].Editor = cboVessel;
                            dictProperties["FishingVesselID"].Value = vesselItem.Key;
                            dictProperties["DateStart"].Value = DateTime.Parse("Jan-1-2010");
                            return;
                        }
                        foreach (PropertyItem prp in PropertyGrid.Properties)
                        {
                            if (prp.PropertyName == _selectedProperty)
                            {
                                cbo = new ComboBox();
                                cbo.Tag = "FishingVessel";
                                foreach (var item in NSAPEntities.FishingVesselViewModel.FishingVesselCollection.OrderBy(t => t.ToString()))
                                {
                                    cbo.Items.Add(new KeyValuePair<int, string>(item.ID, item.ToString()));
                                }
                                prp.Editor = cbo;
                                break;
                            }
                        }
                        break;

                    case "GearCode":
                        if (NSAPEntities.GearViewModel.CurrentEntity != null)
                        {
                            Gear g = NSAPEntities.GearViewModel.CurrentEntity;
                            ComboBox cboGear = new ComboBox();
                            KeyValuePair<string, string> gearItem = new KeyValuePair<string, string>(g.Code, g.GearName);
                            foreach (PropertyItem prp in PropertyGrid.Properties)
                            {
                                dictProperties.Add(prp.PropertyName, prp);
                            }
                            foreach (Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item item in ((ComboBox)dictProperties["GearCode"].Editor).Items)
                            {
                                cboGear.Items.Add(new KeyValuePair<string, string>(item.Value.ToString(), item.DisplayName));
                            }
                            cboGear.DisplayMemberPath = "Value";
                            cboGear.Tag = "new gear";
                            cboGear.Items.Add(gearItem);
                            cboGear.SelectedValue = gearItem;
                            dictProperties["GearCode"].Editor = cboGear;
                            dictProperties["GearCode"].Value = gearItem.Key;
                            dictProperties["DateStart"].Value = DateTime.Parse("Jan-1-2010");
                            return;
                        }
                        foreach (PropertyItem prp in PropertyGrid.Properties)
                        {
                            if (prp.PropertyName == _selectedProperty)
                            {
                                cbo = new ComboBox();
                                cbo.Tag = "Gear";
                                foreach (var item in NSAPEntities.GearViewModel.GearCollection.OrderBy(t => t.GearName))
                                {
                                    cbo.Items.Add(new KeyValuePair<string, string>(item.Code, item.GearName));
                                }
                                prp.Editor = cbo;
                                break;
                            }
                        }
                        break;

                    case "LandingSiteID":
                        if (NSAPEntities.LandingSiteViewModel.CurrentEntity != null)
                        {
                            LandingSite ls = NSAPEntities.LandingSiteViewModel.CurrentEntity;
                            ComboBox cboProv = new ComboBox();
                            KeyValuePair<int, string> provinceItem = new KeyValuePair<int, string>(ls.Municipality.Province.ProvinceID, ls.Municipality.Province.ProvinceName);

                            Province province = ls.Municipality.Province;
                            Municipality municipality = ls.Municipality;
                            foreach (PropertyItem prp in PropertyGrid.Properties)
                            {
                                dictProperties.Add(prp.PropertyName, prp);
                            }
                            foreach (Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item item in ((ComboBox)dictProperties["Province"].Editor).Items)
                            {
                                cboProv.Items.Add(new KeyValuePair<string, string>(item.Value.ToString(), item.DisplayName));
                            }
                            cboProv.Items.Add(provinceItem);
                            cboProv.SelectedValue = provinceItem;
                            cboProv.DisplayMemberPath = "Value";
                            dictProperties["RowID"].Value = NSAPEntities.GetMaxItemSetID() + 1;
                            dictProperties["Province"].Editor = cboProv;
                            dictProperties["Province"].Value = province.ProvinceID;
                            ((ComboBox)dictProperties["Municipality"].Editor).SelectedValue = new KeyValuePair<int, string>(municipality.MunicipalityID, municipality.MunicipalityName);
                            ((ComboBox)dictProperties["LandingSiteID"].Editor).SelectedValue = new KeyValuePair<int, string>(ls.LandingSiteID, ls.LandingSiteName);
                            dictProperties["DateStart"].Value = DateTime.Parse("Jan-1-2010");

                            return;
                        }
                        else
                        {
                            foreach (PropertyItem prp in PropertyGrid.Properties)
                            {
                                if (prp.PropertyName == _selectedProperty)
                                {
                                    cbo = new ComboBox();
                                    cbo.Tag = "Province";
                                    foreach (var item in NSAPEntities.LandingSiteViewModel.LandingSiteCollection.GroupBy(t => t.Municipality.Province.ProvinceName).Select(t => t.First()))
                                    {
                                        cbo.Items.Add(new KeyValuePair<int, string>(item.Municipality.Province.ProvinceID, item.Municipality.Province.ProvinceName));
                                    }
                                    prp.Editor = cbo;
                                    break;
                                }
                            }
                        }
                        break;

                    case "FishingGroundCode":
                        if (NSAPEntities.FishingGroundViewModel.CurrentEntity != null)
                        {
                            FishingGround fg = NSAPEntities.FishingGroundViewModel.CurrentEntity;
                            ComboBox cboFG = new ComboBox();
                            KeyValuePair<string, string> fishingGroundItem = new KeyValuePair<string, string>(fg.Code, fg.Name);
                            foreach (PropertyItem prp in PropertyGrid.Properties)
                            {
                                dictProperties.Add(prp.PropertyName, prp);
                            }
                            foreach (Xceed.Wpf.Toolkit.PropertyGrid.Attributes.Item item in ((ComboBox)dictProperties["FishingGroundCode"].Editor).Items)
                            {
                                cboFG.Items.Add(new KeyValuePair<string, string>(item.Value.ToString(), item.DisplayName));
                            }
                            cboFG.DisplayMemberPath = "Value";
                            cboFG.Tag = "new fishing ground";
                            cboFG.Items.Add(fishingGroundItem);
                            cboFG.SelectedValue = fishingGroundItem;
                            dictProperties["FishingGroundCode"].Editor = cboFG;
                            dictProperties["FishingGroundCode"].Value = fishingGroundItem.Key;
                            dictProperties["DateStart"].Value = DateTime.Parse("Jan-1-2010");

                            return;
                        }
                        foreach (PropertyItem prp in PropertyGrid.Properties)
                        {
                            if (prp.PropertyName == "FishingGroundCode")
                            {
                                cbo = new ComboBox();
                                cbo.Tag = "FishingGround";
                                foreach (var item in NSAPEntities.FishingGroundViewModel.FishingGroundCollection)
                                {
                                    cbo.Items.Add(new KeyValuePair<string, string>(item.Code, item.Name));
                                }
                                prp.Editor = cbo;
                                break;
                            }
                        }
                        break;
                }
                cbo.DisplayMemberPath = "Value";
                cbo.SelectionChanged += OnComboSelectionChanged;
            }
        }

        private void OnsfDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEdit.IsEnabled = true;
            buttonDelete.IsEnabled = true;
        }

        private void FillPropertyGrid()
        {
            PropertyGrid.AutoGenerateProperties = false;
            switch (_nsapEntity)
            {

                case NSAPEntity.NSAPRegionFMAFishingGroundLandingSite:
                    
                    NSAPRegionFMAFishingGroundLandingSiteEdit nsapRegionFMAFGLS = new NSAPRegionFMAFishingGroundLandingSiteEdit();
                    var landingSite = (NSAPRegionFMAFishingGroundLandingSite)_nsapObject;
                    Title = "Landing site in fishing ground";
                    LabelTop.Content = $"New landing site in {landingSite.NSAPRegionFMAFishingGround}";
                    if (!_isNew)
                    {
                        LabelTop.Content = $"Landing site details: {landingSite.LandingSite}, {landingSite.NSAPRegionFMAFishingGround}";
                        var nsffls = new NSAPRegionFMAFishingGroundLandingSiteEdit((NSAPRegionFMAFishingGroundLandingSite)_nsapObject);
                        nsapRegionFMAFGLS = new NSAPRegionFMAFishingGroundLandingSiteEdit
                        {
                            RowID = nsffls.RowID,
                            LandingSiteID = nsffls.LandingSiteID,
                            DateStart = nsffls.DateStart,
                            DateEnd = nsffls.DateEnd,
                            Province = nsffls.Province,
                            Municipality = nsffls.Municipality,
                            FMAFishingGround = nsffls.FMAFishingGround
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Province", DisplayName = "Province", DisplayOrder = 1, Description = "Province where landing site is found" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Municipality", DisplayName = "Municipality", DisplayOrder = 2, Description = "Municipality where landing site is found" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSiteID", DisplayName = "Landing site*", DisplayOrder = 3, Description = "Name of landing site.\r\nDouble click to directly add a new landing site to the fishing ground" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateStart", DisplayName = "Date included in the list", DisplayOrder = 4, Description = "Date when landing site was included in the fishing ground" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateEnd", DisplayName = "Date removed from the list", DisplayOrder = 5, Description = "Date when landing site was removed from the fishing ground" });

                    if (_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier used in the database" });
                    //FillProperties();
                    PropertyGrid.SelectedObject = nsapRegionFMAFGLS;
                    break;

                case NSAPEntity.FishingGearEffortSpecification:
                    var fgs = (GearEffortSpecification)_nsapObject;
                    GearEffortSpecification gearEffortSpec = new GearEffortSpecification();
                    Title = $"Fishing effort indicator of fishing gear";
                    LabelTop.Content = "New effort specification for fishing gear";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of effort specification for current fishing gear";
                       
                        gearEffortSpec = new GearEffortSpecification
                        {
                            RowID = fgs.RowID,
                            EffortSpecification = fgs.EffortSpecification
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EffortSpecificationID", DisplayName = "Effort specification", DisplayOrder = 3, Description = "Effort specification of fishing gear" });

                    if (!_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier used in the database" });

                    PropertyGrid.SelectedObject = gearEffortSpec;
                    break;

                case NSAPEntity.NSAPRegionFMAFishingGround:
                    Title = "Fishing ground in FMA";
                    NSAPRegionFMAFishingGround nsapRegionFMAFishingGround = (NSAPRegionFMAFishingGround)_nsapObject;
                    LabelTop.Content = $"New fishing ground for {nsapRegionFMAFishingGround.RegionFMA.FMA}, {nsapRegionFMAFishingGround.RegionFMA.NSAPRegion}";
                    if (!_isNew)
                    {
                        LabelTop.Content = $"Fishing ground details: {nsapRegionFMAFishingGround.FishingGround}, {nsapRegionFMAFishingGround.RegionFMA}";
                        var fg = (NSAPRegionFMAFishingGround)_nsapObject;
                        nsapRegionFMAFishingGround = new NSAPRegionFMAFishingGround
                        {
                            RowID = fg.RowID,
                            FishingGround = fg.FishingGround,
                            DateStart = fg.DateStart,
                            DateEnd = fg.DateEnd,
                            LandingSites = fg.LandingSites
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGroundCode", DisplayName = "Name of fishing ground*", DisplayOrder = 1, Description = "Name of fishing ground included the region\r\nDouble click to directly add a new fishing ground to the FMA" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateStart", DisplayName = "Date included in the region", DisplayOrder = 2, Description = "Date when the fishing ground was included in the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateEnd", DisplayName = "Date removed from the region", DisplayOrder = 3, Description = "Date when the fishing ground was removed from the region" });

                    if (!_isNew)
                    {
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LandingSiteCount", DisplayName = "Number of landing sites", DisplayOrder = 4, Description = "Number of landing sites" });
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier used in the database" });
                    }

                    PropertyGrid.SelectedObject = nsapRegionFMAFishingGround;

                    break;

                case NSAPEntity.NSAPRegionFishingVessel:
                    Title = "Fishing vessel in NSAP Region";
                    NSAPRegionFishingVesselEdit nsapRegionFishingVesselEdit = new NSAPRegionFishingVesselEdit();
                    LabelTop.Content = $"New fishing vessel to be added to {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of fishing vessel included in the region";
                        var nrfv = (NSAPRegionFishingVessel)_nsapObject;
                        nsapRegionFishingVesselEdit = new NSAPRegionFishingVesselEdit(nrfv);
                        NSAPEntities.FisheriesSector = nsapRegionFishingVesselEdit.FisheriesSector;

                    }
                    else
                    {
                        nsapRegionFishingVesselEdit.FisheriesSector = FisheriesSector.Municipal;
                        nsapRegionFishingVesselEdit.RowID = NSAPEntities.GetMaxItemSetID() + 1;

                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FisheriesSector", DisplayName = "Fisheries sector", DisplayOrder = 1, Description = "Fisheries sector" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingVesselID", DisplayName = "Name of fishing vessel/Owner's name*", DisplayOrder = 2, Description = "Name of fishing vessel included the region\r\nDouble click to directly add a fishing vessel to a region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateStart", DisplayName = "Date vessel included in the region", DisplayOrder = 3, Description = "Date when the vessel was included in the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateEnd", DisplayName = "Date vessel removed from the region", DisplayOrder = 4, Description = "Date when the vessel was removed from the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 5, Description = "Identifier used in the database" });

                    PropertyGrid.SelectedObject = nsapRegionFishingVesselEdit;
                    break;

                case NSAPEntity.NSAPRegionFMA:
                    if (!_isNew)
                    {
                        this.Title = "FMA in NSAP Region";
                        NSAPRegionFMA nrfma = (NSAPRegionFMA)_nsapObject;
                        NSAPRegionFMA nsapRegionFMA = new NSAPRegionFMA
                        {
                            RowID = nrfma.RowID,
                            FMAID = nrfma.FMAID,
                            FishingGrounds = nrfma.FishingGrounds
                        };
                        LabelTop.Content = $"Details for {nrfma.FMA}, {nrfma.NSAPRegion}";
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMAID", DisplayName = "Name of FMA", DisplayOrder = 2, Description = "Name of FMA that is part of the region" });
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FishingGroundCount", DisplayName = "Number of fishing grounds", DisplayOrder = 3, Description = "Number of fishing grounds that is part of the FMA" });

                        if (!_isNew)
                            PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier used in the database" });

                        PropertyGrid.SelectedObject = nsapRegionFMA;
                    }
                    break;

                case NSAPEntity.NSAPRegionEnumerator:
                    this.Title = "Enumerator in NSAP Region";
                    NSAPRegionEnumerator nsapRegionEnumerator = new NSAPRegionEnumerator();
                    LabelTop.Content = $"New enumerator to be added to {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    if (!_isNew)
                    {
                        var nse = (NSAPRegionEnumerator)_nsapObject;
                        LabelTop.Content = $"Detail of {nse}, {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                        nsapRegionEnumerator = new NSAPRegionEnumerator
                        {
                            DateEnd = nse.DateEnd,
                            DateStart = nse.DateStart,
                            EnumeratorID = nse.EnumeratorID,
                            RowID = nse.RowID,
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EnumeratorID", DisplayName = "Enumerator*", DisplayOrder = 1, Description = "Enumerator included in the region\r\nDouble click to add an enumerator to a region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateStart", DisplayName = "Date enumerator added to the region", DisplayOrder = 2, Description = "Date when enumerator was included" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateEnd", DisplayName = "Date enumerator removed from the region", DisplayOrder = 3, Description = "Date when enumerator was removed" });

                    if (!_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier used in the database" });

                    PropertyGrid.SelectedObject = nsapRegionEnumerator;
                    break;

                case NSAPEntity.NSAPRegionGear:
                    NSAPRegionGear nsapRegionGear = new NSAPRegionGear();
                    this.Title = "Fishing gear in NSAP Region";
                    LabelTop.Content = $"New gear type to be added to {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    if (!_isNew)
                    {
                        var nrg = (NSAPRegionGear)_nsapObject;
                        nsapRegionGear = new NSAPRegionGear
                        {
                            RowID = nrg.RowID,
                            DateStart = nrg.DateStart,
                            DateEnd = nrg.DateEnd,
                            GearCode = nrg.GearCode
                        };
                        LabelTop.Content = $"Details of {nrg}, {nrg.NSAPRegion}";
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearCode", DisplayName = "Gear*", DisplayOrder = 1, Description = "Gear that is used in the region\r\nDouble click to directly add a fishing gear to a region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateStart", DisplayName = "Date gear added to the region", DisplayOrder = 2, Description = "Date when gear was included" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "DateEnd", DisplayName = "Date gear removed from the region", DisplayOrder = 3, Description = "Date when gear was removed" });

                    if (!_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier used in the database" });

                    PropertyGrid.SelectedObject = nsapRegionGear;
                    break;

                case NSAPEntity.FishSpecies:
                    var fishSpeciesEdit = new FishSpeciesEdit();
                    this.Title = "Fish species";
                    LabelTop.Content = "New fish species";
                    if (!_isNew)
                    {
                        var fs = NSAPEntities.FishSpeciesViewModel.GetSpecies(int.Parse(_entityID));
                        LabelTop.Content = $"Details of the {fs.Family.TrimEnd(new char[] { 'a', 'e' })} fish {fs.GenericName} {fs.SpecificName}";
                        fishSpeciesEdit = new FishSpeciesEdit(fs);
                        _oldGenus = fs.GenericName;
                        _oldSpecies = fs.SpecificName;
                    }
                    else
                    {
                        fishSpeciesEdit.RowNumber = NSAPEntities.FishSpeciesViewModel.NextRecordNumber;
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RowNumber", DisplayName = "Species ID", DisplayOrder = 1, Description = "Identifier used in the database" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SpeciesCode", DisplayName = "Fishbase species ID", DisplayOrder = 2, Description = "Identifier of the species in FishBase" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GenericName", DisplayName = "Genus", DisplayOrder = 3, Description = "Generic name of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SpecificName", DisplayName = "Species", DisplayOrder = 4, Description = "Specific name of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Family", DisplayName = "Family", DisplayOrder = 5, Description = "Family of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "LengthType", DisplayName = "Length type", DisplayOrder = 6, Description = "Length category of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MaxLength", DisplayName = "Max length", DisplayOrder = 7, Description = "Maximum length recorded for the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "CommonLength", DisplayName = "Common length", DisplayOrder = 8, Description = "Maximum length recorded for the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Importance", DisplayName = "Importance to fishery", DisplayOrder = 9, Description = "Importance of the species to the fishery" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MainCatchingMethod", DisplayName = "Main catching method", DisplayOrder = 10, Description = "Main catching method" });
                    PropertyGrid.SelectedObject = fishSpeciesEdit;
                    break;

                case NSAPEntity.NonFishSpecies:
                    Title = "Non fish species";
                    NotFishSpeciesEdit notFishSpeciesEdit = new NotFishSpeciesEdit();
                    LabelTop.Content = "New non-fish species";
                    if (!_isNew)
                    {
                        var nfs = NSAPEntities.NotFishSpeciesViewModel.GetSpecies(int.Parse(_entityID));
                        LabelTop.Content = $"Details of the {nfs.Taxa.Name.ToLower().TrimEnd(new char[] { 's' })} {nfs.Genus} {nfs.Species}";
                        notFishSpeciesEdit = new NotFishSpeciesEdit(nfs);
                        _oldGenus = nfs.Genus;
                        _oldSpecies = nfs.Species;
                    }
                    else
                    {
                        notFishSpeciesEdit.SpeciesID = NSAPEntities.NotFishSpeciesViewModel.NextRecordNumber;
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SpeciesID", DisplayName = "Species ID", DisplayOrder = 1, Description = "Identifier used in the database" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Genus", DisplayName = "Genus", DisplayOrder = 2, Description = "Generic name of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Species", DisplayName = "Species", DisplayOrder = 3, Description = "Specific name of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "TaxaCode", DisplayName = "Taxonomic category", DisplayOrder = 4, Description = "Taxonomic category of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "SizeTypeCode", DisplayName = "Size type", DisplayOrder = 5, Description = "Size category of the species" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MaxSize", DisplayName = "Max size", DisplayOrder = 6, Description = "Maximum size recorded for the species" });
                    PropertyGrid.SelectedObject = notFishSpeciesEdit;

                    break;

                case NSAPEntity.EffortIndicator:
                    Title = "Effort indicator";
                    EffortSpecification effortSpecification = new EffortSpecification();
                    LabelTop.Content = "New fishing effort specification";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of fishing effort specification";
                        var fes = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(int.Parse(_entityID));
                        _oldName = fes.Name;
                        _oldIsForAllTypesFishing = fes.IsForAllTypesFishing;
                        effortSpecification = new EffortSpecification
                        {
                            ID = fes.ID,
                            Name = _oldName,
                            IsForAllTypesFishing = _oldIsForAllTypesFishing,
                            ValueType = fes.ValueType
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name", DisplayOrder = 1, Description = "Name of specification" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsForAllTypesFishing", DisplayName = "This is a universal effort specification", DisplayOrder = 2, Description = "Whether spec is universal and is applicable for all types of fishing" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ValueType", DisplayName = "Type of value required", DisplayOrder = 3, Description = "Value required for the spec" });

                    if (!_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier of the specification in the database" });

                    PropertyGrid.SelectedObject = effortSpecification;
                    break;

                case NSAPEntity.Municipality:
                    Title = "Municipality";
                    MunicipalityEdit munEdit = new MunicipalityEdit();
                    LabelTop.Content = "New municipality";
                    if (!_isNew)
                    {
                        munEdit = new MunicipalityEdit((Municipality)_nsapObject);
                        LabelTop.Content = "Details of municipality";
                        _oldName = munEdit.MunicipalityName;
                    }
                    else
                    {
                        munEdit.MunicipalityID = MunicipalityRepository.MunicipalityMaxRecordNumber() + 1;
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MunicipalityID", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Identifier of the municipality in database." });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ProvinceID", DisplayName = "Province", DisplayOrder = 2, Description = "Province of municipality" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MunicipalityName", DisplayName = "Name of municipality", DisplayOrder = 3, Description = "Name of municipality" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsCoastal", DisplayName = "Is this a coastal municipality?", DisplayOrder = 4, Description = "Whether or not this municipality is coastal" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Longitude", DisplayName = "Longitude", DisplayOrder = 5, Description = "Longitude of municipality's location" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Latitude", DisplayName = "Latitude", DisplayOrder = 6, Description = "Latitude of municipality's location" });
                    PropertyGrid.SelectedObject = munEdit;
                    break;

                case NSAPEntity.Province:
                    Title = "Province";
                    Province prv = new Province();
                    LabelTop.Content = "New province";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of province";
                        var p = NSAPEntities.ProvinceViewModel.GetProvince(int.Parse(_entityID));
                        _oldName = p.ProvinceName;
                        prv = new Province
                        {
                            ProvinceID = p.ProvinceID,
                            ProvinceName = _oldName,
                            Municipalities = p.Municipalities
                        };
                    }
                    else
                    {
                        prv.ProvinceID = NSAPEntities.ProvinceViewModel.NextRecordNumber;
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ProvinceID", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Identifier of the province in database." });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ProvinceName", DisplayName = "Name of province", DisplayOrder = 2, Description = "Name of province" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "MunicipalityCount", DisplayName = "Number of municipalities", DisplayOrder = 3, Description = "Number of municipalities in the province" });
                    PropertyGrid.SelectedObject = prv;
                    break;

                case NSAPEntity.FishingGround:
                    Title = "Fishing ground";
                    FishingGround fishingGround = new FishingGround();
                    LabelTop.Content = "New fishing ground";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of fishing ground";
                        var fg = NSAPEntities.FishingGroundViewModel.GetFishingGround(_entityID);
                        _oldName = fg.Name;
                        _oldIdentifier = fg.Code;
                        fishingGround = new FishingGround
                        {
                            Code = _oldIdentifier,
                            Name = _oldName
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Code", DisplayName = "Database identifier", DisplayOrder = 1, Description = "Identifier of the fishing ground in database. Cannot be changed once saved" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name of fishing ground", DisplayOrder = 2, Description = "Name of fishing ground" });
                    PropertyGrid.SelectedObject = fishingGround;

                    if (!_isNew)
                    {
                        MakePropertyReadOnly("Code");
                    }
                    break;

                case NSAPEntity.NSAPRegion:
                    Title = "NSAP Region";
                    NSAPRegionEdit nsapRegionEdit = new NSAPRegionEdit();
                    if (!_isNew)
                    {
                        var nsr = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(_entityID);
                        nsapRegionEdit = new NSAPRegionEdit(nsr);
                        LabelTop.Content = $"Details for {nsr}";
                    }

                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name", DisplayOrder = 1, Description = "Name of region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ShortName", DisplayName = "Short name", DisplayOrder = 2, Description = "Short name of region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FMAs", DisplayName = "Number of FMAs", DisplayOrder = 3, Description = "Number of FMAs included in the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Gears", DisplayName = "Number of Gears", DisplayOrder = 4, Description = "Number of gear types used in the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Vessels", DisplayName = "Number of Vessels", DisplayOrder = 5, Description = "Number of vessels listed in the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Enumerators", DisplayName = "Number of Enumerators", DisplayOrder = 6, Description = "Number of enumerators listed in the region" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier of the landing site in database" });
                    PropertyGrid.SelectedObject = nsapRegionEdit;
                    break;

                case NSAPEntity.FMA:
                    FMA fma = new FMA();
                    Title = "FMA";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of Fisheries Management Area (FMA)";
                        var f = NSAPEntities.FMAViewModel.GetFMA(int.Parse(_entityID));
                        fma = new FMA
                        {
                            FMAID = f.FMAID,
                            Name = f.Name
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name", DisplayOrder = 1, Description = "Name of FMA" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ID", DisplayName = "Database identifier", DisplayOrder = 2, Description = "Identifier of FMA in the database" });
                    PropertyGrid.SelectedObject = fma;
                    break;

                case NSAPEntity.LandingSite:
                    LandingSiteEdit landingSiteEdit = new LandingSiteEdit();
                    LabelTop.Content = "New landing site";
                    Title = "Landing site";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of landing site";
                        landingSiteEdit = new LandingSiteEdit(NSAPEntities.LandingSiteViewModel.GetLandingSite(int.Parse(_entityID)));
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name", DisplayOrder = 1, Description = "Name of landing site" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Province", DisplayName = "Province", DisplayOrder = 2, Description = "Province where landing site is located" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Municipality", DisplayName = "Municipality", DisplayOrder = 3, Description = "Municipality of landing site" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Barangay", DisplayName = "Barangay", DisplayOrder = 4, Description = "Barangay of landing site" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Longitude", DisplayName = "Longitude", DisplayOrder = 5, Description = "Longitude of landing site's location" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Latitude", DisplayName = "Latitude", DisplayOrder = 6, Description = "Latitude of landing site's location" });
                    if (!_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ID", DisplayName = "Database identifier", DisplayOrder = 7, Description = "Identifier of the landing site in database" });

                    PropertyGrid.SelectedObject = landingSiteEdit;
                    break;
                case NSAPEntity.GPS:
                    LabelTop.Content = "New GPS";
                    Title = "GPS";
                    GPS gps = new GPS();
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of GPS";
                        var g = NSAPEntities.GPSViewModel.GetGPS(_entityID);
                        _oldName = g.AssignedName;
                        _oldIdentifier = g.Code;
                        gps = new GPS
                        {
                            Code = _oldIdentifier,
                            AssignedName = _oldName,
                            Brand = g.Brand,
                            Model = g.Model
                        };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Code", DisplayName = "Code", DisplayOrder = 1, Description = "Code of gear. Must be made of 3 characters. Cannot be changed once sved" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "AssignedName", DisplayName = "Assigned name", DisplayOrder = 2, Description = "Assigned name of GPS" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Brand", DisplayName = "GPS brand", DisplayOrder = 3, Description = "Brand name of GPS" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Model", DisplayName = "Name of model", DisplayOrder = 4, Description = "Model name of GPS" });


                    PropertyGrid.SelectedObject = gps;

                    if (!_isNew)
                    {
                        MakePropertyReadOnly("Code");
                    }
                    break;
                case NSAPEntity.FishingGear:
                    GearEdit gearEdit = new GearEdit();
                    LabelTop.Content = "New fishing gear";
                    Title = "Fishing gear";
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of fishing gear";
                        gearEdit = new GearEdit(NSAPEntities.GearViewModel.GetGear(_entityID));
                        _oldName = gearEdit.GearName;
                        _oldIdentifier = gearEdit.Code;
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Code", DisplayName = "Code", DisplayOrder = 1, Description = "Code of gear not to exceed 5 characters in length. Must be made of 1 to 5 characters. Cannot be changed once sved" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "GearName", DisplayName = "Name", DisplayOrder = 2, Description = "Name of gear" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "IsGeneric", DisplayName = "Gear is generic", DisplayOrder = 3, Description = "Whether or not this gear is generic" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "BaseGear", DisplayName = "Name of base gear", DisplayOrder = 4, Description = "Gear from which current gear is derived" });

                    if (!_isNew)
                    {
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "EffortSpecifiers", DisplayName = "Number of effort specifiers", DisplayOrder = 4, Description = "Count of specifiers of effort for this gear" });
                    }

                    PropertyGrid.SelectedObject = gearEdit;

                    if (!_isNew)
                    {
                        MakePropertyReadOnly("Code");
                    }
                    break;

                case NSAPEntity.Enumerator:
                    LabelTop.Content = "New sampling enumerator";
                    Title = "Enumerator";
                    NSAPEnumerator nsapEnumerator = new NSAPEnumerator();
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of sampling enumerator";
                        var ne = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator(int.Parse(_entityID));
                        nsapEnumerator = new NSAPEnumerator { ID = ne.ID, Name = ne.Name };
                    }
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name of enumerator", DisplayOrder = 1, Description = "Name of enumerator" });

                    if (!_isNew)
                        PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ID", DisplayName = "Database identifier", DisplayOrder = 2, Description = "Identifier of the landing site in database" });

                    PropertyGrid.SelectedObject = nsapEnumerator;
                    break;

                case NSAPEntity.FishingVessel:
                    LabelTop.Content = "New fishing vessel";
                    Title = "Fishing vessel";
                    FishingVessel fishingVessel = new FishingVessel();
                    if (!_isNew)
                    {
                        LabelTop.Content = "Details of fishing vessel";
                        var fv = NSAPEntities.FishingVesselViewModel.GetFishingVessel(int.Parse(_entityID));
                        fishingVessel = new FishingVessel
                        {
                            ID = fv.ID,
                            Length = fv.Length,
                            Depth = fv.Depth,
                            Breadth = fv.Breadth,
                            RegistrationNumber = fv.RegistrationNumber,
                            Name = fv.Name,
                            NameOfOwner = fv.NameOfOwner,
                            FisheriesSector = fv.FisheriesSector
                        };
                    }
                    else
                    {
                        fishingVessel.FisheriesSector = FisheriesSector.Municipal;
                        fishingVessel.ID = NSAPEntities.FishingVesselViewModel.NextRecordNumber;
                    }

                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "FisheriesSector", DisplayName = "Sector", DisplayOrder = 1, Description = "Fisheries sector" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Name", DisplayName = "Name of vessel", DisplayOrder = 2, Description = "Name of fishing vessel" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "NameOfOwner", DisplayName = "Name of owner", DisplayOrder = 3, Description = "Name of owner" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "RegistrationNumber", DisplayName = "Registration number", DisplayOrder = 4, Description = "Registration number of vessel" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Length", DisplayName = "Length", DisplayOrder = 5, Description = "Length of vessel" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Depth", DisplayName = "Depth", DisplayOrder = 6, Description = "Depth of vessel" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "Breadth", DisplayName = "Breadth", DisplayOrder = 7, Description = "Breadth of vessel" });
                    PropertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "ID", DisplayName = "Database identifier", DisplayOrder = 8, Description = "Identifier of the vessel in database" });

                    PropertyGrid.SelectedObject = fishingVessel;

                    break;
            }
            foreach (PropertyItem prp in PropertyGrid.Properties)
            {
                if (prp.Value != null && prp.Value.GetType().Name == "DateTime" && ((DateTime)prp.Value).ToOADate() == 0)
                {
                    prp.Value = DateTime.Parse("Jan-1-2010");
                }
            }
        }

        private void MakePropertyReadOnly(string propertyName)
        {
            foreach (PropertyItem prp in PropertyGrid.Properties)
            {
                if (prp.PropertyName == propertyName)
                {
                    var label = new Label();
                    label.Content = prp.Value;
                    label.BorderThickness = new Thickness(0);
                    prp.Editor = label;
                    break;
                }
            }
        }

        private void OnFormLoaded(object sender, RoutedEventArgs e)
        {
            _rowBottomLabelHeight = rowBottomLabel.Height;
            _rowDataGridHeight = rowDataGrid.Height;
            rowDataGrid.Height = new GridLength(0);
            rowBottomLabel.Height = new GridLength(0);
            FillPropertyGrid();
            buttonEdit.IsEnabled = false;
            buttonDelete.IsEnabled = false;
        }

        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            switch (cbo.Tag.ToString())
            {
                case "FishingGears":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "BaseGear")
                        {
                            prp.Value = ((KeyValuePair<string, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;
                case "FishingVessels":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "FishingVesselID")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;
                case "Enumerator":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "EnumeratorID")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;

                case "Gear":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "GearCode")
                        {
                            prp.Value = ((KeyValuePair<string, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;

                case "Province":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Province")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;

                case "Municipality":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Municipality")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;

                case "FishingVessel":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "FishingVesselID")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;

                case "LandingSite":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "LandingSiteID")
                        {
                            prp.Value = ((KeyValuePair<int, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;

                case "FishingGround":
                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "FishingGroundCode")
                        {
                            prp.Value = ((KeyValuePair<string, string>)cbo.SelectedItem).Key;
                            return;
                        }
                    }
                    break;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            bool cancel = false;
            bool success = false;
            EditWindowEx ewx = null;
            switch (((Button)sender).Name)
            {
                case "buttonDelete":
                    NSAPRegion nsr = NSAPEntities.NSAPRegionViewModel.CurrentEntity;
                    var entitiesRepository = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nsr);
                    switch (_nsapEntity)
                    {
                        case NSAPEntity.NSAPRegionFMAFishingGround:
                            NSAPRegionFMAFishingGround nrfg = (NSAPRegionFMAFishingGround)_nsapObject;
                            NSAPRegionFMAFishingGroundLandingSite regionLandingSite = (NSAPRegionFMAFishingGroundLandingSite)sfDataGrid.SelectedItem;

                            if (entitiesRepository.DeleteLandingSite(regionLandingSite.RowID))
                            {
                                success = nrfg.LandingSites.Remove(regionLandingSite);
                            }
                            break;
                        case NSAPEntity.NSAPRegionFMA:
                            NSAPRegionFMA regionFMA = (NSAPRegionFMA)_nsapObject;
                            NSAPRegionFMAFishingGround regionFishingGround = (NSAPRegionFMAFishingGround)sfDataGrid.SelectedItem;
                            if (entitiesRepository.DeleteFishingGround(regionFishingGround.RowID))
                            {
                                success = regionFMA.FishingGrounds.Remove(regionFishingGround);

                            }
                            else
                            {
                                MessageBox.Show(entitiesRepository.DatabaseErrorMessage,
                                    "Cannot delete fishing ground",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                            }
                            break;
                        case NSAPEntity.NSAPRegion:
                            NSAPRegion region = (NSAPRegion)_nsapObject;
                            if (sfDataGrid.SelectedItem != null)
                            {
                                switch (_selectedProperty)
                                {
                                    case "Gears":

                                        NSAPRegionGear regionGear = (NSAPRegionGear)sfDataGrid.SelectedItem;
                                        if (entitiesRepository.DeleteGear(regionGear.RowID))
                                        {
                                            success = nsr.Gears.Remove(regionGear);
                                        }
                                        break;
                                    case "Vessels":
                                        NSAPRegionFishingVessel regionVessel = (NSAPRegionFishingVessel)sfDataGrid.SelectedItem;
                                        if (entitiesRepository.DeleteFishingVessel(regionVessel.RowID))
                                        {
                                            success = nsr.FishingVessels.Remove(regionVessel);
                                        }
                                        break;
                                    case "Enumerators":
                                        NSAPRegionEnumerator regionEnumerator = (NSAPRegionEnumerator)sfDataGrid.SelectedItem;
                                        if (entitiesRepository.DeleteEnumerator(regionEnumerator.RowID))
                                        {
                                            success = nsr.NSAPEnumerators.Remove(regionEnumerator);
                                        }
                                        break;
                                }
                            }
                            break;
                    }

                    if (success)
                    {
                        SetUpSubFormSource();
                        PropertyGrid.Update();
                    }
                    break;
                case "buttonOK":
                    List<EntityValidationMessage> entityMessages = new List<EntityValidationMessage>();
                    EntityValidationResult validationResult = null;
                    var nsapRegion = NSAPEntities.NSAPRegionViewModel.CurrentEntity;

                    NSAPRegionWithEntitiesRepository rvm = null;

                    switch (_nsapEntity)
                    {
                        case NSAPEntity.GPS:
                            GPS gps = (GPS)PropertyGrid.SelectedObject;
                            validationResult = NSAPEntities.GPSViewModel.ValidateGPS(gps, _isNew, _oldName, _oldIdentifier);
                            if (validationResult.WarningMessage.Length > 0)
                            {

                            }
                            else if (validationResult.ErrorMessage.Length > 0)
                            {
                                MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                cancel = true;
                            }

                            if (!cancel)
                            {
                                if (_isNew)

                                {
                                    NSAPEntities.GPSViewModel.AddRecordToRepo(gps);
                                }
                                else
                                {
                                    NSAPEntities.GPSViewModel.UpdateRecordInRepo(gps);
                                }
                                success = true;
                            }
                            break;
                        case NSAPEntity.Municipality:
                            var munEdit = (MunicipalityEdit)PropertyGrid.SelectedObject;
                            Municipality municipality = new Municipality
                            {
                                MunicipalityID = munEdit.MunicipalityID,
                                MunicipalityName = munEdit.MunicipalityName,
                                Province = NSAPEntities.ProvinceViewModel.GetProvince(munEdit.ProvinceID),
                                IsCoastal = munEdit.IsCoastal,
                                Latitude = munEdit.Latitude,
                                Longitude = munEdit.Longitude
                            };
                            if (municipality.MunicipalityName != null && municipality.Province != null && municipality.MunicipalityName.Length > 0)
                            {
                                validationResult = municipality.Province.Municipalities.ValidateMunicipality(municipality, _isNew, _oldName);
                                if (validationResult.ErrorMessage.Length > 0)
                                {
                                    MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    cancel = true;
                                }
                                else if (validationResult.WarningMessage.Length > 0)
                                {
                                    var dialogResult = MessageBox.Show(validationResult.WarningMessage + "\r\n\r\nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                    if (dialogResult == MessageBoxResult.No)
                                    {
                                        cancel = true;
                                    }
                                }

                                if (!cancel)
                                {
                                    if (_isNew)
                                    {
                                        municipality.Province.Municipalities.AddRecordToRepo(municipality);
                                    }
                                    else
                                    {
                                        municipality.Province.Municipalities.UpdateRecordInRepo(municipality);
                                    }
                                    success = true;
                                }
                            }
                            break;

                        case NSAPEntity.Province:
                            var prv = (Province)PropertyGrid.SelectedObject;
                            Province provice = new Province
                            {
                                ProvinceID = prv.ProvinceID,
                                ProvinceName = prv.ProvinceName
                            };
                            break;

                        case NSAPEntity.NonFishSpecies:
                            var nfe = (NotFishSpeciesEdit)PropertyGrid.SelectedObject;
                            NotFishSpecies notFish = new NotFishSpecies
                            {
                                Genus = nfe.Genus,
                                Species = nfe.Species,
                                SpeciesID = nfe.SpeciesID,
                                MaxSize = nfe.MaxSize,
                                SizeType = NSAPEntities.SizeTypeViewModel.GetSizeType(nfe.SizeTypeCode),
                                Taxa = NSAPEntities.TaxaViewModel.GetTaxa(nfe.TaxaCode)
                            };
                            validationResult = NSAPEntities.NotFishSpeciesViewModel.ValidateNonFishSpecies(notFish, _isNew, _oldGenus, _oldSpecies);
                            if (validationResult.ErrorMessage.Length > 0)
                            {
                                MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                cancel = true;
                            }
                            else if (validationResult.WarningMessage.Length > 0)
                            {
                                var dialogResult = MessageBox.Show(validationResult.WarningMessage + "\r\n\r\nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (dialogResult == MessageBoxResult.No)
                                {
                                    cancel = true;
                                }
                            }

                            if (!cancel)
                            {
                                if (_isNew)
                                {
                                    NSAPEntities.NotFishSpeciesViewModel.AddRecordToRepo(notFish);
                                }
                                else
                                {
                                    NSAPEntities.NotFishSpeciesViewModel.UpdateRecordInRepo(notFish);
                                }
                                success = true;
                            }
                            break;

                        case NSAPEntity.FishSpecies:
                            var fse = (FishSpeciesEdit)PropertyGrid.SelectedObject;
                            FishSpecies fishSpecies = new FishSpecies
                            {
                                GenericName = fse.GenericName,
                                SpecificName = fse.SpecificName,
                                RowNumber = fse.RowNumber,
                                SpeciesCode = fse.SpeciesCode,
                                Family = fse.Family,
                                Importance = fse.Importance,
                                MainCatchingMethod = fse.MainCatchingMethod,
                                LengthMax = fse.MaxLength,
                                LengthCommon = fse.CommonLength,
                                LengthType = NSAPEntities.SizeTypeViewModel.GetSizeType(fse.LengthType)
                            };
                            validationResult = NSAPEntities.FishSpeciesViewModel.ValidateFishSpecies(fishSpecies, _isNew, _oldGenus, _oldSpecies);
                            if (validationResult.ErrorMessage.Length > 0)
                            {
                                MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                cancel = true;
                            }
                            else if (validationResult.WarningMessage.Length > 0)
                            {
                                var dialogResult = MessageBox.Show(validationResult.WarningMessage + "\r\n\r\nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (dialogResult == MessageBoxResult.No)
                                {
                                    cancel = true;
                                }
                            }

                            if (!cancel)
                            {
                                if (_isNew)
                                {
                                    NSAPEntities.FishSpeciesViewModel.AddRecordToRepo(fishSpecies);
                                }
                                else
                                {
                                    NSAPEntities.FishSpeciesViewModel.UpdateRecordInRepo(fishSpecies);
                                }
                                success = true;
                            }
                            break;

                        case NSAPEntity.NSAPRegion:
                        case NSAPEntity.NSAPRegionFMA:
                            success = true;
                            break;

                        case NSAPEntity.NSAPRegionEnumerator:
                            rvm = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nsapRegion);
                            var regionEnumerator = (NSAPRegionEnumerator)PropertyGrid.SelectedObject;
                            regionEnumerator.NSAPRegion = nsapRegion;
                            if (regionEnumerator.EnumeratorID > 0)
                            {
                                regionEnumerator.Enumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator(regionEnumerator.EnumeratorID);
                                validationResult = rvm.ValidateNSAPEnumerator(regionEnumerator, _isNew, (NSAPRegionEnumerator)_nsapObject);
                                if (validationResult.WarningMessage.Length > 0)
                                {
                                }
                                else if (validationResult.ErrorMessage.Length > 0)
                                {
                                    cancel = true;
                                    MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                if (!cancel)
                                {
                                    if (_isNew)
                                    {
                                        regionEnumerator.RowID = NSAPEntities.GetMaxItemSetID() + 1;
                                        //regionEnumerator.RowID = NSAPRegionWithEntitiesRepository.MaxRecordNumber_Enumerator() + 1;
                                        success = rvm.AddEnumerator(regionEnumerator);
                                    }
                                    else
                                    {
                                        success = rvm.EditEnumerator(regionEnumerator);
                                    }
                                }
                            }
                            break;

                        case NSAPEntity.FishingGearEffortSpecification:
                            var gearEffortSpec = (GearEffortSpecification)PropertyGrid.SelectedObject;
                            gearEffortSpec.Gear = NSAPEntities.GearViewModel.CurrentEntity;
                            gearEffortSpec.EffortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(gearEffortSpec.EffortSpecificationID);
                            validationResult = gearEffortSpec.Gear.GearEffortSpecificationViewModel.ValidateGearEfforSpecifier(gearEffortSpec, _isNew);

                            if (validationResult.WarningMessage.Length > 0)
                            {
                            }
                            else if (validationResult.ErrorMessage.Length > 0)
                            {
                                cancel = true;
                                MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            if (!cancel)
                            {
                                if (_isNew)
                                {
                                    gearEffortSpec.RowID = gearEffortSpec.Gear.GearEffortSpecificationViewModel.NextRecordNumber + 1;
                                    gearEffortSpec.Gear.GearEffortSpecificationViewModel.AddRecordToRepo(gearEffortSpec);
                                }
                                else
                                {
                                    gearEffortSpec.Gear.GearEffortSpecificationViewModel.UpdateRecordInRepo(gearEffortSpec);
                                }
                                success = true;
                            }
                            break;

                        case NSAPEntity.NSAPRegionFMAFishingGroundLandingSite:
                            rvm = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nsapRegion);
                            var fmaFishngGroundLandingSiteEdit = (NSAPRegionFMAFishingGroundLandingSiteEdit)PropertyGrid.SelectedObject;
                            if (fmaFishngGroundLandingSiteEdit.LandingSiteID > 0)
                            {
                                fmaFishngGroundLandingSiteEdit.FMAFishingGround = ((NSAPRegionFMAFishingGroundLandingSite)_nsapObject).NSAPRegionFMAFishingGround;
                                fmaFishngGroundLandingSiteEdit.LandingSite = NSAPEntities.LandingSiteViewModel.GetLandingSite(fmaFishngGroundLandingSiteEdit.LandingSiteID);
                                var nsapRegionFMAFishingGroundLandingSite = fmaFishngGroundLandingSiteEdit.NSAPRegionFMAFishingGroundLandingSite;
                                validationResult = rvm.ValidateFishingGroundLandingSite(nsapRegionFMAFishingGroundLandingSite, _isNew, (NSAPRegionFMAFishingGroundLandingSite)_nsapObject);
                                if (validationResult.WarningMessage.Length > 0)
                                {
                                }
                                else if (validationResult.ErrorMessage.Length > 0)
                                {
                                    cancel = true;
                                    MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                if (!cancel)
                                {
                                    if (_isNew)
                                    {
                                        nsapRegionFMAFishingGroundLandingSite.RowID = NSAPEntities.GetMaxItemSetID() + 1;
                                        success = rvm.AddFMAFishingGroundLandingSite(nsapRegionFMAFishingGroundLandingSite);
                                    }
                                    else
                                    {
                                        success = rvm.EditLandingSite(nsapRegionFMAFishingGroundLandingSite);
                                    }
                                }
                            }
                            break;

                        case NSAPEntity.NSAPRegionFMAFishingGround:
                            rvm = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nsapRegion);
                            var fmaFishngGround = (NSAPRegionFMAFishingGround)PropertyGrid.SelectedObject;
                            if (fmaFishngGround.FishingGroundCode != null)
                            {
                                fmaFishngGround.FishingGround = NSAPEntities.FishingGroundViewModel.GetFishingGround(fmaFishngGround.FishingGroundCode);
                                fmaFishngGround.RegionFMA = ((NSAPRegionFMAFishingGround)_nsapObject).RegionFMA;
                                validationResult = rvm.ValidateNSAPRegionFMAFishingGround(fmaFishngGround, _isNew, (NSAPRegionFMAFishingGround)_nsapObject);
                                if (validationResult.WarningMessage.Length > 0)
                                {
                                }
                                else if (validationResult.ErrorMessage.Length > 0)
                                {
                                    cancel = true;
                                    MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                if (!cancel)
                                {
                                    if (_isNew)
                                    {
                                        fmaFishngGround.RowID = NSAPEntities.GetMaxItemSetID() + 1;
                                        success = rvm.AddFMAFishingGround(fmaFishngGround);
                                    }
                                    else
                                    {
                                        success = rvm.EditFMAFishingGround(fmaFishngGround);
                                    }
                                }
                            }
                            break;

                        case NSAPEntity.NSAPRegionFishingVessel:
                            rvm = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nsapRegion);
                            var regionVesselEdit = (NSAPRegionFishingVesselEdit)PropertyGrid.SelectedObject;
                            if (regionVesselEdit.FishingVesselID > 0)
                            {
                                regionVesselEdit.NSAPRegion = nsapRegion;
                                regionVesselEdit.FishingVessel = NSAPEntities.FishingVesselViewModel.GetFishingVessel(regionVesselEdit.FishingVesselID);
                                var regionVessel = new NSAPRegionFishingVessel
                                {
                                    DateEnd = regionVesselEdit.DateEnd,
                                    DateStart = regionVesselEdit.DateStart,
                                    FishingVessel = regionVesselEdit.FishingVessel,
                                    NSAPRegion = regionVesselEdit.NSAPRegion,
                                    RowID = regionVesselEdit.RowID
                                };
                                validationResult = rvm.ValidateNSAPRegionFishingVessel(regionVessel, _isNew, (NSAPRegionFishingVessel)_nsapObject);
                                if (validationResult.WarningMessage.Length > 0)
                                {
                                }
                                else if (validationResult.ErrorMessage.Length > 0)
                                {
                                    cancel = true;
                                    MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                if (!cancel)
                                {
                                    if (_isNew)
                                    {
                                        success = rvm.AddFishingVessel(regionVessel);
                                    }
                                    else
                                    {
                                        success = rvm.EditFishingVessel(regionVessel);
                                    }
                                }
                            }
                            break;

                        case NSAPEntity.NSAPRegionGear:
                            rvm = NSAPEntities.NSAPRegionViewModel.GetNSAPRegionWithEntitiesRepository(nsapRegion);
                            var regionGear = (NSAPRegionGear)PropertyGrid.SelectedObject;
                            if (regionGear.GearCode != null)
                            {
                                regionGear.NSAPRegion = nsapRegion;

                                regionGear.Gear = NSAPEntities.GearViewModel.GetGear(regionGear.GearCode);
                                validationResult = rvm.ValidateNSAPRegionGear(regionGear, _isNew, (NSAPRegionGear)_nsapObject);
                                if (validationResult.WarningMessage.Length > 0)
                                {
                                }
                                else if (validationResult.ErrorMessage.Length > 0)
                                {
                                    cancel = true;
                                    MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                if (!cancel)
                                {
                                    if (_isNew)
                                    {
                                        regionGear.RowID = NSAPEntities.GetMaxItemSetID() + 1;
                                        success = rvm.AddGear(regionGear);
                                    }
                                    else
                                    {
                                        success = rvm.EditGear(regionGear);
                                    }
                                }
                            }
                            break;

                        case NSAPEntity.EffortIndicator:
                            var effortSpec = (EffortSpecification)PropertyGrid.SelectedObject;
                            validationResult = NSAPEntities.EffortSpecificationViewModel.EntityValidated(effortSpec, _isNew, _oldName, _oldIsForAllTypesFishing);
                            if (validationResult.ErrorMessage.Length > 0)
                            {
                                MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                cancel = true;
                            }
                            else if (validationResult.WarningMessage.Length > 0)
                            {
                                var dialogResult = MessageBox.Show(validationResult.WarningMessage + "\r\n\r\nDo you wish to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (dialogResult == MessageBoxResult.No)
                                {
                                    cancel = true;
                                }
                            }

                            if (!cancel)
                            {
                                if (_isNew)
                                {
                                    effortSpec.ID = NSAPEntities.GetMaxItemSetID() + 1;
                                    NSAPEntities.EffortSpecificationViewModel.AddRecordToRepo(effortSpec);
                                }
                                else
                                {
                                    NSAPEntities.EffortSpecificationViewModel.UpdateRecordInRepo(effortSpec);

                                }
                                success = true;

                                if (_isNew && effortSpec.IsForAllTypesFishing)
                                {
                                    NSAPEntities.GearViewModel.AddUniversalSpec(effortSpec);



                                }
                                else if (!_isNew && _oldIsForAllTypesFishing != effortSpec.IsForAllTypesFishing)
                                {
                                    if (_oldIsForAllTypesFishing)
                                    {
                                        //this is now an effort spec for all types of fishing
                                    }
                                    else if (!_oldIsForAllTypesFishing)
                                    {
                                        //this is now an effort not for all types of fishing
                                    }
                                }

                            }
                            break;

                        case NSAPEntity.FishingVessel:
                            FishingVessel fishingVessel = (FishingVessel)PropertyGrid.SelectedObject;
                            if (NSAPEntities.FishingVesselViewModel.EntityValidated(fishingVessel, out entityMessages, _isNew))
                            {
                                if (_isNew)
                                {
                                    NSAPEntities.FishingVesselViewModel.AddRecordToRepo(fishingVessel);
                                }
                                else
                                {
                                    NSAPEntities.FishingVesselViewModel.UpdateRecordInRepo(fishingVessel);
                                }
                                success = true;
                            }
                            break;

                        case NSAPEntity.Enumerator:
                            NSAPEnumerator nse = (NSAPEnumerator)PropertyGrid.SelectedObject;
                            if (NSAPEntities.NSAPEnumeratorViewModel.EntityValidated(nse, out entityMessages, _isNew))
                            {
                                if (_isNew)
                                {
                                    nse.ID = NSAPEntities.NSAPEnumeratorViewModel.NextRecordNumber;
                                    NSAPEntities.NSAPEnumeratorViewModel.AddRecordToRepo(nse);
                                }
                                else
                                {
                                    NSAPEntities.NSAPEnumeratorViewModel.UpdateRecordInRepo(nse);
                                }
                                success = true;
                            }
                            break;

                        case NSAPEntity.LandingSite:
                            var lsEdit = (LandingSiteEdit)PropertyGrid.SelectedObject;
                            var prov = NSAPEntities.ProvinceViewModel.GetProvince(lsEdit.Province);
                            LandingSite landingSite = new LandingSite
                            {
                                LandingSiteName = lsEdit.Name,
                                Municipality = prov.Municipalities.GetMunicipality(lsEdit.Municipality),
                                Latitude = lsEdit.Latitude,
                                Longitude = lsEdit.Longitude,
                                Barangay = lsEdit.Barangay
                            };
                            landingSite.LandingSiteID = _isNew ? NSAPEntities.LandingSiteViewModel.NextRecordNumber : lsEdit.ID;
                            validationResult = NSAPEntities.LandingSiteViewModel.EntityValidated(landingSite, _isNew);

                            if (validationResult.WarningMessage.Length > 0)
                            {
                            }
                            else if (validationResult.ErrorMessage.Length > 0)
                            {
                                cancel = true;
                                MessageBox.Show(validationResult.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            if (!cancel)
                            {
                                if (_isNew)
                                {
                                    NSAPEntities.LandingSiteViewModel.AddRecordToRepo(landingSite);
                                }
                                else
                                {
                                    NSAPEntities.LandingSiteViewModel.UpdateRecordInRepo(landingSite);
                                }
                                success = true;
                            }

                            break;

                        case NSAPEntity.FishingGround:
                            FishingGround fg = (FishingGround)PropertyGrid.SelectedObject;
                            if (NSAPEntities.FishingGroundViewModel.EntityValidated(fg, out entityMessages, _isNew, _oldName, _oldIdentifier))
                            {
                                if (_isNew)
                                {
                                    NSAPEntities.FishingGroundViewModel.AddRecordToRepo(fg);
                                }
                                else
                                {
                                    NSAPEntities.FishingGroundViewModel.UpdateRecordInRepo(fg);
                                }
                                success = true;
                            }
                            break;

                        case NSAPEntity.FishingGear:

                            var g = ((GearEdit)PropertyGrid.SelectedObject).Save(_isNew);
                            Gear gear = new Gear
                            {
                                BaseGear = g.BaseGear,
                                Code = g.Code,
                                GearName = g.GearName,
                                IsGenericGear = g.IsGenericGear
                            };
                            validationResult = NSAPEntities.GearViewModel.ValidateEntity(gear, _isNew, _oldName, _oldIdentifier);
                            if(validationResult.ErrorMessage.Length>0)
                            {
                                cancel = true;
                                MessageBox.Show(validationResult.ErrorMessage, "Validation error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if(validationResult.WarningMessage.Length>0)
                            {

                            }

                            if(!cancel)
                            {
                                if(_isNew)
                                {
                                    NSAPEntities.GearViewModel.AddRecordToRepo(gear);
                                }
                                else
                                {
                                    NSAPEntities.GearViewModel.UpdateRecordInRepo(gear);
                                }
                                success = true;
                            }

                            break;
                    }

                    if (success)
                    {

                        //SetEditWIndowVisibility();

                        if (_isNew)
                        {
                            _isNew = false;
                        }
                        else
                        {

                        }
                        _saveButtonClicked = true;

                        try
                        {
                            this.DialogResult = success;
                            NSAPEntities.EntityToRefresh = NSAPEntity.Nothing;

                        }
                        catch (Exception ex)
                        {
                            NSAPEntities.EntityToRefresh = _nsapEntity;
                        }


                        Close();
                    }
                    else
                    {
                        if (entityMessages.Count > 0)
                        {
                            string errorMessages = "";
                            string warningMessages = "";
                            string infoMessages = "";
                            foreach (var msg in entityMessages)
                            {
                                switch (msg.MessageType)
                                {
                                    case MessageType.Error:
                                        errorMessages += $"{msg.Message}\r\n";
                                        break;

                                    case MessageType.Warning:
                                        warningMessages += $"{msg.Message}\r\n";
                                        break;

                                    case MessageType.Information:
                                        infoMessages += $"{msg.Message}\r\n";
                                        break;
                                }
                            }
                            System.Windows.MessageBox.Show
                                (
                                 $"Information:{infoMessages}\r\nWarnings:{warningMessages}\r\nErrors:{errorMessages}",
                                 "There are messages",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Information
                                );
                        }
                    }
                    break;

                case "buttonCancel":
                    Cancelled = true;
                    //SetEditWIndowVisibility();
                    Close();
                    break;

                case "buttonEdit":
                    OnDataGridMouseDoubleClick(sfDataGrid, null);
                    break;

                case "buttonAdd":
                    switch (_nsapEntity)
                    {
                        case NSAPEntity.Province:
                            ewx = new EditWindowEx(NSAPEntity.Municipality);
                            break;

                        case NSAPEntity.FishingGear:
                            if (_selectedProperty == "EffortSpecifiers")
                            {
                                ewx = new EditWindowEx(NSAPEntity.FishingGearEffortSpecification, "", new GearEffortSpecification());
                                this.Visibility = Visibility.Hidden;
                                _editWindowsDict.Add(_nsapEntity, this);
                            }
                            break;

                        case NSAPEntity.NSAPRegion:

                            if (_selectedProperty == "Gears"
                                || _selectedProperty == "Vessels"
                                || _selectedProperty == "Enumerators")
                            {
                                this.Visibility = Visibility.Hidden;
                                _editWindowsDict.Add(_nsapEntity, this);
                            }
                            switch (_selectedProperty)
                            {
                                case "Gears":
                                    ewx = new EditWindowEx(NSAPEntity.NSAPRegionGear);
                                    break;

                                case "FMAs":
                                    MessageBox.Show("FMAs per region is final and cannot be changed", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    break;

                                case "Vessels":
                                    ewx = new EditWindowEx(NSAPEntity.NSAPRegionFishingVessel);
                                    break;

                                case "Enumerators":
                                    ewx = new EditWindowEx(NSAPEntity.NSAPRegionEnumerator);
                                    break;
                            }

                            break;

                        case NSAPEntity.NSAPRegionFMA:

                            if (_selectedProperty == "FishingGroundCount")
                            {
                                var nsapRegionFishingGround = new NSAPRegionFMAFishingGround();
                                nsapRegionFishingGround.RegionFMA = (NSAPRegionFMA)_nsapObject;
                                this.Visibility = Visibility.Hidden;
                                _editWindowsDict.Add(_nsapEntity, this);
                                ewx = new EditWindowEx(NSAPEntity.NSAPRegionFMAFishingGround, "", nsapRegionFishingGround);
                            }
                            break;

                        case NSAPEntity.NSAPRegionFMAFishingGround:
                            var nsapRegionFMAFishingGroundLandingSite = new NSAPRegionFMAFishingGroundLandingSite();
                            nsapRegionFMAFishingGroundLandingSite.NSAPRegionFMAFishingGround = (NSAPRegionFMAFishingGround)_nsapObject;
                            if (_selectedProperty == "LandingSiteCount")
                            {
                                ewx = new EditWindowEx(NSAPEntity.NSAPRegionFMAFishingGroundLandingSite, "", nsapRegionFMAFishingGroundLandingSite);
                                this.Visibility = Visibility.Hidden;
                                _editWindowsDict.Add(_nsapEntity, this);
                            }
                            break;
                    }

                    if (ewx != null && (bool)ewx.ShowDialog())
                    {


                        sfDataGrid.Items.Refresh();
                        switch (_nsapEntity)
                        {
                            case NSAPEntity.NSAPRegion:
                                ((NSAPRegionEdit)PropertyGrid.SelectedObject).Refresh();
                                break;
                        }

                        PropertyGrid.Update();
                        SetUpSubFormSource();

                    }

                    break;


            }
        }

        private void OnDataGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;

            if (_editWindowsDict.ContainsKey(_nsapEntity))
            {
                _editWindowsDict.Remove(_nsapEntity);
            }

            _editWindowsDict.Add(_nsapEntity, this);
            switch (_selectedProperty)
            {
                case "MunicipalityCount":
                    Municipality mun = (Municipality)sfDataGrid.SelectedItem;
                    if (mun != null)
                    {
                        new EditWindowEx(NSAPEntity.Municipality, mun.MunicipalityID.ToString(), mun).ShowDialog();

                    }
                    break;

                case "LandingSiteCount":
                    NSAPRegionFMAFishingGroundLandingSite nsrls = (NSAPRegionFMAFishingGroundLandingSite)sfDataGrid.SelectedItem;
                    if (nsrls != null)
                    {
                        new EditWindowEx(NSAPEntity.NSAPRegionFMAFishingGroundLandingSite, nsrls.RowID.ToString(), nsrls).ShowDialog();

                    }
                    break;

                case "EffortSpecifiers":
                    GearEffortSpecification es = (GearEffortSpecification)sfDataGrid.SelectedItem;
                    if (es != null)
                    {
                        new EditWindowEx(NSAPEntity.FishingGearEffortSpecification, es.RowID.ToString(), es).ShowDialog();

                    }
                    break;

                case "Gears":
                    NSAPRegionGear nrg = (NSAPRegionGear)sfDataGrid.SelectedItem;
                    if (nrg != null)
                    {
                        new EditWindowEx(NSAPEntity.NSAPRegionGear, nrg.RowID.ToString(), nrg).ShowDialog();

                    }
                    break;

                case "FMAs":
                    NSAPRegionFMA nrfma = (NSAPRegionFMA)sfDataGrid.SelectedItem;
                    if (nrfma != null)
                    {
                        new EditWindowEx(NSAPEntity.NSAPRegionFMA, nrfma.RowID.ToString(), nrfma).ShowDialog();

                    }
                    break;

                case "Enumerators":
                    NSAPRegionEnumerator nre = (NSAPRegionEnumerator)sfDataGrid.SelectedItem;
                    if (nre != null)
                    {
                        new EditWindowEx(NSAPEntity.NSAPRegionEnumerator, nre.RowID.ToString(), nre).ShowDialog();

                    }
                    break;

                case "Vessels":
                    NSAPRegionFishingVessel nrfv = (NSAPRegionFishingVessel)sfDataGrid.SelectedItem;
                    if (nrfv != null)
                    {
                        new EditWindowEx(NSAPEntity.NSAPRegionFishingVessel, nrfv.RowID.ToString(), nrfv).ShowDialog();

                    }
                    break;

                case "FishingGroundCount":
                    NSAPRegionFMAFishingGround nrffg = (NSAPRegionFMAFishingGround)sfDataGrid.SelectedItem;
                    if (nrffg != null)
                    {
                        new EditWindowEx(NSAPEntity.NSAPRegionFMAFishingGround, nrffg.RowID.ToString(), nrffg).ShowDialog();

                    }
                    break;
            }
            SetUpSubFormSource();
            //sfDataGrid.Items.Refresh();
        }


        private void SetEditWIndowVisibility()
        {
            //if MainWindow is closing (CloseCommandFromMainWindow==true), 
            //then we disregard what the dictionary _editWindowsDict is meant to do
            if (_editWindowsDict.Count > 0 && !CloseCommandFromMainWindow)
            {
                switch (_nsapEntity)
                {
                    case NSAPEntity.LandingSite:
                        _editWindowsDict[NSAPEntity.NSAPRegionFMAFishingGroundLandingSite].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionFMAFishingGroundLandingSite);
                        break;
                    case NSAPEntity.FishingGround:
                        _editWindowsDict[NSAPEntity.NSAPRegionFMAFishingGround].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionFMAFishingGround);
                        break;
                    case NSAPEntity.FishingVessel:
                        _editWindowsDict[NSAPEntity.NSAPRegionFishingVessel].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionFishingVessel);
                        break;
                    case NSAPEntity.Enumerator:
                        _editWindowsDict[NSAPEntity.NSAPRegionEnumerator].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionEnumerator);
                        break;
                    case NSAPEntity.FishingGear:
                        _editWindowsDict[NSAPEntity.NSAPRegionGear].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionGear);
                        break;
                    case NSAPEntity.NSAPRegionFMAFishingGroundLandingSite:
                        _editWindowsDict[NSAPEntity.NSAPRegionFMAFishingGround].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionFMAFishingGround);
                        break;
                    case NSAPEntity.NSAPRegionFMAFishingGround:
                        _editWindowsDict[NSAPEntity.NSAPRegionFMA].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegionFMA);
                        break;
                    case NSAPEntity.NSAPRegionFMA:
                        _editWindowsDict[NSAPEntity.NSAPRegion].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegion);
                        break;
                    case NSAPEntity.NSAPRegionGear:
                    case NSAPEntity.NSAPRegionEnumerator:
                    case NSAPEntity.NSAPRegionFishingVessel:
                        _editWindowsDict[NSAPEntity.NSAPRegion].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.NSAPRegion);
                        break;
                    case NSAPEntity.FishingGearEffortSpecification:
                        _editWindowsDict[NSAPEntity.FishingGear].Visibility = Visibility.Visible;
                        _editWindowsDict.Remove(NSAPEntity.FishingGear);
                        break;
                }
            }
        }
        private void ClosingTrigger(object sender, EventArgs e)
        {
            Cancelled = !_saveButtonClicked;
            this.SavePlacement();


            SetEditWIndowVisibility();
        }

        //This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void OnSelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> e)
        {
            rowDataGrid.Height = new GridLength(0);
            rowBottomLabel.Height = rowDataGrid.Height;
            var propertyItem = (PropertyItem)((PropertyGrid)e.Source).SelectedPropertyItem;
            if (propertyItem != null)
                _selectedProperty = propertyItem.PropertyName;

            switch (_selectedProperty)
            {
                case "MunicipalityCount":
                    LabelBottom.Content = $"List of municipalities in {NSAPEntities.ProvinceViewModel.CurrentEntity}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "EffortSpecifiers":
                    LabelBottom.Content = $"List of effort specifiers for {NSAPEntities.GearViewModel.CurrentEntity}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "LandingSiteCount":
                    LabelBottom.Content = $"List of landing sites in {(NSAPRegionFMAFishingGround)_nsapObject}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "FishingGroundCount":
                    LabelBottom.Content = $"List of fishing grounds with landing sites in {((NSAPRegionFMA)_nsapObject)}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "Gears":
                    LabelBottom.Content = $"List of gears in {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "FMAs":
                    LabelBottom.Content = $"List of FMAs with fishing grounds in {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "Vessels":
                    LabelBottom.Content = $"List of vessels in {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;

                case "Enumerators":
                    LabelBottom.Content = $"List of enumerators in {NSAPEntities.NSAPRegionViewModel.CurrentEntity}";
                    rowDataGrid.Height = new GridLength(4, GridUnitType.Star);
                    rowBottomLabel.Height = new GridLength(40);
                    SetUpSubForm();
                    break;
            }
        }

        private void SetUpSubFormSource()
        {
            NSAPRegion nsr = null;
            switch (_selectedProperty)
            {
                case "MunicipalityCount":
                    Province prv = (Province)PropertyGrid.SelectedObject;
                    sfDataGrid.ItemsSource = prv.Municipalities.MunicipalityCollection.OrderBy(t => t.MunicipalityName);
                    break;

                case "EffortSpecifiers":
                    Gear gear = NSAPEntities.GearViewModel.GetGear(_entityID);
                    if (gear.GearEffortSpecificationViewModel != null)
                    {
                        sfDataGrid.ItemsSource = gear.GearEffortSpecificationViewModel.GearEffortSpecificationCollection.OrderBy(t => t.EffortSpecification.Name);
                    }
                    break;

                case "LandingSiteCount":
                    sfDataGrid.ItemsSource = ((NSAPRegionFMAFishingGround)_nsapObject).LandingSites.OrderBy(t => t.LandingSite.LandingSiteName);
                    break;

                case "FishingGroundCount":
                    sfDataGrid.ItemsSource = ((NSAPRegionFMA)_nsapObject).FishingGrounds.OrderBy(t => t.FishingGround.Name);
                    break;

                case "Gears":
                    nsr = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(_entityID);
                    sfDataGrid.ItemsSource = nsr.Gears.OrderBy(t => t.Gear.GearName);
                    break;

                case "FMAs":
                    nsr = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(_entityID);
                    sfDataGrid.ItemsSource = nsr.FMAs.OrderBy(t => t.FMA.Name);
                    break;

                case "Vessels":
                    nsr = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(_entityID);
                    sfDataGrid.ItemsSource = nsr.FishingVessels.OrderBy(t => t.FishingVessel.Name);
                    break;

                case "Enumerators":
                    nsr = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(_entityID);
                    sfDataGrid.ItemsSource = nsr.NSAPEnumerators.OrderBy(t => t.Enumerator.Name);
                    break;
            }
        }

        private void SetUpSubForm()
        {
            sfDataGrid.Columns.Clear();
            //sfDataGrid.Items.Clear();
            SetUpSubFormSource();
            switch (_selectedProperty)
            {
                case "MunicipalityCount":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("MunicipalityID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Municipality", Binding = new Binding("MunicipalityName") });
                    sfDataGrid.Columns.Add(new DataGridCheckBoxColumn { Header = "Coastal", Binding = new Binding("IsCoastal") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Longitude", Binding = new Binding("Longitude") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Latitude", Binding = new Binding("Latitude") });
                    break;

                case "EffortSpecifiers":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Effort specification", Binding = new Binding("EffortSpecification") });
                    break;

                case "LandingSiteCount":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSite") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date added", Binding = new Binding("DateStart") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date removed", Binding = new Binding("DateEnd") });
                    break;

                case "FishingGroundCount":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing ground", Binding = new Binding("FishingGround") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date added", Binding = new Binding("DateStart") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date removed", Binding = new Binding("DateEnd") });

                    var style = new Style(typeof(TextBlock));
                    style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Landing sites", Binding = new Binding("LandingSiteList"), ElementStyle = style });

                    break;

                case "Gears":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Gear.Code") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("Gear") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date added", Binding = new Binding("DateStart") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date removed", Binding = new Binding("DateEnd") });
                    break;

                case "FMAs":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "FMA", Binding = new Binding("FMA.Name") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing grounds", Binding = new Binding("FishingGrounds.Count") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "List of fishing grounds", Binding = new Binding("FishingGroundList") });
                    break;

                case "Vessels":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Fishing vessel", Binding = new Binding("FishingVessel") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("FishingVessel.FisheriesSector") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date added", Binding = new Binding("DateStart") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date removed", Binding = new Binding("DateEnd") });
                    break;

                case "Enumerators":
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("Enumerator.Name") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date added", Binding = new Binding("DateStart") });
                    sfDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date removed", Binding = new Binding("DateEnd") });
                    break;
            }

            foreach (DataGridColumn c in sfDataGrid.Columns)
            {
                if (c.GetType().Name == "DataGridTextColumn" && (((Binding)((DataGridTextColumn)c).Binding).Path.Path == "DateStart"
                    || ((Binding)((DataGridTextColumn)c).Binding).Path.Path == "DateEnd"))
                {
                    ((Binding)((DataGridTextColumn)c).Binding).StringFormat = "MMM-dd-yyyy";
                }
            }
        }

        private void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            ComboBox cbo = new ComboBox();
            cbo.Items.Clear();
            cbo.SelectionChanged += OnComboSelectionChanged;
            cbo.DisplayMemberPath = "Value";
            var currentProperty = (PropertyItem)e.OriginalSource;
            switch (currentProperty.PropertyName)
            {

                case "IsGeneric":
                    cbo.Tag = "FishingGears";
                    foreach (PropertyItem prp in this.PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "BaseGear")
                        {
                            prp.Value = null;
                            prp.Editor = cbo;
                            if (!(bool)currentProperty.Value)
                            {
                                foreach(var baseGear in NSAPEntities.GearViewModel.GearCollection
                                    .Where(t=>t.IsGenericGear==true)
                                    .OrderBy(t=>t.GearName))
                                {
                                    cbo.Items.Add(new KeyValuePair<string, string>(baseGear.Code, baseGear.GearName));
                                }
                            }
                        }
                    }
                    break;
                case "FisheriesSector":
                    cbo.Tag = "FishingVessels";
                    foreach (var fv in NSAPEntities.FishingVesselViewModel.FishingVesselCollection
                        .Where(t => t.FisheriesSector == (FisheriesSector)((PropertyItem)e.OriginalSource).Value)
                        .OrderBy(t => t.ToString()))
                    {
                        cbo.Items.Add(new KeyValuePair<int, string>(fv.ID, fv.ToString()));
                    }

                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "FishingVesselID")
                        {
                            prp.Editor = cbo;
                        }
                    }

                    break;
                case "Municipality":
                    cbo.Tag = "LandingSite";
                    foreach (var ls in NSAPEntities.LandingSiteViewModel.LandingSiteCollection
                        .Where(t => t.Municipality.MunicipalityID == (int)e.NewValue)
                        .OrderBy(t => t.LandingSiteName))
                    {
                        cbo.Items.Add(new KeyValuePair<int, string>(ls.LandingSiteID, ls.LandingSiteName));
                    }

                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "LandingSiteID")
                        {
                            prp.Editor = cbo;
                        }
                    }

                    break;

                case "Province":
                    cbo.Tag = "Municipality";
                    if (_nsapEntity == NSAPEntity.NSAPRegionFMAFishingGroundLandingSite)
                    {
                        List<int> municipalityIDs = new List<int>();
                        foreach (var ls in NSAPEntities.LandingSiteViewModel.LandingSiteCollection.Where(t => t.Municipality.Province.ProvinceID == (int)e.NewValue))
                        {
                            var mun = ls.Municipality;
                            if (!municipalityIDs.Contains(mun.MunicipalityID))
                            {
                                cbo.Items.Add(new KeyValuePair<int, string>(mun.MunicipalityID, mun.MunicipalityName));
                                municipalityIDs.Add(mun.MunicipalityID);
                            }
                        }
                    }
                    else if (_nsapEntity == NSAPEntity.LandingSite)
                    {
                        var province = NSAPEntities.ProvinceViewModel.GetProvince((int)e.NewValue);
                        foreach (var mun in province.Municipalities.MunicipalityCollection
                            .Where(t => t.Province.ProvinceID == (int)e.NewValue)
                            .OrderBy(t => t.MunicipalityName))
                        {
                            cbo.Items.Add(new KeyValuePair<int, string>(mun.MunicipalityID, mun.MunicipalityName));
                        }
                    }
                    else
                    {
                    }

                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "Municipality")
                        {
                            prp.Editor = cbo;
                            break;
                        }
                    }

                    foreach (PropertyItem prp in PropertyGrid.Properties)
                    {
                        if (prp.PropertyName == "LandingSiteID")
                        {
                            prp.Editor = new ComboBox();
                            break;
                        }
                    }

                    break;
            }
        }

    }
}