﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSAP_ODK.Utilities;


namespace NSAP_ODK.Entities
{
    public enum CSVType
    {
        Unknown,
        ExtSelectFromFile,
        ExtSelect
    }
    public static class GenerateCSV
    {
        private static List<int> _majorGrid50N = new List<int>();
        private static List<int> _majorGrid51N = new List<int>();
        private static string _fileName;
        private static string _folderSaveLocation;

        static GenerateCSV()
        {
            if (NSAPEntities.Grid25InlandLocationViewModel == null)
            {
                NSAPEntities.Grid25InlandLocationViewModel = new Grid25InlandLocationViewModel();
            }
            if (NSAPEntities.MajorGridFMAViewModel == null)
            {
                NSAPEntities.MajorGridFMAViewModel = new MajorGridFMAViewModel();
            }
            CSVType = CSVType.ExtSelectFromFile;
            LocationDelimeter = '»';

        }

        public static char LocationDelimeter { get; set; }

        public static int FilesCount { get; private set; }
        public static LogType LogType { get; set; }

        public static CSVType CSVType { get; set; }


        public static async Task<int> GenerateAll()
        {
            FilesCount = 0;
            _fileName = $"{_folderSaveLocation}\\effortspec.csv";
            int result = await GenerateEffortSpecCSV();
            FilesCount++;

            _fileName = $"{_folderSaveLocation}\\gear.csv";
            result += await GenerateGearsCSV();
            FilesCount++;

            if (CSVType == CSVType.ExtSelectFromFile)
            {
                result += await GenerateItemsetsCSVEX();
            }
            else
            {
                result += await GenerateItemsetsCSV();
                FilesCount++;
            }


            _fileName = $"{_folderSaveLocation}\\regionFMA_fmaID.csv";
            result += await GenerateFMAIDsCSV();
            FilesCount++;

            _fileName = $"{_folderSaveLocation}\\majorgrid_fma.csv";
            result += await GenerateMajorGridsInFMA();
            FilesCount++;

            _fileName = $"{_folderSaveLocation}\\inlandgrids.csv";
            result += await GenerateInlandGridCSV();
            FilesCount++;

            _fileName = $"{_folderSaveLocation}\\sp.csv";
            result += await GenerateMultiSpeciesCSV();


            _fileName = $"{_folderSaveLocation}\\size_measure.csv";
            result += await GenerateSizeTypesCSV();
            FilesCount++;

            Dictionary<FisheriesSector, string> filePaths = new Dictionary<FisheriesSector, string>();
            filePaths.Add(FisheriesSector.Municipal, $"{_folderSaveLocation}\\vessel_name_municipal.csv");
            filePaths.Add(FisheriesSector.Commercial, $"{_folderSaveLocation}\\vessel_name_commercial.csv");
            result += await GenerateFishingVesselNamesCSV(filePaths);


            return result;
        }

        public static string FolderSaveLocation
        {
            get { return _folderSaveLocation; }
            set
            {
                _folderSaveLocation = value;
                switch (LogType)
                {
                    case LogType.EffortSpec_csv:
                        _fileName = $"{_folderSaveLocation}\\effortspec.csv";
                        break;

                    case LogType.Gear_csv:
                        _fileName = $"{_folderSaveLocation}\\gear.csv";
                        break;

                    case LogType.ItemSets_csv:
                        _fileName = $"{_folderSaveLocation}\\itemsets.csv";
                        break;

                    case LogType.Species_csv:
                        _fileName = $"{_folderSaveLocation}\\sp.csv";
                        break;

                    case LogType.SizeMeasure_csv:
                        _fileName = $"{_folderSaveLocation}\\size_measure.csv";
                        break;

                    case LogType.VesselName_csv:
                        _fileName = $"{_folderSaveLocation}\\vessel_name_municipal.csv";
                        break;

                    case LogType.FMACode_csv:
                        _fileName = $"{_folderSaveLocation}\\fma_code.csv";
                        break;

                    case LogType.FishingGroundCode_csv:
                        _fileName = $"{_folderSaveLocation}\\fishing_ground_code.csv";
                        break;
                    case LogType.FMAID_csv:
                        _fileName = $"{_folderSaveLocation}\\regionFMA_fmaID.csv";
                        break;
                    case LogType.InlandGridCells_csv:
                        _fileName = $"{_folderSaveLocation}\\inlandgrids.csv";
                        break;
                }
            }
        }

        //private static async Task LogAsync(string s, string filePath = "")
        private static async Task LogAsync(string s, string filePath)
        {
            if (filePath!=null &&  filePath.Length > 0)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    await writer.WriteLineAsync(s);
                }
            }
            else
            { 
                throw new ArgumentNullException("Error: filepath must be provided");
            }
            //else
            //{
            //    using (StreamWriter writer = new StreamWriter(_fileName, true))
            //    {
            //        await writer.WriteLineAsync(s);
            //    }
            //}
        }

        public static async Task<int> GenerateFMAIDsCSV()
        {
            int counter = 0;
            string header = "region_fma_key,fma_key\r\n";
            StringBuilder sb = new StringBuilder(header);
            foreach (var reg in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in reg.FMAs)
                {
                    sb.AppendLine($"{fma.RowID},{fma.FMA.FMAID}");
                    counter++;
                }
            }
            if (counter > 0)
            {
                ////Logger.LogType = LogType.MajorGrid_csv;
                //await Logger.LogAsync(sb.ToString(), _fileName);
                await LogAsync(sb.ToString(), _fileName);
            }
            ////Logger.LogType = LogType.Logfile;
            return counter;
        }
        public static async Task<int> GenerateMultiSpeciesCSV()
        {
            double maxSize = -1;
            string szType = "";
            int counter = 0;
            string header = "taxa_key,name_key,label_key,len_max, len_type\r\n";
            StringBuilder sb = new StringBuilder();
            if (CSVType == CSVType.ExtSelectFromFile)
            {
                sb = new StringBuilder(header);
                foreach (var fishspecies in NSAPEntities.FishSpeciesViewModel.SpeciesCollection
                    .OrderBy(t => t.ToString()))
                {
                    maxSize = fishspecies.LengthMax == null ? -1 : (double)fishspecies.LengthMax;
                    szType = fishspecies.LengthType == null ? string.Empty : fishspecies.LengthType.Code;
                    sb.AppendLine($"FIS,{fishspecies.SpeciesCode},\"{fishspecies.ToString()}\",{maxSize},{szType}");
                    counter++;
                }
                //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\fish_species.csv");
                await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\fish_species.csv");
                FilesCount++;

                sb = new StringBuilder(header);
                foreach (var notFish in NSAPEntities.NotFishSpeciesViewModel.NotFishSpeciesCollection
                    .OrderBy(t => t.Taxa.Code)
                    .ThenBy(t => t.ToString()))
                {
                    maxSize = notFish.MaxSize == null ? -1 : (double)notFish.MaxSize;
                    szType = notFish.SizeType == null ? string.Empty : notFish.SizeType.Code;
                    sb.AppendLine($"{notFish.Taxa.Code},{notFish.SpeciesID},\"{notFish.ToString()}\",{maxSize},{szType}");
                    counter++;
                }
                //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\not_fish_species.csv");
                await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\not_fish_species.csv");
                FilesCount++;
            }
            else
            {
                sb = new StringBuilder(header);
                SpeciesMultiTaxaViewModel smtvm = new SpeciesMultiTaxaViewModel();
                foreach (var sp in smtvm.SpeciesMultiTaxaCollection)
                {
                    maxSize = sp.Size == null ? -1 : (double)sp.Size;
                    szType = sp.SizeType == null ? string.Empty : sp.SizeType.Code;
                    sb.AppendLine($"{sp.Taxa.Code},{sp.SpeciesID},\"{sp.SpeciesName}\",{maxSize},{szType}");
                    counter++;
                }
                if (counter > 0)
                {
                    ////Logger.LogType = LogType.Species_csv;
                    //await Logger.LogAsync(sb.ToString(), _fileName);
                    await LogAsync(sb.ToString(), _fileName);
                    FilesCount++;
                }
            }
            ////Logger.LogType = LogType.Logfile;
            return counter;
        }


        private static async Task<int> WriteToVesselFileName(FisheriesSector fisheriesSector, string fileName)
        {

            int counter = 0;
            StringBuilder sb = new StringBuilder("sector_key,name_key,label_key,region_key\r\n");
            switch (fisheriesSector)
            {
                case FisheriesSector.Municipal:
                    foreach (var reg in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                         .Where(t => NSAPEntities.Regions.Contains(t.Code)))
                    {
                        foreach (var fv in reg.FishingVessels
                            .Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Municipal)
                            .OrderBy(t => t.FishingVessel.ToString()))
                        {
                            sb.AppendLine($"{fv.FishingVessel.FisheriesSector.ToString().Substring(0, 1)},{fv.FishingVessel.ID},\"{fv.FishingVessel.ToString()}\",{reg.Code}");
                            counter++;
                        }
                    }

                    //await Logger.LogAsync(sb.ToString(), fileName);
                    await LogAsync(sb.ToString(), fileName);

                    break;
                case FisheriesSector.Commercial:
                    sb = new StringBuilder("sector_key,name_key,label_key,region_key\r\n");
                    foreach (var reg in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                         .Where(t => NSAPEntities.Regions.Contains(t.Code)))
                    {
                        foreach (var fv in reg.FishingVessels
                            .Where(t => t.FishingVessel.FisheriesSector == FisheriesSector.Commercial)
                            .OrderBy(t => t.FishingVessel.ToString()))
                        {
                            sb.AppendLine($"{fv.FishingVessel.FisheriesSector.ToString().Substring(0, 1)},{fv.FishingVessel.ID},\"{fv.FishingVessel.ToString()}\",{reg.Code}");
                            counter++;
                        }
                    }


                    //await Logger.LogAsync(sb.ToString(), fileName);
                    await LogAsync(sb.ToString(), fileName);

                    break;
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }
        public static async Task<int> GenerateFishingVesselNamesCSV(Dictionary<FisheriesSector, string> filePaths)
        {
            int counter = 0;
            foreach (var item in filePaths)
            {
                counter += await WriteToVesselFileName(item.Key, item.Value);
                FilesCount++;
            }
            return counter;
        }

        public static async Task<int> GenerateInlandGridCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("rowid_key,inland_grid_key\r\n");

            foreach (var inlandGrid in NSAPEntities.Grid25InlandLocationViewModel.Grid25InlandLocationCollection
            .Where(t => t.Grid25GridCell.UTMZone.ZoneNumber == 51))
            {
                if (_majorGrid51N.Contains(inlandGrid.Grid25GridCell.GridNumber))
                {
                    sb.AppendLine($"{counter + 1},{inlandGrid.Grid25GridCell.ToString()}");
                    counter++;
                }

            }



            //Logger.LogType = LogType.InlandGridCells_csv;
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\inlandgrids51N.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\inlandgrids51N.csv");


            sb = new StringBuilder("rowid_key,inland_grid_key\r\n");

            foreach (var inlandGrid in NSAPEntities.Grid25InlandLocationViewModel.Grid25InlandLocationCollection
            .Where(t => t.Grid25GridCell.UTMZone.ZoneNumber == 50))
            {
                if (_majorGrid50N.Contains(inlandGrid.Grid25GridCell.GridNumber))
                {
                    sb.AppendLine($"{counter + 1},{inlandGrid.Grid25GridCell.ToString()}");
                    counter++;
                }

            }


            //Logger.LogType = LogType.InlandGridCells_csv;
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\inlandgrids50N.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\inlandgrids50N.csv");


            //Logger.LogType = LogType.Logfile;
            return counter;
        }


        public static async Task<int> GenerateMajorGridsInFMA()
        {
            List<string> list_mg_fma = new List<string>();
            _majorGrid51N.Clear();
            _majorGrid50N.Clear();

            int counter = 0;
            StringBuilder sb = new StringBuilder("majorgrid_number_key,gridNumber_fma_key\r\n");
            foreach (var reg in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                 .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in reg.FMAs)
                {
                    foreach (var mgf in NSAPEntities.MajorGridFMAViewModel.MajorGridFMACollection
                        .Where(t => t.FMA != null)
                        .Where(t => t.FMA.FMAID == fma.FMA.FMAID)
                        .Where(t => t.UTMZone.ZoneNumber == 51))
                    {
                        string mg_fma = $"{mgf.MajorGridNumber}-{fma.FMAID}";
                        if (!list_mg_fma.Contains(mg_fma))
                        {
                            list_mg_fma.Add(mg_fma);
                            sb.AppendLine($"{mgf.MajorGridNumber},{mg_fma}");
                            _majorGrid51N.Add(mgf.MajorGridNumber);
                            counter++;
                        }
                    }
                }
            }

            //Logger.LogType = LogType.MajorGrid_csv;
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\majorgrid_fma_51N.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\majorgrid_fma_51N.csv");


            list_mg_fma.Clear();
            sb = new StringBuilder("majorgrid_number_key,gridNumber_fma_key\r\n");
            list_mg_fma.Clear();
            foreach (var reg in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                 .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in reg.FMAs)
                {
                    foreach (var mgf in NSAPEntities.MajorGridFMAViewModel.MajorGridFMACollection
                        .Where(t => t.FMA != null)
                        .Where(t => t.FMA.FMAID == fma.FMA.FMAID)
                        .Where(t => t.UTMZone.ZoneNumber == 50))
                    {
                        string mg_fma = $"{mgf.MajorGridNumber}-{fma.FMAID}";
                        if (!list_mg_fma.Contains(mg_fma))
                        {
                            list_mg_fma.Add(mg_fma);
                            sb.AppendLine($"{mgf.MajorGridNumber},{mg_fma}");
                            _majorGrid50N.Add(mgf.MajorGridNumber);
                            counter++;
                        }
                    }
                }
            }

            //Logger.LogType = LogType.MajorGrid_csv;
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\majorgrid_fma_50N.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\majorgrid_fma_50N.csv");

            //Logger.LogType = LogType.Logfile;
            return counter;
        }
        public static async Task<int> GenerateFishSpeciesCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("list_name,name_key,label_key,len_max\r\n");
            foreach (var sp in NSAPEntities.FishSpeciesViewModel.SpeciesCollection)
            {
                double maxlen = sp.LengthMax == null ? -1 : (double)sp.LengthMax;
                sb.AppendLine($"sp,{sp.SpeciesCode},\"{sp.GenericName} {sp.SpecificName}\",{maxlen}");
                counter++;
            }
            if (counter > 0)
            {
                //Logger.LogType = LogType.Species_csv;
                //await Logger.LogAsync(sb.ToString());
                await LogAsync(sb.ToString(), _fileName);
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }

        public static async Task<int> GenerateSizeTypesCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("size_type_key,label_key\r\n");
            foreach (var st in NSAPEntities.SizeTypeViewModel.SizeTypeCollection)
            {
                sb.AppendLine($"{st.Code},\"{st.Name}\"");
                counter++;
            }
            if (counter > 0)
            {
                //Logger.LogType = LogType.SizeMeasure_csv;
                //await Logger.LogAsync(sb.ToString(), _fileName);
                await LogAsync(sb.ToString(), _fileName);
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }
        public static async Task<int> GenerateGearsCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("name_key,label_key,code_key\r\n");
            foreach (var r in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var g in r.Gears)
                {
                    sb.AppendLine($"{g.RowID},\"{g.Gear.GearName}\",{g.Gear.Code}");
                    counter++;
                }
            }
            if (counter > 0)
            {
                //Logger.LogType = LogType.Gear_csv;
                //await Logger.LogAsync(sb.ToString(), _fileName);
                //await Logger.LogAsync(sb.ToString());
                await LogAsync(sb.ToString(), _fileName);
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }

        public static async Task<int> GenerateFishingGroundCodeCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("name_key,code_key,fg_name_key\r\n");
            foreach (var r in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fg in r.FMAs)
                {
                    foreach (var fgfma in fg.FishingGrounds)
                    {
                        sb.AppendLine($"{fgfma.RowID},\"{fgfma.FishingGroundCode}\",\"{fgfma.FishingGround.Name}\"");
                    }
                    counter++;
                }
            }
            if (counter > 0)
            {
                //Logger.LogType = LogType.FishingGroundCode_csv;
                //await Logger.LogAsync(sb.ToString(), _fileName);
                await Logger.LogAsync(sb.ToString());
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }

        public static async Task<int> GenerateFMACodeCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("name_key,label_key\r\n");
            foreach (var r in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in r.FMAs)
                {
                    sb.AppendLine($"{fma.RowID},\"{fma.FMA.Name}\"");
                    counter++;
                }
            }
            if (counter > 0)
            {
                //Logger.LogType = LogType.FMACode_csv;
                //await Logger.LogAsync(sb.ToString(), _fileName);
                await Logger.LogAsync(sb.ToString());
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }

        public static async Task<int> GenerateItemsetsCSVEX()
        {
            int counter = 0;
            string header = "list_name,name,label,option\r\n";



            //generate fma section
            StringBuilder sb = new StringBuilder(header);
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in region.FMAs.OrderBy(t => t.FMA.Name))
                {
                    sb.AppendLine($"fma,{fma.RowID},{fma.FMA.Name},{fma.NSAPRegion.Code}");
                    counter++;
                }

            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\fma_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\fma_select.csv"); 
            FilesCount++;


            //generate fishing ground section
            sb = new StringBuilder(header);
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in region.FMAs)
                {
                    foreach (var fg in fma.FishingGrounds)
                    {
                        sb.AppendLine($"fg,{fg.RowID},{fg.FishingGround.Name},{fg.RegionFMA.RowID}");
                        counter++;
                    }
                }
            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\fg_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\fg_select.csv");
            FilesCount++;

            //generate ls (landing site) section
            sb = new StringBuilder(header);
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in region.FMAs)
                {
                    foreach (var fg in fma.FishingGrounds)
                    {
                        foreach (var ls in fg.LandingSites
                            .OrderBy(t => t.LandingSite.ToString()))
                        {

                            if (CSVType == CSVType.ExtSelectFromFile)
                            {
                                sb.AppendLine($"landing_site,{ls.RowID},{ls.LandingSite.ToString().Replace(',', GenerateCSV.LocationDelimeter)},{fg.RowID}");
                            }
                            else
                            {
                                sb.AppendLine($"landing_site,{ls.RowID},\"{ls.LandingSite.ToString()}\",{fg.RowID}");
                            }
                            counter++;
                        }
                    }
                }
            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\ls_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\ls_select.csv");
            FilesCount++;

            //generate gear section
            sb = new StringBuilder(header);
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                 .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var gear in region.Gears.OrderBy(t => t.NSAPRegion.Name).ThenBy(t => t.Gear.GearName))
                {
                    sb.AppendLine($"gear,{gear.RowID},{gear.Gear.GearName},{region.Code}");
                    counter++;
                }
            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\gear_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\gear_select.csv");
            FilesCount++;


            //generate enumerator section
            sb = new StringBuilder(header);
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                 .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var enumerator in region.NSAPEnumerators.OrderBy(t => t.Enumerator.Name))
                {
                    sb.AppendLine($"enumerator,{enumerator.RowID},{enumerator.Enumerator.Name},{region.Code}");
                    counter++;
                }
            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\enumerator_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\enumerator_select.csv"); 
            FilesCount++;


            //generate effort_spec section
            sb = new StringBuilder(header);
            NSAPEntities.GearViewModel.FillGearEffortSpecifications();
            foreach (var item in NSAPEntities.GearViewModel.GearEffortSpecifications)
            {
                sb.AppendLine($"effort_spec,{item.Value.EffortSpecification.ID},{item.Value.EffortSpecification.Name},{item.Value.Gear.Code}");
                counter++;
            }

            //next we add lines for gear specs for "Others"
            foreach (var item in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection
                .Where(t => t.IsForAllTypesFishing))
            {
                sb.AppendLine($"effort_spec,{item.ID},{item.Name},_OT");
                counter++;
            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\effort_spec_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\effort_spec_select.csv");
            FilesCount++;


            //GPS section
            sb = new StringBuilder(header);
            foreach (var gps in NSAPEntities.GPSViewModel.GPSCollection.OrderBy(t => t.AssignedName))
            {
                sb.AppendLine($"gps,{gps.Code},{gps.AssignedName},gps");
                counter++;
            }
            //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\gps_select.csv");
            await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\gps_select.csv");
            FilesCount++;


            return counter;
        }
        public static async Task<int> GenerateItemsetsCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("list_name,name,label,option\r\n");


            //generate fma section
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in region.FMAs.OrderBy(t => t.FMA.Name))
                {
                    sb.AppendLine($"fma,{fma.RowID},{fma.FMA.Name},{fma.NSAPRegion.Code}");
                    counter++;
                }

            }


            //generate fishing ground section
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in region.FMAs)
                {
                    foreach (var fg in fma.FishingGrounds)
                    {
                        sb.AppendLine($"fg,{fg.RowID},{fg.FishingGround.Name},{fg.RegionFMA.RowID}");
                        counter++;
                    }
                }
            }


            //generate ls (landing site) section
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var fma in region.FMAs)
                {
                    foreach (var fg in fma.FishingGrounds)
                    {
                        foreach (var ls in fg.LandingSites
                            .OrderBy(t => t.LandingSite.ToString()))
                        {
                            sb.AppendLine($"ls,{ls.RowID},\"{ls.LandingSite.ToString()}\",{fg.RowID}");
                            counter++;
                        }
                    }
                }
            }

            //generate gear section
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                 .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var gear in region.Gears.OrderBy(t => t.NSAPRegion.Name).ThenBy(t => t.Gear.GearName))
                {
                    sb.AppendLine($"gear,{gear.RowID},{gear.Gear.GearName},{region.Code}");
                    counter++;
                }
            }

            //generate enumerator section
            foreach (var region in NSAPEntities.NSAPRegionViewModel.NSAPRegionCollection
                 .Where(t => NSAPEntities.Regions.Contains(t.Code)))
            {
                foreach (var enumerator in region.NSAPEnumerators.OrderBy(t => t.Enumerator.Name))
                {
                    sb.AppendLine($"enumerator,{enumerator.RowID},{enumerator.Enumerator.Name},{region.Code}");
                    counter++;
                }
            }

            //generate effort_spec section
            NSAPEntities.GearViewModel.FillGearEffortSpecifications();
            foreach (var item in NSAPEntities.GearViewModel.GearEffortSpecifications)
            {
                sb.AppendLine($"effort_spec,{item.Value.EffortSpecification.ID},\"{item.Value.EffortSpecification.Name}\",{item.Value.Gear.Code}");
                counter++;
            }




            //next we add lines for gear specs for "Others"
            foreach (var item in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection
                .Where(t => t.IsForAllTypesFishing))
            {
                sb.AppendLine($"effort_spec,{item.ID},{item.Name},_OT");
                counter++;
            }


            //GPS section
            foreach (var gps in NSAPEntities.GPSViewModel.GPSCollection.OrderBy(t => t.AssignedName))
            {
                sb.AppendLine($"gps,{gps.Code},{gps.AssignedName},gps");
                counter++;
            }

            if (counter > 0)
            {
                //Logger.LogType = LogType.ItemSets_csv;
                //await Logger.LogAsync(sb.ToString(), $"{_folderSaveLocation}\\itemsets.csv");
                await LogAsync(sb.ToString(), $"{_folderSaveLocation}\\itemsets.csv");
            }
            FilesCount++;
            //Logger.LogType = LogType.Logfile;
            return counter;
        }
        public static async Task<int> GenerateEffortSpecCSV()
        {
            int counter = 0;
            StringBuilder sb = new StringBuilder("ID_key,spec_key,value_type_key\r\n");
            foreach (var spec in NSAPEntities.EffortSpecificationViewModel.EffortSpecCollection)
            {
                string valueType = "I";
                switch (spec.ValueType)
                {
                    case ODKValueType.isBoolean:
                        valueType = "B";
                        break;
                    case ODKValueType.isInteger:
                        valueType = "I";
                        break;
                    case ODKValueType.isDecimal:
                        valueType = "D";
                        break;
                    case ODKValueType.isText:
                        valueType = "T";
                        break;
                    case ODKValueType.isUndefined:
                        valueType = "U";
                        break;
                }
                sb.AppendLine($"{spec.ID},\"{spec.Name}\",{valueType}");
                counter++;
            }
            if (counter > 0)
            {
                //Logger.LogType = LogType.EffortSpec_csv;
                //await Logger.LogAsync(sb.ToString(), _fileName);
                await LogAsync(sb.ToString(), _fileName);
            }
            //Logger.LogType = LogType.Logfile;
            return counter;
        }
    }
}
