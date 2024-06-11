using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities;
using System.Windows.Controls;
namespace NSAP_ODK.TreeViewModelControl
{
    public class AllSamplingEntitiesEventHandler : EventArgs
    {
        public TreeViewItemViewModel TreeViewItem { get; set; }
        public Entities.Database.CalendarViewType CalendarView { get; set; }
        public NSAPRegion NSAPRegion { get; set; }

        public LandingSite LandingSite { get; set; }

        public string LandingSiteText { get; set; }

        public Gear Gear { get; set; }
        public string GearUsed { get; set; }

        public FMA FMA { get; set; }

        public DateTime? MonthSampled { get; set; }
        public FishingGround FishingGround { get; set; }

        public string TreeViewEntity { get; set; }

        public string ContextMenuTopic { get; set; }

        public string GUID { get; set; }
        public override int GetHashCode()
        {
            return (NSAPRegion.Code, FMA.FMAID, FishingGround.Code, LandingSite.LandingSiteID, MonthSampled).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is AllSamplingEntitiesEventHandler)
                {
                    var aseeh = obj as AllSamplingEntitiesEventHandler;
                    if (aseeh.FishingGround != null && aseeh.LandingSite != null && MonthSampled != null)
                    {
                        return aseeh.NSAPRegion.Code == NSAPRegion.Code &&
                            aseeh.FMA.FMAID == FMA.FMAID &&
                            aseeh.FishingGround.Code == FishingGround.Code &&
                            aseeh.LandingSite.LandingSiteID == LandingSite.LandingSiteID &&
                            aseeh.MonthSampled == MonthSampled;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
