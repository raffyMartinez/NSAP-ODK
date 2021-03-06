﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities.Database
{
    public class CatchLengthFlattened
    {
        public  CatchLengthFlattened(CatchLength cl)
        {
            ID = cl.PK;
            ParentID = cl.Parent.PK;
            CatchName = cl.Parent.CatchName;
            Taxa = cl.Parent.Taxa.ToString();
            Gear = cl.Parent.Parent.Parent.Gear.ToString();
            Length = cl.Length;

        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string CatchName { get; set; }
        public string Taxa { get; set; }
        public string Gear { get; set; }
        public double Length { get; set; }
    }
    public class CatchLength
    {
        private VesselCatch _parent;
        public int PK { get; set; }
        public int VesselCatchID { get; set; }

        public double Length { get; set; }
        public VesselCatch Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.VesselCatchViewModel.getVesselCatch(VesselCatchID);
                }
                return _parent;
            }
        }
    }
}
