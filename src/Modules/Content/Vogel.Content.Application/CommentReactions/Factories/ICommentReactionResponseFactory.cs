using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.MongoEntities.CommentReactions;
namespace Vogel.Content.Application.CommentReactions.Factories
{
    public interface ICommentReactionResponseFactory : IResponseFactory
    {

        Task<List<CommentReactionDto>> PrepareListCommentReactionDto(List<CommentReactionMongoEntity> data);

        Task<CommentReactionDto> PrepareCommentReactionDto(CommentReactionMongoEntity comment);
    }
}
