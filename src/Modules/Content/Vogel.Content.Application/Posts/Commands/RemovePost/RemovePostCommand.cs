using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.Domain.Medias;
namespace Vogel.Content.Application.Posts.Commands.RemovePost
{
    public class RemovePostCommand : ICommand
    {
        public string PostId { get; set; }
    }
}
