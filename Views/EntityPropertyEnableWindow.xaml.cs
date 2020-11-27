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

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for EntityPropertyEnable.xaml
    /// </summary>
    public partial class EntityPropertyEnableWindow : Window
    {
        private object _nsapObject;
        private NSAPEntity _entityType;
        public EntityPropertyEnableWindow(object nsapObject, NSAPEntity entityType)
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            _nsapObject = nsapObject;
            _entityType = entityType;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckBox chk = null;

            switch(_entityType)
            {
                case NSAPEntity.EffortIndicator:
                    EffortSpecification spec = (EffortSpecification)_nsapObject;
                    Title = spec.Name;
                    LabelTitle.Content = $"List of gears with specification {spec.Name}";
                    var d = NSAPEntities.EffortSpecificationViewModel.GearsHasEffortSpec(spec);
                    foreach(var item in d)
                    {
                        if (item.Key.IsGenericGear)
                        {
                            chk = new CheckBox();
                            chk.Checked += OnCheckBoxValueChanged;
                            chk.Unchecked += OnCheckBoxValueChanged;
                            chk.Margin = new Thickness(20, 5, 0, 5);
                            chk.Content = item.Key.GearName;
                            chk.IsChecked = item.Value;
                            chk.Tag = item;
                            PanelChecks.Children.Add(chk);
                            foreach (var subItem in d)
                            {
                                if( subItem.Key.CodeOfBaseGear!=subItem.Key.Code && subItem.Key.BaseGear.Code==item.Key.Code)
                                {
                                    chk = new CheckBox();
                                    chk.Margin = new Thickness(40, 5, 0, 5);
                                    chk.Content = subItem.Key.GearName;
                                    chk.IsChecked = subItem.Value;
                                    chk.Tag = subItem;
                                    if(item.Value)
                                    {
                                        chk.IsEnabled = !item.Value;
                                    }
                                    else
                                    {
                                        chk.IsEnabled = true;
                                    }
                                    PanelChecks.Children.Add(chk);
                                }
                            }
                        }

                    }
                    break;
            }
        }

        private void OnCheckBoxValueChanged(object sender, RoutedEventArgs e)
        {
            switch(e.RoutedEvent.Name)
            {
                case "Checked":
                    break;
                case "Unchecked":
                    break;
            }
            //throw new NotImplementedException();
        }


        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "ButtonOK":
                    break;
                case "ButtonCancel":
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
