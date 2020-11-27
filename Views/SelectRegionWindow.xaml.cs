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
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for SelectRegionWindow.xaml
    /// </summary>
    public partial class SelectRegionWindow : Window
    {
        public SelectRegionWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (NSAPEntities.Regions != null && NSAPEntities.Regions.Count > 0)
            {
                foreach (var item in NSAPEntities.Regions)
                {
                    foreach (CheckBox chk in PanelSelectRegion.Children)
                    {
                        if (chk.Name == "r" + item)
                        {
                            chk.IsChecked = true;
                            break;
                        }
                    }
                }
            }
        }

        private void OnButtoClicked(object sender, RoutedEventArgs e)
        {

            switch (((Button)sender).Name)
            {
                case "ButtonSelectAll":
                    foreach (CheckBox chk in PanelSelectRegion.Children)
                    {
                        chk.IsChecked = true;
                    }
                    break;
                case "ButtonSelectNone":
                    foreach (CheckBox chk in PanelSelectRegion.Children)
                    {
                        chk.IsChecked = false;
                    }
                    break;
                case "ButtonOk":
                    List<string> regions = new List<string>();
                    foreach (CheckBox chk in PanelSelectRegion.Children)
                    {
                        if (chk.IsChecked == true)
                        {
                            regions.Add(chk.Name.Trim(new char[] { 'r' }));
                        }
                    }
                    NSAPEntities.Regions = regions;
                    DialogResult = true;
                    Close();
                    break;
                case "ButtonCancel":
                    DialogResult = false;
                    Close();
                    break;
            }
        }

        private void ClosingTrigger(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

    }
}
