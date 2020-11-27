using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities
{
   public class SpeciesMultiTaxaViewModel
    {
        public ObservableCollection<SpeciesMultiTaxa> SpeciesMultiTaxaCollection{ get; set; }
        private SpeciesMultiTaxaRepository SpeciesMultiTaxaRepo { get; set; }



        public SpeciesMultiTaxaViewModel()
        {
            SpeciesMultiTaxaRepo = new SpeciesMultiTaxaRepository();
            SpeciesMultiTaxaCollection = new ObservableCollection<SpeciesMultiTaxa>(SpeciesMultiTaxaRepo.ListSpeciesMultiTaxa);
            //SpeciesMultiTaxaCollection.CollectionChanged += Collection_CollectionChanged;
        }
        public List<SpeciesMultiTaxa> GetAllList()
        {
            return SpeciesMultiTaxaCollection.ToList();
        }

        public SpeciesMultiTaxa GetSpecies(int id)
        {
            return SpeciesMultiTaxaCollection.FirstOrDefault(n => n.SpeciesID == id);

        }
    }
}
