using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public class ConversationResponseFactory : IConversationResponseFactory
    {
        private readonly IParticipantResponseFactory _participantResponseFactory;
        private readonly IMessageResponseFactory _messageResponseFactory;

        public ConversationResponseFactory(IParticipantResponseFactory participantResponseFactory, IMessageResponseFactory messageResponseFactory)
        {
            _participantResponseFactory = participantResponseFactory;
            _messageResponseFactory = messageResponseFactory;
        }

        public async Task<List<ConversationDto>> PrepareListConversationDto(List<ConversationMongoView> conversations)
        {
            var tasks = conversations.Select(PrepareConversationDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<ConversationDto> PrepareConversationDto(ConversationMongoView conversation)
        {
            var dto = new ConversationDto
            {
                Id = conversation.Id,
                Name = conversation.Name,
                Participants = new BuildingBlocks.Shared.Models.Paging<ParticipantDto>
                {
                    Data = await _participantResponseFactory.PrepareListParticipantDto(conversation.Participants.Data),
                    Info =  conversation.Participants.Info
                },
                Messages = new BuildingBlocks.Shared.Models.Paging<Messages.Dtos.MessageDto>
                {
                    Data = await _messageResponseFactory.PrepareListMessageDto(conversation.Messages.Data),
                    Info  = conversation.Messages.Info
                }
            };

            return dto;
        }
    }
}
