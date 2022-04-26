using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
    class SampledLandingSiteViewModel
    {
        public ObservableCollection<SampledLandingSite> SampledLandingSiteCollection { get; set; }
        private SampledLandingSiteRepository SampledLandingSites { get; set; }

        public SampledLandingSiteViewModel(FishingGround fg, FMA fma, NSAPRegion reg)
        {
            SampledLandingSites = new SampledLandingSiteRepository(fg, fma, reg);
            SampledLandingSiteCollection = new ObservableCollection<SampledLandingSite>(SampledLandingSites.SampledLandingSites);
            //LandingSiteCollection.CollectionChanged += LandingSiteCollection_CollectionChanged;
        }
    }
}
