using Vogel.Application.Comments.Dtos;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;
namespace Vogel.Application.Comments.Factories
{
    public interface ICommentResponseFactory : IResponseFactory
    {
        Task<List<CommentAggregateDto>> PreapreListCommentAggregateDto(List<CommentAggregateView> comments);
        Task<CommentAggregateDto> PrepareCommentAggregateDto(CommentAggregateView comment);
    }
}
