using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelGMS
    {
        private ExcelCatchComposition _parent;
        [Column("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/individual_length")]
        public double? Length { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/individual_weight")]
        public double? Weight { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/sex")]
        public string SexCode { get; set; }

        public string Sex
        {
            get
            {
                return SexCode == "f" ? "Female" :
                       SexCode == "m" ? "Male" : "Juvenile";
            }
        }

        public string GutContentClassification
        {
            get
            {
                return GutContentCode == "F" ? "Full" :
                       GutContentCode == "HF" ? "Half full" :
                       GutContentCode == "E" ? "Empty":""; 
            }
        }

        public string GonadMaturity
        {
            get
            {
                return MaturityCode == "pr" ? "Premature" :
                       MaturityCode == "im" ? "Immature" :
                       MaturityCode == "de" ? "Developing" :
                       MaturityCode == "ma" ? "Maturing" :
                       MaturityCode == "ri" ? "Ripening" :
                       MaturityCode == "mt" ? "Mature" :
                       MaturityCode == "spw" ? "Spawning" :
                       MaturityCode == "gr" ? "Gravid" :
                       MaturityCode == "sp" ?  "Spent": "";
            }
        }

        [Column("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gut_content_category")]
        public string GutContentCode { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gms_repeat")]
        public string MaturityCode { get; set; }

        [Column("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/stomach_content_wt")]
        public double? StomachContentWeight { get; set; }
        
        [Column("_parent_index")]
        public int ParentIndex { get; set; }



        public ExcelCatchComposition Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelCatchCompositions.FirstOrDefault(t=>t.RowIndex==ParentIndex);
                }
                return _parent;
            }
        }
    }
}
