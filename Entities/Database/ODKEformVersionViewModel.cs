using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities.Database
{
   public class ODKEformVersionViewModel
    {
        public ObservableCollection<ODKEformVersion> ODKEformVersionCollection { get; set; }
         private ODKEformVersionRepository ODKEformVersions { get; set; }
        public ODKEformVersionViewModel()
        {
            //Refresh();
            ODKEformVersions = new ODKEformVersionRepository();
            ODKEformVersionCollection = new ObservableCollection<ODKEformVersion>();
            ODKEformVersionCollection.CollectionChanged += ODKEformVersionCollection_CollectionChanged;
        }

        public void Refresh()
        {
            
            ODKEformVersionCollection = new ObservableCollection<ODKEformVersion>(ODKEformVersions.ODKEformVersions);
        }
        private void ODKEformVersionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        //int newIndex = e.NewStartingIndex;

                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {

                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        
                    }
                    break;
            }
        }
    }
}
