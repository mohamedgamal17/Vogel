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
    public class FriendQueryHandler :
        IApplicationRequestHandler<ListFriendQuery, Paging<FriendDto>>,
        IApplicationRequestHandler<GetFriendByIdQuery, FriendDto>
    {
        private readonly FriendMongoViewRepository _friendMongoViewRepository;
        private readonly IFriendshipResponseFactory _friendResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public FriendQueryHandler(FriendMongoViewRepository friendMongoViewRepository, IFriendshipResponseFactory friendResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _friendMongoViewRepository = friendMongoViewRepository;
            _friendResponseFactory = friendResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Paging<FriendDto>>> Handle(ListFriendQuery request, CancellationToken cancellationToken)
        {
            var query = _friendMongoViewRepository.AsMongoCollection().Aggregate();

            query = SortQuery(query, request);

            var data = await Paginate(query, request);

            var pagingInfo = await PreparePagingInfo(query, request);

            var response = new Paging<FriendDto>
            {
                Data = await _friendResponseFactory.PrepareListFriendDto(data),
                Info = pagingInfo
            };

            return response;
        }

        public async Task<Result<FriendDto>> Handle(GetFriendByIdQuery request, CancellationToken cancellationToken)
        {
            var friend = await _friendMongoViewRepository.FindByIdAsync(request.Id);

            if(friend == null)
            {
                return new Result<FriendDto>(new EntityNotFoundException(typeof(Friend), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friend, FriendshipOperationalRequirments.IsFriendOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendDto>(authorizationResult.Exception!);
            }


            return await _friendResponseFactory.PrepareFriendDto(friend);
         }


        private IAggregateFluent<FriendMongoView> SortQuery(IAggregateFluent<FriendMongoView> query, ListFriendQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        private async Task<List<FriendMongoView>> Paginate(IAggregateFluent<FriendMongoView> query, ListFriendQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<FriendMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<FriendMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<FriendMongoView> query, ListFriendQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<FriendMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<FriendMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<FriendMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<FriendMongoView>.Filter.Lt(x => x.Id, request.Cursor);

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
