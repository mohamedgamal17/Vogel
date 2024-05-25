﻿using Vogel.Application.Common.Interfaces;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;
using Vogel.Domain.Users;

namespace Vogel.Application.Users.Factories
{
    public interface IUserResponseFactory : IResponseFactory
    {
        Task<List<UserAggregateDto>> PrepareListUserAggregateDto(List<UserAggregate> users);
        Task<UserAggregateDto> PrepareUserAggregateDto(UserAggregate user);
        Task<UserAggregateDto> PrepareUserAggregateDto(User user);
        Task<PublicUserDto> PreparePublicUserDto(PublicUserView user);
    }
}
