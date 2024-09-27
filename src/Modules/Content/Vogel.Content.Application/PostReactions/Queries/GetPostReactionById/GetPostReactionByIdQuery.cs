using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.PostReactions.Dtos;

namespace Vogel.Content.Application.PostReactions.Queries.GetPostReactionById
{
    public class GetPostReactionByIdQuery : IQuery<PostReactionDto>
    {
        public string PostId { get; set; }
        public string PostReactionId { get; set; }
    }
}
