using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using freepoll.ViewModels;
using freepoll.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using freepoll.Helpers;
using Microsoft.CodeAnalysis.Diagnostics;

namespace freepoll.Controllers
{
    [Route("api/survey")]
    [ApiController]
    public class SurveyController : Controller
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SurveyController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("add")]
        [HttpPut]
        public SurveyViewModel AddNewSurvey([FromBody] SurveyViewModel newSurvey)
        {
            int PublishedStatusId = _dBContext.Status.Where(x => x.Statusname == "Published").Select(x => x.Statusid).FirstOrDefault();
            Survey s = new Survey();
            s.Welcometitle = newSurvey.Welcometitle;
            s.Endtitle = newSurvey.Endtitle;
            s.StatusId = PublishedStatusId;
            s.CreatedBy = Resources.SystemUser;
            s.CreatedDate = DateTime.UtcNow;
            s.Allowduplicate = newSurvey.Allowduplicate;
            s.Emailidrequired = newSurvey.Emailidrequired;
            s.SurveyGuid = ShortUrl.GenerateShortUrl();

            _dBContext.Survey.Add(s);
            _dBContext.SaveChanges();

            List<SurveyQuestions> qlist = new List<SurveyQuestions>();
            int qcount = 0;
            foreach (var item in newSurvey.SurveyQuestions)
            {
                SurveyQuestions question = new SurveyQuestions();
                question.SurveyId = s.Surveyid;
                question.Title = item.Title;
                question.CreatedBy = Resources.SystemUser;
                question.CreatedDate = DateTime.UtcNow; 
                question.QuestionDisplayOrder = qcount;
                question.TypeId = item.TypeId;
                question.Isrequired = item.Isrequired;
                question.StatusId = item.StatusId;
                qlist.Add(question);
                qcount++;
            }

            _dBContext.SurveyQuestions.AddRange(qlist);
            _dBContext.SaveChanges();

           
            for (int i=0;i<qlist.Count;i++)
            {
                int qid = qlist[i].SurveyQuestionId;
                var options = newSurvey.SurveyQuestions[i].Options;
                List<SurveyQuestionOptions> qoplist = new List<SurveyQuestionOptions>();
                foreach (var item in options)
                {
                    SurveyQuestionOptions qop = new SurveyQuestionOptions();
                    qop.SurveyQuestionId = qid;
                    qop.OptionKey = item.Key;
                    qop.OptionValue = Convert.ToString(item.Value);
                    qop.CreatedBy = Resources.SystemUser;
                    qop.CreatedDate = DateTime.UtcNow;
                    qoplist.Add(qop);
                }
                _dBContext.SurveyQuestionOptions.AddRange(qoplist);
                _dBContext.SaveChanges();
            }

            return GetSurvey(s.Surveyid);
        }

        [Route("{id}")]
        [HttpGet]
        public SurveyViewModel GetSurvey(int id)
        {
            SurveyViewModel surview = new SurveyViewModel();

            Survey sur = _dBContext.Survey.Where(x => x.Surveyid == id).FirstOrDefault();

            surview.Welcometitle = sur.Welcometitle;
            surview.Endtitle = sur.Endtitle;
            surview.Welcomeimage = sur.Welcomeimage;
            surview.SurveyGuid = sur.SurveyGuid;

            List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == id).ToList();

            List<SurveyQuestionsViewModel> viewquestions = new List<SurveyQuestionsViewModel>();

            foreach(var item in questions)
            {
                SurveyQuestionsViewModel viewquestion = new SurveyQuestionsViewModel();
                viewquestion.Title = item.Title;
                viewquestion.Subtitle = item.Subtitle;
                List<SurveyQuestionOptions> options = new List<SurveyQuestionOptions>();
                options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId).ToList();
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var opt in options)
                {
                    dict.Add(opt.OptionKey, opt.OptionValue);
                }
                viewquestion.Options = dict;
                viewquestions.Add(viewquestion);
            }
            surview.SurveyQuestions = viewquestions;

            return surview;
        }

        [Route("guid/{guid}")]
        [HttpGet]
        public SurveyViewModel GetSurveyBasedonGuid(string guid)
        {
            SurveyViewModel surv = new SurveyViewModel();
            Survey sur = _dBContext.Survey.Where(x => x.SurveyGuid == guid).FirstOrDefault();

            if (sur.SurveyGuid != null)
            {
                surv.Welcometitle = sur.Welcometitle;
                surv.Endtitle = sur.Endtitle;
                surv.Welcomeimage = sur.Welcomeimage;
                surv.SurveyGuid = sur.SurveyGuid;

                List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == sur.Surveyid).ToList();

                List<SurveyQuestionsViewModel> viewquestions = new List<SurveyQuestionsViewModel>();

                foreach (var item in questions)
                {
                    SurveyQuestionsViewModel viewquestion = new SurveyQuestionsViewModel();
                    viewquestion.Title = item.Title;
                    viewquestion.Subtitle = item.Subtitle;
                    List<SurveyQuestionOptions> options = new List<SurveyQuestionOptions>();
                    options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId).ToList();
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (var opt in options)
                    {
                        dict.Add(opt.OptionKey, opt.OptionValue);
                    }
                    viewquestion.Options = dict;
                    viewquestions.Add(viewquestion);
                }
                surv.SurveyQuestions = viewquestions;
            }

            return surv;
        }
    }
}
