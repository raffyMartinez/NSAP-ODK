using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit;
namespace NSAP_ODK.Entities
{
    public class NSAPRegionEnumerator

    {
        private NSAPEnumerator _nsapEnumerator;
       
        [ReadOnly(true)]
        public int RowID { get; set; }
        public NSAPRegion NSAPRegion { get; set; }
        public NSAPEnumerator Enumerator 
        { get { return _nsapEnumerator; }
            set
            {
                _nsapEnumerator = value;
                EnumeratorID = _nsapEnumerator.ID;

            }
        }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime DateStart { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime? DateEnd { get; set; }

        [ItemsSource(typeof(NSAPEnumeratorItemsSource))]
        public int EnumeratorID { get; set; }

        public override string ToString()
        {
            return Enumerator.Name;
        }
    }
}
