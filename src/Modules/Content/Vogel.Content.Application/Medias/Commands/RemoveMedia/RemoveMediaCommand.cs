using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
namespace Vogel.Content.Application.Medias.Commands.RemoveMedia
{
    [Authorize]
    public class RemoveMediaCommand : ICommand
    {
        public string Id { get; set; }
    }
}
