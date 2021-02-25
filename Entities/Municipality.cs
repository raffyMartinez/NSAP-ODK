using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
namespace NSAP_ODK.Entities
{
    public class MunicipalityEdit
    {
        public MunicipalityEdit(Municipality municipality)
        {
            Municipality = municipality;
            MunicipalityID = municipality.MunicipalityID;
            MunicipalityName = municipality.MunicipalityName;
            ProvinceID = municipality.Province.ProvinceID;
            Longitude = municipality.Longitude;
            Latitude = municipality.Latitude;
            IsCoastal = municipality.IsCoastal;
        }

        public MunicipalityEdit()
        {
        }

        public Municipality Municipality { get; set; }
        public int MunicipalityID { get; set; }
        public string MunicipalityName { get; set; }

        [ItemsSource(typeof(ProvinceItemsSource))]
        public int ProvinceID { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public bool IsCoastal { get; set; }
    }

    public class Municipality
    {
        public int MunicipalityID { get; set; }
        public string MunicipalityName { get; set; }
        public Province Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public bool IsCoastal { get; set; }

        public override string ToString()
        {

            return $"{MunicipalityName}, {Province.ProvinceName}";

        }

        public string Acronym
        {
            get
            {
                string acronym = "";
                var mun_arr = MunicipalityName.Split(new char[] { ' ' });
                if (mun_arr.Length == 1)
                {
                    if (mun_arr[0].Length <= 3)
                    {
                        acronym = mun_arr[0];
                    }
                    else
                    {
                        acronym = mun_arr[0].Substring(0, 3);
                    }
                }
                else if (mun_arr[0].Length == 2)
                {
                    acronym = mun_arr[0].Substring(0, 2) + mun_arr[1].Substring(0, 1);
                }
                else
                {
                    bool middleDone = false; 
                    for (int x = 0; x < mun_arr.Length; x++)
                    {
                        if(x==0)
                        {
                            acronym = mun_arr[0].Substring(0, 1);
                        }
                        else if(mun_arr[x].Length==2 && mun_arr[x].Substring(1,1)=="." && !middleDone)
                        {
                            acronym += mun_arr[x].Substring(0, 1);
                            middleDone = true;
                            break;
                        }
                        else if(mun_arr[x].Length==1 && !middleDone)
                        {
                            acronym += mun_arr[x];
                            middleDone = true;
                            break;
                        }
                        else if(x == mun_arr.Length-2)
                        {
                            acronym += mun_arr[1].Substring(0, 1);
                        }
                    }

                    acronym += mun_arr[mun_arr.Length - 1].Substring(0, 1);
                }
                acronym += Province.Acronym;
                return acronym.ToUpper();

            }
        }
    }
}