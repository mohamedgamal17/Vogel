using MediatR;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Conversations;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.Presentation.Hubs.Chat.Handlers
{
    public class MessageSentEventConsumer : INotificationHandler<EntityCreatedEvent<Message>>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;
        private readonly IMongoRepository<ParticipantMongoEntity> _participantMongoRepository;
        private readonly IMessageResponseFactory _messageResponseFactory;
        private readonly IHubContext<Chathub, IChatHub> _hubContext;

        public MessageSentEventConsumer(IMongoRepository<UserMongoEntity> userMongoRepository, IMongoRepository<ParticipantMongoEntity> participantMongoRepository, IMessageResponseFactory messageResponseFactory, IHubContext<Chathub, IChatHub> hubContext)
        {
            _userMongoRepository = userMongoRepository;
            _participantMongoRepository = participantMongoRepository;
            _messageResponseFactory = messageResponseFactory;
            _hubContext = hubContext;
        }

        public async Task Handle(EntityCreatedEvent<Message> notification, CancellationToken cancellationToken)
        {
            var conversationId = notification.Entity.ConversationId;

            var sender = await _userMongoRepository.FindByIdAsync(notification.Entity.SenderId);

            var dto = await _messageResponseFactory.PrepareMessageDto(notification.Entity, sender);

            using var cursor = await _participantMongoRepository.AsMongoCollection()
                . Find(
                        Builders<ParticipantMongoEntity>.Filter.And(
                            Builders<ParticipantMongoEntity>.Filter.Eq(x=> x.ConversationId, conversationId),
                            Builders<ParticipantMongoEntity>.Filter.Not(Builders<ParticipantMongoEntity>.Filter.Eq(x=> x.Id , notification.Entity.SenderId))
                        )
                   )            
                .ToCursorAsync();

            while(await cursor.MoveNextAsync())
            {
                await _hubContext.Clients.Users(cursor.Current.Select(x => x.Id)).ReciveMessage(dto);
            }

        }


    }
}
