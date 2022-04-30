using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
namespace NSAP_ODK.TreeViewModelControl
{
    public class tv_LandingSiteViewModel : TreeViewItemViewModel
    {
        public readonly LandingSite _landingSite;
        public readonly FishingGround _fishingGround;
        public readonly FMA _fma;
        public readonly NSAPRegion _region;
        public readonly string _landingSiteText;
        //public readonly int _numberOfSampledLandings;
        public tv_LandingSiteViewModel(LandingSite landingSite, tv_FishingGroundViewModel parent, string landingSiteText = "") : base(parent, true)
        {
            _landingSite = landingSite;
            _fishingGround = parent._fishingGround;
            _fma = parent._fma;
            _region = parent._region;
            _landingSiteText = landingSiteText;
            //_numberOfSampledLandings = NSAPEntities.VesselUnloadViewModel
        }


        public void Refresh()
        {
            Children.Clear();
            LoadChildren();
        }

        public string Name
        {
            get
            {
                return _landingSiteText;
            }
        }

        public void Add(tv_MonthViewModel month)
        {
            base.Children.Add(month);
        }

        protected override void LoadChildren()
        {
            //string lsName = _landingSite == null ? _landingSiteText : _landingSite.LandingSiteName;
            List<DateTime> listMonthYear = new List<DateTime>();
            //foreach (var item in NSAPEntities.LandingSiteSamplingViewModel.LandingSiteSamplingCollection
            //    .Where(t => t.NSAPRegionID == _region.Code)
            //    .Where(t => t.FMAID == _fma.FMAID)
            //    .Where(t => t.FishingGroundID == _fishingGround.Code)
            //    .Where(ls => ls.LandingSiteName == lsName ))
            //{
            //    var monthYear = item.SamplingDate.ToString("MMM-yyyy");
            //    if (!listMonthYear.Contains(DateTime.Parse( monthYear)))
            //    {
            //        listMonthYear.Add(DateTime.Parse( monthYear));
            //    }
            //}

            NSAPEntities.SummaryItemViewModel.TreeViewData = new AllSamplingEntitiesEventHandler
            {
                LandingSite = _landingSite,
                FishingGround = _fishingGround,
                FMA = _fma,
                NSAPRegion = _region,
                LandingSiteText = _landingSiteText,
                TreeViewEntity = this.GetType().Name
            };


            foreach (var item in NSAPEntities.SummaryItemViewModel.SummaryResults)
            {
                listMonthYear.Add(item.DBSummary.SampledMonth);
            }

            foreach (var my in listMonthYear.OrderBy(t => t))
            {
                base.Children.Add(new tv_MonthViewModel(my.ToString("MMM-yyyy"), this));
            }
        }
    }
}
