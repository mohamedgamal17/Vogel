using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.Application.PostReactions.EventHandlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<PostReaction>>,
        INotificationHandler<EntityUpdatedEvent<PostReaction>>,
        INotificationHandler<EntityDeletedEvent<PostReaction>>
    {

        private readonly IMapper _mapper;
        private readonly ReactionMongoRepository _reactionMongoRepository;

        public MongoSynchronizationHandler(IMapper mapper, ReactionMongoRepository reactionMongoRepository)
        {
            _mapper = mapper;
            _reactionMongoRepository = reactionMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<PostReaction> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<PostReaction, ReactionMongoEntity>(notification.Entity);

            await _reactionMongoRepository.InsertAsync(mongoEntity);

        }

        public async Task Handle(EntityUpdatedEvent<PostReaction> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<PostReaction, ReactionMongoEntity>(notification.Entity);

            await _reactionMongoRepository.UpdateAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<PostReaction> notification, CancellationToken cancellationToken)
        {
            await _reactionMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}
