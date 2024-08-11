using MongoDB.Driver;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Medias.Factories;
using Vogel.Application.Medias.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Medias;
using Vogel.MongoDb.Entities.Medias;
namespace Vogel.Application.Medias.Queries
{
    public class MediaQueryHandler : 
        IApplicationRequestHandler<ListMediaQuery, List<MediaAggregateDto>>,
        IApplicationRequestHandler<GetMediaByIdQuery, MediaAggregateDto>
    {
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly ISecurityContext _securityContext;
        private readonly IMediaResponseFactory _mediaResponeFactory;
        private readonly MediaMongoRepository _mediaMongoRepository;

        public MediaQueryHandler(IApplicationAuthorizationService applicationAuthorizationService,
            ISecurityContext securityContext,
            IMediaResponseFactory mediaResponeFactory, 
            MediaMongoRepository mediaMongoRepository)
        {
            _applicationAuthorizationService = applicationAuthorizationService;
            _securityContext = securityContext;
            _mediaResponeFactory = mediaResponeFactory;
            _mediaMongoRepository = mediaMongoRepository;
        }

        public async Task<Result<List<MediaAggregateDto>>> Handle(ListMediaQuery request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var filter = new FilterDefinitionBuilder<MediaMongoEntity>()
                .Eq(x => x.UserId,currentUserId);

            var result =  await _mediaMongoRepository.ApplyFilterAsync(filter);

            return await _mediaResponeFactory.PrepareListMediaAggregateDto(result);
        }

        public async Task<Result<MediaAggregateDto>> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            var media = await _mediaMongoRepository.FindByIdAsync(request.Id);

            if(media == null)
            {
                return new Result<MediaAggregateDto>(new EntityNotFoundException(typeof(Media), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<MediaAggregateDto>(authorizationResult.Exception!);
            }

            return await _mediaResponeFactory.PrepareMedaiAggregateDto(media);
        }   
    }
}
