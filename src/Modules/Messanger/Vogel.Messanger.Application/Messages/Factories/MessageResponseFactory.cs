using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.MongoEntities.Messages;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Messanger.Application.Messages.Factories
{
    public class MessageResponseFactory : IMessageResponseFactory

    {
        private readonly IUserService _userService;

        public MessageResponseFactory(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<MessageDto>> PrepareListMessageDto(List<MessageMongoEntity> messages)
        {
            if(messages == null || messages.Count <= 0)
            {
                return new List<MessageDto>();
            }

            var message = messages.First();

            var users = new List<string> { message.SenderId, message.ReciverId };

            var usersDict = await PrepareDictionaryOfUsers(users);

            return messages.Select(msg => PrepareMessageDto(msg, usersDict[msg.SenderId], usersDict[msg.ReciverId])).ToList();
        }

        public async Task<MessageDto> PrepareMessageDto(MessageMongoEntity message)
        {
            var users = new List<string> { message.SenderId, message.ReciverId };

            var usersDict = await PrepareDictionaryOfUsers(users);

            return PrepareMessageDto(message, usersDict[message.SenderId], usersDict[message.ReciverId]);
        }

        private MessageDto PrepareMessageDto(MessageMongoEntity message , UserDto sender , UserDto reciver)
        {
            var dto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                Sender = sender,
                Reciver = reciver,
                ReciverId = message.ReciverId,
                IsSeen = message.IsSeen,
                Content = message.Content,
            };

            return dto;
        }

        private async Task<Dictionary<string, UserDto>> PrepareDictionaryOfUsers(List<string> ids)
        {

            var result = await _userService.ListUsersByIds(ids, limit: ids.Count);

            result.ThrowIfFailure();

            return result.Value!.Data.ToDictionary((k) => k.Id, v => v);
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var userResult = await _userService.GetUserById(id);

            userResult.ThrowIfFailure();

            return userResult.Value!;
        }
    }
}
