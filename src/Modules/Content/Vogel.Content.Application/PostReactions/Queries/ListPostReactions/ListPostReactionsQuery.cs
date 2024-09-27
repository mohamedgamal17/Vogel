using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.PostReactions.Dtos;
namespace Vogel.Content.Application.PostReactions.Queries.ListPostReactions
{
    public class ListPostReactionsQuery : PagingParams , IQuery<Paging<PostReactionDto>>
    {
        public string PostId { get; set; }
    }
}
