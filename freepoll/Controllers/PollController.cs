using AutoMapper;
using freepoll.Common;
using freepoll.Helpers;
using freepoll.Models;
using freepoll.UserModels;
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
    [Route("api/poll")]
    public class PollController : ControllerBase
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public PollController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        [Route("add")]
        [HttpPost]
        public IActionResult AddNewPoll([FromBody] NewPollViewModel newPoll)
        {
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Poll p = _mapper.Map<Poll>(newPoll);
            p.CreatedDate = DateTime.UtcNow;
            p.CreatedBy = user.Userid;

            _dBContext.Poll.Add(p);
            _dBContext.SaveChanges();

            List<PollOptions> lstPollOptions = new List<PollOptions>();

            int order = 0;
            foreach (var item in newPoll.options)
            {
                PollOptions options = new PollOptions();
                options.PollId = p.PollId;
                options.OptionText = item;
                options.StatusId = p.StatusId;
                options.CreatedBy = user.Userid;
                options.CreatedDate = DateTime.UtcNow;
                options.OrderId = order;
                lstPollOptions.Add(options);
                order++;
            }

            _dBContext.PollOptions.AddRange(lstPollOptions);
            _dBContext.SaveChanges();

            PollViewModel pCache = _mapper.Map<PollViewModel>(p);
            pCache.PollOptions = lstPollOptions;

            _memoryCache.Set($"dbpoll_id_{p.PollId}", p);
            _memoryCache.Set($"poll_guid_{p.PollGuid}", pCache);
            _memoryCache.Set($"poll_id_{p.PollId}", pCache);
            _memoryCache.Remove($"poll_userpoll_userid_{user.UserGuid}");

            DashboardMetricsViewModel dashboardMetricsViewModel;
            _memoryCache.TryGetValue($"dashboard_{user.UserGuid}", out dashboardMetricsViewModel);
            if (dashboardMetricsViewModel != null)
            {
                dashboardMetricsViewModel.polls += 1;
                _memoryCache.Set($"dashboard_{user.UserGuid}", dashboardMetricsViewModel);
            }

            return GetPoll(p.PollId);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetPoll(int id)
        {
            PollViewModel pollView = new PollViewModel();
            if (_memoryCache.TryGetValue($"poll_id_{id}", out pollView))
                return Ok(pollView);

            Poll poll = _dBContext.Poll.Where(x => x.PollId == id).FirstOrDefault();
            pollView = _mapper.Map<PollViewModel>(poll);
            List<PollOptions> options = _dBContext.PollOptions.Where(x => x.PollId == id).ToList();
            pollView.PollOptions = options;

            _memoryCache.Set($"poll_id_{poll.PollId}", pollView);
            return Ok(pollView);
        }

        [Route("guid/{guid}")]
        [HttpGet]
        public IActionResult GetPollByGuid(string guid)
        {
            PollViewModel pollView = new PollViewModel();
            if (_memoryCache.TryGetValue("poll_guid_" + guid.ToString(), out pollView))
                return Ok(pollView);

            Poll poll = _dBContext.Poll.Where(x => x.PollGuid.Trim().Equals(guid.Trim())).FirstOrDefault();

            if (poll == null) return NotFound(Messages.PollNotFoundError);
            if (poll.Enddate <= DateTime.Now.Date.AddDays(1).AddSeconds(-1)) return BadRequest(Messages.PollEnded);

            pollView = _mapper.Map<PollViewModel>(poll);

            List<PollOptions> options = _dBContext.PollOptions.Where(x => x.PollId == poll.PollId).ToList();
            pollView.PollOptions = options;
            _memoryCache.Set($"poll_guid_{guid}", pollView);
            return Ok(pollView);
        }

        [Route("vote")]
        [HttpPost]
        public IActionResult VotePoll([FromBody] PollVoteRequestViewModel newPoll)
        {
            Poll poll; ;
            if (_memoryCache.TryGetValue("dbpoll_id_" + newPoll.pollId, out poll))
                return Ok(poll);
            else
                poll = _dBContext.Poll.Where(x => x.PollId == newPoll.pollId).FirstOrDefault();

            if (poll == null) return BadRequest(Messages.PollNotFoundError);
            if (poll.Enddate <= DateTime.Now.Date.AddDays(1).AddSeconds(-1)) return BadRequest(Messages.PollEnded);

            IPLocation userLocationDetails = LocationHelper.GetIpAndLocation(_httpContextAccessor);

            if (poll.Duplicate == 0)
            {
                var voted = _dBContext.PollVotes.Where(x => x.IpAddress == userLocationDetails.IP && x.UserLocation == userLocationDetails.Region).Any();
                if (voted) return BadRequest(Messages.PollVoted);
            }

            List<PollVotes> lstPollVotes = new List<PollVotes>();
            foreach (var item in newPoll.options)
            {
                PollVotes pollVote = new PollVotes();
                pollVote.PollId = poll.PollId;
                pollVote.OptionId = Int32.Parse(item);
                pollVote.IpAddress = userLocationDetails.IP;
                pollVote.UserLocation = userLocationDetails.Region;
                pollVote.CreatedDate = DateTime.UtcNow;
                lstPollVotes.Add(pollVote);
            }
            _dBContext.PollVotes.AddRange(lstPollVotes);
            _dBContext.SaveChanges();
            return Ok(true);
        }

        [Route("result/{pollGuid}")]
        [HttpGet]
        public IActionResult GetResults(string pollGuid)
        {
            PollVoteResponseViewModel dynamicData = new PollVoteResponseViewModel();
            if (_memoryCache.TryGetValue("poll_guid_results" + pollGuid.ToString(), out dynamicData))
                return Ok(dynamicData);

            var poll = _dBContext.Poll.Where(x => x.PollGuid == pollGuid).FirstOrDefault();
            if (poll == null) return BadRequest(Messages.PollNotFoundError);

            List<PollVotes> votes = _dBContext.PollVotes.Where(x => x.PollId == poll.PollId).ToList();
            List<PollOptions> options = _dBContext.PollOptions.Where(x => x.PollId == poll.PollId).ToList();

            List<PollVoteOptionResponseViewModel> lstResponse = new List<PollVoteOptionResponseViewModel>();
            foreach (var item in options)
            {
                var _count = votes.Where(x => x.OptionId == item.PollOptionId).Count();
                lstResponse.Add(new PollVoteOptionResponseViewModel() { label = item.OptionText, count = _count });
            }

            lstResponse = lstResponse.OrderBy(x => x.count).ToList();

            List<PollVoteOptionResponseViewModel> lstRegion = new List<PollVoteOptionResponseViewModel>();
            foreach (var item in votes.GroupBy(x => x.UserLocation).ToList())
            {
                lstRegion.Add(new PollVoteOptionResponseViewModel() { label = string.IsNullOrEmpty(item.Key) ? "Unknown" : item.Key, count = item.Count() });
            }

            dynamicData = new PollVoteResponseViewModel()
            {
                Question = poll.Name,
                Options = lstResponse,
                Regions = lstRegion
            };

            _memoryCache.Set("poll_guid_results" + pollGuid.ToString(), dynamicData);
            return Ok(dynamicData);
        }

        [Route("user/pagenumber/{pagenum}/pagesize/{pagesize}")]
        [HttpGet]
        public IActionResult UserPoll(int pagenum, int pagesize)
        {
            string userId = Request.Headers[Constants.UserToken];

            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            List<UserPoll> filteredUserPollsList = new List<UserPoll>();
            UserPollResponse userpollres = new UserPollResponse();

            if (_memoryCache.TryGetValue($"poll_userpoll_userid_{user.UserGuid}", out userpollres))
                return Ok(userpollres);

            userpollres = new UserPollResponse();

            List<Status> statuses = _dBContext.Status.ToList();

            var listpoll = from poll in _dBContext.Poll
                           where poll.CreatedBy == user.Userid && poll.StatusId != 3
                           orderby poll.CreatedDate descending
                           select new UserPoll()
                           {
                               pollId = poll.PollId,
                               pollGuid = poll.PollGuid,
                               date = Convert.ToDateTime(poll.CreatedDate),
                               pollName = poll.Name,
                               status = poll.StatusId.ToString(),
                               votes = 0
                           };

            List<UserPoll> totalUserPolls = listpoll.ToList();

            filteredUserPollsList = totalUserPolls.Skip(pagesize * pagenum)
                             .Take(pagesize).ToList();

            List<int> pollIdsFilteredList = filteredUserPollsList.Select(x => x.pollId).ToList();

            List<PollVotes> pollVotes = (from eachPoll in _dBContext.PollVotes
                                         where pollIdsFilteredList.Contains(Convert.ToInt32(eachPoll.PollId))
                                         select eachPoll).ToList();

            var pollVotesReceived = (from eachPoll in pollVotes
                                     group new { eachPoll.PollId } by new { eachPoll.CreatedDate, eachPoll.PollId } into eachGroup
                                     select eachGroup).ToList();

            //Update only finaly display values
            for (int i = 0; i < filteredUserPollsList.Count(); i++)
            {
                filteredUserPollsList[i].status = statuses.Where(x => x.Statusid.ToString() == filteredUserPollsList[i].status).SingleOrDefault().Statusname;
                filteredUserPollsList[i].votes = pollVotesReceived.Where(x => x.Key.PollId == filteredUserPollsList[i].pollId).Count();
            }

            userpollres.userPolls = filteredUserPollsList;
            userpollres.totalPolls = listpoll.ToList().Count;

            _memoryCache.Set($"poll_userpoll_userid_{user.UserGuid}", userpollres);
            return Ok(userpollres);
        }

        [Route("delete/{pollId}")]
        [HttpDelete]
        public IActionResult UserPollDelete(int pollId)
        {
            UserPollResponse response = new UserPollResponse();

            string userId = Request.Headers[Constants.UserToken];

            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Poll poll = _dBContext.Poll.Where(x => x.CreatedBy == user.Userid && x.PollId == pollId).FirstOrDefault();
            if (poll == null) return BadRequest(Messages.PollNotFoundError);
            poll.StatusId = 3;

            int result = _dBContext.SaveChanges();

            if (result > 0)
            {
                response.Response = Messages.PollDeleteSuccess;
            }

            _memoryCache.Remove("poll_guid_" + poll.PollGuid);
            _memoryCache.Remove("poll_id_" + poll.PollId);

            UserPollResponse userpollres = new UserPollResponse();
            _memoryCache.TryGetValue($"poll_userpoll_userid_{user.UserGuid}", out userpollres);
            if (userpollres != null)
            {
                var pollToDelete = userpollres.userPolls.Where(x => x.pollId == poll.PollId).FirstOrDefault();
                userpollres.userPolls.Remove(pollToDelete);
                userpollres.totalPolls = (userpollres.totalPolls - 1) >= 0 ? (userpollres.totalPolls - 1) : 0;
            }

            DashboardMetricsViewModel dashboardMetricsViewModel;
            _memoryCache.TryGetValue($"dashboard_{user.UserGuid}", out dashboardMetricsViewModel);
            if (dashboardMetricsViewModel != null)
            {
                dashboardMetricsViewModel.polls = (dashboardMetricsViewModel.polls - 1) > 0 ? (dashboardMetricsViewModel.polls - 1) : 0;
                _memoryCache.Set($"dashboard_{user.UserGuid}", dashboardMetricsViewModel);
            }
            return Ok(response);
        }

        //Mapper.CreateMap<Employee, User>(); //Creates the map and all fields are copied if properties are same

        ////If properties are different we need to map fields of employee to that of user as below.
        //AutoMapper.Mapper.CreateMap<Employee, User>()
        //.ForMember(o => o.Userid, b => b.MapFrom(z => z.EmployeeId))
    }
}