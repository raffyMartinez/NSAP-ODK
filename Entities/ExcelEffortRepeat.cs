using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelEffortRepeat
    {
        private ExcelMainSheet _parent;
        private EffortSpecification _effortSpec;

        [Column("efforts_group/effort_repeat/group_effort/effort_type")]
        public int EffortTypeID { get; set; }

        [Column("efforts_group/effort_repeat/group_effort/response_type")]
        public string EffortValueTypeCode { get; set; }

        [Column("efforts_group/effort_repeat/group_effort/effort_intensity")]
        public Double? EffortIntensityNumericValue { get; set; }

        [Column("efforts_group/effort_repeat/group_effort/effort_desc")]
        public string EffortIntenstityTextValue { get; set; }

        [Column("_parent_index")]
        public int ParentIndex { get; set; }

        public ExcelMainSheet Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelMainSheets.FirstOrDefault(t => t.RowIndex == ParentIndex);
                }
                return _parent;
            }

        }

        public EffortSpecification EffortSpecification
        {
            set { _effortSpec = value; }
            get
            {
                if(_effortSpec==null)
                {
                    _effortSpec = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(EffortTypeID);
                }
                return _effortSpec;
            }

        }

        public string EfforValueText
        {
            get
            {
                string textValue = EffortIntenstityTextValue;
                if (EffortIntensityNumericValue!=null)
                {
                    textValue = ((double)EffortIntensityNumericValue).ToString();
                }
                return textValue;
            }
        }
    }
}
