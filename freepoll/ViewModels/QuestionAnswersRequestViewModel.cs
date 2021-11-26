using System.Collections.Generic;

namespace freepoll.ViewModels
{
    public class QuestionAnswersViewModel
    {
        public string key { get; set; }
        public string text { get; set; }
        public int number { get; set; }
        public List<string> selected { get; set; }
    }

    public class QuestionAnswersRequestViewModel
    {
        public List<QuestionAnswersViewModel> data { get; set; }
    }
}