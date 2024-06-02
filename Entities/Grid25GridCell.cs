using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.Entities
{
    public enum SubGridStyle
    {
        SubgridStyleNone,
        SubgridStyle_2 = 2,
        SubgridStyle_3,
        SubgridStyle_4
    }
    public class Grid25GridCell
    {
        public bool IsValid { get; private set; }
        public override bool Equals(object obj)
        {
            if (obj is Grid25GridCell)
            {
                Grid25GridCell cell= (Grid25GridCell)obj;
                return cell.UTMZone.ToString()==UTMZone.ToString() && cell.ToString() == this.ToString();
            }
            else
            {
                return false;
            }
        }
        public ISO_Classes.Coordinate Coordinate { get; private set; }
        public UTMCoordinate UTMCoordinate { get; private set; }
        public UTMZone UTMZone { get; set; }
        public ushort GridNumber { get; private set; }
        public char Column { get; private set; }
        public byte Row { get; private set; }


        public SubGridStyle SubGridStyle { get; set; }

        public void Add(Grid25SubGrid subGrid)
        {
            int totalCell = (int)SubGridStyle * (int)SubGridStyle;
            if (SubGrids.Count < totalCell)
            {
                SubGrids.Add(subGrid);
            }
            else
            {
                throw new IndexOutOfRangeException($"Number of subgrids cannot be more than {totalCell}");
            }
        }

        public List<Grid25SubGrid> SubGrids { get; private set; }


        /// <summary>
        /// tests grid number if it is part of majorgrid number names
        /// </summary>
        /// <param name="gridNumber"></param>
        /// <returns></returns>
        private bool NumberIsIncludedInGrid(ushort gridNumber)
        {
            switch (UTMZone.ToString())
            {
                case "50N":
                    return gridNumber >= UTMZone.MajorGridNumberingStart && gridNumber <= UTMZone.MaxGridNumber;
                case "51N":
                    //the first 10 numbers for each row is not included
                    //Example:
                    //1st row ranges from 11 to 30
                    //2nd row ranges from 41 to 60
                    //So this makes sure that a grid number is always equal or greater than start row number
                    int startRowNumber = (1 + (int)Math.Floor((double)(gridNumber / UTMZone.MajorGridColumns))) * UTMZone.MajorGridColumns - 19;

                    //return true if gridNumber is >= startRowNumber or gridNumber is perfectly divisible by 30 (MajorGridColumns)
                    return gridNumber >= startRowNumber || gridNumber % UTMZone.MajorGridColumns == 0;
            }
            return true;
        }
        private void ProcessProperties(char column, byte row, ushort gridNumber)
        {
            IsValid = true;
            column = Char.ToUpper(column);
            if (column >= 'A' && column <= 'Y')
            {
                Column = column;
                if (row >= 1 && row <= 25)
                {
                    Row = row;
                    IsValid = gridNumber <= UTMZone.MaxGridNumber && NumberIsIncludedInGrid(gridNumber);
                    if (IsValid)
                    {
                        GridNumber = gridNumber;
                        UTMCoordinate = Grid25GridCell.Grid25CellToUTMCoordinate(this);
                        Coordinate = Grid25GridCell.UTMCoordinateToLongLatCoordinate(UTMCoordinate);

                    }
                    else
                    {
                        throw new Exception("Grid25 cell major grid numbers cannot exceed maximum grid number and must be found in the grid");
                    }
                }
                else
                {
                    IsValid = false;
                    throw new Exception("Grid25 cell row names can only be from 1 to 25");
                }
            }
            else
            {
                IsValid = false;
                throw new Exception("Grid25 cell column names can only be from A to Y");
            }

        }
        public Grid25GridCell(UTMZone utmZone, ushort gridNumber, char column, byte row)
        {
            UTMZone = utmZone;
            ProcessProperties(column, row, gridNumber);
            SubGrids = new List<Grid25SubGrid>();

        }

        public override string ToString()
        {
            return $"{GridNumber.ToString()}-{Column}{Row}";
        }
        public string SerializeToString()
        {
            return $"{GridNumber.ToString()}-{Column}{Row}";
        }
        public Grid25GridCell(UTMZone utmZone, string gridString, bool processProperties=true)
        {
            bool proceed = false;
            byte row = 0;
            char col = '0';
            UTMZone = utmZone;
            var arr = gridString.Split('-');
            ushort gridNumber = ushort.Parse(arr[0]);

            try
            {
                col = arr[1][0];
                proceed = true;
                try
                {
                    row = byte.Parse(arr[1].Substring(1));
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    proceed = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                proceed = false;
            }



            if (proceed && processProperties)
            {
                ProcessProperties(col, row, gridNumber);
                SubGrids = new List<Grid25SubGrid>();
            }
        }

        public static ISO_Classes.Coordinate UTMCoordinateToLongLatCoordinate(UTMCoordinate utmCoordinate)

        {
            Oware.LatLngUTMConverter converter = new Oware.LatLngUTMConverter("WGS 84");
            var ll = converter.convertUtmToLatLng(utmCoordinate.X, utmCoordinate.Y, utmCoordinate.UTMZone.ZoneNumber, char.ToString(utmCoordinate.UTMZone.ZoneNS));
            return new ISO_Classes.Coordinate((float)ll.Lat, (float)ll.Lng);
        }
        public static ISO_Classes.Coordinate Grid25CellToLongLatCoordinate(Grid25GridCell cell)
        {
            int x;
            int y;
            Grid25CellToUTMString(cell, out x, out y);
            Oware.LatLngUTMConverter converter = new Oware.LatLngUTMConverter("WGS 84");
            var ll = converter.convertUtmToLatLng(x, y, cell.UTMZone.ZoneNumber, char.ToString(cell.UTMZone.ZoneNS));
            return new ISO_Classes.Coordinate((float)ll.Lat, (float)ll.Lng);
        }
        public static string Grid25CellToUTMString(Grid25GridCell cell, out int x, out int y)
        {
            int halfGridSize = cell.UTMZone.MinorGridCellSizeMeters / 2;
            int majorX = GridXOrigin(cell);
            int majorY = GridYOrigin(cell);
            int minorCol = cell.Column - 64;
            int minorRow = 25 - cell.Row;
            x = majorX + (minorCol * cell.UTMZone.MinorGridCellSizeMeters) - halfGridSize;
            y = majorY + (minorRow * cell.UTMZone.MinorGridCellSizeMeters) + halfGridSize;
            return $"{cell.UTMZone.ToString()} {x} {y}";
        }

        public static UTMCoordinate Grid25CellToUTMCoordinate(Grid25GridCell cell)
        {
            int x;
            int y;
            Grid25CellToUTMString(cell, out x, out y);
            return new UTMCoordinate(cell.UTMZone, x, y);
        }
        private static int GridYOrigin(Grid25GridCell cell)
        {
            return MajorGridRowPosition(cell) * cell.UTMZone.MajorGridSizeMeters + (cell.UTMZone.MajorGridYOrigin - cell.UTMZone.MajorGridSizeMeters);
        }
        private static int MajorGridRowPosition(Grid25GridCell cell)
        {
            double d = ((double)cell.GridNumber / cell.UTMZone.MajorGridColumns);
            if (d == Math.Floor(d))
            {
                d--;
            }
            return (int)Math.Floor(d) + 1;
        }
        private static int GridXOrigin(Grid25GridCell cell)
        {
            return MajorGridColPosition(cell) * cell.UTMZone.MajorGridSizeMeters + (cell.UTMZone.MajorGridXOrigin + (-cell.UTMZone.MajorGridSizeMeters));
        }

        private static int MajorGridColPosition(Grid25GridCell cell)
        {
            var rv = 0;
            var colCount = cell.UTMZone.MajorGridColumns;
            if (cell.GridNumber > colCount)
            {
                double d = cell.GridNumber / colCount;
                rv = cell.GridNumber - ((int)(Math.Floor(d) * colCount));
                if (rv == 0)
                {
                    rv = colCount;
                }
            }
            else
            {
                rv = cell.GridNumber;
            }
            return rv;
        }
    }
}
