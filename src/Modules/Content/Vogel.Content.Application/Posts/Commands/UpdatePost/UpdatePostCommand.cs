using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Dtos;

namespace Vogel.Content.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommand : ICommand<PostDto>
    {
        public string PostId { get; set; }
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }
}
