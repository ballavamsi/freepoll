using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class SurveyUserQuestionValues
    {
        public int SurveyUserQuestionValueId { get; set; }
        public int SurveyUserQuestionId { get; set; }
        public int SurveyUserTypeId { get; set; }
        public int SurveyUserId { get; set; }
    }
}
