using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Pictures.Factories;
using Vogel.Social.Application.Pictures.Policies;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Queries.GetPictureById
{
    public class GetPictureByIdQueryHandler : IApplicationRequestHandler<GetPictureByIdQuery, PictureDto>
    {
        private readonly PictureMongoRepository _pictureMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IPictureResponseFactory _pictureResponseFactory;

        public GetPictureByIdQueryHandler(PictureMongoRepository pictureMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IPictureResponseFactory pictureResponseFactory)
        {
            _pictureMongoRepository = pictureMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _pictureResponseFactory = pictureResponseFactory;
        }

        public async Task<Result<PictureDto>> Handle(GetPictureByIdQuery request, CancellationToken cancellationToken)
        {
            var picture = await _pictureMongoRepository.FindByIdAsync(request.Id);

            if (picture == null)
            {
                return new Result<PictureDto>(new EntityNotFoundException(typeof(Picture), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(picture, PictureOperationRequirements.IsPictureOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<PictureDto>(authorizationResult.Exception!);
            }

            return await _pictureResponseFactory.PreparePictureDto(picture);
        }
    }
}
