using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapWinGIS;

namespace NSAP_ODK.Mapping
{
    public static class ShapefileAttributeTableManager
    {
        private static bool FieldContainsUniqueIntegerValues(Shapefile sf, int fieldIndex)
        {
            var intValues = new List<int>();
            for (int x = 0; x < sf.NumShapes; x++)
            {
                var val = (int)sf.CellValue[fieldIndex, x];
                if (intValues.Count == 0)
                {
                    intValues.Add(val);
                }
                else if (intValues.Contains(val))
                {
                    return false;
                }
                else
                {
                    intValues.Add(val);
                }
            }
            return true;
        }

        private static bool FillFieldWithUniqueInt(Shapefile sf, int fieldIndex)
        {
            int counter = 0;
            for (int x = 0; x < sf.NumShapes; x++)
            {
                if (!sf.EditCellValue(fieldIndex, x, counter++))
                {
                    return false;
                }
            }
            return true;
        }

        public static Callback Callback { get; private set; }
        public static string UnqiueIDColumnName { get; private set; }
        public static int UnqiueIDColumnIndex { get; private set; }
        public static void SetupIDColumn(Shapefile sf, bool saveChanges = false)
        {
            bool proceed = true;
            if (!sf.EditingTable)
            {
                Callback = new Callback();
                proceed = sf.StartEditingTable(Callback);
            }

            int idx;
            if (proceed)
            {
                if (sf.NumFields == 1)
                {
                    if (sf.Field[0].Type != FieldType.INTEGER_FIELD)
                    {
                        idx = sf.EditAddField("MWShapeID", FieldType.INTEGER_FIELD, 1, 1);
                        if (FillFieldWithUniqueInt(sf, idx))
                        {
                            UnqiueIDColumnName = sf.Field[idx].Name;
                            UnqiueIDColumnIndex = idx;
                        }
                    }
                    else
                    {
                        if (!FieldContainsUniqueIntegerValues(sf, 0))
                        {
                            idx = sf.EditAddField("MWShapeID", FieldType.INTEGER_FIELD, 1, 1);
                            if (FillFieldWithUniqueInt(sf, idx))
                            {
                                UnqiueIDColumnName = sf.Field[idx].Name;
                                UnqiueIDColumnIndex = idx;
                            }
                        }
                        else

                        {
                            UnqiueIDColumnName = sf.Field[0].Name;
                            UnqiueIDColumnIndex = 0;
                        }
                    }

                }
                else if (sf.NumFields == 0)
                {
                    idx = sf.EditAddField("MWShapeID", FieldType.INTEGER_FIELD, 1, 1);
                    if (FillFieldWithUniqueInt(sf, idx))
                    {
                        UnqiueIDColumnName = sf.Field[idx].Name;
                        UnqiueIDColumnIndex = idx;
                    }
                }
                else
                {
                    bool sfHasUniqueIDs = false;
                    for (int x = 0; x < sf.NumFields; x++)
                    {
                        if (sf.Field[x].Type == FieldType.INTEGER_FIELD)
                        {
                            if (FieldContainsUniqueIntegerValues(sf, x))
                            {
                                sfHasUniqueIDs = true;
                                UnqiueIDColumnName = sf.Field[x].Name;
                                UnqiueIDColumnIndex = x;
                                sf.StopEditingTable();
                                break;
                            }
                        }
                    }

                    if (!sfHasUniqueIDs)
                    {
                        idx = sf.EditAddField("MWShapeID", FieldType.INTEGER_FIELD, 1, 1);
                        if (FillFieldWithUniqueInt(sf, idx))
                        {
                            UnqiueIDColumnName = sf.Field[idx].Name;
                            UnqiueIDColumnIndex = idx;
                        }
                    }
                }
                if (sf.EditingTable)
                {
                    Callback = new Callback();
                    sf.StopEditingTable(saveChanges, Callback);
                }
            }
        }
        public static string DataCaption { get; internal set; }
        public static MapInterActionHandler MapInterActionHandler { get; set; }

        /// <summary>
        /// creates a datatable that represents the dbf table of shapefile attributes
        /// </summary>
        /// <param name="sf"></param>
        /// <returns></returns>
        public static DataTable SetupAttributeTable(Shapefile sf, bool selectedOnly = false)
        {
            DataTable dt = new DataTable();
            DataCaption = $"Name of layer: {MapInterActionHandler.MapLayersHandler.CurrentMapLayer.Name}";
            for (int y = 0; y < sf.NumFields; y++)
            {
                Field fld = sf.Field[y];
                string fieldCaption;
                if (string.IsNullOrEmpty(fld.Alias))
                {
                    fieldCaption = fld.Name;
                }
                else
                {
                    fieldCaption = fld.Alias;
                }
                Type t = typeof(int);
                switch (fld.Type)
                {
                    case FieldType.DOUBLE_FIELD:
                        t = typeof(double);
                        break;
                    case FieldType.STRING_FIELD:
                        t = typeof(string);
                        break;
                    case FieldType.INTEGER_FIELD:
                        t = typeof(int);
                        break;
                    case FieldType.DATE_FIELD:
                        t = typeof(DateTime);
                        break;
                    case FieldType.BOOLEAN_FIELD:
                        t = typeof(bool);
                        break;
                }
                try
                {
                    dt.Columns.Add(new DataColumn { Caption = fieldCaption, DataType = t, ColumnName = fieldCaption });
                }
                catch (Exception ex)
                {

                }
            }



            DataRow row;
            if (selectedOnly)
            {


                for (int x = 0; x < sf.NumShapes; x++)
                {
                    if (sf.ShapeIsHidden[x] || !sf.ShapeRendered[x])
                    {

                    }
                    if (sf.ShapeSelected[x] && sf.ShapeVisible[x])
                    {
                        row = dt.NewRow();
                        for (int z = 0; z < sf.NumFields; z++)
                        {
                            //row[z] = sf.CellValue[z, x];
                            if (sf.CellValue[z, x] == null)
                            {
                                row[z] = DBNull.Value;
                            }
                            else
                            {
                                row[z] = sf.CellValue[z, x];
                            }
                        }
                        dt.Rows.Add(row);
                    }
                }
            }
            else
            {
                if (sf.NumSelected == 0)
                {
                    if (!string.IsNullOrEmpty(sf.VisibilityExpression))
                    {
                        object results = null;
                        string errMsg = "";
                        if (sf.Table.Query(sf.VisibilityExpression, ref results, ref errMsg))
                        {
                            var handles= ((int[])results).ToList();
                            for (int x = 0; x < sf.NumShapes; x++)
                            {
                                if(handles.Contains(x))
                                {
                                    row = dt.NewRow();
                                    for (int z = 0; z < sf.NumFields; z++)
                                    {
                                        if (sf.CellValue[z, x] == null)
                                        {
                                            row[z] = DBNull.Value;
                                        }
                                        else
                                        {
                                            row[z] = sf.CellValue[z, x];
                                        }

                                    }
                                    dt.Rows.Add(row);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int x = 0; x < sf.NumShapes; x++)
                        {

                            var vis = sf.ShapeVisible[x];
                            if (sf.ShapeVisible[x])
                            {
                                row = dt.NewRow();
                                for (int z = 0; z < sf.NumFields; z++)
                                {
                                    if (sf.CellValue[z, x] == null)
                                    {
                                        row[z] = DBNull.Value;
                                    }
                                    else
                                    {
                                        //try
                                        //{
                                        row[z] = sf.CellValue[z, x];
                                        //}
                                        //catch
                                        //{
                                        //    //row[z] = "";
                                        //}
                                    }

                                }
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < sf.NumShapes; x++)
                    {
                        if (sf.ShapeSelected[x] && sf.ShapeVisible[x])
                        {
                            row = dt.NewRow();
                            for (int z = 0; z < sf.NumFields; z++)
                            {
                                //row[z] = sf.CellValue[z, x];
                                if (sf.CellValue[z, x] == null)
                                {
                                    row[z] = DBNull.Value;
                                }
                                else
                                {
                                    row[z] = sf.CellValue[z, x];
                                }
                            }
                            dt.Rows.Add(row);
                        }
                    }
                }
            }


            return dt;
        }
    }
}
