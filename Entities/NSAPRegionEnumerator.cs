using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit;
using NSAP_ODK.Entities.ItemSources;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.Entities
{
    public class NSAPRegionEnumerator

    {
        private NSAPEnumerator _nsapEnumerator;
       
        [ReadOnly(true)]
        public int RowID { get; set; }
        public NSAPRegion NSAPRegion { get; set; }

        public string NSAPRegionCode { get; set; }
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

        public DateTime? DateFirstSampling { get; set; }

        public VesselUnload FirstSampling { get; set; }

        public string FirstSamplingDate
        {
            get
            {
                if (DateFirstSampling == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ((DateTime)DateFirstSampling).ToString("MMM-dd-yyyy");
                }
            }
        }

        public override string ToString()
        {
            return Enumerator.Name;
        }

    }
}
