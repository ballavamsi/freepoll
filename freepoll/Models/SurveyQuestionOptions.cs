using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class SurveyQuestionOptions
    {
        public int SurveyQuestionOptionId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string OptionKey { get; set; }
        public string OptionValue { get; set; }
        public int? DisplayOrder { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
