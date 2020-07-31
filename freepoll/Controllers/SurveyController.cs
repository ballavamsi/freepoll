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
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public IActionResult AddNewSurvey([FromBody] SurveyViewModel newSurvey)
        {
            int PublishedStatusId = _dBContext.Status.Where(x => x.Statusname == "Published").Select(x => x.Statusid).FirstOrDefault();
            Survey s = new Survey();
            s.Welcometitle = newSurvey.Welcometitle;
            s.Welcomedescription = newSurvey.WelcomeDescription;
            s.Welcomeimage = newSurvey.Welcomeimage;
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


            for (int i = 0; i < qlist.Count; i++)
            {
                int qid = qlist[i].SurveyQuestionId;
                var options = newSurvey.SurveyQuestions[i].Options;
                List<SurveyQuestionOptions> qoplist = new List<SurveyQuestionOptions>();
                if (options != null)
                {
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
                }

                _dBContext.SaveChanges();
            }

            return Ok(GetSurvey(s.Surveyid));
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetSurvey(int id)
        {
            SurveyViewModel surview = new SurveyViewModel();

            Survey sur = _dBContext.Survey.Where(x => x.Surveyid == id).FirstOrDefault();

            if (sur == null)
                return BadRequest("SurveyNotFound");

            surview.SurveyId = sur.Surveyid;
            surview.Welcometitle = sur.Welcometitle;
            surview.WelcomeDescription = sur.Welcomedescription;
            surview.Emailidrequired = sur.Emailidrequired;
            surview.Endtitle = sur.Endtitle;
            surview.Welcomeimage = sur.Welcomeimage;
            surview.SurveyGuid = sur.SurveyGuid;

            List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == id).ToList();

            List<SurveyQuestionsViewModel> viewquestions = new List<SurveyQuestionsViewModel>();

            foreach (var item in questions)
            {
                SurveyQuestionsViewModel viewquestion = new SurveyQuestionsViewModel();
                viewquestion.SurveyQuestionId = item.SurveyQuestionId;
                viewquestion.QuestionDisplayOrder = item.QuestionDisplayOrder;
                viewquestion.TypeId = item.TypeId;
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
                viewquestion.ObjectOptions = options;
                viewquestions.Add(viewquestion);
            }
            surview.SurveyQuestions = viewquestions;

            return Ok(surview);
        }

        [Route("guid/{guid}")]
        [HttpGet]
        public IActionResult GetSurveyBasedonGuid(string guid)
        {
            SurveyViewModel surv = new SurveyViewModel();
            Survey sur = _dBContext.Survey.Where(x => x.SurveyGuid == guid).FirstOrDefault();
            if (sur == null)
                return BadRequest("SurveyNotFound");

            if (sur.SurveyGuid != null)
            {
                surv.Welcometitle = sur.Welcometitle;
                surv.WelcomeDescription = sur.Welcomedescription;
                surv.Emailidrequired = sur.Emailidrequired;
                surv.Endtitle = sur.Endtitle;
                surv.SurveyId = sur.Surveyid;
                surv.Welcomeimage = sur.Welcomeimage;
                surv.SurveyGuid = sur.SurveyGuid;

                List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == sur.Surveyid).ToList();

                List<SurveyQuestionsViewModel> viewquestions = new List<SurveyQuestionsViewModel>();

                foreach (var item in questions)
                {
                    SurveyQuestionsViewModel viewquestion = new SurveyQuestionsViewModel();
                    viewquestion.SurveyQuestionId = item.SurveyQuestionId;
                    viewquestion.QuestionDisplayOrder = item.QuestionDisplayOrder;
                    viewquestion.Isrequired = item.Isrequired;
                    viewquestion.TypeId = item.TypeId;
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
                    viewquestion.ObjectOptions = options;
                    viewquestions.Add(viewquestion);
                }
                surv.SurveyQuestions = viewquestions;
            }

            return Ok(surv);
        }



        [Route("begin/{guid}/")]
        [HttpPost]
        public IActionResult BeginSurvey(string guid)
        {
            return BeginSurvey(guid, "");
        }

        [Route("begin/{guid}/{emailId}")]
        [HttpPost]
        public IActionResult BeginSurvey(string guid, string emailId)
        {
            Survey sur = _dBContext.Survey.Where(x => x.SurveyGuid == guid).FirstOrDefault();
            emailId = Convert.ToString(emailId).ToLower();
            if (sur == null)
                return BadRequest("SurveyNotFound");

            if (sur.Enddate < DateTime.UtcNow)
                return BadRequest("SurveyEnded");

            bool checkSurveyEmail = _dBContext.SurveyUser.Any(x => x.SurveyId == sur.Surveyid && x.SurveyUserEmail.ToLower().Equals(emailId));
            if (checkSurveyEmail && Convert.ToBoolean(sur.Emailidrequired))
                return BadRequest("SurveyAlreadyTaken");

            SurveyUser surveyUser = new SurveyUser();
            surveyUser.SurveyId = sur.Surveyid;
            surveyUser.SurveyUserEmail = emailId;
            surveyUser.SurveyUserGuid = Guid.NewGuid().ToString();
            surveyUser.InsertedDatetime = DateTime.UtcNow;

            _dBContext.SurveyUser.Add(surveyUser);
            _dBContext.SaveChanges();
            return Ok(surveyUser);
        }


        [Route("submit/{guid}/{session}")]
        [HttpPost]
        public IActionResult SubmitSurvey(string guid, string session,QuestionAnswersRequestViewModel data)
        {
            List<QuestionAnswersViewModel> jObject = data.data;
            Survey sur = _dBContext.Survey.Where(x => x.SurveyGuid == guid).FirstOrDefault();
            if (sur == null)
                return BadRequest("SurveyNotFound");

            if (sur.Enddate < DateTime.UtcNow)
                return BadRequest("SurveyEnded");

            var surveyUsers = _dBContext.SurveyUser.Where(x => x.SurveyId == sur.Surveyid && x.SurveyUserGuid.Equals(session)).FirstOrDefault();
            if (surveyUsers == null)
                return BadRequest("SurveyWasNotStartedByUser");
            if (surveyUsers.CompletedDatetime != null)
                return BadRequest("SurveyWasAlreadySubmitted");

            List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == sur.Surveyid).ToList();
            List<QuestionType> lstQuestionTypes = _dBContext.QuestionType.ToList();
            List<SurveyUserQuestionOptions> lstSurveyUserQuestionOptions = new List<SurveyUserQuestionOptions>();

            for (int i = 0; i < questions.Count-1; i++)
            {
                SurveyUserQuestionOptions surveyUserQuestionOption = new SurveyUserQuestionOptions();
                surveyUserQuestionOption.SurveyUserId = surveyUsers.SurveyUserId;
                surveyUserQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                surveyUserQuestionOption.InsertedDatetime = DateTime.UtcNow;

                QuestionAnswersViewModel requestViewModel = jObject.Where(x=> x.key == questions[i].SurveyQuestionId.ToString()).FirstOrDefault();

                var questionTypeCode = lstQuestionTypes.Where(x => x.TypeId == questions[i].TypeId).FirstOrDefault().TypeCode;
                bool addToList = true;
                switch (questionTypeCode)
                {
                    case "essay":
                        surveyUserQuestionOption.CustomAnswer = requestViewModel.text;
                        break;
                    case "radiobuttons":
                        surveyUserQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "multiple":
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyUserQuestionOptions tempSurveyUserQuestionOption = new SurveyUserQuestionOptions();
                            tempSurveyUserQuestionOption.SurveyUserId = surveyUsers.SurveyUserId;
                            tempSurveyUserQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyUserQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyUserQuestionOption.SurveyQuestionOptionId = item;
                            lstSurveyUserQuestionOptions.Add(tempSurveyUserQuestionOption);
                        }
                        addToList = false;
                        break;
                    default:
                        surveyUserQuestionOption.CustomAnswer = requestViewModel.text; 
                        break;
                }

                if (addToList) lstSurveyUserQuestionOptions.Add(surveyUserQuestionOption);
            }

            _dBContext.SurveyUserQuestionOptions.AddRange(lstSurveyUserQuestionOptions);
            surveyUsers.CompletedDatetime = DateTime.UtcNow;
            _dBContext.Update(surveyUsers);
            _dBContext.SaveChanges();

            return Ok();
        }


        [Route("questiontypes")]
        [HttpGet]
        public IActionResult GetQuestionTypes()
        {
            List<QuestionType> lstQuestionTypes = _dBContext.QuestionType.Where(x => x.IsActive == 1).OrderBy(x => x.DisplayOrder).ToList();
            List<QuestionTypesViewModel> questionTypesViewModels = new List<QuestionTypesViewModel>();
            foreach (var item in lstQuestionTypes)
            {
                QuestionTypesViewModel questionTypesViewModel = new QuestionTypesViewModel();
                questionTypesViewModel.id = item.TypeId;
                questionTypesViewModel.code = item.TypeCode;
                questionTypesViewModel.name = item.TypeValue;
                questionTypesViewModels.Add(questionTypesViewModel);
            }

            return Ok(questionTypesViewModels);
        }
    }
}
