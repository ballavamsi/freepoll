using freepoll.UserModels;
using freepoll.ViewModels;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace freepoll.Helpers
{
    public partial class SurveyMetrics
    {

        /// <summary>
        /// Can be used for Radio button, checkboxes,
        /// Image radio buttons and checkboxes
        /// </summary>
        /// <param name="questionId"></param>
        public QuestionMetricViewModel forCountAndAverage(int questionId)
        {
            QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
            var question = _dBContext.SurveyQuestions.Where(x => x.StatusId == (int)EnumStatus.Published && x.SurveyQuestionId == questionId).FirstOrDefault();
            questionMetric.Question = question.Title;
            questionMetric.Explanation = question.Subtitle;

            var options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == questionId).ToList();
            var total = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId).Count();
            var count = 0;
            List<OptionsMetric> optionsMetrics = new List<OptionsMetric>();
            foreach (var item in options)
            {
                OptionsMetric optionsMetric = new OptionsMetric();
                optionsMetric.optionId = item.SurveyQuestionOptionId;
                optionsMetric.optionText = item.OptionValue;
                optionsMetric.optionCount = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId && x.SurveyQuestionOptionId.Equals(item.SurveyQuestionOptionId.ToString())).Count();
                optionsMetric.optionAverage = (Convert.ToDouble(optionsMetric.optionCount) / total) * 100;
                count += optionsMetric.optionCount;
                optionsMetrics.Add(optionsMetric);
            }


            if(total != count)
            {
                OptionsMetric tempOptionsMetric = new OptionsMetric();
                tempOptionsMetric.optionId = 0;
                tempOptionsMetric.optionText = "No Option Seletected";
                tempOptionsMetric.optionCount = total - count;
                tempOptionsMetric.optionAverage = (Convert.ToDouble(tempOptionsMetric.optionCount) / total) * 100;
                optionsMetrics.Add(tempOptionsMetric);
            }

            questionMetric.options = optionsMetrics;
            return questionMetric;
        }

        /// <summary>
        /// For Slider and range slider
        /// </summary>
        /// <param name="questionId"></param>
        public QuestionMetricViewModel forSliderAverage(int questionId)
        {
            QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
            var question = _dBContext.SurveyQuestions.Where(x => x.StatusId == (int)EnumStatus.Published && x.SurveyQuestionId == questionId).FirstOrDefault();
            questionMetric.Question = question.Title;
            questionMetric.Explanation = question.Subtitle;

            var options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == questionId).ToList();
            var total = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId).Count();

            var minValue = Convert.ToInt32(options.Where(x => x.OptionKey == "min").Select(x=>x.OptionValue).FirstOrDefault());
            var maxValue = Convert.ToInt32(options.Where(x => x.OptionKey == "max").Select(x => x.OptionValue).FirstOrDefault());

            var userSelected = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId && !string.IsNullOrEmpty(x.SurveyQuestionOptionId)).ToList();
            //double avg = userSelected.Average(x => x - minValue);

            List<OptionsMetric> optionsMetrics = new List<OptionsMetric>();

            foreach (var item in userSelected)
            {
                OptionsMetric optionsMetric = new OptionsMetric();
                optionsMetric.optionId = Convert.ToInt32(item.SurveyQuestionOptionId);
                optionsMetric.optionText = item.CustomAnswer;
                optionsMetric.optionCount = Convert.ToInt32(item.SurveyQuestionOptionId);
                optionsMetrics.Add(optionsMetric);
            }

            questionMetric.options = optionsMetrics;
            return questionMetric;
        }

        /// <summary>
        /// Star rating average
        /// </summary>
        /// <param name="questionId"></param>
        public QuestionMetricViewModel forStarRatingAverage(int questionId)
        {
            QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
            var question = _dBContext.SurveyQuestions.Where(x => x.StatusId == (int)EnumStatus.Published && x.SurveyQuestionId == questionId).FirstOrDefault();
            questionMetric.Question = question.Title;
            questionMetric.Explanation = question.Subtitle;

            var options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == questionId).ToList();
            var total = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId).Count();

            var userSelected = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId).Select(x => x.SurveyQuestionOptionId).ToList();
            //double avg = userSelected.Average(x => x - minValue);

            List<OptionsMetric> optionsMetrics = new List<OptionsMetric>();
            OptionsMetric tempOptionsMetric = new OptionsMetric();
            tempOptionsMetric.optionId = 0;
            tempOptionsMetric.optionText = "Average Rating";
            tempOptionsMetric.optionCount = userSelected.Count();
            tempOptionsMetric.optionAverage = userSelected.Where(x=>!string.IsNullOrEmpty(x)).Average(x=>Convert.ToInt32(x));
            optionsMetrics.Add(tempOptionsMetric);
            questionMetric.options = optionsMetrics;
            return questionMetric;
        }

        /// <summary>
        /// For multiple star rating average
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public QuestionMetricViewModel forMultipleStarRatings(int questionId)
        {
            QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
            var question = _dBContext.SurveyQuestions.Where(x => x.StatusId == (int)EnumStatus.Published && x.SurveyQuestionId == questionId).FirstOrDefault();
            questionMetric.Question = question.Title;
            questionMetric.Explanation = question.Subtitle;

            var options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == questionId).ToList();
            var total = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId).Count();
            var count = 0;
            List<OptionsMetric> optionsMetrics = new List<OptionsMetric>();
            foreach (var item in options)
            {
                var userSelected = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId && x.SurveyQuestionOptionId == item.SurveyQuestionOptionId.ToString()).Select(x => x.CustomAnswer).ToList();

                OptionsMetric optionsMetric = new OptionsMetric();
                optionsMetric.optionId = item.SurveyQuestionOptionId;
                optionsMetric.optionText = item.OptionValue;
                optionsMetric.optionCount = userSelected.Count();
                optionsMetric.optionAverage = userSelected.Where(x => !string.IsNullOrEmpty(x)).Average(x => Convert.ToInt32(x));
                count += optionsMetric.optionCount;
                optionsMetrics.Add(optionsMetric);
            }
            questionMetric.options = optionsMetrics;
            return questionMetric;
        }



        /// <summary>
        /// For multiple star rating average
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public QuestionMetricViewModel forCustomRatings(int questionId)
        {
            QuestionMetricViewModel questionMetric = new QuestionMetricViewModel();
            var question = _dBContext.SurveyQuestions.Where(x => x.StatusId == (int)EnumStatus.Published && x.SurveyQuestionId == questionId).FirstOrDefault();
            questionMetric.Question = question.Title;
            questionMetric.Explanation = question.Subtitle;

            var options = _dBContext.SurveyQuestionOptions.Where(x => x.SurveyQuestionId == questionId).OrderBy(x=>x.OptionKey).ToList();
            var feedbackOptions = _dBContext.SurveyFeedbackQuestionOptions.Where(x => x.SurveyQuestionId == questionId).ToList();
            var count = 0;
            List<OptionsMetric> optionsMetrics = new List<OptionsMetric>();
            foreach (var item in options.Where(x=>x.OptionKey.StartsWith("value")))
            {
                OptionsMetric optionsMetric = new OptionsMetric();
                optionsMetric.optionId = item.SurveyQuestionOptionId;
                optionsMetric.optionText = item.OptionValue;

                List<OptionsMetric> subOptionsMetrics = new List<OptionsMetric>();
                var currentOptionSelectedChoicesList = feedbackOptions.Where(x=>x.SurveyQuestionOptionId == item.SurveyQuestionOptionId.ToString());

                foreach (var subitem in options.Where(x => x.OptionKey.StartsWith("x_value")))
                {
                    var userSelected = currentOptionSelectedChoicesList.Where(x=>x.CustomAnswer == subitem.SurveyQuestionOptionId.ToString()).Select(x => x.CustomAnswer).ToList();

                    OptionsMetric subOptionsMetric = new OptionsMetric();
                    subOptionsMetric.optionId = subitem.SurveyQuestionOptionId;
                    subOptionsMetric.optionText = subitem.OptionValue;
                    subOptionsMetric.optionCount = userSelected.Where(x=>x == subitem.SurveyQuestionOptionId.ToString()).Count();
                    subOptionsMetric.optionAverage = (Convert.ToDouble(subOptionsMetric.optionCount) / currentOptionSelectedChoicesList.Count()) * 100;
                    count += subOptionsMetric.optionCount;
                    subOptionsMetrics.Add(subOptionsMetric);
                }
                optionsMetric.subOptions = subOptionsMetrics;
                optionsMetrics.Add(optionsMetric);
            }
            questionMetric.options = optionsMetrics;
            return questionMetric;
        }
    }
}
