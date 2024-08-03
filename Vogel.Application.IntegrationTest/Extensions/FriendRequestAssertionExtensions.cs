using FluentAssertions;
using System.Security.Claims;
using Vogel.Application.Friendship.Commands;
using Vogel.Application.Friendship.Dtos;
using Vogel.Domain.Friendship;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Friendship;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class FriendRequestAssertionExtensions
    {
        public static void AssertSendFriendRequestCommand(this FriendRequest friendRequest , SendFriendRequestCommand command)
        {
            string userId = CurrentUser!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            friendRequest.SenderId.Should().Be(userId);
            friendRequest.ReciverId.Should().Be(command.ReciverId);
            friendRequest.State.Should().Be(Domain.Friendship.FriendRequestState.Pending);
        }



        public static void AssertFriendRequestMongoEntity(this FriendRequestMongoEntity mongoEntity , FriendRequest friendRequest)
        {
            mongoEntity.SenderId.Should().Be(friendRequest.SenderId);
            mongoEntity.ReciverId.Should().Be(friendRequest.ReciverId);
            mongoEntity.State.Should().Be((MongoDb.Entities.Friendship.FriendRequestState)friendRequest.State);
            friendRequest.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertFriendRequestDto(this FriendRequestDto friendRequestDto , FriendRequest friendRequest , User? sender =null , User? reciver = null)
        {
            friendRequestDto.Id.Should().Be(friendRequest.Id);
            friendRequestDto.SenderId.Should().Be(friendRequest.SenderId);

            if(sender != null)
            {
                friendRequestDto.Sender.Should().NotBeNull();
                friendRequestDto.Sender.AssertUserDto(sender);
            }

            if (reciver != null )
            {
                friendRequestDto.Reciver.Should().NotBeNull();
                friendRequestDto.Reciver.AssertUserDto(reciver);
               
            }
        }
    }
}
