using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class PollVotes
    {
        public int PollVoteId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string IpAddress { get; set; }
        public int? OptionId { get; set; }
        public int PollId { get; set; }
        public string UserLocation { get; set; }
    }
}
