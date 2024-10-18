using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public class ConversationResponseFactory : IConversationResponseFactory
    {
        private readonly IParticipantResponseFactory _participantResponseFactory;

        public ConversationResponseFactory(IParticipantResponseFactory participantResponseFactory)
        {
            _participantResponseFactory = participantResponseFactory;
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
                Participants = await _participantResponseFactory.PrepareListParticipantDto(conversation.Participants)
            };

            return dto;
        }
    }
}
