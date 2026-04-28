using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Application.Medias.Factories;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.MongoEntities.Medias;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Queries.GetMediaById
{
    public class GetMediaByIdQueryHandler : IApplicationRequestHandler<GetMediaByIdQuery, MediaDto>
    {
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;
        private readonly IMediaResponseFactory _mediaResponseFactory;
        private readonly ISecurityContext _securityContext;

        public GetMediaByIdQueryHandler(
            IMongoRepository<MediaMongoEntity> mediaMongoRepository,
            IMediaResponseFactory mediaResponseFactory,
            ISecurityContext securityContext)
        {
            _mediaMongoRepository = mediaMongoRepository;
            _mediaResponseFactory = mediaResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<MediaDto>> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _securityContext.User!.Id;
            var media = await _mediaMongoRepository.FindByIdAsync(request.MediaId);

            if (media == null)
            {
                return new Result<MediaDto>(new EntityNotFoundException(typeof(Media), request.MediaId));
            }

            if (!media.IsOwnedBy(userId))
            {
                return new Result<MediaDto>(new ForbiddenAccessException());
            }

            return await _mediaResponseFactory.PrepareMediaDto(media);
        }
    }
}
