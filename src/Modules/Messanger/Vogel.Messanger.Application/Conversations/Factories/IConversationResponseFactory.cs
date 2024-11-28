using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public interface IConversationResponseFactory : IResponseFactory
    {
        Task<List<ConversationDto>> PrepareListConversationDto(List<ConversationMongoView> views);
        Task<ConversationDto> PrepareConversationDto(ConversationMongoView conversation);
    }
}
