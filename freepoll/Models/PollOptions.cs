using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class PollOptions
    {
        public int PollOptionId { get; set; }
        public int? PollId { get; set; }
        public string OptionText { get; set; }
        public int StatusId { get; set; }
        public int OrderId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
