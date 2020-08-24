using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.ViewModels
{
    public class DashboardMetricsViewModel
    {
        public int polls { get; set; }
        public int surveys { get; set; }
        public int pollVotes { get; set; }
        public int surveyFeedbacks { get; set; }
    }
}
