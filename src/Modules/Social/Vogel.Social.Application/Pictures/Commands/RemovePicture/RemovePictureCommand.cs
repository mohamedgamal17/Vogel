using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
namespace Vogel.Social.Application.Pictures.Commands.RemovePicture
{
    [Authorize]
    public class RemovePictureCommand : ICommand
    {
        public string Id { get; set; }
    }
}
