using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.Database
{
    [CategoryOrder("Overall", 1)]
    [CategoryOrder("Number of enumerators by region", 2)]
    public  class DatabaseEnumeratorSummary
    {
        private Dictionary<NSAPRegion, List<NSAPEnumerator>> _dictEnumeratorsByRegion;
        public void Refresh()
        {
            _dictEnumeratorsByRegion = NSAPEntities.NSAPRegionViewModel.GetEnumeratorsByRegionDictionary();
            TotalInDatabase = NSAPEntities.NSAPEnumeratorViewModel.Count;
            CountWithVesselLandingRecords = NSAPEntities.VesselUnloadViewModel.CountEnumeratorsWithUnloadRecord;
        }

        [ReadOnly(true)]
        public int TotalInDatabase { get;  set;}

        [ReadOnly(true)]
        public int CountWithVesselLandingRecords { get; set; }

        [ReadOnly(true)]
        public int CountIlocos 
        {
            get 
            {
                
                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("1")].Count;
            } 
        }

        public int CountCagayanValley
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("2")].Count;
            }
        }

        public int CountCentralLuzon
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("3")].Count;
            }
        }

        public int CountCalabarzon
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("4a")].Count;
            }
        }

        public int CountMimaropa
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("4b")].Count;
            }
        }

        public int CountBicol
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("5")].Count;
            }
        }

        public int CountWesternVisayas
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("6")].Count;
            }
        }

        public int CountCentralVisayas
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("7")].Count;
            }
        }

        public int CountEasternVisayas
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("8")].Count;
            }
        }

        public int CountZamboangaPeninsula
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("9")].Count;
            }
        }

        public int CountNorhternMindanao
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("10")].Count;
            }
        }

        public int CountDavao
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("11")].Count;
            }
        }

        public int CountSoccsksargen
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("12")].Count;
            }
        }

        public int CountCaraga
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("13")].Count;
            }
        }

        public int CountBARMM
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("BARMM")].Count;
            }
        }

        public int CountNCR
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("NCR")].Count;
            }
        }

        public int CountCar
        {
            get
            {

                return _dictEnumeratorsByRegion[NSAPEntities.NSAPRegionViewModel.GetNSAPRegion("CAR")].Count;
            }
        }

    }
}
