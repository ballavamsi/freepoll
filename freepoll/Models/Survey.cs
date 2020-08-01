using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class Survey
    {
        public int Surveyid { get; set; }
        public string Welcometitle { get; set; }
        public string Welcomedescription { get; set; }
        public string Endtitle { get; set; }
        public string Welcomeimage { get; set; }
        public int Allowduplicate { get; set; }
        public DateTime? Enddate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string SurveyGuid { get; set; }
        public int Emailidrequired { get; set; }
        public int Askemail { get; set; }
    }
}
