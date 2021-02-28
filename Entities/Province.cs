using System.ComponentModel;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using NSAP_ODK.Entities.ItemSources;
namespace NSAP_ODK.Entities
{

    public class ProvinceEdited
    {
        public ProvinceEdited(Province p)
        {

        }
        public int ProvinceID { get; set; }
        public string RegionCode { get; set; }
        public string ProvinceName { get; set; }

        public NSAPRegion NSAPRegion { get; set; }
    }

    public class Province
    {
        private NSAPRegion _nsapRegion;
        public Province()
        {
        }

        [ReadOnly(true)]
        public int ProvinceID { get; set; }

       [ItemsSource(typeof(NSAPRegionItemsSource))]
        public string RegionCode { get; set; }
        public string ProvinceName { get; set; }


        public NSAPRegion NSAPRegion {
            get
            {
                if (_nsapRegion == null)
                {
                    _nsapRegion = NSAPEntities.NSAPRegionViewModel.GetNSAPRegion(RegionCode);
                }
                return _nsapRegion;
            }
            set 
            {
                _nsapRegion = value;
                RegionCode = _nsapRegion.Code;
            }
        }
        public int MunicipalityCount
        {
            get {
                if (Municipalities == null)
                {
                    return 0;
                }
                else
                {
                    return Municipalities.Count;
                }
            }
        }

        public void SetMunicipalities()
        {
            if (ProvinceID > 0 && ProvinceName != null)
                Municipalities = new MunicipalityViewModel(this);
        }

        public MunicipalityViewModel Municipalities { get; set; }

        public override string ToString()
        {
            return ProvinceName;
        }



        public string Acronym
        {
            get
            {
                string withouFirstChar = "";
                string acronym = ProvinceName.Substring(0, 1);
                var arr = ProvinceName.Split(new char[] { ' ' });
                if(arr.Length==1)
                {
                    withouFirstChar = arr[0].Substring(1, arr[0].Length - 1);
                    if (withouFirstChar.Length <= 3)
                    {
                        acronym += withouFirstChar;
                    }
                    else
                    {
                        acronym += Regex.Replace(withouFirstChar, "[aeiou]", "", RegexOptions.IgnoreCase).Substring(0, 3);
                    }
                }
                else if(arr.Length==2)
                {
                    withouFirstChar = arr[0].Substring(1, arr[0].Length - 1);
                    acronym += Regex.Replace(withouFirstChar, "[aeiou]", "", RegexOptions.IgnoreCase).Substring(0, 1);
                    
                    acronym += arr[1].Substring(0, 1);
                    withouFirstChar = arr[1].Substring(1, arr[1].Length - 1);                    
                    acronym += Regex.Replace(withouFirstChar, "[aeiou]", "", RegexOptions.IgnoreCase).Substring(0, 1);
                }
                else
                {
                    for(int x=0;x<arr.Length;x++)
                    {
                        if(x==0)
                        {
                            withouFirstChar = arr[0].Substring(1, arr[0].Length - 1);
                            acronym += Regex.Replace(withouFirstChar, "[aeiou]", "", RegexOptions.IgnoreCase).Substring(0, 1);
                        }
                        else
                        {
                            acronym += arr[x].Substring(0, 1);
                        }
                    }
                }

                return acronym.ToUpper();
            }
        }
    }
}