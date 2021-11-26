using System;

namespace freepoll.Models
{
    public partial class SurveyFeedbackQuestionOptions
    {
        public int Id { get; set; }
        public int SurveyFeedbackId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string SurveyQuestionOptionId { get; set; }
        public DateTime? InsertedDatetime { get; set; }
        public string CustomAnswer { get; set; }
    }
}