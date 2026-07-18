using AutoMapper;
using Internship.EmployeeManagement.Api.Models.Employees;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeHistoryByEmployeeId;
using Internship.EmployeeManagement.Core.Entities;

namespace Internship.EmployeeManagement.Api.MapperProfiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeEntity, GetEmployeeByIdQueryResponse>().ReverseMap();

            CreateMap<CreateEmployeeCommandParam, CreateEmployeeCommand>();
            CreateMap<UpdateEmployeeCommandParam, UpdateEmployeeCommand>()
                .ForMember(d => d.EntityId, m => m.MapFrom(s => s.Id));

            CreateMap<UpdateEmployeeCommand, EmployeeEntity>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.EntityId));
            CreateMap<CreateEmployeeCommand, EmployeeEntity>();

            CreateMap<EmployeeHistoryEntity, GetEmployeeHistoryByIdQueryResponse>();

            CreateMap<EmployeeEntity, EmployeeCreatedEvent>().ReverseMap();
            CreateMap<EmployeeEntity, EmployeeUpdatedEvent>().ReverseMap();
        }
    }
}
