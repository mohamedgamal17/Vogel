using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public interface IParticipantResponseFactory : IResponseFactory
    {
        Task<List<ParticipantDto>> PrepareListParticipantDto(List<ParticipantMongoView> participants);

        Task<ParticipantDto> PrepareParticipantDto(ParticipantMongoView participant);
    }
}
