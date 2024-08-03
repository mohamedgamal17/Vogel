using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.MongoDb.Entities.Friendship;

namespace Vogel.Application.Friendship.Factories
{
    public class FriendshipResponseFactory : IFriendshipResponseFactory
    {
        private readonly IUserResponseFactory _userResponseFactory;

        public FriendshipResponseFactory(IUserResponseFactory userResponseFactory)
        {
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<FriendRequestDto>> PrepareListFriendRequestDto(List<FriendRequestMongoView> friendRequests)
        {
            var tasks = friendRequests.Select(PrepareFriendRequestDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<FriendRequestDto> PrepareFriendRequestDto(FriendRequestMongoView friendRequest)
        {
            var dto = new FriendRequestDto
            {
                Id = friendRequest.Id,
                SenderId = friendRequest.SenderId,
                ReciverId = friendRequest.ReciverId,
                State = friendRequest.State,
            };

            if(friendRequest.Sender != null)
            {
                dto.Sender = await _userResponseFactory.PrepareUserDto(friendRequest.Sender);
            }

            if(friendRequest.Reciver != null)
            {
                dto.Reciver = await _userResponseFactory.PrepareUserDto(friendRequest.Reciver);
            }

            return dto;
        }


        public Task<List<FriendDto>> PrepareListFriendDto(List<FriendMongoView> friends)
        {
            throw new NotImplementedException();
        }

        public async Task<FriendDto> PrepareFriendDto(FriendMongoView friend)
        {
            var dto = new FriendDto
            {
                Id = friend.Id,
                SourceId = friend.SourceId,
                TargetId = friend.TargetId,
            };

            if(friend.Source != null)
            {
                dto.Source = await _userResponseFactory.PrepareUserDto(friend.Source);
            }

            if(friend.Target != null)
            {
                dto.Target = await _userResponseFactory.PrepareUserDto(friend.Target);
            }

            return dto;
        }

    

     

       
    }
}
