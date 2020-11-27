using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
namespace NSAP_ODK.TreeViewModelControl
{
    public class tv_NSAPRegionViewModel:TreeViewItemViewModel
    {
        public readonly NSAPRegion _region;
        public tv_NSAPRegionViewModel(NSAPRegion region):base(null,true)
        {

            _region = region;
        }
        public string Name
        {
            get { return _region.ToString(); }

        }

        protected override void LoadChildren()
        {
            foreach (var fma in NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(_region.Code).FMAs)
            {
                base.Children.Add(new tv_FMAViewModel(fma.FMA,this));
            }
            
        }
    }
}
