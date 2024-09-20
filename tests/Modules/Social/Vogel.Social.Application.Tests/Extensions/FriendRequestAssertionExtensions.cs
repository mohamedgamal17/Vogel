using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Friendship.Commands.SendFriendRequest;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Tests.Extensions
{
    public static class FriendRequestAssertionExtensions
    {
        public static void AssertSendFriendRequestCommand(this FriendRequest friendRequest, SendFriendRequestCommand command , string userId )
        {
            friendRequest.SenderId.Should().Be(userId);
            friendRequest.ReciverId.Should().Be(command.ReciverId);
            friendRequest.State.Should().Be(FriendRequestState.Pending);
        }



        public static void AssertFriendRequestMongoEntity(this FriendRequestMongoEntity mongoEntity, FriendRequest friendRequest)
        {
            mongoEntity.SenderId.Should().Be(friendRequest.SenderId);
            mongoEntity.ReciverId.Should().Be(friendRequest.ReciverId);
            mongoEntity.State.Should().Be(friendRequest.State);
            friendRequest.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertFriendRequestDto(this FriendRequestDto friendRequestDto, FriendRequest friendRequest, User? sender = null, User? reciver = null)
        {
            friendRequestDto.Id.Should().Be(friendRequest.Id);
            friendRequestDto.SenderId.Should().Be(friendRequest.SenderId);

            if (sender != null)
            {
                friendRequestDto.Sender.Should().NotBeNull();
                friendRequestDto.Sender.AssertUserDto(sender);
            }

            if (reciver != null)
            {
                friendRequestDto.Reciver.Should().NotBeNull();
                friendRequestDto.Reciver.AssertUserDto(reciver);

            }
        }
    }
}
