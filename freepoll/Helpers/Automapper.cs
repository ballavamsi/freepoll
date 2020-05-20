using AutoMapper;
using freepoll.Models;
using freepoll.ViewModels;

namespace freepoll.Helpers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Poll, PollViewModel>();
        }
    }
}
