using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Factories
{
    public interface IPictureResponseFactory : IResponseFactory
    {
        Task<PictureDto> PreparePictureDto(Picture picture);
        Task<PictureDto> PreparePictureDto(PictureMongoEntity picture);
    }
}
