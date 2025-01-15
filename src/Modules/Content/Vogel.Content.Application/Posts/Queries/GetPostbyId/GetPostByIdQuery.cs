using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Dtos;

namespace Vogel.Content.Application.Posts.Queries.GetPostbyId
{
    [Authorize]
    public class GetPostByIdQuery : IQuery<PostDto>
    {
        public string PostId { get; set; }
    }
}
