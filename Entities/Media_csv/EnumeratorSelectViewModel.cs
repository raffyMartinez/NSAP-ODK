using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities.Media_csv
{
    public class EnumeratorSelectViewModel
    {
        private EnumeratorSelectRepository EnumeratorSelects { get; set; }
        public ObservableCollection<EnumeratorSelect> EnumeratorSelectCollection { get; set; }
        public EnumeratorSelectViewModel(string patchToCSVFile)
        {
            EnumeratorSelects = new EnumeratorSelectRepository(patchToCSVFile);
            EnumeratorSelectCollection = new ObservableCollection<EnumeratorSelect>(EnumeratorSelects.EnumeratorSelects);
            EnumeratorSelectCollection.CollectionChanged += EnumeratorSelectCollection_CollectionChanged;
        }

        private void EnumeratorSelectCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //
        }
    }
}
