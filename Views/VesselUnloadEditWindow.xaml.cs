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
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
using NSAP_ODK.Utilities;
using System.Diagnostics;
using NSAP_ODK.VesselUnloadEditorControl;
using System.Data.OleDb;
using NSAP_ODK.Views;
using NPOI.Util;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for VesselUnloadEditWindow.xaml
    /// </summary>
    public partial class VesselUnloadEditWindow : Window
    {
        private static VesselUnloadEditWindow _instance;
        private VesselUnload _vesselUnload;
        private bool _editMode;
        private string _canAddMessage;
        public VesselUnloadEditWindow(Window parent, VesselUnload vu=null)
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            unloadEditor.ButtonClicked += OnUnloadEditorButtonClicked;
            unloadEditor.GridDoubleClicked += OnUnloadEditorGridDoubleClicked;
            unloadEditor.DeleteProceed += OnUnloadEditor_DeleteProceed;
            unloadEditor.UndoEditVesselUnload += UnloadEditor_UndoEditVesselUnload;
            Loaded += OnWindowLoaded;

            if (NSAPEntities.Grid25InlandLocationViewModel == null)
            {
                NSAPEntities.Grid25InlandLocationViewModel = new Grid25InlandLocationViewModel();
                NSAPEntities.Grid25InlandLocationViewModel.GetAllGrid25InlandCells();
            }

            switch (parent.GetType().Name)
            {
                case "MainWindow":
                    MainWindowParent = (MainWindow)parent;
                    break;
                case "GearUnloadWindow":
                    GearUnloadWindowParent = (GearUnloadWindow)parent;
                    break;
            }
            _vesselUnload = vu;
        }

        private void UnloadEditor_UndoEditVesselUnload(object sender, EventArgs e)
        {
            treeItemVesselUnload.IsSelected = true;
        }

        private void OnUnloadEditor_DeleteProceed(object sender, UnloadEditorEventArgs e)
        {
            bool refreshEditor = false;
            switch (e.UnloadView)
            {
                case "treeItemVesselUnload":
                    SummaryItemViewModel.DeletedVesselUnloadID = null;
                    bool success = false;
                    var parent = VesselUnload.Parent;
                    int id = VesselUnload.PK;
                    if (VesselUnload.GearSoakViewModel == null)
                    {
                        VesselUnload.GearSoakViewModel = new GearSoakViewModel(VesselUnload);
                    }
                    if (VesselUnload.FishingGroundGridViewModel == null)
                    {
                        VesselUnload.FishingGroundGridViewModel = new FishingGroundGridViewModel(VesselUnload);
                    }
                    if (VesselUnload.GearSoakViewModel.DeleteAllInCollection() && VesselUnload.FishingGroundGridViewModel.DeleteAllInCollection())
                    {
                        if (VesselUnload.IsMultiGear)
                        {
                            if (VesselUnload.VesselUnload_FishingGearsViewModel == null)
                            {
                                VesselUnload.VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(VesselUnload);
                            }
                            success = VesselUnload.VesselUnload_FishingGearsViewModel.DeleteAllInCollection();
                        }
                        else
                        {
                            success = (VesselUnload.VesselEffortViewModel.DeleteAllInCollection() &&
                            VesselUnload.VesselCatchViewModel.DeleteAllInCollection());
                        }
                    }
                    if (success)
                    {
                        if (parent.VesselUnloadViewModel.DeleteRecordFromRepo(id))
                        {
                            SummaryItemViewModel.DeletedVesselUnloadID = id;
                            if (NSAPEntities.SummaryItemViewModel.DeleteUsingVesselUnloadID(id))
                            {
                                //var f = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.VesselUnloadID == id);
                                if (parent.VesselUnloadViewModel.Count == 0)
                                {
                                    var samplingDay = parent.Parent;
                                    if (samplingDay.GearUnloadViewModel.DeleteRecordFromRepo(parent.PK) && samplingDay.GearUnloadViewModel.Count == 0)
                                    {
                                        NSAPEntities.LandingSiteSamplingViewModel.DeleteRecordFromRepo(samplingDay);
                                    }
                                }

                                if (MainWindowParent != null)
                                {
                                    MainWindowParent.RefreshAfterDeleteVesselUnload();
                                }
                                else
                                {
                                    NSAPEntities.SummaryItemViewModel.RefreshMonthCalendarSource();
                                    GearUnloadWindowParent.RefreshAfterDeleteVesselUnload();
                                }

                                Close();
                            }
                        }
                    }
                    break;
                case "treeItemFishingGears":
                    if (VesselUnload.IsMultiGear)
                    {
                        if (unloadEditor.VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.VesselCatchViewModel.Count == 0)
                        {
                            refreshEditor = unloadEditor.VesselUnload.VesselUnload_FishingGearsViewModel.DeleteRecordFromRepo(unloadEditor.VesselUnload_FishingGear_Edited.RowID);
                        }
                        else
                        {
                            foreach (VesselCatch c in unloadEditor.VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.VesselCatchViewModel.VesselCatchCollection.ToList())
                            {
                                foreach (CatchLenFreq clf in c.CatchLenFreqViewModel.CatchLenFreqCollection.ToList())
                                {
                                    c.CatchLenFreqViewModel.DeleteRecordFromRepo(clf.PK);
                                }
                                foreach (CatchLength cl in c.CatchLengthViewModel.CatchLengthCollection.ToList())
                                {
                                    c.CatchLenFreqViewModel.DeleteRecordFromRepo(cl.PK);
                                }
                                foreach (CatchLengthWeight clw in c.CatchLengthWeightViewModel.CatchLengthWeightCollection.ToList())
                                {
                                    c.CatchLengthWeightViewModel.DeleteRecordFromRepo(clw.PK);
                                }
                                foreach (CatchMaturity cm in c.CatchMaturityViewModel.CatchMaturityCollection.ToList())
                                {
                                    c.CatchMaturityViewModel.DeleteRecordFromRepo(cm.PK);
                                }
                                unloadEditor.VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.VesselCatchViewModel.DeleteRecordFromRepo(c.PK);
                            }

                            refreshEditor = unloadEditor.VesselUnload.VesselUnload_FishingGearsViewModel.DeleteRecordFromRepo(unloadEditor.VesselUnload_FishingGear_Edited.RowID);
                        }
                    }
                    break;
                case "treeItemSoakTime":
                    refreshEditor = unloadEditor.VesselUnload.GearSoakViewModel.DeleteRecordFromRepo(unloadEditor.GearSoakEdited.PK);
                    break;
                case "treeItemFishingGround":
                    refreshEditor = unloadEditor.VesselUnload.FishingGroundGridViewModel.DeleteRecordFromRepo(unloadEditor.FishingGroundGridEdited.PK);
                    break;
                case "treeItemEffortDefinition":
                    if (VesselUnload.IsMultiGear)
                    {
                        refreshEditor = unloadEditor.VesselUnload_Gear_Spec_Edited.VesselUnload_Gear_Spec.Parent.VesselUnload_Gear_Specs_ViewModel.DeleteRecordFromRepo(unloadEditor.VesselUnload_Gear_Spec_Edited.RowID);
                    }
                    else
                    {
                        if (unloadEditor.VesselUnload.VesselEffortViewModel.DeleteRecordFromRepo(unloadEditor.VesselUnload_Gear_Spec_Edited.RowID))
                        {
                            VesselEffort ve = new VesselEffort
                            {
                                PK = unloadEditor.VesselUnload_Gear_Spec_Edited.RowID,
                                Parent = unloadEditor.VesselUnload
                            };
                            refreshEditor = VesselUnloadViewModel.DeleteGearSpecForSingleGearUnload(ve);
                        }
                    }
                    break;

                case "treeItemCatchComposition":
                    var vc = unloadEditor.VesselCatchEdited.VesselCatch;
                    foreach (CatchLenFreq clf in vc.CatchLenFreqViewModel.CatchLenFreqCollection.ToList())
                    {
                        vc.CatchLenFreqViewModel.DeleteRecordFromRepo(clf.PK);
                    }
                    foreach (CatchLength cl in vc.CatchLengthViewModel.CatchLengthCollection.ToList())
                    {
                        vc.CatchLengthViewModel.DeleteRecordFromRepo(cl.PK);
                    }
                    foreach (CatchLengthWeight clw in vc.CatchLengthWeightViewModel.CatchLengthWeightCollection.ToList())
                    {
                        vc.CatchLengthWeightViewModel.DeleteRecordFromRepo(clw.PK);
                    }
                    foreach (CatchMaturity cm in vc.CatchMaturityViewModel.CatchMaturityCollection.ToList())
                    {
                        vc.CatchMaturityViewModel.DeleteRecordFromRepo(cm.PK);
                    }
                    if (unloadEditor.VesselUnload.IsMultiGear)
                    {
                        refreshEditor = vc.ParentFishingGear.VesselCatchViewModel.DeleteRecordFromRepo(vc.PK);
                    }
                    else 
                    {
                        refreshEditor = vc.Parent.VesselCatchViewModel.DeleteRecordFromRepo(vc.PK);
                    }
                    break;
                case "treeItemLenFreq":
                    refreshEditor = unloadEditor.VesselCatchEdited.VesselCatch.CatchLenFreqViewModel.DeleteRecordFromRepo(unloadEditor.CatchLenFreqEdited.PK);
                    break;
                case "treeItemLenWeight":
                    refreshEditor = unloadEditor.VesselCatchEdited.VesselCatch.CatchLengthWeightViewModel.DeleteRecordFromRepo(unloadEditor.CatchLengthWeightEdited.PK);
                    break;
                case "treeItemLenList":
                    refreshEditor = unloadEditor.VesselCatchEdited.VesselCatch.CatchLengthViewModel.DeleteRecordFromRepo(unloadEditor.CatchLengthEdited.PK);
                    break;
                case "treeItemMaturity":
                    refreshEditor = unloadEditor.VesselCatchEdited.VesselCatch.CatchMaturityViewModel.DeleteRecordFromRepo(unloadEditor.CatchMaturityEdited.PK);
                    break;

            }
            if (refreshEditor)
            {
                unloadEditor.RefreshView();
            }
        }

        public void EditGearSoakTime()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.GearSoakEdited = unloadEditor.GearSoakEdited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditGridFishingGround()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.FishingGroundGridEdited = unloadEditor.FishingGroundGridEdited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditVesselUnloadGearSpec()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.VesselUnload_Gear_Spec_Edited = unloadEditor.VesselUnload_Gear_Spec_Edited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditCatchLength()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.CatchLengthEdited = unloadEditor.CatchLengthEdited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditCatchLenFreq()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.CatchLenFreqEdited = unloadEditor.CatchLenFreqEdited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditCatchLenWeight()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.CatchLengthWeightEdited = unloadEditor.CatchLengthWeightEdited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditCatchMaturity()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.CatchMaturityEdited = unloadEditor.CatchMaturityEdited;
            evui.VesselUnloadEditWindow = this;
            evui.Owner = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void OnUnloadEditorGridDoubleClicked(object sender, UnloadEditorEventArgs e)
        {
            switch (e.UnloadView)
            {
                case "treeItemCatchComposition":
                    EditCatch();
                    break;
                case "treeItemFishingGears":
                    EditVesselUnloadFishingGear();
                    break;
                case "treeItemEffortDefinition":
                    EditVesselUnloadGearSpec();
                    break;
                case "treeItemFishingGround":
                    EditGridFishingGround();
                    break;
                case "treeItemSoakTime":
                    EditGearSoakTime();
                    break;
                case "treeItemLenList":
                    EditCatchLength();
                    break;
                case "treeItemLenFreq":
                    EditCatchLenFreq();
                    break;
                case "treeItemLenWeight":
                    EditCatchLenWeight();
                    break;
                case "treeItemMaturity":
                    EditCatchMaturity();
                    break;
            }
        }

        public VesselUnloadEditor UnloadEditor { get { return unloadEditor; } }

        public GearUnloadWindow GearUnloadWindowParent { get; set; }
        public MainWindow MainWindowParent { get; set; }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //buttonEdit.Visibility = Visibility.Collapsed;
            //if (Debugger.IsAttached)
            //{
            buttonEdit.Visibility = Visibility.Visible;
            if(_vesselUnload!=null)
            {
                VesselUnload = _vesselUnload;
            }
            //}
        }

        private void EditVesselUnloadFishingGear()
        {
            EditVesselUnloadItemsWindow evui = EditVesselUnloadItemsWindow.GetInstance();
            evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
            evui.VesselUnload_FishingGear_Edited = unloadEditor.VesselUnload_FishingGear_Edited;
            evui.VesselUnloadEditWindow = this;
            if (evui.Visibility == Visibility.Visible)
            {
                evui.BringIntoView();
            }
            else
            {
                evui.Show();
            }
            evui.Owner = this;
            evui.UnloadView = unloadEditor.UnloadView;
        }
        private void EditCatch()
        {
            EditCatchWindow ecw = EditCatchWindow.GetInstance(isNew: false, vuEdit: unloadEditor.VesselUnloadEdit);
            ecw.VesselCatchEdited = unloadEditor.VesselCatchEdited;
            ecw.Owner = this;
            if (ecw.Visibility == Visibility.Visible)
            {
                ecw.BringIntoView();
            }
            else
            {
                ecw.Show();
            }
        }
        public VesselUnload_FishingGear_Edited VesselUnload_FishingGear_Edited { get; set; }
        private void OnUnloadEditorButtonClicked(object sender, VesselUnloadEditorControl.UnloadEditorEventArgs e)
        {
            EditCatchWindow ecw = null;
            EditVesselUnloadItemsWindow evui = null;
            string deleteMessage = "";
            bool confirmDeleteContinue = false;
            switch (e.ButtonPressed)
            {
                case "buttonDelete":
                    switch (e.UnloadView)
                    {
                        case  "treeItemVesselUnload":
                            deleteMessage = "Deleting vessel unload will also delete data connected to the sampled landing such as catch composition, fishing gears, and fishing grounds\r\n\r\nDo you want to proceed?";
                            confirmDeleteContinue = true;
                            break;
                        case "treeItemCatchComposition":
                            deleteMessage = "Deleting selected catch will also delete any maturity/length measurements of the catch\r\n\r\nDo you want to proceed?";
                            confirmDeleteContinue = true;
                            break;
                        case "treeItemFishingGears":
                            if (!VesselUnload.IsMultiGear)
                            {
                                deleteMessage = "Cannot delete gear from single gear Catch and Effort eForm";
                            }
                            else if (VesselUnload.IsMultiGear && VesselUnload.Parent.GearUsedName == unloadEditor.VesselUnload_FishingGear_Edited.GearUsedName)
                            {
                                deleteMessage = "Cannot delete main gear of multiple gear Catch and Effort eForm";
                            }
                            else if (unloadEditor.VesselUnload_FishingGear_Edited.VesselUnload_FishingGear.VesselCatchViewModel.Count > 0)
                            {
                                deleteMessage = "Deleting selected gear will also delete catch composition of the gear and any maturity/length measurements of the catch\r\n\r\nDo you want to proceed?";
                                confirmDeleteContinue = true;
                            }
                            break;
                    }
                    if(deleteMessage.Length==0)
                    {
                        deleteMessage = "Are you sure you want to delete?";
                        confirmDeleteContinue = true;
                    }
                    if (confirmDeleteContinue)
                    {
                        if (MessageBox.Show(deleteMessage,
                            Global.MessageBoxCaption,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            e.Proceed = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(deleteMessage,
                                Global.MessageBoxCaption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                    }
                    break;
                case "buttonEdit":
                    switch (e.UnloadView)
                    {
                        case "treeItemFishingGround":
                            EditGridFishingGround();
                            break;
                        case "treeItemCatchComposition":

                            EditCatch();
                            break;
                        case "treeItemFishingGears":
                            EditVesselUnloadFishingGear();
                            break;
                        case "treeItemEffortDefinition":
                            EditVesselUnloadGearSpec();
                            break;
                        case "treeItemSoakTime":
                            EditGearSoakTime();
                            break;
                    }
                    break;
                case "buttonAdd":
                    try
                    {
                        unloadEditor.EditorMessage = "";
                        switch (e.UnloadView)
                        {
                            case "treeItemLenList":
                            case "treeItemLenFreq":
                            case "treeItemLenWeight":
                            case "treeItemMaturity":
                            case "treeItemEffortDefinition":
                            case "treeItemSoakTime":
                            case "treeItemFishingGround":
                                evui = EditVesselUnloadItemsWindow.GetInstance(isNew: true);
                                break;
                            case "treeItemCatchComposition":
                                if (UnloadEditor.CanAddToCatchComposition())
                                {
                                    ecw = EditCatchWindow.GetInstance(isNew: true, vuEdit: unloadEditor.VesselUnloadEdit);
                                    //ecw.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
                                }
                                break;

                            case "treeItemFishingGears":
                                if (unloadEditor.CanAddGear())
                                {
                                    evui = EditVesselUnloadItemsWindow.GetInstance(isNew: true);
                                }
                                break;
                        }

                        if (!string.IsNullOrEmpty(unloadEditor.EditorMessage))
                        {
                            MessageBox.Show(UnloadEditor.EditorMessage,
                                    Global.MessageBoxCaption,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    break;
            }

            if (ecw != null)
            {
                ecw.Owner = this;
                if (ecw.Visibility == Visibility.Visible)
                {
                    ecw.BringIntoView();
                }
                else
                {
                    ecw.Show();
                }
            }
            else if (evui != null)
            {
                if (evui.Visibility == Visibility.Visible)
                {
                    evui.BringIntoView();
                }
                else
                {
                    evui.Show();
                }
                evui.Owner = this;
                evui.VesselUnloadEditWindow = this;
                evui.VesselUnloadEdit = unloadEditor.VesselUnloadEdit;
                evui.UnloadView = e.UnloadView;
            }
        }
        public void VesselUnloadEditItemsSaved()
        {
            unloadEditor.RefreshView();
        }
        private bool CanAddToCatchComposition()
        {
            _canAddMessage = "";
            bool canAdd = false;
            if (!VesselUnload.IsMultiGear)
            {
                canAdd = unloadEditor.VesselUnloadEdit.VesselCatches.Count == 0 && unloadEditor.VesselUnloadEdit.HasCatchComposition == true;
            }
            else if (VesselUnload.IsMultiGear)
            {
                double gear_catch_wt = 0;
                foreach (var gear in VesselUnload.ListUnloadFishingGears)
                {
                    gear_catch_wt += (double)gear.WeightOfCatch;
                }
            }
            if (!canAdd)
            {
                if (unloadEditor.VesselUnloadEdit.HasCatchComposition)
                {
                    double wtOfCatch = 0;
                    foreach (var item in unloadEditor.VesselUnloadEdit.VesselCatches)
                    {
                        wtOfCatch += (double)item.Catch_kg;
                    }

                    canAdd = wtOfCatch < unloadEditor.VesselUnloadEdit.WeightOfCatch;
                    if (!canAdd)
                    {
                        _canAddMessage = "Weight of total catch is not greater than weight of catch composition.\n\nNew items cannot be added.";
                    }
                }
                else
                {
                    _canAddMessage = "Sampled landing does not include catch composition";

                }
            }
            return canAdd;
        }
        public VesselUnload VesselUnload
        {
            get { return _vesselUnload; }
            set
            {
                _vesselUnload = value;
                if (_vesselUnload != null)
                {
                    NSAPEntities.NSAPRegion = _vesselUnload.Parent.Parent.NSAPRegion;
                    NSAPEntities.NSAPRegionFMA = NSAPEntities.NSAPRegion.FMAs.Where(t => t.FMAID == _vesselUnload.Parent.Parent.FMA.FMAID).FirstOrDefault();

                    NSAPEntities.NSAPRegionFMAFishingGround = NSAPEntities.NSAPRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == _vesselUnload.Parent.Parent.FishingGround.Code).FirstOrDefault();
                    unloadEditor.Owner = this;
                    unloadEditor.VesselUnload = _vesselUnload;


                    if (unloadEditor.UnloadView != "treeItemVesselUnload")
                    {
                        treeItemVesselUnload.IsSelected = true;
                    }
                    else
                    {
                        unloadEditor.UnloadView = "treeItemVesselUnload";
                    }
                }
            }
        }

        public static VesselUnloadEditWindow Instance()
        {
            return _instance;
        }
        public static VesselUnloadEditWindow GetInstance(Window parent)
        {
            if (_instance == null)
            {
                _instance = new VesselUnloadEditWindow(parent);
            }
            return _instance;
        }
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GearUnloadWindowParent != null)
            {
                GearUnloadWindowParent.VesselWindowClosed();
                GearUnloadWindowParent.Focus();
            }
            else if (MainWindowParent != null)
            {
                MainWindowParent.VesselWindowClosed();
                MainWindowParent.Focus();
            }

            GearUnloadWindowParent = null;
            MainWindowParent = null;
            _instance = null;
            this.SavePlacement();
        }

        private void OnTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            unloadEditor.UnloadView = ((TreeViewItem)e.NewValue).Name.ToString();
        }
        private void ResetEditButton()
        {
            buttonEdit.Background = Brushes.SkyBlue;
            buttonEdit.Content = "Edit";
        }

        private void SetUnloadView()
        {
            unloadEditor.EditMode = _editMode;
            treeItemVesselUnload.IsSelected = true;
            unloadEditor.UnloadView = "treeItemVesselUnload";
        }
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonEdit":

                    _editMode = !_editMode;

                    if (_editMode)
                    {
                        buttonEdit.Background = Brushes.Yellow;
                        buttonEdit.Content = "Stop edits";
                        buttonSave.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ResetEditButton();
                        buttonSave.Visibility = Visibility.Collapsed;
                    }

                    //unloadEditor.EditMode = _editMode;
                    SetUnloadView();
                    break;
                case "buttonSave":
                    if (UnloadEditor.VesselUnloadHasChangedProperties && UnloadEditor.SaveChangesToVesselUnload())
                    {
                        ResetEditButton();
                        _editMode = false;
                        buttonSave.Visibility = Visibility.Collapsed;
                        SetUnloadView();
                    }
                    break;
                case "buttonOk":
                    if (UnloadEditor.VesselUnloadHasChangedProperties && UnloadEditor.SaveChangesToVesselUnload())
                    {
                        Close();
                        FocusParentWindow();
                    }
                    else if (!UnloadEditor.VesselUnloadHasChangedProperties)
                    {
                        Close();
                        FocusParentWindow();
                    }
                    //Close();
                    break;
                case "buttonCancel":
                    Close();
                    FocusParentWindow();
                    break;
            }
        }

        private void FocusParentWindow()
        {
            if (MainWindowParent != null)
            {
                MainWindowParent.Focus();
            }
            else if (GearUnloadWindowParent != null)
            {
                GearUnloadWindowParent.Focus();
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
    }
}
