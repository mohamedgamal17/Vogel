using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Models;
using Vogel.Application.PostReactions.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;

namespace Vogel.Application.PostReactions.Queries
{
    [Authorize]
    public class ListPostReactionQuery : PagingParams ,IQuery<Paging<PostReactionDto>>
    {
        public string PostId { get; set; }
    }

    [Authorize]
    public class GetPostReactionQuery : IQuery<PostReactionDto>
    {
        public string PostId { get; set; }

        public string Id { get; set; }
    }
}
