using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Media_csv
{
    public class LSSelectViewModel
    {
        private LSSelectRepository LSSelects { get; set; }
        public ObservableCollection<LSSelect> LSSelectCollection { get; set; }
        public LSSelectViewModel(string patchToCSVFile)
        {
            LSSelects = new LSSelectRepository(patchToCSVFile);
            LSSelectCollection = new ObservableCollection<LSSelect>(LSSelects.LSSelects);
            LSSelectCollection.CollectionChanged += LSSelectCollection_CollectionChanged;
        }

        private void LSSelectCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }
    }
}
