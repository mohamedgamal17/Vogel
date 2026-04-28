using MediatR;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Application.Medias.Queries.GetMediaById;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.MediaEngine.Shared.Services;

namespace Vogel.MediaEngine.Application.Medias.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediator _mediator;

        public MediaService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<MediaDto>> GetMediaById(string id)
        {
            var result = await _mediator.Send(new GetMediaByIdQuery { MediaId = id });
            if (result.IsFailure)
            {
                return new Result<MediaDto>(result.Exception!);
            }

            var media = result.Value!;
            return new MediaDto
            {
                Id = media.Id,
                File = media.File,
                MediaType = (Shared.Enums.MediaType)media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                UserId = media.UserId,
            };
        }

        public async Task<Result<PublicMediaFileDto>> GetPublicMediaById(string id)
        {
            var result = await _mediator.Send(new GetMediaByIdQuery { MediaId = id });
            if (result.IsFailure)
            {
                return new Result<PublicMediaFileDto>(result.Exception!);
            }

            var media = result.Value!;
            return new PublicMediaFileDto
            {
                Id = media.Id,
                Reference = media.File,
                File = media.File,
                MediaType = (Shared.Enums.MediaType)media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                UserId = media.UserId,
            };
        }

        public async Task<Result<List<MediaDto>>> ListMediaByIds(List<string> ids)
        {
            var tasks = ids.Select(GetMediaById);
            var results = await Task.WhenAll(tasks);
            var firstFailure = results.FirstOrDefault(x => x.IsFailure);
            if (firstFailure != null)
            {
                return new Result<List<MediaDto>>(firstFailure.Exception!);
            }

            return results.Select(x => x.Value!).ToList();
        }

        public async Task<Result<List<PublicMediaFileDto>>> ListPublicMediaByIds(List<string> ids)
        {
            var tasks = ids.Select(GetPublicMediaById);
            var results = await Task.WhenAll(tasks);
            var firstFailure = results.FirstOrDefault(x => x.IsFailure);
            if (firstFailure != null)
            {
                return new Result<List<PublicMediaFileDto>>(firstFailure.Exception!);
            }

            return results.Select(x => x.Value!).ToList();
        }
    }
}
