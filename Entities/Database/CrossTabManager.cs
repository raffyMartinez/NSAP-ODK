﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace NSAP_ODK.Entities.Database
{
    public static class CrossTabManager
    {
        private static List<CrossTabEffort> _crossTabEfforts;
        private static List<CrossTabLenFreq> _crossTabLenFreqs;
        
        public static void GearByMonthYear(TreeViewModelControl.AllSamplingEntitiesEventHandler sev)
        {
            _crossTabEfforts = new List<CrossTabEffort>();
            _crossTabLenFreqs = new List<CrossTabLenFreq>();
            List<GearUnload> gearUnloads = new List<GearUnload>();
            if (sev.GearUsed==null || sev.GearUsed.Length == 0)
            {
                   gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                    .Where(t => t.Parent.NSAPRegion.Code == sev.NSAPRegion.Code &&
                              t.Parent.FMA.FMAID == sev.FMA.FMAID &&
                              t.Parent.FishingGround.Code == sev.FishingGround.Code &&
                              t.Parent.LandingSiteName == sev.LandingSiteText &&
                              t.Parent.SamplingDate.Date >= (DateTime)sev.MonthSampled &&
                              t.Parent.SamplingDate.Date < ((DateTime)sev.MonthSampled).AddMonths(1)).ToList();

            }
            else
            {
                gearUnloads = NSAPEntities.GearUnloadViewModel.GearUnloadCollection
                 .Where(t => t.Parent.NSAPRegion.Code == sev.NSAPRegion.Code &&
                           t.Parent.FMA.FMAID == sev.FMA.FMAID &&
                           t.Parent.FishingGround.Code == sev.FishingGround.Code &&
                           t.Parent.LandingSiteName == sev.LandingSiteText &&
                           t.GearUsedName == sev.GearUsed && 
                           t.Parent.SamplingDate.Date >= (DateTime)sev.MonthSampled &&
                           t.Parent.SamplingDate.Date < ((DateTime)sev.MonthSampled).AddMonths(1)).ToList();
            }


            foreach(var gu in gearUnloads)
            {

                
                foreach (var item in NSAPEntities.VesselCatchViewModel.VesselCatchCollection
                    .Where(t => t.Parent.Parent.PK  == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabEfforts.Add(new CrossTabEffort { CrossTabCommon = ctc });
                }


                
                foreach (var item in NSAPEntities.CatchLenFreqViewModel.CatchLenFreqCollection
                    .Where(t => t.Parent.Parent.Parent.PK == gu.PK).ToList())
                {
                    CrossTabCommon ctc = new CrossTabCommon(item);
                    _crossTabLenFreqs.Add(new CrossTabLenFreq { CrossTabCommon = ctc });
                }

            }




        }
        public static List<CrossTabLength> CrossTabLengths { get; set; }
        public static List<CrossTabMaturity> CrossTabMaturities { get; set; }

        public static DataTable CrossTabEfforts { get; set; }
    }
}
