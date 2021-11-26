using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class Survey
    {
        public int Surveyid { get; set; }
        public int Allowduplicate { get; set; }
        public int? CreatedBy { get; set; }
        public int Emailidrequired { get; set; }
        public int Enableprevious { get; set; }
        public DateTime? Enddate { get; set; }
        public string Endtitle { get; set; }
        public int? StatusId { get; set; }
        public string SurveyGuid { get; set; }
        public int? UpdatedBy { get; set; }
        public string Welcomedescription { get; set; }
        public string Welcomeimage { get; set; }
        public string Welcometitle { get; set; }
        public int? Askemail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
