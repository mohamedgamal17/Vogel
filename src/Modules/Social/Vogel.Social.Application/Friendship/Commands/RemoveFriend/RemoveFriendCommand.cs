using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.MongoEntities.Friendship;

namespace Vogel.Social.Application.Friendship.Commands.RemoveFriend
{
    [Authorize]
    public class RemoveFriendCommand : ICommand
    {
        public string Id { get; set; }
    }
}
