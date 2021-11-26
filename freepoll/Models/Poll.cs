using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class Poll
    {
        public int PollId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Duplicate { get; set; }
        public DateTime? Enddate { get; set; }
        public string Name { get; set; }
        public string PollGuid { get; set; }
        public int? StatusId { get; set; }
        public int? Type { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
