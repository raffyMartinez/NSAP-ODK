﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities;
using Xceed.Wpf.Toolkit;
using System.Windows.Markup;

namespace NSAP_ODK.Mapping
{
    public class TripEdited
    {
        private int? _operatorID;
        public TripEdited() { }
        public TripEdited(GPS gps)
        {
            //GPS = gps;
            //DeviceID = GPS.DeviceID;
            //Track = new Track();
        }

        public TripEdited(Trip trip)
        {
            GPS = trip.GPS;
            Operator = trip.Operator;
            VesselName = trip.VesselName;
            DateTimeArrival = trip.DateTimeArrival;
            DateTimeDeparture = trip.DateTimeDeparture;
            TripID = trip.TripID;
            GearCode = trip.Gear.Code;
            DeviceID = trip.DeviceID;
            TripWaypoints = trip.TripWaypoints;
            Track = trip.Track;
            Notes = trip.Notes;
            GPXFileName = trip.GPXFileName;
            XML = trip.XML;
            Trip = trip;

        }

        public Trip Trip { get; set; }
        public string TrackSummary
        {
            get
            {
                if (Track.Statistics != null)
                {
                    var h = (int)Track.Statistics.Duration.TotalMinutes / 60;
                    var m = Track.Statistics.Duration.TotalMinutes % 60;
                    return $"Length: {Track.Statistics.Length.ToString("N2")} Duration: { h.ToString("000")}:{ m.ToString("N1")} Points: {Track.Waypoints.Count}";
                }
                else
                {
                    return "";
                }
            }
        }
        public Track Track { get; set; }

        public Fisher Operator { get; set; }
        public DateTime MonthYear
        {
            get
            {
                return new DateTime(DateTimeDeparture.Year, DateTimeDeparture.Month, 1);
            }
        }
        [Editor(typeof(DateTimePickerWithTime), typeof(DateTimePicker))]
        public DateTime DateTimeDeparture { get; set; }

        [Editor(typeof(DateTimePickerWithTime), typeof(DateTimePicker))]
        public DateTime DateTimeArrival { get; set; }

        [ItemsSource(typeof(GearItemsSource))]
        public string GearCode { get; set; }

        public string GPXFileName { get; set; }

        public string Notes { get; set; }

        public string XML { get; set; }

        //[ReadOnly(true)]
        [ItemsSource(typeof(GPSItemsSource))]
        public string DeviceID { get; set; }
        public string OtherGear { get; set; }
        [ReadOnly(true)]
        public int TripID { get; set; }

        [ItemsSource(typeof(VesselNameItemsSource))]
        public string VesselName { get; set; }

        //[ReadOnly(true)]
        public GPS GPS { get; set; }

        [ItemsSource(typeof(FisherItemsSource))]
        public int? OperatorID
        {
            get
            {
                if (_operatorID == null && Operator != null)
                {
                    _operatorID = Operator.FisherID;
                }
                return _operatorID;
            }
            set { _operatorID = value; }
        }


        public List<TripWaypointLite> TripWaypoints { get; set; }

    }
    public class Trip
    {

        private string _deviceID;
        private int? _operatorID;
        public Trip()
        {
            Track = new Track();
        }
        public DateTime DateTimeDeparture { get; set; }
        public string Notes { get; set; }

        public DateTime DateTimeArrival { get; set; }
        public int? OperatorID
        {
            get
            {
                if (_operatorID == null)
                {
                    _operatorID = Operator.FisherID;
                }
                return _operatorID;
            }
            set { _operatorID = value; }
        }

        public Track Track { get; set; }
        public Gear Gear { get; set; }
        public string GPXFileName { get; set; }
        public string DeviceID
        {
            get { return _deviceID; }
            set
            {
                _deviceID = value;
                GPS = Entities.GPSViewModel.GetGPSEx(_deviceID);
            }
        }
        public string OtherGear { get; set; }
        public override string ToString()
        {
            return $"{TripID.ToString()} ({GPS.DeviceName})";
        }

        public DateTime MonthYear
        {
            get
            {
                return new DateTime(DateTimeDeparture.Year, DateTimeDeparture.Month, 1);
            }
        }
        public int TripID { get; set; }
        public string VesselName { get; set; }

        [ReadOnly(true)]
        public GPS GPS { get; set; }

        public string XML { get; set; }

        public List<TripWaypoint> Waypoints
        {
            get
            {
                return Entities.TripWaypointViewModel.GetAllTripWaypoints(TripID);
            }
        }
        public int WaypointCount
        {
            get
            {
                if (Waypoints == null)
                {
                    return 0;
                }
                else
                {
                    return Waypoints.Count;
                }
                //var waypoints = Entities.TripWaypointViewModel.GetAllTripWaypoints(TripID);
                //if (waypoints == null)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return waypoints.Count;
                //}
            }
        }

        public Fisher Operator { get { return Entities.FisherViewModel.GetFisher((int)OperatorID); } }
        public List<TripWaypointLite> TripWaypoints { get; set; }

        public List<int> ShapeIndexes { get; set; }

        public bool ShownInMap { get; set; }
        public string TrackSummary
        {
            get
            {
                if (Track.Statistics != null)
                {
                    var h = (int)Track.Statistics.Duration.TotalMinutes / 60;
                    var m = Track.Statistics.Duration.TotalMinutes % 60;
                    return $"Length: {Track.Statistics.Length.ToString("N2")} Duration: { h.ToString("000")}:{ m.ToString("N1")} Points: {Track.Waypoints.Count}";
                }
                else
                {
                    return "";
                }
            }
        }

        public DateTime DateAdded { get; set; }
    }
}
