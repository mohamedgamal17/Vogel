using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Messages.Events;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Conversations.Commands.MarkConversationAsSeen
{
    public class MarkConversationAsSeenCommandHandler : IApplicationRequestHandler<MarkConversationAsSeenCommand, Unit>
    {
        const int BATCH_SIZE = 100;
        private readonly IMessangerRepository<Conversation> _conversationRepository;
        private readonly IMessangerRepository<Message> _messageRepository;
        private readonly IMessangerRepository<MessageLog> _messageActivityRepository;
        private readonly IMessangerRepository<Participant> _participantRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public MarkConversationAsSeenCommandHandler(IMessangerRepository<Conversation> conversationRepository, IMessangerRepository<Message> messageRepository, IMessangerRepository<MessageLog> messageActivityRepository, IMessangerRepository<Participant> participantRepository, IPublishEndpoint publishEndpoint)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _messageActivityRepository = messageActivityRepository;
            _participantRepository = participantRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<Unit>> Handle(MarkConversationAsSeenCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationRepository.SingleOrDefaultAsync(x => x.Id == request.ConversationId);

            if (conversation == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var authorizationResult = await CheckIfUserParticipantInConversation(conversation.Id, request.UserId);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }

            var query = from message in _messageRepository.AsQuerable().Where(x => x.ConversationId == request.ConversationId && x.SenderId != request.UserId)
                        join messageActivity in _messageActivityRepository.AsQuerable()
                        on message.Id equals messageActivity.MessageId into joined
                        from activity in joined.DefaultIfEmpty()
                        select new { Message = message, Activity = activity };


            var result = from pr in query
                         where pr.Activity == null
                         select pr.Message;
            var unReadedMessages = await result.ToListAsync();

            int counter = 0;

            var seenAt = DateTime.UtcNow;

            while (counter < unReadedMessages.Count)
            {
                var batch = unReadedMessages.Skip(counter).Take(BATCH_SIZE).ToList();

                List<MessageLog> messageActivities = new List<MessageLog>();

                foreach (var item in batch)
                {
                    var activity = new MessageLog
                    {
                        MessageId = item.Id,
                        SeenById = request.UserId,
                        SeenAt = seenAt
                    };

                    messageActivities.Add(activity);
                }

                await _messageActivityRepository.InsertManyAsync(messageActivities);

                counter += BATCH_SIZE;
            }

            var @event = new LogMessagesActivitiesEvent(request.ConversationId, request.UserId, seenAt);

            await _publishEndpoint.Publish(@event);

            return Unit.Value;
        }

        private async Task<Result<Unit>> CheckIfUserParticipantInConversation(string conversationtId, string userId)
        {
            var participant = await _participantRepository.SingleOrDefaultAsync(x => x.ConversationId == conversationtId && x.UserId == userId);

            if (participant == null)
            {
                return new Result<Unit>(new ForbiddenAccessException($"Current user is not participant in conversation : ({conversationtId})"));
            }

            return Unit.Value;
        }
    }
}
