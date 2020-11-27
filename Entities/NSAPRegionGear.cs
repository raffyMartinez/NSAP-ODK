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

    public class NSAPRegionGear
    {
        private Gear _gear;

        public NSAPRegionGear()
        {
            //EffortSpecifications = new List<GearEffortSpecification>();
        }
        [ReadOnly(true)]
        public int RowID { get; set; }
        public NSAPRegion NSAPRegion { get; set; }
        [ItemsSource(typeof(GearItemsSource))]
        public string GearCode { get; set; }  
        
        
        public Gear Gear
        {
            get { return _gear; }
            set
            {
                _gear = value;
                GearCode = _gear.Code;
            }
        }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime DateStart { get; set; }

        [Editor(typeof(DateTimePickerEditor), typeof(DateTimePicker))]
        public DateTime? DateEnd { get; set; }
        public override string ToString()
        {
            return Gear.ToString();
        }

        //public List<GearEffortSpecification> EffortSpecifications;
    }
}
