using MongoDB.Driver;
using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Friendship.Factories;
using Vogel.Application.Friendship.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Friendship;
namespace Vogel.Application.Friendship.Queries
{
    public class FriendRequestQueryHandler : 
        IApplicationRequestHandler<ListFriendRequestQuery, Paging<FriendRequestDto>>,
        IApplicationRequestHandler<GetFriendRequestByIdQuery, FriendRequestDto>
    {
        private readonly FriendRequestMongoViewRepository _friendRequestMongoviewRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public FriendRequestQueryHandler(FriendRequestMongoViewRepository friendRequestMongoviewRepository, IApplicationAuthorizationService applicationAuthorizationService, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestMongoviewRepository = friendRequestMongoviewRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<Paging<FriendRequestDto>>> Handle(ListFriendRequestQuery request, CancellationToken cancellationToken)
        {
            var query = _friendRequestMongoviewRepository.AsMongoCollection().Aggregate();

            query = SortQuery(query, request);

            var data = await Paginate(query, request);

            var pagingInfo = await PreparePagingInfo(query, request);


            var response = new Paging<FriendRequestDto>()
            {
                Data = await _friendshipResponseFactory.PrepareListFriendRequestDto(data),
                Info = pagingInfo
            };

            return response;
        }

        public async Task<Result<FriendRequestDto>> Handle(GetFriendRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestMongoviewRepository.FindByIdAsync(request.Id);

            if(friendRequest == null)
            {
                return new Result<FriendRequestDto>(new EntityNotFoundException(typeof(FriendRequest), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friendRequest, FriendshipOperationalRequirments.IsFriendRequestOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendRequestDto>(authorizationResult.Exception!);
            }

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequest);
        }

        private IAggregateFluent<FriendRequestMongoView> SortQuery(IAggregateFluent<FriendRequestMongoView> query, ListFriendRequestQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        private async Task<List<FriendRequestMongoView>> Paginate(IAggregateFluent<FriendRequestMongoView> query, ListFriendRequestQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<FriendRequestMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<FriendRequestMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<FriendRequestMongoView> query, ListFriendRequestQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<FriendRequestMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<FriendRequestMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<FriendRequestMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<FriendRequestMongoView>.Filter.Lt(x => x.Id, request.Cursor);

                var next = await query.Match(nextFilter).Skip(request.Limit - 1).FirstOrDefaultAsync();

                var previos = await query.Match(previosFilter).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, previos?.Id, request.Asending);
            }
            else
            {
                var next = await query.Skip(request.Limit - 1).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, null, request.Asending);
            }
        }
     

    }
}
