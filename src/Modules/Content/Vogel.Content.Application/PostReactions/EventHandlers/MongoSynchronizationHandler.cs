﻿using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.PostReactions.EventHandlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<PostReaction>>,
        INotificationHandler<EntityUpdatedEvent<PostReaction>>,
        INotificationHandler<EntityDeletedEvent<PostReaction>>
    {

        private readonly IMapper _mapper;
        private readonly IMongoRepository<PostReactionMongoEntity> _reactionMongoRepository;

        public MongoSynchronizationHandler(IMapper mapper, IMongoRepository<PostReactionMongoEntity> reactionMongoRepository)
        {
            _mapper = mapper;
            _reactionMongoRepository = reactionMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<PostReaction> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<PostReaction, PostReactionMongoEntity>(notification.Entity);

            await _reactionMongoRepository.ReplaceOrInsertAsync(mongoEntity);

        }

        public async Task Handle(EntityUpdatedEvent<PostReaction> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<PostReaction, PostReactionMongoEntity>(notification.Entity);

            await _reactionMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<PostReaction> notification, CancellationToken cancellationToken)
        {
            await _reactionMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}
