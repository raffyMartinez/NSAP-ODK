using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Media_csv
{
    public class GearSelectViewModel

    {
        private GearSelectRepository GearSelects { get; set; }
        public ObservableCollection<GearSelect> GearSelectCollection { get; set; }
        public GearSelectViewModel(string patchToCSVFile)
        {
            GearSelects = new GearSelectRepository(patchToCSVFile);
            GearSelectCollection = new ObservableCollection<GearSelect>(GearSelects.GearSelects);
            GearSelectCollection.CollectionChanged += GearSelectCollection_CollectionChanged;
        }

        private void GearSelectCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }
    }

}
