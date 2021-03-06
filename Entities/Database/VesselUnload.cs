﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.ItemSources;

namespace NSAP_ODK.Entities.Database
{
    using System.ComponentModel;
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
    using Xceed.Wpf.Toolkit;



    [CategoryOrder("Header", 1)]
    [CategoryOrder("Effort", 2)]
    [CategoryOrder("Tracking", 3)]
    [CategoryOrder("Device metadata", 4)]
    public class VesselUnloadEdit
    {
        public VesselUnloadEdit()
        {

        }
        public VesselUnloadEdit(VesselUnload vesselUnload)
        {
            if (vesselUnload != null)
            {
                NSAPRegion = vesselUnload.Parent.Parent.NSAPRegion;
                Region = NSAPRegion.Name;
                RegionCode = NSAPRegion.Code;
                FMA = vesselUnload.Parent.Parent.FMA;
                FMAName = FMA.Name;
                FMAID = FMA.FMAID;
                FishingGround = vesselUnload.Parent.Parent.FishingGround;
                FishingGroundName = FishingGround.Name;
                FishingGroundCode = FishingGround.Code;
                LandingSite = vesselUnload.Parent.Parent.LandingSite;
                LandingSiteName = vesselUnload.Parent.Parent.LandingSiteName;
                LandingSiteID = LandingSite.LandingSiteID;
                OtherLandingSite = vesselUnload.Parent.Parent.LandingSiteText;

                Gear = vesselUnload.Parent.Gear;
                GearName = vesselUnload.Parent.GearUsedName;
                if (Gear != null)
                {
                    GearCode = Gear.Code;
                }
                else
                {
                    GearCode = "";
                }
                OtherFishingGear = vesselUnload.Parent.GearUsedText;

                Identifier = vesselUnload.PK;
                SamplingDate = vesselUnload.SamplingDate;
                IsBoatUsed = vesselUnload.IsBoatUsed;
                VesselID = vesselUnload.VesselID;
                VesselText = vesselUnload.VesselText;
                WeightOfCatch = vesselUnload.WeightOfCatch;
                WeightOfCatchSample = vesselUnload.WeightOfCatchSample;
                Boxes = vesselUnload.Boxes;
                BoxesSampled = vesselUnload.BoxesSampled;
                OperationIsSuccessful = vesselUnload.OperationIsSuccessful;
                RaisingFactor = vesselUnload.RaisingFactor;

                //NSAPRegionEnumeratorID = vesselUnload.NSAPRegionEnumeratorID;
                NSAPEnumeratorID = vesselUnload.NSAPEnumeratorID;
                EnumeratorText = vesselUnload.EnumeratorText;


                OperationIsTracked = vesselUnload.OperationIsTracked;
                DepartureFromLandingSite = vesselUnload.DepartureFromLandingSite;
                ArrivalAtLandingSite = vesselUnload.ArrivalAtLandingSite;
                GPSCode = vesselUnload.GPSCode;
                Notes = vesselUnload.Notes;

                Submitted = vesselUnload.DateTimeSubmitted.ToString("MMM-dd-yyyy HH:mm");
                FormVersion = vesselUnload.FormVersion;
                UserName = vesselUnload.UserName;
                DeviceID = vesselUnload.DeviceID;
                XFormIdentifier = vesselUnload.XFormIdentifier;
                XFormDate = vesselUnload.XFormDate == null ? "" : ((DateTime)vesselUnload.XFormDate).ToString("MMM-dd-yyyy HH:mm");
                DateAddedToDatabase = ((DateTime)vesselUnload.DateAddedToDatabase).ToString("MMM-dd-yyyy HH:mm");
                SectorCode = vesselUnload.SectorCode;
                FromExcelDownload = vesselUnload.FromExcelDownload;
            }

        }
        [ReadOnly(true)]
        public NSAPRegion NSAPRegion { get; set; }
        public string Region { get; set; }

        [ItemsSource(typeof(NSAPRegionItemsSource))]
        public string RegionCode { get; set; }
        [ReadOnly(true)]
        public string FMAName { get; set; }

        public FMA FMA { get; set; }

        [ItemsSource(typeof(FMAInRegionItemsSource))]
        public int FMAID { get; set; }

        public FishingGround FishingGround { get; set; }

        [ReadOnly(true)]
        public string FishingGroundName { get; set; }

        [ItemsSource(typeof(FishingGroundInRegionFMAItemsSource))]
        public string FishingGroundCode { get; set; }

        public LandingSite LandingSite { get; set; }
        [ReadOnly(true)]
        public string LandingSiteName { get; set; }

        public string OtherLandingSite { get; set; }

        [ItemsSource(typeof(LandingSiteInFMAFishingGroundItemsSource))]
        public int? LandingSiteID { get; set; }

        public Gear Gear { get; set; }
        [ReadOnly(true)]
        public string GearName { get; set; }

        [ItemsSource(typeof(GearsInNSAPRegionItemsSource))]
        public string GearCode { get; set; }

        public string OtherFishingGear { get; set; }

        [ReadOnly(true)]
        public int Identifier { get; set; }

        [Editor(typeof(DateTimePickerWithTime), typeof(DateTimePicker))]
        public DateTime SamplingDate { get; set; }

        public bool IsBoatUsed { get; set; }

        [ItemsSource(typeof(FishingVesselInRegionItemsSource))]
        public int? VesselID { get; set; }

        public string VesselText { get; set; }

        public double? WeightOfCatch { get; set; }

        public double? WeightOfCatchSample { get; set; }

        public int? Boxes { get; set; }

        public int? BoxesSampled { get; set; }

        public double? RaisingFactor { get; set; }

        public string Notes { get; set; }

        [ItemsSource(typeof(RegionEnumeratorItemsSource))]
        public int? NSAPRegionEnumeratorID { get; set; }

        public string EnumeratorText { get; set; }

        [ItemsSource(typeof(RegionEnumeratorItemsSource))]
        public int? NSAPEnumeratorID { get; set; }

        public bool OperationIsSuccessful { get; set; }

        public bool OperationIsTracked { get; set; }

        [Editor(typeof(DateTimePickerWithTime), typeof(DateTimePicker))]
        public DateTime? DepartureFromLandingSite { get; set; }

        [Editor(typeof(DateTimePickerWithTime), typeof(DateTimePicker))]
        public DateTime? ArrivalAtLandingSite { get; set; }

        [ItemsSource(typeof(SectorTypeItemsSource))]
        public string SectorCode { get; set; }

        [ItemsSource(typeof(GPSItemsSource))]
        public string GPSCode { get; set; }

        [ReadOnly(true)]
        public string DateAddedToDatabase { get; set; }


        //ODK Metadata

        /// <summary>
        /// GUID of row, this value is the same in all excel sheet versions
        /// </summary>
        [ReadOnly(true)]
        public string ODKRowID { get; set; }

        /// <summary>
        /// name part of excel filename
        /// </summary>
        [ReadOnly(true)]
        public string XFormIdentifier { get; set; }

        /// <summary>
        /// date part of filename
        /// </summary>
        [ReadOnly(true)]
        public string XFormDate { get; private set; }

        /// <summary>
        /// user name  inputted into the device
        /// </summary>
        [ReadOnly(true)]
        public string UserName { get; set; }

        /// <summary>
        /// unique id of the device used for encoding
        /// </summary>
        [ReadOnly(true)]
        public string DeviceID { get; set; }

        /// <summary>
        /// Date when data was uploaded by the device into the net
        /// </summary>
        [ReadOnly(true)]
        public string Submitted { get; private set; }


        /// <summary>
        /// form version 
        /// </summary>
        [ReadOnly(true)]
        public string FormVersion { get; set; }

        /// <summary>
        /// to differentiated from excel and json download 
        /// </summary>
        [ReadOnly(true)]
        public bool FromExcelDownload { get; set; }
    }
    public class VesselUnloadTrackedFlattened
    {
        public int SamplingDayID { get; set; }
        public string Region { get; set; }
        public string FMA { get; set; }
        public string FishingGround { get; set; }

        public string LandingSite { get; set; }
        public DateTime SamplingDate { get; set; }
        public int GearUnloadID { get; set; }
        public string Gear { get; set; }
        public int? BoatsLanded { get; set; }

        public double? RaisingFactor { get; set; }
        public double? CatchTotalLanded { get; set; }
        public int VesselUnloadID { get; set; }
        public DateTime SamplingDateTime { get; set; }

        public string Enumerator { get; set; }

        public bool IsBoatUsed { get; set; }
        public string Vessel { get; set; }
        public double? CatchTotalWt { get; set; }
        public double? CatchSampleWt { get; set; }
        public int? Boxes { get; set; }
        public int? BoxesSampled { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsTracked { get; set; }
        public string GPS { get; set; }
        public DateTime? Departure { get; set; }
        public DateTime? Arrival { get; set; }
        public string RowID { get; set; }
        public string XFormIdentifier { get; set; }
        public DateTime? XFormDate { get; set; }
        public string UserName { get; set; }
        public string DeviceID { get; set; }

        public DateTime Submitted { get; set; }
        public string FormVersion { get; set; }
        public string Notes { get; set; }
        public DateTime? DateAddedToDatabase { get; set; }

        public string Sector { get; set; }

        public bool FromExcelDownload { get; set; }

    }


    [CategoryOrder("Header", 1)]
    [CategoryOrder("Effort", 2)]
    [CategoryOrder("Tracking", 3)]
    [CategoryOrder("Device metadata", 4)]
    public class VesselUnloadForDisplay
    {
        public VesselUnloadForDisplay(VesselUnload vesselUnload)
        {
            Region = vesselUnload.Parent.Parent.NSAPRegion.Name;
            FMA = vesselUnload.Parent.Parent.FMA.Name;
            FishingGround = vesselUnload.Parent.Parent.FishingGround.Name;
            LandingSite = vesselUnload.Parent.Parent.LandingSiteName;
            FishingGear = vesselUnload.Parent.GearUsedName;

            Identifier = vesselUnload.PK;
            SamplingDate = vesselUnload.SamplingDate.ToString("dd-MMM-yyyy HH:mm");
            Enumerator = vesselUnload.EnumeratorName;
            IsBoatUsed = vesselUnload.IsBoatUsed;
            FishingVessel = vesselUnload.VesselName;
            SectorCode = vesselUnload.Sector;
            OperationIsSuccessful = vesselUnload.OperationIsSuccessful;
            WeightOfCatch = vesselUnload.WeightOfCatch;
            WeightOfCatchSample = vesselUnload.WeightOfCatchSample;
            Boxes = vesselUnload.Boxes;
            BoxesSampled = vesselUnload.BoxesSampled;
            RaisingFactor = vesselUnload.RaisingFactor;
            Notes = vesselUnload.Notes;

            OperationIsTracked = vesselUnload.OperationIsTracked;
            DepartureFromLandingSite = vesselUnload.DepartureFromLandingSite == null ? "" : ((DateTime)vesselUnload.DepartureFromLandingSite).ToString("dd-MMM-yyyy HH:mm");
            ArrivalAtLandingSite = vesselUnload.ArrivalAtLandingSite == null ? "" : ((DateTime)vesselUnload.ArrivalAtLandingSite).ToString("dd-MMM-yyyy HH:mm");
            GPS =   vesselUnload.GPS==null ? "" : vesselUnload.GPS.AssignedName;

            UserName = vesselUnload.UserName;
            DeviceID = vesselUnload.DeviceID;
            XFormIdentifier = vesselUnload.XFormIdentifier;
            XFormDate =   vesselUnload.XFormDate==null ? "" :  ((DateTime)vesselUnload.XFormDate).ToString("dd-MMM-yyyy HH:mm");
            FormVersion = vesselUnload.FormVersion;
            Submitted = vesselUnload.DateTimeSubmitted.ToString("dd-MMM-yyyy HH:mm");
            DateAddedToDatabase = ((DateTime)vesselUnload.DateAddedToDatabase).ToString("dd-MMM-yyyy HH:mm");

        }
        public string Region { get; private set; }
        public string FMA { get; private set; }
        public string FishingGround { get; private set; }
        public string LandingSite { get; private set; }
        public string FishingGear { get; private set; }
        public int Identifier { get; private set; }
        public string SamplingDate { get; private set; }
        public string Enumerator { get; private set; }

        public bool IsBoatUsed { get; private set; }
        public string FishingVessel { get; private set; }

        public string SectorCode { get; private set; }

        public bool OperationIsSuccessful { get; private set; }

        public double? WeightOfCatch { get; private set; }

        public double? WeightOfCatchSample { get; private set; }
        public int? Boxes { get; private set; }

        public int? BoxesSampled { get; private set; }

        public double? RaisingFactor { get; private set; }

        public string Notes { get; private set; }


        public bool OperationIsTracked { get; private set; }

        public string DepartureFromLandingSite { get; private set; }

        public string ArrivalAtLandingSite { get; private set; }

        public string GPS { get; private set; }
        public string UserName { get; private set; }
        public string DeviceID { get; private set; }
        public string XFormIdentifier { get; private set; }
        public string XFormDate { get; private set; }
        public string FormVersion { get; private set; }
        public string Submitted { get; private set; }
        public string DateAddedToDatabase { get; private set; }
        public string FromExcelDownload { get; private set; }


    }
    public class VesselUnloadFlattened
    {
        public VesselUnloadFlattened(VesselUnload vesselUnload)
        {

            ID = vesselUnload.PK;
            ParentID = vesselUnload.Parent.PK;
            SamplingDate = vesselUnload.SamplingDate;
            LandingSite = vesselUnload.Parent.Parent.LandingSiteName;
            IsBoatUsed = vesselUnload.IsBoatUsed;
            FishingVessel = vesselUnload.VesselName;
            Gear = vesselUnload.Parent.GearUsedName;
            WeightOfCatch = vesselUnload.WeightOfCatch;
            WeightOfCatchSample = vesselUnload.WeightOfCatchSample;
            SamplingEnumerator = vesselUnload.EnumeratorName;
            Boxes = vesselUnload.Boxes;
            BoxesSampled = vesselUnload.BoxesSampled;
            RaisingFactor = vesselUnload.RaisingFactor;
            OperationIsSuccessful = vesselUnload.OperationIsSuccessful;
            OperationIsTracked = vesselUnload.OperationIsTracked;
            DepartureFromLandingSite = vesselUnload.DepartureFromLandingSite;
            ArrivalAtLandingSite = vesselUnload.ArrivalAtLandingSite;
            GPS = vesselUnload.GPSText;
            ODKRowID = vesselUnload.ODKRowID;
            XFormIdentifier = vesselUnload.XFormIdentifier;
            XFormDate = vesselUnload.XFormDate;
            UserName = vesselUnload.UserName;
            DeviceID = vesselUnload.DeviceID;
            FormVersion = vesselUnload.FormVersion;
            Notes = vesselUnload.Notes;
            Submitted = vesselUnload.DateTimeSubmitted;
            DateAddedToDatabase = vesselUnload.DateAddedToDatabase;
            Sector = vesselUnload.Sector;
            FromExcelDownload = vesselUnload.FromExcelDownload;
        }
        public int ID { get; set; }
        public int ParentID { get; set; }

        public string LandingSite { get; set; }

        public bool IsBoatUsed { get; set; }
        public string FishingVessel { get; set; }
        public string Gear { get; set; }
        public double? WeightOfCatch { get; set; }
        public double? WeightOfCatchSample { get; set; }
        public string SamplingEnumerator { get; set; }
        public int? Boxes { get; set; }

        public double? RaisingFactor { get; set; }
        public int? BoxesSampled { get; set; }
        public bool OperationIsSuccessful { get; set; }

        public bool OperationIsTracked { get; set; }
        public DateTime? DepartureFromLandingSite { get; set; }
        public DateTime? ArrivalAtLandingSite { get; set; }
        public string Sector { get; set; }
        public string GPS { get; set; }

        public string ODKRowID { get; set; }
        public string XFormIdentifier { get; set; }
        public DateTime? XFormDate { get; set; }

        public string UserName { get; set; }
        public string DeviceID { get; set; }
        public DateTime Submitted { get; set; }
        public string FormVersion { get; set; }
        public string Notes { get; set; }
        public DateTime SamplingDate { get; set; }

        public DateTime? DateAddedToDatabase { get; set; }

        public bool FromExcelDownload { get; set; }
    }
    public class VesselUnload
    {
        private GPS _gps;
        private GearUnload _parent;
        private FishingVessel _fishingVessel;
        private NSAPEnumerator _nsapEnumerator;

        public DateTime TimeStart { get; set; }
        public int PK { get; set; }
        public int GearUnloadID { get; set; }
        public double? WeightOfCatch { get; set; }
        public int? NSAPRegionEnumeratorID { get; set; }
        public string EnumeratorText { get; set; }

        public DateTime MonthSampled
        {
            get

            {
                var sDate = Parent.Parent.SamplingDate;
                return new DateTime(sDate.Year, sDate.Month, 1);
            }
        }

        public int? NSAPEnumeratorID { get; set; }

        public NSAPEnumerator NSAPEnumerator
        {
            get
            {
                if (_nsapEnumerator == null && NSAPEnumeratorID != null)
                {
                    _nsapEnumerator = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)NSAPEnumeratorID);
                }
                return _nsapEnumerator;
            }
            set { _nsapEnumerator = value; }
        }
        public string EnumeratorName
        {
            get
            {
                if (NSAPEnumeratorID == null)
                {
                    return EnumeratorText;
                }
                else
                {
                    return NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)NSAPEnumeratorID).Name;
                }
            }
        }
        public string CatchCompositionCountText
        {
            get { return OperationIsSuccessful ? ListVesselCatch.Count.ToString() : ""; }
        }
        public string WeightOfCatchText
        {
            get
            {
                return OperationIsSuccessful ? ((Double)WeightOfCatch).ToString("N1") : "";
            }
        }
        public double? WeightOfCatchSample { get; set; }
        public int? Boxes { get; set; }
        public double? RaisingFactor { get; set; }
        public string Sector
        {
            get
            {
                return SectorCode == "c" ? "Commercial" :
                       SectorCode == "m" ? "Municipal" : "";
            }
        }
        public string SectorCode { get; set; }
        public int? BoxesSampled { get; set; }
        public bool FromExcelDownload { get; set; }

        public bool IsBoatUsed { get; set; }
        public int? VesselID { get; set; }

        public string VesselText { get; set; }

        public List<VesselEffort> ListVesselEffort
        {
            get
            {
                return NSAPEntities.VesselEffortViewModel.VesselEffortCollection
                    .Where(t => t.Parent.PK == PK).ToList();
            }
        }

        public string VesselName
        {
            get
            {

                if (VesselText != null && VesselText.Length > 0)
                {
                    return VesselText;
                }
                else
                {

                    return FishingVessel != null ? FishingVessel.ToString() : "";
                }
            }

        }
        public List<GearSoak> ListGearSoak
        {
            get
            {
                return NSAPEntities.GearSoakViewModel.GearSoakCollection
                    .Where(t => t.Parent.PK == PK).ToList();
            }
        }
        public List<FishingGroundGrid> ListFishingGroundGrid
        {
            get
            {
                return NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection
                    .Where(t => t.Parent.PK == PK).ToList();
            }
        }
        public List<VesselCatch> ListVesselCatch
        {
            get
            {
                return NSAPEntities.VesselCatchViewModel.VesselCatchCollection
                    .Where(t => t.Parent.PK == PK).ToList();
            }
        }
        public bool OperationIsSuccessful { get; set; }

        public bool OperationIsTracked { get; set; }
        public DateTime? DepartureFromLandingSite { get; set; }
        public DateTime? ArrivalAtLandingSite { get; set; }
        public string ODKRowID { get; set; }

        public FishingVessel FishingVessel
        {
            set { _fishingVessel = value; }
            get
            {
                if (_fishingVessel == null && VesselID != null)
                {
                    _fishingVessel = NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)VesselID);
                }
                return _fishingVessel;
            }
        }
        public GearUnload Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.GearUnloadViewModel.getGearUnload(GearUnloadID);
                }
                return _parent;
            }
        }

        public string XFormIdentifier { get; set; }
        public DateTime? XFormDate { get; set; }

        public string UserName { get; set; }
        public string DeviceID { get; set; }
        public DateTime DateTimeSubmitted { get; set; }
        public string FormVersion { get; set; }
        public string GPSCode { get; set; }
        public string Notes { get; set; }
        public DateTime SamplingDate { get; set; }

        public GPS GPS
        {
            set { _gps = value; }
            get
            {
                if (_gps == null)
                {
                    _gps = NSAPEntities.GPSViewModel.GetGPS(GPSCode);
                }
                return _gps;
            }
        }

        public string GPSText
        {
            get
            {
                return GPS == null ? "" : GPS.ToString();
            }
        }

        public DateTime? DateAddedToDatabase { get; set; }

    }
}
