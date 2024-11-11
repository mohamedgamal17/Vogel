using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
using Vogel.Messanger.MongoEntities.Users;
namespace Vogel.Messanger.Application.Messages.Factories
{
    public class MessageResponseFactory : IMessageResponseFactory
    {
        private readonly IUserResponseFactory _userResponseFactory;

        public MessageResponseFactory(IUserResponseFactory userResponseFactory)
        {
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<MessageDto>> PrepareListMessageDto(List<MessageMongoView> messages)
        {
            if(messages == null || messages.Count <= 0)
            {
                return new List<MessageDto>();
            }

            var tasks = messages.Select(PrepareMessageDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<MessageDto> PrepareMessageDto(MessageMongoView message)
        {
            var dto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content
            };

            if (message.Sender != null)
            {
                dto.Sender = await _userResponseFactory.PreapreUserDto(message.Sender);
            }

            return dto;
        }

        public async Task<MessageDto> PrepareMessageDto(Message message, UserMongoEntity? sender)
        {
            var dto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content
            };

            if (sender != null)
            {
                dto.Sender = await _userResponseFactory.PreapreUserDto(sender);
            }

            return dto;
        }
    }
}
