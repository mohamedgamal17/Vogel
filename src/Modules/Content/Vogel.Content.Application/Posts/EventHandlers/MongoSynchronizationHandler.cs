using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Posts.EventHandlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Post>>,
        INotificationHandler<EntityUpdatedEvent<Post>>,
        INotificationHandler<EntityDeletedEvent<Post>>
    {
        private readonly IMapper _mapper;

        private readonly PostMongoRepository _postMongoRepository;

        public MongoSynchronizationHandler(IMapper mapper, PostMongoRepository postMongoRepository)
        {
            _mapper = mapper;
            _postMongoRepository = postMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<Post> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Post, PostMongoEntity>(notification.Entity);

            await _postMongoRepository.InsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Post> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Post, PostMongoEntity>(notification.Entity);

            await _postMongoRepository.UpdateAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<Post> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Post, PostMongoEntity>(notification.Entity);

            await _postMongoRepository.DeleteAsync(mongoEntity);
        }
    }
}
