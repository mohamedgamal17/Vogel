using Vogel.Application.Comments.Dtos;
using Vogel.Domain;
using Vogel.MongoDb.Entities.Comments;
using Vogel.BuildingBlocks.Application.Factories;
namespace Vogel.Application.Comments.Factories
{
    public interface ICommentResponseFactory : IResponseFactory
    {
        Task<List<CommentAggregateDto>> PreapreListCommentAggregateDto(List<CommentMongoView> comments);
        Task<CommentAggregateDto> PrepareCommentAggregateDto(CommentMongoView comment);
    }
}
