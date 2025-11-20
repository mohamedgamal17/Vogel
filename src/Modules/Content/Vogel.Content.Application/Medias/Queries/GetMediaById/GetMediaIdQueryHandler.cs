using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Application.Medias.Factories;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;

namespace Vogel.Content.Application.Medias.Queries.GetMediaById
{
    public class GetMediaIdQueryHandler : IApplicationRequestHandler<GetMediaByIdQuery, MediaDto>
    {
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;
        private readonly IMediaResponseFactory _mediaResponseFactory;
        private readonly ISecurityContext _securityContext;

        public GetMediaIdQueryHandler(IMongoRepository<MediaMongoEntity> mediaMongoRepository, IMediaResponseFactory mediaResponseFactory, ISecurityContext securityContext)
        {
            _mediaMongoRepository = mediaMongoRepository;
            _mediaResponseFactory = mediaResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<MediaDto>> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

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
