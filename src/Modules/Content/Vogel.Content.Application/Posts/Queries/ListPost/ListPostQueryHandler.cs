using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.MongoEntities.Posts;
using Vogel.Social.Shared.Services;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.Medias;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.Content.Application.Posts.Factories;
namespace Vogel.Content.Application.Posts.Queries.ListPost
{
    public class ListPostQueryHandler : IApplicationRequestHandler<ListPostQuery, Paging<PostDto>>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IUserFriendService _userFriendService;
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoEntity;
        private readonly IPostResponseFactory _postResponseFactory;

        public ListPostQueryHandler(PostMongoRepository postMongoRepository, ISecurityContext securityContext, IUserFriendService userFriendService, IMongoRepository<MediaMongoEntity> mediaMongoEntity, IPostResponseFactory postResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _securityContext = securityContext;
            _userFriendService = userFriendService;
            _mediaMongoEntity = mediaMongoEntity;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<Paging<PostDto>>> Handle(ListPostQuery request, CancellationToken cancellationToken)
        {
            var userFriends = await PrepareUserFriends();

            var query = _postMongoRepository.AsMongoCollection()
                 .Aggregate()
                 .Lookup<PostMongoEntity, MediaMongoEntity, PostMongoView>(
                     _mediaMongoEntity.AsMongoCollection(),
                     l => l.MediaId,
                     f => f.Id,
                     x => x.Media
                 )
                 .Unwind(x => x.Media, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });


            query = query.Match(
                   Builders<PostMongoView>.Filter.In(x => x.UserId, userFriends)
                );


            var paged = await query.ToPaged(request.Cursor, request.Limit, request.Asending);

            var response = new Paging<PostDto>
            {
                Data = await _postResponseFactory.PrepareListPostDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }


        private async Task<List<string>> PrepareUserFriends()
        {
            string userId = _securityContext.User!.Id;

            List<string> friends = new List<string>();

            bool hasNext = true;
            string? cursor = null;
            int limit = 100;

            while (hasNext)
            {
                var response = await _userFriendService.ListFriends(userId, cursor, limit: limit);

                response.ThrowIfFailure();
                response.Value!.Data.ForEach(f =>
                {
                    string friendId = f.SourceId == userId ? f.TargetId : f.SourceId;
                    friends.Add(friendId);
                });
                hasNext = response.Value!.Info.HasNext;
                cursor = response.Value!.Info.NextCursor;
            }

            friends.Add(userId);

            return friends;
        }
    }
}
