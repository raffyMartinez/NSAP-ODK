using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Media_csv
{
    public class VesselSelectViewModel
    {
        private VesselSelectRepository VesselSelects { get; set; }
        public ObservableCollection<VesselSelect> VesselSelectCollection { get; set; }
        public VesselSelectViewModel(string patchToCSVFile, FisheriesSector sector)
        {
            VesselSelects = new VesselSelectRepository(patchToCSVFile, sector);
            VesselSelectCollection = new ObservableCollection<VesselSelect>(VesselSelects.VesselSelects);
            VesselSelectCollection.CollectionChanged += VesselSelectCollection_CollectionChanged;
        }

        private void VesselSelectCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }
    }
}
