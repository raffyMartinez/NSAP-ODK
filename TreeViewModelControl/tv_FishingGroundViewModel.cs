using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
using NSAP_ODK.Entities.Database;
namespace NSAP_ODK.TreeViewModelControl
{
   public class tv_FishingGroundViewModel:TreeViewItemViewModel
    {
        public readonly FishingGround _fishingGround;
        public readonly FMA _fma;
        public readonly NSAPRegion _region;
        public tv_FishingGroundViewModel(FishingGround fishingGround, tv_FMAViewModel parent):base(parent,true)
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

        protected override void LoadChildren()
        {
            //foreach(var fma in _region.FMAs 
            //.Where(t => t.FMAID == _fma.FMAID))
            //{
            //    foreach (var fishingGround in fma.FishingGrounds)
            //    {
            //        foreach (var landingSite in fishingGround.LandingSites
            //            .Where(t=>t.NSAPRegionFMAFishingGround.FishingGroundCode==_fishingGround.Code))
            //        {
            //            base.Children.Add(new tv_LandingSiteViewModel(landingSite.LandingSite, this));
            //        }
            //    }
            //}

            var thisList = new List<string>();
            foreach (var item in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
                .Where(t => t.NSAPRegionID == _region.Code)
                .Where(t => t.FMAID == _fma.FMAID)
                .Where(t => t.FishingGroundID == _fishingGround.Code))
            {
                if(!thisList.Contains(item.LandingSiteName))
                {
                    thisList.Add(item.LandingSiteName);
                }
                //base.Children.Add(new tv_LandingSiteViewModel(item.LandingSite, this, item.LandingSiteText));
            }
            foreach (var item in thisList)
            {
                base.Children.Add(new tv_LandingSiteViewModel(null, this, item));
            }

        }
    }
}
