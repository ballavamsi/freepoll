using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class SurveyQuestionOptions
    {
        public int SurveyQuestionOptionId { get; set; }
        public int? CreatedBy { get; set; }
        public int? DisplayOrder { get; set; }
        public string OptionKey { get; set; }
        public string OptionValue { get; set; }
        public int? SurveyQuestionId { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
