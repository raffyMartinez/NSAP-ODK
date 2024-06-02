using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database.NSAPReports
{
    public  class DateGearBoatsCatch
    {
        public DateTime Date { get; set; }
        public int Boats { get; set; }
        public string GearName { get; set; }
        public double Catch { get; set; }

        public override string ToString()
        {
            return $"Date:{Date.ToString("MMM-dd-yyyy")} - Gear:{GearName} - Boats:{Boats} - Catch:{Catch.ToString("N2")}";
        }
    }
}
