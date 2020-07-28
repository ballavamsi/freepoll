using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class SurveyUserQuestionOptions
    {
        public int Id { get; set; }
        public int SurveyUserQuestionId { get; set; }
        public int SurveyUserQuestionOptionid { get; set; }
        public string SurveyUserQuestionOptionvalueq { get; set; }
    }
}
