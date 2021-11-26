using System;

namespace freepoll.Models
{
    public partial class Poll
    {
        public int PollId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int Duplicate { get; set; }
        public DateTime? Enddate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string PollGuid { get; set; }
    }
}