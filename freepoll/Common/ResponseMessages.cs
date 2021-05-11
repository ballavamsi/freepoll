using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.Common
{
    public static class ResponseMessages
    {
        public static class Messages
        {
            public const string UnauthorizedUserError = "Unauthorized User";
            public const string Inactiveusererror = "InactiveUser";
            public const string PollNotFoundError = "PollNotFound";
            public const string UserNotFoundError = "UserNotFound";
            public const string PollDeleteSuccess = "Poll Deleted Successfully";
            public const string PollEnded = "PollEnded";
            public const string PollVoted = "PollVoted";
            public const string SurveyNotFoundError = "SurveyNotFound";
            public const string SurveyUserNotFoundError = "SurveyUserNotFound";
            public const string SurveyEnded = "SurveyEnded";
            public const string SurveyAlreadyTaken = "SurveyAlreadyTaken";
            public const string SurveyWasNotStartedByUser = "SurveyWasNotStartedByUser";
            public const string SurveyWasAlreadySubmitted = "SurveyWasAlreadySubmitted";
            public const string FeedbackNotFound = "FeedbackNotFound";
        }
    }
}
