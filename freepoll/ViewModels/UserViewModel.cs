using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{

    public class UserResponseViewModel
    {
        public string UserGuid { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string profileUrl { get; set; }
        public string Response { get; set; }
    }


    public class LoginViewModel
    {
        public string email { get; set; }
        public string name { get; set; }
        public Social platformdetail { get; set; }


    }

    public class Social
    {
        public string platform { get; set; }
        public string platformid { get; set; }
        public string platformImage { get; set; }
    }

    public class UserPoll
    {
        public int pollId { get; set; }
        public string pollGuid { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }

        public int votes { get; set; }

        public string pollName { get; set; }
    }

    public class UserPollResponse
    {
        public List<UserPoll> userPolls { get; set; }
        public int totalPolls { get; set; }

        public string Response { get; set; }
    }
}
