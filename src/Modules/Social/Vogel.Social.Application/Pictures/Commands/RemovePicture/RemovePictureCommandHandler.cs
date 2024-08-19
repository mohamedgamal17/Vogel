using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Pictures.Policies;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
namespace Vogel.Social.Application.Pictures.Commands.RemovePicture
{
    public class RemovePictureCommandHandler :
        IApplicationRequestHandler<RemovePictureCommand, Unit>
    {
        private readonly ISocialRepository<Picture> _pictureRepository;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public RemovePictureCommandHandler(ISocialRepository<Picture> pictureRepository, IS3ObjectStorageService s3ObjectStorageService, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _pictureRepository = pictureRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemovePictureCommand request, CancellationToken cancellationToken)
        {

            var picture = await _pictureRepository.FindByIdAsync(request.Id);

            if (picture == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Picture), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(picture, PictureOperationRequirements.IsPictureOwner);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }


            await _s3ObjectStorageService.RemoveObjectAsync(picture.File);

            await _pictureRepository.DeleteAsync(picture);

            return Unit.Value;

        }
    }
}
