using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;
namespace Vogel.Content.Application.Medias.Commands.RemoveMedia
{
    public class RemoveMediaCommandHandler : IApplicationRequestHandler<RemoveMediaCommand, Unit>
    {
        private readonly IContentRepository<Media> _mediaRepository;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly ISecurityContext _securityContext;

        public RemoveMediaCommandHandler(IContentRepository<Media> mediaRepository, IS3ObjectStorageService s3ObjectStorageService, ISecurityContext securityContext)
        {
            _mediaRepository = mediaRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
            _securityContext = securityContext;
        }

        public async Task<Result<Unit>> Handle(RemoveMediaCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var media = await _mediaRepository.FindByIdAsync(request.Id);

            if (media == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Media), request.Id));
            }

            if (!media.IsOwnedBy(userId))
            {
                return new Result<Unit>(new ForbiddenAccessException());
            }

            await _s3ObjectStorageService.RemoveObjectAsync(media.File);
            await _mediaRepository.DeleteAsync(media);

            return Unit.Value;
        }
    }
}
