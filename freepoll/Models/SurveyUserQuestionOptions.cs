using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class SurveyUserQuestionOptions
    {
        public int Id { get; set; }
        public int SurveyUserId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string SurveyQuestionOptionId { get; set; }
        public DateTime? InsertedDatetime { get; set; }
        public string CustomAnswer { get; set; }
    }
}
