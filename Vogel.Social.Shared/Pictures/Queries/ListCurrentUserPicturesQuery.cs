using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Pictures.Dtos;

namespace Vogel.Social.Shared.Pictures.Queries
{
    [Authorize]
    public class ListCurrentUserPicturesQuery : IQuery<PictureDto>
    {
    }
}
