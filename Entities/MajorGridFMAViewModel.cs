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

        private static List<MajorGridFMA> _majorGridsInFMA;

        public MajorGridFMAViewModel()
        {
            MajorGridFMAs = new MajorGridFMARepository();
            MajorGridFMACollection = new ObservableCollection<MajorGridFMA>(MajorGridFMAs.MajorGridFMAs);
        }
        public List<MajorGridFMA> GetAllMajorGridFMA()
        {
            return MajorGridFMACollection.ToList();
        }

        public static List<MajorGridFMA> MajorGridsInFMA { get { return _majorGridsInFMA; } }

        public List<MajorGridFMA> GetAllMajorGridFMA(FMA fMA)
        {
            _majorGridsInFMA = MajorGridFMACollection.Where(t => t.FMA !=null && t.FMA.FMAID==fMA.FMAID).ToList();
            return _majorGridsInFMA;
        }

        public List<MajorGridFMA> GetAllMajorGridFMA(FMA fMA, UTMZone utmZone)
        {
            _majorGridsInFMA = MajorGridFMACollection.Where(t => t.FMA != null && t.FMA.FMAID == fMA.FMAID && t.UTMZone.ZoneNumber==utmZone.ZoneNumber).ToList();
            return _majorGridsInFMA;
        }


    }
}
