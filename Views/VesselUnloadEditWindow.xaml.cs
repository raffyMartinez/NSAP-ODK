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
        public VesselUnloadEditWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            unloadEditor.ButtonClicked += OnUnloadEditorButtonClicked;
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            buttonEdit.Visibility = Visibility.Collapsed;
            if(Debugger.IsAttached)
            {
                buttonEdit.Visibility = Visibility.Visible;
            }
        }

        private void OnUnloadEditorButtonClicked(object sender, VesselUnloadEditorControl.UnloadEditorEventArgs e)
        {
            switch(e.ButtonPressed)
            {
                case "buttonDelete":
                    if(MessageBox.Show("Are you sure you want to delete?",
                        "NSAP-ODK Database",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question)==MessageBoxResult.Yes    )
                    {
                        e.Proceed = true;
                    }
                    break;
                case "buttonEdit":
                    if(treeItemCatchComposition.IsSelected)
                    {

                    }
                    break;
                case "buttonAdd":
                    if(treeItemCatchComposition.IsSelected)
                    {
                        EditVesselCatchCompWindow evccw = EditVesselCatchCompWindow.GetInstance();
                        
                        if(evccw.Visibility==Visibility.Visible)
                        {
                            evccw.BringIntoView();
                        }
                        else
                        {
                            evccw.Show();
                        }
                        evccw.VesselCatch = unloadEditor.VesselCatch;
                    }
                    break;
            }
        }

        public VesselUnload VesselUnload
        {
            get { return _vesselUnload; }
            set
            {                
                _vesselUnload = value;
                NSAPEntities.NSAPRegion = _vesselUnload.Parent.Parent.NSAPRegion;
                NSAPEntities.NSAPRegionFMA = NSAPEntities.NSAPRegion.FMAs.Where(t => t.FMAID == _vesselUnload.Parent.Parent.FMAID).FirstOrDefault();
                NSAPEntities.NSAPRegionFMAFishingGround = NSAPEntities.NSAPRegionFMA.FishingGrounds.Where(t => t.FishingGroundCode == _vesselUnload.Parent.Parent.FishingGroundID).FirstOrDefault();
                unloadEditor.Owner = this;
                unloadEditor.VesselUnload = _vesselUnload;
                treeItemVesselUnload.IsSelected = true;
            }
        }

        public static VesselUnloadEditWindow  Instance()
        {
            return _instance;
        }
        public static VesselUnloadEditWindow GetInstance()
        {
            if (_instance == null)
            {
                _instance = new VesselUnloadEditWindow();
            }
            return _instance;
        }
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance = null; 
            this.SavePlacement();
        }

        private void OnTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            unloadEditor.UnloadView = ((TreeViewItem)e.NewValue).Name.ToString();
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
                        treeItemVesselUnload.IsSelected = true;

                    }
                    else
                    {
                        buttonEdit.Background = Brushes.SkyBlue;
                        buttonEdit.Content = "Edit";
                    }
                    unloadEditor.EditMode = _editMode;
                    break;
                case "buttonOk":
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
    }
}
