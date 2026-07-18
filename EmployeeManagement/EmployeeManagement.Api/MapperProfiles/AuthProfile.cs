using AutoMapper;
using Internship.EmployeeManagement.Api.Models.Auth;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Login;

namespace Internship.EmployeeManagement.Api.MapperProfiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<LoginQueryParam, LoginQuery>();
        }
    }
}
