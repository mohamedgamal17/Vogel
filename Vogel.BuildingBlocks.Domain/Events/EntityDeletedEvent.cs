namespace Vogel.BuildingBlocks.Domain.Events
{
    public class EntityDeletedEvent<TEntity> : IEvent
    {
        public TEntity Entity { get; set; }

        public EntityDeletedEvent(TEntity entity)
        {
            Entity = entity;
        }

        public EntityDeletedEvent()
        {
            
        }
    }

}
