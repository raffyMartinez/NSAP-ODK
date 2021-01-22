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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NSAP_ODK.Entities;
using System.ComponentModel;
namespace NSAP_ODK.TreeViewModelControl
{
    /// <summary>
    /// Interaction logic for TreeControl.xaml
    /// </summary>
    public partial class TreeControl : UserControl
    {
        public event EventHandler<AllSamplingEntitiesEventHandler> TreeViewItemSelected;
        public event EventHandler<AllSamplingEntitiesEventHandler> TreeContextMenu;
        public TreeViewItemViewModel _selectedItem;
        private tv_NSAPViewModel _nsapViewModel;
        public TreeControl()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                InitializeComponent();
            }
        }


        public void ReadDatabase()
        {
            if (NSAPEntities.NSAPRegionViewModel != null 
                && NSAPEntities.NSAPRegionViewModel.Count > 0
                && NSAPEntities.FMAViewModel.Count>0 
                && NSAPEntities.FishSpeciesViewModel.Count>0 )
            {
                List<NSAPRegion> listRegions = NSAPEntities.NSAPRegionViewModel.GetAllNSAPRegions().ToList();
                _nsapViewModel = new tv_NSAPViewModel(listRegions, this);
                base.DataContext = _nsapViewModel;
                treeView.Visibility = Visibility.Visible;
            }
            else
            {
                treeView.Visibility = Visibility.Collapsed;
            }
        }
        private void Tree_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime && NSAPEntities.NSAPRegionViewModel!=null)
            {
                ReadDatabase();
            }

        }
        private void TreeSelectedItem_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tv = (TreeView)sender;
            var tvi = tv.SelectedItem;

            if (tvi == null)
                return;

            _selectedItem = (TreeViewItemViewModel)tvi;
            AllSamplingEntitiesEventHandler args = new AllSamplingEntitiesEventHandler();
            string treeViewEntity = tvi.GetType().Name;
            args.TreeViewEntity = treeViewEntity;
            switch (treeViewEntity)
            {
                case "tv_NSAPRegionViewModel":
                    args.NSAPRegion = ((tv_NSAPRegionViewModel)tvi)._region;
                    //tv_CurrentEntities.CurrentRegion = args.NSAPRegion;
                    break;
                case "tv_FMAViewModel":
                    args.FMA = ((tv_FMAViewModel)tvi)._fma;
                    args.NSAPRegion = ((tv_FMAViewModel)tvi)._region;
                    //tv_CurrentEntities.CurrentFMA = args.FMA;
                    break;
                case "tv_FishingGroundViewModel":
                    args.FishingGround = ((tv_FishingGroundViewModel)tvi)._fishingGround;
                    args.FMA = ((tv_FishingGroundViewModel)tvi)._fma;
                    args.NSAPRegion = ((tv_FishingGroundViewModel)tvi)._region;
                    //tv_CurrentEntities.CurrentFishingGround = args.FishingGround;
                    break;
                case "tv_LandingSiteViewModel":
                    args.LandingSite = ((tv_LandingSiteViewModel)tvi)._landingSite;
                    args.FishingGround = ((tv_LandingSiteViewModel)tvi)._fishingGround;
                    args.FMA = ((tv_LandingSiteViewModel)tvi)._fma;
                    args.NSAPRegion = ((tv_LandingSiteViewModel)tvi)._region;
                    args.LandingSiteText= ((tv_LandingSiteViewModel)tvi)._landingSiteText;
                    //tv_CurrentEntities.CurrentLandingSite = args.LandingSite;
                    break;

                case "tv_MonthViewModel":
                    args.MonthSampled = DateTime.Parse(((tv_MonthViewModel)tvi)._month);
                    args.LandingSite = ((tv_MonthViewModel)tvi)._landingSite;
                    args.LandingSiteText= ((tv_MonthViewModel)tvi)._landingSiteName;
                    args.FishingGround = ((tv_MonthViewModel)tvi)._fishingGround;
                    args.FMA = ((tv_MonthViewModel)tvi)._fma;
                    args.NSAPRegion = ((tv_MonthViewModel)tvi)._nsapRegion;
                    //tv_CurrentEntities.CurrentMonth = args.MonthSampled;
                    break;

            }
            TreeViewItemSelected?.Invoke(this, args);
        }

        private void OnContextMenuClick(object sender, RoutedEventArgs e)
        {
            AllSamplingEntitiesEventHandler args = new AllSamplingEntitiesEventHandler();
            string treeViewEntity = _selectedItem.GetType().Name;
            args.TreeViewEntity = treeViewEntity;

            switch (((MenuItem)sender).Name)
            {
                case "contextMenuEditGearUnload":
                    args.FishingGround = ((tv_FishingGroundViewModel)_selectedItem)._fishingGround;
                    args.FMA = ((tv_FishingGroundViewModel)_selectedItem)._fma;
                    args.NSAPRegion = ((tv_FishingGroundViewModel)_selectedItem)._region;
                    args.ContextMenuTopic = "editFishingGroundGearUnload";
                    break;

                case "contextMenuCrosstabMonth":
                    args.LandingSiteText = ((tv_MonthViewModel)_selectedItem)._landingSiteName;
                    args.FishingGround = ((tv_MonthViewModel)_selectedItem)._fishingGround;
                    args.FMA = ((tv_MonthViewModel)_selectedItem)._fma;
                    args.NSAPRegion = ((tv_MonthViewModel)_selectedItem)._nsapRegion;
                    args.MonthSampled = DateTime.Parse(((tv_MonthViewModel)_selectedItem).Name);
                    args.ContextMenuTopic = "crosstabByMonth";
                    break;
            }

            

            TreeContextMenu?.Invoke(this, args);
        }

    }

}
