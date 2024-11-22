using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarDailySummary
    {
        public CalendarDailySummary()
        {
            CalendarDays = new List<CalendarDay>();
        }
        public List<CalendarDay> CalendarDays { get; set; }
    }
}
