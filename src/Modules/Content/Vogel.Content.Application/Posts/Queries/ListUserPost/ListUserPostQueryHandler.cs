using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Posts.Queries.ListUserPost
{
    public class ListUserPostQueryHandler : IApplicationRequestHandler<ListUserPostQuery, Paging<PostDto>>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public ListUserPostQueryHandler(PostMongoRepository postMongoRepository, IPostResponseFactory postResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<Paging<PostDto>>> Handle(ListUserPostQuery request, CancellationToken cancellationToken)
        {
            var query = _postMongoRepository.PreparePostViewQuery();

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
