using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Application.Medias.Factories;
using Vogel.Content.Application.Medias.Policies;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;

namespace Vogel.Content.Application.Medias.Queries.GetMediaById
{
    public class GetMediaIdQueryHandler : IApplicationRequestHandler<GetMediaByIdQuery, MediaDto>
    {
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IMediaResponseFactory _mediaResponseFactory;
        public GetMediaIdQueryHandler(IMongoRepository<MediaMongoEntity> mediaMongoRepository,  IApplicationAuthorizationService applicationAuthorizationService, IMediaResponseFactory mediaResponseFactory)
        {
            _mediaMongoRepository = mediaMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _mediaResponseFactory = mediaResponseFactory;
        }

        public async Task<Result<MediaDto>> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            var media = await _mediaMongoRepository.FindByIdAsync(request.MediaId);

            if (media == null)
            {
                return new Result<MediaDto>(new EntityNotFoundException(typeof(Media), request.MediaId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<MediaDto>(authorizationResult.Exception!);
            }

            return await _mediaResponseFactory.PrepareMediaDto(media);
        }
    }
}
