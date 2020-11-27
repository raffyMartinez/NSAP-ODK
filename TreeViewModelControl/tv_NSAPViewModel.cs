using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NSAP_ODK.Entities;
namespace NSAP_ODK.TreeViewModelControl
{
    public class tv_NSAPViewModel
    {
        public TreeControl ParentTreeView { get; set; }
        private List<tv_NSAPRegionViewModel> _listRegions = new List<tv_NSAPRegionViewModel>();
        public ReadOnlyCollection<tv_NSAPRegionViewModel> Regions { get { return _listRegions.AsReadOnly(); } }


        public void Add(NSAPRegion region)
        {
            tv_NSAPRegionViewModel tvRegion = new tv_NSAPRegionViewModel(region);
            _listRegions.Add(tvRegion);
            ParentTreeView.treeView.Items.Refresh();
        }
        public tv_NSAPViewModel(List<NSAPRegion>regions, TreeControl treeControl)
        {
            ParentTreeView = treeControl;
            foreach(var region in regions)
            {
                tv_NSAPRegionViewModel tvRegion = new tv_NSAPRegionViewModel(region);
                _listRegions.Add(tvRegion);
            }
        }
    }
}
