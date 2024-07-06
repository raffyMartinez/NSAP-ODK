using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HPSF;
using NSAP_ODK.Entities;
namespace NSAP_ODK.TreeViewModelControl
{
    public class tv_MonthViewModel:TreeViewItemViewModel
    {
        public readonly string _month;
        public readonly LandingSite _landingSite;
        public readonly FishingGround _fishingGround;
        public readonly FMA _fma;
        public readonly NSAPRegion _nsapRegion;
        public readonly string _landingSiteName;
        public string GUID { get; set; }
        public tv_MonthViewModel(string month, tv_LandingSiteViewModel parent):base(parent,true)
        {
            _month = month;
            _landingSiteName = parent._landingSiteText;
            _landingSite = parent._landingSite;
            _fishingGround = parent._fishingGround;
            _fma = parent._fma;
            _nsapRegion = parent._region;
            if(string.IsNullOrEmpty(GUID))
            {
                GUID = Guid.NewGuid().ToString();
            }

            
        }


        public string Name
        {
            get { return DateTime.Parse(_month).ToString("MMM-yyyy"); }
        }
    }
}
