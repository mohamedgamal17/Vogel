using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Posts.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
namespace Vogel.Application.Posts.Commands
{
    public class PostCommandBase
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }

    [Authorize]
    public class CreatePostCommand : PostCommandBase , ICommand<PostDto> { }

    [Authorize]
    public class UpdatePostCommand  : PostCommandBase , ICommand<PostDto>
    {
        public string Id { get; set; }
    }

    [Authorize]
    public class RemovePostCommand : ICommand<Unit>
    {
        public string Id { get; set; }
    }
}
