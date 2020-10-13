using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using freepoll.Common;
using freepoll.Models;
using freepoll.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static freepoll.Common.ResponseMessages;

namespace freepoll.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("tilemetrics")]
        [HttpGet]
        public IActionResult GetTileMetrics()
        {
            DashboardMetricsViewModel dashboardMetricsViewModel = new DashboardMetricsViewModel();

            string userguid = Request.Headers[Constants.UserToken];
            string decyrptstring = Security.Decrypt(userguid);
            if (string.IsNullOrEmpty(decyrptstring)) return BadRequest("Unauthorized User");
            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();
            if (user == null) return BadRequest(Messages.UserNotFoundError);

            List<Poll> polls = _dBContext.Poll.Where(x => x.CreatedBy == user.Userid && x.StatusId != 3).ToList();

            //Update total Polls
            dashboardMetricsViewModel.polls = polls.Count;

            List<int> pollIds = polls.Select(x => x.PollId).ToList();
            List<PollVotes> pollVotes = (from eachPoll in _dBContext.PollVotes
                                         where pollIds.Contains(eachPoll.PollId)
                                         select eachPoll).ToList();
            var pollVotesReceived = (from eachPoll in pollVotes
                                     group new { eachPoll.PollId } by new { eachPoll.CreatedDate, eachPoll.PollId } into eachGroup
                                     select eachGroup).ToList();

            //Update total Poll Votes
            dashboardMetricsViewModel.pollVotes = pollVotesReceived.Count;

            List<Survey> surveys = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.StatusId != 3).ToList();

            //Update total Surveys
            dashboardMetricsViewModel.surveys = surveys.Count;

            List<int> surveyIds = surveys.Select(x => x.Surveyid).ToList();
            List<SurveyFeedback> surveyUsers = (from eachSurvey in _dBContext.SurveyFeedback
                                            where surveyIds.Contains(eachSurvey.SurveyId) && eachSurvey.CompletedDatetime != null
                                            select eachSurvey).ToList();

            //Update total Surveys Feedbacks
            dashboardMetricsViewModel.surveyFeedbacks = surveyUsers.Count;

            return Ok(dashboardMetricsViewModel);
        }
    }
}