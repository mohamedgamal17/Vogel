using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
using Vogel.Messanger.MongoEntities.Users;
namespace Vogel.Messanger.Application.Messages.Factories
{
    public interface IMessageResponseFactory : IResponseFactory
    {
        Task<List<MessageDto>> PrepareListMessageDto(List<MessageMongoView> messages);
        Task<MessageDto> PrepareMessageDto(MessageMongoView message);
        Task<MessageDto> PrepareMessageDto(Message message, UserMongoEntity? sender);
    }
}
