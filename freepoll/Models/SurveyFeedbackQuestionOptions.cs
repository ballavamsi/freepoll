using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class SurveyFeedbackQuestionOptions
    {
        public int Id { get; set; }
        public string CustomAnswer { get; set; }
        public DateTime? InsertedDatetime { get; set; }
        public int? SurveyFeedbackId { get; set; }
        public int? SurveyQuestionId { get; set; }
        public string SurveyQuestionOptionId { get; set; }
    }
}
