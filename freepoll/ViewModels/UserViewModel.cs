using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{
    public class UserViewModel
    {
        public int userid { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string password { get; set; }
        public string github { get; set; }
        public string google { get; set; }

        public string facebook { get; set; }
        public DateTime createdTime { get; set; }
        public string userGuid { get; set; }
        public int status { get; set; }

    }

    public class UserResponseViewModel
    {
        public string UserGuid { get; set; }

        public string Response { get; set; }
    }


    public class LoginViewModel
    {
        public string loginemail { get; set; }
        public string loginpassword { get; set; }

        public Social platformdetail { get; set; }


    }

    public class Social
    {
        public string platform { get; set; }
        public string platformid { get; set; }
    }
}
