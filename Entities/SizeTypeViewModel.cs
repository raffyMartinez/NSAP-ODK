using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace NSAP_ODK.Entities
{
    public class SizeTypeViewModel
    {
        public ObservableCollection<SizeType> SizeTypeCollection { get; set; }
        private SizeTypeRepository SizeTypes{ get; set; }



        public SizeTypeViewModel()
        {
            SizeTypes = new SizeTypeRepository();
            SizeTypeCollection = new ObservableCollection<SizeType>(SizeTypes.SizeTypes);
            SizeTypeCollection.CollectionChanged += Collection_CollectionChanged;
        }
        public List<SizeType> GetAllSizeTypes()
        {
            return SizeTypeCollection.ToList();
        }

        public bool TypeCodeExists(string code)
        {
            foreach (SizeType st in SizeTypeCollection)
            {
                if (st.Code== code)
                {
                    return true;
                }
            }
            return false;
        }
        public SizeType GetSizeType(string code)
        {
            return SizeTypeCollection.FirstOrDefault(n => n.Code == code);

        }
        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        SizeTypes.Add(SizeTypeCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<SizeType> tempListOfRemovedItems = e.OldItems.OfType<SizeType>().ToList();
                        SizeTypes.Delete(tempListOfRemovedItems[0].Code);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<SizeType> tempList = e.NewItems.OfType<SizeType>().ToList();
                        SizeTypes.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return SizeTypeCollection.Count; }
        }

        public void AddRecordToRepo(SizeType s)
        {
            if (s == null)
                throw new ArgumentNullException("Error: The argument is Null");
            SizeTypeCollection.Add(s);
        }

        public void UpdateRecordInRepo(SizeType s)
        {
            if (s.Code == null)
                throw new Exception("Error: code cannot be null");

            int index = 0;
            while (index < SizeTypeCollection.Count)
            {
                if (SizeTypeCollection[index].Code == s.Code)
                {
                    SizeTypeCollection[index] = s;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(string code)
        {
            if (code == null)
                throw new Exception("Record code cannot be null");

            int index = 0;
            while (index < SizeTypeCollection.Count)
            {
                if (SizeTypeCollection[index].Code == code)
                {
                    SizeTypeCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
    }
}
