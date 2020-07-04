using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{
    public class SurveyViewModel
    {
        public string Welcometitle { get; set; }
        public string Endtitle { get; set; }
        public string Welcomeimage { get; set; }
        public int Allowduplicate { get; set; }
        public DateTime? Enddate { get; set; }
        public int Emailidrequired { get; set; }

        public List<SurveyQuestionsViewModel> SurveyQuestions { get; set; }
    }
}
