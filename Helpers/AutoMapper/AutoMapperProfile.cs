using Alody.Models;
using AutoMapper;
using ViewModels;

namespace Alody.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //User Model
            CreateMap<UserModel, UserViewModel>();
            CreateMap<UserViewModel, UserModel>();
        }
    }
}
