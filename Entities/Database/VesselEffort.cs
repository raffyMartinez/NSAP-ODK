using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Entities.Database
{
    public class VesselEffortFlattened
    {
        public  VesselEffortFlattened(VesselEffort ve)
        {
            ID = ve.PK;
            ParentID = ve.Parent.PK;
            EffortSpecification = ve.EffortSpecification.ToString();
            EffortValueNumeric = ve.EffortValueNumeric;
            EffortValueText = ve.EffortValueText;
        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string EffortSpecification { get; set; }
        public double? EffortValueNumeric { get; set; }
        public string EffortValueText { get; set; }
    }
   public class VesselEffort
    {
        private VesselUnload _parent;
        private EffortSpecification _effortSpecification;
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }
        public int EffortSpecID { get; set; }
       
        public EffortSpecification EffortSpecification
        {
            set { _effortSpecification = value; }
            get
            {
                if(_effortSpecification==null)
                {
                    _effortSpecification = NSAPEntities.EffortSpecificationViewModel.GetEffortSpecification(EffortSpecID);
                }
                return _effortSpecification;
            }
        }
        public double? EffortValueNumeric { get; set; }
        public string EffortValueText { get; set; }

        public string EffortValue
        {
            get
            {
                if(EffortValueNumeric==null)
                {
                    return EffortValueText;
                }
                else
                {
                    return ((double)EffortValueNumeric).ToString();
                }
            }
        }
        public VesselUnload Parent
        {
            set { _parent = value; }
            get
            {
                if(_parent==null)
                {
                    _parent = NSAPEntities.VesselUnloadViewModel.getVesselUnload(VesselUnloadID);
                }
                return _parent;
            }
        }

    }
}
