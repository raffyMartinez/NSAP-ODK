﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace NSAP_ODK.Entities.Database
{
    public class FishingGroundGridFlattened
    {
        public FishingGroundGridFlattened(FishingGroundGrid fgg)
        {
            ID = fgg.PK;
            ParentID = fgg.Parent.PK;
            UTMZone = fgg.UTMZone.ToString();
            Grid = fgg.GridCell.ToString();
            Grid25Grid = new Grid25GridCell(new UTMZone(UTMZone), Grid);
        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string UTMZone { get; set; }
        public string Grid { get; set; }

        public Grid25GridCell Grid25Grid { get; internal set; }
        public string LonLat
        {
            get
            {
                float x = 0f; float y = 0f;
                Grid25Grid.Coordinate.GetD(out y, out x);
                return $"{x}, {y}";
            }
        }

        public string UTMCoordinate { get { return Grid25Grid.UTMCoordinate.ToString(); } }

    }

    public class FishingGroundGridEdited
    {
        public FishingGroundGridEdited()
        {

        }
        public FishingGroundGridEdited(FishingGroundGrid fgg)
        {
            if (fgg != null)
            {
                PK = fgg.PK;
                FishingGroundGrid = fgg;
                UTMZoneText = fgg.UTMZoneText;
                Grid = fgg.Grid;
            }
        }

        [ReadOnly(true)]
        public int PK { get; set; }
        public FishingGroundGrid FishingGroundGrid { get; set; }
        
        [ItemsSource(typeof(UTMZoneItemsSource))]
        public string UTMZoneText { get; set; }
        public string Grid
        {
            get; set;
        }
    }
    public class FishingGroundGrid
    {
        private VesselUnload _parent;

        public bool DelayedSave { get; set; }
        public int PK { get; set; }
        public int VesselUnloadID { get; set; }
        public string UTMZoneText { get; set; }
        public string Grid { get; set; }

        public UTMZone UTMZone
        {
            get
            {
                return new UTMZone(UTMZoneText);
            }
        }

        public Grid25GridCell GridCell
        {
            get { return new Grid25GridCell(UTMZone, Grid); }
        }

        public override string ToString()
        {
            return $"{UTMZoneText} - {Grid}";
        }

        public VesselUnload Parent
        {
            set { _parent = value; }
            get
            {
                if (_parent == null)
                {
                    _parent = NSAPEntities.VesselUnloadViewModel.getVesselUnload(VesselUnloadID);
                }
                return _parent;
            }
        }

    }
}
