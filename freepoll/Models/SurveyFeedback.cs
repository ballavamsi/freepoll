using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class SurveyFeedback
    {
        public int SurveyFeedbackId { get; set; }
        public DateTime? CompletedDatetime { get; set; }
        public DateTime? InsertedDatetime { get; set; }
        public string ReviewComment { get; set; }
        public int? ReviewCompleted { get; set; }
        public DateTime? ReviewDatetime { get; set; }
        public int SurveyId { get; set; }
        public string SurveyUserEmail { get; set; }
        public string SurveyUserGuid { get; set; }
    }
}
