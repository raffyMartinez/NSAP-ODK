using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities
{
    public class TaxaViewModel
    {
        public ObservableCollection<Taxa> TaxaCollection { get; set; }
        private TaxaRepository Taxas{ get; set; }


        public TaxaViewModel()
        {
            
            Taxas = new TaxaRepository();
            TaxaCollection = new ObservableCollection<Taxa>(Taxas.Taxas);
            TaxaCollection.CollectionChanged += Taxas_CollectionChanged;
        }
        public List<Taxa> GetAllTaxa()
        {
            return TaxaCollection.ToList();
        }

        public ObservableCollection<string> AllTaxaTerms()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            foreach (var t in TaxaCollection)
            {
                list.Add(t.Name);
            }
            return list;
        }
        public ObservableCollection<KeyValuePair<string,string>>AllTaxaTermsKV()
        {
            ObservableCollection<KeyValuePair<string,string>> list = new ObservableCollection<KeyValuePair<string,string>>();
            foreach(var t in TaxaCollection)
            {
                list.Add(new KeyValuePair<string,string>(t.Code,t.Name));
            }
            return list;
        }
        public bool TaxaName(string name)
        {
            foreach (Taxa t in TaxaCollection)
            {
                if (t.Name==name)
                {
                    return true;
                }
            }
            return false;
        }

        public Taxa FishTaxa
        {
            get
            {
                return TaxaCollection.Where(t => t.Code == "FIS").FirstOrDefault();
            }
        }
        public bool TaxaCodeExist(string code)
        {
            foreach (Taxa t in TaxaCollection)
            {
                if (t.Code == code)
                {
                    return true;
                }
            }
            return false;
        }
        public Taxa GetTaxa(string code)
        {
            return TaxaCollection.FirstOrDefault(n => n.Code== code);

        }
        private void Taxas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Taxas.Add(TaxaCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Taxa> tempListOfRemovedItems = e.OldItems.OfType<Taxa>().ToList();
                        Taxas.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Taxa> tempList = e.NewItems.OfType<Taxa>().ToList();
                        Taxas.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return TaxaCollection.Count; }
        }

        public void AddRecordToRepo(Taxa t)
        {
            if (t == null)
                throw new ArgumentNullException("Error: The argument is Null");
            TaxaCollection.Add(t);
        }

        public void UpdateRecordInRepo(Taxa t)
        {
            if (t.Code == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < TaxaCollection.Count)
            {
                if (TaxaCollection[index].Code == t.Code)
                {
                    TaxaCollection[index] = t;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(string code)
        {
            if (code == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < TaxaCollection.Count)
            {
                if (TaxaCollection[index].Code == code)
                {
                    TaxaCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
