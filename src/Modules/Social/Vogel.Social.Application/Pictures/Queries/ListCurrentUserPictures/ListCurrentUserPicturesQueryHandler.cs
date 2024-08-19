using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Pictures.Factories;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Queries.ListCurrentUserPictures
{
    public class ListCurrentUserPicturesQueryHandler : IApplicationRequestHandler<ListCurrentUserPicturesQuery, Paging<PictureDto>>
    {
        private readonly PictureMongoRepository _pictureMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IPictureResponseFactory _pictureResponseFactory;
        public ListCurrentUserPicturesQueryHandler(PictureMongoRepository pictureMongoRepository, ISecurityContext securityContext, IPictureResponseFactory pictureResponseFactory)
        {
            _pictureMongoRepository = pictureMongoRepository;
            _securityContext = securityContext;
            _pictureResponseFactory = pictureResponseFactory;
        }

        public async Task<Result<Paging<PictureDto>>> Handle(ListCurrentUserPicturesQuery request, CancellationToken cancellationToken)
        {
            string curentUserId = _securityContext.User!.Id;

            var query = _pictureMongoRepository.AsMongoCollection()
                .Aggregate();

            query = query.Match(Builders<PictureMongoEntity>.Filter.Eq(x => x.UserId, curentUserId));

            var paged = await query.ToPaged(request.Cursor, request.Limit, request.Asending);

            var result = new Paging<PictureDto>
            {
                Data = await _pictureResponseFactory.PrepareListPictureDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }
    }
}
