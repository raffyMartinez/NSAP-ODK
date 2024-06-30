﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace NSAP_ODK.Mapping.views
{
    /// <summary>
    /// Interaction logic for GridMappingWindow.xaml
    /// </summary>
    public partial class GridMappingWindow : Window
    {
        private static GridMappingWindow _instance;
        public GridMappingWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
            Closed += OnWindowClosed;
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            _instance = null;
        }

        public static GridMappingWindow GetInstance()
        {
            if (_instance == null) _instance = new GridMappingWindow();
            return _instance;
        }
        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner.Focus();
            //_instance = null;

        }

        public bool MultipleAOIs { get; set; }
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            labelTitle.Content = "Grid mapping of AOIs";

        }

        public AOI AOI { get; set; }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            if (b.Name == "buttonMapClose")
            {
                Close();
            }
            else
            {
                foreach (var aoi in Entities.AOIViewModel.GetSelectedAOIs())
                {
                    if (Grid25.ProjectionIsWGS84(aoi.SubGrids.GeoProjection.Name))
                    {
                        //MapWindowManager.SelectTracksInAOI(aoi);
                        aoi.GridMapping.SelectedTrackIndexes = MapWindowManager.SelectedTrackIndexes;

                        if (MapWindowManager.SelectedTrackIndexes?.Count() > 0)
                        {
                            aoi.GridMapping.SelectedTracks = MapWindowManager.SelectedTracks;
                            if ((bool)checkMapIntensity.IsChecked)
                            {

                                var count = aoi.GridMapping.ComputeFishingFrequency();
                                if(count>0)
                                {
                                    aoi.EffortGridColumn = "Hits";
                                }
                                textStatus.Text += $"{count} cells were computed for fishing inntensity for {aoi.Name}\r\n";
                            }
                        }
                        else
                        {
                            textStatus.Text += $"{aoi.Name} does not contain fishing tracks\r\n";
                        }
                    }
                    else
                    {
                        textStatus.Text += $"{aoi.Name} grids is not projected to WGS84\r\n";
                    }
                }
            }

        }
    }
}
