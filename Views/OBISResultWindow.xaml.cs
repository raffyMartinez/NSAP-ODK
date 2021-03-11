using NSAP_ODK.Entities;
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
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace NSAP_ODK.Views
{
    /// <summary>
    /// Interaction logic for OBISResultWindow.xaml
    /// </summary>
    public partial class OBISResultWindow : Window
    {
        private OBIResponseRoot _response;
        public OBISResultWindow(OBIResponseRoot response)
        {
            InitializeComponent();
            _response = response;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_response.total > 0)
            {
                propertyGrid.SelectedObject = _response.results[0];
                //propertyGrid.NameColumnWidth = 350;
                propertyGrid.AutoGenerateProperties = false;

                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "acceptedNameUsage", DisplayName = "Accepted name", DisplayOrder = 1, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "acceptedNameUsageID", DisplayName = "Accepted name ID", DisplayOrder = 2, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "kingdom", DisplayName = "Kingdom", DisplayOrder = 3, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "kingdomid", DisplayName = "Kingdom ID", DisplayOrder = 4, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "phylum", DisplayName = "Phylum", DisplayOrder = 5, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "phylumid", DisplayName = "Phylum ID", DisplayOrder = 6, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "subphylum", DisplayName = "Subphylum", DisplayOrder = 7, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "subphylumid", DisplayName = "Subphylum ID", DisplayOrder = 8, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "superclass", DisplayName = "Superclass", DisplayOrder = 9, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "superclassid", DisplayName = "Superclass ID", DisplayOrder = 10, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "@class", DisplayName = "Class", DisplayOrder = 11, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "classid", DisplayName = "Class ID", DisplayOrder = 12, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "order", DisplayName = "Order", DisplayOrder = 13, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "orderid", DisplayName = "Order ID", DisplayOrder = 14, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "suborder", DisplayName = "Suborder", DisplayOrder = 15, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "suborderid", DisplayName = "Suboirder ID", DisplayOrder = 16, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "family", DisplayName = "Family", DisplayOrder = 17, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "familyid", DisplayName = "Family ID", DisplayOrder = 18, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "genus", DisplayName = "Genus", DisplayOrder = 19, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "genusid", DisplayName = "Genus ID", DisplayOrder = 20, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "species", DisplayName = "Species", DisplayOrder = 21, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "speciesid", DisplayName = "Species ID", DisplayOrder = 22, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "scientificName", DisplayName = "Scientific name", DisplayOrder = 23, Description = "NSAP region", Category = "Identification" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "scientificNameAuthorship", DisplayName = "Authorship", DisplayOrder = 24, Description = "NSAP region", Category = "Identification" });

                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "taxonomicStatus", DisplayName = "Taxonomic status", DisplayOrder = 25, Description = "NSAP region", Category = "Taxonomy" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "taxonRank", DisplayName = "Rank", DisplayOrder = 26, Description = "NSAP region", Category = "Taxonomy" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "taxonID", DisplayName = "Taxon ID", DisplayOrder = 27, Description = "NSAP region", Category = "Taxonomy" });

                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "is_marine", DisplayName = "Is marine", DisplayOrder = 1, Description = "NSAP region", Category = "Occurence" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "is_brackish", DisplayName = "Is brackish", DisplayOrder = 2, Description = "NSAP region", Category = "Occurence" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "is_freshwater", DisplayName = "Is freshwater", DisplayOrder = 3, Description = "NSAP region", Category = "Occurence" });
                propertyGrid.PropertyDefinitions.Add(new PropertyDefinition { Name = "is_terrestrial", DisplayName = "Is terrestrial", DisplayOrder = 4, Description = "NSAP region", Category = "Occurence" });
            }
        }
    }
}
