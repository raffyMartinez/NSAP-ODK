﻿using System;
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
        private TaxaRepository Taxas { get; set; }
        private bool _editSuccess;

        public TaxaViewModel()
        {

            Taxas = new TaxaRepository();
            TaxaCollection = new ObservableCollection<Taxa>(Taxas.Taxas);
            TaxaCollection.CollectionChanged += Taxas_CollectionChanged;
            if (GetTaxa("NOID") == null)
            {
                AddRecordToRepo(new Taxa { Code = "NOID", Name = "Not identified", Description = "Taxa is not identified" });
            }
        }
        public List<Taxa> GetAllTaxa()
        {
            return TaxaCollection.ToList();
        }

        public bool AddAllToMySQL()
        {
            int count = 0;
            if (Utilities.Global.Settings.UsemySQL)
            {
                foreach (var t in TaxaCollection)
                {
                    if (Taxas.Add(t))
                    {
                        count++;
                    }
                }
            }
            return count == TaxaCollection.Count;
        }

        public string TaxaCodeFromName(string taxaName)
        {
            return TaxaCollection.FirstOrDefault(t => t.Name == taxaName).Code;
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
        public ObservableCollection<KeyValuePair<string, string>> AllTaxaTermsKV()
        {
            ObservableCollection<KeyValuePair<string, string>> list = new ObservableCollection<KeyValuePair<string, string>>();
            foreach (var t in TaxaCollection)
            {
                list.Add(new KeyValuePair<string, string>(t.Code, t.Name));
            }
            return list;
        }
        public bool TaxaName(string name)
        {
            foreach (Taxa t in TaxaCollection)
            {
                if (t.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public Taxa NotIdentified
        {
            get
            {
                return TaxaCollection.FirstOrDefault(t => t.Code == "NOID");
            }
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
            return TaxaCollection.FirstOrDefault(n => n.Code == code);

        }
        private void Taxas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = Taxas.Add(TaxaCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Taxa> tempListOfRemovedItems = e.OldItems.OfType<Taxa>().ToList();
                        _editSuccess = Taxas.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Taxa> tempList = e.NewItems.OfType<Taxa>().ToList();
                        _editSuccess = Taxas.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return TaxaCollection.Count; }
        }

        public bool AddRecordToRepo(Taxa t)
        {
            if (t == null)
                throw new ArgumentNullException("Error: The argument is Null");
            TaxaCollection.Add(t);

            return _editSuccess;
        }

        public bool UpdateRecordInRepo(Taxa t)
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
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(string code)
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
            return _editSuccess;
        }
    }
}
