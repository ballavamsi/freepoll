using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using freepoll.ViewModels;
using freepoll.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using freepoll.Helpers;
using freepoll.Common;
using static freepoll.Common.ResponseMessages;
using freepoll.UserModels;
using Microsoft.Extensions.Caching.Memory;

namespace freepoll.Controllers
{
    [Route("api/survey")]
    [ApiController]
    public class SurveyController : Controller
    {
        private readonly FreePollDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        public SurveyController(FreePollDBContext dBContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _dBContext = dBContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        [Route("add")]
        [HttpPost]
        public IActionResult AddNewSurvey([FromBody] SurveyViewModel newSurvey)
        {
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            int PublishedStatusId = _dBContext.Status.Where(x => x.Statusname == "Published").Select(x => x.Statusid).FirstOrDefault();
            
            Survey s = _mapper.Map<Survey>(newSurvey);
            s.StatusId = PublishedStatusId;
            s.CreatedDate = DateTime.UtcNow;
            s.CreatedBy = user.Userid;
            s.Enddate = DateTime.UtcNow.AddYears(1);

            _dBContext.Survey.Add(s);
            _dBContext.SaveChanges();

            List<SurveyQuestions> qlist = new List<SurveyQuestions>();
            int qcount = 0;
            foreach (var item in newSurvey.SurveyQuestions)
            {
                SurveyQuestions question = _mapper.Map<SurveyQuestions>(item);
                question.SurveyId = s.Surveyid;
                question.CreatedBy = Convert.ToInt32(s.CreatedBy);
                question.CreatedDate = DateTime.UtcNow;
                question.StatusId = PublishedStatusId;
                question.QuestionDisplayOrder = qcount;
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
                        qop.CreatedBy = Convert.ToInt32(s.CreatedBy);
                        qop.CreatedDate = DateTime.UtcNow;
                        qoplist.Add(qop);
                    }
                    _dBContext.SurveyQuestionOptions.AddRange(qoplist);
                }

                _dBContext.SaveChanges();
            }

            return GetSurvey(s.Surveyid);
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetSurvey(int id)
        {
            SurveyViewModel surview = new SurveyViewModel();

            Survey sur = _dBContext.Survey.Where(x => x.Surveyid == id).FirstOrDefault();

            if (sur == null)
                return BadRequest(Messages.SurveyNotFoundError);

            surview = _mapper.Map<SurveyViewModel>(sur);

            List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == id && x.StatusId != (int)EnumStatus.Deleted).ToList();

            List<SurveyQuestionsViewModel> viewquestions = new List<SurveyQuestionsViewModel>();
           
            foreach (var item in questions)
            {
                SurveyQuestionsViewModel viewquestion = _mapper.Map<SurveyQuestionsViewModel>(item);
                List<SurveyQuestionOptions> options = new List<SurveyQuestionOptions>();
                options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId).OrderBy(x=> x.OptionKey).ToList();
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
                return BadRequest(Messages.SurveyNotFoundError);

            if (sur.SurveyGuid != null)
            {
                surv.Welcometitle = sur.Welcometitle;
                surv.WelcomeDescription = sur.Welcomedescription;
                surv.Emailidrequired = sur.Emailidrequired;
                surv.Askemail = sur.Askemail;
                surv.Enableprevious = sur.Enableprevious;
                surv.Endtitle = sur.Endtitle;
                surv.SurveyId = sur.Surveyid;
                surv.Welcomeimage = sur.Welcomeimage;
                surv.SurveyGuid = sur.SurveyGuid;

                List<SurveyQuestions> questions = _dBContext.SurveyQuestions.OrderBy(x=>x.QuestionDisplayOrder).Where(x => x.SurveyId == sur.Surveyid && x.StatusId != (int)EnumStatus.Deleted).ToList();

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
                    options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId).OrderBy(x => x.OptionKey).ToList();
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
                return BadRequest(Messages.SurveyNotFoundError);

            if (sur.Enddate < DateTime.UtcNow)
                return BadRequest(Messages.SurveyEnded);

            var surveyRestart = _dBContext.SurveyFeedback.Where(x => x.SurveyId == sur.Surveyid && x.SurveyUserEmail.ToLower().Equals(emailId) && x.CompletedDatetime == null).FirstOrDefault();
            if (surveyRestart != null)
                return Ok(surveyRestart);

            bool checkSurveyEmail = _dBContext.SurveyFeedback.Any(x => x.SurveyId == sur.Surveyid && x.SurveyUserEmail.ToLower().Equals(emailId) && x.CompletedDatetime != null);
            if (checkSurveyEmail && Convert.ToBoolean(sur.Emailidrequired) && !string.IsNullOrEmpty(emailId))
                return BadRequest(Messages.SurveyAlreadyTaken);

            SurveyFeedback surveyUser = new SurveyFeedback();
            surveyUser.SurveyId = sur.Surveyid;
            surveyUser.SurveyUserEmail = emailId;
            surveyUser.SurveyUserGuid = Guid.NewGuid().ToString();
            surveyUser.InsertedDatetime = DateTime.UtcNow;

            _dBContext.SurveyFeedback.Add(surveyUser);
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
                return BadRequest(Messages.SurveyNotFoundError);

            if (sur.Enddate < DateTime.UtcNow)
                return BadRequest(Messages.SurveyEnded);

            var surveyUsers = _dBContext.SurveyFeedback.Where(x => x.SurveyId == sur.Surveyid && x.SurveyUserGuid.Equals(session)).FirstOrDefault();
            if (surveyUsers == null)
                return BadRequest(Messages.SurveyWasNotStartedByUser);
            if (surveyUsers.CompletedDatetime != null)
                return BadRequest(Messages.SurveyWasAlreadySubmitted);

            List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == sur.Surveyid && x.StatusId != (int)EnumStatus.Deleted).ToList();
            List<QuestionType> lstQuestionTypes = _dBContext.QuestionType.ToList();
            List<SurveyFeedbackQuestionOptions> lstSurveyFeedbackQuestionOptions = new List<SurveyFeedbackQuestionOptions>();

            for (int i = 0; i < questions.Count; i++)
            {
                SurveyFeedbackQuestionOptions surveyFeedbackQuestionOption = new SurveyFeedbackQuestionOptions();
                surveyFeedbackQuestionOption.SurveyFeedbackId = surveyUsers.SurveyFeedbackId;
                surveyFeedbackQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                surveyFeedbackQuestionOption.InsertedDatetime = DateTime.UtcNow;

                QuestionAnswersViewModel requestViewModel = jObject.Where(x=> x.key == questions[i].SurveyQuestionId.ToString()).FirstOrDefault();
                List<SurveyQuestionOptions> surveyQuestionOptions = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == surveyFeedbackQuestionOption.SurveyQuestionId).OrderBy(x=>x.OptionKey).ToList();
                var questionTypeCode = lstQuestionTypes.Where(x => x.TypeId == questions[i].TypeId).FirstOrDefault().TypeCode;
                bool addToList = true;
                int tempNumber = 0;

                switch (questionTypeCode)
                {
                    case "essay":
                        surveyFeedbackQuestionOption.CustomAnswer = requestViewModel.text;
                        break;
                    case "radiobuttons":
                        surveyFeedbackQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "imageradiobuttons":
                        surveyFeedbackQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "multiple":
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyFeedbackQuestionOptions tempSurveyFeedbackQuestionOption = new SurveyFeedbackQuestionOptions();
                            tempSurveyFeedbackQuestionOption.SurveyFeedbackId = surveyUsers.SurveyFeedbackId;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyFeedbackQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionOptionId = item;
                            lstSurveyFeedbackQuestionOptions.Add(tempSurveyFeedbackQuestionOption);
                        }
                        addToList = false;
                        break;
                    case "imagemultiple":
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyFeedbackQuestionOptions tempSurveyFeedbackQuestionOption = new SurveyFeedbackQuestionOptions();
                            tempSurveyFeedbackQuestionOption.SurveyFeedbackId = surveyUsers.SurveyFeedbackId;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyFeedbackQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionOptionId = item;
                            lstSurveyFeedbackQuestionOptions.Add(tempSurveyFeedbackQuestionOption);
                        }
                        addToList = false;
                        break;
                    case "slider":
                        surveyFeedbackQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "rangeslider":
                        int j = 0;
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyFeedbackQuestionOptions tempSurveyFeedbackQuestionOption = new SurveyFeedbackQuestionOptions();
                            tempSurveyFeedbackQuestionOption.SurveyFeedbackId = surveyUsers.SurveyFeedbackId;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyFeedbackQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionOptionId = item;
                            tempSurveyFeedbackQuestionOption.CustomAnswer = j == 0 ? "min" : "max";
                            lstSurveyFeedbackQuestionOptions.Add(tempSurveyFeedbackQuestionOption);
                            j++;
                        }
                        addToList = false;
                        break;
                    case "starrating":
                        surveyFeedbackQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "multiplerating":
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyFeedbackQuestionOptions tempSurveyFeedbackQuestionOption = new SurveyFeedbackQuestionOptions();
                            tempSurveyFeedbackQuestionOption.SurveyFeedbackId = surveyUsers.SurveyFeedbackId;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyFeedbackQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionOptionId = surveyQuestionOptions[tempNumber].SurveyQuestionOptionId.ToString();
                            tempSurveyFeedbackQuestionOption.CustomAnswer = item;
                            lstSurveyFeedbackQuestionOptions.Add(tempSurveyFeedbackQuestionOption);
                            tempNumber++;
                        }
                        addToList = false;
                        break;
                    case "customrating":

                        var surveyQuestionOptionsValues = surveyQuestionOptions.Where(x => x.OptionKey.StartsWith("value")).OrderBy(x => x.OptionKey).ToList();
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyFeedbackQuestionOptions tempSurveyFeedbackQuestionOption = new SurveyFeedbackQuestionOptions();
                            tempSurveyFeedbackQuestionOption.SurveyFeedbackId = surveyUsers.SurveyFeedbackId;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyFeedbackQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyFeedbackQuestionOption.SurveyQuestionOptionId = surveyQuestionOptionsValues[tempNumber].SurveyQuestionOptionId.ToString();
                            tempSurveyFeedbackQuestionOption.CustomAnswer = item;
                            lstSurveyFeedbackQuestionOptions.Add(tempSurveyFeedbackQuestionOption);
                            tempNumber++;
                        }
                        addToList = false;
                        break;
                    default:
                        surveyFeedbackQuestionOption.CustomAnswer = requestViewModel.text; 
                        break;
                }

                if (addToList) lstSurveyFeedbackQuestionOptions.Add(surveyFeedbackQuestionOption);
            }

            _dBContext.SurveyFeedbackQuestionOptions.AddRange(lstSurveyFeedbackQuestionOptions);
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

        [Route("data/staroptions")]
        [HttpGet]
        public IActionResult GetDataForStarOptions()
        {
            return Ok(_dBContext.DataStarOptions.Where(x => x.IsActive == 1).OrderBy(x => x.DisplayOrder).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [Route("user/pagenumber/{pagenum}/pagesize/{pagesize}")]
        [HttpGet]
        public IActionResult UserSurvey(int pagenum, int pagesize)
        {
            string userId = Request.Headers[Constants.UserToken];

            List<UserSurvey> filteredUserSurveysList = new List<UserSurvey>();
            UserSurveyResponse usersurveyres = new UserSurveyResponse();

            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            List<Status> statuses = _dBContext.Status.ToList();

            var listSurveys = from survey in _dBContext.Survey
                              where survey.CreatedBy == user.Userid && survey.StatusId != 3
                              orderby survey.CreatedDate descending
                              select new UserSurvey()
                              {
                                  surveyId = survey.Surveyid,
                                  surveyGuid = survey.SurveyGuid,
                                  date = survey.CreatedDate,
                                  surveyName = survey.Welcometitle,
                                  status = survey.StatusId.ToString(),
                                  feedbacks = 0
                              };

            List<UserSurvey> totalUserSurveys = listSurveys.ToList();
            filteredUserSurveysList = totalUserSurveys.Skip(pagesize * pagenum)
                             .Take(pagesize).ToList();

            List<int> pollIdsFilteredList = filteredUserSurveysList.Select(x => x.surveyId).ToList();

            List<SurveyFeedback> surveyFeedback = (from eachSurvey in _dBContext.SurveyFeedback
                                                where pollIdsFilteredList.Contains(eachSurvey.SurveyId)
                                         select eachSurvey).ToList();

            var surveyFeedbacksReceived = (from eachSurvey in surveyFeedback
                                           group new { eachSurvey.SurveyId } by new { eachSurvey.CompletedDatetime , eachSurvey.SurveyId } into eachGroup
                                           select eachGroup).ToList();

            //Update only finaly display values
            for (int i = 0; i < filteredUserSurveysList.Count(); i++)
            {
                filteredUserSurveysList[i].status = statuses.Where(x => x.Statusid.ToString() == filteredUserSurveysList[i].status).SingleOrDefault().Statusname;
                filteredUserSurveysList[i].feedbacks = surveyFeedbacksReceived.Where(x => x.Key.SurveyId == filteredUserSurveysList[i].surveyId).Count();
            }

            usersurveyres.userSurveys = filteredUserSurveysList;
            usersurveyres.totalSurveys = listSurveys.ToList().Count;

            return Ok(usersurveyres);
        }


        [Route("delete/{surveyId}")]
        [HttpDelete]
        public IActionResult UserSurveyDelete(int surveyId)
        {
            UserSurveyResponse response = new UserSurveyResponse();

            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.Surveyid == surveyId).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            survey.StatusId = 3;

            int result = _dBContext.SaveChanges();

            if (result > 0)
            {
                response.Response = Messages.PollDeleteSuccess;
            }
            return Ok(response);
        }


        [Route("user/feedbacks/comment/{surveyId}")]
        [HttpPut]
        public IActionResult UpdateFeedbackComment(int surveyId, SurveyFeedbackViewModel surveyFeedbackViewModel)
        {
            UserFeedbackResponse userFeedbackResponse = new UserFeedbackResponse();
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.Surveyid == surveyId).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            SurveyFeedback surveyFeedback = _dBContext.SurveyFeedback.Where(x => x.SurveyId == surveyId && x.SurveyFeedbackId == surveyFeedbackViewModel.FeedbackId).FirstOrDefault();

            if (surveyFeedback == null) return BadRequest(Messages.FeedbackNotFound);

            surveyFeedback.ReviewComment = surveyFeedbackViewModel.Comment;
            surveyFeedback.ReviewCompleted = Convert.ToBoolean(surveyFeedbackViewModel.reviewComplete) ? 1 : 0;
            surveyFeedback.ReviewDatetime = DateTime.UtcNow;
            _dBContext.SurveyFeedback.Update(surveyFeedback);
            _dBContext.SaveChanges();
            return Ok(surveyFeedback);
        }

        [Route("user/feedbacks/{surveyGuid}/pagenumber/{pagenum}/pagesize/{pagesize}")]
        [HttpGet]
        public IActionResult GetFeedbacks(string surveyGuid, int pagenum,int pagesize)
        {
            UserFeedbackResponse userFeedbackResponse = new UserFeedbackResponse();
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.SurveyGuid == surveyGuid).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            userFeedbackResponse.surveyTitle = survey.Welcometitle;
            userFeedbackResponse.surveyLogo = survey.Welcomeimage;

            var userFeedbacks = from fb in _dBContext.SurveyFeedback
                                where fb.SurveyId == survey.Surveyid && fb.CompletedDatetime != null
                                orderby fb.InsertedDatetime descending
                                select new Feedbacks()
                                {
                                    surveyUserId = fb.SurveyFeedbackId,
                                    surveyUserGuid = fb.SurveyUserGuid,
                                    EmailId = fb.SurveyUserEmail,
                                    receivedDate = fb.CompletedDatetime,
                                    Comment = fb.ReviewComment,
                                    reviewComplete = Convert.ToBoolean(fb.ReviewCompleted),
                                    reviewUpdatedDate = fb.ReviewDatetime 
                                };

            var total = userFeedbacks.Count();

            var filteredFeedbacks = userFeedbacks.Skip(pagesize * pagenum)
                            .Take(pagesize).ToList();

            userFeedbackResponse.feedbacks = filteredFeedbacks;
            userFeedbackResponse.total = total;

            return Ok(userFeedbackResponse);
        }

        [Route("user/graphmetrics/{surveyGuid}/{surveyQuestionId}")]
        [HttpGet]
        public IActionResult UserSurveyReports(string surveyGuid,int surveyQuestionId)
        {
            SurveyMetrics metric = new SurveyMetrics(_dBContext, _mapper);
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.SurveyGuid == surveyGuid).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            SurveyQuestions surveyQuestion = _dBContext.SurveyQuestions.Where(x => x.StatusId != (int)EnumStatus.Deleted && x.SurveyId == survey.Surveyid && x.SurveyQuestionId == surveyQuestionId).ToList().FirstOrDefault();

            List<QuestionType> questionType = _dBContext.QuestionType.ToList();

            SurveyMetricViewModel surveyMetric = new SurveyMetricViewModel();
            List<QuestionMetricViewModel> questionMetrics = new List<QuestionMetricViewModel>();
            QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
            var type = questionType.Where(x => x.TypeId == surveyQuestion.TypeId).Select(x => x.TypeCode).FirstOrDefault();
            switch (type)
            {
                case "radiobuttons":
                    questionMetric = metric.forCountAndAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "multiple":
                    questionMetric = metric.forCountAndAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "imageradiobuttons":
                    questionMetric = metric.forCountAndAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "imagemultiple":
                    questionMetric = metric.forCountAndAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "slider":
                    questionMetric = metric.forSliderAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "rangeslider":
                    questionMetric = metric.forSliderAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "starrating":
                    questionMetric = metric.forStarRatingAverage(surveyQuestion.SurveyQuestionId);
                    break;
                case "multiplerating":
                    questionMetric = metric.forMultipleStarRatings(surveyQuestion.SurveyQuestionId);
                    break;
                case "customrating":
                    questionMetric = metric.forCustomRatings(surveyQuestion.SurveyQuestionId);
                    break;
                default:
                    break;
            }
            questionMetric.QuestionType = type;
            questionMetric.originalQuestionOptions = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == surveyQuestion.SurveyQuestionId).ToList();

            questionMetrics.Add(questionMetric);

            surveyMetric.Title = survey.Welcometitle;
            surveyMetric.Description = survey.Welcomedescription;
            surveyMetric.logo = survey.Welcomeimage;
            surveyMetric.Questions = questionMetrics;
            return Ok(surveyMetric);
        }

        [Route("user/graphmetrics/{surveyGuid}")]
        [HttpGet]
        public IActionResult UserSurveyReports(string surveyGuid)
        {
            SurveyMetrics metric = new SurveyMetrics(_dBContext, _mapper);
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.SurveyGuid == surveyGuid).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            List<SurveyQuestions> surveyQuestions = _dBContext.SurveyQuestions.Where(x => x.StatusId != (int)EnumStatus.Deleted && x.SurveyId == survey.Surveyid).ToList();

            List<QuestionType> questionType = _dBContext.QuestionType.ToList();

            SurveyMetricViewModel surveyMetric = new SurveyMetricViewModel();
            List<QuestionMetricViewModel> questionMetrics = new List<QuestionMetricViewModel>();
            foreach (var item in surveyQuestions)
            {
                QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
                var type = questionType.Where(x => x.TypeId == item.TypeId).Select(x => x.TypeCode).FirstOrDefault();
                switch (type)
                {
                    case "radiobuttons":
                        questionMetric = metric.forCountAndAverage(item.SurveyQuestionId);
                        break;
                    case "multiple":
                        questionMetric = metric.forCountAndAverage(item.SurveyQuestionId);
                        break;
                    case "imageradiobuttons":
                        questionMetric = metric.forCountAndAverage(item.SurveyQuestionId);
                        break;
                    case "imagemultiple":
                        questionMetric = metric.forCountAndAverage(item.SurveyQuestionId);
                        break;
                    case "slider":
                        questionMetric = metric.forSliderAverage(item.SurveyQuestionId);
                        break;
                    case "rangeslider":
                        questionMetric = metric.forSliderAverage(item.SurveyQuestionId);
                        break;
                    case "starrating":
                        questionMetric = metric.forStarRatingAverage(item.SurveyQuestionId);
                        break;
                    case "multiplerating":
                        questionMetric = metric.forMultipleStarRatings(item.SurveyQuestionId);
                        break;
                    case "customrating":
                        questionMetric = metric.forCustomRatings(item.SurveyQuestionId);
                        break;
                    default:
                        break;
                }
                questionMetric.QuestionType = type;
                questionMetric.originalQuestionOptions = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId).ToList();

                questionMetrics.Add(questionMetric);
            }

            surveyMetric.Title = survey.Welcometitle;
            surveyMetric.Description = survey.Welcomedescription;
            surveyMetric.logo = survey.Welcomeimage;
            surveyMetric.Questions = questionMetrics;
            return Ok(surveyMetric);
        }

        [Route("user/feedback/{surveyUserGuid}")]
        [HttpGet]
        public IActionResult GetSurveyUserFeedback(string surveyUserGuid)
        {
            SurveyUserFeedbackViewModel surveyUserFeedbackViewModel = new SurveyUserFeedbackViewModel();
            SurveyViewModel surview = new SurveyViewModel();
            string userId = Request.Headers[Constants.UserToken];
            User user;
            _memoryCache.TryGetValue(userId, out user);
            if (user == null) return Unauthorized(Messages.UserNotFoundError);

            SurveyFeedback surveyFeedback = _dBContext.SurveyFeedback.Where(x => x.SurveyUserGuid == surveyUserGuid).FirstOrDefault();

            if (surveyFeedback == null) return BadRequest(Messages.SurveyUserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.Surveyid == surveyFeedback.SurveyId).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            surview = _mapper.Map<SurveyViewModel>(survey);

            List<SurveyQuestions> surveyQuestions = _dBContext.SurveyQuestions.Where(x => x.StatusId != (int)EnumStatus.Deleted && x.SurveyId == survey.Surveyid).ToList();
            List<QuestionType> questiontypes = _dBContext.QuestionType.ToList();
            List<SurveyQuestionsViewModel> viewquestions = new List<SurveyQuestionsViewModel>();

            foreach (var item in surveyQuestions)
            {
                SurveyQuestionsViewModel viewquestion = _mapper.Map<SurveyQuestionsViewModel>(item);
                List<SurveyFeedbackQuestionOptions> surveyFeedbackquestionoptions = new List<SurveyFeedbackQuestionOptions>();
                List<string> selectedoptions = new List<string>();
                string optiontypevalue = null;
                List<SurveyQuestionOptions> options = new List<SurveyQuestionOptions>();
                options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId).OrderBy(x => x.OptionKey).ToList();
                surveyFeedbackquestionoptions = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == item.SurveyQuestionId && x.SurveyFeedbackId == surveyFeedback.SurveyFeedbackId).ToList();
                if (questiontypes != null)
                {
                    optiontypevalue = questiontypes.FirstOrDefault(x => x.TypeId == item.TypeId).TypeValue;
                }
                if (surveyFeedbackquestionoptions != null)
                {
                        foreach (var itemq in surveyFeedbackquestionoptions)
                        {
                            string option = null;
                            option = itemq.SurveyQuestionOptionId;
                            selectedoptions.Add(option);
                        }
                }                
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var opt in options)
                {                   
                    dict.Add(opt.OptionKey, opt.OptionValue);
                }
                viewquestion.Options = dict;
                viewquestion.ObjectOptions = options;
                viewquestion.questiontype = optiontypevalue;
                viewquestion.selectedValues = selectedoptions;
                viewquestions.Add(viewquestion);
            }
            surview.SurveyQuestions = viewquestions;

            surveyUserFeedbackViewModel = _mapper.Map < SurveyUserFeedbackViewModel>(surview);
            surveyUserFeedbackViewModel.SurveyFeedbacks = new List<SurveyFeedback>() { surveyFeedback };
            return Ok(surveyUserFeedbackViewModel);
        }

    }
}
