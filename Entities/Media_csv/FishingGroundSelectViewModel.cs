using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Media_csv
{
    public class FishingGroundSelectViewModel
    {
        private FishingGroundSelectRepository FishingGroundSelects { get; set; }
        public ObservableCollection<FishingGroundSelect> FishingGroundSelectCollection { get; set; }
        public FishingGroundSelectViewModel(string patchToCSVFile)
        {
            FishingGroundSelects = new FishingGroundSelectRepository(patchToCSVFile);
            FishingGroundSelectCollection = new ObservableCollection<FishingGroundSelect>(FishingGroundSelects.FishingGroundSelects);
            FishingGroundSelectCollection.CollectionChanged += FishingGroundSelectCollection_CollectionChanged;
        }

        private void FishingGroundSelectCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }
    }
}
