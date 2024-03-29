﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Wordprocessing;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities.Database
{

    public enum WeightValidationFlag
    {
        WeightValidationNotValidated,
        WeightValidationValid,
        WeightValidationInValid,
        WeightValidationNotApplicable
    }

    public enum SamplingTypeFlag
    {
        SamplingTypeNone,
        SamplingTypeMixed,
        SamplingTypeTotalEnumeration,
        SamplingTypeSampled
    }
    public class CatchCompGroupCatchCompositionRepeatLenWtRepeat
    {
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }


        public static void SetRowIDs()
        {
            //if (NSAPEntities.CatchLengthWeightViewModel.CatchLengthWeightCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.CatchLengthWeightViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLenWtPK;
            CatchLengthWeightViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }

        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat/len_wt_group/wt_lenwt")]
        public double Weight { get; set; }
        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat/len_wt_group/len_lenwt")]
        public double Length { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat/len_wt_group/sex_lw")]
        public string Sex { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeatLengthFreqRepeat
    {
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            //if (NSAPEntities.CatchLenFreqViewModel.CatchLenFreqCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.CatchLenFreqViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLenFreqPK;
            CatchLenFreqViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }

        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/length_class")]
        public double LengthClass { get; set; }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/freq")]
        public int Frequency { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat/group_LF/sex_lf")]
        public string Sex { get; set; }
    }

    public class CatchCompGroupCatchCompositionRepeatLengthListRepeat
    {
        private static int _pk;
        private int _rowID;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            //if (NSAPEntities.CatchLengthViewModel.CatchLengthCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.CatchLengthViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLengthsPK;
            if (_pk == 0)
            {

            }
            CatchLengthViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }
        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_list_repeat/length")]
        public double Length { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/length_list_repeat/sex_l")]
        public string Sex { get; set; }
    }
    public class CatchCompGroupCatchCompositionRepeatGmsRepeatGroup
    {
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            //if (NSAPEntities.CatchMaturityViewModel.CatchMaturityCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.CatchMaturityViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastMaturityPK;
            CatchMaturityViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }


            }
        }

        public CatchCompGroupCatchCompositionRepeat Parent { get; set; }
        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/individual_weight")]
        public double? Weight { get; set; }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gms_repeat")]
        public string GMSCode { get; set; }
        public string GMS
        {
            get
            {
                return GMSCode == "pr" ? "Premature" :
                       GMSCode == "im" ? "Immature" :
                       GMSCode == "de" ? "Developing" :
                       GMSCode == "ma" ? "Maturing" :
                       GMSCode == "dt" ? "Mature" :
                       GMSCode == "ri" ? "Ripening" :
                       GMSCode == "gr" ? "Gravid" :
                       GMSCode == "spw" ? "Spawning" :
                       GMSCode == "sp" ? "Spent" : "";
            }
        }

        public string MaturityStage { get { return GMS; } }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/individual_length")]
        public double? Length { get; set; }


        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gonad_wt")]
        public double? GonadWeight { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/sex")]
        public string SexCode { get; set; }

        public string Sex
        {
            get
            {
                return SexCode == "f" ? "Female" :
                       SexCode == "m" ? "Male" :
                       SexCode == "j" ? "Juvenile" : "";
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/gut_content_category")]
        public string GutContentCategoryCode { get; set; }
        public string GutContentCategory
        {
            get
            {
                return GutContentCategoryCode == "F" ? "Full" :
                       GutContentCategoryCode == "HF" ? "Half full" :
                       GutContentCategoryCode == "E" ? "Empty" : "";
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group/gms_group/stomach_content_wt")]
        public double? StomachContentWt { get; set; }

        public double? GutContentWeight { get { return StomachContentWt; } }

    }

    public class CatchCompGroupCatchCompositionRepeat
    {
        private static int _pk;
        private int _rowID;
        private string _speciesNameSelected;

        private List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> _lenWtRepeat;
        private List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> _lenFreqRepeat;
        private List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> _lengthListRepeat;
        private List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> _gmsRepeat;

        public VesselLanding Parent { get; set; }

        public int ParentID { get { return Parent.PK; } }


        //the position of the gear in the select list
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gear_species")]
        public string Gear_species { get; set; }


        //the name of the gear used in catching
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gear_species_name")]
        public string Gear_species_name { get; set; }


        //the code of the gear used in catching. Maybe null if the gear is typed-in and not selected from the list of gears
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gear_species_code")]
        public string Gear_species_code { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/gear_species_code_final")]
        public string Gear_species_code_final { get; set; }

        public List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> GetDuplicatedLenFreq()
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();

            if (_lenFreqRepeat != null && _lenFreqRepeat.Count > 1)
            {
                foreach (var item in _lenFreqRepeat
                    .GroupBy(t => t.LengthClass)
                    .Select
                        (
                            group => new
                            {
                                lenFreqObjects = group.ToList(),
                                lenClassCount = group.Count()
                            }
                        )
                    )
                {
                    if (item.lenClassCount > 1)
                    {
                        foreach (var lfItem in item.lenFreqObjects)
                        {
                            thisList.Add(lfItem);
                        }
                    }

                }
            }

            return thisList;
        }

        [JsonProperty("catch_comp_group/catch_composition_repeat/len_wt_repeat")]
        public List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> LenWtRepeat
        {
            get { return _lenWtRepeat; }
            set
            {
                _lenWtRepeat = value;
                foreach (var item in _lenWtRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_freq_repeat")]
        public List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> LenFreqRepeat
        {
            get { return _lenFreqRepeat; }
            set
            {
                _lenFreqRepeat = value;
                foreach (var item in _lenFreqRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/length_list_repeat")]
        public List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> LengthListRepeat
        {
            get { return _lengthListRepeat; }
            set
            {
                _lengthListRepeat = value;
                foreach (var item in _lengthListRepeat)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/gms_repeat_group")]
        public List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> GMSRepeat
        {

            get { return _gmsRepeat; }
            set
            {
                _gmsRepeat = value;
                foreach (var item in _gmsRepeat)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/taxa")]
        public string TaxaCode { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/price_of_species")]
        public double? PriceOfSpecies { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/pricing_unit")]
        public string PriceUnit { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/other_pricing_unit")]
        public string OtherPriceUnit { get; set; }


        public string PriceUnitText
        {
            get
            {
                if (OtherPriceUnit != null && OtherPriceUnit.Length > 0)
                {
                    return OtherPriceUnit;
                }
                else if (PriceUnit != null && PriceUnit.Length > 0)
                {
                    return PriceUnit;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Taxa Taxa { get { return NSAPEntities.TaxaViewModel.GetTaxa(TaxaCode); } }

        public string TaxaName { get { return Taxa.ToString(); } }


        //this is the combined name from fish species, invert species or other name
        //catch_comp_group/catch_composition_repeat/speciesname_group/species_data_group/species_name_selected

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_name_selected")]
        public string SpeciesNameSelected
        {
            get
            {
                if (_speciesNameSelected == null)
                {
                    return "";
                }
                else
                {
                    return _speciesNameSelected.Replace("0 <span style=\"color:blue\">(", "").Replace(")</span>", "");
                }
            }
            set { _speciesNameSelected = value; }
        }

        //species ID for both Fish and Not fish species
        public int? CatchCompSpeciesID
        {
            get
            {
                int? sp_id;
                switch (Taxa.Code)
                {
                    case "FIS":
                        sp_id = SpeciesID;
                        break;
                    case "NOID":
                        sp_id = null;
                        break;
                    default:
                        sp_id = SpeciesNotFish;
                        break;
                }
                return sp_id;
            }
        }

        //alias of SpeciesNameSelected
        public string SpeciesName { get { return SpeciesNameSelected; } }


        public string TaxaSpecies
        {
            get { return $"{TaxaCode}-{SpeciesName}"; }
        }

        //this is the invert species ID
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_notfish")]
        public int? SpeciesNotFish { get; set; }

        //this is the combined species id from fish or invert
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/sp_id")]
        public int? SpeciesID { get; set; }

        //this is fish species ID
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species")]
        public int? Species { get; set; }

        public bool FromTotalCatch
        {
            get
            {
                return From_total_catch == "yes";
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/from_total_catch")]
        public string From_total_catch { get; set; }
        //this is species name other
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/spName_other")]
        public string SpeciesNameOther { get; set; }

        //sampled weight of the species
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_sample_wt")]
        public double? SpeciesSampleWt { get; set; }

        //unit used when weighing individuals (kg or g)
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/individual_wt_unit")]
        public string IndividualWeightUnit { get; set; }

        //weight of the species
        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/species_wt")]
        public double? SpeciesWt { get; set; }

        [JsonProperty("catch_comp_group/catch_composition_repeat/speciesname_group/tws")]
        public double? TWS { get; set; }


        //alias of SpeciesWt
        public double? WeightOfCatch { get { return SpeciesWt; } }

        //alias of SpeciesSampleWt
        public double? SampleWeightOfCatch { get { return SpeciesSampleWt; } }
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {

            //if (NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.VesselCatchViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselCatchPK;
            VesselCatchViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return 0;
                    //var this_catch = NSAPEntities.VesselCatchViewModel.getVesselCatch(Parent, SpeciesID, SpeciesNameOther);
                    //if (this_catch != null)
                    //{
                    //    _rowID = NSAPEntities.VesselCatchViewModel.getVesselCatch(Parent, SpeciesID, SpeciesNameOther).PK;
                    //}
                    //else
                    //{
                    //    Utilities.Logger.Log($"IMPORTANT: Catch composition item not found in repository" +
                    //        $"\r\n Vessel unload PK is {Parent.PK}" +
                    //        $"\r\n Vessel name is {Parent.FishingVesselName}" +
                    //        $"\r\n Catch name is {SpeciesNameSelected}" +
                    //        $"\r\n Vessel unload is saved is {Parent.SavedInLocalDatabase}");

                    //    Utilities.VesselLandingFixDownload.VesselLandingToRepair(Parent);

                    //}
                }
                else
                {

                    if (RowIDSet && _rowID == 0)
                    {

                        _rowID = ++_pk;
                    }

                }
                return _rowID;

            }
        }
    }

    //public class ValidationStatus
    //{
    //}
    public class SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat
    {
        private static int _pk;
        private int _rowID;
        public VesselLanding Parent { get; set; }

        public int ParentID { get { return Parent.PK; } }

        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_set")]
        public string WaypointAtSet { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/wpt_haul")]
        public string WaypointAtHaul { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/haul_time")]
        public DateTime HaulTime { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat/set_time")]
        public DateTime SetTime { get; set; }

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            //if (NSAPEntities.GearSoakViewModel.GearSoakCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.GearSoakViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearSoaksPK;
            GearSoakViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }


    }
    public class GridCoordGroupBingoRepeat
    {
        private static int _pk;
        private VesselLanding _parent;
        private UTMZone _utmZone;
        private int _rowID;
        public VesselLanding Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (_parent.UTMZone.Length > 0)
                {
                    _utmZone = new UTMZone(_parent.UTMZone);
                }
            }
        }
        public int ParentID { get { return Parent.PK; } }


        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/col_name")]
        public string Column { get; set; }
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/bingo_complete")]
        public string CompleteGridName { get; set; }
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/major_grid")]
        public int MajorGrid { get; set; }
        [JsonProperty("grid_coord_group/bingo_repeat/bingo_group/row_name")]
        public int Row { get; set; }
        public string LongLat { get { return $"{Grid25Cell.Coordinate.Longitude.ToString("N6")}, {Grid25Cell.Coordinate.Latitude.ToString("N6")}"; } }

        public string UTMCoordinate { get { return Grid25Cell.UTMCoordinate.ToString(); } }
        public Grid25GridCell Grid25Cell
        {
            get
            {
                return new Grid25GridCell(_utmZone, CompleteGridName);
            }
        }
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {

            //if (NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastFishingGridsPK;
            FishingGroundGridViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
    }

    public class EffortGroupEffortRepeatMultiGear
    {

    }

    public class EffortSpecSingleGear
    {
        private static int _pk;
        private int _rowID;
        private VesselLanding _parent;
        public VesselLanding Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
            }
        }

        public int ParentID { get { return Parent.PK; } }
        [JsonProperty("efforts_group/effort_repeat/group_effort/selected_effort_measure")]
        public string SelectedEffortMeasure { get; set; }

        public string EffortSpecification { get { return SelectedEffortMeasure; } }
        [JsonProperty("efforts_group/effort_repeat/group_effort/response_type")]
        public string ResponseType { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_spec_name")]
        public string EffortSpecName { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_intensity")]
        public double? EffortIntensity { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_type")]
        public int EffortType { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_desc")]
        public string EffortDescription { get; set; }
        [JsonProperty("efforts_group/effort_repeat/group_effort/effort_bool")]
        public string EffortBoolYesNo { get; set; }

        public bool EffortBool { get { return EffortBoolYesNo == "yes"; } }

        public double? EffortNumericValue { get { return EffortIntensity; } }

        public string EffortTextValue { get { return EffortDescription; } }
        public bool? EffortBooleanValue
        {
            get
            {
                if (EffortBoolYesNo != null && EffortBoolYesNo.Length > 0)
                {
                    return EffortBool;
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {
            //if (NSAPEntities.VesselEffortViewModel.VesselEffortCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.VesselEffortViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselEffortsPK;
            VesselEffortViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }
    }
    public class VesselLandingFlattened
    {
        public VesselLandingFlattened(VesselLanding vl)
        {
            ID = vl.PK;
            Start = vl.start;
            Device_id = vl.device_id;
            User_name = vl.user_name;
            //Email = vl.em;
            Version = vl.intronote;
            SamplingDate = vl.SamplingDate;
            NSAPRegion = vl.NSAPRegion.Name;
            Enumerator = vl.EnumeratorName;
            FMA = vl.FMA == null ? "" : vl.FMA.ToString();
            FishingGround = vl.FishingGround.Name;
            LandingSite = vl.LandingSiteName;
            Gear = vl.GearName;
            Sector = vl.Sector;
            NumberOfFishers = vl.NumberOfFishers;
            IsBoatUsed = vl.IsBoatUsed;
            Vessel = vl.FishingVesselName;
            SuccessOperation = vl.TripIsSuccess;
            TripIsCompleted = vl.TripIsCompleted;
            CatchTotalWt = vl.CatchTotalWt;
            CatchSampleWt = vl.CatchSampleWt;
            BoxesTotal = vl.BoxesTotal;
            BoxesSampled = vl.BoxesSampled;
            RaisingFactor = vl.RaisingFactor;
            Remarks = vl.Remarks;
            IncludeTracking = vl.IncludeTracking;
            UTMZone = vl.UTMZone;
            DepartureLandingSite = vl.TimeDepartLandingSite;
            ArrivalLandingSite = vl.TimeArriveLandingSite;
            GPS = vl.GPS == null ? "" : vl.GPS.AssignedName;
            _version = vl.__version__;
            _metaID = vl.Meta_InstanceID;
            _id = vl._id;
            _uuid = vl._uuid;
            SubmissionTime = vl._submission_time;
            SavedToDatabase = vl.SavedInLocalDatabase;
        }
        public int ID { get; set; }
        public DateTime Start { get; set; }
        public string Device_id { get; set; }
        public string User_name { get; set; }
        public string Email { get; set; }
        public string Version { get; set; }
        public DateTime SamplingDate { get; set; }
        public string NSAPRegion { get; set; }
        public string Enumerator { get; set; }
        public string FMA { get; set; }
        public string FishingGround { get; set; }
        public string LandingSite { get; set; }

        public int? NumberOfFishers { get; set; }
        public string Gear { get; set; }
        public string Sector { get; set; }

        public bool IsBoatUsed { get; set; }
        public string Vessel { get; set; }
        public bool SuccessOperation { get; set; }
        public bool TripIsCompleted { get; set; }
        public double? CatchTotalWt { get; set; }
        public double? CatchSampleWt { get; set; }

        public int? BoxesTotal { get; set; }
        public int? BoxesSampled { get; set; }

        public double? RaisingFactor { get; set; }
        public string Remarks { get; set; }
        public bool IncludeTracking { get; set; }
        public string UTMZone { get; set; }
        public DateTime? DepartureLandingSite { get; set; }
        public DateTime? ArrivalLandingSite { get; set; }
        public string GPS { get; set; }
        public string _version { get; set; }
        public string _metaID { get; set; }
        public int _id { get; set; }

        public string _uuid { get; set; }

        public DateTime SubmissionTime { get; set; }
        public bool SavedToDatabase { get; set; }
    }

    public class VesselSamplingRepeatGear
    {
        private VesselLanding _parent;
        private static int _pk;
        private int _rowID;

        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {

            //if (NSAPEntities.VesselCatchViewModel.VesselCatchCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.VesselCatchViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadGearPK;
            VesselUnload_FishingGearViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {

                    if (RowIDSet && _rowID == 0)
                    {

                        _rowID = ++_pk;
                    }

                }
                return _rowID;

            }
        }
        public Gear Gear { get { return NSAPEntities.GearViewModel.GetGear(GearCode); } }
        public VesselLanding Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
            }
        }
        [JsonProperty("vessel_sampling/repeat_gears/group_gear/select_gear")]
        public string SelectGear { get; set; }

        [JsonProperty("vessel_sampling/repeat_gears/group_gear/gear_used")]
        public string GearUsed { get; set; }

        [JsonProperty("vessel_sampling/repeat_gears/group_gear/repeat_gear_code")]
        public string GearCode { get; set; }

        [JsonProperty("vessel_sampling/repeat_gears/group_gear/repeat_gear_name")]
        public string GearName { get; set; }


        [JsonProperty("vessel_sampling/repeat_gears/group_gear/gear_used_text")]
        public string GearUsedText { get; set; }


    }

    public class UnmatchedEnumeratorJSONFile
    {
        public string JSONFileName { get; set; }
        public int EnumeratorID { get; set; }
    }
    public class VesselLanding : IDisposable
    {
        private string _gps2;
        private static int _pk;
        private int _rowid;
        private bool? _isSaved;
        private string _includeTrackingYesNo;
        private string _tripIsSuccessYesNo;
        private string _tripIsCompletedYesNo;
        private bool _tripIsCompleted;
        private List<GridCoordGroupBingoRepeat> _gridCoordinates;
        private List<EffortSpecSingleGear> _effortSpecs;
        private List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> _gearSoakTimes;
        private List<CatchCompGroupCatchCompositionRepeat> _catchComps;
        private List<VesselSamplingRepeatGear> _samplingGears;
        private List<MultiGearEffortSpecContainer> _gearsEfforts;
        private string _includeCatchComposition;
        private static List<int> _unmatchedENumeratorIDs = new List<int>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _gridCoordinates?.Clear();
                _gridCoordinates = null;
                _effortSpecs?.Clear();
                _effortSpecs = null;
                _gearSoakTimes?.Clear();
                _gearSoakTimes = null;
                _catchComps?.Clear();
                _catchComps = null;
                _samplingGears?.Clear();
                _samplingGears = null;
                _gearsEfforts?.Clear();
                _gearsEfforts = null;
                _unmatchedENumeratorIDs?.Clear();
                _unmatchedENumeratorIDs = null;

            }
            // free native resources if there are any.
        }
        public bool IncludeEffort { get { return IncludeEffortYesNo == "yes"; } }
        [JsonProperty("efforts_group/include_effort")]
        public string IncludeEffortYesNo { get; set; }

        [JsonProperty("efforts_group/repeat_gear_effort_count")]
        public string efforts_grouprepeat_gear_effort_count { get; set; }




        public SamplingTypeFlag SamplingTypeFlag { get; internal set; }

        public WeightValidationFlag WeightValidationFlag { get; internal set; }
        public GearUnload GearUnload { get; set; }
        public List<CatchCompGroupCatchCompositionRepeat> GetDuplicatedCatchComposition()
        {
            var thisList = new List<CatchCompGroupCatchCompositionRepeat>();
            if (_catchComps != null && _catchComps.Count > 1)
            {
                foreach (var item in _catchComps
                    .GroupBy(t => t.SpeciesName)
                    .Select
                        (
                            group => new
                            {
                                catchCompObjects = group.ToList(),
                                catchCompCount = group.Count()
                            }
                        )
                    )
                {
                    if (item.catchCompCount > 1)
                    {
                        foreach (var catchItem in item.catchCompObjects)
                        {
                            thisList.Add(catchItem);
                        }
                    }

                }
            }
            return thisList;
        }

        public List<EffortSpecSingleGear> GetDuplicatedEffortSpecs()
        {
            var thisList = new List<EffortSpecSingleGear>();
            if (_effortSpecs != null && _effortSpecs.Count > 1)
            {
                foreach (var item in _effortSpecs
                    .GroupBy(t => t.EffortType)
                    .Select
                        (
                            group => new
                            {
                                effortObjects = group.ToList(),
                                effortSpecCount = group.Count()
                            }
                        )
                    )
                {
                    if (item.effortSpecCount > 1)
                    {
                        foreach (var effortItem in item.effortObjects)
                        {
                            thisList.Add(effortItem);
                        }
                    }

                }
            }
            return thisList;
        }
        public double SumOfCatchCompSampleWeight { get; internal set; }
        public double RaisingFactorComputed { get; set; }
        public double SumOfCatchCompWeight { get; internal set; }

        public double DifferenceInWeight { get; internal set; }


        public bool IsMultiGear { get { return GearTypeCount != null; } }
        [JsonProperty("vessel_sampling/repeat_gears_count")]
        public int? GearTypeCount { get; set; }


        //effort specs of gears for multi-gear eform
        [JsonProperty("efforts_group/repeat_gear_effort")]
        public List<MultiGearEffortSpecContainer> GearsEfforts
        {
            get { return _gearsEfforts; }
            set
            {
                _gearsEfforts = value;
                foreach (MultiGearEffortSpecContainer item in _gearsEfforts)
                {
                    item.Parent = this;
                }

            }
        }
        [JsonProperty("vessel_sampling/repeat_gears")]
        public List<VesselSamplingRepeatGear> SamplingGears
        {
            get { return _samplingGears; }
            set
            {
                _samplingGears = value;
                foreach (VesselSamplingRepeatGear item in _samplingGears)
                {
                    item.Parent = this;
                }
            }
        }
        [JsonProperty("catch_comp_group/catch_composition_repeat")]
        public List<CatchCompGroupCatchCompositionRepeat> CatchComposition
        {
            get { return _catchComps; }
            set
            {
                int countTotalEnum = 0;
                int countFromSample = 0;
                _catchComps = value;
                bool computeForRaisedValue = false;

                if (CatchTotalWt != null && CatchSampleWt != null && CatchTotalWt > 0 && CatchSampleWt > 0)
                {
                    computeForRaisedValue = true;
                    double from_total_sum = 0;

                    if (FormVersion >= 6.43)
                    {
                        foreach (CatchCompGroupCatchCompositionRepeat item in _catchComps)
                        {
                            item.Parent = this;
                            if (item.FromTotalCatch)
                            {
                                from_total_sum += (double)item.SpeciesWt;
                                countTotalEnum++;
                            }
                            else if (item.SpeciesSampleWt != null)
                            {
                                SumOfCatchCompSampleWeight += (double)item.SpeciesSampleWt;
                                countFromSample++;
                            }
                        }
                    }
                    else
                    {
                        foreach (CatchCompGroupCatchCompositionRepeat item in _catchComps)
                        {
                            item.Parent = this;
                            if (item.SpeciesSampleWt == null)
                            {
                                if (item.SpeciesWt == null)
                                {
                                    from_total_sum += 0;
                                }
                                else
                                {
                                    from_total_sum += (double)item.SpeciesWt;
                                }
                                countTotalEnum++;
                            }
                            else
                            {
                                SumOfCatchCompSampleWeight += (double)item.SpeciesSampleWt;
                                countFromSample++;
                            }
                        }
                    }
                    RaisingFactorComputed = ((double)CatchTotalWt - from_total_sum) / (double)CatchSampleWt;
                }

                else
                {
                    foreach (CatchCompGroupCatchCompositionRepeat item in _catchComps)
                    {
                        item.Parent = this;
                        if (item.SpeciesWt != null)
                        {
                            SumOfCatchCompWeight += (double)item.SpeciesWt;
                        }

                        if (item.SpeciesSampleWt != null)
                        {
                            countFromSample++;
                        }
                        else if (item.WeightOfCatch != null && item.SpeciesSampleWt == null)
                        {
                            countTotalEnum++;
                        }
                    }


                }

                if (computeForRaisedValue)
                {
                    foreach (CatchCompGroupCatchCompositionRepeat item in _catchComps)
                    {
                        if (item.WeightOfCatch != null && (item.FromTotalCatch || item.SpeciesSampleWt == null))
                        {
                            SumOfCatchCompWeight += (double)item.WeightOfCatch;
                        }
                        else
                        {
                            if (item.SpeciesSampleWt == null)
                            {
                                SumOfCatchCompWeight += 0;
                            }
                            else
                            {
                                if (item.SpeciesSampleWt == null)
                                {
                                    SumOfCatchCompWeight += 0;
                                }
                                else
                                {
                                    SumOfCatchCompWeight += ((double)item.SpeciesSampleWt * RaisingFactorComputed);
                                }
                            }
                        }
                    }

                }

                if (CatchTotalWt != null && SumOfCatchCompWeight > 0)
                {
                    DifferenceInWeight = Math.Abs((double)CatchTotalWt - (double)SumOfCatchCompWeight) / (double)CatchTotalWt * 100;
                    if (DifferenceInWeight <= (int)Utilities.Global.Settings.AcceptableWeightsDifferencePercent)
                    {
                        WeightValidationFlag = WeightValidationFlag.WeightValidationValid;
                    }
                    else
                    {
                        WeightValidationFlag = WeightValidationFlag.WeightValidationInValid;
                    }
                }



                if (FormVersion < 6.43)
                {
                    if (CatchTotalWt != null && CatchTotalWt > 0 && CatchSampleWt == null)
                    {
                        SamplingTypeFlag = SamplingTypeFlag.SamplingTypeTotalEnumeration;
                    }
                }
                else if (countFromSample > 0 && countTotalEnum > 0)
                {
                    SamplingTypeFlag = SamplingTypeFlag.SamplingTypeMixed;
                }
                else if (countFromSample > 0)
                {
                    SamplingTypeFlag = SamplingTypeFlag.SamplingTypeSampled;
                }
                else if (countTotalEnum > 0)
                {
                    SamplingTypeFlag = SamplingTypeFlag.SamplingTypeTotalEnumeration;
                }

            }
        }
        [JsonProperty("grid_coord_group/bingo_repeat")]
        public List<GridCoordGroupBingoRepeat> GridCoordinates
        {
            get { return _gridCoordinates; }
            set
            {
                _gridCoordinates = value;
                foreach (GridCoordGroupBingoRepeat item in _gridCoordinates)
                {
                    item.Parent = this;
                }
            }
        }

        //effort specification of gear for non-multigear eform
        [JsonProperty("efforts_group/effort_repeat")]
        public List<EffortSpecSingleGear> SingleGearEffortSpecs
        {
            get { return _effortSpecs; }
            set
            {
                _effortSpecs = value;
                foreach (EffortSpecSingleGear effort in _effortSpecs)
                {
                    effort.Parent = this;
                }
            }
        }
        [JsonProperty("soak_time_group/soaktime_tracking_group/soak_time_repeat")]
        public List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> GearSoakTimes
        {
            get { return _gearSoakTimes; }
            set
            {
                _gearSoakTimes = value;
                foreach (SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat item in _gearSoakTimes)
                {
                    item.Parent = this;
                }
            }
        }

        [JsonProperty("vessel_sampling/gps2")]
        public string GPS2
        {
            get { return _gps2; }
            set
            {
                _gps2 = value;
                if (_gps2.Length > 0)
                {
                    IncludeTracking = true;
                }
            }
        }

        [JsonProperty("vessel_sampling/time_depart_landingsite")]
        public DateTime? DateTimeDepartLandingSite { get; set; }

        [JsonProperty("vessel_sampling/time_arrive_landingsite")]
        public DateTime? DateTimeArriveLandingSite { get; set; }

        [JsonProperty("vessel_sampling/sampling_date")]
        public DateTime SamplingDate { get; set; }
        [JsonProperty("vessel_sampling/nsap_region")]
        public string NSAPRegionCode { get; set; }

        public NSAPRegion NSAPRegion
        {
            get { return NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(NSAPRegionCode); }
            set { NSAPRegionCode = value.Code; }
        }
        [JsonProperty("vessel_sampling/region_enumerator")]
        public int? RegionEnumeratorID { get; set; }
        [JsonProperty("vessel_sampling/region_enumerator_text")]
        public string EnumeratorText { get; set; }

        public NSAPEnumerator NSAPEnumerator
        {
            get
            {
                if (RegionEnumeratorID != null)
                {
                    var regionEnumerator = (NSAPEntities.NSAPEnumeratorViewModel.GetNSAPEnumerator((int)RegionEnumeratorID));
                    if (regionEnumerator != null && NSAPRegion.NSAPEnumerators.Count > 0 && RegionEnumeratorID != null)
                    {
                        var enumerator = NSAPRegion.NSAPEnumerators.FirstOrDefault(t => t.RowID == (int)RegionEnumeratorID)?.Enumerator;
                        return enumerator;
                    }
                    else
                    {
                        if (regionEnumerator == null)
                        {
                            //NSAP_ODK.Utilities.Logger.Log($"NSAP enumerator with ID {RegionEnumeratorID} was not found");

                            if (VesselUnloadServerRepository.UnmatchedEnumeratorIDs.FirstOrDefault(t => t.EnumeratorID == (int)RegionEnumeratorID) == null)
                            {
                                VesselUnloadServerRepository.UnmatchedEnumeratorIDs.Add(new UnmatchedEnumeratorJSONFile
                                {
                                    EnumeratorID = (int)RegionEnumeratorID,
                                    JSONFileName = VesselUnloadServerRepository.CurrentJSONFileName
                                });
                            }
                        }
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        public string EnumeratorName
        {
            get
            {
                if (RegionEnumeratorID != null)
                {
                    if (NSAPEnumerator != null)
                    {
                        return NSAPEnumerator.Name;
                    }
                    else
                    {
                        return "NOT RECOGNIZED";
                    }
                }
                else
                {
                    return EnumeratorText;
                }
            }
        }
        [JsonProperty("vessel_sampling/fma_in_region")]
        public int FMAInRegionID { get; set; }

        public FMA FMA
        {
            get { return NSAPEntities.FMAViewModel.GetFMA(fma_number); }
        }

        public NSAPRegionFMA NSAPRegionFMA
        {
            get
            {
                return NSAPRegion.FMAs.Where(t => t.RowID == FMAInRegionID).FirstOrDefault();
            }
        }
        [JsonProperty("vessel_sampling/fishing_ground")]
        public int RegionFishingGroundID { get; set; }
        public FishingGround FishingGround
        {
            get
            {
                return RegionFishingGround?.FishingGround;
            }
        }

        public NSAPRegionFMAFishingGround RegionFishingGround
        {
            get
            {
                return NSAPRegionFMA.FishingGrounds.FirstOrDefault(t => t.RowID == RegionFishingGroundID);
            }
        }

        [JsonProperty("vessel_sampling/landing_site")]
        public int? LandingSiteID { get; set; }
        [JsonProperty("vessel_sampling/landing_site_text")]
        public string LandingSiteText { get; set; }

        public LandingSite LandingSite
        {
            get
            {
                if (LandingSiteID != null)
                {
                    var ls = RegionFishingGround?.LandingSites.FirstOrDefault(t => t.RowID == (int)LandingSiteID)?.LandingSite;
                    if (ls != null)
                    {
                        return ls;
                    }
                    else
                    {
                        return null;
                    }
                    //return RegionFishingGround.LandingSites.FirstOrDefault(t => t.RowID == (int)LandingSiteID).LandingSite;
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonProperty("landing_site_name")]
        public string LandingSiteName2 { get; set; }

        public string LandingSiteName
        {
            get
            {
                if (LandingSiteID == null)
                {
                    return LandingSiteText;
                }
                else
                {
                    if (LandingSite == null)
                    {
                        return "NOT RECOGNIZED";
                    }
                    else
                    {
                        return LandingSite?.ToString();
                    }
                }
            }
        }
        [JsonProperty("vessel_sampling/gear_used")]
        public int? GearUsed { get; set; }

        public Gear Gear { get { return NSAPEntities.GearViewModel.GetGear(GearCode); } }
        [JsonProperty("vessel_sampling/gear_used_text")]
        public string GearUsedText { get; set; }


        public string GearNameToUse
        {
            get
            {
                if (Gear != null)
                {
                    return Gear.GearName;
                }
                else
                {
                    if (GearUsedText != null && GearUsedText.Length > 0)
                    {
                        return GearUsedText;
                    }
                    else
                    {
                        return "NOT RECOGNIZED";
                    }
                }
            }
        }
        public string length_type { get; set; }
        [JsonProperty("vessel_catch/trip_isSuccess")]
        public string TripIsSuccessYesNo
        {
            get { return _tripIsSuccessYesNo; }
            set
            {
                _tripIsSuccessYesNo = value;
                TripIsSuccess = _tripIsSuccessYesNo == "yes";
            }
        }

        [JsonProperty("vessel_catch/trip_isCompleted")]
        public string TripIsCompletedYesNo
        {
            get { return _tripIsCompletedYesNo; }
            set
            {
                _tripIsCompletedYesNo = value;
                _tripIsCompleted = _tripIsCompletedYesNo == "yes" || _tripIsCompletedYesNo == "";

            }

        }
        public bool TripIsCompleted
        {
            get
            {
                if (_tripIsCompletedYesNo == null)
                {
                    _tripIsCompleted = true;
                }
                return _tripIsCompleted;
            }
            set
            {
                _tripIsCompleted = value;
            }
        }
        public bool TripIsSuccess { get; set; }

        [JsonProperty("vessel_catch/catch_total")]
        public double? CatchTotalWt { get; set; }
        [JsonProperty("vessel_catch/catch_sampled")]
        public double? CatchSampleWt { get; set; }
        [JsonProperty("vessel_catch/remarks")]
        public string Remarks { get; set; }
        [JsonProperty("vessel_catch/boxes_sampled")]
        public int? BoxesSampled { get; set; }
        [JsonProperty("vessel_catch/boxes_total")]
        public int? BoxesTotal { get; set; }
        [JsonProperty("catch_comp_group/raising_factor")]
        public double? RaisingFactor { get; set; }



        [JsonProperty("soak_time_group/include_tracking")]
        public string IncludeTrackingYesNo
        {
            get { return _includeTrackingYesNo; }
            set
            {
                _includeTrackingYesNo = value;
                IncludeTracking = _includeTrackingYesNo == "yes";
            }
        }

        public bool IncludeTracking { get; set; }

        [JsonProperty("soak_time_group/soaktime_tracking_group/gps")]
        public string GPSCode { get; set; }
        public GPS GPS
        {
            get
            {
                if (GPS2 != null)
                {
                    if (GPS2 == "gps")
                    {
                        if (!NSAPEntities.GPSViewModel.Exists(GPS2))
                        {
                            GPS gps = new GPS
                            {
                                Code = "gps",
                                AssignedName = "Generic GPS",
                                DeviceType = DeviceType.DeviceTypeGPS
                            };


                            NSAPEntities.GPSViewModel.AddGenericGPS(gps);
                        }
                    }


                    return NSAPEntities.GPSViewModel.GetGPS(GPS2);

                }
                else
                {
                    return NSAPEntities.GPSViewModel.GetGPS(GPSCode);
                }
            }
        }
        [JsonProperty("soak_time_group/soaktime_tracking_group/time_depart_landingsite")]
        public DateTime? TimeDepartLandingSite { get; set; }
        [JsonProperty("soak_time_group/soaktime_tracking_group/time_arrive_landingsite")]
        public DateTime? TimeArriveLandingSite { get; set; }





        [JsonProperty("fishing_vessel_group/fish_sector")]
        public string SectorCode { get; set; }

        [JsonProperty("fishing_vessel_group/number_of_fishers")]
        public int? NumberOfFishers { get; set; }

        public string Sector
        {
            get
            {
                return SectorCode == "m" ? "Municipal" :
                       SectorCode == "c" ? "Commercial" : "";
            }
        }
        [JsonProperty("fishing_vessel_group/boat_name")]
        public string BoatName { get; set; }
        [JsonProperty("fishing_vessel_group/boat_used")]
        public int? BoatUsed { get; set; }
        [JsonProperty("fishing_vessel_group/boat_used_text")]
        public string BoatUsedText { get; set; }
        [JsonProperty("fishing_vessel_group/gear_code")]
        public string GearCode { get; set; }
        [JsonProperty("fishing_vessel_group/gear_name")]
        public string GearName { get; set; }
        [JsonProperty("fishing_vessel_group/is_boat_used")]
        public string IsBoatUsedYesNo { get; set; }

        public bool IncludeCatchComp { get { return IncludeCatchComposition == "yes"; } }
        [JsonProperty("catch_comp_group/include_catchcomp")]
        public string IncludeCatchComposition
        {
            get { return _includeCatchComposition; }
            set
            {
                _includeCatchComposition = value;
            }
        }

        [JsonProperty("vessel_catch/is_catch_sold")]
        public string IsCatchSoldYesNo { get; set; }


        public bool IsCatchSold
        {
            get
            {
                if (IsCatchSoldYesNo == "yes")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsBoatUsed
        {
            get
            {
                return IsBoatUsedYesNo == null ? BoatUsed != null || (BoatUsedText != null && BoatUsedText.Length > 0) : IsBoatUsedYesNo == "yes";
            }
            set
            {
                IsBoatUsedYesNo = value ? "yes" : "no";
            }
        }


        public string FishingVesselName
        {
            get
            {
                if (BoatUsed == null)
                {
                    return BoatUsedText;
                }
                else
                {
                    var boatUsed = NSAPEntities.FishingVesselViewModel.GetFishingVessel((int)BoatUsed);
                    if (boatUsed == null)
                    {
                        return "";
                    }
                    else
                    {

                        return boatUsed.ToString();
                    }
                }
            }
        }




        [JsonProperty("grid_coord_group/utmZone")]
        public string UTMZone { get; set; }





        public ValidationStatus _validation_status { get; set; }
        public string __version__ { get; set; }
        public List<object> _attachments { get; set; }
        public string user_name { get; set; }
        public List<object> _geolocation { get; set; }
        public string _submitted_by { get; set; }
        public string _uuid { get; set; }

        public DateTime _submission_time { get; set; }
        [JsonProperty("formhub/uuid")]
        public string formhub_uuid { get; set; }
        public int _id { get; set; }
        [JsonProperty("catch_comp_group/group_final_tally/ref_no")]
        public string ref_no { get; set; }
        public string device_id { get; set; }
        public List<object> _notes { get; set; }
        public DateTime today { get; set; }
        public string intronote { get; set; }
        public double FormVersion
        {
            get
            {
                if (double.TryParse(Form_version, out double v))
                {
                    return double.Parse(Form_version);
                }
                else
                {
                    var s = Form_version.Split('.');
                    if (s.Length > 0)
                    {
                        string ss = "";
                        int count = 0;
                        foreach (var i in s)
                        {
                            if (int.TryParse(i, out int ii))
                            {
                                if (count == 0)
                                {
                                    ss = $"{ii.ToString()}.";
                                }
                                else
                                {
                                    ss += ii.ToString();
                                }
                            }
                            count++;
                        }
                        if (ss.Length > 0)
                        {
                            return double.Parse(ss);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }
        public string Form_version
        {
            get
            {
                if (intronote == null)
                {
                    return "6";
                }
                else
                {
                    return intronote.Replace("Version ", "");
                }
            }
        }
        public string _bamboo_dataset_id { get; set; }
        public List<object> _tags { get; set; }
        public string _xform_id_string { get; set; }
        [JsonProperty("meta/instanceID")]
        public string Meta_InstanceID { get; set; }

        [JsonProperty("fishing_ground_name")]
        public string FishingGroundName { get; set; }
        public int fma_number { get; set; }
        public DateTime start { get; set; }
        public string _status { get; set; }


        public static bool RowIDSet { get; private set; }


        public static void SetRowIDs()
        {
            //if (NSAPEntities.SummaryItemViewModel.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    NSAPEntities.SummaryItemViewModel.RefreshLastPrimaryLeys(VesselUnloadServerRepository.DelayedSave);
            //    _pk = NSAPEntities.SummaryItemViewModel.GetNextRecordNumber() - 1;
            //    _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadPK;
            //    VesselUnloadViewModel.CurrentIDNumber = _pk;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadPK;
            VesselUnloadViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        //private VesselUnload _savedVesselUnloadObject;
        private SummaryItem _savedVesselUnloadObject;

        public int PK
        {
            get
            {
                if (!SavedInLocalDatabase)
                {
                    if (_rowid == 0)
                    {
                        _rowid = ++_pk;
                    }
                }
                else
                {
                    if (_savedVesselUnloadObject == null)
                    {
                        _savedVesselUnloadObject = SavedVesselUnloadObject;
                    }

                    if (Debugger.IsAttached)
                    {
                        try
                        {
                            _rowid = (int)_savedVesselUnloadObject.VesselUnloadID;
                        }
                        catch
                        {
                            _rowid = (int)SavedVesselUnloadObject.VesselUnloadID;
                            _savedVesselUnloadObject = SavedVesselUnloadObject;
                            //_savedVesselUnloadObject = SavedVesselUnloadObject;
                            //_rowid = _savedVesselUnloadObject.VesselUnloadID;
                        }
                    }
                    else
                    {
                        _rowid = (int)_savedVesselUnloadObject.VesselUnloadID;
                    }
                }
                return _rowid;
            }
        }

        private readonly object collectionLock = new object();


        private SummaryItem SavedVesselUnloadObject
        {
            get
            {
                lock (collectionLock)
                {
                    try
                    {

                        //_savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.SummaryItemCollection.FirstOrDefault(t => t.ODKRowID == _uuid);
                        _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItem(_uuid);
                    }
                    catch (Exception ex)
                    {
                        if (Debugger.IsAttached)
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject SummaryItemCollection was modified while in debug mode when getting SavedVesselUnloadObject.");
                            try
                            {
                                _savedVesselUnloadObject = NSAPEntities.SummaryItemViewModel.GetItem(_uuid);
                            }
                            catch
                            {
                                _savedVesselUnloadObject = null;
                            }
                        }
                        else
                        {
                            Utilities.Logger.Log("VesselUnloadServerRepository.SavedVesselUnloadObject Error when getting savedVeselUnloadObject when in release mode", ex);
                        }
                    }
                }
                return _savedVesselUnloadObject;
            }

        }
        public bool SavedInLocalDatabase
        {
            get
            {
                if (_isSaved == null)
                {
                    _isSaved = SavedVesselUnloadObject != null;
                }
                return (bool)_isSaved;
            }
            set { _isSaved = value; }
        }
    }


    /// <summary>
    /// vessel unload gear specs
    /// </summary>
    public class MultiGearEffortSpecContainer
    {
        private static int _pk;
        private VesselLanding _parent;
        private int _rowID;
        private List<MultiGearEffortSpec> _effortsSpecsOfGearinMultiGear;



        // effort specs of gears for each gear in the sampled landing
        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat")]
        public List<MultiGearEffortSpec> EffortsSpecsOfGear
        {
            get { return _effortsSpecsOfGearinMultiGear; }
            set
            {
                _effortsSpecsOfGearinMultiGear = value;
                foreach (MultiGearEffortSpec item in _effortsSpecsOfGearinMultiGear)
                {
                    item.Parent = this;
                }
            }
        }
        public VesselLanding Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
            }
        }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_group_inside1/selected_gear_name")]
        public string SelectedGearName { get; set; }


        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_group_inside1/selected_gear_code")]
        public string SelectedGearCode { get; set; }

    }


    /// <summary>
    /// vessel unload gear specs inside group
    /// </summary>
    public class MultiGearEffortSpec
    {
        private static int _pk;
        private int _rowID;
        //private MultiGearEffortSpecContainer _parent;
        public static bool RowIDSet { get; private set; }

        public static void ResetIDState()
        {
            RowIDSet = false;
        }
        public static void SetRowIDs()
        {

            //if (NSAPEntities.FishingGroundGridViewModel.FishingGroundGridCollection.Count == 0)
            //{
            //    _pk = 0;
            //}
            //else
            //{
            //    _pk = NSAPEntities.FishingGroundGridViewModel.NextRecordNumber - 1;
            //}
            _pk = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselEffortsPK;
            VesselEffortViewModel.CurrentIDNumber = _pk;
            RowIDSet = true;
        }

        public int? PK
        {
            get
            {
                if (Parent.Parent.SavedInLocalDatabase)
                {
                    return null;
                }
                else
                {
                    if (_rowID == 0)
                    {
                        _rowID = ++_pk;
                    }
                    return _rowID;
                }

            }
        }

        public MultiGearEffortSpecContainer Parent { get; set; }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_type")]
        public string EffortTypePadded { get; set; }
        public int EffortType { get { return int.Parse(EffortTypePadded); } }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_spec_name")]
        public string EffortSpecName { get; set; }
        public double? EffortNumericValue { get { return EffortIntensity; } }
        public bool EffortBool { get { return EffortBoolYesNo == "yes"; } }
        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_bool")]
        public string EffortBoolYesNo { get; set; }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_desc")]
        public string EffortDescription { get; set; }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_intensity")]
        public double? EffortIntensity { get; set; }

        [JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/selected_effort_measure")]
        public string SelectedEffortMeasure { get; set; }

        //[JsonProperty("efforts_group/repeat_gear_effort/effort_group_inside/effort_repeat/group_effort/effort_desc")]
        //public string efforts_grouprepeat_gear_efforteffort_group_insideeffort_repeatgroup_efforteffort_desc { get; set; }
    }
    public static class VesselUnloadServerRepository
    {
        private static List<GridCoordGroupBingoRepeat> _listGridBingoCoordinates;
        private static List<EffortSpecSingleGear> _listGearEfforts;
        private static List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> _listGearSoakTimes;
        private static List<CatchCompGroupCatchCompositionRepeat> _listCatchComps;
        private static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> _listLenFreqs;
        private static List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> _listGMS;
        private static List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> _listLenWts;
        private static List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> _listLengths;
        private static List<UnrecognizedFishingGround> _unrecognizedFishingGrounds;

        public static bool DelayedSave { get; set; }
        public static void ClearUnrecgnizedFishingGroundsList()
        {
            if (_unrecognizedFishingGrounds != null && _unrecognizedFishingGrounds.Count > 0)
            {
                _unrecognizedFishingGrounds.Clear();
            }
        }

        public static HashSet<UnmatchedEnumeratorJSONFile> UnmatchedEnumeratorIDs { get; set; } = new HashSet<UnmatchedEnumeratorJSONFile>();
        public static VesselUnload LastVesselUnload { get; private set; }
        public static string JSON { get; set; }

        public static void CreateLandingsFromJSON()
        {
            //How are PKs assigned to each landings contained in each incoming batch of JSON?
            //call VesselLanding.SetRowIDs()

            //VesselLanding.SetRowIDs();
            try
            {
                VesselLandings = JsonConvert.DeserializeObject<List<VesselLanding>>(JSON);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
        }
        public static DateTime DownloadedLandingsEarliestLandingDate()
        {
            
            return VesselLandings.OrderBy(t => t.SamplingDate).FirstOrDefault().SamplingDate;
        }
        public static List<string> GetLandingIdentifiers()
        {
            List<string> ids = new List<string>();
            foreach (var item in VesselLandings)
            {
                ids.Add(item._uuid);
            }
            return ids;
        }
        public static DateTime DownloadedLandingsLatestLandingDate()
        {
            return VesselLandings.OrderByDescending(t => t.SamplingDate).FirstOrDefault().SamplingDate;
        }

        public static int DownloadedLandingsCount()
        {
            return VesselLandings.Count;
        }

        public static List<MultiVesselGear_SampledLanding> MultiVesselLandings { get; internal set; }
        public static List<VesselLanding> VesselLandings { get; internal set; }

        public static event EventHandler<UploadToDbEventArg> UploadSubmissionToDB;
        public static event Action<int> LandingWithUpdatedLandingSite;

        /// <summary>
        /// Reset ID is set boolean to false for repeating groups in a vessel landing json
        /// </summary>
        public static void ResetGroupIDState()
        {
            MultiVesselGear_SoakTime.ResetIDState();
            GridCoordGroupBingoRepeat.ResetIDState();
            EffortSpecSingleGear.ResetIDState();
            MultiGearEffortSpec.ResetIDState();
            VesselSamplingRepeatGear.ResetIDState();
            CatchCompGroupCatchCompositionRepeat.ResetIDState();
            CatchCompGroupCatchCompositionRepeatLenWtRepeat.ResetIDState();
            CatchCompGroupCatchCompositionRepeatLengthListRepeat.ResetIDState();
            CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.ResetIDState();
            CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.ResetIDState();
        }
        public static void ResetGroupIDs(bool delayedSave = false)
        {
            NSAPEntities.SummaryItemViewModel.RefreshLastPrimaryLeys(delayedSave);
            LandingSiteSamplingViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastLandingSiteSamplingPK;
            GearUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearUnloadPK;
            VesselLanding.SetRowIDs();
            SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
            GridCoordGroupBingoRepeat.SetRowIDs();
            EffortSpecSingleGear.SetRowIDs();
            MultiGearEffortSpec.SetRowIDs();
            VesselSamplingRepeatGear.SetRowIDs();
            CatchCompGroupCatchCompositionRepeat.SetRowIDs();
            CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
            CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
            CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
            CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
        }

        public static void ResetUnmatchedEnumeratorIDList()
        {
            if (UnmatchedEnumeratorIDs.Count > 0)
            {
                foreach (var item in UnmatchedEnumeratorIDs.OrderBy(t => t.EnumeratorID))
                {
                    Utilities.Logger.Log($"Enumerator ID not in database: {item.EnumeratorID} JSON file:{item.JSONFileName}", addNewLine: false);
                }
                UnmatchedEnumeratorIDs.Clear();
            }
        }
        public static void ResetLists(bool includeJSON = false)
        {
            _listGridBingoCoordinates = null;
            _listGearEfforts = null;
            _listGearSoakTimes = null;
            _listCatchComps = null;
            _listLenFreqs = null;
            _listGMS = null;
            _listLenWts = null;
            _listLengths = null;

            DuplicatedLenFreq = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
            DuplicatedCatchComposition = new List<CatchCompGroupCatchCompositionRepeat>();
            DuplicatedEffortSpec = new List<EffortSpecSingleGear>();

            if (includeJSON)
            {
                JSON = "";
                VesselLandings = null;
            }

        }

        public static void FillDuplicatedLists()
        {
            foreach (var item in VesselLandings)
            {
                if (item.CatchComposition != null && item.CatchComposition.Count > 1)
                {
                    var ccd = item.GetDuplicatedCatchComposition();
                    if (ccd.Count > 1)
                    {
                        DuplicatedCatchComposition.AddRange(ccd);
                    }

                    foreach (var catchItem in item.CatchComposition)
                    {
                        var duplicatedLFItems = catchItem.GetDuplicatedLenFreq();
                        if (duplicatedLFItems.Count > 1)
                        {
                            DuplicatedLenFreq.AddRange(duplicatedLFItems);
                        }
                    }
                }

                if (item.SingleGearEffortSpecs != null && item.SingleGearEffortSpecs.Count > 1)
                {
                    var esd = item.GetDuplicatedEffortSpecs();
                    if (esd.Count > 1)
                    {
                        DuplicatedEffortSpec.AddRange(esd);
                    }
                }


            }
        }

        public static List<CatchCompGroupCatchCompositionRepeat> DuplicatedCatchComposition { get; private set; }
        public static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> DuplicatedLenFreq { get; private set; }
        public static List<EffortSpecSingleGear> DuplicatedEffortSpec { get; private set; }


        public static List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> GetLengthList()
        {
            List<CatchCompGroupCatchCompositionRepeatLengthListRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLengthListRepeat>();
            if (_listLengths == null)
            {
                CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.LengthListRepeat != null)
                            {
                                foreach (var cc_len in cc.LengthListRepeat)
                                {
                                    thisList.Add(cc_len);
                                }
                            }
                        }
                    }
                }
                _listLengths = thisList;
            }
            return _listLengths;
        }
        public static List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> GetLenWtList()
        {
            List<CatchCompGroupCatchCompositionRepeatLenWtRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLenWtRepeat>();
            if (_listLenWts == null)
            {
                CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.LenWtRepeat != null)
                            {
                                foreach (var cc_lw in cc.LenWtRepeat)
                                {
                                    thisList.Add(cc_lw);
                                }
                            }
                        }
                    }
                }
                _listLenWts = thisList;
            }
            return _listLenWts;
        }
        public static List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> GetLenFreqList()
        {
            List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeatLengthFreqRepeat>();
            if (_listLenFreqs == null)
            {
                CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.LenFreqRepeat != null)
                            {
                                foreach (var cc_lf in cc.LenFreqRepeat)
                                {
                                    thisList.Add(cc_lf);
                                }
                            }
                        }
                    }
                }
                _listLenFreqs = thisList;
            }
            return _listLenFreqs;
        }
        public static List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> GetGMSList()
        {
            List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup> thisList = new List<CatchCompGroupCatchCompositionRepeatGmsRepeatGroup>();
            if (_listGMS == null)
            {
                CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
                foreach (var landing in VesselLandings)
                {
                    if (landing.CatchComposition != null)
                    {
                        foreach (var cc in landing.CatchComposition)
                        {
                            if (cc.GMSRepeat != null)
                            {
                                foreach (var cc_gms in cc.GMSRepeat)
                                {
                                    thisList.Add(cc_gms);
                                }
                            }
                        }
                    }
                }
                _listGMS = thisList;
            }
            return _listGMS;
        }
        public static List<GridCoordGroupBingoRepeat> GetGridBingoCoordinates()
        {
            List<GridCoordGroupBingoRepeat> thisList = new List<GridCoordGroupBingoRepeat>();
            if (_listGridBingoCoordinates == null)
            {
                if (!GridCoordGroupBingoRepeat.RowIDSet)
                {
                    GridCoordGroupBingoRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.GridCoordinates != null)
                    {
                        foreach (var bingoCoord in item.GridCoordinates)
                        {
                            thisList.Add(bingoCoord);
                        }
                    }
                }
                _listGridBingoCoordinates = thisList;
            }
            return _listGridBingoCoordinates;

        }

        public static List<EffortSpecSingleGear> GetGearEfforts()
        {
            List<EffortSpecSingleGear> thisList = new List<EffortSpecSingleGear>();
            if (_listGearEfforts == null)
            {
                if (!EffortSpecSingleGear.RowIDSet)
                {
                    EffortSpecSingleGear.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.SingleGearEffortSpecs != null)
                    {
                        foreach (var effort in item.SingleGearEffortSpecs)
                        {
                            thisList.Add(effort);
                        }
                    }
                }
                _listGearEfforts = thisList;
            }

            return _listGearEfforts;

        }

        public static List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> GetGearSoakTimes()
        {
            List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat> thisList = new List<SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat>();
            if (_listGearSoakTimes == null)
            {
                if (!SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.RowIDSet)
                {
                    SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.GearSoakTimes != null)
                    {
                        foreach (var soakTime in item.GearSoakTimes)
                        {
                            thisList.Add(soakTime);
                        }
                    }
                }
                _listGearSoakTimes = thisList;
            }

            return _listGearSoakTimes;

        }
        public static List<CatchCompGroupCatchCompositionRepeat> GetCatchCompositions()
        {
            List<CatchCompGroupCatchCompositionRepeat> thisList = new List<CatchCompGroupCatchCompositionRepeat>();
            if (_listCatchComps == null)
            {
                if (!CatchCompGroupCatchCompositionRepeat.RowIDSet)
                {
                    CatchCompGroupCatchCompositionRepeat.SetRowIDs();
                }
                foreach (var item in VesselLandings)
                {
                    if (item.CatchComposition != null)
                    {
                        foreach (var catchComp in item.CatchComposition)
                        {
                            thisList.Add(catchComp);
                        }
                    }
                }
                _listCatchComps = thisList;
            }

            return _listCatchComps;

        }
        public static bool UpdateInProgress { get; set; }
        public static bool UploadInProgress { get; set; }
        public static bool CancelUpload { get; set; }

        public static DateTime? JSONFileCreationTime { get; set; }
        public static Task<bool> UploadToDBAsync(string jsonFullFileName = "", int loopCount = 0)
        {
            return Task.Run(() => UploadToDatabase(jsonFullFileName: jsonFullFileName, loopCount: loopCount));
        }

        public static Task<bool> UploadToDBResolvedLandingsAsync(List<VesselLanding> resolvedLanddings)
        {
            return Task.Run(() => UploadToDatabase(resolvedLanddings));
        }

        public static Task<int> ProcessXFormIDsAsync(bool locateUnsavedFromServer = false)
        {
            if (locateUnsavedFromServer)
            {
                return Task.Run(() => LocateXFormIDsUnsaved());
            }
            else
            {
                return Task.Run(() => ProcessXFormIDsEx());
            }
        }

        public static List<VesselLanding> UnsavedVesselLandings { get; private set; }

        public static Task UpdateMissingLandingSitesAsync()
        {

            return Task.Run(() => UpdateMissingLandingSites());
        }

        private static void UpdateMissingLandingSites()
        {
            int foundCount = 0;
            foreach (VesselLanding landing in VesselLandings)
            {
                int? rowID = NSAPEntities.SummaryItemViewModel.
                    UpdateLandingSiteInLanding(landing._uuid, landing.LandingSiteName2);

                if (rowID != null)
                {
                    foundCount++;
                    VesselUnloadRepository.UpdateMissingLandingSite((int)rowID, landing.LandingSiteName2);
                    LandingWithUpdatedLandingSite?.Invoke(foundCount);
                }
            }

        }

        public static Task<List<VesselLanding>> GetFormWithMissingLandingSiteInfoAsync()
        {
            return Task.Run(() => GetFormWithMissingLandingSiteInfo());
        }
        private static List<VesselLanding> GetFormWithMissingLandingSiteInfo()
        {
            List<VesselLanding> formsWithMissingLSInfo = new List<VesselLanding>();
            foreach (VesselLanding landing in VesselLandings)
            {
                //if (landing.LandingSiteID == null && string.IsNullOrEmpty(landing.LandingSiteText))
                //{
                //    formsWithMissingLSInfo.Add(landing);
                //}
                if (landing.LandingSite == null &&
                    string.IsNullOrEmpty(landing.LandingSiteText)
                    )
                {
                    formsWithMissingLSInfo.Add(landing);
                }
            }
            return formsWithMissingLSInfo;
        }
        private static int LocateXFormIDsUnsaved()
        {
            UnsavedVesselLandings = new List<VesselLanding>();
            bool locateStart = false;
            int loopCount = 0;
            int foundCount = 0;

            foreach (VesselLanding landing in VesselLandings)
            {
                loopCount++;
                if (!CancelUpload)
                {
                    if (!locateStart)
                    {
                        locateStart = true;
                    }
                    if (!NSAPEntities.SummaryItemViewModel.XformIDExists(landing._uuid))
                    {
                        UnsavedVesselLandings.Add(landing);
                        foundCount++;
                    }
                }
                else
                {

                }
            }
            return 0;
        }
        private static int ProcessXFormIDsEx()
        {
            bool updateStart = false;
            int updatedCount = 0;
            int loopCount = 0;
            UpdateInProgress = true;
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.SearchingUpdates });
            var landings = VesselLandings.Where(t => t._xform_id_string.Length > 0).ToList();
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToUpdateCount = VesselLandings.Count, Intent = UploadToDBIntent.StartOfUpdate });
            foreach (VesselLanding landing in landings)
            {
                //bool updated = false;
                loopCount++;
                if (!CancelUpload)
                {
                    if (!updateStart)
                    {
                        updateStart = true;
                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.UpdateFound });
                    }

                    if (VesselUnloadRepository.UpdateXFormID(landing._uuid, landing._xform_id_string))
                    {
                        if (NSAPEntities.SummaryItemViewModel.UpdateRecordInRepo(landing._uuid, landing._xform_id_string))
                        {
                            updatedCount++;
                            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadUpdatedCount = updatedCount, Intent = UploadToDBIntent.Updating, VesselUnloadFoundCount = loopCount });
                            //updated = true;
                        }
                    }
                    else
                    {
                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.UnloadFound, VesselUnloadFoundCount = loopCount });
                    }
                }
                else
                {
                    UploadInProgress = false;
                    UpdateInProgress = false;
                    break;
                }


            }
            if (CancelUpload)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Cancelled });
            }
            else
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadUpdatedCount = updatedCount, Intent = UploadToDBIntent.EndOfUpdate });
            }
            UploadInProgress = false;
            return updatedCount;
        }
        private static int UpdateXFormIDs()
        {
            bool updateStart = false;
            UpdateInProgress = true;
            int updatedCount = 0;
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.SearchingUpdates });
            foreach (var landing in VesselLandings)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToUpdateCount = VesselLandings.Count, Intent = UploadToDBIntent.StartOfUpdate });
                if (!CancelUpload)
                {
                    try
                    {
                        var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(landing);
                        if (landingSiteSampling != null)
                        {
                            if (landingSiteSampling.GearUnloadViewModel == null)
                            {
                                landingSiteSampling.GearUnloadViewModel = new GearUnloadViewModel(landingSiteSampling);
                            }




                            GearUnload gear_unload = null;

                            gear_unload = landingSiteSampling.GearUnloadViewModel.GetGearUnloads(landingSiteSampling).FirstOrDefault(t => t.GearUsedText == landing.GearUsedText || t.GearID == landing.GearCode);
                            if (gear_unload != null)
                            {
                                if (gear_unload.VesselUnloadViewModel == null)
                                {
                                    gear_unload.VesselUnloadViewModel = new VesselUnloadViewModel(gear_unload);
                                }

                                bool proceed = true;
                                VesselUnload vu = gear_unload.VesselUnloadViewModel.getVesselUnload(landing._uuid);
                                if (vu != null)
                                {
                                    if (vu.XFormIdentifier.Length == 0)
                                    {
                                        vu.XFormIdentifier = landing._xform_id_string;
                                    }
                                    else
                                    {
                                        proceed = false;
                                    }
                                }
                                else if (vu == null)
                                {
                                    vu = new VesselUnload
                                    {
                                        XFormIdentifier = landing._xform_id_string,
                                        ODKRowID = landing._uuid
                                    };
                                }

                                if (proceed && gear_unload.VesselUnloadViewModel.UpdateRecordInRepo(vu, updateXFormID: true))
                                {
                                    if (!updateStart)
                                    {
                                        updateStart = true;
                                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.UpdateFound });
                                    }
                                    if (NSAPEntities.SummaryItemViewModel.UpdateRecordInRepo(vu))
                                    {
                                        updatedCount++;
                                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadUpdatedCount = updatedCount, Intent = UploadToDBIntent.Updating });
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {
                    break;
                }
            }
            if (CancelUpload)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Cancelled });
            }
            else
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadUpdatedCount = updatedCount, Intent = UploadToDBIntent.EndOfUpdate });
            }
            UploadInProgress = false;
            return updatedCount;
        }


        public static List<VesselLanding> ResolvedLandingsFromUnrecognizedFishingGrounds { get; private set; }
        public static List<UnrecognizedFishingGround> UnrecognizedFishingGrounds
        {
            get
            {
                return _unrecognizedFishingGrounds;
            }
        }

        public static void ResetTotalUploadCounter(bool uploadingDone = false)
        {
            TotalUploadCount = 0;
            if (uploadingDone)
            {
                UpdateInProgress = false;
                UploadInProgress = false;
                UnmatchedEnumeratorIDs.Clear();
            }
        }
        public static int TotalUploadCount { get; private set; }

        public static string CurrentJSONFileName { get; set; }
        public static bool UploadToDatabase(List<VesselLanding> resolvedLandings = null, string jsonFullFileName = "", int loopCount = 0)
        {
            bool hasUnrecognizedFG = false;
            bool isVersion643 = false;
            DelayedSave = true;
            UploadInProgress = true;
            int savedCount = 0;

            //GearUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.GetGearUnloadMaxRecordNumber();
            //GearUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearUnloadPK;

            if (loopCount == 0)
            {
                int idFromDB = GearUnloadRepository.MaxRecordNumberFromDB();
                if (idFromDB > GearUnloadViewModel.CurrentIDNumber)
                {
                    GearUnloadViewModel.CurrentIDNumber = idFromDB;
                }
            }

            //VesselUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadPK;

            List<VesselLanding> landings;
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Searching });
            if (resolvedLandings != null)
            {
                landings = resolvedLandings;
            }
            else
            {
                landings = VesselLandings.Where(t => t.SavedInLocalDatabase == false).ToList();
            }
            if (VesselLandings.Count > 0 && landings.Count == 0)
            {
                return true;
            }
            else if (landings.Count > 0)
            {
                hasUnrecognizedFG = false;
                _unrecognizedFishingGrounds = new List<UnrecognizedFishingGround>();
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToSaveCount = landings.Count, Intent = UploadToDBIntent.StartOfUpload });
                foreach (var landing in landings)
                {
                    if (!CancelUpload)
                    {
                        try
                        {
                            bool proceed = false;
                            SamplingDaySubmission sds = null;
                            LandingSiteSampling landingSiteSampling = null;


                            if (landing.LandingSite != null && landing.FishingGround != null)
                            {
                                sds = NSAPEntities.SamplingDaySubmissionViewModel.GetSamplingDaySubmission(landing.LandingSite.LandingSiteID, landing.FishingGround.Code, landing.SamplingDate.Date);
                            }
                            if (sds != null)
                            {
                                if (sds.LandingSiteSampling == null)
                                {
                                    sds.LandingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.CreateInstance(sds.SamplingDayID);
                                }
                                landingSiteSampling = sds.LandingSiteSampling;
                            }
                            else if (!string.IsNullOrEmpty(landing.LandingSiteText) && landing.FishingGround != null)
                            {
                                sds = NSAPEntities.SamplingDaySubmissionViewModel.GetSamplingDaySubmission(landing.LandingSiteText, landing.FishingGround.Code, landing.SamplingDate);
                                if (sds != null && sds.LandingSiteSampling != null)
                                {
                                    landingSiteSampling = sds.LandingSiteSampling;
                                }
                            }
                            //var 
                            if (landingSiteSampling == null)
                            {
                                var code = landing.FishingGround?.Code;
                                if (string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(landing.FishingGroundName))
                                {
                                    var fg = NSAPRegionWithEntitiesRepository.GetFishingGround(landing.NSAPRegion.ShortName, landing.FMA.Name, landing.FishingGroundName);
                                    if (fg != null)
                                    {
                                        code = fg.Code;
                                    }
                                }

                                if (code != null)
                                {
                                    landingSiteSampling = new LandingSiteSampling
                                    {
                                        //PK = NSAPEntities.LandingSiteSamplingViewModel.NextRecordNumber,
                                        PK = LandingSiteSamplingViewModel.CurrentIDNumber + 1,
                                        LandingSiteID = landing.LandingSite == null ? null : (int?)landing.LandingSite.LandingSiteID,
                                        FishingGroundID = code,
                                        IsSamplingDay = true,
                                        SamplingDate = landing.SamplingDate.Date,
                                        NSAPRegionID = landing.NSAPRegion.Code,
                                        LandingSiteText = landing.LandingSiteText == null ? landing.LandingSiteName2 : landing.LandingSiteText,
                                        FMAID = landing.NSAPRegionFMA.FMA.FMAID,
                                        DelayedSave = DelayedSave,
                                        HasFishingOperation = true,
                                        DateAdded = DateTime.Now
                                    };

                                    if (NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(landingSiteSampling))
                                    {
                                        LandingSiteSamplingViewModel.CurrentIDNumber = landingSiteSampling.PK;
                                        proceed = NSAPEntities.SamplingDaySubmissionViewModel.Add(landingSiteSampling);
                                    }
                                }

                                if (!proceed && landing.FishingGround == null)
                                {
                                    if (!string.IsNullOrEmpty(landing.FishingGroundName))
                                    {
                                        UnrecognizedFishingGround urf = new UnrecognizedFishingGround
                                        {
                                            FishingGroundName = string.IsNullOrEmpty(landing.FishingGroundName) ? "" : landing.FishingGroundName,
                                            RegionFishingGround = landing.RegionFishingGroundID,
                                            FishingGear = landing.GearName,
                                            FishingVessel = landing.BoatName,
                                            LandingSite = landing.LandingSiteName2.Replace('»', ','),
                                            FMA = landing.FMA.Name,
                                            Region = landing.NSAPRegion.ShortName,
                                            Enumerator = landing.EnumeratorName,
                                            SamplingDate = landing.SamplingDate,
                                            RowID = landing._uuid,
                                            VesselLanding = landing,

                                        };
                                        _unrecognizedFishingGrounds.Add(urf);
                                        hasUnrecognizedFG = true;

                                        Utilities.Logger.Log($"Missing fishing ground\r\n ODK row ID:{landing._uuid}, user name:{landing.user_name}, region:{landing.NSAPRegion.ShortName}, fma:{landing.FMA}, version:{landing.intronote}");
                                    }
                                    else
                                    {
                                        Utilities.Logger.Log("Missing fishing ground info.Cannot upload\r\n" +
                                            $"ODK row ID:{landing._uuid}, user name:{landing.user_name}, region:{landing.NSAPRegion.ShortName}, fma:{landing.FMA}, version:{landing.intronote}");
                                    }
                                }
                            }
                            else
                            {
                                proceed = true;
                            }

                            if (proceed && landingSiteSampling.GearUnloadViewModel == null)
                            {
                                landingSiteSampling.GearUnloadViewModel = new GearUnloadViewModel(landingSiteSampling);
                            }

                            GearUnload gear_unload = null;
                            if (proceed)
                            {
                                proceed = false;

                                //Oct 21: added fisheries sector when making a new gear unload
                                gear_unload = landingSiteSampling.GearUnloadViewModel.GetGearUnloads(landingSiteSampling).FirstOrDefault(t => (t.GearUsedText == landing.GearUsedText || t.GearID == landing.GearCode) && t.SectorCode == landing.SectorCode);

                                if (gear_unload == null)
                                {
                                    if (landing.GearCode == "_OT")
                                    {
                                        landing.GearCode = null;
                                    }

                                    gear_unload = new GearUnload
                                    {
                                        PK = GearUnloadViewModel.CurrentIDNumber + 1,
                                        Parent = landingSiteSampling,
                                        LandingSiteSamplingID = landingSiteSampling.PK,
                                        GearID = NSAPEntities.GearViewModel.GearCodeExist(landing.GearCode) ? landing.GearCode : string.Empty,
                                        GearUsedText = landing.GearUsedText == null ? landing.GearName : landing.GearUsedText,

                                        //added on Oct 21 2022
                                        SectorCode = landing.SectorCode,

                                        Remarks = "",
                                        DelayedSave = DelayedSave
                                    };
                                    try
                                    {
                                        if (landingSiteSampling.GearUnloadViewModel.AddRecordToRepo(gear_unload))
                                        {
                                            proceed = true;
                                            GearUnloadViewModel.CurrentIDNumber = gear_unload.PK;
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        Logger.Log(ex);
                                    }

                                }
                                else
                                {
                                    proceed = true;
                                }

                                if (proceed && gear_unload.VesselUnloadViewModel == null)
                                {
                                    gear_unload.VesselUnloadViewModel = new VesselUnloadViewModel(isNew: true);
                                }
                            }
                            if (proceed)
                            {
                                proceed = false;

                                var gpscode = "";
                                if (landing.GPS2 != null)
                                {
                                    gpscode = landing.GPS2;
                                }
                                else
                                {
                                    gpscode = landing.GPSCode;
                                }

                                bool withCatchComp;
                                if (landing.IncludeCatchComposition == null)
                                {
                                    withCatchComp = false;
                                }
                                else
                                {
                                    withCatchComp = landing.IncludeCatchComposition == "yes" ? true : false;
                                }
                                isVersion643 = false;
                                if (!string.IsNullOrEmpty(landing.Form_version))
                                {
                                    if (double.TryParse(landing.Form_version, out double form_version))
                                    {
                                        isVersion643 = form_version >= 6.43;
                                    }
                                }

                                VesselUnload vu = new VesselUnload
                                {
                                    //PK = landing.PK,
                                    PK = VesselUnloadViewModel.CurrentIDNumber + 1,
                                    Parent = gear_unload,
                                    GearUnloadID = gear_unload.PK,
                                    IsBoatUsed = landing.IsBoatUsed,
                                    VesselID = landing.IsBoatUsed == false ? null :
                                                landing.BoatUsed == null ? null : landing.BoatUsed,
                                    VesselText = landing.BoatUsedText,
                                    NumberOfFishers = landing.NumberOfFishers,
                                    SectorCode = landing.SectorCode,
                                    WeightOfCatch = landing.CatchTotalWt,
                                    WeightOfCatchSample = landing.CatchSampleWt,
                                    Boxes = landing.BoxesTotal,
                                    BoxesSampled = landing.BoxesSampled,
                                    //RaisingFactor = landing.RaisingFactor,
                                    OperationIsSuccessful = landing.TripIsSuccess,
                                    OperationIsTracked = landing.IncludeTracking,
                                    FishingTripIsCompleted = landing.TripIsCompleted,
                                    DepartureFromLandingSite = landing.DateTimeDepartLandingSite,
                                    ArrivalAtLandingSite = landing.DateTimeArriveLandingSite,
                                    ODKRowID = landing._uuid,
                                    UserName = landing.user_name,
                                    DeviceID = landing.device_id,
                                    DateTimeSubmitted = landing._submission_time,
                                    FormVersion = landing.Form_version,
                                    GPSCode = gpscode,
                                    SamplingDate = landing.SamplingDate,
                                    Notes = landing.Remarks,
                                    NSAPEnumeratorID = landing.NSAPEnumerator == null ? null : (int?)landing.NSAPEnumerator.ID,
                                    EnumeratorText = string.IsNullOrEmpty(landing.EnumeratorText) ? landing.user_name : landing.EnumeratorText,
                                    DateAddedToDatabase = DateTime.Now,
                                    FromExcelDownload = false,
                                    TimeStart = landing.start,
                                    HasCatchComposition = withCatchComp,
                                    XFormIdentifier = landing._xform_id_string,
                                    DelayedSave = DelayedSave,
                                    RefNo = landing.ref_no,
                                    IsCatchSold = landing.IsCatchSold,
                                    //JSONFileName = jsonFileName,
                                    JSONFileName = Path.GetFileName(jsonFullFileName),
                                    IsMultiGear = landing.IsMultiGear,
                                    IncludeEffortIndicators = landing.IncludeEffort,
                                    SubmissionID=landing._id
                                };

                                if (vu.IsMultiGear)
                                {
                                    vu.CountGearTypesUsed = (int)landing.GearTypeCount;
                                }
                                else
                                {
                                    vu.CountGearTypesUsed = 1;
                                }



                                if (JSONFileCreationTime != null)
                                {
                                    vu.DateAddedToDatabase = (DateTime)JSONFileCreationTime;
                                }

                                if (gear_unload.VesselUnloadViewModel.AddRecordToRepo(vu))
                                {
                                    VesselUnloadViewModel.CurrentIDNumber = vu.PK;
                                    if (vu.IsMultiGear)
                                    {
                                        vu.VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(vu);
                                        if (!VesselSamplingRepeatGear.RowIDSet)
                                        {
                                            VesselSamplingRepeatGear.SetRowIDs();
                                        }
                                        foreach (var landing_fg in landing.SamplingGears)
                                        {
                                            VesselUnload_FishingGear vu_fg = new VesselUnload_FishingGear
                                            {
                                                GearText = landing_fg.GearUsedText,
                                                GearCode = landing_fg.GearCode,
                                                Parent = vu,
                                                //RowID = (int)landing_fg.PK,
                                                RowID = VesselUnload_FishingGearViewModel.CurrentIDNumber + 1,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.VesselUnload_FishingGearsViewModel.AddRecordToRepo(vu_fg))
                                            {
                                                VesselUnload_FishingGearViewModel.CurrentIDNumber = vu_fg.RowID;


                                                vu_fg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel(vu_fg);
                                                MultiGearEffortSpecContainer egr;

                                                if (string.IsNullOrEmpty(vu_fg.GearCode))
                                                {
                                                    egr = landing.GearsEfforts.FirstOrDefault(t => t.SelectedGearName == vu_fg.GearText);
                                                }
                                                else
                                                {
                                                    egr = landing.GearsEfforts.FirstOrDefault(t => t.SelectedGearCode == vu_fg.GearCode);
                                                }

                                                if (!MultiGearEffortSpec.RowIDSet)
                                                {
                                                    MultiGearEffortSpec.SetRowIDs();
                                                }


                                                foreach (var gear_effortspec in egr.EffortsSpecsOfGear)
                                                {
                                                    VesselUnload_Gear_Spec vu_gs = new VesselUnload_Gear_Spec
                                                    {
                                                        EffortSpecID = gear_effortspec.EffortType,
                                                        EffortValueNumeric = gear_effortspec.EffortNumericValue,
                                                        EffortValueText = gear_effortspec.EffortDescription,
                                                        Parent = vu_fg,
                                                        //RowID = (int)gear_effortspec.PK,
                                                        RowID = VesselEffortViewModel.CurrentIDNumber + 1,
                                                        DelayedSave = DelayedSave
                                                    };

                                                    if (vu_fg.VesselUnload_Gear_Specs_ViewModel.AddRecordToRepo(vu_gs))
                                                    {
                                                        VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = vu_gs.RowID;
                                                        VesselEffortViewModel.CurrentIDNumber = vu_gs.RowID;
                                                        vu_fg.CountEffortIndicators++;
                                                    }

                                                }

                                                vu_fg.VesselUnload_Gear_Specs_ViewModel.Dispose();

                                            }
                                        }
                                        vu.VesselUnload_FishingGearsViewModel.Dispose();
                                    }

                                    else if (landing.SingleGearEffortSpecs != null)
                                    {
                                        vu.VesselEffortViewModel = new VesselEffortViewModel(isNew: true);
                                        vu.CountEffortIndicators = landing.SingleGearEffortSpecs.Count;
                                        if (!EffortSpecSingleGear.RowIDSet)
                                        {
                                            EffortSpecSingleGear.SetRowIDs();
                                        }
                                        foreach (var effort in landing.SingleGearEffortSpecs
                                            .Where(t => t.Parent.PK == landing.PK))
                                        {
                                            VesselEffort ve = new VesselEffort
                                            {
                                                PK = VesselEffortViewModel.CurrentIDNumber + 1,
                                                Parent = vu,
                                                VesselUnloadID = vu.PK,
                                                EffortSpecID = effort.EffortType,
                                                EffortValueNumeric = effort.EffortIntensity,
                                                EffortValueText = effort.EffortDescription,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.VesselEffortViewModel.AddRecordToRepo(ve))
                                            {
                                                VesselEffortViewModel.CurrentIDNumber = ve.PK;
                                                VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = ve.PK;
                                            }

                                        }
                                        vu.VesselEffortViewModel.Dispose();
                                    }

                                    if (landing.GearSoakTimes != null)
                                    {
                                        vu.GearSoakViewModel = new GearSoakViewModel(isNew: true);
                                        vu.CountGearSoak = landing.GearSoakTimes.Count;
                                        if (!SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.RowIDSet)
                                        {
                                            SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
                                        }
                                        foreach (var soak in landing.GearSoakTimes
                                            .Where(t => t.Parent.PK == landing.PK))
                                        {
                                            GearSoak gs = new GearSoak
                                            {
                                                //PK = (int)soak.PK,
                                                PK = GearSoakViewModel.CurrentIDNumber + 1,
                                                Parent = vu,
                                                VesselUnloadID = vu.PK,
                                                TimeAtSet = soak.SetTime,
                                                TimeAtHaul = soak.HaulTime,
                                                WaypointAtSet = soak.WaypointAtSet,
                                                WaypointAtHaul = soak.WaypointAtHaul,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.GearSoakViewModel.AddRecordToRepo(gs))
                                            {
                                                GearSoakViewModel.CurrentIDNumber = gs.PK;
                                            }
                                        }
                                        vu.GearSoakViewModel.Dispose();
                                    }

                                    if (landing.GridCoordinates != null)
                                    {
                                        vu.FishingGroundGridViewModel = new FishingGroundGridViewModel(isNew: true);
                                        vu.CountGrids = landing.GridCoordinates.Count;
                                        if (!GridCoordGroupBingoRepeat.RowIDSet)
                                        {
                                            GridCoordGroupBingoRepeat.SetRowIDs();
                                        }
                                        foreach (var gr in landing.GridCoordinates
                                             .Where(t => t.Parent.PK == landing.PK))
                                        {
                                            FishingGroundGrid fgg = new FishingGroundGrid
                                            {
                                                //PK = (int)gr.PK,
                                                PK = FishingGroundGridViewModel.CurrentIDNumber + 1,
                                                Parent = vu,
                                                VesselUnloadID = vu.PK,
                                                UTMZoneText = gr.Parent.UTMZone,
                                                Grid = gr.CompleteGridName,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.FishingGroundGridViewModel.AddRecordToRepo(fgg))
                                            {
                                                FishingGroundGridViewModel.CurrentIDNumber = fgg.PK;
                                            }
                                        }
                                        vu.FishingGroundGridViewModel.Dispose();
                                    }

                                    if (landing.CatchComposition != null)
                                    {
                                        int missingCatchInfoCounter = 0;
                                        vu.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                        vu.CountCatchCompositionItems = landing.CatchComposition.Count;
                                        if (!CatchCompGroupCatchCompositionRepeat.RowIDSet)
                                        {
                                            CatchCompGroupCatchCompositionRepeat.SetRowIDs();
                                        }

                                        foreach (var catchComp in landing.CatchComposition
                                            .Where(t => t.Parent.PK == landing.PK))
                                        {

                                            if (catchComp.SpeciesID != null || !string.IsNullOrEmpty(catchComp.SpeciesNameOther))
                                            {
                                                bool fromTotal = false;

                                                if (isVersion643)
                                                {
                                                    if (vu.WeightOfCatchSample == null)
                                                    {
                                                        fromTotal = true;
                                                    }
                                                    else
                                                    {
                                                        fromTotal = catchComp.FromTotalCatch;
                                                    }
                                                }
                                                else
                                                {
                                                    fromTotal = false;
                                                }


                                                VesselCatch vc = new VesselCatch
                                                {
                                                    //PK = catchComp.PK,
                                                    PK = VesselCatchViewModel.CurrentIDNumber + 1,
                                                    Parent = vu,
                                                    VesselUnloadID = vu.PK,
                                                    SpeciesID = catchComp.CatchCompSpeciesID,
                                                    Catch_kg = catchComp.SpeciesWt,
                                                    Sample_kg = catchComp.SpeciesSampleWt,
                                                    TaxaCode = catchComp.TaxaCode,
                                                    SpeciesText = catchComp.SpeciesNameOther,
                                                    DelayedSave = DelayedSave,
                                                    WeighingUnit = catchComp.IndividualWeightUnit,
                                                    FromTotalCatch = fromTotal,
                                                    PriceOfSpecies = catchComp.PriceOfSpecies,
                                                    PriceUnit = catchComp.PriceUnitText
                                                };

                                                if (landing.IsMultiGear)
                                                {
                                                    vc.GearCode = catchComp.Gear_species_code;
                                                    vc.GearText = catchComp.Gear_species_name;
                                                }


                                                if (vu.VesselCatchViewModel.AddRecordToRepo(vc))
                                                {
                                                    VesselCatchViewModel.CurrentIDNumber = vc.PK;

                                                    if (catchComp.LenFreqRepeat != null)
                                                    {
                                                        vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(isNew: true);
                                                        vu.CountLenFreqRows += catchComp.LenFreqRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
                                                        }
                                                        foreach (var lf in catchComp.LenFreqRepeat
                                                            .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchLenFreq clf = new CatchLenFreq
                                                            {
                                                                //PK = (int)lf.PK,
                                                                PK = CatchLenFreqViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                LengthClass = lf.LengthClass,
                                                                Frequency = lf.Frequency,
                                                                Sex = lf.Sex,
                                                                DelayedSave = DelayedSave
                                                            };
                                                            if (vc.CatchLenFreqViewModel.AddRecordToRepo(clf))
                                                            {
                                                                CatchLenFreqViewModel.CurrentIDNumber = clf.PK;
                                                            }
                                                        }
                                                        vc.CatchLenFreqViewModel.Dispose();
                                                    }

                                                    if (catchComp.LenWtRepeat != null)
                                                    {
                                                        vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(isNew: true);
                                                        vu.CountLenWtRows += catchComp.LenWtRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatLenWtRepeat.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
                                                        }
                                                        foreach (var lw in catchComp.LenWtRepeat
                                                             .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchLengthWeight clw = new CatchLengthWeight
                                                            {
                                                                //PK = (int)lw.PK,
                                                                PK = CatchLengthWeightViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                Length = lw.Length,
                                                                Weight = lw.Weight,
                                                                Sex = lw.Sex,
                                                                DelayedSave = DelayedSave
                                                            };
                                                            if (vc.CatchLengthWeightViewModel.AddRecordToRepo(clw))
                                                            {
                                                                CatchLengthWeightViewModel.CurrentIDNumber = clw.PK;
                                                            }

                                                        }
                                                        vc.CatchLengthWeightViewModel.Dispose();
                                                    }

                                                    if (catchComp.LengthListRepeat != null)
                                                    {
                                                        vc.CatchLengthViewModel = new CatchLengthViewModel(isNew: true);
                                                        vu.CountLengthRows += catchComp.LengthListRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatLengthListRepeat.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
                                                        }
                                                        foreach (var l in catchComp.LengthListRepeat
                                                             .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchLength cl = new CatchLength
                                                            {
                                                                //PK = (int)l.PK,
                                                                PK = CatchLengthViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                Length = l.Length,
                                                                Sex = l.Sex,
                                                                DelayedSave = DelayedSave

                                                            };
                                                            if (vc.CatchLengthViewModel.AddRecordToRepo(cl))
                                                            {
                                                                CatchLengthViewModel.CurrentIDNumber = cl.PK;
                                                            }
                                                        }
                                                        vc.CatchLengthViewModel.Dispose();
                                                    }

                                                    if (catchComp.GMSRepeat != null)
                                                    {
                                                        vc.CatchMaturityViewModel = new CatchMaturityViewModel(isNew: false);
                                                        vu.CountMaturityRows += catchComp.GMSRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
                                                        }
                                                        foreach (var m in catchComp.GMSRepeat
                                                             .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchMaturity cm = new CatchMaturity
                                                            {
                                                                //PK = (int)m.PK,
                                                                PK = CatchMaturityViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                Length = m.Length,
                                                                Weight = m.Weight,
                                                                SexCode = m.SexCode,
                                                                MaturityCode = m.GMSCode,
                                                                WeightGutContent = m.StomachContentWt,
                                                                GutContentCode = m.GutContentCategoryCode,
                                                                GonadWeight = m.GonadWeight,
                                                                DelayedSave = DelayedSave
                                                            };
                                                            if (cm.GonadWeight != null)
                                                            {

                                                            }
                                                            if (vc.CatchMaturityViewModel.AddRecordToRepo(cm))
                                                            {
                                                                CatchMaturityViewModel.CurrentIDNumber = cm.PK;
                                                            }
                                                        }
                                                        vc.CatchMaturityViewModel.Dispose();
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                missingCatchInfoCounter++;
                                            }
                                        }


                                        if (missingCatchInfoCounter > 0)
                                        {
                                            Utilities.Logger.LogMissingCatchInfo($@"""{vu.ODKRowID}"",{missingCatchInfoCounter}, ""{vu.XFormIdentifier}"",""{vu.FormVersion}"",""{vu.Parent.GearUsedName}"",""{vu.EnumeratorName}"",""{vu.Parent.Parent.LandingSiteName}"",{vu.SamplingDate},""{System.IO.Path.GetFileName(CurrentJSONFileName)}"",{DateTime.Now}");
                                        }
                                    }


                                    if (gear_unload.VesselUnloadViewModel.UpdateUnloadStats(vu) && NSAPEntities.SummaryItemViewModel.AddRecordToRepo(vu))
                                    {
                                        if (landing.IncludeCatchComp)
                                        {
                                            gear_unload.VesselUnloadViewModel.UpdateWeightValidation(NSAPEntities.SummaryItemViewModel.CurrentEntity, vu);
                                            vu.VesselCatchViewModel?.Dispose();
                                        }
                                        vu.Dispose();
                                        //gear_unload.VesselUnloadViewModel.Dispose();
                                        //gear_unload.Dispose();
                                        savedCount++;
                                        landing.SavedInLocalDatabase = true;
                                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
                                        TotalUploadCount++;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.Logger.Log(ex);
                        }
                    }
                    else
                    {
                        UploadInProgress = false;
                        UpdateInProgress = false;
                        break;
                    }
                }
            }
            if (CancelUpload)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Cancelled });
            }
            else
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadTotalSavedCount = savedCount, Intent = UploadToDBIntent.EndOfUpload });
                if (resolvedLandings == null)
                {
                    _unrecognizedFishingGrounds = new List<UnrecognizedFishingGround>();
                }
                else
                {
                    ResolvedLandingsFromUnrecognizedFishingGrounds = resolvedLandings;
                }
            }
            UploadInProgress = false;
            UpdateInProgress = false;

            foreach (var landing in landings)
            {
                //if (landing.GearUnload != null)
                //{
                landing.GearUnload?.Dispose();
                //}
                landing.Dispose();
            }
            landings.Clear();
            landings = null;
            //VesselLandings?.Clear();
            //VesselLandings = null;

            return savedCount > 0 || hasUnrecognizedFG;
        }
        public static bool UploadToDatabase1(List<VesselLanding> resolvedLandings = null, string jsonFileName = "", int loopCount = 0)
        {
            bool hasUnrecognizedFG = false;
            bool isVersion643 = false;
            DelayedSave = true;
            UploadInProgress = true;
            int savedCount = 0;

            //GearUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.GetGearUnloadMaxRecordNumber();
            GearUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastGearUnloadPK;

            if (loopCount == 0)
            {
                int idFromDB = GearUnloadRepository.MaxRecordNumberFromDB();
                if (idFromDB > GearUnloadViewModel.CurrentIDNumber)
                {
                    GearUnloadViewModel.CurrentIDNumber = idFromDB;
                }
            }

            VesselUnloadViewModel.CurrentIDNumber = NSAPEntities.SummaryItemViewModel.LastPrimaryKeys.LastVesselUnloadPK;

            List<VesselLanding> landings;
            UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Searching });
            if (resolvedLandings != null)
            {
                landings = resolvedLandings;
            }
            else
            {
                landings = VesselLandings.Where(t => t.SavedInLocalDatabase == false).ToList();
            }
            if (VesselLandings.Count > 0 && landings.Count == 0)
            {
                return true;
            }
            else if (landings.Count > 0)
            {
                hasUnrecognizedFG = false;
                _unrecognizedFishingGrounds = new List<UnrecognizedFishingGround>();
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadToSaveCount = landings.Count, Intent = UploadToDBIntent.StartOfUpload });
                foreach (var landing in landings)
                {
                    if (!CancelUpload)
                    {
                        try
                        {
                            bool proceed = false;
                            var landingSiteSampling = NSAPEntities.LandingSiteSamplingViewModel.GetLandingSiteSampling(landing);
                            if (landingSiteSampling == null)
                            {
                                var code = landing.FishingGround?.Code;
                                if (string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(landing.FishingGroundName))
                                {
                                    var fg = NSAPRegionWithEntitiesRepository.GetFishingGround(landing.NSAPRegion.ShortName, landing.FMA.Name, landing.FishingGroundName);
                                    if (fg != null)
                                    {
                                        code = fg.Code;
                                    }
                                }

                                if (code != null)
                                {
                                    landingSiteSampling = new LandingSiteSampling
                                    {
                                        PK = NSAPEntities.LandingSiteSamplingViewModel.NextRecordNumber,
                                        LandingSiteID = landing.LandingSite == null ? null : (int?)landing.LandingSite.LandingSiteID,
                                        FishingGroundID = code,
                                        IsSamplingDay = true,
                                        SamplingDate = landing.SamplingDate.Date,
                                        NSAPRegionID = landing.NSAPRegion.Code,
                                        LandingSiteText = landing.LandingSiteText == null ? landing.LandingSiteName2 : landing.LandingSiteText,
                                        FMAID = landing.NSAPRegionFMA.FMA.FMAID,
                                        DelayedSave = DelayedSave,
                                        HasFishingOperation = true,
                                        DateAdded = DateTime.Now
                                    };

                                    proceed = NSAPEntities.LandingSiteSamplingViewModel.AddRecordToRepo(landingSiteSampling);
                                }

                                if (!proceed && landing.FishingGround == null)
                                {
                                    if (!string.IsNullOrEmpty(landing.FishingGroundName))
                                    {
                                        UnrecognizedFishingGround urf = new UnrecognizedFishingGround
                                        {
                                            FishingGroundName = string.IsNullOrEmpty(landing.FishingGroundName) ? "" : landing.FishingGroundName,
                                            RegionFishingGround = landing.RegionFishingGroundID,
                                            FishingGear = landing.GearName,
                                            FishingVessel = landing.BoatName,
                                            LandingSite = landing.LandingSiteName2.Replace('»', ','),
                                            FMA = landing.FMA.Name,
                                            Region = landing.NSAPRegion.ShortName,
                                            Enumerator = landing.EnumeratorName,
                                            SamplingDate = landing.SamplingDate,
                                            RowID = landing._uuid,
                                            VesselLanding = landing,

                                        };
                                        _unrecognizedFishingGrounds.Add(urf);
                                        hasUnrecognizedFG = true;

                                        Utilities.Logger.Log($"Missing fishing ground\r\n ODK row ID:{landing._uuid}, user name:{landing.user_name}, region:{landing.NSAPRegion.ShortName}, fma:{landing.FMA}, version:{landing.intronote}");
                                    }
                                    else
                                    {
                                        Utilities.Logger.Log("Missing fishing ground info.Cannot upload\r\n" +
                                            $"ODK row ID:{landing._uuid}, user name:{landing.user_name}, region:{landing.NSAPRegion.ShortName}, fma:{landing.FMA}, version:{landing.intronote}");
                                    }
                                }
                            }
                            else
                            {
                                proceed = true;
                            }

                            if (proceed && landingSiteSampling.GearUnloadViewModel == null)
                            {
                                landingSiteSampling.GearUnloadViewModel = new GearUnloadViewModel(landingSiteSampling);
                            }

                            GearUnload gear_unload = null;
                            if (proceed)
                            {
                                proceed = false;

                                //Oct 21: added fisheries sector when making a new gear unload
                                gear_unload = landingSiteSampling.GearUnloadViewModel.GetGearUnloads(landingSiteSampling).FirstOrDefault(t => (t.GearUsedText == landing.GearUsedText || t.GearID == landing.GearCode) && t.SectorCode == landing.SectorCode);

                                if (gear_unload == null)
                                {
                                    if (landing.GearCode == "_OT")
                                    {
                                        landing.GearCode = null;
                                    }

                                    gear_unload = new GearUnload
                                    {
                                        PK = GearUnloadViewModel.CurrentIDNumber + 1,
                                        Parent = landingSiteSampling,
                                        LandingSiteSamplingID = landingSiteSampling.PK,
                                        GearID = NSAPEntities.GearViewModel.GearCodeExist(landing.GearCode) ? landing.GearCode : string.Empty,
                                        GearUsedText = landing.GearUsedText == null ? landing.GearName : landing.GearUsedText,

                                        //added on Oct 21 2022
                                        SectorCode = landing.SectorCode,

                                        Remarks = "",
                                        DelayedSave = DelayedSave
                                    };
                                    if (landingSiteSampling.GearUnloadViewModel.AddRecordToRepo(gear_unload))
                                    {
                                        proceed = true;
                                        GearUnloadViewModel.CurrentIDNumber = gear_unload.PK;
                                    }

                                }
                                else
                                {
                                    proceed = true;
                                }

                                if (proceed && gear_unload.VesselUnloadViewModel == null)
                                {
                                    gear_unload.VesselUnloadViewModel = new VesselUnloadViewModel(isNew: true);
                                }
                            }
                            if (proceed)
                            {
                                proceed = false;

                                var gpscode = "";
                                if (landing.GPS2 != null)
                                {
                                    gpscode = landing.GPS2;
                                }
                                else
                                {
                                    gpscode = landing.GPSCode;
                                }

                                bool withCatchComp;
                                if (landing.IncludeCatchComposition == null)
                                {
                                    withCatchComp = false;
                                }
                                else
                                {
                                    withCatchComp = landing.IncludeCatchComposition == "yes" ? true : false;
                                }
                                isVersion643 = false;
                                if (!string.IsNullOrEmpty(landing.Form_version))
                                {
                                    if (double.TryParse(landing.Form_version, out double form_version))
                                    {
                                        isVersion643 = form_version >= 6.43;
                                    }
                                }

                                VesselUnload vu = new VesselUnload
                                {
                                    //PK = landing.PK,
                                    PK = VesselUnloadViewModel.CurrentIDNumber + 1,
                                    Parent = gear_unload,
                                    GearUnloadID = gear_unload.PK,
                                    IsBoatUsed = landing.IsBoatUsed,
                                    VesselID = landing.IsBoatUsed == false ? null :
                                                landing.BoatUsed == null ? null : landing.BoatUsed,
                                    VesselText = landing.BoatUsedText,
                                    NumberOfFishers = landing.NumberOfFishers,
                                    SectorCode = landing.SectorCode,
                                    WeightOfCatch = landing.CatchTotalWt,
                                    WeightOfCatchSample = landing.CatchSampleWt,
                                    Boxes = landing.BoxesTotal,
                                    BoxesSampled = landing.BoxesSampled,
                                    //RaisingFactor = landing.RaisingFactor,
                                    OperationIsSuccessful = landing.TripIsSuccess,
                                    OperationIsTracked = landing.IncludeTracking,
                                    FishingTripIsCompleted = landing.TripIsCompleted,
                                    DepartureFromLandingSite = landing.DateTimeDepartLandingSite,
                                    ArrivalAtLandingSite = landing.DateTimeArriveLandingSite,
                                    ODKRowID = landing._uuid,
                                    UserName = landing.user_name,
                                    DeviceID = landing.device_id,
                                    DateTimeSubmitted = landing._submission_time,
                                    FormVersion = landing.Form_version,
                                    GPSCode = gpscode,
                                    SamplingDate = landing.SamplingDate,
                                    Notes = landing.Remarks,
                                    NSAPEnumeratorID = landing.NSAPEnumerator == null ? null : (int?)landing.NSAPEnumerator.ID,
                                    EnumeratorText = string.IsNullOrEmpty(landing.EnumeratorText) ? landing.user_name : landing.EnumeratorText,
                                    DateAddedToDatabase = DateTime.Now,
                                    FromExcelDownload = false,
                                    TimeStart = landing.start,
                                    HasCatchComposition = withCatchComp,
                                    XFormIdentifier = landing._xform_id_string,
                                    DelayedSave = DelayedSave,
                                    RefNo = landing.ref_no,
                                    IsCatchSold = landing.IsCatchSold,
                                    JSONFileName = jsonFileName,
                                    IsMultiGear = landing.IsMultiGear,
                                    IncludeEffortIndicators = landing.IncludeEffort
                                };

                                if (vu.IsMultiGear)
                                {
                                    vu.CountGearTypesUsed = (int)landing.GearTypeCount;
                                }
                                else
                                {
                                    vu.CountGearTypesUsed = 1;
                                }



                                if (JSONFileCreationTime != null)
                                {
                                    vu.DateAddedToDatabase = (DateTime)JSONFileCreationTime;
                                }

                                if (gear_unload.VesselUnloadViewModel.AddRecordToRepo(vu))
                                {
                                    VesselUnloadViewModel.CurrentIDNumber = vu.PK;
                                    if (vu.IsMultiGear)
                                    {
                                        vu.VesselUnload_FishingGearsViewModel = new VesselUnload_FishingGearViewModel(vu);
                                        if (!VesselSamplingRepeatGear.RowIDSet)
                                        {
                                            VesselSamplingRepeatGear.SetRowIDs();
                                        }
                                        foreach (var landing_fg in landing.SamplingGears)
                                        {
                                            VesselUnload_FishingGear vu_fg = new VesselUnload_FishingGear
                                            {
                                                GearText = landing_fg.GearUsedText,
                                                GearCode = landing_fg.GearCode,
                                                Parent = vu,
                                                //RowID = (int)landing_fg.PK,
                                                RowID = VesselUnload_FishingGearViewModel.CurrentIDNumber + 1,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.VesselUnload_FishingGearsViewModel.AddRecordToRepo(vu_fg))
                                            {
                                                VesselUnload_FishingGearViewModel.CurrentIDNumber = vu_fg.RowID;


                                                vu_fg.VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel(vu_fg);
                                                MultiGearEffortSpecContainer egr;

                                                if (string.IsNullOrEmpty(vu_fg.GearCode))
                                                {
                                                    egr = landing.GearsEfforts.FirstOrDefault(t => t.SelectedGearName == vu_fg.GearText);
                                                }
                                                else
                                                {
                                                    egr = landing.GearsEfforts.FirstOrDefault(t => t.SelectedGearCode == vu_fg.GearCode);
                                                }

                                                if (!MultiGearEffortSpec.RowIDSet)
                                                {
                                                    MultiGearEffortSpec.SetRowIDs();
                                                }


                                                foreach (var gear_effortspec in egr.EffortsSpecsOfGear)
                                                {
                                                    VesselUnload_Gear_Spec vu_gs = new VesselUnload_Gear_Spec
                                                    {
                                                        EffortSpecID = gear_effortspec.EffortType,
                                                        EffortValueNumeric = gear_effortspec.EffortNumericValue,
                                                        EffortValueText = gear_effortspec.EffortDescription,
                                                        Parent = vu_fg,
                                                        //RowID = (int)gear_effortspec.PK,
                                                        RowID = VesselEffortViewModel.CurrentIDNumber + 1,
                                                        DelayedSave = DelayedSave
                                                    };

                                                    if (vu_fg.VesselUnload_Gear_Specs_ViewModel.AddRecordToRepo(vu_gs))
                                                    {
                                                        VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = vu_gs.RowID;
                                                        VesselEffortViewModel.CurrentIDNumber = vu_gs.RowID;
                                                        vu_fg.CountEffortIndicators++;
                                                    }

                                                }

                                                vu_fg.VesselUnload_Gear_Specs_ViewModel.Dispose();

                                            }
                                        }
                                        vu.VesselUnload_FishingGearsViewModel.Dispose();
                                    }

                                    else if (landing.SingleGearEffortSpecs != null)
                                    {
                                        vu.VesselEffortViewModel = new VesselEffortViewModel(isNew: true);
                                        vu.CountEffortIndicators = landing.SingleGearEffortSpecs.Count;
                                        if (!EffortSpecSingleGear.RowIDSet)
                                        {
                                            EffortSpecSingleGear.SetRowIDs();
                                        }
                                        foreach (var effort in landing.SingleGearEffortSpecs
                                            .Where(t => t.Parent.PK == landing.PK))
                                        {
                                            VesselEffort ve = new VesselEffort
                                            {
                                                PK = VesselEffortViewModel.CurrentIDNumber + 1,
                                                Parent = vu,
                                                VesselUnloadID = vu.PK,
                                                EffortSpecID = effort.EffortType,
                                                EffortValueNumeric = effort.EffortIntensity,
                                                EffortValueText = effort.EffortDescription,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.VesselEffortViewModel.AddRecordToRepo(ve))
                                            {
                                                VesselEffortViewModel.CurrentIDNumber = ve.PK;
                                                VesselUnload_Gear_Spec_ViewModel.CurrentIDNumber = ve.PK;
                                            }

                                        }
                                        vu.VesselEffortViewModel.Dispose();
                                    }

                                    if (landing.GearSoakTimes != null)
                                    {
                                        vu.GearSoakViewModel = new GearSoakViewModel(isNew: true);
                                        vu.CountGearSoak = landing.GearSoakTimes.Count;
                                        if (!SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.RowIDSet)
                                        {
                                            SoakTimeGroupSoaktimeTrackingGroupSoakTimeRepeat.SetRowIDs();
                                        }
                                        foreach (var soak in landing.GearSoakTimes
                                            .Where(t => t.Parent.PK == landing.PK))
                                        {
                                            GearSoak gs = new GearSoak
                                            {
                                                //PK = (int)soak.PK,
                                                PK = GearSoakViewModel.CurrentIDNumber + 1,
                                                Parent = vu,
                                                VesselUnloadID = vu.PK,
                                                TimeAtSet = soak.SetTime,
                                                TimeAtHaul = soak.HaulTime,
                                                WaypointAtSet = soak.WaypointAtSet,
                                                WaypointAtHaul = soak.WaypointAtHaul,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.GearSoakViewModel.AddRecordToRepo(gs))
                                            {
                                                GearSoakViewModel.CurrentIDNumber = gs.PK;
                                            }
                                        }
                                        vu.GearSoakViewModel.Dispose();
                                    }

                                    if (landing.GridCoordinates != null)
                                    {
                                        vu.FishingGroundGridViewModel = new FishingGroundGridViewModel(isNew: true);
                                        vu.CountGrids = landing.GridCoordinates.Count;
                                        if (!GridCoordGroupBingoRepeat.RowIDSet)
                                        {
                                            GridCoordGroupBingoRepeat.SetRowIDs();
                                        }
                                        foreach (var gr in landing.GridCoordinates
                                             .Where(t => t.Parent.PK == landing.PK))
                                        {
                                            FishingGroundGrid fgg = new FishingGroundGrid
                                            {
                                                //PK = (int)gr.PK,
                                                PK = FishingGroundGridViewModel.CurrentIDNumber + 1,
                                                Parent = vu,
                                                VesselUnloadID = vu.PK,
                                                UTMZoneText = gr.Parent.UTMZone,
                                                Grid = gr.CompleteGridName,
                                                DelayedSave = DelayedSave
                                            };
                                            if (vu.FishingGroundGridViewModel.AddRecordToRepo(fgg))
                                            {
                                                FishingGroundGridViewModel.CurrentIDNumber = fgg.PK;
                                            }
                                        }
                                        vu.FishingGroundGridViewModel.Dispose();
                                    }

                                    if (landing.CatchComposition != null)
                                    {
                                        int missingCatchInfoCounter = 0;
                                        vu.VesselCatchViewModel = new VesselCatchViewModel(isNew: true);
                                        vu.CountCatchCompositionItems = landing.CatchComposition.Count;
                                        if (!CatchCompGroupCatchCompositionRepeat.RowIDSet)
                                        {
                                            CatchCompGroupCatchCompositionRepeat.SetRowIDs();
                                        }

                                        foreach (var catchComp in landing.CatchComposition
                                            .Where(t => t.Parent.PK == landing.PK))
                                        {

                                            if (catchComp.SpeciesID != null || !string.IsNullOrEmpty(catchComp.SpeciesNameOther))
                                            {
                                                bool fromTotal = false;

                                                if (isVersion643)
                                                {
                                                    if (vu.WeightOfCatchSample == null)
                                                    {
                                                        fromTotal = true;
                                                    }
                                                    else
                                                    {
                                                        fromTotal = catchComp.FromTotalCatch;
                                                    }
                                                }
                                                else
                                                {
                                                    fromTotal = false;
                                                }


                                                VesselCatch vc = new VesselCatch
                                                {
                                                    //PK = catchComp.PK,
                                                    PK = VesselCatchViewModel.CurrentIDNumber + 1,
                                                    Parent = vu,
                                                    VesselUnloadID = vu.PK,
                                                    SpeciesID = catchComp.CatchCompSpeciesID,
                                                    Catch_kg = catchComp.SpeciesWt,
                                                    Sample_kg = catchComp.SpeciesSampleWt,
                                                    TaxaCode = catchComp.TaxaCode,
                                                    SpeciesText = catchComp.SpeciesNameOther,
                                                    DelayedSave = DelayedSave,
                                                    WeighingUnit = catchComp.IndividualWeightUnit,
                                                    FromTotalCatch = fromTotal,
                                                    PriceOfSpecies = catchComp.PriceOfSpecies,
                                                    PriceUnit = catchComp.PriceUnitText
                                                };

                                                if (landing.IsMultiGear)
                                                {
                                                    vc.GearCode = catchComp.Gear_species_code;
                                                    vc.GearText = catchComp.Gear_species_name;
                                                }


                                                if (vu.VesselCatchViewModel.AddRecordToRepo(vc))
                                                {
                                                    VesselCatchViewModel.CurrentIDNumber = vc.PK;

                                                    if (catchComp.LenFreqRepeat != null)
                                                    {
                                                        vc.CatchLenFreqViewModel = new CatchLenFreqViewModel(isNew: true);
                                                        vu.CountLenFreqRows += catchComp.LenFreqRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatLengthFreqRepeat.SetRowIDs();
                                                        }
                                                        foreach (var lf in catchComp.LenFreqRepeat
                                                            .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchLenFreq clf = new CatchLenFreq
                                                            {
                                                                //PK = (int)lf.PK,
                                                                PK = CatchLenFreqViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                LengthClass = lf.LengthClass,
                                                                Frequency = lf.Frequency,
                                                                Sex = lf.Sex,
                                                                DelayedSave = DelayedSave
                                                            };
                                                            if (vc.CatchLenFreqViewModel.AddRecordToRepo(clf))
                                                            {
                                                                CatchLenFreqViewModel.CurrentIDNumber = clf.PK;
                                                            }
                                                        }
                                                        vc.CatchLenFreqViewModel.Dispose();
                                                    }

                                                    if (catchComp.LenWtRepeat != null)
                                                    {
                                                        vc.CatchLengthWeightViewModel = new CatchLengthWeightViewModel(isNew: true);
                                                        vu.CountLenWtRows += catchComp.LenWtRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatLenWtRepeat.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatLenWtRepeat.SetRowIDs();
                                                        }
                                                        foreach (var lw in catchComp.LenWtRepeat
                                                             .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchLengthWeight clw = new CatchLengthWeight
                                                            {
                                                                //PK = (int)lw.PK,
                                                                PK = CatchLengthWeightViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                Length = lw.Length,
                                                                Weight = lw.Weight,
                                                                Sex = lw.Sex,
                                                                DelayedSave = DelayedSave
                                                            };
                                                            if (vc.CatchLengthWeightViewModel.AddRecordToRepo(clw))
                                                            {
                                                                CatchLengthWeightViewModel.CurrentIDNumber = clw.PK;
                                                            }

                                                        }
                                                        vc.CatchLengthWeightViewModel.Dispose();
                                                    }

                                                    if (catchComp.LengthListRepeat != null)
                                                    {
                                                        vc.CatchLengthViewModel = new CatchLengthViewModel(isNew: true);
                                                        vu.CountLengthRows += catchComp.LengthListRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatLengthListRepeat.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatLengthListRepeat.SetRowIDs();
                                                        }
                                                        foreach (var l in catchComp.LengthListRepeat
                                                             .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchLength cl = new CatchLength
                                                            {
                                                                //PK = (int)l.PK,
                                                                PK = CatchLengthViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                Length = l.Length,
                                                                Sex = l.Sex,
                                                                DelayedSave = DelayedSave

                                                            };
                                                            if (vc.CatchLengthViewModel.AddRecordToRepo(cl))
                                                            {
                                                                CatchLengthViewModel.CurrentIDNumber = cl.PK;
                                                            }
                                                        }
                                                        vc.CatchLengthViewModel.Dispose();
                                                    }

                                                    if (catchComp.GMSRepeat != null)
                                                    {
                                                        vc.CatchMaturityViewModel = new CatchMaturityViewModel(isNew: false);
                                                        vu.CountMaturityRows += catchComp.GMSRepeat.Count;
                                                        if (!CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.RowIDSet)
                                                        {
                                                            CatchCompGroupCatchCompositionRepeatGmsRepeatGroup.SetRowIDs();
                                                        }
                                                        foreach (var m in catchComp.GMSRepeat
                                                             .Where(t => t.Parent.PK == catchComp.PK))
                                                        {
                                                            CatchMaturity cm = new CatchMaturity
                                                            {
                                                                //PK = (int)m.PK,
                                                                PK = CatchMaturityViewModel.CurrentIDNumber + 1,
                                                                Parent = vc,
                                                                VesselCatchID = vc.PK,
                                                                Length = m.Length,
                                                                Weight = m.Weight,
                                                                SexCode = m.SexCode,
                                                                MaturityCode = m.GMSCode,
                                                                WeightGutContent = m.StomachContentWt,
                                                                GutContentCode = m.GutContentCategoryCode,
                                                                GonadWeight = m.GonadWeight,
                                                                DelayedSave = DelayedSave
                                                            };
                                                            if (cm.GonadWeight != null)
                                                            {

                                                            }
                                                            if (vc.CatchMaturityViewModel.AddRecordToRepo(cm))
                                                            {
                                                                CatchMaturityViewModel.CurrentIDNumber = cm.PK;
                                                            }
                                                        }
                                                        vc.CatchMaturityViewModel.Dispose();
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                missingCatchInfoCounter++;
                                            }
                                        }


                                        if (missingCatchInfoCounter > 0)
                                        {
                                            Utilities.Logger.LogMissingCatchInfo($@"""{vu.ODKRowID}"",{missingCatchInfoCounter}, ""{vu.XFormIdentifier}"",""{vu.FormVersion}"",""{vu.Parent.GearUsedName}"",""{vu.EnumeratorName}"",""{vu.Parent.Parent.LandingSiteName}"",{vu.SamplingDate},""{System.IO.Path.GetFileName(CurrentJSONFileName)}"",{DateTime.Now}");
                                        }
                                    }


                                    if (gear_unload.VesselUnloadViewModel.UpdateUnloadStats(vu) && NSAPEntities.SummaryItemViewModel.AddRecordToRepo(vu))
                                    {
                                        if (landing.IncludeCatchComp)
                                        {
                                            gear_unload.VesselUnloadViewModel.UpdateWeightValidation(NSAPEntities.SummaryItemViewModel.CurrentEntity, vu);
                                            vu.VesselCatchViewModel.Dispose();
                                        }
                                        savedCount++;
                                        landing.SavedInLocalDatabase = true;
                                        UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadSavedCount = savedCount, Intent = UploadToDBIntent.Uploading });
                                        TotalUploadCount++;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.Logger.Log(ex);
                        }
                    }
                    else
                    {
                        UploadInProgress = false;
                        UpdateInProgress = false;
                        break;
                    }
                }
            }
            if (CancelUpload)
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { Intent = UploadToDBIntent.Cancelled });
            }
            else
            {
                UploadSubmissionToDB?.Invoke(null, new UploadToDbEventArg { VesselUnloadTotalSavedCount = savedCount, Intent = UploadToDBIntent.EndOfUpload });
                if (resolvedLandings == null)
                {
                    _unrecognizedFishingGrounds = new List<UnrecognizedFishingGround>();
                }
                else
                {
                    ResolvedLandingsFromUnrecognizedFishingGrounds = resolvedLandings;
                }
            }
            UploadInProgress = false;
            UpdateInProgress = false;

            return savedCount > 0 || hasUnrecognizedFG;
        }
    }
}