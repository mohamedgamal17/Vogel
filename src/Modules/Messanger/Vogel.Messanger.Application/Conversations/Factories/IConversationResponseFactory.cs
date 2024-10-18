using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public interface IConversationResponseFactory
    {
        Task<List<ConversationDto>> PrepareListConversationDto(List<ConversationMongoView> conversations);
        Task<ConversationDto> PrepareConversationDto(ConversationMongoView conversation);
    }
}
