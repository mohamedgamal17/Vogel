using AutoMapper;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
namespace Vogel.Messanger.Application.Messages.Mappers
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageMongoEntity>();
            CreateMap<MessageActivity, MessageActivityMongoEntity>();
        }
    }
}
