// AutoMapping.cs
using AutoMapper;
using freepoll.Models;
using freepoll.ViewModels;
using System;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        //Creating Poll AddNewPoll([FromBody]NewPollViewModel newPoll)
        CreateMap<NewPollViewModel, Poll>()
            .ForMember(dest => dest.StatusId, opts => opts.MapFrom(src => src.status))
            .ForMember(dest => dest.Type, opts => opts.MapFrom(src => Convert.ToInt16(src.type)))
            .ForMember(dest => dest.Duplicate, opts => opts.MapFrom(src => Convert.ToInt16(src.duplicate)))
            .ForMember(dest => dest.Enddate, opts => opts.MapFrom(src => Convert.ToDateTime(src.endDate)))
            .ForMember(dest => dest.PollGuid, opts => opts.MapFrom(src => src.pollGuid));

        CreateMap<NewPollViewModel, Survey>()
            .ForMember(dest => dest.StatusId, opts => opts.MapFrom(src => src.status));

        CreateMap<SurveyQuestionsViewModel, SurveyQuestions>();
        CreateMap<SurveyQuestions, SurveyQuestionsViewModel>();

        CreateMap<SurveyViewModel, Survey>();
        CreateMap<Survey, SurveyViewModel>();

        CreateMap<SurveyUserFeedbackViewModel, SurveyViewModel>();
        CreateMap<SurveyViewModel, SurveyUserFeedbackViewModel>();
    }
}