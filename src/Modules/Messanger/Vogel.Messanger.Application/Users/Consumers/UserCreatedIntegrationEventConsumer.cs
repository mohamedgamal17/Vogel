using MassTransit;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Users;
using Vogel.Social.Shared.Events;

namespace Vogel.Messanger.Application.Users.Consumers
{
    public class UserCreatedIntegrationEventConsumer : IConsumer<UserCreatedIntegrationEvent>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;

        public UserCreatedIntegrationEventConsumer(IMongoRepository<UserMongoEntity> userMongoRepository)
        {
            _userMongoRepository = userMongoRepository;
        }

        public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
        {
            var mongoEntity = new UserMongoEntity
            {
                Id = context.Message.Id,
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                AvatarId = context.Message.AvatarId,
                BirthDate = context.Message.BirthDate,
                Gender = context.Message.Gender,
                CreationTime = DateTime.UtcNow,
                ModificationTime = DateTime.UtcNow
            };

            await _userMongoRepository.InsertAsync(mongoEntity);
        }
    }
}
