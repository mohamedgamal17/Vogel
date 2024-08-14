using Microsoft.AspNetCore.Authorization;

namespace Vogel.Social.Shared.Pictures.Commands
{
    [Authorize]
    public class CreatePictureCommand
    {
        public string Name { get; set; }
        public Stream Content { get; set; }
    }
}
