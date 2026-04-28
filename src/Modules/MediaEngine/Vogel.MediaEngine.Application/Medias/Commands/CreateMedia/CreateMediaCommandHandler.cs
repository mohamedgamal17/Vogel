using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Application.Medias.Factories;
using Vogel.MediaEngine.Domain;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Commands.CreateMedia
{
    public class CreateMediaCommandHandler : IApplicationRequestHandler<CreateMediaCommand, MediaDto>
    {
        private readonly IMediaEngineRepository<Media> _mediaRepository;
        private readonly IMediaResponseFactory _mediaResponseFactory;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly ISecurityContext _securityContext;

        public CreateMediaCommandHandler(
            IMediaEngineRepository<Media> mediaRepository,
            IMediaResponseFactory mediaResponseFactory,
            IS3ObjectStorageService s3ObjectStorageService,
            ISecurityContext securityContext)
        {
            _mediaRepository = mediaRepository;
            _mediaResponseFactory = mediaResponseFactory;
            _s3ObjectStorageService = s3ObjectStorageService;
            _securityContext = securityContext;
        }

        public async Task<Result<MediaDto>> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
        {
            var objectSaveModel = new S3ObjectStorageSaveModel
            {
                FileName = request.Name,
                Content = request.Content,
                ContentType = request.MimeType,
            };

            var objectResponse = await _s3ObjectStorageService.SaveObjectAsync(objectSaveModel);

            var media = new Media
            {
                File = objectResponse.ObjectName,
                Size = objectResponse.Size,
                MimeType = request.MimeType,
                MediaType = request.MediaType,
                UserId = _securityContext.User!.Id,
            };

            media = await _mediaRepository.InsertAsync(media);
            return await _mediaResponseFactory.PrepareMediaDto(media);
        }
    }
}
