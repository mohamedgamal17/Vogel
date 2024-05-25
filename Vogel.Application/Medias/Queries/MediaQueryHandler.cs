using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Medias.Factories;
using Vogel.Application.Medias.Policies;
using Vogel.Domain.Medias;
using Vogel.Domain.Utils;
namespace Vogel.Application.Medias.Queries
{
    public class MediaQueryHandler : 
        IApplicationRequestHandler<ListMediaQuery, List<MediaAggregateDto>>,
        IApplicationRequestHandler<GetMediaByIdQuery, MediaAggregateDto>
    {
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly ISecurityContext _securityContext;
        private readonly IMediaResponseFactory _mediaResponeFactory;
        private readonly IMongoDbRepository<Media> _mediaDbRepository;

        public MediaQueryHandler(IApplicationAuthorizationService applicationAuthorizationService, ISecurityContext securityContext, IMediaResponseFactory mediaResponeFactory, IMongoDbRepository<Media> mediaDbRepository)
        {
            _applicationAuthorizationService = applicationAuthorizationService;
            _securityContext = securityContext;
            _mediaResponeFactory = mediaResponeFactory;
            _mediaDbRepository = mediaDbRepository;
        }

        public async Task<Result<List<MediaAggregateDto>>> Handle(ListMediaQuery request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var filter = new FilterDefinitionBuilder<Media>()
                .Eq(x => x.UserId,currentUserId);

            var result =  await _mediaDbRepository.ApplyFilterAsync(filter);

            return await _mediaResponeFactory.PrepareListMediaAggregateDto(result);
        }

        public async Task<Result<MediaAggregateDto>> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
        {
            var media = await _mediaDbRepository.FindByIdAsync(request.Id);

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
