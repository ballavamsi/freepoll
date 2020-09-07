using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{

    public class SurveyMetric
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string logo { get; set; }
        public List<QuestionMetric> Questions { get; set; }
    }
    public class QuestionMetric
    {
        public string Question { get; set; }
        public string Explanation { get; set; }
        public string QuestionType { get; set; }
        public List<OptionsMetric> options { get; set; }
    }

    public class OptionsMetric
    {
        public int optionId { get; set; }
        public string optionText { get; set; }
        public int optionCount { get; set; }
        public double optionAverage { get; set; }
    }
}
