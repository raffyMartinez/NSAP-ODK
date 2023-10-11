using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public static class CatchNameURLGenerator
    {
        private static Dictionary<string, string> _urls = new Dictionary<string, string>();
        private static string _catchName;

        public static string CatchName
        {
            get { return _catchName; }
            set
            {
                if (_catchName != value)
                {
                    _catchName = value;
                    _urls.Clear();
                    _urls.Add("Wikipaedia", $"https://en.wikipedia.org/wiki/{_catchName.Replace(' ', '_')}");
                    _urls.Add("FishBase", $"https://www.fishbase.de/summary/{_catchName.Replace(' ', '-')}.html");
                    _urls.Add("SeaLifeBase", $"http://www.sealifebase.org/summary/{_catchName.Replace(' ', '-')}.html");
                    _urls.Add("World Register of Marine Species", $"http://www.marinespecies.org/aphia.php?p=taxlist&tName={_catchName}");
                    _urls.Add("Google image search", $"https://www.google.com/images?q={_catchName.Replace(' ', '+')}");
                    _urls.Add("Google scholar", $"http://scholar.google.com/scholar?q={_catchName.Replace(" ", "%20")}");
                }
            }
        }

        public static Dictionary<string, string> URLS
        {
            get { return _urls; }
        }
    }
}
