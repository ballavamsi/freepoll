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


    public class UserSurvey
    {
        public int surveyId { get; set; }
        public string surveyGuid { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }

        public int feedbacks { get; set; }

        public string surveyName { get; set; }
    }

    public class UserSurveyResponse
    {
        public List<UserSurvey> userSurveys { get; set; }
        public int totalSurveys { get; set; }

        public string Response { get; set; }
    }


    public class Feedbacks
    {
        public int surveyUserId { get; set; }
        public string surveyUserGuid { get; set; }
        public string EmailId { get; set; }
        public DateTime? receivedDate { get; set; }
        public string Comment { get; set; }
        public bool reviewComplete { get; set; }
        public DateTime? reviewUpdatedDate { get; set; }
    }

    public class UserFeedbackResponse
    {
        public string surveyTitle { get; set; }
        public string surveyLogo { get; set; }
        public List<Feedbacks> feedbacks { get; set; }
        public int total { get; set; }
    }
}
