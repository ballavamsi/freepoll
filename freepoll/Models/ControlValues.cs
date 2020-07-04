using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class ControlValues
    {
        public int ControlvalueId { get; set; }
        public int ControlId { get; set; }
        public string Minlength { get; set; }
        public string Maxlength { get; set; }
        public string Placeholder { get; set; }
        public string Imageupload { get; set; }
        public string TextArea { get; set; }
        public string TextBox { get; set; }
    }
}
