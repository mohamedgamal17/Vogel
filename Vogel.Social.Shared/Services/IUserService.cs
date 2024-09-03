using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Shared.Services
{
    public interface IUserService
    {
        Task<Result<Paging<UserDto>>> ListUsers(string? cursor = null, bool ascending = false, int limit = 10);
        Task<Result<UserDto>> GetUserById(string id);
    }
}

