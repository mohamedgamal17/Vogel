using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.Comments.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.MongoDb.Entities.CommentReactions;

namespace Vogel.Application.CommentReactions.Factories
{
    public interface ICommentReactionResponseFactory : IResponseFactory
    {

        Task<List<CommentReactionDto>> PrepareListCommentReactionDto(List<CommentReactionMongoView> data);

        Task<CommentReactionDto> PrepareCommentReactionDto(CommentReactionMongoView comment);
    }
}
