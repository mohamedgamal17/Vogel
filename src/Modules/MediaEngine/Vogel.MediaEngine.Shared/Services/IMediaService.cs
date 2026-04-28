using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Shared.Services
{
    public interface IMediaService
    {
        Task<Result<MediaDto>> GetMediaById(string id);
        Task<Result<PublicMediaFileDto>> GetPublicMediaById(string id);
        Task<Result<List<MediaDto>>> ListMediaByIds(List<string> ids);
        Task<Result<List<PublicMediaFileDto>>> ListPublicMediaByIds(List<string> ids);
    }
}
