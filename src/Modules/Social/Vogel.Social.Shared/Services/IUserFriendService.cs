using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Shared.Services
{
    public interface IUserFriendService
    {
        Task<Result<Paging<FriendDto>>> ListFriends(string userId,string? cursor = null, bool ascending = false, int limit = 10);
    }
}
