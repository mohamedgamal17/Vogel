namespace Vogel.BuildingBlocks.Domain.Events
{
    public class EntityCreatedEvent<TEntity> : IEvent
    {
        public TEntity Entity { get; set; }

        public EntityCreatedEvent(TEntity entity)
        {
            Entity = entity;
        }

        public EntityCreatedEvent()
        {
            
        }
    }

}
