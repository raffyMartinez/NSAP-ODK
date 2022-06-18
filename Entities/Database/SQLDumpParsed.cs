using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class SQLDumpParsed
    {
        private string _mysqlTableName;
        private string _accessTableName;
        private List<AccessColumn> _columns = new List<AccessColumn>();

        public string AccessTableName { get { return _accessTableName; } }
        public int Sequence { get; set; }

        public bool ForParsing { get; private set; }
        public string MySQLTableName
        {
            get { return _mysqlTableName; }
            set
            {
                Columns = new List<AccessColumn>();
                _mysqlTableName = value;
                Sequence = SetSequence(ref _accessTableName);
            }
        }

        public List<AccessColumn> Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                SetAccessColumnNames();
            }
        }

        private void SetAccessColumnNames()
        {
            if (ForParsing)
            {
                switch (_mysqlTableName)
                {
                    case "fma":
                        break;
                    case "nsap_region":
                        break;
                    case "nsap_region_fma":
                        break;
                    case "nsap_enumerators":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "enumerator_id":
                                    col.AccessColumnName = "EnumeratorID";
                                    break;
                                case "enumerator_name":
                                    col.AccessColumnName = "EnumeratorName";
                                    break;
                            }
                        }
                        break;
                    case "nsap_region_enumerator":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                                case "enumerator_id":
                                    col.AccessColumnName = "EnumeratorID";
                                    break;
                                case "region":
                                    col.AccessColumnName = "NSAPRegionCode";
                                    break;
                                case "date_start":
                                    col.AccessColumnName = "DateStart";
                                    break;
                                case "date_end":
                                    col.AccessColumnName = "DateEnd";
                                    break;
                                case "date_first_sampling":
                                    col.AccessColumnName = "DateFirstSampling";
                                    break;
                            }
                        }
                        break;
                    case "provinces":
                        break;
                    case "municipalities":
                        break;
                    case "landing_sites":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "landing_site_id":
                                    col.AccessColumnName = "LandingSiteID";
                                    break;
                                case "landing_site_name":
                                    col.AccessColumnName = "LandingSiteName";
                                    break;
                                case "municipality":
                                    col.AccessColumnName = "Municipality";
                                    break;
                                case "latitude":
                                    col.AccessColumnName = "Latitude";
                                    break;
                                case "longitude":
                                    col.AccessColumnName = "Longitude";
                                    break;
                                case "barangay":
                                    col.AccessColumnName = "Barangay";
                                    break;
                            }
                        }
                        break;
                    case "effort_specification":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "effort_specification_id":
                                    col.AccessColumnName = "EffortSpecificationID";
                                    break;
                                case "effort_specification":
                                    col.AccessColumnName = "EffortSpecification";
                                    break;
                                case "for_all_types_of_fishing":
                                    col.AccessColumnName = "ForAllTypeOfFishing";
                                    break;
                                case "value_type":
                                    col.AccessColumnName = "ValueType";
                                    break;
                            }
                        }
                        break;
                    case "gears":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "gear_code":
                                    col.AccessColumnName = "GearCode";
                                    break;
                                case "gear_name":
                                    col.AccessColumnName = "GearName";
                                    break;
                                case "generic_code":
                                    col.AccessColumnName = "GenericCode";
                                    break;
                                case "is_generic":
                                    col.AccessColumnName = "IsGeneric";
                                    break;
                            }
                        }
                        break;
                    case "gear_effort_specification":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "gear":
                                    col.AccessColumnName = "GearCode";
                                    break;
                                case "effort_spec":
                                    col.AccessColumnName = "EffortSpec";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowId";
                                    break;
                            }
                        }
                        break;
                    case "fishing_grounds":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "fishing_ground_code":
                                    col.AccessColumnName = "FishingGroundCode";
                                    break;
                                case "fishing_ground_name":
                                    col.AccessColumnName = "FishingGroundName";
                                    break;
                            }
                        }
                        break;
                    case "nsap_region_fma_fishing_grounds":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "region_fma":
                                    col.AccessColumnName = "RegionFMA";
                                    break;
                                case "fishing_ground":
                                    col.AccessColumnName = "FishingGround";
                                    break;
                                case "date_start":
                                    col.AccessColumnName = "DateStart";
                                    break;
                                case "date_end":
                                    col.AccessColumnName = "DateEnd";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                            }
                        }
                        break;
                    case "nsap_region_landing_site":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "nsap_region_fma_fg":
                                    col.AccessColumnName = "NSAPRegionFMAFishingGround";
                                    break;
                                case "landing_site":
                                    col.AccessColumnName = "LandingSiteID";
                                    break;
                                case "date_start":
                                    col.AccessColumnName = "DateStart";
                                    break;
                                case "date_end":
                                    col.AccessColumnName = "DateEnd";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                            }
                        }
                        break;
                    case "nsap_region_gear":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "nsap_region":
                                    col.AccessColumnName = "NSAPRegionCode";
                                    break;
                                case "gear":
                                    col.AccessColumnName = "GearCode";
                                    break;
                                case "date_start":
                                    col.AccessColumnName = "DateStart";
                                    break;
                                case "date_end":
                                    col.AccessColumnName = "DateEnd";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                            }
                        }
                        break;
                    case "GPS":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "gps_code":
                                    col.AccessColumnName = "GPSCode";
                                    break;
                                case "assigned_name":
                                    col.AccessColumnName = "AssignedName";
                                    break;
                                case "brand":
                                    col.AccessColumnName = "Brand";
                                    break;
                                case "model":
                                    col.AccessColumnName = "Model";
                                    break;
                                case "device_type":
                                    col.AccessColumnName = "DeviceType";
                                    break;
                            }
                        }
                        break;
                    case "engines":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "engine_id":
                                    col.AccessColumnName = "EngineID";
                                    break;
                                case "manufacturer_name":
                                    col.AccessColumnName = "ManufacturerName";
                                    break;
                                case "model_name":
                                    col.AccessColumnName = "ModelName";
                                    break;
                                case "horsepower":
                                    col.AccessColumnName = "Horsepower";
                                    break;
                            }
                        }
                        break;
                    case "fishing_vessel":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "vessel_id":
                                    col.AccessColumnName = "VesselID";
                                    break;
                                case "name_of_owner":
                                    col.AccessColumnName = "NameOfOwner";
                                    break;
                                case "vessel_name":
                                    col.AccessColumnName = "VesselName";
                                    break;
                                case "length":
                                    col.AccessColumnName = "Length";
                                    break;
                                case "depth":
                                    col.AccessColumnName = "Depth";
                                    break;
                                case "breadth":
                                    col.AccessColumnName = "Breadth";
                                    break;
                                case "registration_number":
                                    col.AccessColumnName = "RegistrationNumber";
                                    break;
                                case "sector":
                                    col.AccessColumnName = "Sector";
                                    break;
                            }
                        }
                        break;
                    case "nsap_region_vessel":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "vessel":
                                    col.AccessColumnName = "VesselID";
                                    break;
                                case "region":
                                    col.AccessColumnName = "NSAPRegionCode";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                                case "date_start":
                                    col.AccessColumnName = "DateStart";
                                    break;
                                case "date_end":
                                    col.AccessColumnName = "DateEnd";
                                    break;
                            }
                        }
                        break;
                    case "fb_species":
                        break;
                    case "ph_fish":
                        break;
                    case "taxa":
                        break;
                    case "size_types":
                        break;
                    case "not_fish_species":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "species_id":
                                    col.AccessColumnName = "SpeciesID";
                                    break;
                                case "genus":
                                    col.AccessColumnName = "Genus";
                                    break;
                                case "species":
                                    col.AccessColumnName = "Species";
                                    break;
                                case "taxa":
                                    col.AccessColumnName = "Taxa";
                                    break;
                                case "size_indicator":
                                    col.AccessColumnName = "SizeIndicator";
                                    break;
                                case "max_size":
                                    col.AccessColumnName = "MaxSize";
                                    break;
                            }
                        }
                        break;
                    case "dbo_lc_fg_sample_day":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "unload_day_id":
                                    col.AccessColumnName = "unload_day_id";
                                    break;
                                case "region_id":
                                    col.AccessColumnName = "region_id";
                                    break;
                                case "fma":
                                    col.AccessColumnName = "fma";
                                    break;
                                case "sdate":
                                    col.AccessColumnName = "sdate";
                                    break;
                                case "land_ctr_id":
                                    col.AccessColumnName = "land_ctr_id";
                                    break;
                                case "ground_id":
                                    col.AccessColumnName = "ground_id";
                                    break;
                                case "remarks":
                                    col.AccessColumnName = "remarks";
                                    break;
                                case "is_sample_day":
                                    col.AccessColumnName = "sampleday";
                                    break;
                                case "land_ctr_text":
                                    col.AccessColumnName = "land_ctr_text";
                                    break;
                            }
                        }
                        break;
                    case "dbo_lc_fg_sample_day_1":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "unload_day_id":
                                    col.AccessColumnName = "unload_day_id";
                                    break;
                                case "datetime_submitted":
                                    col.AccessColumnName = "datetime_submitted";
                                    break;
                                case "user_name":
                                    col.AccessColumnName = "user_name";
                                    break;
                                case "device_id":
                                    col.AccessColumnName = "device_id";
                                    break;
                                case "xform_identifier":
                                    col.AccessColumnName = "XFormIdentifier";
                                    break;
                                case "date_added":
                                    col.AccessColumnName = "DateAdded";
                                    break;
                                case "from_excel_download":
                                    col.AccessColumnName = "FromExcelDownload";
                                    break;
                                case "form_version":
                                    col.AccessColumnName = "form_version";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                                case "enumerator_id":
                                    col.AccessColumnName = "EnumeratorID";
                                    break;
                                case "enumerator_text":
                                    col.AccessColumnName = "EnumeratorText";
                                    break;
                            }
                        }
                        break;
                    case "dbo_gear_unload":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "unload_gr_id":
                                    col.AccessColumnName = "unload_gr_id";
                                    break;
                                case "unload_day_id":
                                    col.AccessColumnName = "unload_day_id";
                                    break;
                                case "gr_id":
                                    col.AccessColumnName = "gr_id";
                                    break;
                                case "boats":
                                    col.AccessColumnName = "boats";
                                    break;
                                case "catch":
                                    col.AccessColumnName = "catch";
                                    break;
                                case "gr_text":
                                    col.AccessColumnName = "gr_text";
                                    break;
                                case "remarks":
                                    col.AccessColumnName = "remarks";
                                    break;
                            }
                        }
                        break;
                    case "dbo_vessel_unload":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "unload_gr_id":
                                    col.AccessColumnName = "unload_gr_id";
                                    break;
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "boat_id":
                                    col.AccessColumnName = "boat_id";
                                    break;
                                case "catch_total":
                                    col.AccessColumnName = "catch_total";
                                    break;
                                case "catch_samp":
                                    col.AccessColumnName = "catch_samp";
                                    break;
                                case "boxes_total":
                                    col.AccessColumnName = "boxes_total";
                                    break;
                                case "boxes_samp":
                                    col.AccessColumnName = "boxes_samp";
                                    break;
                                case "boat_text":
                                    col.AccessColumnName = "boat_text";
                                    break;
                                case "is_boat_used":
                                    col.AccessColumnName = "is_boat_used";
                                    break;
                                case "raising_factor":
                                    col.AccessColumnName = "raising_factor";
                                    break;
                            }
                        }
                        break;
                    case "dbo_vessel_unload_1":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "success":
                                    col.AccessColumnName = "Success";
                                    break;
                                case "tracked":
                                    col.AccessColumnName = "Tracked";
                                    break;
                                case "trip_is_completed":
                                    col.AccessColumnName = "trip_is_completed";
                                    break;
                                case "departure_landing_site":
                                    col.AccessColumnName = "DepartureLandingSite";
                                    break;
                                case "arrival_landing_site":
                                    col.AccessColumnName = "ArrivalLandingSite";
                                    break;
                                case "sector_code":
                                    col.AccessColumnName = "sector_code";
                                    break;
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                                case "xform_identifier":
                                    col.AccessColumnName = "XFormIdentifier";
                                    break;
                                case "xform_date":
                                    col.AccessColumnName = "XFormDate";
                                    break;

                                case "user_name":
                                    col.AccessColumnName = "user_name";
                                    break;
                                case "device_id":
                                    col.AccessColumnName = "device_id";
                                    break;
                                case "datetime_submitted":
                                    col.AccessColumnName = "datetime_submitted";
                                    break;
                                case "form_version":
                                    col.AccessColumnName = "form_version";
                                    break;
                                case "gps":
                                    col.AccessColumnName = "GPS";
                                    break;
                                case "sampling_date":
                                    col.AccessColumnName = "SamplingDate";
                                    break;
                                case "notes":
                                    col.AccessColumnName = "Notes";
                                    break;
                                case "enumerator_id":
                                    col.AccessColumnName = "EnumeratorID";
                                    break;
                                case "enumerator_text":
                                    col.AccessColumnName = "EnumeratorText";
                                    break;
                                case "date_added":
                                    col.AccessColumnName = "DateAdded";
                                    break;

                                case "from_excel_download":
                                    col.AccessColumnName = "FromExcelDownload";
                                    break;
                                case "has_catch_composition":
                                    col.AccessColumnName = "HasCatchComposition";
                                    break;
                                case "number_of_fishers":
                                    col.AccessColumnName = "NumberOfFishers";
                                    break;
                            }
                        }
                        break;
                    case "dbo_fg_grid":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "fg_grid_id":
                                    col.AccessColumnName = "fg_grid_id";
                                    break;
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "utm_zone":
                                    col.AccessColumnName = "utm_zone";
                                    break;
                                case "grid25":
                                    col.AccessColumnName = "grid25";
                                    break;
                            }
                        }
                        break;
                    case "dbo_gear_soak":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "gear_soak_id":
                                    col.AccessColumnName = "gear_soak_id";
                                    break;
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "time_set":
                                    col.AccessColumnName = "time_set";
                                    break;
                                case "time_hauled":
                                    col.AccessColumnName = "time_hauled";
                                    break;
                                case "wpt_set":
                                    col.AccessColumnName = "wpt_set";
                                    break;
                                case "wpt_haul":
                                    col.AccessColumnName = "wpt_haul";
                                    break;
                            }
                        }
                        break;
                    case "dbo_vessel_effort":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "effort_row_id":
                                    col.AccessColumnName = "effort_row_id";
                                    break;
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "effort_spec_id":
                                    col.AccessColumnName = "effort_spec_id";
                                    break;
                                case "effort_value_numeric":
                                    col.AccessColumnName = "effort_value_numeric";
                                    break;
                                case "effort_value_text":
                                    col.AccessColumnName = "effort_value_text";
                                    break;
                            }
                        }
                        break;
                    case "dbo_vessel_catch":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "catch_id":
                                    col.AccessColumnName = "catch_id";
                                    break;
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "species_id":
                                    col.AccessColumnName = "species_id";
                                    break;
                                case "catch_kg":
                                    col.AccessColumnName = "catch_kg";
                                    break;
                                case "samp_kg":
                                    col.AccessColumnName = "samp_kg";
                                    break;
                                case "taxa":
                                    col.AccessColumnName = "taxa";
                                    break;
                                case "species_text":
                                    col.AccessColumnName = "species_text";
                                    break;
                            }
                        }
                        break;
                    case "dbo_catch_length":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "catch_len_id":
                                    col.AccessColumnName = "catch_len_id";
                                    break;
                                case "catch_id":
                                    col.AccessColumnName = "catch_id";
                                    break;
                                case "length":
                                    col.AccessColumnName = "length";
                                    break;
                            }
                        }
                        break;
                    case "dbo_catch_maturity":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "catch_maturity_id":
                                    col.AccessColumnName = "catch_maturity_id";
                                    break;
                                case "catch_id":
                                    col.AccessColumnName = "catch_id";
                                    break;
                                case "length":
                                    col.AccessColumnName = "length";
                                    break;
                                case "weight":
                                    col.AccessColumnName = "weight";
                                    break;
                                case "sex":
                                    col.AccessColumnName = "sex";
                                    break;
                                case "maturity":
                                    col.AccessColumnName = "maturity";
                                    break;
                                case "gut_content_wt":
                                    col.AccessColumnName = "gut_content_wt";
                                    break;
                                case "gut_content_class":
                                    col.AccessColumnName = "gut_content_code";
                                    break;
                                case "gonad_wt":
                                    col.AccessColumnName = "gonadWt";
                                    break;
                            }
                        }
                        break;
                    case "dbo_catch_len_freq":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "catch_lf_id":
                                    col.AccessColumnName = "catch_len_freq_id";
                                    break;
                                case "catch_id":
                                    col.AccessColumnName = "catch_id";
                                    break;
                                case "length":
                                    col.AccessColumnName = "len_class";
                                    break;
                                case "freq":
                                    col.AccessColumnName = "freq";
                                    break;
                            }
                        }
                        break;
                    case "dbo_catch_len_wt":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "catch_lw_id":
                                    col.AccessColumnName = "catch_len_wt_id";
                                    break;
                                case "catch_id":
                                    col.AccessColumnName = "catch_id";
                                    break;
                                case "length":
                                    col.AccessColumnName = "length";
                                    break;
                                case "weight":
                                    col.AccessColumnName = "weight";
                                    break;
                            }
                        }
                        break; ;
                    case "json_file":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "row_id":
                                    col.AccessColumnName = "RowID";
                                    break;
                                case "filename":
                                    col.AccessColumnName = "FileName";
                                    break;
                                case "count":
                                    col.AccessColumnName = "Count";
                                    break;
                                case "earliest_date":
                                    col.AccessColumnName = "EarliestDate";
                                    break;
                                case "latest_date":
                                    col.AccessColumnName = "LatestDate";
                                    break;
                                case "date_added":
                                    col.AccessColumnName = "DateAdded";
                                    break;
                                case "md5":
                                    col.AccessColumnName = "MD5";
                                    break;
                                case "form_id":
                                    col.AccessColumnName = "FormID";
                                    break;
                                case "description":
                                    col.AccessColumnName = "Description";
                                    break;
                            }
                        }
                        break;
                    case "dbo_vessel_unload_stats":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "v_unload_id":
                                    col.AccessColumnName = "v_unload_id";
                                    break;
                                case "count_effort":
                                    col.AccessColumnName = "count_effort";
                                    break;
                                case "count_grid":
                                    col.AccessColumnName = "count_grid";
                                    break;
                                case "count_soak":
                                    col.AccessColumnName = "count_soak";
                                    break;
                                case "count_catch_composition":
                                    col.AccessColumnName = "count_catch_composition";
                                    break;
                                case "count_lengths":
                                    col.AccessColumnName = "count_lengths";
                                    break;
                                case "count_lenfreq":
                                    col.AccessColumnName = "count_lenfreq";
                                    break;
                                case "count_lenwt":
                                    col.AccessColumnName = "count_lenwt";
                                    break;
                                case "count_maturity":
                                    col.AccessColumnName = "count_maturity";
                                    break;
                            }
                        }
                        break; 
                    case "kobo_servers":
                        foreach (AccessColumn col in Columns)
                        {
                            switch (col.MySQLColumnName)
                            {
                                case "server_numeric_id":
                                    col.AccessColumnName = "server_numeric_id";
                                    break;
                                case "form_name":
                                    col.AccessColumnName = "form_name";
                                    break;
                                case "server_id":
                                    col.AccessColumnName = "server_id";
                                    break;
                                case "owner":
                                    col.AccessColumnName = "owner";
                                    break;
                                case "form_version":
                                    col.AccessColumnName = "form_version";
                                    break;
                                case "e_form_version":
                                    col.AccessColumnName = "e_form_version";
                                    break;
                                case "date_created":
                                    col.AccessColumnName = "date_created";
                                    break;
                                case "date_modified":
                                    col.AccessColumnName = "date_modified";
                                    break;
                                case "date_last_submission":
                                    col.AccessColumnName = "date_last_submission";
                                    break;

                                case "submission_count":
                                    col.AccessColumnName = "submission_count";
                                    break;
                                case "user_count":
                                    col.AccessColumnName = "user_count";
                                    break;
                                case "date_last_accessed":
                                    col.AccessColumnName = "date_last_accessed";
                                    break;
                                case "saved_in_db_count":
                                    col.AccessColumnName = "saved_in_db_count";
                                    break;
                                case "last_uploaded_json":
                                    col.AccessColumnName = "last_uploaded_json";
                                    break;
                                case "last_created_json":
                                    col.AccessColumnName = "last_created_json";
                                    break;
                            }
                        }
                        break;
                }
            }
        }
        private int SetSequence(ref string accessTableName)
        {
            int seq = 0;
            switch (_mysqlTableName)
            {
                case "fma":
                    accessTableName = "fma";
                    seq = 1;
                    ForParsing = false;
                    break;
                case "nsap_region":
                    accessTableName = "nsapRegion";
                    seq = 2;
                    ForParsing = false;
                    break;
                case "nsap_region_fma":
                    accessTableName = "NSAPRegionFMA";
                    seq = 3;
                    ForParsing = false;
                    break;
                case "nsap_enumerators":
                    accessTableName = "NSAPEnumerator";
                    seq = 4;
                    ForParsing = true;
                    break;
                case "nsap_region_enumerator":
                    accessTableName = "NSAPRegionEnumerator";
                    seq = 5;
                    ForParsing = true;
                    break;
                case "provinces":
                    accessTableName = "Provinces";
                    seq = 6;
                    ForParsing = false;
                    break;
                case "municipalities":
                    accessTableName = "Municipalities";
                    seq = 7;
                    ForParsing = false;
                    break;
                case "landing_sites":
                    accessTableName = "landingSite";
                    seq = 8;
                    ForParsing = true;
                    break;
                case "effort_specification":
                    accessTableName = "effortSpecification";
                    seq = 9;
                    ForParsing = true;
                    break;
                case "gears":
                    accessTableName = "gear";
                    seq = 10;
                    ForParsing = true;
                    break;
                case "gear_effort_specification":
                    accessTableName = "GearEffortSpecification";
                    seq = 11;
                    ForParsing = true;
                    break;
                case "fishing_grounds":
                    accessTableName = "fishingGround";
                    seq = 12;
                    ForParsing = true;
                    break;
                case "nsap_region_fma_fishing_grounds":
                    accessTableName = "NSAPRegionFMAFishingGrounds";
                    seq = 13;
                    ForParsing = true;
                    break;
                case "nsap_region_landing_site":
                    accessTableName = "NSAPRegionLandingSite";
                    seq = 14;
                    ForParsing = true;
                    break;
                case "nsap_region_gear":
                    accessTableName = "NSAPRegionGear";
                    seq = 15;
                    ForParsing = true;
                    break;
                case "GPS":
                    accessTableName = "gps";
                    seq = 16;
                    ForParsing = true;
                    break;
                case "engines":
                    accessTableName = "engine";
                    seq = 17;
                    ForParsing = true;
                    break;
                case "fishing_vessel":
                    accessTableName = "fishingVessel";
                    seq = 18;
                    ForParsing = true;
                    break;
                case "nsap_region_vessel":
                    accessTableName = "NSAPRegionVessel";
                    seq = 19;
                    ForParsing = true;
                    break;
                case "fb_species":
                    accessTableName = "FBSpecies";
                    seq = 20;
                    ForParsing = false;
                    break;
                case "ph_fish":
                    accessTableName = "phFish";
                    seq = 21;
                    ForParsing = false;
                    break;
                case "taxa":
                    accessTableName = "taxa";
                    seq = 22;
                    ForParsing = false;
                    break;
                case "size_types":
                    accessTableName = "sizeTypes";
                    seq = 23;
                    ForParsing = false;
                    break;
                case "not_fish_species":
                    accessTableName = "notFishSpecies";
                    seq = 24;
                    ForParsing = true;
                    break;
                case "dbo_lc_fg_sample_day":
                    accessTableName = "dbo_LC_FG_sample_day";
                    seq = 25;
                    ForParsing = true;
                    break;
                case "dbo_lc_fg_sample_day_1":
                    accessTableName = "dbo_LC_FG_sample_day_1";
                    seq = 26;
                    ForParsing = true;
                    break;
                case "dbo_gear_unload":
                    accessTableName = "dbo_gear_unload";
                    seq = 27;
                    ForParsing = true;
                    break;
                case "dbo_vessel_unload":
                    accessTableName = "dbo_vessel_unload";
                    seq = 28;
                    ForParsing = true;
                    break;
                case "dbo_vessel_unload_1":
                    accessTableName = "dbo_vessel_unload_1";
                    seq = 29;
                    ForParsing = true;
                    break;
                case "dbo_fg_grid":
                    accessTableName = "dbo_fg_grid";
                    seq = 30;
                    ForParsing = true;
                    break;
                case "dbo_gear_soak":
                    accessTableName = "dbo_gear_soak";
                    seq = 31;
                    ForParsing = true;
                    break;
                case "dbo_vessel_effort":
                    accessTableName = "dbo_vessel_effort";
                    seq = 32;
                    ForParsing = true;
                    break;
                case "dbo_vessel_catch":
                    accessTableName = "dbo_vessel_catch";
                    seq = 33;
                    ForParsing = true;
                    break;
                case "dbo_catch_length":
                    accessTableName = "dbo_catch_len";
                    seq = 34;
                    ForParsing = true;
                    break;
                case "dbo_catch_maturity":
                    accessTableName = "dbo_catch_maturity";
                    seq = 35;
                    ForParsing = true;
                    break;
                case "dbo_catch_len_freq":
                    accessTableName = "dbo_catch_len_freq";
                    seq = 36;
                    ForParsing = true;
                    break;
                case "dbo_catch_len_wt":
                    accessTableName = "dbo_catch_len_wt";
                    seq = 37;
                    ForParsing = true;
                    break;
                case "json_file":
                    accessTableName = "JSONFile";
                    seq = 38;
                    ForParsing = true;
                    break;
                case "dbo_vessel_unload_stats":
                    accessTableName = "dbo_vessel_unload_stats";
                    seq = 39;
                    ForParsing = true;
                    break;
                case "kobo_servers":
                    accessTableName = "kobo_servers";
                    seq = 40;
                    ForParsing = true;
                    break;
            }
            return seq;
        }


        public List<string> DataLines { get; set; }

        public override string ToString()
        {
            return $"{MySQLTableName} Datalines:{DataLines.Count}";
        }

    }
}
