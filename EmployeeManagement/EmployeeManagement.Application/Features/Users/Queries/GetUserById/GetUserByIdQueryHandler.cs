using AutoMapper;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler(UserManager<UserEntity> userManager, IMapper mapper) : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse?>
    {
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<GetUserByIdQueryResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userEntity = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

            var userByIdQueryParam = _mapper.Map<GetUserByIdQueryResponse?>(userEntity);

            return userByIdQueryParam;
        }
    }
}
