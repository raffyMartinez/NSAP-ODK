using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CalendarGearSector
    {
        public CalendarGearSector()
        {
            CalendarDays = new List<CalendarDay>();
        }
        public Gear Gear { get; set; }
        public string SectorCode { get; set; }

        public bool NoFishing { get; set; }
        
        public string Sector
        {
            get
            {
                string sector = "Other";
                if (SectorCode == "m")
                {
                    sector = "Municipal";
                }
                else if (SectorCode == "c")
                {
                    sector = "Commercial";
                }
                return sector;
            }
        }
        public List<CalendarDay> CalendarDays { get; set; }
        public CalendarMonth Parent { get; set; }
        public override string ToString()
        {

            if (NoFishing)
            {
                return "NO FISHING DAY";
            }
            else
            {
                int calday_count = 0;
                if (CalendarDays != null)
                {
                    calday_count = CalendarDays.Count;
                }
                return $"{Gear.GearName}: {Sector} ({calday_count})";
            }
        }
        public override int GetHashCode()
        {
            if (NoFishing)
            {
                return "NO FISHING DAY".GetHashCode();
            }
            else
            {
                return (SectorCode, Gear.Code).GetHashCode();
            }
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj != null && obj is FishingGearAndSector)
        //    {
        //        FishingGearAndSector fgs = obj as FishingGearAndSector;
        //        return fgs.Gear.Code == Gear.Code && fgs.SectorCode == SectorCode;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public override bool Equals(object obj)
        {


            if(obj != null && obj is CalendarGearSector)
            {
                CalendarGearSector cgs = obj as CalendarGearSector;
                if (cgs.NoFishing)
                {
                    return NoFishing = true;
                }
                else
                {
                    return SectorCode == cgs.SectorCode && Gear.Code == cgs.Gear.Code;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
