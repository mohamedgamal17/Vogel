using FluentAssertions;
using Vogel.Application.Friendship.Dtos;
using Vogel.Domain.Friendship;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Friendship;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class FriendAssertionExtensions
    {
        public static void AssertFriend(this Friend friend , string sourceId , string targetId)
        {
            friend.SourceId.Should().Be(sourceId);
            friend.TargetId.Should().Be(targetId);
        }

        public static void AssertFriendMongoEntity(this FriendMongoEntity mongoEntity , Friend friend)
        {
            mongoEntity.Id.Should().Be(friend.Id);
            mongoEntity.SourceId.Should().Be(friend.SourceId);
            friend.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertFriendDto(this FriendDto friendDto , Friend friend , UserAggregate? source = null, UserAggregate? target = null)
        {
            friendDto.Id.Should().Be(friend.Id);
            friendDto.SourceId.Should().Be(friend.SourceId);
            friendDto.TargetId.Should().Be(friend.TargetId);

            if(source != null)
            {
                friendDto.Source.Should().NotBeNull();
                friendDto.Source.AssertPublicUserDto(source);
            }

            if(target != null)
            {
                friendDto.Target.Should().NotBeNull();
                friendDto.Target.AssertPublicUserDto(target);
            }
        }
    }
}
