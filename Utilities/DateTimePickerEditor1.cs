using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace NSAP_ODK.Utilities
{
    public class DateTimePickerEditor1 : DateTimePicker, ITypeEditor
    {
        public DateTimePickerEditor1()
        {
            Format = DateTimeFormat.Custom;
            FormatString = "dd.MM.yyyy";

            TimePickerVisibility = System.Windows.Visibility.Collapsed;
            ShowButtonSpinner = false;
            AutoCloseCalendar = true;
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Binding binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

            BindingOperations.SetBinding(this, ValueProperty, binding);
            return this;
        }
    }
}
