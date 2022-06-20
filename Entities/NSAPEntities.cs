using NSAP_ODK.Utilities;
using System.Data.OleDb;
using System.Collections.Generic;
using NSAP_ODK.Entities.Database;
using System.Threading;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{

    public enum FisheriesSector
    {
        Unknown,
        Commercial,
        Municipal,
        Aquaculture
    }
    public enum NSAPEntity
    {
        Nothing,
        FishSpecies,
        LandingSite,
        FishingGround,
        Enumerator,
        FishingVessel,
        FishingGear,
        NSAPRegion,
        NSAPRegionWithEntities,
        FMA,
        NonFishSpecies,
        EffortIndicator,
        FishingGearEffortSpecification,
        NSAPRegionFMA,
        NSAPRegionFMAFishingGround,
        NSAPRegionFMAFishingGroundLandingSite,
        NSAPRegionGear,
        NSAPRegionFishingVessel,
        NSAPRegionEnumerator,
        Province,
        Municipality,
        GPS,
        DBSummary,
        SpeciesName
    }


    public static class NSAPEntities
    {
        public static GPSViewModel GPSViewModel;
        public static EffortSpecificationViewModel EffortSpecificationViewModel;
        public static FMAViewModel FMAViewModel;
        public static NSAPRegionViewModel NSAPRegionViewModel;
        public static TaxaViewModel TaxaViewModel;
        public static SizeTypeViewModel SizeTypeViewModel;
        public static NotFishSpeciesViewModel NotFishSpeciesViewModel;
        public static FishSpeciesViewModel FishSpeciesViewModel;
        public static LandingSiteViewModel LandingSiteViewModel;
        public static GearViewModel GearViewModel;
        public static GearEffortSpecificationViewModel GearEffortSpecificationViewModel;
        public static ProvinceViewModel ProvinceViewModel;

        //public static MunicipalityViewModel MunicipalityViewModel;
        public static EngineViewModel EngineViewModel;

        public static FishingVesselViewModel FishingVesselViewModel;
        public static FishingGroundViewModel FishingGroundViewModel;
        public static NSAPEnumeratorViewModel NSAPEnumeratorViewModel;

        public static Grid25InlandLocationViewModel Grid25InlandLocationViewModel;
        public static MajorGridFMAViewModel MajorGridFMAViewModel;
        public static LandingSiteSamplingViewModel LandingSiteSamplingViewModel;
        public static GearUnloadViewModel GearUnloadViewModel;
        public static VesselUnloadViewModel VesselUnloadViewModel;
        public static VesselEffortViewModel VesselEffortViewModel;
        public static VesselCatchViewModel VesselCatchViewModel;
        public static GearSoakViewModel GearSoakViewModel;
        public static FishingGroundGridViewModel FishingGroundGridViewModel;
        public static CatchLenFreqViewModel CatchLenFreqViewModel;
        public static CatchLengthWeightViewModel CatchLengthWeightViewModel;
        public static CatchLengthViewModel CatchLengthViewModel;
        public static CatchMaturityViewModel CatchMaturityViewModel;
        public static DBSummary DBSummary;
        public static DatabaseEnumeratorSummary DatabaseEnumeratorSummary;
        public static JSONFileViewModel JSONFileViewModel;
        public static ODKEformVersionViewModel ODKEformVersionViewModel;
        public static SummaryItemViewModel SummaryItemViewModel;
        public static KoboServerViewModel KoboServerViewModel;


        static NSAPEntities()
        {
            FisheriesSector = FisheriesSector.Municipal;
            Regions = new List<string>();


        }

        public static Task<bool> ClearNSAPDatabaseTablesAsync(string otherConnectionString = "")
        {
            return Task.Run(() => ClearNSAPDatabaseTables(otherConnectionString));
        }

        public static void ClearCSVData()
        {
            CatchMaturityViewModel.ClearCSV();
            CatchLengthViewModel.ClearCSV();
            CatchLengthWeightViewModel.ClearCSV();
            CatchLenFreqViewModel.ClearCSV();
            VesselCatchViewModel.ClearCSV();
            FishingGroundGridViewModel.ClearCSV();
            GearSoakViewModel.ClearCSV();
            VesselEffortViewModel.ClearCSV();
            VesselUnloadViewModel.ClearCSV();
            GearUnloadViewModel.ClearCSV();
            LandingSiteSamplingViewModel.ClearCSV();

        }
        public static bool ClearNSAPDatabaseTables(string otherConnectionString = "")
        {
            bool success = false;
            JSONFileRepository.ClearTable(otherConnectionString);
            CatchMaturityRepository.ClearTable(otherConnectionString);
            CatchLengthRepository.ClearTable(otherConnectionString);
            CatchLenWeightRepository.ClearTable(otherConnectionString);
            CatchLenFreqRepository.ClearTable(otherConnectionString);
            VesselCatchRepository.ClearTable(otherConnectionString);
            FishingGroundGridRepository.ClearTable(otherConnectionString);
            GearSoakRepository.ClearTable(otherConnectionString);
            VesselEffortRepository.ClearTable(otherConnectionString);
            VesselUnloadRepository.ClearTable(otherConnectionString);
            GearUnloadRepository.ClearTable(otherConnectionString);
            if (LandingSiteSamplingRepository.ClearTable(otherConnectionString))
            {
                success = true;
            }
            return success;
        }
        public static NSAPRegion NSAPRegion { get; set; }

        public static NSAPRegionFMA NSAPRegionFMA { get; set; }
        public static NSAPRegionFMAFishingGround NSAPRegionFMAFishingGround { get; set; }

        public static int? MunicipalityID { get; set; }
        public static int? ProvinceID { get; set; }

        public static FisheriesSector FisheriesSector { get; set; }

        public static List<string> Regions { get; set; }
        public static NSAPEntity EntityToRefresh { get; set; }
        //public static int GetMaxItemSetID()
        //{
        //    int maxRowID = 0;

        //    using (var conn = new OleDbConnection(Global.ConnectionString))
        //    {
        //        conn.Open();
        //        const string sql = "SELECT Max(itemsets.name) AS [max row number] FROM itemsets";

        //        using (OleDbCommand getCount = new OleDbCommand(sql, conn))
        //        {
        //            maxRowID = (int)getCount.ExecuteScalar();
        //        }
        //    }

        //    return maxRowID;
        //}
    }
}