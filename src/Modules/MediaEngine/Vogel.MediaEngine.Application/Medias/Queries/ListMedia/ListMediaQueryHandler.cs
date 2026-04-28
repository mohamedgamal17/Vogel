using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Application.Medias.Factories;
using Vogel.MediaEngine.MongoEntities.Medias;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Queries.ListMedia
{
    public class ListMediaQueryHandler : IApplicationRequestHandler<ListMediaQuery, Paging<MediaDto>>
    {
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IMediaResponseFactory _mediaResponseFactory;

        public ListMediaQueryHandler(
            IMongoRepository<MediaMongoEntity> mediaMongoRepository,
            ISecurityContext securityContext,
            IMediaResponseFactory mediaResponseFactory)
        {
            _mediaMongoRepository = mediaMongoRepository;
            _securityContext = securityContext;
            _mediaResponseFactory = mediaResponseFactory;
        }

        public async Task<Result<Paging<MediaDto>>> Handle(ListMediaQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContext.User!.Id;
            var paged = await _mediaMongoRepository
                .AsMongoCollection()
                .Aggregate()
                .Match(x => x.UserId == currentUserId)
                .ToPaged(request.Cursor, request.Limit, request.Asending);

            return new Paging<MediaDto>
            {
                Data = await _mediaResponseFactory.PrepareListMediaDto(paged.Data),
                Info = paged.Info,
            };
        }
    }
}
