using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public enum KeyValuePairDataTypeEnum
    {
        dataTypeInt,
        dataTypeDate,
        dataTypeText,
        dataTypeDouble,
        dataTypeBool
    }
    public class ObjectDataHolder_KeyValuePair
    {
        public KeyValuePair<string, string> KeyValuePair { get; set; }

        public KeyValuePairDataTypeEnum DataType { get; set; }
    }

    public class ObjectInstanceDataHolder
    {
        public List<ObjectDataHolder_KeyValuePair> ObjectInstanceData { get; set; }
        public string ObjectDataType { get; set; }
        public int Sequence { get; set; }
    }

    public class ObjectInstancesDataHolder
    {
        public List<ObjectInstanceDataHolder> ObjectInstancesData { get; set; }
        public string TableName { get; set; }

    }
}
