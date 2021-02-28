using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.ItemSources
{
    internal class EffortValueTypeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection valueTypes = new ItemCollection();
            valueTypes.Add(ODKValueType.isDecimal, "Decimal");
            valueTypes.Add(ODKValueType.isInteger, "Integer");
            valueTypes.Add(ODKValueType.isText, "Text");
            valueTypes.Add(ODKValueType.isBoolean, "Yes/No");
            valueTypes.Add(ODKValueType.isUndefined, "Not defined");
            return valueTypes;
        }
    }
}