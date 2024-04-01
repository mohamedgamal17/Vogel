﻿using Vogel.Application.Comments.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.Domain;
namespace Vogel.Application.Comments.Factories
{
    public class CommentResponseFactory : ICommentResponseFactory
    {
        private readonly IUserResponseFactory _userFactoryResponse;

        public CommentResponseFactory(IUserResponseFactory userFactoryResponse)
        {
            _userFactoryResponse = userFactoryResponse;
        }

        public async Task<List<CommentAggregateDto>> PreapreListCommentAggregateDto(List<CommentAggregateView> comments)
        {
            var tasks = comments.Select(PrepareCommentAggregateDto);

            var result = await Task.WhenAll(tasks);

            return result.ToList();
        }
        public async Task<CommentAggregateDto> PrepareCommentAggregateDto(CommentAggregateView comment)
        {
            var result = new CommentAggregateDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
            };

            if(comment.User != null)
            {

            }

            if(comment.User != null)
            {
                result.User = await _userFactoryResponse.PreparePublicUserDto(comment.User);
            }

            return result;
        }
    }
}
