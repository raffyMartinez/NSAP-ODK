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
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Views
{
    public enum GearUnloadWindowListSource
    {
        ListSourceNone,
        ListSourceGearUnload,
        listSourceVesselUnload,
        ListSourceWeights
    }

    /// <summary>
    /// Interaction logic for GearUnloadWindow.xaml
    /// </summary>
    /// 
    public partial class GearUnloadWindow : Window
    {
        private GearUnload _gearUnload;
        private List<GearUnload> _gearUnloads;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeItemData;
        private VesselUnload _selectedVesselUnload;
        //private VesselUnloadWIndow _vesselUnloadWindow;
        private VesselUnloadEditWindow _vesselUnloadWindow;
        private MainWindow _parentWindow;
        private bool _changeFromGridClick;
        private List<VesselUnload> _vesselUnloads;
        private GearUnloadWindowListSource _listSource;
        private static GearUnloadWindow _instance;
        private List<SummaryItem> _summaryItems;
        private string _watchedSpecies = "";

        public static GearUnloadWindow GetInstance(List<VesselUnload> vesselUnloads)
        {
            NSAPEntities.NSAPRegion = vesselUnloads[0].Parent.Parent.NSAPRegion;
            if (_instance == null) _instance = new GearUnloadWindow(vesselUnloads);
            return _instance;
        }
        public void RefreshAfterDeleteVesselUnload()
        {
            SetVesselUnloadGridContext();
        }
        public static GearUnloadWindow GetInstance(GearUnload gearUnload, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent)
        {
            if (_instance == null) _instance = new GearUnloadWindow(gearUnload, treeItemData, parent);
            return _instance;
        }
        public GearUnloadWindow(List<VesselUnload> vesselUnloads)
        {
            InitializeComponent();
            _vesselUnloads = vesselUnloads;
            _listSource = GearUnloadWindowListSource.listSourceVesselUnload;
            tabItemPageBoatCount.Visibility = Visibility.Collapsed;

        }

        public GearUnloadWindow(List<GearUnload> gearUnloads)
        {
            InitializeComponent();
            _gearUnloads = gearUnloads;
            _listSource = GearUnloadWindowListSource.listSourceVesselUnload;
            tabItemPageBoatCount.Visibility = Visibility.Collapsed;
            rowMenu.Height = new GridLength(0);

        }
        
        public GearUnloadWindow(List<GearUnload> gearUnloads, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent, string sector_code)
        {
            InitializeComponent();
            rowMenu.Height = new GridLength(0);
            SectorCode = sector_code;
            _gearUnloads = gearUnloads;
            _treeItemData = treeItemData;
            if (_gearUnloads.Count > 0)
            {
                //textBoxBoats.Text = _gearUnloads[0].Boats.ToString();
                //textBoxCatch.Text = _gearUnloads[0].Catch.ToString();

                _parentWindow = parent;
                Title = $"Gear unload for {gearUnloads[0].GearUsedName}";
                _listSource = GearUnloadWindowListSource.ListSourceGearUnload;
                tabItemPageBoatCount.Visibility = Visibility.Visible;
            }
        }

        //public GearUnloadWindow(List<ValidateLandedCatchWeight> vlcws, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData)
        public GearUnloadWindow(List<SummaryItem> summaryItems, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData)
        {
            InitializeComponent();

            Loaded += OnWindowLoaded;
            _summaryItems = summaryItems;
            foreach (TabItem item in TabControl.Items)
            {
                item.Visibility = Visibility.Collapsed;
                if (item.Name == "tabItemWeights")
                {
                    item.Visibility = Visibility.Visible;
                }
            }
            _listSource = GearUnloadWindowListSource.ListSourceWeights;
            rowMenu.Height = new GridLength(30);
            LabelTitle.Content = $"Weight validation for landings sampled at {treeItemData.LandingSiteText}, on {((DateTime)treeItemData.MonthSampled).ToString("MMMM, yyyy")}";
            //ShowWeightsGrid();
        }



        public GearUnloadWindow(GearUnload gearUnload, TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData, MainWindow parent)
        {
            InitializeComponent();
            rowMenu.Height = new GridLength(0);
            _gearUnload = gearUnload;
            _treeItemData = treeItemData;
            textBoxBoats.Text = _gearUnload.Boats.ToString();
            textBoxCatch.Text = _gearUnload.Catch.ToString();
            _parentWindow = parent;
            Title = $"Gear unload for {gearUnload.GearUsedName}";
            _listSource = GearUnloadWindowListSource.ListSourceGearUnload;
            tabItemPageBoatCount.Visibility = Visibility.Visible;
        }
        private void ShowWeightsGrid(string filter = "")
        {
            dataGridWeights.IsReadOnly = true;
            if (_summaryItems != null)
            {
                if (filter.Length > 0)
                {
                    List<SummaryItem> datacontext = null;
                    switch (filter)
                    {
                        case "menuFilterValid":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.WeightValidationFlag == WeightValidationFlag.WeightValidationValid).ToList();
                            break;
                        case "menuFilterInvalid":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.WeightValidationFlag == WeightValidationFlag.WeightValidationInValid).ToList();
                            break;
                        case "menuFilterNotApplicable":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.WeightValidationFlag == WeightValidationFlag.WeightValidationNotApplicable || t.VesselUnload.WeightValidationFlag == WeightValidationFlag.WeightValidationNotValidated).ToList();
                            break;
                        case "menuTotalEnumeration":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeTotalEnumeration).ToList();
                            break;
                        case "menuMixedSampling":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeMixed).ToList();
                            break;
                        case "menuAllSampling":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeMixed).ToList();
                            break;
                        case "menuNotSampled":
                            datacontext = _summaryItems.Where(t => t.VesselUnload.SamplingTypeFlag == SamplingTypeFlag.SamplingTypeNone).ToList();
                            break;
                        case "menuFilterReset":
                            datacontext = _summaryItems;
                            break;
                    }
                    dataGridWeights.DataContext = datacontext;
                }
                else
                {
                    dataGridWeights.DataContext = _summaryItems;
                }
            }
            else
            {
                //List<ValidateLandedCatchWeight> vlcws = new List<ValidateLandedCatchWeight>();
                //_summaryItems = new List<SummaryItem>();
                //foreach (var vu in _vesselUnloads)
                //{
                //    ValidateLandedCatchWeight vlcw = new ValidateLandedCatchWeight(vu);
                //    if (vlcw.SummaryItem == null)
                //    {
                //        vlcw.SummaryItem = NSAPEntities.SummaryItemViewModel.GetItem(vu);
                //    }
                //    _summaryItems.Add(vlcw);

                //}
                //dataGridWeights.DataContext = _summaryItems;
            }

            dataGridWeights.AutoGenerateColumns = false;
            dataGridWeights.Columns.Clear();
            //dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("SummaryItem.VesselUnloadID") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("SamplingDateFormatted") });
            //dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("SummaryItem.EnumeratorName") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearUsedName") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Ref #", Binding = new Binding("RefNo") });
            //dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Form version", Binding = new Binding("VesselUnload.FormVersionNumeric") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Vessel", Binding = new Binding("VesselNameToUse") });

            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("VesselUnload.WeightOfCatch"), CellStyle = AlignRightStyle });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Weight of sample from catch", Binding = new Binding("VesselUnload.WeightOfCatchSampleText"), CellStyle = AlignRightStyle });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Total weight of catch composition", Binding = new Binding("VesselUnload.SumOfCatchCompositionWeights"), CellStyle = AlignRightStyle });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Total weight of sampled catch", Binding = new Binding("VesselUnload.SumOfSampleWeights"), CellStyle = AlignRightStyle });
            //dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Raising factor", Binding = new Binding("VesselUnload.RaisingFactor"), CellStyle = AlignRightStyle  });
            //dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Number of species", Binding = new Binding("NumberOfSpeciesInCatchComposition"), CellStyle = AlignRightStyle  });

            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Type of sampling", Binding = new Binding("VesselUnload.SamplingTypeFlagText") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Validity of weights", Binding = new Binding("VesselUnload.WeightValidationFlagText") });
            dataGridWeights.Columns.Add(new DataGridTextColumn { Header = "Weight difference (%)", Binding = new Binding("VesselUnload.DifferenceCatchWtAndSumCatchCompWtText"), CellStyle = AlignRightStyle });




        }


        private Style AlignRightStyle
        {
            get
            {
                Style alignRightCellStype = new Style(typeof(DataGridCell));

                // Create a Setter object to set (get it? Setter) horizontal alignment.
                Setter setAlign = new
                    Setter(HorizontalAlignmentProperty,
                    HorizontalAlignment.Right);

                // Bind the Setter object above to the Style object
                alignRightCellStype.Setters.Add(setAlign);
                return alignRightCellStype;
            }
        }
        private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_changeFromGridClick)
            {
                _changeFromGridClick = false;
            }
            else
            {
                switch (((TabItem)((TabControl)sender).SelectedItem).Header)
                {
                    case "Unload entities summary":
                        ShowUnloadSummaryGrid();
                        break;
                    case "Vessel unload":
                        ShowVesselUnloadGrid();
                        break;
                    case "Number of boats and sum of catch":

                        //gridGearUnloadNumbers.Visibility = Visibility.Visible;
                        break;
                    case "Weights and weight validation":
                        ShowWeightsGrid();
                        break;
                }
            }
        }
        private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _changeFromGridClick = true;
            switch (((DataGrid)sender).Name)
            {
                case "dataGridUnloadSummary":
                    if (dataGridUnloadSummary.SelectedItem != null)
                    {
                        _selectedVesselUnload = (VesselUnload)dataGridUnloadSummary.SelectedItem;
                    }
                    break;
                case "GridVesselUnload":
                    _selectedVesselUnload = (VesselUnload)GridVesselUnload.SelectedItem;
                    break;
                case "dataGridWeights":
                    //Title = "no selected unload when grid selection changed";
                    if (dataGridWeights.SelectedItem != null)
                    {
                        _selectedVesselUnload = ((SummaryItem)dataGridWeights.SelectedItem).VesselUnload;
                        //if(_selectedVesselUnload!=null)
                        //{
                        //    Title = "has selected unload when grid selection changed";
                        //}

                        //_selectedVesselUnload = ((VesselUnload)dataGridWeights.SelectedItem).VesselUnload;
                    }
                    break;
            }

            if (_vesselUnloadWindow != null)
            {
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
            }

        }
        private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selectedVesselUnload != null)
            {
                if (_vesselUnloadWindow == null)
                {
                    if (NSAPEntities.NSAPRegion == null)
                    {
                        NSAPEntities.NSAPRegion = _selectedVesselUnload.Parent.Parent.NSAPRegion;
                    }
                    //_vesselUnloadWindow = new VesselUnloadWIndow(_selectedVesselUnload, this);
                    _vesselUnloadWindow = new VesselUnloadEditWindow(this);
                    _vesselUnloadWindow.UnloadEditor.UnloadChangesSaved += OnUnloadEditor_UnloadChangesSaved;
                    _vesselUnloadWindow.Owner = this;
                    _vesselUnloadWindow.Show();
                }
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
                //else
                //{
                //    _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
                //}
            }

        }

        private void OnUnloadEditor_UnloadChangesSaved(object sender, VesselUnloadEditorControl.UnloadEditorEventArgs e)
        {
            //GridVesselUnload.DataContext = _vesselUnloads;
            GridVesselUnload.Items.Refresh();
        }
        public string MaturityStage { get; set; }
        public string MaturityCode { get; set; }
        public int SpeciesID { get; set; }

        public string WatchedSpecies
        {
            get { return _watchedSpecies; }
            set
            {
                _watchedSpecies = value;
            }
        }
        public void TurnGridOff()
        {
            dataGridUnloadSummary.Visibility = Visibility.Hidden;
            GridVesselUnload.Visibility = Visibility.Hidden;
            gridGearUnloadNumbers.Visibility = Visibility.Hidden;

            //dataGridUnloadSummary.ItemsSource = null;
            //GridVesselUnload.ItemsSource = null;
            //dataGridUnloadSummary.Items.Clear();
            //GridVesselUnload.Items.Clear();
        }

        public List<VesselUnload> VesselUnloads
        {
            set
            {
                _vesselUnloads = value;
                NSAPEntities.NSAPRegion = _vesselUnloads[0].Parent.Parent.NSAPRegion;
            }
            get
            {
                return _vesselUnloads;
            }
        }
        public string SectorCode { get; set; }
        public List<GearUnload> GearUnloads
        {
            set
            {
                _summaryItems = null;
                _gearUnloads = value;
                //VesselUnloads = VesselUnloadViewModel.GetVesselUnloads(_gearUnloads);
                VesselUnloads = _gearUnloads[0].ListVesselUnload;
                if (VesselUnloads.Count > 0)
                {
                    GridVesselUnload.Visibility = Visibility.Visible;
                    dataGridUnloadSummary.Visibility = Visibility.Visible;
                    gridGearUnloadNumbers.Visibility = Visibility.Visible;
                    switch (((TabItem)this.TabControl.SelectedItem).Header)
                    {
                        case "Unload entities summary":
                            ShowUnloadSummaryGrid();
                            break;
                        case "Vessel unload":
                            ShowVesselUnloadGrid();
                            break;
                        case "Number of boats and sum of catch":
                            //gridGearUnloadNumbers.Visibility = Visibility.Visible;
                            break;
                        case "Weights and weight validation":
                            //_vlcws.Clear();
                            //_vlcws = null;
                            ShowWeightsGrid();
                            break;
                    }



                    object item = GridVesselUnload.Items[0];
                    GridVesselUnload.SelectedItem = item;
                    try
                    {
                        GridVesselUnload.ScrollIntoView(item);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //ignore
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            get
            {
                return _gearUnloads;
            }
        }
        public GearUnload GearUnload
        {
            set
            {

                _gearUnload = value;

                //if (_gearUnload.ListVesselUnload.Count > 0)
                if (NSAPEntities.VesselUnloadViewModel.GetAllVesselUnloads(_gearUnload, true).Count > 0)
                {
                    GridVesselUnload.Visibility = Visibility.Visible;
                    dataGridUnloadSummary.Visibility = Visibility.Visible;
                    gridGearUnloadNumbers.Visibility = Visibility.Visible;
                    switch (((TabItem)this.TabControl.SelectedItem).Header)
                    {
                        case "Unload entities summary":
                            ShowUnloadSummaryGrid();

                            break;
                        case "Vessel unload":
                            ShowVesselUnloadGrid();
                            break;
                        case "Number of boats and sum of catch":
                            //gridGearUnloadNumbers.Visibility = Visibility.Visible;
                            break;
                    }



                    object item = GridVesselUnload.Items[0];
                    GridVesselUnload.SelectedItem = item;
                    try
                    {
                        GridVesselUnload.ScrollIntoView(item);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //ignore
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            get
            {
                return _gearUnload;
            }
        }
        public void VesselWindowClosed()
        {
            _vesselUnloadWindow = null;
        }

        public CalendarViewType CalendarViewType { get; set; }
        private void SetVesselUnloadGridContext()
        {
            //if(_vesselUnloads.Count>0)
            //{

            //}
            if (_gearUnloads.Count > 0)
            {
                //_vesselUnloads = new List<VesselUnload>();
                bool isMeasurementCalendar = false;
                foreach (GearUnload gu in _gearUnloads)
                {

                    if (string.IsNullOrEmpty(MaturityCode))
                    {
                        if (CalendarViewType == CalendarViewType.calendarViewTypeLengthMeasurement ||
                                CalendarViewType == CalendarViewType.calendarViewTypeLengthWeightMeasurement ||
                                CalendarViewType == CalendarViewType.calendarViewTypeLengthFrequencyMeasurement ||
                                CalendarViewType == CalendarViewType.calendarViewTypeMaturityMeasurement)
                        {
                            isMeasurementCalendar = true;
                            //_vesselUnloads.AddRange(gu.ListVesselUnload);
                        }
                        else if (!string.IsNullOrEmpty(_watchedSpecies))
                        {
                            //_vesselUnloads.AddRange(gu.ListVesselUnload);
                        }
                        else
                        {
                            //foreach (VesselUnload vu in gu.ListVesselUnload)
                            //{
                            //    if (vu.SectorCode == SectorCode)
                            //    {
                            //        _vesselUnloads.Add(vu);

                            //    }
                            //}
                            GridVesselUnload.DataContext = _vesselUnloads;
                        }

                    }
                    else
                    {
                        _vesselUnloads.AddRange(gu.ListVesselUnload);
                    }


                }

                if (GridVesselUnload.DataContext == null)
                {
                    List<VesselUnload> vus = new List<VesselUnload>();
                    List<int> vu_ids = new List<int>();
                    if (!string.IsNullOrEmpty(MaturityCode))
                    {
                        vu_ids = CatchMaturityRepository.GetVesselUnloadIDsForFemaleCatchMaturityStage(_treeItemData,
                            _gearUnloads.First().Parent.SamplingDate.Day,
                            _gearUnloads.First().GearID,
                            SectorCode,
                            MaturityCode, SpeciesID);
                        foreach (var vu_id in vu_ids)
                        {
                            vus.Add(_vesselUnloads.Find(t => t.PK == vu_id));
                        }
                        GridVesselUnload.DataContext = vus;
                    }
                    else if (isMeasurementCalendar)
                    {
                        vu_ids = VesselCatchRepository.GetUnloadIDsWithCatchMeasurement(_treeItemData,
                            _gearUnloads.First().GearID,
                            SpeciesID,
                            _gearUnloads.First().Parent.SamplingDate.Day,
                            CalendarViewType);

                        foreach (var vu_id in vu_ids)
                        {
                            vus.Add(_vesselUnloads.Find(t => t.PK == vu_id));
                        }
                        GridVesselUnload.DataContext = vus;
                    }
                    else
                    {
                        vu_ids = VesselCatchRepository.GetUnloadIDsWithCatch(_treeItemData,
                            _gearUnloads.First().GearID,
                            SpeciesID,
                            _gearUnloads.First().Parent.SamplingDate.Day
                            );
                        foreach (var vu_id in vu_ids)
                        {
                            vus.Add(_vesselUnloads.Find(t => t.PK == vu_id));
                        }
                        GridVesselUnload.DataContext = vus;
                    }
                }

                LabelTitle.Content = $"Vessel unloads from {_gearUnloads[0].Parent.LandingSite} using {_gearUnloads[0].GearUsedName} on {_gearUnloads[0].Parent.SamplingDate.ToString("MMM-dd-yyyy")}";




                //((MainWindow)Owner).SetupCalendar();
            }
        }

        private void Gu_GetVesselUnloadEvent(object sender, ProcessingItemsEventArg e)
        {
            throw new NotImplementedException();
        }

        private void ShowVesselUnloadGrid()
        {

            GridVesselUnload.Columns.Clear();
            if (_listSource == GearUnloadWindowListSource.ListSourceWeights)
            {

            }
            else
            {
                switch (_listSource)
                {
                    case GearUnloadWindowListSource.ListSourceGearUnload:
                        SetVesselUnloadGridContext();
                        break;
                    case GearUnloadWindowListSource.listSourceVesselUnload:
                        LabelTitle.Content = "Vessel unloads from summary";
                        GridVesselUnload.DataContext = _vesselUnloads;
                        break;
                }


                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Identifier", Binding = new Binding("PK") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("DateTimeSampling") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearUsed") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Ref #", Binding = new Binding("RefNo") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Vessel", Binding = new Binding("VesselName") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Number of fishers", Binding = new Binding("NumberOfFishers") });
                GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Fishing trip success", Binding = new Binding("OperationIsSuccessful") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Weight of catch", Binding = new Binding("WeightOfCatchText") });
                GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Includes catch composition", Binding = new Binding("HasCatchComposition") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CatchCompositionCountText") });
                GridVesselUnload.Columns.Add(new DataGridCheckBoxColumn { Header = "Tracking", Binding = new Binding("OperationIsTracked") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS.AssignedName") });
                GridVesselUnload.Columns.Add(new DataGridTextColumn { Header = "Notes", Binding = new Binding("Notes") });
            }
        }

        private void ShowUnloadSummaryGrid()
        {
            labelUnloadSummary.Content = "Summary of vessel unloads of selected sampling day";
            dataGridUnloadSummary.Columns.Clear();
            dataGridUnloadSummary.AutoGenerateColumns = false;

            dataGridUnloadSummary.DataContext = _vesselUnloads;
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("PK") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Date sampled", Binding = new Binding("DateTimeSampling") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Enumerator", Binding = new Binding("EnumeratorName") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Sector", Binding = new Binding("Sector") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Fishing gear", Binding = new Binding("GearUsed") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Fishing grid count", Binding = new Binding("CountGrids") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Soak time count", Binding = new Binding("CountGearSoak") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Effort indicator count", Binding = new Binding("CountEffortIndicators") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Catch composition count", Binding = new Binding("CountCatchCompositionItems") });

            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length freq count", Binding = new Binding("CountLenFreqRows") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length count", Binding = new Binding("CountLengthRows") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Length weight count", Binding = new Binding("CountLenWtRows") });
            dataGridUnloadSummary.Columns.Add(new DataGridTextColumn { Header = "Maturity count", Binding = new Binding("CountMaturityRows") });

        }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //ShowVesselUnloadGrid();
            //ShowUnloadSummaryGrid();
            if (_summaryItems != null)
            {
                tabItemWeights.IsSelected = true;
            }
            dataGridWeights.PreviewKeyDown += OnDataGridWeights_PreviewKeyDown;
        }
        private void ShowSelectedInVesselUnloadWindow()
        {
            //Title = "no selected unload in ShowSelectedInVesselUnloadWindow";
            if (_selectedVesselUnload != null)
            {
                //Title = "has selected unload in ShowSelectedInVesselUnloadWindow";
                //if (NSAPEntities.NSAPRegion == null)
                //{
                //    NSAPEntities.NSAPRegion = _selectedVesselUnload.Parent.Parent.NSAPRegion;
                //}
                //if (_vesselUnloadWindow == null)
                //{
                //    _vesselUnloadWindow = new VesselUnloadEditWindow(this);
                //    _vesselUnloadWindow.Owner = this;
                //    _vesselUnloadWindow.Show();
                //}
                //_vesselUnloadWindow.VesselUnload = _selectedVesselUnload;
                _vesselUnloadWindow = VesselUnloadEditWindow.GetInstance(this);
                if (_vesselUnloadWindow.Visibility == Visibility.Visible)
                {
                    _vesselUnloadWindow.BringIntoView();
                }
                else
                {
                    _vesselUnloadWindow.Show();
                }
                _vesselUnloadWindow.Owner = this;
                _vesselUnloadWindow.VesselUnload = _selectedVesselUnload;

            }
        }
        private void OnDataGridWeights_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var u = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && u != null)
            {
                e.Handled = true;
                //Title = "no selected unload when enter key is pressed";
                //if(_selectedVesselUnload!=null)
                //{
                //    Title = "has selected unload when enter key is pressed";
                //}
                ShowSelectedInVesselUnloadWindow();
                //u.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (_vesselUnloadWindow?.UnloadEditor != null)
            //{
            //    _vesselUnloadWindow.UnloadEditor.UnloadChangesSaved -= OnUnloadEditor_UnloadChangesSaved;
            //}

            this.SavePlacement();
            _instance = null;
            if (_parentWindow != null)
            {
                _parentWindow.GearUnloadWindowClosed();
            }
        }

        //This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void OnButtonCLick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "ButtonSaveBoatsCatch":
                    if (textBoxCatch.Text.Length > 0 && textBoxBoats.Text.Length > 0
                        && double.TryParse(textBoxCatch.Text, out double c)
                        && int.TryParse(textBoxBoats.Text, out int b))
                    {

                        _gearUnload.Boats = b;
                        _gearUnload.Catch = c;

                        GearUnload gu = new GearUnload
                        {
                            Boats = b,
                            Catch = c,
                            PK = _gearUnload.PK,
                            GearID = _gearUnload.GearID,
                            LandingSiteSamplingID = _gearUnload.Parent.PK
                        };
                        NSAPEntities.GearUnloadViewModel.UpdateRecordInRepo(gu);
                    }
                    else
                    {
                        if (textBoxCatch.Text.Length > 0 || textBoxBoats.Text.Length > 0)
                        {
                            MessageBox.Show("Number of boats and total catch are both required", Global.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    break;
                case "ButtonClose":
                    Close();
                    break;
            }

        }
        public void ResetFilter()
        {
            dataGridWeights.DataContext = _summaryItems;
        }
        public void FilterWeightGrid(List<SummaryItem> filterdItems)
        {
            dataGridWeights.DataContext = filterdItems;
        }
        private void OnWindowClosed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).Focus();
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void onMenuClicked(object sender, RoutedEventArgs e)
        {
            string selectedMenu = (((MenuItem)sender).Name);
            switch (selectedMenu)
            {
                case "menuTallyValidity":
                    WeightValidationTallyWindow wvtw = WeightValidationTallyWindow.GetInstance(_summaryItems);
                    wvtw.Owner = this;
                    wvtw.DataGrid = dataGridWeights;
                    if (wvtw.Visibility == Visibility.Visible)
                    {
                        wvtw.BringIntoView();
                    }
                    else
                    {
                        wvtw.Show();
                    }
                    break;

                case "menuExit":
                    Close();
                    break;

            }
        }
    }
}
