using freepoll.Models;
using Microsoft.AspNetCore.Mvc;
using freepoll.ViewModels;
using System.Linq;
using System;
using freepoll.Helpers;
using System.Collections.Generic;
using AutoMapper;

namespace freepoll.Controllers
{
    [Route("api/poll")]
    public class PollController
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        public PollController(FreePollDBContext dBContext, IMapper mapper)
        {
            _dBContext = dBContext;
            _mapper = mapper;
        }

        [Route("add")]
        [HttpPut]
        public PollViewModel AddNewPoll([FromBody]NewPollViewModel newPoll){

            int PublishedStatusId = _dBContext.Status.Where(x => x.Statusname == "Published").Select(x => x.Statusid).FirstOrDefault();
            Poll p = new Poll();
            p.Name = newPoll.name;
            p.StatusId = PublishedStatusId;
            p.Enddate = Convert.ToDateTime(newPoll.endDate);
            p.CreatedDate = DateTime.UtcNow;
            p.CreatedBy = Resources.SystemUser;
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
                options.StatusId = PublishedStatusId;
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
        public PollViewModel GetPoll(int id)
        {
            Poll poll = _dBContext.Poll.Where(x => x.PollId == id).FirstOrDefault();

            PollViewModel pollView = _mapper.Map<PollViewModel>(poll);


            List<PollOptions> options = _dBContext.PollOptions.Where(x => x.PollId == id).ToList();
            pollView.PollOptions = options;
            return pollView;
        }


        //Mapper.CreateMap<Employee, User>(); //Creates the map and all fields are copied if properties are same   

        ////If properties are different we need to map fields of employee to that of user as below.
        //AutoMapper.Mapper.CreateMap<Employee, User>()
        //.ForMember(o => o.Userid, b => b.MapFrom(z => z.EmployeeId))
    }
}