using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npoi.Mapper.Attributes;
namespace NSAP_ODK.Entities
{
    public class ExcelSoakTime
    {
        private ExcelMainSheet _parent;
        
        [Column("soak_time_group/soaktime_tracking_group/soak_time_repeat/set_time")]
        public DateTime DateTimeSet { get; set; }

        [Column("soak_time_group/soaktime_tracking_group/soak_time_repeat/haul_time")]
        public DateTime DateTimeHaul { get; set; }
        
        [Column("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_set")]
        public string GPSWaypointAtSet { get; set; }

        [Column("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_haul")]
        public string GPSWaypointAtHaul { get; set; }
        
        [Column("_parent_index")]
        public int ParentIndex { get; set; }

        public ExcelMainSheet Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAP_ODK.Utilities.ImportExcel.ExcelMainSheets.FirstOrDefault(t=>t.RowIndex==ParentIndex);
                }
                return _parent;
            }
        }
    }
}
