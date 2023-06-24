﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class VesselUnload_FishingGear
    {
        private VesselUnload _vesselUnload;
        public List<VesselCatch> Catches
        {
            get
            {
                List<VesselCatch> vcs = new List<VesselCatch>();
                if (Parent.VesselCatchViewModel == null || Parent.VesselCatchViewModel.VesselCatchCollection == null)
                {
                    Parent.VesselCatchViewModel = new VesselCatchViewModel(Parent);
                }
                foreach (VesselCatch vc in Parent.VesselCatchViewModel.VesselCatchCollection)
                {
                    if (string.IsNullOrEmpty(GearCode))
                    {
                        if (vc.GearText == GearText)
                        {
                            vcs.Add(vc);
                        }
                    }
                    else
                    {
                        if (vc.GearCode == GearCode)
                        {
                            vcs.Add(vc);
                        }
                    }
                }

                return vcs;
            }
        }
        private Gear _gear;
        public VesselUnload_Gear_Spec_ViewModel VesselUnload_Gear_Specs_ViewModel { get; set; }
        public void SetChildEntities()
        {
            if (VesselUnload_Gear_Specs_ViewModel == null)
            {
                VesselUnload_Gear_Specs_ViewModel = new VesselUnload_Gear_Spec_ViewModel(this);
            }
        }
        public VesselUnload Parent
        {
            get { return _vesselUnload; }
            set
            {
                _vesselUnload = value;
                //if (!_vesselUnload.IsMultiGear)
                //{
                Guid = Guid.NewGuid();
                //}
            }
        }
        public Guid Guid { get; private set; }
        public bool DelayedSave { get; set; }
        public string GearCode { get; set; }
        public Gear Gear
        {
            get
            {
                if (GearCode == null)
                {
                    return null;
                }
                else
                {
                    if (_gear == null)
                    {
                        _gear = NSAPEntities.GearViewModel.GetGear(GearCode);
                    }
                    return _gear;
                }
            }
        }
        public string GearText { get; set; }

        public int RowID { get; set; }

        public string GearUsedName
        {
            get
            {
                if (string.IsNullOrEmpty(GearCode))
                {
                    return GearText;
                }
                else
                {
                    return Gear.ToString();
                }
            }
        }
    }
}
