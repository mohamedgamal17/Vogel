using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Queries.GetPictureById
{
    [Authorize]
    public class GetPictureByIdQuery : IQuery<PictureDto>
    {
        public string Id { get; set; }
    }
}
