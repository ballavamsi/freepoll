using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class SurveyUser
    {
        public int SurveyUserId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyUserEmail { get; set; }
        public string SurveyUserGuid { get; set; }
        public DateTime? InsertedDatetime { get; set; }
        public DateTime? CompletedDatetime { get; set; }
    }
}
