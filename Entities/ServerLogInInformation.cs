using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Entities
{
    public class ServerLogInInformation
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public bool CanLogIn
        {
            get
            {
                return
                    !string.IsNullOrEmpty(UserName) &&
                    !string.IsNullOrEmpty(Password) &&
                    !string.IsNullOrEmpty(Token);
            }
        }
    }
}
