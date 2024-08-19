using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Queries.ListCurrentUserPictures
{
    [Authorize]
    public class ListCurrentUserPicturesQuery : PagingParams, IQuery<Paging<PictureDto>>
    {

    }
}
