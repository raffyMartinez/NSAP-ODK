using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelLenFreq
    {
        private ExcelCatchComposition _parent;
        [Column("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/length_class")]
        public double LengthClass { get; set; }
        [Column("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/freq")]
        public int Frequency{ get; set; }
        [Column("_parent_index")]
        public int ParentIndex { get; set; }

        public ExcelCatchComposition Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelCatchCompositions.FirstOrDefault(t => t.RowIndex == ParentIndex);
                }
                return _parent;
            }
        }
    }
}
