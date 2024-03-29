﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class DownloadHistoryEntity
    {
        public static DateTime DownloadDate { get; set; }
        public static List<VesselUnload> VesselUnloads { get; set; }
        public static List<LandingSiteSampling> LandingSiteGearUnload { get; set; }
    }
    public static class DownloadToDatabaseHistory
    {
        //public static Dictionary<DateTime, List<VesselUnload>> DownloadToDatabaseHistoryDictionary 
        public static Dictionary<DateTime, List<SummaryItem>> DownloadToDatabaseHistoryDictionary
        {
            get
            {
                var d= NSAPEntities.SummaryItemViewModel.SummaryItemCollection
                        .OrderByDescending(t => t.DateAdded)
                        .GroupBy(t => ((DateTime)t.DateAdded).Date)
                        .ToDictionary(t => t.Key, t => t.ToList());
                return d;   

                //return NSAPEntities.SummaryItemViewModel.SummaryItemCollection
                //    .OrderByDescending(t => t.DateAdded)
                //    .GroupBy(t => ((DateTime)t.DateAdded).Date)
                //    .ToDictionary(t => t.Key, t => t.ToList());

                //return NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection
                //    .OrderByDescending(t=>t.DateAddedToDatabase)
                //    .GroupBy(t => ((DateTime)t.DateAddedToDatabase).Date)
                //    .ToDictionary(t => t.Key, t => t.ToList());
            }

        }

        public static Dictionary<DateTime, DownloadHistoryEntity> DownloadToDatabaseHistoryDictionary1
        {
            get
            {
                return null;
            }
        }

    }
}
