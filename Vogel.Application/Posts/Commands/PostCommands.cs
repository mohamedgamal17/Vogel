using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Posts.Dtos;

namespace Vogel.Application.Posts.Commands
{
    public class PostCommandBase
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }

    [Authorize]
    public class CreatePostCommand : PostCommandBase , ICommand<PostAggregateDto> { }

    [Authorize]
    public class UpdatePostCommand  : PostCommandBase , ICommand<PostAggregateDto>
    {
        public string Id { get; set; }
    }

    [Authorize]
    public class RemovePostCommand : ICommand<Unit>
    {
        public string Id { get; set; }
    }
}
