using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace NSAP_ODK.Entities
{
    public class MajorGridFMAViewModel
    {
            
        public ObservableCollection<MajorGridFMA> MajorGridFMACollection { get; set; }
        private MajorGridFMARepository MajorGridFMAs { get; set; }



        public MajorGridFMAViewModel()
        {
            MajorGridFMAs = new MajorGridFMARepository();
            MajorGridFMACollection = new ObservableCollection<MajorGridFMA>(MajorGridFMAs.MajorGridFMAs);
        }
        public List<MajorGridFMA> GetAllMajorGridFMA()
        {
            return MajorGridFMACollection.ToList();
        }
    }
}
