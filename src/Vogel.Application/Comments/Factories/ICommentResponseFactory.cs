﻿using Vogel.Application.Comments.Dtos;
using Vogel.Domain;
using Vogel.MongoDb.Entities.Comments;
using Vogel.BuildingBlocks.Application.Factories;
namespace Vogel.Application.Comments.Factories
{
    public interface ICommentResponseFactory : IResponseFactory
    {
        Task<List<CommentDto>> PreapreListCommentDto(List<CommentMongoView> comments);
        Task<CommentDto> PrepareCommentDto(CommentMongoView comment);
    }
}