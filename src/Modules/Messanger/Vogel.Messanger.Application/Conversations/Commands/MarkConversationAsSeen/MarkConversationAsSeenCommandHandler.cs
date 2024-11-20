using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Application.Messages.Events;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Conversations.Commands.MarkConversationAsSeen
{
    public class MarkConversationAsSeenCommandHandler : IApplicationRequestHandler<MarkConversationAsSeenCommand, Unit>
    {
        private readonly IRepository<Conversation> _conversationRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<MessageActivity> _messageActivityRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IPublishEndpoint _publishEndpoint;

        const int BATCH_SIZE = 100;
        public MarkConversationAsSeenCommandHandler(IRepository<Conversation> conversationRepository, IRepository<Message> messageRepository, IRepository<MessageActivity> messageActivityRepository, IApplicationAuthorizationService applicationAuthorizationService, IPublishEndpoint publishEndpoint)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _messageActivityRepository = messageActivityRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<Unit>> Handle(MarkConversationAsSeenCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationRepository.SingleOrDefaultAsync(x => x.Id == request.ConversationId);

            if (conversation == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var authorizationRequirment = new IsParticipantInConversationRequirment
            {
                ConversationId = request.ConversationId,
                UserId = request.UserId
            };

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(authorizationRequirment);

            if (authorizationResult.IsFailure)
            {
                return new Result<Unit>(authorizationResult.Exception!);
            }

            var query = from message in _messageRepository.AsQuerable().Where(x => x.ConversationId == request.ConversationId && x.SenderId != request.UserId)
                        join messageActivity in _messageActivityRepository.AsQuerable()
                        on message.Id equals messageActivity.Id into joined
                        from activity in joined.DefaultIfEmpty()
                        select new { Message = message, Activity = activity };


            var result = from pr in query
                                where pr.Activity == null
                                select pr.Message;


            var unReadedMessages = await result.ToListAsync();

            int counter = 0;

            var seenAt = DateTime.UtcNow;

            while(counter < unReadedMessages.Count)
            {
                var batch = unReadedMessages.Skip(counter).Take(BATCH_SIZE).ToList();

                List<MessageActivity> messageActivities = new List<MessageActivity>();

                foreach (var item in batch)
                {
                    var activity = new MessageActivity
                    {
                        MessageId = item.Id,
                        SeenById = request.UserId,
                        SeenAt = seenAt
                    };

                    messageActivities.Add(activity);
                }

                await _messageActivityRepository.InsertManyAsync(messageActivities);

                var @event = new LogMessagesActivitiesEvent
                {
                    Activities = messageActivities,
                    SeenById = request.UserId
                };

                await _publishEndpoint.Publish(@event);

                counter += BATCH_SIZE;
            }

            return Unit.Value;
        }
    }
}
