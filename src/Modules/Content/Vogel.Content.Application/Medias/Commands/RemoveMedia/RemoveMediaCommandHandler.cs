using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Medias.Factories;
using Vogel.Content.Application.Medias.Policies;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;
namespace Vogel.Content.Application.Medias.Commands.RemoveMedia
{
    public class RemoveMediaCommandHandler : IApplicationRequestHandler<RemoveMediaCommand, Unit>
    {
        private readonly IContentRepository<Media> _mediaRepository;
        private readonly IMediaResponseFactory _mediaResponseFactory;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public RemoveMediaCommandHandler(IContentRepository<Media> mediaRepository, IMediaResponseFactory mediaResponseFactory, IS3ObjectStorageService s3ObjectStorageService, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _mediaRepository = mediaRepository;
            _mediaResponseFactory = mediaResponseFactory;
            _s3ObjectStorageService = s3ObjectStorageService;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemoveMediaCommand request, CancellationToken cancellationToken)
        {
            var media = await _mediaRepository.FindByIdAsync(request.Id);

            if (media == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Media), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }


            await _s3ObjectStorageService.RemoveObjectAsync(media.File);

            await _mediaRepository.DeleteAsync(media);

            return Unit.Value;
        }
    }
}
