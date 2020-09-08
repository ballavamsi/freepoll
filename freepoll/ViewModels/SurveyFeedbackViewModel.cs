using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{
    public class SurveyFeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public string Comment { get; set; }
        public int reviewComplete { get; set; }
    }
}
