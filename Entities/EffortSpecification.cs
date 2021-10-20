using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Entities
{
    public enum ODKValueType
    {
        isUndefined,
        isInteger,
        isDecimal,
        isText,
        isBoolean
    }

    public class EffortSpecification
    {
        [ReadOnly(true)]
        public int ID { get; set; }

        public string Name { get; set; }

        
        public bool IsForAllTypesFishing { get; set; }

        [ItemsSource(typeof(EffortValueTypeItemsSource))]
        public ODKValueType ValueType { get; set; }

        public string ValueTypeString
        {
            get
            {
                string valueType = "";
                switch (ValueType)
                {
                    case ODKValueType.isDecimal:
                        valueType = "Decimal";
                        break;

                    case ODKValueType.isInteger:
                        valueType = "Integer";
                        break;

                    case ODKValueType.isText:
                        valueType = "Text";
                        break;

                    case ODKValueType.isUndefined:
                        valueType = "Not defined";
                        break;

                    case ODKValueType.isBoolean:
                        valueType = "Yes/No";
                        break;
                }
                return valueType;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}