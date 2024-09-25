using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.MongoEntities.Medias;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Posts.Queries.ListUserPost
{
    public class ListUserPostQueryHandler : IApplicationRequestHandler<ListUserPostQuery, Paging<PostDto>>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public ListUserPostQueryHandler(PostMongoRepository postMongoRepository, IMongoRepository<MediaMongoEntity> mediaMongoRepository, IPostResponseFactory postResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _mediaMongoRepository = mediaMongoRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<Paging<PostDto>>> Handle(ListUserPostQuery request, CancellationToken cancellationToken)
        {
            var query = _postMongoRepository.AsMongoCollection()
             .Aggregate()
             .Lookup<PostMongoEntity, MediaMongoEntity, PostMongoView>(
                 _mediaMongoRepository.AsMongoCollection(),
                 l => l.MediaId,
                 f => f.Id,
                 x => x.Media
             )
             .Unwind(x => x.Media, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });

            query = query.Match(Builders<PostMongoView>.Filter.Eq(x => x.UserId, request.UserId));

            var paged = await query.ToPaged(request.Cursor, request.Limit, request.Asending);

            var response = new Paging<PostDto>
            {
                Data = await _postResponseFactory.PrepareListPostDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }
    }
}
