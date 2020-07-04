using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{
    public class SurveyQuestionsViewModel
    {
        public int SurveyQuestionId { get; set; }
        public int SurveyId { get; set; }
        public int TypeId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Isrequired { get; set; }
        public int QuestionDisplayOrder { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? StatusId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public Dictionary<string,string> Options { get; set; }

    }
}
