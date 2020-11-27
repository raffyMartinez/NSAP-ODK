using System;
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
using NSAP_ODK.Entities;
using NSAP_ODK.Utilities;
namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for QueryAPIWIndow.xaml
    /// </summary>
    public partial class QueryAPIWIndow : Window
    {
        MainWindow _parent;
        public QueryAPIWIndow(MainWindow parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private async Task<bool> QueryFishbaseAPI(string apiQuery)
        {
            txtAPIResult.Text = "";
            int loopCount = 0;
            int max = 500;
            int i = 0;
            if (apiQuery.Length > 0)
            {
                var arr = apiQuery.Split(' ');
                if (apiQuery == "UpdateBlankLen")
                {
                    foreach (var sp in NSAPEntities.FishSpeciesViewModel.SpeciesCollection.ToList())
                    {
                        if (sp.LengthMax != null && sp.LengthType == null)
                        {
                            var result = await NSAPEntities.FishSpeciesViewModel.GetSpeciesDataFromAPI(sp, true);
                            if (!result.Success)
                            {
                                txtAPIResult.Text += ($"Could not show detail for {sp.GenericName} {sp.SpecificName}\r\n {result.Message} \r\n");
                            }
                            else
                            {
                                txtAPIResult.Text += $"{i + 1}: Succeeded in getting data for {sp.GenericName} {sp.SpecificName} \r\n";
                                i++;
                            }
                        }
                    }
                    _parent.RefreshSpeciesGrid(i);

                    return true;
                }
                else if (arr.Count() == 1 && arr[0] == "all")
                {
                    foreach (var sp in NSAPEntities.FishSpeciesViewModel.SpeciesCollection.ToList())
                    {
                        if (sp.SpeciesCode != null)
                        {
                            var result = await NSAPEntities.FishSpeciesViewModel.GetSpeciesDataFromAPI(sp, true);
                            if (!result.Success)
                            {
                                txtAPIResult.Text += ($"Could not show detail for {sp.GenericName} {sp.SpecificName}\r\n {result.Message} \r\n");
                            }
                            else
                            {
                                txtAPIResult.Text += $"{i + 1}: Succeeded in getting data for {sp.GenericName} {sp.SpecificName} \r\n";
                                i++;
                            }
                        }
                    }
                    _parent.RefreshSpeciesGrid(i);
                    return true;
                }
                else if (arr.Count() == 2 && arr[0] == "all" && int.TryParse(arr[1], out int start))
                {
                    foreach (var sp in NSAPEntities.FishSpeciesViewModel.SpeciesCollection.ToList())
                    {
                        if (sp.SpeciesCode != null && sp.RowNumber > start)
                        {
                            var result = await NSAPEntities.FishSpeciesViewModel.GetSpeciesDataFromAPI(sp, true);
                            if (!result.Success)
                            {
                                txtAPIResult.Text += ($"Could not show detail for {sp.GenericName} {sp.SpecificName}\r\n {result.Message} \r\n");
                            }
                            else
                            {
                                txtAPIResult.Text += $"{i + 1}: Succeeded in getting data for {sp.GenericName} {sp.SpecificName} \r\n";
                                i++;
                            }
                        }
                    }
                    _parent.RefreshSpeciesGrid(i);
                    return true;
                }
                else if (int.TryParse(apiQuery, out int v) && v > 0 && v <= max)
                {
                    foreach (var sp in NSAPEntities.FishSpeciesViewModel.SpeciesCollection.ToList())
                    {
                        var result = await NSAPEntities.FishSpeciesViewModel.GetSpeciesDataFromAPI(sp, true);
                        if (!result.Success)
                        {
                            txtAPIResult.Text += ($"Could not show detail for {sp.GenericName} {sp.SpecificName}\r\n {result.Message} \r\n");
                        }
                        else
                        {
                            txtAPIResult.Text += $"{loopCount + 1}: Succeeded in getting data for {sp.GenericName} {sp.SpecificName} \r\n";
                            loopCount++;
                            if (loopCount >= v)
                            {
                                _parent.RefreshSpeciesGrid(v);
                                return true; ;
                            }
                        }
                    }

                    MessageBox.Show("There are no species left to update!");
                }
                else if (v < 1 || v > 100)
                {
                    MessageBox.Show($"API query count must be from 1 to {max}");
                }
                else
                {
                    MessageBox.Show($"Please provide a value from 1 to {max}");
                }
            }
            return false;
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "ButtonOk":
                    try
                    {
                        if (TextBoxQuery.Text.Length>0 && await QueryFishbaseAPI(TextBoxQuery.Text))
                        {
                            Close();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("An internet connection is required for this operation", "Internet connection required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "ButtonCancel":
                    Close();
                    break;
            }
        }

        private void ClosingTrigger(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SavePlacement();
        }
    }
}
