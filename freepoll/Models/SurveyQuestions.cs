using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class SurveyQuestions
    {
        public int SurveyQuestionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Isrequired { get; set; }
        public int QuestionDisplayOrder { get; set; }
        public int? StatusId { get; set; }
        public string Subtitle { get; set; }
        public int? SurveyId { get; set; }
        public string Title { get; set; }
        public int TypeId { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
