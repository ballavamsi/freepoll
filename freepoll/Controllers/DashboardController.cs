using AutoMapper;
using freepoll.Common;
using freepoll.Helpers;
using freepoll.Models;
using freepoll.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMemoryCache _memoryCache;

        public DashboardController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        [Route("tilemetrics")]
        [HttpGet]
        public IActionResult GetTileMetrics()
        {
            DashboardMetricsViewModel dashboardMetricsViewModel = new DashboardMetricsViewModel();

            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            if (_memoryCache.TryGetValue($"dashboard_{user.UserGuid}", out dashboardMetricsViewModel))
                return Ok(dashboardMetricsViewModel);

            dashboardMetricsViewModel = new DashboardMetricsViewModel();

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
            _memoryCache.Set($"dashboard_{user.UserGuid}", dashboardMetricsViewModel, Resources.CachedTime);
            return Ok(dashboardMetricsViewModel);
        }
    }
}