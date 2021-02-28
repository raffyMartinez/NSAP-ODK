using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Entities
{
    public class GearEffortSpecification
    {
        private EffortSpecification _effortSpecification;
        public Gear Gear { get; set; }

        public EffortSpecification EffortSpecification
        {
            get { return _effortSpecification; }
            set
            {
                _effortSpecification = value;
                EffortSpecificationID = _effortSpecification.ID;
            }
        }

        [ItemsSource(typeof(EffortSpecificationItemsSource))]
        public int EffortSpecificationID { get; set; }

        [ReadOnly(true)]
        public int RowID { get; set; }

        public override string ToString()
        {
            return $"{Gear.ToString()}-{EffortSpecification.ToString()}";
        }
    }
}