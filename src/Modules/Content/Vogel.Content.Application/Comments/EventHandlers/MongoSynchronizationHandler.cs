using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Comments;
namespace Vogel.Content.Application.Comments.EventHandlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Comment>>,
        INotificationHandler<EntityUpdatedEvent<Comment>>,
        INotificationHandler<EntityDeletedEvent<Comment>>
    {
        private readonly IMapper _mapper;

        private readonly CommentMongoRepository _commentMongoRepository;

        public MongoSynchronizationHandler(IMapper mapper, CommentMongoRepository commentMongoRepository)
        {
            _mapper = mapper;
            _commentMongoRepository = commentMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<Comment> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Comment, CommentMongoEntity>(notification.Entity);

            await _commentMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Comment> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Comment, CommentMongoEntity>(notification.Entity);

            await _commentMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<Comment> notification, CancellationToken cancellationToken)
        {
            await _commentMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}
