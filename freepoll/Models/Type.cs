﻿using System;
using System.Collections.Generic;

namespace freepoll.Models
{
    public partial class Type
    {
        public int? TypeId { get; set; }
        public string TypeValue { get; set; }
        public int IsActive { get; set; }
    }
}
