using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
namespace Vogel.Content.Application.Comments.Commands.RemoveComent
{
    [Authorize]
    public class RemoveCommentCommand : ICommand
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}
