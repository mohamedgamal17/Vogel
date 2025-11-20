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
        private readonly ISecurityContext _securityContext;

        public GetPictureByIdQueryHandler(PictureMongoRepository pictureMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IPictureResponseFactory pictureResponseFactory, ISecurityContext securityContext)
        {
            _pictureMongoRepository = pictureMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _pictureResponseFactory = pictureResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<PictureDto>> Handle(GetPictureByIdQuery request, CancellationToken cancellationToken)
        {

            string userId = _securityContext.User!.Id;

            var picture = await _pictureMongoRepository.FindByIdAsync(request.Id);

            if (picture == null)
            {
                return new Result<PictureDto>(new EntityNotFoundException(typeof(Picture), request.Id));
            }

            if (!picture.IsOwnedBy(userId))
            {
                return new Result<PictureDto>(new ForbiddenAccessException());
            }

            return await _pictureResponseFactory.PreparePictureDto(picture);
        }
    }
}
