using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace freepoll.Models
{
    public partial class DataStarOptions
    {
        public int StarOptionId { get; set; }
        public int? DisplayOrder { get; set; }
        public int? IsActive { get; set; }
        public string OptionDisplayText { get; set; }
        public string OptionText { get; set; }
    }
}
