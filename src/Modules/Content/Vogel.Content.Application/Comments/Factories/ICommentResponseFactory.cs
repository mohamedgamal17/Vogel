using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.MongoEntities.Comments;
namespace Vogel.Content.Application.Comments.Factories
{
    public interface ICommentResponseFactory : IResponseFactory
    {
        Task<List<CommentDto>> PreapreListCommentDto(List<CommentMongoView> comments);
        Task<CommentDto> PrepareCommentDto(CommentMongoView comment);
    }
}
