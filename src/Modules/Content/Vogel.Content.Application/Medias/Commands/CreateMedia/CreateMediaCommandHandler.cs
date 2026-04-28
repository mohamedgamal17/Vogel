using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Application.Medias.Factories;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;
namespace Vogel.Content.Application.Medias.Commands.CreateMedia
{
    public class CreateMediaCommandHandler : IApplicationRequestHandler<CreateMediaCommand, MediaDto>
    {
        private readonly IContentRepository<Media> _mediaRepository;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly ISecurityContext _securityContext;
        private readonly IMediaResponseFactory _mediaResponseFactory;
        public CreateMediaCommandHandler(IContentRepository<Media> mediaRepository, IS3ObjectStorageService s3ObjectStorageService, ISecurityContext securityContext, IMediaResponseFactory mediaResponseFactory)
        {
            _mediaRepository = mediaRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
            _securityContext = securityContext;
            _mediaResponseFactory = mediaResponseFactory;
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
                UserId = _securityContext.User!.Id,
                MimeType = request.MimeType,
                Size = request.Content.Length,
                MediaType = request.MediaType
            };

            await _mediaRepository.InsertAsync(media);

            return await _mediaResponseFactory.PrepareMediaDto(media);
        }
    }
}
