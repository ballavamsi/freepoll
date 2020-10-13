using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class SurveyFeedback
    {
        public int SurveyFeedbackId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyUserEmail { get; set; }
        public string SurveyUserGuid { get; set; }
        public DateTime? InsertedDatetime { get; set; }
        public DateTime? CompletedDatetime { get; set; }
        public string ReviewComment { get; set; }
        public int? ReviewCompleted { get; set; }
        public DateTime? ReviewDatetime { get; set; }
    }
}
