﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Entities.ItemSources;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;

namespace NSAP_ODK.Entities.Database
{
    public enum MaturityStageEnum
    {
        stageUnknown,
        stagePremature,
        stageImmature,
        stageDeveloping,
        stageMaturing,
        stageMature,
        stageRipening,
        stageGravid,
        stageSpawning,
        stageSpent
    }
    public class CatchMaturityEdited
    {
        public CatchMaturityEdited()
        {
            //
        }

        public CatchMaturityEdited(CatchMaturity cm)
        {
            SexCode = cm.SexCode;
            MaturityCode = cm.MaturityCode;
            WeightGutContent = cm.WeightGutContent;
            Weight = cm.Weight;
            Length = cm.Length;
            PK = cm.PK;
            GonadWeight = cm.GonadWeight;
            Parent = cm.Parent;
            GutContentCode = cm.GutContentCode;

        }
        [ItemsSource(typeof(GutContentItemsSource))]
        public string GutContentCode { get; set; }

        [ItemsSource(typeof(MaturityItemsSource))]
        public string MaturityCode { get; set; }

        public double? WeightGutContent { get; set; }
        [ReadOnly(true)]
        public int PK { get; set; }

        public bool DelayedSave { get; set; }

        public double? GonadWeight { get; set; }


        public VesselCatch Parent { get; set; }

        public double? Length { get; set; }
        public double? Weight { get; set; }

        [ItemsSource(typeof(SexItemsSource))]
        public string SexCode { get; set; }


    }
    public class CatchMaturityFlattened
    {

        public CatchMaturityFlattened(CatchMaturity cm)
        {

            ID = cm.PK;
            ParentID = cm.Parent.PK;
            CatchName = cm.Parent.CatchName;
            Taxa = cm.Parent.Taxa.ToString();
            Gear = cm.Parent.Parent.Parent.Gear.ToString();
            Length = cm.Length;
            Weight = cm.Weight;
            Sex = cm.Sex;
            Maturity = cm.Maturity;
            WeightGutContent = cm.WeightGutContent;
            GutContentClassification = cm.GutContentClassification;
            GonadWeight = cm.GonadWeight;

        }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string CatchName { get; set; }
        public string Taxa { get; set; }
        public string Gear { get; set; }
        public double? Length { get; set; }
        public double? Weight { get; set; }

        public string Sex { get; set; }

        public string Maturity { get; set; }
        public double? WeightGutContent { get; set; }

        public double? GonadWeight { get; set; }
        public string GutContentClassification { get; set; }
    }
    public class CatchMaturityCrossTab
    {
        public int RowID { get; set; }

        public int ParentCatchID { get; set; }
        public int V_unload_id { get; set; }
        public VesselUnload VesselUnload { get; set; }
        public double WeightOfCatch { get; set; }
        public string GearCode { get; set; }
        public string GearName { get; set; }
        public double WeightGearCatch { get; set; }
        public double? SampleWeightGearCatch { get; set; }
        public string SpeciesName { get; set; }
        public string Taxa { get; set; }
        public double WeightSpecies { get; set; }
        public double? Length { get; set; }
        public double? Weight { get; set; }
        public string SexCode { get; set; }
        public string MaturityCode { get; set; }
        public double? GonadWeight { get; set; }
        public string GutContentCode { get; set; }
        public double? GutContentWeight { get; set; }
        public VesselCatch Parent { get; set; }

        public string Sex
        {
            get
            {
                string sex;
                switch (SexCode)
                {
                    case "m":
                        sex = "Male";
                        break;
                    case "f":
                        sex = "Female";
                        break;
                    case "j":
                        sex = "Juvenile";
                        break;
                    default:
                        sex = "";
                        break;
                }
                return sex;
            }
        }
        public string GutContentClassification
        {
            get
            {
                string gutContent;
                switch (GutContentCode)
                {
                    case "F":
                        gutContent = "Full";
                        break;
                    case "HF":
                        gutContent = "Half full";
                        break;
                    case "E":
                        gutContent = "Empty";
                        break;
                    default:
                        gutContent = "";
                        break;


                }
                return gutContent;
            }
        }
        public string Maturity
        {
            get
            {
                string maturity;
                switch (MaturityCode)
                {
                    case "pr":
                        maturity = "Premature";
                        break;
                    case "im":
                        maturity = "Immature";
                        break;
                    case "de":
                        maturity = "Developing";
                        break;
                    case "ma":
                        maturity = "Maturing";
                        break;

                    case "mt":
                        maturity = "Mature";
                        break;
                    case "ri":
                        maturity = "Ripening";
                        break;
                    case "gr":
                        maturity = "Gravid";
                        break; ;
                    case "spw":
                        maturity = "Spawning";
                        break;
                    case "sp":
                        maturity = "Spent";
                        break;
                    default:
                        maturity = "";
                        break;

                }
                return maturity;
            }
        }

    }
    public class CatchMaturity
    {
        private VesselCatch _parent;
        public int PK { get; set; }

        public bool DelayedSave { get; set; }

        public double? GonadWeight { get; set; }
        public int VesselCatchID { get; set; }

        public double? Length { get; set; }
        public double? Weight { get; set; }

        public string SexCode { get; set; }

        public override string ToString()
        {
            return $"Sex:{Sex} - Maturity:{Maturity}";
        }

        public string Sex
        {
            get
            {
                string sex;
                switch (SexCode)
                {
                    case "m":
                        sex = "Male";
                        break;
                    case "f":
                        sex = "Female";
                        break;
                    case "j":
                        sex = "Juvenile";
                        break;
                    default:
                        sex = "";
                        break;
                }
                return sex;
            }
        }
        public string GutContentClassification
        {
            get
            {
                string gutContent;
                switch (GutContentCode)
                {
                    case "F":
                        gutContent = "Full";
                        break;
                    case "HF":
                        gutContent = "Half full";
                        break;
                    case "E":
                        gutContent = "Empty";
                        break;
                    default:
                        gutContent = "";
                        break;


                }
                return gutContent;
            }
        }
        public static MaturityStageEnum GetStageFromCode(string stageCode)
        {
            MaturityStageFromCode(stageCode);
            return MaturityStageEnum;    

        }
        public static MaturityStageEnum GetStageEnumFromName(string stageName)
        {
            CodeFromMaturityStage(stageName);
            return MaturityStageEnum;
        }
        public static MaturityStageEnum MaturityStageEnum { get; set; }
        public static string CodeFromMaturityStage(string stage)
        {


            string code = "";
            MaturityStageEnum = MaturityStageEnum.stageUnknown;
            switch (stage)
            {
                case "Premature":
                    code = "pr";
                    MaturityStageEnum = MaturityStageEnum.stagePremature;
                    break;
                case "Immature":
                    code = "im";
                    MaturityStageEnum = MaturityStageEnum.stageImmature;
                    break;
                case "Developing":
                    code = "de";
                    MaturityStageEnum = MaturityStageEnum.stageDeveloping;
                    break;
                case "Maturing":
                    code = "ma";
                    MaturityStageEnum = MaturityStageEnum.stageMaturing;
                    break;
                case "Mature":
                    code = "mt";
                    MaturityStageEnum = MaturityStageEnum.stageMature;
                    break;
                case "Ripening":
                    code = "ri";
                    MaturityStageEnum = MaturityStageEnum.stageRipening;
                    break;
                case "Gravid":
                    code = "gr";
                    MaturityStageEnum = MaturityStageEnum.stageGravid;
                    break; ;
                case "Spawning":
                    code = "spw";
                    MaturityStageEnum = MaturityStageEnum.stageSpawning;
                    break;
                case "Spent":
                    code = "sp";
                    MaturityStageEnum = MaturityStageEnum.stageSpent;
                    break;

            }
            return code;
        }

        public static string MaturityStageFromCode(string maturityCode)
        {
            string maturity = "";
            switch (maturityCode)
            {
                case "pr":
                    maturity = "Premature";
                    MaturityStageEnum = MaturityStageEnum.stagePremature;
                    break;
                case "im":
                    maturity = "Immature";
                    MaturityStageEnum = MaturityStageEnum.stageImmature;
                    break;
                case "de":
                    maturity = "Developing";
                    MaturityStageEnum = MaturityStageEnum.stageDeveloping;
                    break;
                case "ma":
                    maturity = "Maturing";
                    MaturityStageEnum = MaturityStageEnum.stageMaturing;
                    break;

                case "mt":
                    maturity = "Mature";
                    MaturityStageEnum = MaturityStageEnum.stageMature;
                    break;
                case "ri":
                    maturity = "Ripening";
                    MaturityStageEnum = MaturityStageEnum.stageRipening;
                    break;
                case "gr":
                    maturity = "Gravid";
                    MaturityStageEnum = MaturityStageEnum.stageGravid;
                    break; ;
                case "spw":
                    maturity = "Spawning";
                    MaturityStageEnum = MaturityStageEnum.stageSpawning;
                    break;
                case "sp":
                    maturity = "Spent";
                    MaturityStageEnum = MaturityStageEnum.stageSpent;
                    break;
                default:
                    maturity = "";
                    MaturityStageEnum = MaturityStageEnum.stageUnknown;
                    break;

            }
            return maturity;
        }
        public string Maturity
        {
            get
            {
                string maturity;
                switch (MaturityCode)
                {
                    case "pr":
                        maturity = "Premature";
                        break;
                    case "im":
                        maturity = "Immature";
                        break;
                    case "de":
                        maturity = "Developing";
                        break;
                    case "ma":
                        maturity = "Maturing";
                        break;

                    case "mt":
                        maturity = "Mature";
                        break;
                    case "ri":
                        maturity = "Ripening";
                        break;
                    case "gr":
                        maturity = "Gravid";
                        break; ;
                    case "spw":
                        maturity = "Spawning";
                        break;
                    case "sp":
                        maturity = "Spent";
                        break;
                    default:
                        maturity = "";
                        break;

                }
                return maturity;
            }
        }
        public string GutContentCode { get; set; }
        public string MaturityCode { get; set; }

        public double? WeightGutContent { get; set; }
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
