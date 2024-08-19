using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Commands.CreatePicture
{
    [Authorize]
    public class CreatePictureCommand : ICommand<PictureDto>
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public Stream Content { get; set; }
    }
}
