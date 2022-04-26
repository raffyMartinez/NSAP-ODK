using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.TreeViewModelControl
{
    public class tv_FishingGroundViewModel : TreeViewItemViewModel
    {
        public readonly FishingGround _fishingGround;
        public readonly FMA _fma;
        public readonly NSAPRegion _region;
        private SampledLandingSiteViewModel _sampledLandingSiteViewModel;
        public tv_FishingGroundViewModel(FishingGround fishingGround, tv_FMAViewModel parent) : base(parent, true)
        {
            _fishingGround = fishingGround;
            _fma = parent._fma;
            _region = parent._region;
        }

        public void Refresh()
        {
            Children.Clear();
            LoadChildren();
        }

        public string Name
        {
            get { return _fishingGround.ToString(); }
        }

        public void Add(tv_LandingSiteViewModel landingSite)
        {
            base.Children.Add(landingSite);
        }

        private int CountSampledLandings(string lsName)
        {
            return NSAPEntities.VesselUnloadViewModel.VesselUnloadCollection.Count(t => t.Parent.Parent.NSAPRegionID == _region.Code &&
                    t.Parent.Parent.FMAID == _fma.FMAID &&
                    t.Parent.Parent.FishingGroundID == _fishingGround.Code &&
                    t.Parent.Parent.LandingSiteName == lsName);
        }
        protected override void LoadChildren()
        {
            _sampledLandingSiteViewModel = new SampledLandingSiteViewModel(_fishingGround, _fma, _region);
            HashSet<LandingSite> hsLS = new HashSet<LandingSite>();
            foreach (var item in _sampledLandingSiteViewModel.SampledLandingSiteCollection.OrderBy(t => t.LandingSiteName))
            {
                LandingSite ls = new LandingSite(item.LandingSiteID, item.LandingSiteName, item.Municipality)
                {
                    Barangay = item.Barangay,
                    Longitude = 1,
                    Latitude = 1

                };
                hsLS.Add(ls);
            }
            foreach (var ls in hsLS)
            {
                base.Children.Add(new tv_LandingSiteViewModel(null, this, ls.ToString()));
            }
        }

        //protected override void LoadChildren()
        //{
        //    var thisList = new List<string>();
        //    foreach (var item in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
        //        .Where(t => t.NSAPRegionID == _region.Code)
        //        .Where(t => t.FMAID == _fma.FMAID)
        //        .Where(t => t.FishingGroundID == _fishingGround.Code))
        //    {
        //        if (!thisList.Contains(item.LandingSiteName) && CountSampledLandings(item.LandingSiteName) > 0)
        //        {
        //            thisList.Add(item.LandingSiteName);
        //        }
        //        base.Children.Add(new tv_LandingSiteViewModel(item.LandingSite, this, item.LandingSiteText));
        //    }
        //    foreach (var item in thisList)
        //    {
        //        base.Children.Add(new tv_LandingSiteViewModel(null, this, item));
        //    }

        //}
    }
}
