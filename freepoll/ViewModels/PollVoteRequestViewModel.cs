using System.Collections.Generic;

namespace freepoll.ViewModels
{
    public class PollVoteRequestViewModel
    {
        public int pollId { get; set; }
        public List<string> options { get; set; }
    }
}