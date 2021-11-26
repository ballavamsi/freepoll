using System.Collections.Generic;

namespace freepoll.ViewModels
{
    public class PollVoteOptionResponseViewModel
    {
        public string label { get; set; }
        public int count { get; set; }
    }

    public class PollVoteResponseViewModel
    {
        public string Question { get; set; }
        public List<PollVoteOptionResponseViewModel> Options { get; set; }
        public List<PollVoteOptionResponseViewModel> Regions { get; set; }
    }
}