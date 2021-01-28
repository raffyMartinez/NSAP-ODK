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
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EditGearUnloadByRegionFMAWIndow.xaml
    /// </summary>
    public partial class EditGearUnloadByRegionFMAWIndow : Window
    {
        private List<GearUnload> _gearUnloads;
        private TreeViewModelControl.AllSamplingEntitiesEventHandler _treeItemData;
        private bool _saveChanges;
        public EditGearUnloadByRegionFMAWIndow(TreeViewModelControl.AllSamplingEntitiesEventHandler treeItemData)
        {
            InitializeComponent();
            _treeItemData = treeItemData;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {

            switch((( Button)sender).Name)
            {
                case "buttonOk":
                    SaveChanges();
                    Close();
                    break;
                case "buttonUndo":
                    UndoChanges();
                    break;
                case "buttonCancel":
                    UndoChanges();
                    Close();
                    break;
            }
            
        }


        private void UndoChanges(bool refresh=true)
        {
            NSAPEntities.GearUnloadViewModel.UndoChangesToGearUnloadBoatCatch(_gearUnloads);
            if (refresh)
            {
                dataGridGearUnload.Items.Refresh();
            }

        }
        private void SaveChanges()
        {
            NSAPEntities.GearUnloadViewModel.SaveChangesToBoatAndCatch(_gearUnloads);
            _saveChanges = true;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _saveChanges = false;
            //_gearUnloads = NSAPEntities.GearUnloadViewModel.GetAllGearUnloads
            //    (
            //    _treeItemData.NSAPRegion,
            //    _treeItemData.FMA,
            //    _treeItemData.FishingGround
            //    );

            _gearUnloads = NSAPEntities.GearUnloadViewModel.GetAllGearUnloads(_treeItemData);

            dataGridGearUnload.DataContext = _gearUnloads;

            dataGridGearUnload.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Parent.LandingSiteName"), IsReadOnly = true });
            dataGridGearUnload.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("GearUsedName"), IsReadOnly = true });
            var col = new DataGridTextColumn()
            {
                Binding = new Binding("Parent.SamplingDate"),
                Header = "Sampling date",
                IsReadOnly=true
            };
            col.Binding.StringFormat = "MMM-dd-yyyy";
            dataGridGearUnload.Columns.Add(col);

            dataGridGearUnload.Columns.Add(new DataGridTextColumn { Header = "Number of boats landed", Binding = new Binding("Boats"), IsReadOnly = false });
            dataGridGearUnload.Columns.Add(new DataGridTextColumn { Header = "Weight total catch landed", Binding = new Binding("Catch"), IsReadOnly = false });
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!_saveChanges)
            {
                UndoChanges();
            }

            this.SavePlacement();
        }

        //This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }



        private void OnDatagridLoadingRow(object sender, DataGridRowEventArgs e)
        {
             e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
