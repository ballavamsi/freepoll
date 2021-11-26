using freepoll.Helpers;
using System;
using System.Collections.Generic;

namespace freepoll.ViewModels
{
    public class NewPollViewModel
    {
        public string name { get; set; }
        public List<string> options { get; set; }
        public string type { get; set; }
        public string duplicate { get; set; }
        public DateTime endDate { get; set; }

        public int status { get; set; }

        public string pollGuid { get; set; } = ShortUrl.GenerateShortUrl();
    }
}