using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelBingoGroup
    {
        private Grid25GridCell _grid25GridCell;
        private ExcelMainSheet _parent;
        [Column("grid_coord_group/bingo_repeat/bingo_group/bingo_complete")]
        public string BingoCoordinate { get; set; }

        [Column("_parent_index")]
        public int ParentIndex { get; set; }

        public Grid25GridCell Grid25Grid
            {
            set { _grid25GridCell = value; }
            get
            {
                if(_grid25GridCell==null)
                {
                    _grid25GridCell = new Grid25GridCell(new UTMZone(Parent.UTMZone), BingoCoordinate);
                }
                return _grid25GridCell;
            }
            }
        public ExcelMainSheet Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelMainSheets.FirstOrDefault(t => t.RowIndex == ParentIndex);
                }
                return _parent;
            }
        }
    }
}
