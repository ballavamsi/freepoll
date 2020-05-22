using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{
    public class PollVoteRequestViewModel
    {
        public int pollId { get; set; }
        public List<string> options { get; set; }
    }
}
