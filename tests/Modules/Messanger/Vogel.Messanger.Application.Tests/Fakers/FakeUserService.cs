using NUnit.Framework.Constraints;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;

namespace Vogel.Messanger.Application.Tests.Fakers
{
    public class FakeUserService : IUserService
    {
        public async Task<Result<UserDto>> GetUserById(string id)
        {
            return new UserDto
            {
                Id = id,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Gender = Social.Shared.Common.Gender.Male,
                BirthDate = DateTime.Now.AddYears(-18).ToShortDateString()
            };
        }

        public async Task<Result<Paging<UserDto>>> ListUsers(string? cursor = null, bool ascending = false, int limit = 10)
        {
            var data = Enumerable.Range(0, limit)
                .Select(_ => new UserDto
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Gender = Social.Shared.Common.Gender.Male,
                    BirthDate = DateTime.Now.AddYears(-18).ToShortDateString()
                }).ToList();

            return new Paging<UserDto>
            {
                Data = data,
                Info = new PagingInfo(cursor, cursor, ascending)

            };
        }

        public async Task<Result<Paging<UserDto>>> ListUsersByIds(IEnumerable<string> ids, string? cursor = null, bool ascending = false, int limit = 10)
        {
            var data = ids.Select(id => new UserDto
            {
                Id = id,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Gender = Social.Shared.Common.Gender.Male,
                BirthDate = DateTime.Now.AddYears(-18).ToShortDateString()

            }).ToList();

            return new Paging<UserDto>
            {
                Data = data,
                Info = new PagingInfo(cursor, cursor, ascending)

            };
        }
    }
}
