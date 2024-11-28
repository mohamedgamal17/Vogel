using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Commands.CreateConversation
{
    public class CreateConversationCommandHandler : IApplicationRequestHandler<CreateConversationCommand, ConversationDto>
    {
        private readonly IMessangerRepository<Conversation> _conversationRepository;
        private readonly IMessangerRepository<Participant> _participantRepository;
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IConversationResponseFactory _conversationResponseFactory;
        public CreateConversationCommandHandler(IMessangerRepository<Conversation> conversationRepository, IMessangerRepository<Participant> participantRepository, ConversationMongoRepository conversationMongoRepository, ISecurityContext securityContext, IConversationResponseFactory conversationResponseFactory)
        {
            _conversationRepository = conversationRepository;
            _participantRepository = participantRepository;
            _conversationMongoRepository = conversationMongoRepository;
            _securityContext = securityContext;
            _conversationResponseFactory = conversationResponseFactory;
        }

        public async Task<Result<ConversationDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var conversation = new Conversation
            {
                Name = request.Name,
            };

            await _conversationRepository.InsertAsync(conversation);

            var usersIds = PrepareConversationParticipants(request);

            List<Participant> participants = new List<Participant>();

            foreach (var user in usersIds)
            {
                var participant = new Participant
                {
                    UserId = user,
                    ConversationId = conversation.Id,
                };

                participants.Add(participant);
            }


            await _participantRepository.InsertManyAsync(participants);

            var conversationMongoEntity = await _conversationMongoRepository.FindViewAsync(currentUserId, conversation.Id);

            return await _conversationResponseFactory.PrepareConversationDto(conversationMongoEntity!);
        }


        private List<string> PrepareConversationParticipants(CreateConversationCommand command)
        {
            List<string> participant = new List<string> { _securityContext.User!.Id };

            command.Participants!.ForEach((p) => participant.Add(p));

            return participant;
        }
    }
}
