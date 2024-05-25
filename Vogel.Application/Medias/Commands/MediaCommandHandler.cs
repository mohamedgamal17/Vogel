using MediatR;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Medias.Factories;
using Vogel.Application.Medias.Policies;
using Vogel.Domain.Medias;
using Vogel.Domain.Utils;
namespace Vogel.Application.Medias.Commands
{
    public class MediaCommandHandler :
        IApplicationRequestHandler<CreateMediaCommand, MediaAggregateDto>,
        IApplicationRequestHandler<RemoveMediaCommand , Unit>
    {

        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly IMongoDbRepository<Media> _mediaDbRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IMediaResponseFactory _mediaResponeFactory;

        public MediaCommandHandler(IS3ObjectStorageService s3ObjectStorageService, IMongoDbRepository<Media> mediaDbRepository, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService, IMediaResponseFactory mediaResponeFactory)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
            _mediaDbRepository = mediaDbRepository;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
            _mediaResponeFactory = mediaResponeFactory;
        }

        public async Task<Result<MediaAggregateDto>> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
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
                UserId = _securityContext.User!.Id
            };

            media = await _mediaDbRepository.InsertAsync(media);

           
            return await _mediaResponeFactory.PrepareMedaiAggregateDto(media);
        }

        public async Task<Result<Unit>> Handle(RemoveMediaCommand request, CancellationToken cancellationToken)
        {
            var media = await _mediaDbRepository.FindByIdAsync(request.Id);

            if(media == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Media), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }

            await _s3ObjectStorageService.RemoveObjectAsync(media.File);

            await _mediaDbRepository.DeleteAsync(media);

            return Unit.Value;
        }
    }
}
