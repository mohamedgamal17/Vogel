using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.MongoEntities.Medias;
using Vogel.Content.MongoEntities.Posts;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Posts.Queries.GetUserPostById
{
    public class GetUserPostByUdQueryHandler : IApplicationRequestHandler<GetUserPostByIdQuery, PostDto>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public GetUserPostByUdQueryHandler(PostMongoRepository postMongoRepository, IMongoRepository<MediaMongoEntity> mediaMongoRepository, IPostResponseFactory postResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _mediaMongoRepository = mediaMongoRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<PostDto>> Handle(GetUserPostByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _postMongoRepository.AsMongoCollection()
                .Aggregate()
                .Lookup<PostMongoEntity, MediaMongoEntity, PostMongoView>(
                    foreignCollection: _mediaMongoRepository.AsMongoCollection(),
                    localField: l => l.MediaId,
                    foreignField: f => f.Id,
                    @as: r => r.Media
                )
                .Unwind(x => x.Media, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });

            query = query.Match(
                Builders<PostMongoView>.Filter.And(
                    Builders<PostMongoView>.Filter.Eq(x => x.UserId, request.UserId),
                    Builders<PostMongoView>.Filter.Eq(x => x.Id, request.PostId)
                )
              );

            var post = await query.SingleOrDefaultAsync();

            if (post == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            return await _postResponseFactory.PreparePostDto(post);
        }
    }
}
