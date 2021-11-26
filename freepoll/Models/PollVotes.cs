using System;

namespace freepoll.Models
{
    public partial class PollVotes
    {
        public int PollVoteId { get; set; }
        public int PollId { get; set; }
        public int OptionId { get; set; }
        public string IpAddress { get; set; }
        public string UserLocation { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}