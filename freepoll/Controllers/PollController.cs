using freepoll.Models;
using Microsoft.AspNetCore.Mvc;
using freepoll.ViewModels;
using System.Linq;
using System;
using freepoll.Helpers;
using System.Collections.Generic;
using AutoMapper;
using freepoll.UserModels;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Dynamic;
using freepoll.Common;
using static freepoll.Common.ResponseMessages;

namespace freepoll.Controllers
{
    [Route("api/poll")]
    public class PollController : ControllerBase
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public PollController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("add")]
        [HttpPut]
        public IActionResult AddNewPoll([FromBody]NewPollViewModel newPoll)
        {
            //     int PublishedStatusId = _dBContext.Status.Where(x => x.Statusname == "Published").Select(x => x.Statusid).FirstOrDefault();

            string userId = Request.Headers["userid"];
            string decyrptstring = Security.DecryptString(userId);
            if (decyrptstring == null) return BadRequest();

            int decrpteduser = Convert.ToInt32(decyrptstring);

            User user = _dBContext.User.Where(x => x.Userid == decrpteduser).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

            Poll p = new Poll();
            p.Name = newPoll.name;
            p.StatusId = newPoll.status;
            p.Enddate = Convert.ToDateTime(newPoll.endDate);
            p.CreatedDate = DateTime.UtcNow;
            p.CreatedBy = decrpteduser;
            p.Type = Int16.Parse(newPoll.type);
            p.Duplicate = Int16.Parse(newPoll.duplicate);
            p.PollGuid = ShortUrl.GenerateShortUrl();

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
                options.CreatedBy = Resources.SystemUser;
                options.CreatedDate = DateTime.UtcNow;
                options.OrderId = order;
                lstPollOptions.Add(options);
                order++;
            }

            _dBContext.PollOptions.AddRange(lstPollOptions);
            _dBContext.SaveChanges();
            return GetPoll(p.PollId);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetPoll(int id)
        {
            Poll poll = _dBContext.Poll.Where(x => x.PollId == id).FirstOrDefault();

            PollViewModel pollView = _mapper.Map<PollViewModel>(poll);


            List<PollOptions> options = _dBContext.PollOptions.Where(x => x.PollId == id).ToList();
            pollView.PollOptions = options;
            return Ok(pollView);
        }

        [Route("guid/{guid}")]
        [HttpGet]
        public IActionResult GetPollByGuid(string guid)
        {
            Poll poll = _dBContext.Poll.Where(x => x.PollGuid.Trim().Equals(guid.Trim())).FirstOrDefault();

            if(poll == null)  return NotFound(Messages.PollNotFoundError);
            if (poll.Enddate <= DateTime.Now.Date.AddDays(1).AddSeconds(-1)) return BadRequest(Messages.PollEnded);

            PollViewModel pollView = _mapper.Map<PollViewModel>(poll);

            List<PollOptions> options = _dBContext.PollOptions.Where(x => x.PollId == poll.PollId).ToList();
            pollView.PollOptions = options;
            return Ok(pollView);
        }

        [Route("vote")]
        [HttpPut]
        public IActionResult VotePoll([FromBody]PollVoteRequestViewModel newPoll)
        {
            var poll = _dBContext.Poll.Where(x => x.PollId == newPoll.pollId).FirstOrDefault();

            if (poll == null) return BadRequest(Messages.PollNotFoundError);
            if (poll.Enddate <= DateTime.Now.Date.AddDays(1).AddSeconds(-1)) return BadRequest(Messages.PollEnded);

            IPLocation userLocationDetails = LocationHelper.GetIpAndLocation(_httpContextAccessor);

            if(poll.Duplicate == 0)
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

            List<PollVoteOptionResponseViewModel> lstRegion = new List<PollVoteOptionResponseViewModel>();
            foreach (var item in votes.GroupBy(x => x.UserLocation).ToList())
            {
                lstRegion.Add(new PollVoteOptionResponseViewModel() { label = string.IsNullOrEmpty(item.Key) ? "Unknown" : item.Key, count = item.Count() });
            }

            PollVoteResponseViewModel dynamicData = new PollVoteResponseViewModel()
            {
                Question = poll.Name,
                Options = lstResponse,
                Regions = lstRegion
            };

            return Ok(dynamicData);
        }

        //Mapper.CreateMap<Employee, User>(); //Creates the map and all fields are copied if properties are same   

        ////If properties are different we need to map fields of employee to that of user as below.
        //AutoMapper.Mapper.CreateMap<Employee, User>()
        //.ForMember(o => o.Userid, b => b.MapFrom(z => z.EmployeeId))
    }
}