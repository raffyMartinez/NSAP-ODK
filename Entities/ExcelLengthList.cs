using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelLengthList
    {
        private ExcelCatchComposition _parent;

        [Column("_parent_index")]
        public int ParentIndex { get; set; }
        public ExcelCatchComposition Parent
        {
            get
            {
                if(_parent==null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelCatchCompositions.FirstOrDefault(t=>t.RowIndex==ParentIndex);
                }
                return _parent;
            }
            set { _parent = value; }
        }

        [Column("catch_comp_group/catch_composition_repeat/length_list_repeat/length")]
        public double Length { get; set; }
    }
}
