﻿using Vogel.Application.Common.Interfaces;
using Vogel.Application.Posts.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Posts.Factories
{
    public interface IPostResponseFactory : IResponseFactory
    {
        Task<List<PostAggregateDto>> PrepareListPostAggregateDto(List<PostAggregate> posts);
        Task<PostAggregateDto> PreparePostAggregateDto(Post post);
        Task<PostAggregateDto> PreparePostAggregateDto(PostAggregate post);
    }
}