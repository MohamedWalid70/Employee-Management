using AutoMapper;
using Internship.EmployeeManagement.Api.Models.Users;
using Internship.EmployeeManagement.Application.Features.Users.Commands.CreateUser;
using Internship.EmployeeManagement.Application.Features.Users.Queries.GetUserById;
using Internship.EmployeeManagement.Core.Entities;

namespace Internship.EmployeeManagement.Api.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {

            CreateMap<CreateUserCommand, UserEntity>();
            CreateMap<CreateUserCommandParam, CreateUserCommand>();
            CreateMap<UserEntity, GetUserByIdQueryResponse>();
        }
    }
}
