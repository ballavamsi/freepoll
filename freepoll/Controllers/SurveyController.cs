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
        [HttpPost]
        public IActionResult AddNewSurvey([FromBody] SurveyViewModel newSurvey)
        {
            string userId = Request.Headers[Constants.UserToken];
            string decyrptstring = Security.Decrypt(userId);
            if (decyrptstring == null) return BadRequest();

            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

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

            bool checkSurveyEmail = _dBContext.SurveyUser.Any(x => x.SurveyId == sur.Surveyid && x.SurveyUserEmail.ToLower().Equals(emailId));
            if (checkSurveyEmail && Convert.ToBoolean(sur.Emailidrequired) && !string.IsNullOrEmpty(emailId))
                return BadRequest(Messages.SurveyAlreadyTaken);

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
                return BadRequest(Messages.SurveyNotFoundError);

            if (sur.Enddate < DateTime.UtcNow)
                return BadRequest(Messages.SurveyEnded);

            var surveyUsers = _dBContext.SurveyUser.Where(x => x.SurveyId == sur.Surveyid && x.SurveyUserGuid.Equals(session)).FirstOrDefault();
            if (surveyUsers == null)
                return BadRequest(Messages.SurveyWasNotStartedByUser);
            if (surveyUsers.CompletedDatetime != null)
                return BadRequest(Messages.SurveyWasAlreadySubmitted);

            List<SurveyQuestions> questions = _dBContext.SurveyQuestions.Where(x => x.SurveyId == sur.Surveyid && x.StatusId != (int)EnumStatus.Deleted).ToList();
            List<QuestionType> lstQuestionTypes = _dBContext.QuestionType.ToList();
            List<SurveyUserQuestionOptions> lstSurveyUserQuestionOptions = new List<SurveyUserQuestionOptions>();

            for (int i = 0; i < questions.Count; i++)
            {
                SurveyUserQuestionOptions surveyUserQuestionOption = new SurveyUserQuestionOptions();
                surveyUserQuestionOption.SurveyUserId = surveyUsers.SurveyUserId;
                surveyUserQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                surveyUserQuestionOption.InsertedDatetime = DateTime.UtcNow;

                QuestionAnswersViewModel requestViewModel = jObject.Where(x=> x.key == questions[i].SurveyQuestionId.ToString()).FirstOrDefault();
                List<SurveyQuestionOptions> surveyQuestionOptions = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == surveyUserQuestionOption.SurveyQuestionId).OrderBy(x=>x.OptionKey).ToList();
                var questionTypeCode = lstQuestionTypes.Where(x => x.TypeId == questions[i].TypeId).FirstOrDefault().TypeCode;
                bool addToList = true;
                int tempNumber = 0;

                switch (questionTypeCode)
                {
                    case "essay":
                        surveyUserQuestionOption.CustomAnswer = requestViewModel.text;
                        break;
                    case "radiobuttons":
                        surveyUserQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "imageradiobuttons":
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
                    case "imagemultiple":
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
                    case "slider":
                        surveyUserQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "rangeslider":
                        int j = 0;
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyUserQuestionOptions tempSurveyUserQuestionOption = new SurveyUserQuestionOptions();
                            tempSurveyUserQuestionOption.SurveyUserId = surveyUsers.SurveyUserId;
                            tempSurveyUserQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyUserQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyUserQuestionOption.SurveyQuestionOptionId = item;
                            tempSurveyUserQuestionOption.CustomAnswer = j == 0 ? "min" : "max";
                            lstSurveyUserQuestionOptions.Add(tempSurveyUserQuestionOption);
                            j++;
                        }
                        addToList = false;
                        break;
                    case "starrating":
                        surveyUserQuestionOption.SurveyQuestionOptionId = requestViewModel.number.ToString();
                        break;
                    case "multiplerating":
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyUserQuestionOptions tempSurveyUserQuestionOption = new SurveyUserQuestionOptions();
                            tempSurveyUserQuestionOption.SurveyUserId = surveyUsers.SurveyUserId;
                            tempSurveyUserQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyUserQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyUserQuestionOption.SurveyQuestionOptionId = surveyQuestionOptions[tempNumber].SurveyQuestionOptionId.ToString();
                            tempSurveyUserQuestionOption.CustomAnswer = item;
                            lstSurveyUserQuestionOptions.Add(tempSurveyUserQuestionOption);
                            tempNumber++;
                        }
                        addToList = false;
                        break;
                    case "customrating":

                        var surveyQuestionOptionsValues = surveyQuestionOptions.Where(x => x.OptionKey.StartsWith("value")).OrderBy(x => x.OptionKey).ToList();
                        foreach (var item in requestViewModel.selected)
                        {
                            SurveyUserQuestionOptions tempSurveyUserQuestionOption = new SurveyUserQuestionOptions();
                            tempSurveyUserQuestionOption.SurveyUserId = surveyUsers.SurveyUserId;
                            tempSurveyUserQuestionOption.SurveyQuestionId = questions[i].SurveyQuestionId;
                            tempSurveyUserQuestionOption.InsertedDatetime = DateTime.UtcNow;
                            tempSurveyUserQuestionOption.SurveyQuestionOptionId = surveyQuestionOptionsValues[tempNumber].SurveyQuestionOptionId.ToString();
                            tempSurveyUserQuestionOption.CustomAnswer = item;
                            lstSurveyUserQuestionOptions.Add(tempSurveyUserQuestionOption);
                            tempNumber++;
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
            string userguid = Request.Headers[Constants.UserToken];

            List<UserSurvey> filteredUserSurveysList = new List<UserSurvey>();
            UserSurveyResponse usersurveyres = new UserSurveyResponse();

            string decyrptstring = Security.Decrypt(userguid);

            if (string.IsNullOrEmpty(decyrptstring)) return BadRequest("Unauthorized User");

            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

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

            List<SurveyUser> surveyUsers = (from eachSurvey in _dBContext.SurveyUser
                                         where pollIdsFilteredList.Contains(eachSurvey.SurveyId)
                                         select eachSurvey).ToList();

            var surveyFeedbacksReceived = (from eachSurvey in surveyUsers
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

            string userguid = Request.Headers[Constants.UserToken];
            string decyrptstring = Security.Decrypt(userguid);
            if (string.IsNullOrEmpty(decyrptstring)) return BadRequest(Messages.UnauthorizedUserError);

            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

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
            string userguid = Request.Headers[Constants.UserToken];
            string decyrptstring = Security.Decrypt(userguid);
            if (string.IsNullOrEmpty(decyrptstring)) return BadRequest(Messages.UnauthorizedUserError);

            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.Surveyid == surveyId).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            SurveyUser surveyUser = _dBContext.SurveyUser.Where(x => x.SurveyId == surveyId && x.SurveyUserId == surveyFeedbackViewModel.FeedbackId).FirstOrDefault();

            if (surveyUser == null) return BadRequest(Messages.FeedbackNotFound);

            surveyUser.ReviewComment = surveyFeedbackViewModel.Comment;
            surveyUser.ReviewCompleted = Convert.ToBoolean(surveyFeedbackViewModel.reviewComplete) ? 1 : 0;
            surveyUser.ReviewDatetime = DateTime.UtcNow;
            _dBContext.SurveyUser.Update(surveyUser);
            _dBContext.SaveChanges();
            return Ok(surveyUser);
        }

        [Route("user/feedbacks/{surveyGuid}/pagenumber/{pagenum}/pagesize/{pagesize}")]
        [HttpGet]
        public IActionResult GetFeedbacks(string surveyGuid, int pagenum,int pagesize)
        {
            UserFeedbackResponse userFeedbackResponse = new UserFeedbackResponse();
            string userguid = Request.Headers[Constants.UserToken];
            string decyrptstring = Security.Decrypt(userguid);
            if (string.IsNullOrEmpty(decyrptstring)) return BadRequest(Messages.UnauthorizedUserError);

            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

            Survey survey = _dBContext.Survey.Where(x => x.CreatedBy == user.Userid && x.SurveyGuid == surveyGuid).FirstOrDefault();

            if (survey == null) return BadRequest(Messages.SurveyNotFoundError);

            userFeedbackResponse.surveyTitle = survey.Welcometitle;
            userFeedbackResponse.surveyLogo = survey.Welcomeimage;

            var userFeedbacks = from fb in _dBContext.SurveyUser
                                where fb.SurveyId == survey.Surveyid && fb.CompletedDatetime != null
                                orderby fb.InsertedDatetime descending
                                select new Feedbacks()
                                {
                                    surveyUserId = fb.SurveyUserId,
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

        [Route("user/graphmetrics/{surveyGuid}")]
        [HttpGet]
        public IActionResult UserSurveyReports(string surveyGuid)
        {
            SurveyMetrics metric = new SurveyMetrics(_dBContext, _mapper);
            string userguid = Request.Headers[Constants.UserToken];
            string decyrptstring = Security.Decrypt(userguid);
            if (string.IsNullOrEmpty(decyrptstring)) return BadRequest(Messages.UnauthorizedUserError);

            User user = _dBContext.User.Where(x => x.UserGuid == decyrptstring).FirstOrDefault();

            if (user == null) return BadRequest(Messages.UserNotFoundError);

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


    }
}
