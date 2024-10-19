using AutoMapper;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Mappers
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            CreateMap<Conversation, ConversationMongoEntity>();

            CreateMap<Participant, ParticipantMongoEntity>();
                
        }
    }
}
