using System;
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
    using Newtonsoft.Json;

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
                if (LandingSite != null)
                {
                    LandingSiteID = LandingSite.LandingSiteID;
                }
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
                RefNo = vesselUnload.RefNo;

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
                FishingTripIsCompleted = vesselUnload.FishingTripIsCompleted;
                RaisingFactor = vesselUnload.RaisingFactor;
                HasCatchComposition = vesselUnload.HasCatchComposition;
                NumberOfFishers = vesselUnload.NumberOfFishers;


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
                DateAddedToDatabase = vesselUnload.DateAddedToDatabase == null ? DateTime.Now.ToString("MMM-dd-yyyy HH:mm") : ((DateTime)vesselUnload.DateAddedToDatabase).ToString("MMM-dd-yyyy HH:mm");
                SectorCode = vesselUnload.SectorCode;
                FromExcelDownload = vesselUnload.FromExcelDownload;
                IsCatchSold = vesselUnload.IsCatchSold;
                IsMultigear = vesselUnload.IsMultiGear;
                CountGearTypesUsed = vesselUnload.CountGearTypesUsed;
                MainGearName = vesselUnload.GearUsed;
                NumberOfSpeciesInCatchComposition = vesselUnload.NumberOfSpeciesInCatchComposition;

                //if (vesselUnload.VesselCatchViewModel == null)
                //{
                //    vesselUnload.VesselCatchViewModel = new VesselCatchViewModel(vesselUnload);
                //}
                //VesselCatches = vesselUnload.VesselCatchViewModel.VesselCatchCollection.ToList();
            }

        }
        [ReadOnly(true)]
        public NSAPRegion NSAPRegion { get; set; }

        //public List<VesselCatch> VesselCatches { get; set; }
        public int? NumberOfSpeciesInCatchComposition { get; set; }
        public bool FishingTripIsCompleted { get; set; }
        public string Region { get; set; }

        [ItemsSource(typeof(NSAPRegionItemsSource))]
        public string RegionCode { get; set; }
        [ReadOnly(true)]
        public string FMAName { get; set; }
        public string RefNo { get; set; }
        public FMA FMA { get; set; }

        [ItemsSource(typeof(FMAInRegionItemsSource))]
        public int FMAID { get; set; }
        [ReadOnly(true)]
        public bool IsMultigear { get; set; }
        public FishingGround FishingGround { get; set; }
        public bool IsCatchSold { get; set; }
        public int? NumberOfFishers { get; set; }

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
        [ItemsSource(typeof(GearsInNSAPRegionItemsSource))]
        public string MainGearName { get; set; }
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

        public int? CountGearTypesUsed { get; set; }

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
        //[ReadOnly(true)]
        public bool HasCatchComposition { get; set; }

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

    public class VesselUnloadWithMaturityFlattened
    {
        public VesselUnloadWithMaturityFlattened()
        {
            ListOfCatchWithMaturity = new List<VesselCatch>();
        }
        public int SamplingDayID { get; set; }
        public string Region { get; set; }
        public string FMA { get; set; }
        public string FishingGround { get; set; }

        public string LandingSite { get; set; }
        public DateTime SamplingDate { get; set; }
        public int GearUnloadID { get; set; }
        public string Gear { get; set; }
        public int VesselUnloadID { get; set; }
        public DateTime SamplingDateTime { get; set; }

        public string Enumerator { get; set; }

        public bool IsBoatUsed { get; set; }
        public string Vessel { get; set; }
        public double? CatchTotalWt { get; set; }
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

        public List<VesselCatch> ListOfCatchWithMaturity { get; set; }

        public string FishingGroundGird { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }


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
            RefNo = vesselUnload.RefNo;

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
            IsMultigear = vesselUnload.IsMultiGear;
            IsCatchSold = vesselUnload.IsCatchSold;
            IncludeEffortIndicators = vesselUnload.IncludeEffortIndicators;
            Notes = vesselUnload.Notes;

            OperationIsTracked = vesselUnload.OperationIsTracked;
            FishingTripIsCompleted = vesselUnload.FishingTripIsCompleted;
            DepartureFromLandingSite = vesselUnload.DepartureFromLandingSite == null ? "" : ((DateTime)vesselUnload.DepartureFromLandingSite).ToString("dd-MMM-yyyy HH:mm");
            ArrivalAtLandingSite = vesselUnload.ArrivalAtLandingSite == null ? "" : ((DateTime)vesselUnload.ArrivalAtLandingSite).ToString("dd-MMM-yyyy HH:mm");
            GPS = vesselUnload.GPS == null ? "" : vesselUnload.GPS.AssignedName;

            UserName = vesselUnload.UserName;
            DeviceID = vesselUnload.DeviceID;
            XFormIdentifier = vesselUnload.XFormIdentifier;
            XFormDate = vesselUnload.XFormDate == null ? "" : ((DateTime)vesselUnload.XFormDate).ToString("dd-MMM-yyyy HH:mm");
            FormVersion = vesselUnload.FormVersion;
            Submitted = vesselUnload.DateTimeSubmitted.ToString("dd-MMM-yyyy HH:mm");
            DateAddedToDatabase = vesselUnload.DateAddedToDatabase == null ? DateTime.Now.ToString("MMM-dd-yyyy HH:mm") : ((DateTime)vesselUnload.DateAddedToDatabase).ToString("dd-MMM-yyyy HH:mm");

            HasCatchComposition = vesselUnload.HasCatchComposition;
            NumberOfFishers = vesselUnload.NumberOfFishers;
            CountGearTypesUsed = vesselUnload.CountGearTypesUsed;
            ODKRowID = vesselUnload.ODKRowID;
        }
        public bool IncludeEffortIndicators { get; private set; }
        public int? CountGearTypesUsed { get; private set; }
        public bool IsCatchSold { get; private set; }
        public string Region { get; private set; }
        public string FMA { get; private set; }
        public string FishingGround { get; private set; }
        public string LandingSite { get; private set; }
        public string FishingGear { get; private set; }

        public string RefNo { get; private set; }
        public int Identifier { get; private set; }
        public string SamplingDate { get; private set; }
        public string Enumerator { get; private set; }
        public string ODKRowID { get; private set; }
        public bool IsBoatUsed { get; private set; }
        public string FishingVessel { get; private set; }

        public string SectorCode { get; private set; }

        public bool OperationIsSuccessful { get; private set; }

        public bool FishingTripIsCompleted { get; private set; }

        public double? WeightOfCatch { get; private set; }

        public int? NumberOfFishers { get; private set; }
        public double? WeightOfCatchSample { get; private set; }
        public int? Boxes { get; private set; }

        public int? BoxesSampled { get; private set; }

        public double? RaisingFactor { get; private set; }

        public string Notes { get; private set; }

        public bool IsMultigear { get; private set; }
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

        public bool HasCatchComposition { get; private set; }


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
            FishingTripIsCompleted = vesselUnload.FishingTripIsCompleted;
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
            HasCatchComposition = vesselUnload.HasCatchComposition;
            IsMultigear = vesselUnload.IsMultiGear;
            IsCatchSold = vesselUnload.IsCatchSold;
            CountGearTypesUsed = vesselUnload.CountGearTypesUsed;
        }
        public int ID { get; set; }
        public int? CountGearTypesUsed { get; set; }
        public bool IsMultigear { get; set; }

        public bool IsCatchSold { get; set; }
        public int ParentID { get; set; }
        public bool FishingTripIsCompleted { get; set; }
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
        public bool HasCatchComposition { get; set; }
    }
    public enum LandedCatchValidationResult
    {
        ValidationResultNotApplicable,
        ValidationResultNoValidationDone,
        ValidationResultNoCatchComposition,
        ValidationResultCatchWeightIsInvalid,
        ValidationResultCatchWeightIsValid,
    }

    public class VesselUnloadWeights
    {
        private VesselUnload _vesselUnload;
        private string _raisingFactor = "";
        private string _catchWeight = "";
        private string _catchSampleWeight = "";
        private string _sumCatchCompositonWeight = "";
        private string _sumCatchCompositionWeightFromSample = "";
        private string _differenceInWeights = "";
        public VesselUnloadWeights(VesselUnload vu)
        {
            _vesselUnload = vu;
            if (_vesselUnload.RaisingFactor != null && _vesselUnload.RaisingFactor > 0)
            {
                _raisingFactor = ((double)_vesselUnload.RaisingFactor).ToString("N2");
            }
            if (_vesselUnload.DifferenceCatchWtAndSumCatchCompWt != null && _vesselUnload.WeightOfCatchSample != null)
            {
                _differenceInWeights = ((double)_vesselUnload.DifferenceCatchWtAndSumCatchCompWt).ToString("N2");
            }
            if (_vesselUnload.SumOfCatchCompositionWeights != 0)
            {
                _sumCatchCompositonWeight = _vesselUnload.SumOfCatchCompositionWeights.ToString("N2");
            }
            if (_vesselUnload.WeightOfCatch != null)
            {
                _catchWeight = ((double)_vesselUnload.WeightOfCatch).ToString("N2");
            }
            if (_vesselUnload.WeightOfCatchSample != null)
            {
                _catchSampleWeight = ((double)_vesselUnload.WeightOfCatchSample).ToString("N2");
            }
            if (_vesselUnload.SumOfSampleWeights != 0)
            {
                _sumCatchCompositionWeightFromSample = _vesselUnload.SumOfSampleWeights.ToString("N2");
            }
        }

        public string SumOfCatchCompositionFromSampleWeight
        {
            get { return _sumCatchCompositionWeightFromSample; }
        }
        public string DifferenceInWeights
        {
            get { return _differenceInWeights; }
        }
        public string SumOfCatchCompositionWeights
        {
            get { return _sumCatchCompositonWeight; }
        }
        public string CatchWeight
        {
            get
            {
                return _catchWeight;
            }
        }

        public string CatchSampleWeight
        {
            get
            {
                return _catchSampleWeight;
            }
        }
        public string RaisingFactor
        {
            get { return _raisingFactor; }
        }
    }

    //[JsonObject(MemberSerialization.OptIn)]
    public class VesselUnload
    {
        private LandedCatchValidationResult _landedCatchValidationResult;
        private GPS _gps;
        private GearUnload _parent;
        private FishingVessel _fishingVessel;
        private NSAPEnumerator _nsapEnumerator;
        private double _runningSum = 0;
        private bool _speciesWeightIsZero;
        public bool IncludeEffortIndicators { get; set; }
        public int? NumberOfSpeciesInCatchComposition { get; set; }
        public int? SequenceOfSampling { get; set; }
        public int? CountGearTypesUsed { get; set; }
        public LandedCatchValidationResult LandedCatchValidationResult
        {
            get { return _landedCatchValidationResult; }
        }

        //[JsonProperty]
        //public int ParentPK { get { return Parent.PK; } }
        public string Gears
        {
            get
            {
                if (IsMultiGear)
                {
                    if (VesselUnload_FishingGearsViewModel == null || VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection == null)
                    {
                        VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(this);
                    }
                    string g = "";
                    foreach (var item in VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                    {
                        g += $"{item.GearUsedName},";
                    }
                    return g.Trim(',');
                }
                else
                {
                    return Parent.GearUsedName;
                }
            }
        }
        public string JSONFileName { get; set; }
        public VesselUnloadWeights VesselUnloadWeights { get; set; }
        public bool IsCatchSold { get; set; }
        public bool DelayedSave { get; set; }
        //public VesselUnloadViewModel ContainerViewModel { get; set; }
        public VesselCatchViewModel VesselCatchViewModel { get; set; }

        public VesselUnload_FishingGearViewModel VesselUnload_FishingGearsViewModel { get; set; }
        public FishingGroundGridViewModel FishingGroundGridViewModel { get; set; }

        public GearSoakViewModel GearSoakViewModel { get; set; }

        public VesselEffortViewModel VesselEffortViewModel { get; set; }
        public override string ToString()
        {
            if (Parent.Parent == null)
            {
                return $"[ID:{PK}] {VesselName}-{SamplingDate.ToString("MMM-dd-yyyy")}-Enumerator:{EnumeratorName}-Sector:{SectorCode}";
            }
            else
            {
                return $"[ID:{PK}] {VesselName}-{Parent.Parent.LandingSiteName}-{SamplingDate.ToString("MMM-dd-yyyy")}-Enumerator:{EnumeratorName}-Sector:{SectorCode}";
            }
        }

        public string DateSampling { get { return SamplingDate.ToString("MMM-dd-yyyy"); } }

        public string DateTimeSampling { get { return SamplingDate.ToString("MMM-dd-yyyy HH:mm"); } }
        public bool HasCatchComposition { get; set; }
        public DateTime TimeStart { get; set; }
        public int PK { get; set; }
        public int GearUnloadID { get; set; }
        public double? WeightOfCatch { get; set; }
        public int? NSAPRegionEnumeratorID { get; set; }
        public string EnumeratorText { get; set; }
        public string RefNo { get; set; }
        public int CountGrids { get; set; }
        public int CountGearSoak { get; set; }
        public int CountEffortIndicators { get; set; }
        public int CountCatchCompositionItems { get; set; }
        public int CountLengthRows { get; set; }
        public int CountLenFreqRows { get; set; }
        public int CountLenWtRows { get; set; }
        public int CountMaturityRows { get; set; }
        public int? NumberOfFishers { get; set; }
        public bool FishingTripIsCompleted { get; set; }

        public string GearUsed
        {
            get
            {
                return Parent.GearUsedName;
            }
        }
        public bool HasBSCInCatchComposition()
        {
            if (ListVesselCatch == null)
            {
                return false;
            }
            else if (ListVesselCatch.Count == 0)
            {
                return false;
            }
            else
            {
                return ListVesselCatch.Where(t => t.CatchName == "Portunus pelagicus").FirstOrDefault() != null;
            }
        }

        public DateTime MonthSampled
        {
            get

            {
                var sDate = Parent.Parent.SamplingDate;
                return new DateTime(sDate.Year, sDate.Month, 1);
            }
        }

        public int? NSAPEnumeratorID { get; set; }

        public string RaisingFactorText
        {
            get
            {
                if (RaisingFactor == null)
                {
                    return "";
                }
                else
                {
                    return ((double)RaisingFactor).ToString("N1");
                }
            }
        }
        public double? RaisingFactor { get; set; }
        //{
        //    get
        //    {
        //        if (WeightOfCatch == null || WeightOfCatchSample == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            if (FormVersionNumeric >= 6.43 && ListVesselCatch.Count > 0)
        //            {
        //                double from_total_sum = ListVesselCatch.Where(t => t.FromTotalCatch).Sum(t => (double)t.Catch_kg);
        //                //double from_total_sum = 0;
        //                //foreach (var item in ListVesselCatch.OrderByDescending(t => t.FromTotalCatch))
        //                //{
        //                //    if (item.FromTotalCatch)
        //                //    {
        //                //        from_total_sum += (double)item.Catch_kg;
        //                //    }
        //                //    else
        //                //    {
        //                //        break;
        //                //    }
        //                //}
        //                return ((double)WeightOfCatch - from_total_sum) / WeightOfCatchSample;
        //            }
        //            else
        //            {
        //                return (double)WeightOfCatch / (double)WeightOfCatchSample;
        //            }
        //        }
        //    }
        //}

        public double SumOfSampleWeights { get; set; }
        //{
        //    get
        //    {
        //        if (ListVesselCatch.Count > 0)
        //        {
        //            double runningSum = 0;
        //            foreach (var item in ListVesselCatch)
        //            {
        //                if (item.Sample_kg != null)
        //                {
        //                    runningSum += (double)item.Sample_kg;
        //                }
        //            }
        //            return runningSum;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public double? PrecisionOfWeights
        {
            get
            {
                //return ((double)WeightOfCatch/ Math.Abs((double)WeightOfCatch - _runningSum)) * 100;
                if (WeightOfCatch == null)
                {
                    return null;
                }
                else
                {
                    return Math.Abs((double)WeightOfCatch - _runningSum) / (double)WeightOfCatch * 100;
                }
            }
        }

        public string PrecisionOfWeightsText
        {
            get
            {
                if (PrecisionOfWeights == null)
                {
                    return "";
                }
                else
                {
                    return ((double)PrecisionOfWeights).ToString("N1");
                }
            }
        }
        public string ValidationResult
        {
            get
            {
                switch (_landedCatchValidationResult)
                {
                    case LandedCatchValidationResult.ValidationResultCatchWeightIsInvalid:
                        return "Not valid";
                    case LandedCatchValidationResult.ValidationResultNotApplicable:
                        return "Not applicable";
                    case LandedCatchValidationResult.ValidationResultCatchWeightIsValid:
                        return "Valid";
                    default:
                        if (ListVesselCatch.Count == 0)
                        {
                            return "Not applicable - No catch composition";
                        }
                        else
                        {
                            return "Else case";
                        }

                }
            }
        }

        public bool SpeciesWeightIsZero
        {
            get { return _speciesWeightIsZero; }
        }
        public string WeightValidationFlagText
        {
            get
            {
                switch (WeightValidationFlag)
                {
                    case WeightValidationFlag.WeightValidationInValid:
                        return "Not valid";
                    case WeightValidationFlag.WeightValidationNotApplicable:
                        return "Not applicable";
                    case WeightValidationFlag.WeightValidationValid:
                        return "Valid";
                    case WeightValidationFlag.WeightValidationNotValidated:
                        return "Not validated";
                    default:
                        return "";
                }
            }
        }

        public string SamplingTypeFlagText
        {
            get
            {
                switch (SamplingTypeFlag)
                {
                    case SamplingTypeFlag.SamplingTypeMixed:
                        return "Mixed";
                    case SamplingTypeFlag.SamplingTypeNone:
                        return "None";
                    case SamplingTypeFlag.SamplingTypeSampled:
                        return "Sample of catch";
                    case SamplingTypeFlag.SamplingTypeTotalEnumeration:
                        return "Total enumeration";
                    default:
                        return "";
                }
            }
        }
        public NSAP_ODK.Entities.Database.WeightValidationFlag WeightValidationFlag { get; set; }
        public NSAP_ODK.Entities.Database.SamplingTypeFlag SamplingTypeFlag { get; set; }

        public double? DifferenceCatchWtAndSumCatchCompWt { get; set; }
        public string DifferenceCatchWtAndSumCatchCompWtText
        {
            get
            {
                if (DifferenceCatchWtAndSumCatchCompWt == null) return "";
                else
                {
                    if ((double)DifferenceCatchWtAndSumCatchCompWt == 0 || (double)DifferenceCatchWtAndSumCatchCompWt < 0.1)
                    {
                        return "0";
                    }
                    else
                    {
                        return ((double)DifferenceCatchWtAndSumCatchCompWt).ToString("N1");
                    }
                }
            }
        }
        public double SumOfCatchCompositionWeights { get; set; }
        //public double? SumOfCatchCompositionWeights { get; set; }
        //{
        //    get
        //    {
        //        _speciesWeightIsZero = false;
        //        _runningSum = 0;

        //        //if (FormVersionNumeric >= 6.43 && ListVesselCatch.Count > 0 && RaisingFactor != null && WeightOfCatchSample!=null)
        //        if (FormVersionNumeric >= 6.43 && ListVesselCatch.Count > 0 && WeightOfCatchSample != null)
        //        {
        //            double? rf = null;
        //            double wt_of_from_total = 0;
        //            //var g = ListVesselCatch.OrderByDescending(t => t.FromTotalCatch).ToList();
        //            foreach (var sp in ListVesselCatch.OrderByDescending(t => t.FromTotalCatch).ToList())
        //            {
        //                if (sp.FromTotalCatch)
        //                {
        //                    _runningSum += (double)sp.Catch_kg;
        //                    wt_of_from_total += (double)sp.Catch_kg;
        //                }
        //                else if (sp.Sample_kg != null)
        //                {
        //                    if (rf == null)
        //                    {
        //                        rf = ((double)WeightOfCatch - wt_of_from_total) / (double)WeightOfCatchSample;
        //                    }
        //                    _runningSum += (double)sp.Sample_kg * (double)rf;
        //                }
        //            }
        //        }
        //        else if (ListVesselCatch.Count > 0 && RaisingFactor != null)
        //        {
        //            double? rf = (double)RaisingFactor;
        //            //double runningSum = 0;

        //            if (WeightOfCatchSample != null)
        //            {
        //                foreach (var item in ListVesselCatch)
        //                {
        //                    if (item.Catch_kg == 0 && !_speciesWeightIsZero)
        //                    {
        //                        _speciesWeightIsZero = true;
        //                    }

        //                    if (item.Sample_kg != null && item.Sample_kg < WeightOfCatchSample)
        //                    {
        //                        _runningSum += (double)item.Sample_kg * (double)rf;
        //                    }
        //                    else
        //                    {
        //                        _runningSum += (double)item.Catch_kg;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (var item in ListVesselCatch)
        //                {
        //                    if (item.Catch_kg == 0 && !_speciesWeightIsZero)
        //                    {
        //                        _speciesWeightIsZero = true;
        //                    }
        //                    _runningSum += (double)item.Catch_kg;
        //                }
        //            }

        //        }
        //        else if (ListVesselCatch.Count > 0)
        //        {
        //            foreach (var item in ListVesselCatch)
        //            {
        //                _runningSum += (double)item.Catch_kg;

        //            }
        //            //return _runningSum;
        //        }
        //        else
        //        {
        //            _landedCatchValidationResult = LandedCatchValidationResult.ValidationResultNoCatchComposition;
        //            return null;
        //        }
        //        if (_speciesWeightIsZero)
        //        {
        //            _landedCatchValidationResult = LandedCatchValidationResult.ValidationResultCatchWeightIsInvalid;
        //        }
        //        else
        //        {
        //            if (PrecisionOfWeights <= Utilities.Global.Settings.AcceptableWeightsDifferencePercent)
        //            {
        //                _landedCatchValidationResult = LandedCatchValidationResult.ValidationResultCatchWeightIsValid;
        //            }
        //            else
        //            {
        //                _landedCatchValidationResult = LandedCatchValidationResult.ValidationResultCatchWeightIsInvalid;
        //            }
        //        }

        //        return _runningSum;
        //    }
        //}
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
                    NSAPEnumerator ne = NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)NSAPEnumeratorID);
                    if (ne != null)
                    {
                        return ne.Name;
                    }
                    else
                    {
                        return string.Empty;
                    }
                    //return NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)NSAPEnumeratorID).Name;
                }
            }
        }
        public string CatchCompositionCountText
        {
            get { return OperationIsSuccessful ? ListVesselCatch.Count.ToString() : ""; }
        }

        public int? CatchCompositionCountValue
        {
            get
            {
                if (OperationIsSuccessful)
                {
                    return ListVesselCatch.Count;
                }
                else
                {
                    return null;
                }
            }
        }

        public double? WeightOfCatchValue
        {
            get
            {
                if (OperationIsSuccessful)
                {
                    return (double)WeightOfCatch;
                }
                else
                {
                    return null;
                }
            }
        }
        public string WeightOfCatchText
        {
            get
            {
                return OperationIsSuccessful ? ((Double)WeightOfCatch).ToString("N1") : "";
            }
        }

        public string WeightOfCatchSampleText
        {
            get
            {
                if (WeightOfCatchSample == null)
                {
                    return "";
                }
                else
                {
                    return ((double)WeightOfCatchSample).ToString("N1");
                }
            }
        }
        public double? WeightOfCatchSample { get; set; }
        public int? Boxes { get; set; }
        //public double? RaisingFactor { get; set; }
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

        public List<VesselUnload_Gear_Spec> ListVesselGearSpec
        {
            get
            {
                List<VesselUnload_Gear_Spec> vu_gses = new List<VesselUnload_Gear_Spec>();
                if (VesselUnload_FishingGearsViewModel == null || VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection == null)
                {
                    VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(this);
                }
                foreach (VesselUnload_FishingGear vu_fg in VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                {
                    if (vu_fg.VesselUnload_Gear_Specs_ViewModel == null)
                    {
                        vu_fg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel(vu_fg);
                    }
                    vu_gses.AddRange(vu_fg.VesselUnload_Gear_Specs_ViewModel.VesselUnload_Gear_SpecCollection.ToList());
                }
                return vu_gses;
            }
        }
        public List<VesselUnload_FishingGear> ListUnloadFishingGears
        {
            get
            {
                List<VesselUnload_FishingGear> vu_fgs = new List<VesselUnload_FishingGear>();
                if (VesselUnload_FishingGearsViewModel == null || VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection == null)
                {
                    VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(this);
                }
                foreach (VesselUnload_FishingGear vu_fg in VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                {
                    vu_fgs.Add(vu_fg);
                }
                return vu_fgs;
            }
        }
        public List<VesselEffort> ListVesselEffort
        {
            get
            {
                if (VesselEffortViewModel == null || VesselEffortViewModel.VesselEffortCollection == null)
                {
                    VesselEffortViewModel = new VesselEffortViewModel(this);
                }
                return VesselEffortViewModel.VesselEffortCollection.ToList();
                //.Where(t => t.Parent != null && t.Parent.PK == PK).ToList();
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

                    //return FishingVessel != null ? FishingVessel.ToString() : "";
                    return FishingVessel != null ? FishingVessel.NameToUse(addPrefix: false) : "";
                }
            }

        }
        public List<GearSoak> ListGearSoak
        {
            get
            {
                if (GearSoakViewModel == null || GearSoakViewModel.GearSoakCollection == null)
                {
                    GearSoakViewModel = new GearSoakViewModel(this);
                }
                return GearSoakViewModel.GearSoakCollection?.ToList();
                //.Where(t => t.Parent != null && t.Parent.PK == PK).ToList();
            }
        }
        public List<FishingGroundGrid> ListFishingGroundGrid
        {
            get
            {
                if (FishingGroundGridViewModel == null || FishingGroundGridViewModel.FishingGroundGridCollection == null)
                {
                    FishingGroundGridViewModel = new FishingGroundGridViewModel(this);
                }
                return FishingGroundGridViewModel.FishingGroundGridCollection.ToList();
                //.Where(t => t.Parent != null && t.Parent.PK == PK).ToList();
            }
        }
        public void SetSubModels()
        {
            if (VesselCatchViewModel == null)
            {
                VesselCatchViewModel = new VesselCatchViewModel(this);
            }
            if (VesselUnload_FishingGearsViewModel == null)
            {
                VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(this);
            }
            if (FishingGroundGridViewModel == null)
            {
                FishingGroundGridViewModel = new FishingGroundGridViewModel(this);
            }
            if (GearSoakViewModel == null)
            {
                GearSoakViewModel = new GearSoakViewModel(this);
            }
            if (VesselEffortViewModel == null)
            {
                VesselEffortViewModel = new VesselEffortViewModel(this);
            }
        }
        public void GetCounts()
        {
            if (VesselCatchViewModel == null || VesselCatchViewModel.VesselCatchCollection == null)
            {
                VesselCatchViewModel = new VesselCatchViewModel(this);
            }
            if (VesselCatchViewModel != null)
            {
                foreach (VesselCatch vc in VesselCatchViewModel.VesselCatchCollection)
                {
                    CountLenFreqRows += vc.CatchLenFreqViewModel.CatchLenFreqCollection.Count;
                    CountLengthRows += vc.CatchLengthViewModel.CatchLengthCollection.Count;
                    CountLenWtRows += vc.CatchLengthWeightViewModel.CatchLengthWeightCollection.Count;
                    CountMaturityRows += vc.CatchMaturityViewModel.CatchMaturityCollection.Count;
                }
            }
            if (FishingGroundGridViewModel.FishingGroundGridCollection == null)
            {
                FishingGroundGridViewModel = new FishingGroundGridViewModel(this);
            }
            if (GearSoakViewModel.GearSoakCollection == null)
            {
                GearSoakViewModel = new GearSoakViewModel(this);
            }
            if (VesselEffortViewModel.VesselEffortCollection == null)
            {
                VesselEffortViewModel = new VesselEffortViewModel(this);
            }
            CountGrids = FishingGroundGridViewModel.FishingGroundGridCollection.Count;
            CountGearSoak = GearSoakViewModel.GearSoakCollection.Count;
            CountCatchCompositionItems = VesselCatchViewModel.VesselCatchCollection.Count;
            CountEffortIndicators = VesselEffortViewModel.VesselEffortCollection.Count;
        }
        public List<VesselCatch> ListVesselCatch
        {
            get
            {
                List<VesselCatch> vcs = new List<VesselCatch>();
                if (Parent.Parent.IsMultiVessel)
                {
                    if(VesselUnload_FishingGearsViewModel==null)
                    {
                        VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(this);
                    }
                    if (VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection == null)
                    {
                        VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(this);
                    }
                    foreach (var fg in VesselUnload_FishingGearsViewModel.VesselUnload_FishingGearsCollection)
                    {
                        if (fg.CountItemsInCatchComposition > 0)
                        {
                            foreach (VesselCatch c in fg.VesselCatchViewModel.VesselCatchCollection)
                            {
                                vcs.Add(c);
                            }
                        }
                    }
                    return vcs;

                }
                else
                {
                    if (VesselCatchViewModel == null)
                    {
                        VesselCatchViewModel = new VesselCatchViewModel(this);

                    }
                    SetSubModels();
                    GetCounts();
                    return VesselCatchViewModel.VesselCatchCollection.ToList();
                }

            }
        }

        public List<VesselCatchEdited> ListVesselCatchEdited
        {
            get
            {
                List<VesselCatchEdited> the_list = new List<VesselCatchEdited>();
                foreach (VesselCatch vc in ListVesselCatch)
                {
                    VesselCatchEdited vce = new VesselCatchEdited(vc);
                    the_list.Add(vce);
                }
                return the_list;
            }
        }
        public bool OperationIsSuccessful { get; set; }

        public bool OperationIsTracked { get; set; }
        public DateTime? DepartureFromLandingSite { get; set; }
        public DateTime? ArrivalAtLandingSite { get; set; }
        public string ODKRowID { get; set; }

        public bool IsMultiGear { get; set; }

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
                    //_parent = NSAPEntities.GearUnloadViewModel.getGearUnload(GearUnloadID);
                    _parent = GearUnloadViewModel.GearUnloadFromID(GearUnloadID);
                    //_parent = new GearUnload();
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

        public string FormVersionCleaned
        {
            get
            {
                return FormVersion.Replace("Version", "").Trim();
            }
        }
        public double FormVersionNumeric
        {
            get
            {
                var ver = FormVersion.Replace("Version", "").Trim();
                int? first_number = null;
                if (double.TryParse(ver, out double v))
                {
                    return v;
                }
                else
                {
                    var arr = ver.Split('.');
                    string vers = "";
                    bool proceed = true;
                    for (int x = 0; x <= arr.Length; x++)
                    {
                        if (proceed && x < 2)
                        {
                            if (x == 0)
                            {
                                vers = arr[x];
                            }
                            else
                            {
                                vers += $".{arr[x]}";
                            }
                            if (double.TryParse(vers, out double vv))
                            {
                                proceed = true;
                                if (x == 0)
                                {
                                    first_number = (int)vv;
                                }
                            }
                            else
                            {
                                proceed = false;
                                break;
                            }
                        }
                        else if (x == 2)
                        {
                            break;
                        }
                    }
                    if (proceed)
                    {
                        return double.Parse(vers);
                    }
                    else if (first_number != null)
                    {

                        return (int)first_number;
                    }
                    else
                    {
                        return 0;
                    }

                }

            }
        }
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
