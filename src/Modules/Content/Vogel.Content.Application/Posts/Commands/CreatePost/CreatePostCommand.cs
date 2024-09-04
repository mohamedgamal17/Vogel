using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Application.Posts.Dtos;

namespace Vogel.Content.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommand : ICommand<PostDto>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }
}
