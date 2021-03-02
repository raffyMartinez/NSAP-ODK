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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EditVesselUnloadWindow.xaml
    /// </summary>
    public partial class EditVesselCatchCompWindow : Window
    {
        private static EditVesselCatchCompWindow _instance;
        private VesselCatch _vesselCatch;

        public static EditVesselCatchCompWindow GetInstance()
        {
            if(_instance==null)
            {
                _instance = new EditVesselCatchCompWindow();
            }
            return _instance;
        }

        public static EditVesselCatchCompWindow Instance
        {
            get { return _instance; }
        }
        public EditVesselCatchCompWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
            _instance = null;
        }

       public VesselCatch VesselCatch
        {
            get { return _vesselCatch; }
            set
            {
                _vesselCatch = value;
                SetUpPropertyGrid();
            }
        }

        private void SetUpPropertyGrid()
        {
            propertyGrid.SelectedObject = _vesselCatch;
            propertyGrid.AutoGenerateProperties = true;

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonOk":
                    break;
                case "buttonCancel":
                    Close();
                    break;
            }
        }

        private void OnPropertyValueChanged(object sender, Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs e)
        {

        }
    }
}
