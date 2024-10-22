using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public class ParticipantResponseFactory : IParticipantResponseFactory
    {
        private readonly IUserResponseFactory _userResponseFactory;

        public ParticipantResponseFactory(IUserResponseFactory userResponseFactory)
        {
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<ParticipantDto>> PrepareListParticipantDto(List<ParticipantMongoView> participants)
        {
            var tasks = participants.Select(PrepareParticipantDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<ParticipantDto> PrepareParticipantDto(ParticipantMongoView participant)
        {
            var dto = new ParticipantDto
            {
                Id = participant.Id,
                ConversationId = participant.ConversationId,
                UserId = participant.UserId,
            };

            if(participant.User != null)
            {
                dto.User = await _userResponseFactory.PreapreUserDto(participant.User);
            }

            return dto;
        }

    }
}
