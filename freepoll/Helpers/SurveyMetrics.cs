using AutoMapper;
using freepoll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace freepoll.Helpers
{
    public partial class SurveyMetrics
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;

        public SurveyMetrics(FreePollDBContext dBContext, IMapper mapper)
        {
            _dBContext = dBContext;
            _mapper = mapper;
        }
    }
}
