using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NSAP_ODK.Entities
{
    public class EngineViewModel
    {
        public ObservableCollection<Engine> EngineCollection { get; set; }
        private EngineRepository Engines{ get; set; }



        public EngineViewModel()
        {
            Engines = new EngineRepository();
            EngineCollection = new ObservableCollection<Engine>(Engines.Engines);
            EngineCollection.CollectionChanged += EngineCollection_CollectionChanged;
        }
        public List<Engine> GetAllEngines()
        {
            return EngineCollection.ToList();
        }

        public Engine GetEngine(int id)
        {
            return EngineCollection.FirstOrDefault(n => n.EngineID == id);

        }
        private void EngineCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Engines.Add(EngineCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Engine> tempListOfRemovedItems = e.OldItems.OfType<Engine>().ToList();
                        Engines.Delete(tempListOfRemovedItems[0].EngineID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Engine> tempList = e.NewItems.OfType<Engine>().ToList();
                        Engines.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return EngineCollection.Count; }
        }

        public void AddRecordToRepo(Engine en)
        {
            if (en == null)
                throw new ArgumentNullException("Error: The argument is Null");
            EngineCollection.Add(en);
        }

        public void UpdateRecordInRepo(Engine en)
        {
            if (en.EngineID== 0)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < EngineCollection.Count)
            {
                if (EngineCollection[index].EngineID == en.EngineID)
                {
                    EngineCollection[index] = en;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < EngineCollection.Count)
            {
                if (EngineCollection[index].EngineID == id)
                {
                    EngineCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
