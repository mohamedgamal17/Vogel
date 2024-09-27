using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Dtos;
namespace Vogel.Content.Application.Posts.Queries.GetUserPostById
{
    public class GetUserPostByIdQuery : IQuery<PostDto>
    {
        public string UserId { get; set; }

        public string PostId { get; set; }
    }
}
