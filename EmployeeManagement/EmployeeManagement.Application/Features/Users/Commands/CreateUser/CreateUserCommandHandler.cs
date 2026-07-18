using AutoMapper;
using FluentResults;
using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Application.Features.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Internship.EmployeeManagement.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(IMapper mapper, UserManager<UserEntity> userManager) : IRequestHandler<CreateUserCommand, IdResponse<int>>
    {
        private readonly IMapper _mapper = mapper;
        UserManager<UserEntity> _userManager = userManager;

        public async Task<IdResponse<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userEntity = _mapper.Map<UserEntity>(request);

            userEntity.UserName = userEntity.Email.Split('@')[0].ToUpperInvariant();

            var identityResult = await _userManager.CreateAsync(userEntity, request.Password);

            var checkResult = CheckIdentityResult(identityResult);

            if (!checkResult.IsSuccess)
                throw new BadRequestException(checkResult.Errors[0].Message);

            identityResult = await _userManager.AddToRoleAsync(userEntity, "User");

            checkResult = CheckIdentityResult(identityResult);

            if (!checkResult.IsSuccess)
                throw new BadRequestException(checkResult.Errors[0].Message);
                
            return new IdResponse<int> { Id = userEntity.Id };
        }

        private static Result CheckIdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
                return Result.Ok();

            StringBuilder errorMessage = new();

            foreach (var error in identityResult.Errors)
                errorMessage.Append($"-{error.Description}\n");

            return Result.Fail(errorMessage.ToString());
        }
    }
}
