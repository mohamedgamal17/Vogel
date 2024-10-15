using MassTransit;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Users;
using Vogel.Social.Shared.Events;

namespace Vogel.Messanger.Application.Users.Consumers
{
    public class UserUpdatedIntegrationEventConsumer : IConsumer<UserUpdatedIntegrationEvent>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;

        public UserUpdatedIntegrationEventConsumer(IMongoRepository<UserMongoEntity> userMongoRepository)
        {
            _userMongoRepository = userMongoRepository;
        }

        public async Task Consume(ConsumeContext<UserUpdatedIntegrationEvent> context)
        {
            var oldEntity =  await _userMongoRepository.FindByIdAsync(context.Message.Id);

            var mongoEntity = new UserMongoEntity
            {
                Id = context.Message.Id,
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                AvatarId = context.Message.AvatarId,
                BirthDate = context.Message.BirthDate,
                Gender = context.Message.Gender,
                CreationTime = oldEntity!.CreationTime,
                ModificationTime = DateTime.UtcNow
            };

            if (context.Message.Avatar != null)
            {
                mongoEntity.Avatar = new Avatar
                {
                    Id = context.Message.Avatar.Id,
                    File = context.Message.Avatar.File,
                    UserId = context.Message.Avatar.UserId
                };
            }

            await _userMongoRepository.UpdateAsync(mongoEntity);
        }
    }
}
