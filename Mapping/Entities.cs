﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace NSAP_ODK.Mapping
{
    public enum GPXEntityType
    {
        GPS,
        LogBookImage,
        Fisher,
        Trip,
        Track,
        Waypoint,
        GPXFile
    }
    public static class Entities
    {
        //public static GPSViewModel GPSViewModel { get; set; }
        //public static DetectedDeviceViewModel DetectedDeviceViewModel { get; set; }

        //public static GPXFileViewModel GPXFileViewModel { get; set; }

        //public static GearViewModel GearViewModel { get; set; }

        //public static TripViewModel TripViewModel { get; set; }

        //public static TripWaypointViewModel TripWaypointViewModel { get; set; }

        //public static WaypointViewModel WaypointViewModel { get; set; }

        //public static TrackViewModel TrackViewModel { get; set; }

        //public static DeviceGPXViewModel DeviceGPXViewModel { get; set; }

        public static AOIViewModel AOIViewModel { get; set; }

        //public static LogbookImageViewModel LogbookImageViewModel { get; set; }

        //public static LandingSiteViewModel LandingSiteViewModel { get; set; }

        //public static FisherViewModel FisherViewModel { get; set; }

        //public static CTXFileViewModel CTXFileViewModel { get; set; }

        //public static FisherDeviceAssignmentViewModel FisherDeviceAssignmentViewModel { get; set; }
        
        //public static ExtractedFishingTrackViewModel ExtractedFishingTrackViewModel { get; set; }


        public static bool ClearTables()
        {
            bool result = true;
            //var result = DeviceGPXViewModel.ClearRepository();

            //if (result)
            //{
            //    result = TripWaypointViewModel.ClearRepository();
            //}
            
            //if(result)
            //{
            //    result = TripViewModel.ClearRepository();
            //}

            //if(result)
            //{
            //    result = GPSViewModel.ClearRepository();
            //}
            
            return result;
            
        }
    }


}
