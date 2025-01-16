using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
using Vogel.Application.Tests.Extensions;
namespace Vogel.Messanger.Application.Tests.Fakers
{
    public class FakeUserService : IUserService
    {
        private static List<UserDto> _users = new List<UserDto>();
        public Task<Result<UserDto>> GetUserById(string id)
        {
            var user = _users.Single(x => x.Id == id);

            var result = new Result<UserDto>(user);

            return Task.FromResult(result);
        }

        public async Task<Result<Paging<UserDto>>> ListUsers(string? cursor = null, bool ascending = false, int limit = 10)
        {
            return await Task.FromResult(PageResult(_users, cursor, ascending, limit));
        }

        public async Task<Result<Paging<UserDto>>> ListUsersByIds(IEnumerable<string> ids, string? cursor = null, bool ascending = false, int limit = 10)
        {
            var data = _users.Where(x => ids.Contains(x.Id)).ToList();

            return await Task.FromResult(PageResult(data, cursor, ascending, limit));
        }

        private Paging<UserDto> PageResult(List<UserDto> data, string? cursor = null, bool ascending = false, int limit = 10)
        {

            var orderedData = ascending ? data.OrderBy(x => x.Id) : data.OrderByDescending(x => x.Id);

            var filterdData = orderedData.AsEnumerable();

            if (cursor != null)
            {

                filterdData = ascending ? filterdData.Where(x => x.Id.CompareTo(cursor) >= 0) : filterdData.Where(x => x.Id.CompareTo(cursor) <= 0);
            }

            var pagingInfo = PreparePagingInfo(filterdData, cursor, limit, ascending);

            return new Paging<UserDto>
            {
                Data = filterdData.ToList(),
                Info = pagingInfo
            };

            PagingInfo PreparePagingInfo(IEnumerable<UserDto> data, string? cursor = null
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



        public void AddUser(UserDto user)
        {
            _users.Add(user);
        }


        public void AddRangeOfUsers(IEnumerable<UserDto> users)
        {
            foreach (var user in users)
            {
                AddUser(user);
            }
        }

        public UserDto? PickRandomUser()
        {
            return _users.PickRandom();
        }
        public List<UserDto>? PickRandomUser(int count)
        {
            return _users.PickRandom(count);
        }

        public void Reset()
        {
            _users.Clear();

        }
    }
}
