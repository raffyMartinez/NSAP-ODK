using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
namespace NSAP_ODK.TreeViewModelControl
{
    public class tv_FMAViewModel:TreeViewItemViewModel
    {
        public readonly FMA _fma;
        public readonly NSAPRegion _region;
        public tv_FMAViewModel(FMA fma, tv_NSAPRegionViewModel parent):base(null,true)
        {
            _fma = fma;
            _region = parent._region;
        }

        public string Name
        {
            get { return _fma.ToString(); }

        }

        protected override void LoadChildren()
        {
            foreach (var fma in _region.FMAs
                .Where(t=>t.FMAID==_fma.FMAID))
            {
                foreach (var fishingGround in fma.FishingGrounds)
                {
                    base.Children.Add(new tv_FishingGroundViewModel(fishingGround.FishingGround, this));
                }
            }
            

        }
    }
}
