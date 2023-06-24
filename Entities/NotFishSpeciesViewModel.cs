using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;

namespace NSAP_ODK.Entities
{
    public class NotFishSpeciesViewModel
    {
        private bool _editSuccess;
        public ObservableCollection<NotFishSpecies> NotFishSpeciesCollection { get; set; }
        private NotFishSpeciesRepository NotFishSpeciesRepo { get; set; }



        public NotFishSpeciesViewModel()
        {
            NotFishSpeciesRepo = new NotFishSpeciesRepository();
            NotFishSpeciesCollection = new ObservableCollection<NotFishSpecies>(NotFishSpeciesRepo.ListSpeciesNotFish);
            NotFishSpeciesCollection.CollectionChanged += Collection_CollectionChanged;
        }
        public List<NotFishSpecies> GetAllSpeices()
        {
            return NotFishSpeciesCollection.ToList();
        }


        public DataSet DataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Invertebrate sepecies");


            DataColumn dc = new DataColumn { ColumnName = "Genus", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Species", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Id", DataType = typeof(int) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Taxa", DataType = typeof(string) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Max length", DataType = typeof(double) };
            dt.Columns.Add(dc);

            dc = new DataColumn { ColumnName = "Length type", DataType = typeof(string) };
            dt.Columns.Add(dc);


            foreach (var item in NotFishSpeciesCollection)
            {
                var row = dt.NewRow();
                row["Genus"] = item.Genus;
                row["Species"] = item.Species;
                row["Id"] = item.SpeciesID;
                row["Taxa"] = item.Taxa.ToString();
                if (item.MaxSize == null)
                {
                    row["Max length"] = DBNull.Value;
                }
                else
                {
                    row["Max length"] = item.MaxSize;
                }
                if (item.SizeType == null)
                {
                    row["Length type"] = DBNull.Value;
                }
                else
                {
                    row["Length type"] = item.SizeType.ToString();
                }

                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            return ds;
        }
        public List<NotFishSpecies> GetAllSpecies(string search = "")
        {
            if (search.Length > 0)
            {
                return NotFishSpeciesCollection.Where(t => t.ToString().ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                return NotFishSpeciesCollection.OrderBy(t => t.ToString()).ToList();
            }
        }
        public List<string> GetAllGenus(Taxa taxa)
        {
            List<string> genusList = new List<string>();
            foreach (var item in NotFishSpeciesCollection.Where(t => t.Taxa.Code == taxa.Code).OrderBy(t => t.Genus).GroupBy(t => t.Genus).ToList())
            {
                genusList.Add(item.Key);
            }
            return genusList;
        }

        public bool SpeciesNameExist(string name)
        {
            foreach (NotFishSpecies nfs in NotFishSpeciesCollection)
            {
                if (!string.IsNullOrEmpty(nfs.Name) && nfs.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public bool SpeciesNameExist(string genus, string species)
        {
            foreach (NotFishSpecies nfs in NotFishSpeciesCollection)
            {
                if (nfs.Genus == genus && nfs.Species == species)
                {
                    return true;
                }
            }
            return false;
        }

        public NotFishSpecies GetSpecies(string speciesName)
        {
            return NotFishSpeciesCollection.FirstOrDefault(t => t.ToString() == speciesName);
        }
        public NotFishSpecies GetSpecies(int id)
        {
            return NotFishSpeciesCollection.FirstOrDefault(n => n.SpeciesID == id);

        }
        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editSuccess = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        _editSuccess = NotFishSpeciesRepo.Add(NotFishSpeciesCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<NotFishSpecies> tempListOfRemovedItems = e.OldItems.OfType<NotFishSpecies>().ToList();
                        _editSuccess = NotFishSpeciesRepo.Delete(tempListOfRemovedItems[0].SpeciesID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<NotFishSpecies> tempList = e.NewItems.OfType<NotFishSpecies>().ToList();
                        _editSuccess = NotFishSpeciesRepo.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }
        public int NextRecordNumber
        {
            get
            {
                if (NotFishSpeciesCollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    //return NotFishSpeciesCollection.Max(t => t.SpeciesID) + 1;
                    return NotFishSpeciesRepo.MaxRecordNumber() + 1;
                }
            }
        }
        public int Count
        {
            get { return NotFishSpeciesCollection.Count; }
        }

        public bool AddRecordToRepo(NotFishSpecies nfs)
        {
            if (nfs == null)
                throw new ArgumentNullException("Error: The argument is Null");
            NotFishSpeciesCollection.Add(nfs);
            return _editSuccess;
        }

        public bool UpdateRecordInRepo(NotFishSpecies nfs)
        {
            if (nfs.SpeciesID == 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < NotFishSpeciesCollection.Count)
            {
                if (NotFishSpeciesCollection[index].SpeciesID == nfs.SpeciesID)
                {
                    NotFishSpeciesCollection[index] = nfs;
                    break;
                }
                index++;
            }
            return _editSuccess;
        }

        public bool DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < NotFishSpeciesCollection.Count)
            {
                if (NotFishSpeciesCollection[index].SpeciesID == id)
                {
                    NotFishSpeciesCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
            return _editSuccess;
        }
        public EntityValidationResult ValidateNonFishSpecies(NotFishSpecies species, bool isNew, string oldGenus, string oldSpecies, string oldName = "")
        {
            EntityValidationResult evr = new EntityValidationResult();

            if (isNew)
            {
                if (!string.IsNullOrEmpty(species.Name))
                {
                    if (SpeciesNameExist(species.Name))
                    {
                        evr.AddMessage("Name already used");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(species.Species) || string.IsNullOrEmpty(species.Genus))
                    {
                        evr.AddMessage("Generic and specific names must not be empty");
                    }
                    else if (isNew && SpeciesNameExist(species.Genus, species.Species))
                    {
                        evr.AddMessage("Species name already used");
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(oldName))
                {
                    if (species.Genus.Length > 0 && species.Species.Length > 0
                        && oldGenus != species.Genus && oldSpecies != species.Species
                        && SpeciesNameExist(oldGenus, oldSpecies))
                    {
                        evr.AddMessage("Species name already used");
                    }
                }
                else
                {
                    if (oldName != species.Name && SpeciesNameExist(species.Name))
                    {
                        evr.AddMessage("Name already used");
                    }
                }

            }
            //if (!string.IsNullOrEmpty(species.Name) && isNew && SpeciesNameExist(species.Name))
            //{
            //    evr.AddMessage("Name already used");
            //}
            //else if (string.IsNullOrEmpty(species.Species) || string.IsNullOrEmpty(species.Genus))
            //{
            //    evr.AddMessage("Generic and specific names must not be empty");
            //}
            //else if (isNew && SpeciesNameExist(species.Genus, species.Species))
            //{
            //    evr.AddMessage("Species name already used");
            //}

            if (species.Taxa == null)
            {
                evr.AddMessage("Taxonomic category cannot be empty");
            }




            return evr;
        }
    }
}
