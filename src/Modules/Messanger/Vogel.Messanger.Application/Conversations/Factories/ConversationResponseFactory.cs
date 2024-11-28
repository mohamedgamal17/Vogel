using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Factories
{
    public class ConversationResponseFactory : IConversationResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        private readonly IParticipantResponseFactory _participantResponseFactory;

        public ConversationResponseFactory(IS3ObjectStorageService s3ObjectStorageService, IParticipantResponseFactory participantResponseFactory)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
            _participantResponseFactory = participantResponseFactory;
        }

        public async Task<List<ConversationDto>> PrepareListConversationDto(List<ConversationMongoView> views)
        {
            var tasks = views.Select(PrepareConversationDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<ConversationDto> PrepareConversationDto(ConversationMongoView view)
        {
            var dto = new ConversationDto
            {
                Id = view.Id,
                Name = view.Name,
                Avatar = view.Avatar,
                TotalParticpants = view.TotalParticpants,
                Participants = await _participantResponseFactory.PrepareListParticipantDto(view.Participants)         
            };

            if(view.Avatar != null)
            {
                dto.Avatar = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(view.Avatar);
            }

            return dto;
        }
    }
}
