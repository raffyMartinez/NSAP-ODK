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
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;

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
        string _currentCheckedMenuName="contextMenuMonitoriedLandingsCalendar";

        public TreeControl()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                InitializeComponent();
            }
            CalendarView = CalendarViewType.calendarViewTypeSampledLandings;
        }


        public void ReadDatabase()
        {
            if (NSAPEntities.NSAPRegionViewModel != null
                && NSAPEntities.NSAPRegionViewModel.Count > 0
                && NSAPEntities.FMAViewModel.Count > 0
                && NSAPEntities.FishSpeciesViewModel.Count > 0)
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

            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime && NSAPEntities.NSAPRegionViewModel != null)
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
            //args.TreeViewItem = _selectedItem;
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
                    args.LandingSiteText = ((tv_LandingSiteViewModel)tvi)._landingSiteText;
                    args.TreeViewItem = (tv_LandingSiteViewModel)tvi;
                    //tv_CurrentEntities.CurrentLandingSite = args.LandingSite;
                    break;

                case "tv_MonthViewModel":
                    
                    args.MonthSampled = DateTime.Parse(((tv_MonthViewModel)tvi)._month);
                    args.LandingSite = ((tv_MonthViewModel)tvi)._landingSite;
                    args.LandingSiteText = ((tv_MonthViewModel)tvi)._landingSiteName;
                    args.FishingGround = ((tv_MonthViewModel)tvi)._fishingGround;
                    args.FMA = ((tv_MonthViewModel)tvi)._fma;
                    args.NSAPRegion = ((tv_MonthViewModel)tvi)._nsapRegion;
                    args.CalendarView = CalendarView;
                    
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

            string menuName = ((MenuItem)sender).Name;
            switch (menuName)
            {
                case "contextMenuGearUnloadFishingGround":
                    args.FishingGround = ((tv_FishingGroundViewModel)_selectedItem)._fishingGround;
                    args.FMA = ((tv_FishingGroundViewModel)_selectedItem)._fma;
                    args.NSAPRegion = ((tv_FishingGroundViewModel)_selectedItem)._region;
                    //args.ContextMenuTopic = menuName;
                    break;

                case "contextMenuCrosstabLandingSite":
                case "contextMenuGearUnloadLandingSite":
                    args.LandingSiteText = ((tv_LandingSiteViewModel)_selectedItem)._landingSiteText;
                    args.FishingGround = ((tv_LandingSiteViewModel)_selectedItem)._fishingGround;
                    args.FMA = ((tv_LandingSiteViewModel)_selectedItem)._fma;
                    args.NSAPRegion = ((tv_LandingSiteViewModel)_selectedItem)._region;
                    //switch (menuName)
                    //{
                    //    case "contextMenuCrosstabLandingSite":
                    //        args.ContextMenuTopic = "crosstabByLandingSite";
                    //        break;
                    //    case "contextMenuGearUnloadLandingSite":
                    //        args.ContextMenuTopic = "editGearGearUnloadByLandingSite";
                    //        break;
                    //}
                    break;

                case "contextMenuCrosstabMonth":
                case "contextMenuGearUnloadMonth":
                    args.LandingSite = ((tv_MonthViewModel)_selectedItem)._landingSite;
                    args.LandingSiteText = ((tv_MonthViewModel)_selectedItem)._landingSiteName;
                    args.FishingGround = ((tv_MonthViewModel)_selectedItem)._fishingGround;
                    args.FMA = ((tv_MonthViewModel)_selectedItem)._fma;
                    args.NSAPRegion = ((tv_MonthViewModel)_selectedItem)._nsapRegion;
                    args.MonthSampled = DateTime.Parse(((tv_MonthViewModel)_selectedItem).Name);

                    //switch(menuName)
                    //{
                    //    case "contextMenuCrosstabMonth":
                    //        args.ContextMenuTopic = "crosstabByMonth";
                    //        break;
                    //    case "contextMenuGearUnloadMonth":
                    //        args.ContextMenuTopic = "editGearGearUnloadByMonth";
                    //        break;
                    //}
                    break;
            }
            args.ContextMenuTopic = menuName;


            TreeContextMenu?.Invoke(this, args);
        }

        public CalendarViewType CalendarView { get; private set; }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void UncheckEditMenuItems(ContextMenu cm, MenuItem source)
        {
            foreach (var mi in cm.Items)
            {
                if (mi.GetType().Name != "Separator")
                {
                    var menu = (MenuItem)mi;
                    if (menu.IsCheckable)
                    {

                        if (menu.Name != source.Name)
                        {
                            menu.IsChecked = false;
                        }


                    }
                    //if (menu.Name != ((MenuItem)e.Source).Name)
                    //{
                    //menu.IsChecked = false;
                    //}
                }
            }
        }
        private void OnContextMenuCheckChange(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            if(mi.Name==_currentCheckedMenuName)
            {
                mi.IsChecked = true;
                e.Handled = true;
                return;
            }
            _currentCheckedMenuName = mi.Name;
            UncheckEditMenuItems(mi.Parent as ContextMenu, source: mi);
            if (mi.IsCheckable)
            {
                switch (mi.Name)
                {
                    case "contextMenuMonitoriedLandingsCalendar":
                        if (mi.IsChecked)
                        {
                            CalendarView = CalendarViewType.calendarViewTypeSampledLandings;
                        }
                        break;
                    case "contextMenuAllLandingsCalendar":
                        if (mi.IsChecked)
                        {
                            CalendarView = CalendarViewType.calendarViewTypeCountAllLandingsByGear;
                        }
                        break;
                    case "contextMenuTotalLandedWtCalendar":
                        if(mi.IsChecked)
                        {
                            CalendarView = CalendarViewType.calendarViewTypeWeightAllLandingsByGear;
                        }
                        break;
                }

                var tvi = treeView.SelectedItem;
                AllSamplingEntitiesEventHandler args = new AllSamplingEntitiesEventHandler();
                args.MonthSampled = DateTime.Parse(((tv_MonthViewModel)tvi)._month);
                args.LandingSite = ((tv_MonthViewModel)tvi)._landingSite;
                args.LandingSiteText = ((tv_MonthViewModel)tvi)._landingSiteName;
                args.FishingGround = ((tv_MonthViewModel)tvi)._fishingGround;
                args.FMA = ((tv_MonthViewModel)tvi)._fma;
                args.NSAPRegion = ((tv_MonthViewModel)tvi)._nsapRegion;
                args.CalendarView = CalendarView;
                args.TreeViewEntity = tvi.GetType().Name;

                TreeViewItemSelected?.Invoke(this, args);
            }
        }
    }

}
