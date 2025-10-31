using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace NSAP_ODK.Entities
{
    public class NSAPEnumerator
    {
        [ReadOnly(true)]
        public int ID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} ID:{ID}";
        }


        public string Acronym
        {
            get
            {
                string acronym = "";
                var names = Name.Split(new char[] { ' ' });
                if (names.Count() == 2)
                {
                    acronym = names[0].Substring(0, 3) + names[1].Substring(0, 2);

                }
                else
                {
                    bool middleDone = false;
                    for (int x = 0; x < names.Count(); x++)
                    {
                        if(x==0)
                        {
                            acronym = names[x].Substring(0, 2);
                        }
                        else if(names[x].Substring(names[x].Length-1,1)=="." && !middleDone)
                        {
                            acronym += names[x].Substring(0, 1);
                            middleDone = true;
                            break;
                        }
                        else if(names[x].Length==1 && !middleDone)
                        {
                            acronym += names[x].Substring(0, 1);
                            middleDone = true;
                            break;
                        }
                    }

                    acronym += names[names.Count() - 1].Substring(0, 2);
                }

                return acronym.ToUpper();
            }
        }
    }
}
