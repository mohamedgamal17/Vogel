using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.CommentReactions;


namespace Vogel.Content.Application.CommentReactions.EventHandlers
{
    public class CommentReactionSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<CommentReaction>>,
        INotificationHandler<EntityUpdatedEvent<CommentReaction>>,
        INotificationHandler<EntityDeletedEvent<CommentReaction>>
    {
        private readonly IMapper _mapper;
        private readonly IMongoRepository<CommentReactionMongoEntity> _commentReactionMongoRepository;

        public CommentReactionSynchronizationHandler(IMapper mapper, CommentReactionMongoRepository commentReactionMongoRepository)
        {
            _mapper = mapper;
            _commentReactionMongoRepository = commentReactionMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<CommentReaction> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<CommentReaction, CommentReactionMongoEntity>(notification.Entity);

            await _commentReactionMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<CommentReaction> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<CommentReaction, CommentReactionMongoEntity>(notification.Entity);

            await _commentReactionMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<CommentReaction> notification, CancellationToken cancellationToken)
        {
            await _commentReactionMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}
