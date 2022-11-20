using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Media_csv
{
    public class FMASelectViewModel
    {
        private FMASelectRepository FMASelects { get; set; }
        public ObservableCollection<FMASelect> FMASelectCollection { get; set; }
        public FMASelectViewModel(string patchToCSVFile)
        {
            FMASelects = new FMASelectRepository(patchToCSVFile);
            FMASelectCollection = new ObservableCollection<FMASelect>(FMASelects.FMASelects);
            FMASelectCollection.CollectionChanged += FMASelectCollection_CollectionChanged;
        }

        private void FMASelectCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }
    }
}
