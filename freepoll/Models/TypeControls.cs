using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class TypeControls
    {
        public int TypeControlId { get; set; }
        public int TypeId { get; set; }
        public int ControlId { get; set; }
        public int DisplayOrder { get; set; }
        public int? IsActive { get; set; }
    }
}
