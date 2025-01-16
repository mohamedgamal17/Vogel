using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;

namespace Vogel.Messanger.Application.Tests.Fakers
{
    public class FakeUserFriendService : IUserFriendService
    {
        private static Dictionary<string, List<FriendDto>> _friends = new Dictionary<string, List<FriendDto>>();


        public async Task<Result<Paging<FriendDto>>> ListFriends(string userId, string? cursor = null, bool ascending = false, int limit = 10)
        {
            var userFriends = _friends[userId];

            return await Task.FromResult(PageResult(userFriends, cursor, ascending, limit));

        }

        private Paging<FriendDto> PageResult(List<FriendDto> data, string? cursor = null, bool ascending = false, int limit = 10)
        {

            var orderedData = ascending ? data.OrderBy(x => x.Id) : data.OrderByDescending(x => x.Id);

            var filterdData = orderedData.AsEnumerable();

            if (cursor != null)
            {

                filterdData = ascending ? filterdData.Where(x => x.Id.CompareTo(cursor) >= 0) : filterdData.Where(x => x.Id.CompareTo(cursor) <= 0);
            }

            var pagingInfo = PreparePagingInfo(filterdData, cursor, limit, ascending);

            return new Paging<FriendDto>
            {
                Data = filterdData.ToList(),
                Info = pagingInfo
            };

            PagingInfo PreparePagingInfo(IEnumerable<FriendDto> data, string? cursor = null
          , int limit = 10, bool ascending = false)
            {
                if (cursor != null)
                {
                    var previous = ascending ? data.Where(x => x.Id.CompareTo(cursor) < 0).FirstOrDefault() : data.Where(x => x.Id.CompareTo(cursor) > 0).FirstOrDefault();
                    var next = ascending ? data.Where(x => x.Id.CompareTo(cursor) > 0).FirstOrDefault() : data.Where(x => x.Id.CompareTo(cursor) < 0).FirstOrDefault();

                    return new PagingInfo(next?.Id, previous?.Id, ascending);
                }
                else
                {
                    var next = data.Skip(limit - 1).FirstOrDefault();

                    return new PagingInfo(next?.Id, null, ascending);
                }
            }
        }

        public void AddRangeOfFriens(UserDto source, List<UserDto> targets)
        {
            foreach (var target in targets)
            {
                AddFriend(source, target);
            }
        }

        public void AddFriend(UserDto source, UserDto target)
        {
            var friend = PrepareFriendDto(source, target);

            if (!_friends.TryGetValue(source.Id, out var _))
            {
                _friends[source.Id] = new List<FriendDto>();
            }

            _friends[source.Id].Add(friend);
        }

        private FriendDto PrepareFriendDto(UserDto source, UserDto target)
        {
            var friend = new FriendDto
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = source.Id,
                TargetId = target.Id,
                Source = source,
                Target = target
            };

            return friend;
        }
        public List<FriendDto> GetUserFriends(string id)
        {
            return _friends[id];
        }


        public List<FriendDto> PickRandomFriend(string userId, int count)
        {
            return _friends[userId].PickRandom(count);
        }

        public FriendDto? PickRandomFriend(string userId)
        {
            return _friends[userId].PickRandom();
        }
        public void Reset()
        {
            _friends.Clear();
        }
    }
}
