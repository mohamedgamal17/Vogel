using Microsoft.AspNetCore.Authorization;

namespace Vogel.Social.Shared.Pictures.Queries
{
    [Authorize]
    public class GetPictureByIdQuery
    {
        public string Id { get; set; }
    }
}
