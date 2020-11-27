using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NSAP_ODK.Entities
{
    public class FMAViewModel
    {
        public ObservableCollection<FMA> FMACollection { get; set; }
        private FMARepository FMAs { get; set; }

        public int NextRecordNumber
        {
            get
            {
                if (FMACollection.Count == 0)
                {
                    return 1;
                }
                else
                {
                    return FMACollection.Max(t => t.FMAID) + 1;
                }
            }
        }

        public FMAViewModel()
        {
            FMAs = new FMARepository();
            FMACollection = new ObservableCollection<FMA>(FMAs.FMAs);
        }

        public List<FMA> GetAllFMAs()
        {
            return FMACollection.ToList();
        }

        public FMA GetFMA(int id)
        {
            return FMACollection.FirstOrDefault(n => n.FMAID == id);
        }

        public int Count
        {
            get { return FMACollection.Count; }
        }
    }
}